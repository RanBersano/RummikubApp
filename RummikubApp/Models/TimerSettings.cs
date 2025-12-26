using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RummikubApp.Models
{
    internal class TimerSettings(long totalTimeInMillseconds, long intervalInMillseconds)
    {
        public long TotalTimeInMillseconds { get; set; } = totalTimeInMillseconds;
        public long TotalValInMillseconds { get; set; } = intervalInMillseconds;
    }
}
