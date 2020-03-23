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
            Header_DiemDanh_NangSuat_LaoDong_Detail header_detail = new Header_DiemDanh_NangSuat_LaoDong_Detail();
            header_detail.HeaderID = headerID;
            try
            {
                Connection.context.Header_DiemDanh_NangSuat_LaoDong_Detail.Add(header_detail);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
