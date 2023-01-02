using CSharp.Choices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models
{
    [AsChoice]
    public static partial class ShopOrder
    {
        public interface IShopOrder { }

        public record UnvalidatedShopOrders: IShopOrder {
            public UnvalidatedShopOrders(IReadOnlyCollection<UnvalidatedOrder> gradeList)
            {
                GradeList = gradeList;
            }

            public IReadOnlyCollection<UnvalidatedOrder> GradeList { get; }
        }

        public record InvalidatedShopOrders : IShopOrder {
            internal InvalidatedShopOrders(IReadOnlyCollection<UnvalidatedOrder> gradeList, string reason)
            {
                GradeList = gradeList;
                Reason = reason;
            }

            public IReadOnlyCollection<UnvalidatedOrder> GradeList { get; }
            public string Reason { get; }
        }

        public record ValidatedShopOrders : IShopOrder {
            internal ValidatedShopOrders(IReadOnlyCollection<ValidatedOrder> gradesList)
            {
                GradeList = gradesList;
            }

            public IReadOnlyCollection<ValidatedOrder> GradeList { get; }
        }

/*        public record ShippedShopOrders : IShopOrder {
            internal ShippedShopOrders(IReadOnlyCollection<CalculatedOrder> gradesList, string csv, DateTime publishedDate)
            {
                GradeList = gradesList;
                PublishedDate = publishedDate;
                Csv = csv;
            }

            public IReadOnlyCollection<CalculatedOrder> GradeList { get; }
            public DateTime PublishedDate { get; }
            public string Csv { get; }
        }*/

        public record CalculatedShopOrders : IShopOrder
        {
            internal CalculatedShopOrders(IReadOnlyCollection<CalculatedOrder> gradesList)
            {
                GradeList = gradesList;
            }

            public IReadOnlyCollection<CalculatedOrder> GradeList { get; }
        }

        public record FailedShopOrders : IShopOrder {
            internal FailedShopOrders(IReadOnlyCollection<UnvalidatedOrder> gradeList, Exception exception) {
                GradeList = gradeList;
                Exception = exception;
            }

            public IReadOnlyCollection<UnvalidatedOrder> GradeList { get; }
            public Exception Exception { get; }
        }

        public record CancelledShopOrder : IShopOrder {
            internal CancelledShopOrder(IReadOnlyCollection<CalculatedOrder> gradesList, string csv, DateTime cancellationDate) {
                GradeList = gradesList;
                Csv = csv;
                CancellationDate = cancellationDate;
            }

            public IReadOnlyCollection<CalculatedOrder> GradeList { get; }
            public DateTime CancellationDate { get; }
            public string Csv { get; }
        }

    }
}
