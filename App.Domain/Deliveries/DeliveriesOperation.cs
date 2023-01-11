using App.Domain.Deliveries.Models;
using static App.Domain.Deliveries.Models.Deliveries;
using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LanguageExt.Prelude;

namespace App.Domain.Deliveries
{
    public static class DeliveriesOperation
    {
        public static Task<IDeliveries> ValidateDeliveries(Func<DeliveryNumber, Option<DeliveryNumber>> checkDeliveryExists, UnvalidatedDeliveries deliveries) =>
            deliveries.DeliveryList
                .Select(ValidateDeliveryNumber(checkDeliveryExists))
                .Aggregate(CreateEmptyDeliveryList().ToAsync(), ReduceValidDeliveries)
                .MatchAsync(
                    LeftAsync: errorMessage => Task.FromResult((IDeliveries)new InvalidatedDeliveries(deliveries.DeliveryList, errorMessage)),
                    Right: validatedDeliveries => new ValidatedDeliveries(validatedDeliveries));

        private static Func<UnvalidatedDelivery, EitherAsync<string, ValidatedDelivery>> ValidateDeliveryNumber(Func<DeliveryNumber, Option<DeliveryNumber>> checkDeliveryExists) =>
        unvalidatedDelivery => ValidateDelivery(checkDeliveryExists, unvalidatedDelivery);

        private static EitherAsync<string, ValidatedDelivery> ValidateDelivery(Func<DeliveryNumber, Option<DeliveryNumber>> checkDeliveryExists, UnvalidatedDelivery unvalidatedDelivery) =>
        from deliveryEntryInfo in DeliveryEntry.TryParseStatus(unvalidatedDelivery.Status, unvalidatedDelivery.orderId)
                                                .ToEitherAsync(() => $"Invalid delivery entry information({unvalidatedDelivery.DeliveryNumber},{unvalidatedDelivery.Status},{unvalidatedDelivery.orderId})")
        from deliveryNumber in DeliveryNumber.TryParse(unvalidatedDelivery.DeliveryNumber)
                                             .ToEitherAsync(() => $"Invalid delivery number({unvalidatedDelivery.DeliveryNumber})")
        from deliveryExists in checkDeliveryExists(deliveryNumber)
                            .ToEitherAsync($"Delivery {deliveryNumber.Value} does not exist.")
        select new ValidatedDelivery(deliveryNumber, deliveryEntryInfo);


        private static Either<string, List<ValidatedDelivery>> CreateEmptyDeliveryList() => Right(new List<ValidatedDelivery>());

        private static EitherAsync<string, List<ValidatedDelivery>> ReduceValidDeliveries(EitherAsync<string, List<ValidatedDelivery>> acc, EitherAsync<string, ValidatedDelivery> next) =>
            from list in acc
            from nextDelivery in next
            select list.AppendDelivery(nextDelivery);

        private static List<ValidatedDelivery> AppendDelivery(this List<ValidatedDelivery> list, ValidatedDelivery validDelivery)
        {
            list.Add(validDelivery);
            return list;
        }

        public static IDeliveries CancelDeliveries(IDeliveries deliveries) =>
            deliveries.Match(
                whenUnvalidatedDeliveries: unvalidDeliveries => unvalidDeliveries,
                whenInvalidatedDeliveries: invalidDeliveries => invalidDeliveries,
                whenCancelledDeliveries: cancelledDeliveries => cancelledDeliveries,
                whenFailedDeliveries: failedDelivery => failedDelivery,
                whenAnnouncedDeliveries: announcedDeliveries => announcedDeliveries,
                whenValidatedDeliveries: CancelDeliveries);

        private static IDeliveries CancelDeliveries(ValidatedDeliveries validDeliveries) =>
            new CancelledDeliveries(validDeliveries.DeliveryList
                                                    .Select(CancelDelivery)
                                                     .ToList()
                                                      .AsReadOnly());


        private static CancelledDelivery CancelDelivery(ValidatedDelivery validDelivery)
        {
            //  validDelivery.DeliveryEntry.Status = 0;  ??
            return new CancelledDelivery(validDelivery.DeliveryNumber, validDelivery.DeliveryEntry);
        }



        public static IDeliveries MergeDeliveries(IDeliveries deliveries, IEnumerable<CancelledDelivery> existingDeliveries) =>
           deliveries.Match(
               whenUnvalidatedDeliveries: unvalidDeliveries => unvalidDeliveries,
               whenInvalidatedDeliveries: invalidDeliveries => invalidDeliveries,
               whenValidatedDeliveries: validaDeliveries => validaDeliveries,
               whenFailedDeliveries: failedDelivery => failedDelivery,
               whenAnnouncedDeliveries: announcedDeliveries => announcedDeliveries,
              whenCancelledDeliveries: cancelledDeliveries => MergeNewDeliveries(cancelledDeliveries.DeliveryList, existingDeliveries));

        private static CancelledDeliveries MergeNewDeliveries(IEnumerable<CancelledDelivery> newList, IEnumerable<CancelledDelivery> existingList)
        {
            var updatedAndNewDeliveries = newList.Select(delivery => delivery with { EntryId = existingList.FirstOrDefault(e => e.DeliveryNumber == delivery.DeliveryNumber)?.EntryId ?? 0, IsUpdated = true });
            var oldDeliveries = existingList.Where(entry => !newList.Any(e => e.DeliveryNumber == entry.DeliveryNumber));
            var allDeliveries = updatedAndNewDeliveries.Union(oldDeliveries).ToList().AsReadOnly();

            return new CancelledDeliveries(allDeliveries);

        }
        public static IDeliveries AnnounceDeliveries(IDeliveries deliveries) => deliveries.Match(
            whenUnvalidatedDeliveries: unvalidDeliveries => unvalidDeliveries,
               whenInvalidatedDeliveries: invalidDeliveries => invalidDeliveries,
               whenValidatedDeliveries: validaDeliveries => validaDeliveries,
               whenFailedDeliveries: failedDelivery => failedDelivery,
               whenAnnouncedDeliveries: announcedDeliveries => announcedDeliveries,
              whenCancelledDeliveries: GenerateExport);
        private static IDeliveries GenerateExport(CancelledDeliveries cancelledDeliveries) =>
           new AnnouncedDeliveries(cancelledDeliveries.DeliveryList,
               cancelledDeliveries.DeliveryList.Aggregate(new StringBuilder(), CreateMessage).ToString(),
               DateTime.Now);


        private static StringBuilder CreateMessage(StringBuilder export, CancelledDelivery delivery) =>
            export.AppendLine($"Delivery Number:{delivery.DeliveryNumber.Value}, status: {delivery.DeliveryEntry.Status}, orderId:{delivery.DeliveryEntry.OrderId}");


    }
}
