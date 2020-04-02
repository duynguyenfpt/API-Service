using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace CallAttendanceAPIService.Models
{
    class APIHandler
    {
        public int[] timelinesHours = new int[] { 5, 10, 13, 18, 21, 26 };
        public int[] timelinesMinutes = new int[] { 55, 0, 55, 0, 55, 0 };

        public int getTimeLines(DateTime time)
        {
            int hour = time.Hour;
            int minutes = time.Minute;
            int index;
            for (index = 0; index < 6; index++)
            {
                if ((hour < timelinesHours[index]) ||
                    (hour == timelinesHours[index] && minutes < timelinesMinutes[index]))
                {
                    // get nearst timeline
                    break;
                }
            }
            return index;
        }

        public async Task<Result> fetchData(DateTime time)
        {
            int timeMileStones = getTimeLines(time);
            // determine which date which session should be call based on time passing as argument
            DateTime dateFetchData = new DateTime(time.Year,time.Month,time.Day);
            int sessionFetchData = 1;
            switch (timeMileStones)
            {
                case 0:
                    //time fetching will be session 3 - previous day 
                    dateFetchData = dateFetchData.AddDays(-1);
                    sessionFetchData = 3;
                    break;
                case 1:
                case 2:
                    //time fetching will be session 1 - current day 
                    sessionFetchData = 1;
                    break;
                case 3:
                case 4:
                    //time fetching will be session 2 - current day 
                    sessionFetchData = 2;
                    break;
                case 5:
                    //time fetching will be session 3 - current day 
                    sessionFetchData = 3;
                    break;
            }

            // start fetching data
            var sentRequest = new RequestParams();
            sentRequest.data.IDCaLamViec = "CA" + sessionFetchData;
            sentRequest.data.ngay = $"{dateFetchData.Year}-{dateFetchData.Month}-{dateFetchData.Day}";
            var serializer = new JavaScriptSerializer();
            var json = serializer.Serialize(sentRequest);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            var url = "http://113.162.60.102:6688/api_dksx/diemdanh";
            var uri = new Uri(url);
            var client = new HttpClient();
            Result dataPostBack = new Result();
            try
            {
                var response = await client.PostAsync(uri, data);
                //string result = response.Content.ReadAsStringAsync().Result;
                response.EnsureSuccessStatusCode();
                var result = response.Content.ReadAsStringAsync().Result;
                // middle step to convert string in "data" object
                var datafromAPI = serializer.Deserialize<DataFromApi>(result);
                //
                dataPostBack.success = datafromAPI.success;
                dataPostBack.message = datafromAPI.message;
                dataPostBack.VERSION = datafromAPI.VERSION;
                
                    dataPostBack.data = datafromAPI.data == null ?new List<Attendance>() : serializer.Deserialize<List<Attendance>>(datafromAPI.data);
            }
            catch (Exception ex)
            {
                await Task.Run(() => {
                    dataPostBack.success = false;
                    dataPostBack.message = ex.Message;
                });

            }
            dataPostBack.actualTimeFetching = time;
            dataPostBack.dateFetching = dateFetchData;
            dataPostBack.Session = sessionFetchData;
            return dataPostBack;
        }
    }

    class DataFromApi
    {
        public bool success { get; set; }
        public string message { get; set; }

        public string data { get; set; }
        public string VERSION { get; set; }
    }
}
