using Domain.Models;
using static LanguageExt.Prelude;
using LanguageExt;
using static Domain.Models.Deliveries;
using System.Text;

namespace Domain
{
    public static class DeliveriesOperation
    {
        public static Task<IDeliveries> ValidateDeliveries(Func<DeliveryNumber, TryAsync<bool>> checkDeliveryExists, UnvalidatedDeliveries deliveries) =>
            deliveries.DeliveryList
                .Select(ValidateDeliveryNumber(checkDeliveryExists))
                .Aggregate(CreateEmptyDeliveryList().ToAsync(), ReduceValidDeliveries)
                .MatchAsync(
                    LeftAsync: errorMessage => Task.FromResult((IDeliveries)new InvalidatedDeliveries(deliveries.DeliveryList, errorMessage)),
                    Right: validatedDeliveries => new ValidatedDeliveries(validatedDeliveries));

        private static Func<UnvalidatedDelivery, EitherAsync<string, ValidatedDelivery>> ValidateDeliveryNumber(Func<DeliveryNumber, TryAsync<bool>> checkDeliveryExists) =>
        unvalidatedDelivery => ValidateDelivery(checkDeliveryExists, unvalidatedDelivery);

        private static EitherAsync<string, ValidatedDelivery> ValidateDelivery(Func<DeliveryNumber, TryAsync<bool>> checkDeliveryExists, UnvalidatedDelivery unvalidatedDelivery) =>
        from deliveryEntryInfo in DeliveryEntry.TryParseStatus(unvalidatedDelivery.statusId, unvalidatedDelivery.orderID)
                                                .ToEitherAsync(() => $"Invalid delivery entry information({unvalidatedDelivery.DeliveryNumber},{unvalidatedDelivery.statusId},{unvalidatedDelivery.orderID})")
        from deliveryNumber in DeliveryNumber.TryParse(unvalidatedDelivery.DeliveryNumber)
                                             .ToEitherAsync(() => $"Invalid delivery number({unvalidatedDelivery.DeliveryNumber})")
        from deliveryExists in checkDeliveryExists(deliveryNumber)
                            .ToEither(error => error.ToString())
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
                whenValidatedDeliveries: CancelDeliveries);

        private static IDeliveries CancelDeliveries(ValidatedDeliveries validDeliveries) =>
            new CancelledDeliveries(validDeliveries.DeliveryList
                                                    .Select(CancelDelivery)
                                                     .ToList()
                                                      .AsReadOnly(),
                                                      validDeliveries.DeliveryList.Aggregate(new StringBuilder(), CreateMessage).ToString(), DateTime.Now);


        private static CancelledDelivery CancelDelivery(ValidatedDelivery validDelivery) =>
            new CancelledDelivery(validDelivery.DeliveryNumber, validDelivery.DeliveryEntry);


        private static StringBuilder CreateMessage(StringBuilder export, ValidatedDelivery delivery) =>
            export.AppendLine($"Delivery Number:{delivery.DeliveryNumber.Value}, status: {delivery.DeliveryEntry.Status}, orderId:{delivery.DeliveryEntry.OrderId}");



    }
}


