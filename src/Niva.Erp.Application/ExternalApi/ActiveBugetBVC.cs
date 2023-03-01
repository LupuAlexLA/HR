using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Niva.Erp.EntityFrameworkCore.Methods;
using Niva.Erp.Repositories.Buget;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Niva.Erp.ExternalApi
{
    
    public interface IActiveBugetBVCManager
    {//test branch nou
        public List<ActiveBugetBVCDto> ActiveBugetBVCList(DateTime data);
        public List<ActiveBugetBVCDto> ActiveBugetCFList(DateTime data);
        public SalarizareBVCDto SalarizareBVCList(int year);
        public SalarizareCFDto SalarizareCFList(int year);
        public List<ZileLibereDto> ZileLibere(int year);
        public List<SalarizareCFRealizatiDto> SalarizareCFRealizatList(DateTime data);
    }

    public class ActiveBugetBVCManager : IActiveBugetBVCManager
    {
        private readonly IConfiguration _configuration;
        string fgdbApiUrl;
        string rusalUrl;
        public ActiveBugetBVCManager(IConfiguration  configuration)
        {
            _configuration = configuration;
            fgdbApiUrl = _configuration["App:FgdbUrl"];
            rusalUrl = _configuration["App:RusalUrl"];
        }

        public List<ActiveBugetBVCDto> ActiveBugetBVCList(DateTime data)
        {
            string url = LazyMethods.LinkGetPlasamenteBuget();
            string dataStr = LazyMethods.DateToString(data);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(fgdbApiUrl+@"/api/external/getPlasamenteBuget/" + dataStr);

            List<ActiveBugetBVCDto> activeList = new List<ActiveBugetBVCDto>();
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            {
                JsonSerializer serializer = new JsonSerializer();
                using (StreamReader reader = new StreamReader(stream))
                {
                    var responseBody = reader.ReadToEnd();
                    activeList = JsonConvert.DeserializeObject<List<ActiveBugetBVCDto>>(responseBody);
                }
            }
            return activeList;
        }

        public List<ActiveBugetBVCDto> ActiveBugetCFList(DateTime data)
        {
            string url = LazyMethods.LinkGetPlasamenteBuget();
            string dataStr = LazyMethods.DateToString(data);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(fgdbApiUrl+@"/api/external/getPlasamenteCashFlow/" + dataStr);

            List<ActiveBugetBVCDto> activeList = new List<ActiveBugetBVCDto>();
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            {
                JsonSerializer serializer = new JsonSerializer();
                using (StreamReader reader = new StreamReader(stream))
                {
                    var responseBody = reader.ReadToEnd();
                    activeList = JsonConvert.DeserializeObject<List<ActiveBugetBVCDto>>(responseBody);
                }
            }
            return activeList;
        }

        public SalarizareBVCDto SalarizareBVCList(int year)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(rusalUrl+@"/api/rusal/buget?an=" + year);

            var salarizareList = new SalarizareBVCDto();
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            {
                JsonSerializer serializer = new JsonSerializer();
                using (StreamReader reader = new StreamReader(stream))
                {
                    var responseBody = reader.ReadToEnd();
                    salarizareList = JsonConvert.DeserializeObject<SalarizareBVCDto>(responseBody);
                }
            }
            return salarizareList;
        }

        public SalarizareCFDto SalarizareCFList(int year)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(rusalUrl+@"/api/rusal/cashflow?an=" + year);

            var salarizareList = new SalarizareCFDto();
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            {
                JsonSerializer serializer = new JsonSerializer();
                using (StreamReader reader = new StreamReader(stream))
                {
                    var responseBody = reader.ReadToEnd();
                    salarizareList = JsonConvert.DeserializeObject<SalarizareCFDto>(responseBody);
                }
            }
            return salarizareList;
        }
        public List<SalarizareCFRealizatiDto> SalarizareCFRealizatList(DateTime data)
        {
            string _data = data.Year.ToString() + "-" + data.Month.ToString();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(rusalUrl + @"/api/rusal/cashflowrealizat?luna=" + _data);

            var salarizareList = new List<SalarizareCFRealizatiDto>();
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            {
                JsonSerializer serializer = new JsonSerializer();
                using (StreamReader reader = new StreamReader(stream))
                {
                    var responseBody = reader.ReadToEnd();
                    salarizareList = JsonConvert.DeserializeObject<List<SalarizareCFRealizatiDto>>(responseBody);
                }
            }
            return salarizareList;
        }
        public List<ZileLibereDto> ZileLibere(int year)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(fgdbApiUrl+@"/api/external/zileLibereAn/" + year);

            var zileLibere = new List<ZileLibereDto>();
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            {
                JsonSerializer serializer = new JsonSerializer();
                using (StreamReader reader = new StreamReader(stream))
                {
                    var responseBody = reader.ReadToEnd();
                    zileLibere = JsonConvert.DeserializeObject<List<ZileLibereDto>>(responseBody);
                }
            }
            return zileLibere;
        }
    }

}
