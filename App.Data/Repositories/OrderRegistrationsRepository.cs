using App.Domain.Models;
using App.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Data.Repositories
{
    public class OrderRegistrationsRepository : IOrderRegistrationNumberRepository
    {
        private readonly OrdersContext ordersContext;

        public OrderRegistrationsRepository(OrdersContext ordersComtext)
        {
            this.ordersContext = ordersComtext;
        }

        public TryAsync<List<OrderRegistrationNumber>> TryGetRegistrationNumbers() {
            return async () =>
            {
                var registrationNumbers = await ordersContext.OrderRegistrations.AsNoTracking().ToListAsync();

                var result = registrationNumbers.Select(reg => new OrderRegistrationNumber(reg.RegistrationNumber)).ToList();

                return result;
            };
        }
    }
}
