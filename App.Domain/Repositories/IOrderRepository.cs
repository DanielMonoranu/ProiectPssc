using App.Domain.Models;
using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Domain.Models.ShopOrder;

namespace App.Domain.Repositories
{
    public interface IOrderRepository {
        TryAsync<List<CalculatedOrder>> TryGetExistingGrades();

        TryAsync<Unit> TrySaveCancelledOrders(CancelledShopOrder order);
    }
}
