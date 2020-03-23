using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallAttendanceAPIService.Models
{
    class Result
    {
        public DateTime dateFetching { get; set; }
        public DateTime actualTimeFetching { get; set; }
        public int Session { get; set; }
        public bool success { get; set; }
        public string message { get; set; }
        public string VERSION { get; set; }

        public List<Attendance> data { get; set; }

        public Result(bool success, string message, List<Attendance> data)
        {
            this.success = success;
            this.message = message;
            this.data = data;
        }
        public Result()
        {
        }
    }
}
