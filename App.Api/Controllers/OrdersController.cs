using App.Domain;
using App.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System;
using App.Api.Models;
using App.Domain.Models;

namespace App.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrdersController : ControllerBase
    {
        public ILogger<OrdersController> _logger;
        public OrdersController(ILogger<OrdersController> logger) {
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrders([FromServices] IOrderRepository orderRepository) =>
            await orderRepository.TryGetExistingGrades().Match(
                Succ: GetAllGradesHandleSuccess,
                Fail: GetAllGradesHandleError
                );

        private ObjectResult GetAllGradesHandleError(Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return base.StatusCode(StatusCodes.Status500InternalServerError, "UnexpectedError");
        }

        private OkObjectResult GetAllGradesHandleSuccess(List<App.Domain.Models.CalculatedOrder> grades) =>
                Ok(grades.Select(grade => new
                {
                    OrderRegistrationNumber = grade.StudentRegistrationNumber.Value,
                    grade.OrderId,
                    grade.Order,
                    grade.FinalOrder
                }));

        [HttpPost]
        public async Task<IActionResult> PublishGrades([FromServices] CancelOrderWorkflow CancelGradeWorkflow, [FromBody] InputOrder[] grades)
        {
            var unvalidatedGrades = grades.Select(MapInputGradeToUnvalidatedGrade)
                                          .ToList()
                                          .AsReadOnly();
            ShipOrderCommand command = new(unvalidatedGrades);
            var result = await CancelGradeWorkflow.ExecuteAsync(command);
            return result.Match<IActionResult>(
                whenShopOrderShippedFaildEvent: failedEvent => StatusCode(StatusCodes.Status500InternalServerError, failedEvent.Reason),
                whenShopOrderShippedScucceededEvent: successEvent => Ok()
            );
        }

        private static UnvalidatedOrder MapInputGradeToUnvalidatedGrade(InputOrder grade) => new UnvalidatedOrder(
                    OrderRegistrationNumber: grade.OrderRegistrationNumberInput,
                    Grade: "" + grade.OrderValue.ToString() + " " + grade.IsCancelled.ToString() + " " + grade.OrderId.ToString())
        { OrderId = grade.OrderId};
    }
}
