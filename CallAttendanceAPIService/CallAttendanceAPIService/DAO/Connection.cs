using CallAttendanceAPIService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallAttendanceAPIService.DAO
{
    public static class Connection
    {
        public static DIEMDANHAPIEntities context = new DIEMDANHAPIEntities();
    }
}
