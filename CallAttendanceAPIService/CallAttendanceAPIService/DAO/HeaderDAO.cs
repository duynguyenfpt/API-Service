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
            try
            {
                Connection.context.Header_DiemDanh_NangSuat_LaoDong.Add(header);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //
        public int getHeader(DateTime date,int session, DateTime timefetching) {
            try
            {
                int headerID = Connection.context.Header_DiemDanh_NangSuat_LaoDong
                   .Where(x=> x.NgayDiemDanh == date && x.Ca == session && x.FetchDataTime == timefetching)
                   .Select(x=> x.HeaderID)
                   .First();
                //
                return headerID;
            }catch (Exception ex)
            {
                throw ex;
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
            string sqlQuery = @"select headerId where FetchDataTime = (Select Min(FetchDataTime) from Header_DiemDanh_NangSuat_LaoDong where NgayDiemDanh = @date and Ca = 
                              @session and Status = true)";
            try{
                return Convert.ToInt32(Connection.context.Database.SqlQuery<int>(sqlQuery, new SqlParameter("date", date), new SqlParameter("session", session)));
            } catch (Exception ex) {
                throw ex;
            }
        }
}
