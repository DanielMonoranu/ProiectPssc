using App.Data.Models;
using App.Domain.Models;
using App.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Domain.Models.ShopOrder;
using static LanguageExt.Prelude;
namespace App.Data.Repositories
{
    public class OrdersRepository: IOrderRepository
    {
        private readonly OrdersContext dbContext;

        public OrdersRepository(OrdersContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public TryAsync<List<CalculatedOrder>> TryGetExistingGrades()
        {
            return async () => (await (
                                    from g in dbContext.Orders
                                    join s in dbContext.OrderRegistrations on g.OrderRegistrationId equals s.RegistrationNumber
                                    select new { s.RegistrationNumber, g.OrderId, g.Price, g.FinalPrice })
                                    .AsNoTracking()
                                    .ToListAsync())
                                    .Select(result => new CalculatedOrder(
                                                                StudentRegistrationNumber: new(result.RegistrationNumber.ToString()),
                                                                Order: new(result.Price ?? 0m, false, result.OrderId),
                                                                FinalOrder: new(result.FinalPrice ?? 0m, false, result.OrderId))
                                    {
                                        OrderId = result.OrderId
                                    })
                                    .ToList();
        }

        public TryAsync<Unit> TrySaveCancelledOrders(CancelledShopOrder orders) 
        {
            return async () =>
            {
                //var registrationNumbers = (await dbContext.OrderRegistrations.ToListAsync()).ToLookup(registrationNumber => registrationNumber.RegistrationNumber);
                var toCancelOrders = orders.GradeList.Select(g => new OrderDTO()
                {
                    OrderId = g.Order.OrderId,
                    OrderRegistrationId = g.StudentRegistrationNumber.Value,
                    Price = g.Order.OrderValue,
                    FinalPrice = g.FinalOrder.OrderValue,
                    IsCancelled = true
                });
                foreach (var entity in toCancelOrders)
                {
                    dbContext.Update(entity);
                };

                await dbContext.SaveChangesAsync();

                return unit;
            };
        }
    }
}
