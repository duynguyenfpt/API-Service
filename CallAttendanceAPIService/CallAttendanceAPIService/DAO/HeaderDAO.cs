using CallAttendanceAPIService.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallAttendanceAPIService.DAO
{
    class HeaderDAO
    {
        public void Insert(Result data)
        {
            Header_DiemDanh_NangSuat_LaoDong header = new Header_DiemDanh_NangSuat_LaoDong();
            header.NgayDiemDanh = data.dateFetching;
            header.Ca = data.Session;
            header.Status = data.success;
            header.Message = data.message;
            header.VERSION = data.VERSION;
            header.FetchDataTime = data.actualTimeFetching;
            using (var db = new DIEMDANHAPIEntities())
            {
                try
                {
                    db.Header_DiemDanh_NangSuat_LaoDong.Add(header);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        //
        public int getHeader(DateTime date, int session, DateTime timefetching)
        {
            using (var db = new DIEMDANHAPIEntities())
            {
                try
                {
                    string sqlQuery = @"select headerId from Header_DiemDanh_NangSuat_LaoDong where (NgayDiemDanh = @date and Ca = @session and FetchDataTime = @fetchDataTime)";
                    var headerID =  db.Database.SqlQuery<int>(sqlQuery, new SqlParameter("date", date), new SqlParameter("session", session), new SqlParameter("fetchDataTime", timefetching)).FirstOrDefault();
                    //
                    return headerID;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        // get the first successful fetch API
        public int getFirstSuccessfullyFetch(DateTime date, int session)
        {
            //string sqlPhongBanTieuChi = "select a.MaPhongBan,a.MaTieuChi,b.TenTieuChi from PhongBan_TieuChi a left join TieuChi b on a.MaTieuChi = b.MaTieuChi\n" +
            //                "where MaPhongBan = @maphongban and Thang = @thang and Nam = @nam";

            //list = db.Database.SqlQuery<TieuChiABC>(sqlPhongBanTieuChi, new SqlParameter("maphongban", departmentID),
            //    new SqlParameter("thang", month),
            //    new SqlParameter("nam", year)).ToList<TieuChiABC>();
            using (var db = new DIEMDANHAPIEntities())
            {
                string sqlQuery = @"select headerId from Header_DiemDanh_NangSuat_LaoDong 
                              where FetchDataTime = (Select Min(FetchDataTime) from Header_DiemDanh_NangSuat_LaoDong where NgayDiemDanh = @date and Ca = @session and (Status = 1 or isCreatedManually =1)) ";
                try
                {
                    var minHeaderIDNull = db.Database.SqlQuery<int?>(sqlQuery, new SqlParameter("date", date), new SqlParameter("session", session)).FirstOrDefault();
                    int minHeaderIDResult = minHeaderIDNull.HasValue ? minHeaderIDNull.Value : -1;
                    return minHeaderIDResult;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
    }
}
