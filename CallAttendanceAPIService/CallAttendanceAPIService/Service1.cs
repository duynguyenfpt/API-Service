using CallAttendanceAPIService.DAO;
using CallAttendanceAPIService.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace CallAttendanceAPIService
{
    public partial class Service1 : ServiceBase
    {
        Timer timer = new Timer(); // name space(using System.Timers;) 
        APIHandler handler = new APIHandler();
        HeaderDAO headerDAO = new HeaderDAO();
        AttendanceDAO attendanceDAO = new AttendanceDAO();
        HeaderDetailDAO headerDetailDAO = new HeaderDetailDAO();

        public async void FetchAndUpdateDatabase(DateTime time)
        {
            using (var transaction = Connection.context.Database.BeginTransaction())
            {
                try
                {
                    //
                    int timePoint = handler.getTimeLines(time);
                    //
                    Result dataReceived = await handler.fetchData(time);
                    headerDAO.Insert(dataReceived);
                    //
                    if (dataReceived.success)
                    {
                        int headerIDMin = headerDAO.getFirstSuccessfullyFetch(dataReceived.dateFetching, dataReceived.Session);
                        int currenHeaderID = headerDAO.getHeader(dataReceived.dateFetching, dataReceived.Session, dataReceived.actualTimeFetching);
                        if (headerIDMin == -1)
                        {
                            headerIDMin = currenHeaderID;
                        }
                        //
                        var listHaveNotAdded = attendanceDAO.getUnExistItemList(dataReceived.data, headerIDMin);
                        List<DiemDanh_NangSuatLaoDong> attendanceList = new List<DiemDanh_NangSuatLaoDong>();
                        foreach(var item in listHaveNotAdded)
                        {
                            DiemDanh_NangSuatLaoDong ddEntity = new DiemDanh_NangSuatLaoDong();
                            ddEntity.HeaderID = headerIDMin;
                            ddEntity.MaNV = item.MaNhanVien;
                            ddEntity.ActualHeaderFetched = currenHeaderID;
                            ddEntity.DiLam = true;
                            ddEntity.isFilledFromAPI = true;
                            ddEntity.isChangedManually = false;
                            ddEntity.ThoiGianXuongLo = item.startTime;
                            ddEntity.ThoiGianLenLo = item.endTime;
                            attendanceList.Add(ddEntity);
                        }
                        attendanceDAO.Insert(attendanceList);
                        DateTime nextTimePoint;
                        if (timePoint == 5)
                        {
                            // next 2 AM morning
                            nextTimePoint = new DateTime(time.Year, time.Month, time.Day + 1, 2, 0, 0);
                        } else
                        {
                            nextTimePoint = new DateTime(time.Year, time.Month, time.Day, handler.timelinesHours[timePoint], handler.timelinesMinutes[timePoint], 0);
                        }

                        timer.Interval = nextTimePoint.Subtract(time).Seconds;
                    } else
                    {
                        // 10 minutes each until successfully fetch
                        timer.Interval = 10 * 1000 * 60;
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    WriteToFile(ex.Message);
                    transaction.Rollback();
                }
            }
        }

        public Service1()
        {
            InitializeComponent();
        }
        protected override void OnStart(string[] args)
        {
            FetchAndUpdateDatabase(DateTime.Now);
            timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            //timer.Interval = 5000; //number in milisecinds  
            timer.Enabled = true;
        }
        protected override void OnStop()
        {
            WriteToFile($"Stop At {DateTime.Now}");
        }
        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            FetchAndUpdateDatabase(DateTime.Now);
        }
        public void WriteToFile(string Message)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ServiceLog_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";
            if (!File.Exists(filepath))
            {
                // Create a file to write to.   
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
        }
    }
}
