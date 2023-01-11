using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Events
{
    public enum EventProcessingResult
    {
        Completed,
        Retry,
        Failed
    }
}
