using App.Domain.Deliveries.Models;
using App.Domain.Deliveries.Repositories;
using LanguageExt;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Domain.Deliveries.Models.CancelDeliveryEvent;
using static App.Domain.Deliveries.Models.Deliveries;
using static LanguageExt.Prelude;
using static App.Domain.Deliveries.DeliveriesOperation;

namespace App.Domain.Deliveries
{
    public class CancelDeliveryWorkflow
    {
        private readonly IDeliveriesRepository _deliveriesRepository;
        private readonly IEntriesRepository _entriesRepository;
        private readonly ILogger<CancelDeliveryWorkflow> _logger;
        public CancelDeliveryWorkflow(IDeliveriesRepository deliveriesRepository, IEntriesRepository entriesRepository, ILogger<CancelDeliveryWorkflow> logger)
        {
            _deliveriesRepository = deliveriesRepository;
            _entriesRepository = entriesRepository;
            _logger = logger;
        }



        public async Task<ICancelDeliveryEvent> ExecuteAsync(CancelDeliveryCommand command)
        {
            UnvalidatedDeliveries unvalidatedDeliveries = new UnvalidatedDeliveries(command.InputDeliveries);

            var result = from deliveriesw in _deliveriesRepository.TryGetExistingDelivery(unvalidatedDeliveries.DeliveryList.Select(entry => entry.DeliveryNumber))
                                          .ToEither(ex => new FailedDeliveries(unvalidatedDeliveries.DeliveryList, ex) as IDeliveries)
                         from existingEntries in _entriesRepository.TryGetExistingDelivery()
                                              .ToEither(ex => new FailedDeliveries(unvalidatedDeliveries.DeliveryList, ex) as IDeliveries)
                         let checkDeliveryExists = (Func<DeliveryNumber, Option<DeliveryNumber>>)(delivery => CheckDeliveryExists(deliveriesw, delivery))
                         from announcedDeliveries in ExecuteWorkFlowAsync(unvalidatedDeliveries, existingEntries, checkDeliveryExists).ToAsync()
                         from _ in _entriesRepository.TrySaveDelivery(announcedDeliveries)
                                                    .ToEither(ex => new FailedDeliveries(unvalidatedDeliveries.DeliveryList, ex) as IDeliveries)
                         select announcedDeliveries;

            return await result.Match(
                Left: failedDeliveries => GenerateFailedEvent(failedDeliveries) as ICancelDeliveryEvent,
                Right: cancelledDeliveries => new CancelDeliverySucceededEvent(cancelledDeliveries.Message, cancelledDeliveries.CancelDate));


        }





        private async Task<Either<IDeliveries, AnnouncedDeliveries>> ExecuteWorkFlowAsync(UnvalidatedDeliveries unvalidatedDeliveries, IEnumerable<CancelledDelivery> existingEntries, Func<DeliveryNumber, Option<DeliveryNumber>> checkDeliveryExists)
        {
            IDeliveries deliveries = await ValidateDeliveries(checkDeliveryExists, unvalidatedDeliveries);
            deliveries = CancelDeliveries(deliveries);
            deliveries = MergeDeliveries(deliveries, existingEntries);
            deliveries = AnnounceDeliveries(deliveries);

            return deliveries.Match<Either<IDeliveries, AnnouncedDeliveries>>(
                whenUnvalidatedDeliveries: unvalidatedDeliveries => Left(unvalidatedDeliveries as IDeliveries),
                whenInvalidatedDeliveries: invalidatedDeliveries => Left(invalidatedDeliveries as IDeliveries),
                whenFailedDeliveries: failedDeliveries => Left(failedDeliveries as IDeliveries),
                whenValidatedDeliveries: validatedDeliveries => Left(validatedDeliveries as IDeliveries),
                whenCancelledDeliveries: cancelledDeliveries => Left(cancelledDeliveries as IDeliveries),
                whenAnnouncedDeliveries: announcedDeliveries => Right(announcedDeliveries)
                );
        }



        private Option<DeliveryNumber> CheckDeliveryExists(IEnumerable<DeliveryNumber> deliveries, DeliveryNumber deliveryNumber)
        {
            if (deliveries.Any(e => e == deliveryNumber))
            {
                return Some(deliveryNumber);
            }
            else
            {
                return None;
            }
        }
        private CancelDeliveryFailedEvent GenerateFailedEvent(IDeliveries failedDeliveries) =>
          failedDeliveries.Match<CancelDeliveryFailedEvent>(
              whenUnvalidatedDeliveries: unvalidatedDeliveries => new($"Invalid state{nameof(UnvalidatedDeliveries)}"),
              whenInvalidatedDeliveries: invalidDeliveries => new(invalidDeliveries.Reason),
              whenValidatedDeliveries: validatedDeliveries => new($"invalid state {nameof(ValidatedDeliveries)}"),
              whenFailedDeliveries: failedDeliveries =>
              {
                  _logger.LogError(failedDeliveries.Exception, failedDeliveries.Exception.Message);
                  return new(failedDeliveries.Exception.Message);
              },
              whenCancelledDeliveries: cancelledDeliveries => new($"Invalid state{nameof(CancelledDeliveries)}"),
              whenAnnouncedDeliveries: announcedDeliveries => new($"Invalid state{nameof(AnnouncedDeliveries)}")
              );
    }
}
