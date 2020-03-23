using CallAttendanceAPIService.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace CallAttendanceAPIService.DAO
{
    class AttendanceDAO
    {
        public List<Attendance> getUnExistItemList(List<Attendance> listID, int headerID)
        {
            var listIDString = $"({listID[0].MaNhanVien}";
            for (int index = 1; index < listID.Count; index++)
            {
                listIDString += $",{listID[index].MaNhanVien}";
                if (index == listID.Count - 1)
                {
                    listIDString += ")";
                }
            }
            string sqlQuery = "select MaNV from DiemDanh_NangSuatLaoDong" +
                              "where HeaderID = @headerID and MaNV in @listMaNV";
            try
            {
                List<String> IDs = Connection.context.Database.SqlQuery<String>(sqlQuery, new SqlParameter("headerID", headerID), new SqlParameter("listMaNV", listIDString)).ToList();
                var filterIDList = listID.Where(x => IDs.Contains(x.MaNhanVien)).ToList();
                return filterIDList;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //
        public void Insert(List<DiemDanh_NangSuatLaoDong> listAttendance)
        {
            using (var transaction = Connection.context.Database.BeginTransaction())
            {
                try
                {
                    Connection.context.DiemDanh_NangSuatLaoDong.AddRange(listAttendance);
                    Connection.context.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }
    }
}
