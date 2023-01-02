using App.Domain.Models;
using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Repositories
{
    public interface IOrderRegistrationNumberRepository {
        TryAsync<List<OrderRegistrationNumber>> TryGetRegistrationNumbers();
    }
}
