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
            if (listID.Count == 0) {
                return new List<Attendance>();
            }
            else
            {
                using (var db = new DIEMDANHAPIEntities())
                {
                    var listIDString = $"{listID[0].MaNhanVien}";
                    for (int index = 1; index < listID.Count; index++)
                    {
                        listIDString += $",{listID[index].MaNhanVien}";
                    }
                    string sqlQuery = "select MaNV from DiemDanh_NangSuatLaoDong " +
                                      "where HeaderID = @headerID and MaNV in (@listMaNV)";
                    try
                    {
                        List<String> IDs = db.Database.SqlQuery<String>(sqlQuery, new SqlParameter("headerID", headerID), new SqlParameter("listMaNV", listIDString)).ToList();
                        var filterIDList = listID.Where(x => !IDs.Contains(x.MaNhanVien)).ToList();
                        return filterIDList;

                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
        }
        //
        public void Insert(List<DiemDanh_NangSuatLaoDong> listAttendance)
        {
            using (var db = new DIEMDANHAPIEntities())
            {
                try
                {
                    db.DiemDanh_NangSuatLaoDong.AddRange(listAttendance);
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
