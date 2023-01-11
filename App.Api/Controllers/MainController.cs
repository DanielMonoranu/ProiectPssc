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
using App.Domain.Deliveries.Repositories;
using App.Domain.Deliveries.Models;
using App.Domain.Deliveries;
using App.Domain.Invoices.Repositories;
using App.Domain.Invoices.Models;
using App.Domain.Invoices;

namespace App.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MainController : ControllerBase
    {
        public ILogger<MainController> _logger;
        public MainController(ILogger<MainController> logger) {
            _logger = logger;
        }


        //ORDERS:
         
        [HttpGet("GetOrders")]
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

        [HttpPost("PostOrder")]
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

      
        //DELIVERIES:
        [HttpGet("GetDeliveries")]
        public async Task<IActionResult> GetAllDeliveries([FromServices] IEntriesRepository entriesRepository) =>
            await entriesRepository.TryGetExistingDelivery().Match(
                Succ: GetAllDeliveriesHandleSucces, Fail: GetAllDeliveriesHandleError);
        private ObjectResult GetAllDeliveriesHandleError(Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return base.StatusCode(StatusCodes.Status500InternalServerError, "UnexpectedError");
        }

        private OkObjectResult GetAllDeliveriesHandleSucces(List< CancelledDelivery> deliveries) =>
            Ok(deliveries.Select(delivery => new
            {
                DeliveryNumber = delivery.DeliveryNumber.Value,
                delivery.DeliveryEntry.Status,
                delivery.DeliveryEntry.OrderId
            }));

        [HttpPost("PostDelivery")]
        public async Task<IActionResult> AnnounceDeliveries([FromServices] CancelDeliveryWorkflow cancelDeliveryWorkflow, [FromBody] InputDelivery[] deliveries)
        {


            var unvalidatedDeliveries = deliveries.Select(MapInputToUnvalidate).ToList().AsReadOnly();

            CancelDeliveryCommand command = new(unvalidatedDeliveries);
            var result = await cancelDeliveryWorkflow.ExecuteAsync(command);


            return result.Match<IActionResult>(
               whenCancelDeliveryFailedEvent: failedEvent => StatusCode(StatusCodes.Status500InternalServerError, failedEvent.Reason),
               whenCancelDeliverySucceededEvent: succesEvent => Ok());
        }

        private static UnvalidatedInvoice MapInputToUnvalidate(InputInvoice entry) =>
            new UnvalidatedInvoice(
                InvoiceNumber: entry.InputInvoiceNumber,
                Status: entry.Status,
                Vat: entry.VAT );


        
        //INVOICES:
        [HttpGet("GetInvoices")]
        public async Task<IActionResult> GetAllInvoices([FromServices] IInvoiceEntriesRepository entriesRepository) =>
           await entriesRepository.TryGetExistingInvoice().Match(
               Succ: GetAllInvoicesHandleSucces, Fail: GetAllInvoicesHandleError);
        private ObjectResult GetAllInvoicesHandleError(Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return base.StatusCode(StatusCodes.Status500InternalServerError, "UnexpectedError");
        }

        private OkObjectResult GetAllInvoicesHandleSucces(List<CancelledInvoice> invoices) =>
            Ok(invoices.Select(invoice => new
            {
                InvoiceNumber = invoice.InvoiceNumber.Value,
                invoice.InvoiceEntry.InvoiceStatus,
                invoice.InvoiceEntry.VAT
            }));

        [HttpPost("PostInvoice")]
        public async Task<IActionResult> AnnounceInvoice([FromServices] CancelInvoiceWorkflow cancelInvoiceWorkflow, [FromBody] InputInvoice[] invoices)
        {

            var unvalidatedInvoices = invoices.Select(MapInputToUnvalidate).ToList().AsReadOnly();

            CancelInvoiceCommand command = new(unvalidatedInvoices);
            var result = await cancelInvoiceWorkflow.ExecuteAsync(command);


            return result.Match<IActionResult>(
                whenCancelInvoiceFailedEvent: failedEvent => StatusCode(StatusCodes.Status500InternalServerError, failedEvent.Reason),
                whenCancelInvoiceSucceededEvent: succesEvent => Ok());

        }

        private static UnvalidatedDelivery MapInputToUnvalidate(InputDelivery entry) =>
            new UnvalidatedDelivery(
            DeliveryNumber: entry.InputDeliveryNumber,
            Status: entry.Status,
            orderId: entry.OrderId);

    }

}
