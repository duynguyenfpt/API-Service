using CallAttendanceAPIService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallAttendanceAPIService.DAO
{
    class HeaderDetailDAO
    {
        public void Insert(int headerID)
        {
            using (var db = new DIEMDANHAPIEntities())
            {
                Header_DiemDanh_NangSuat_LaoDong_Detail header_detail = new Header_DiemDanh_NangSuat_LaoDong_Detail();
                header_detail.HeaderID = headerID;
                try
                {
                    db.Header_DiemDanh_NangSuat_LaoDong_Detail.Add(header_detail);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
    }
}
