using App.Domain.Invoices.Models;
using App.Domain.Invoices.Repositories;
using LanguageExt;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Domain.Invoices.Models.CancelInvoiceEvent;
using static App.Domain.Invoices.Models.Invoices;
using static LanguageExt.Prelude;


namespace App.Domain.Invoices
{
    public class CancelInvoiceWorkflow
    {
        private readonly IInvoicesRepository _invoicesRepository;
        private readonly IInvoiceEntriesRepository _entriesRepository;
        private readonly ILogger<CancelInvoiceWorkflow> _logger;

        public CancelInvoiceWorkflow(IInvoicesRepository invoicesRepository, IInvoiceEntriesRepository entriesRepository, ILogger<CancelInvoiceWorkflow> logger)
        {
            _invoicesRepository = invoicesRepository;
            _entriesRepository = entriesRepository;
            _logger = logger;
        }



        public async Task<  ICancelInvoiceEvent> ExecuteAsync(CancelInvoiceCommand command)
        {
             
            UnvalidatedInvoices unvalidatedInvoices = new UnvalidatedInvoices(command.InputInvoices);

            var result = from invoicesw in _invoicesRepository.TryGetExistingInvoice(unvalidatedInvoices.InvoiceList.Select(entry => entry.InvoiceNumber))
                                          .ToEither(ex => new FailedInvoices(unvalidatedInvoices.InvoiceList, ex) as IInvoices)
                         from existingEntries in _entriesRepository.TryGetExistingInvoice()
                                              .ToEither(ex => new FailedInvoices(unvalidatedInvoices.InvoiceList, ex) as IInvoices)
                         let checkInvoiceExists = (Func<InvoiceNumber, Option<InvoiceNumber>>)(invoice => CheckInvoiceExists(invoicesw, invoice))
                         from announcedInvoices in ExecuteWorkFlowAsync(unvalidatedInvoices, existingEntries, checkInvoiceExists).ToAsync()
                         from _ in _entriesRepository.TrySaveInvoice(announcedInvoices)
                                                    .ToEither(ex => new FailedInvoices(unvalidatedInvoices.InvoiceList, ex) as IInvoices)
                         select announcedInvoices;


            return await result.Match(
                Left: failedInvoices => GenerateFailedEvent(failedInvoices) as ICancelInvoiceEvent,
                Right: cancelledDeliveries => new CancelInvoiceSucceededEvent(cancelledDeliveries.Message, cancelledDeliveries.CancelDate));


        }


        private async Task<Either<IInvoices, AnnouncedInvoices>> ExecuteWorkFlowAsync(UnvalidatedInvoices unvalidatedInvoices, IEnumerable<CancelledInvoice> existingEntries, Func<InvoiceNumber, Option<InvoiceNumber>> checkInvoiceExists)
        {
            IInvoices invoices=  await Domain.Invoices.InvoiceOperation.ValidateInvoices(checkInvoiceExists, unvalidatedInvoices);
            invoices = Domain.Invoices.InvoiceOperation.CancelInvoices(invoices);
            invoices = Domain.Invoices.InvoiceOperation.MergeInvoices(invoices, existingEntries);
            invoices = Domain.Invoices.InvoiceOperation.AnnounceInvoices(invoices);

            return invoices.Match<Either<IInvoices, AnnouncedInvoices>>(
                whenUnvalidatedInvoices: unvalidatedInvoices => Left(unvalidatedInvoices as IInvoices),
                whenInvalidatedInvoices: invalidatedInvoices => Left(invalidatedInvoices as IInvoices),
                whenFailedInvoices: failedInvoices => Left(failedInvoices as IInvoices),
                whenValidatedInvoices: validatedInvoices => Left(validatedInvoices as IInvoices),
                whenCancelledInvoices: cancelledInvoices => Left(cancelledInvoices as IInvoices),
                whenAnnouncedInvoices: announcedInvoices => Right(announcedInvoices ));
                 
              
        }



        private Option<InvoiceNumber> CheckInvoiceExists(IEnumerable<InvoiceNumber> invoices, InvoiceNumber invoiceNumber)
        {
            if (invoices.Any(e => e == invoiceNumber))
            {
                return Some(invoiceNumber);
            }
            else
            {
                return None;
            }
        }

        private CancelInvoiceFailedEvent GenerateFailedEvent(IInvoices failedInvoices) =>
          failedInvoices.Match<CancelInvoiceFailedEvent>(

              whenUnvalidatedInvoices: unvalidatedInvoices => new($"Invalid state{nameof(unvalidatedInvoices)}"),
              whenInvalidatedInvoices: invalidatedInvoices => new(invalidatedInvoices.Reason),
              whenValidatedInvoices: valdiatedInvoices => new($"Invalid state{nameof(ValidatedInvoices)}"),
              whenFailedInvoices: failedInvoices =>
              {
                  _logger.LogError(failedInvoices.Exception, failedInvoices.Exception.Message);
                  return new(failedInvoices.Exception.Message);
              }
              ,
              whenCancelledInvoices: cancelledInvoices => new($"Invalid state{nameof(CancelledInvoices)}"),
              whenAnnouncedInvoices: announcedInvoices => new($"Invalid state{nameof(AnnouncedInvoices)}"));
    }
}
