using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallAttendanceAPIService.Models
{
    class Attendance
    {
        public String MaNhanVien { get; set; }
        public String Ca { get; set; }
        public DateTime startTime { get; set; }
        public DateTime endTime { get; set; }
    }
}
