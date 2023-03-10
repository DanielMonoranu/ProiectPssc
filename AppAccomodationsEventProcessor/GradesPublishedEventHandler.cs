using App.Domain.Models;
using AppEventsListeners.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppAccomodationsEventProcessor
{
    public class GradesPublishedEventHandler
    {
        public override string[] EventTypes => new string[] { typeof(ShopOrderCancelledEvent).Name };

        protected override Task<EventProcessingResult> OnHandleAsync(ShopOrderCancelledEvent eventData)
        {
            Console.WriteLine(eventData.ToString());
            return Task.FromResult(EventProcessingResult.Completed);
        }
    }
}