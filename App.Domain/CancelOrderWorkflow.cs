using App.Domain.Models;
using App.Domain.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Domain.Models.ShopOrder;
using static App.Domain.Models.ShopOrderShippedEvent;
using static App.Domain.ShopOrderOperation;
using static LanguageExt.Prelude;
using LanguageExt;
namespace App.Domain
{
    public class CancelOrderWorkflow
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderRegistrationNumberRepository _registrationNumberRepository;
        private readonly ILogger _logger;

        public CancelOrderWorkflow(IOrderRepository orderRepository, IOrderRegistrationNumberRepository orderRegistrationNumberRepository, ILogger<CancelOrderWorkflow> logger) {
            this._logger = logger;
            this._orderRepository = orderRepository;
            this._registrationNumberRepository = orderRegistrationNumberRepository;
        }

        public async Task<IShopOrdersShippedEvent> ExecuteAsync(ShipOrderCommand command)
        {
            UnvalidatedShopOrders unvalidatedOrders = new UnvalidatedShopOrders(command.InputExamGrades);


            var result = from orderRegistrationNumbers in _registrationNumberRepository.TryGetRegistrationNumbers().ToEither(ex => new FailedShopOrders(unvalidatedOrders.GradeList, ex) as IShopOrder)
                         from existingOrders in _orderRepository.TryGetExistingGrades().ToEither(ex => new FailedShopOrders(unvalidatedOrders.GradeList, ex) as IShopOrder)
                         let checkOrderExists = (Func<OrderRegistrationNumber, Option<OrderRegistrationNumber>>)(orderRegistrationNumber => CheckOrderExists(orderRegistrationNumbers, orderRegistrationNumber))
                         from cancelledOrders in ExecuteWorkflowAsync(unvalidatedOrders, existingOrders, checkOrderExists).ToAsync()
                         from _ in _orderRepository.TrySaveCancelledOrders(cancelledOrders).ToEither(ex => new FailedShopOrders(unvalidatedOrders.GradeList, ex) as IShopOrder)
                         select cancelledOrders;
            return await result.Match(
                    Left: examGrades => GenerateFailedEvent(examGrades) as IShopOrdersShippedEvent,
                    Right: cancelledGrades => new ShopOrderShippedScucceededEvent(cancelledGrades.Csv, cancelledGrades.CancellationDate)
                );

        }

        private Option<OrderRegistrationNumber> CheckOrderExists(IEnumerable<OrderRegistrationNumber> students, OrderRegistrationNumber studentRegistrationNumber)
        {
            if (students.Any(s => s == studentRegistrationNumber))
            {
                return Some(studentRegistrationNumber);
            }
            else
            {
                return None;
            }
        }

        private async Task<Either<IShopOrder, CancelledShopOrder>> ExecuteWorkflowAsync(UnvalidatedShopOrders unvalidatedGrades,
                                                                                  IEnumerable<CalculatedOrder> existingGrades,
                                                                                  Func<OrderRegistrationNumber, Option<OrderRegistrationNumber>> checkStudentExists)
        {

            IShopOrder grades = await ValidateShopOrders(checkStudentExists, unvalidatedGrades);

            grades = CalculateFinalGrades(grades);
            grades = PublishExamGrades(grades);

            return grades.Match<Either<IShopOrder, CancelledShopOrder>>(
                whenUnvalidatedShopOrders: unvalidatedGrades => Left(unvalidatedGrades as IShopOrder),
                whenCalculatedShopOrders: calculatedGrades => Left(calculatedGrades as IShopOrder),
                whenInvalidatedShopOrders: invalidGrades => Left(invalidGrades as IShopOrder),
                whenFailedShopOrders: failedGrades => Left(failedGrades as IShopOrder),
                whenValidatedShopOrders: validatedGrades => Left(validatedGrades as IShopOrder),
                whenCancelledShopOrder: publishedGrades => Right(publishedGrades)
            );
        }

        private ShopOrderShippedFaildEvent GenerateFailedEvent(IShopOrder examGrades) =>
            examGrades.Match<ShopOrderShippedFaildEvent>(
                whenUnvalidatedShopOrders: unvalidatedExamGrades => new($"Invalid state {nameof(UnvalidatedShopOrders)}"),
                whenInvalidatedShopOrders: invalidExamGrades => new(invalidExamGrades.Reason),
                whenValidatedShopOrders: validatedExamGrades => new($"Invalid state {nameof(ValidatedShopOrders)}"),
                whenFailedShopOrders: failedExamGrades =>
                {
                    _logger.LogError(failedExamGrades.Exception, failedExamGrades.Exception.Message);
                    return new(failedExamGrades.Exception.Message);
                },
                whenCalculatedShopOrders: calculatedExamGrades => new($"Invalid state {nameof(CalculatedShopOrders)}"),
                whenCancelledShopOrder: publishedExamGrades => new($"Invalid state {nameof(publishedExamGrades)}"));

    }
}
