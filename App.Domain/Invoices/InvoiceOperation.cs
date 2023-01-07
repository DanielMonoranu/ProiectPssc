using App.Domain.Invoices.Models;
using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Domain.Invoices.Models.Invoices;
using static LanguageExt.Prelude;


namespace App.Domain.Invoices
{

    public static class InvoiceOperation
    {
        public static Task<IInvoices> ValidateInvoices(Func<InvoiceNumber, Option<InvoiceNumber>> checkInvoiceExists, UnvalidatedInvoices invoices) =>
            invoices.InvoiceList
                .Select(ValidateInvoiceNumber(checkInvoiceExists))
                .Aggregate(CreateEmptyInvoiceList().ToAsync(), ReduceValidinvoices)
                .MatchAsync(
                    LeftAsync: errorMessage => Task.FromResult((IInvoices)new InvalidatedInvoices(invoices.InvoiceList, errorMessage)),
                    Right: validatedinvoices => new ValidatedInvoices(validatedinvoices));

        private static Func<UnvalidatedInvoice,EitherAsync<string, ValidatedInvoice>> ValidateInvoiceNumber(Func<InvoiceNumber, Option<InvoiceNumber>> checkInvoiceExists) =>
        unvalidatedInvoice => ValidateInvoice(checkInvoiceExists, unvalidatedInvoice);

        private static EitherAsync<string, ValidatedInvoice> ValidateInvoice(Func<InvoiceNumber, Option<InvoiceNumber>> checkInvoiceExists, UnvalidatedInvoice unvalidatedInvoice) =>
        from invoiceEntryInfo in Invoice.TryParseInvoice(unvalidatedInvoice.Status, unvalidatedInvoice.Vat)
                                                .ToEitherAsync(() => $"Invalid invoice information({unvalidatedInvoice.InvoiceNumber},{unvalidatedInvoice.Status},{unvalidatedInvoice.Vat})")
        from InvoiceNumber in InvoiceNumber.TryParse(unvalidatedInvoice.InvoiceNumber)
                                             .ToEitherAsync(() => $"Invalid invoice number({unvalidatedInvoice.InvoiceNumber})")
        from invoiceExists in checkInvoiceExists(InvoiceNumber)
                            .ToEitherAsync($"Delivery {InvoiceNumber.Value} does not exist.")
        select new ValidatedInvoice(InvoiceNumber, invoiceEntryInfo);


        private static Either<string, List<ValidatedInvoice>> CreateEmptyInvoiceList() => Right(new List<ValidatedInvoice>());

        private static EitherAsync<string, List<ValidatedInvoice>> ReduceValidinvoices(EitherAsync<string, List<ValidatedInvoice>> acc, EitherAsync<string, ValidatedInvoice> next) =>
            from list in acc
            from nextInvoice in next
            select list.AppendDelivery(nextInvoice);

        private static List<ValidatedInvoice> AppendDelivery(this List<ValidatedInvoice> list, ValidatedInvoice validDelivery)
        {
            list.Add(validDelivery);
            return list;
        }

        public static IInvoices CancelInvoices(IInvoices invoices) =>
            invoices.Match(
                whenUnvalidatedInvoices: unvalidinvoices => unvalidinvoices,
                whenCancelledInvoices: cancelledinvoices => cancelledinvoices,
                whenFailedInvoices: failedinvoices => failedinvoices,
                whenAnnouncedInvoices: announcedinvoices => announcedinvoices,
                whenInvalidatedInvoices: invalidatedinvoices => invalidatedinvoices,
                whenValidatedInvoices: CancelInvoices );


        private static IInvoices CancelInvoices(ValidatedInvoices validinvoices) =>
            new CancelledInvoices(validinvoices.InvoiceList
                                                    .Select(CancelDelivery)
                                                     .ToList()
                                                      .AsReadOnly());


        private static CancelledInvoice CancelDelivery(ValidatedInvoice validDelivery)
        {
            return new CancelledInvoice(validDelivery.InvoiceNumber, validDelivery.InvoiceEntry);
        }



        public static IInvoices MergeInvoices(IInvoices invoices, IEnumerable<CancelledInvoice> existinginvoices) =>
           invoices.Match(
                whenUnvalidatedInvoices: unvalidinvoices => unvalidinvoices,
                whenFailedInvoices: failedinvoices => failedinvoices,
                whenAnnouncedInvoices: announcedinvoices => announcedinvoices,
                whenInvalidatedInvoices: invalidatedinvoices => invalidatedinvoices,
                whenValidatedInvoices: validainvoices => validainvoices,
                whenCancelledInvoices: cancelledinvoices => MergeNewInvoices(cancelledinvoices.InvoiceList, existinginvoices));
         

        private static CancelledInvoices MergeNewInvoices(IEnumerable<CancelledInvoice> newList, IEnumerable<CancelledInvoice> existingList)
        {
            var updatedAndNewinvoices = newList.Select(invoice => invoice with { EntryId = existingList.FirstOrDefault(e => e.InvoiceNumber == invoice.InvoiceNumber)?.EntryId ?? 0, IsUpdated = true });
            var oldinvoices = existingList.Where(entry => !newList.Any(e => e.InvoiceNumber == entry.InvoiceNumber));
            var allinvoices = updatedAndNewinvoices.Union(oldinvoices).ToList().AsReadOnly();

            return new CancelledInvoices(allinvoices);

        }
        public static IInvoices AnnounceInvoices(IInvoices invoices) =>
             invoices.Match(
                whenUnvalidatedInvoices: unvalidinvoices => unvalidinvoices,
                whenFailedInvoices: failedinvoices => failedinvoices,
                whenAnnouncedInvoices: announcedinvoices => announcedinvoices,
                whenInvalidatedInvoices: invalidatedinvoices => invalidatedinvoices,
                whenValidatedInvoices: validainvoices => validainvoices,
                whenCancelledInvoices: GenerateExport);




        private static IInvoices GenerateExport(  CancelledInvoices cancelledinvoices) =>
           new AnnouncedInvoices(cancelledinvoices.InvoiceList,
               cancelledinvoices.InvoiceList.Aggregate(new StringBuilder(), CreateMessage).ToString(),
               DateTime.Now);


        private static StringBuilder CreateMessage(StringBuilder export, CancelledInvoice invoice) =>
            export.AppendLine($"Inovice Number:{invoice.InvoiceNumber.Value}, status: {invoice.InvoiceEntry.InvoiceStatus}, VAT:{invoice.InvoiceEntry.VAT}");


    }
}
