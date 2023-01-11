using App.Domain.Deliveries.Models;
using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Domain.Deliveries.Models.Deliveries;

namespace App.Domain.Deliveries.Repositories
{
    public interface IEntriesRepository
    {
        TryAsync<List<CancelledDelivery>> TryGetExistingDelivery();
        TryAsync<Unit> TrySaveDelivery(AnnouncedDeliveries deliveries);
    }
}
