using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Niva.Erp.EntityFrameworkCore.Methods;
using Niva.Erp.SectoareBnr.Dto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Niva.Erp.ExternalApi
{
    public interface IPlasamentBNRManager
    {
        public List<PlasamentBNRDto> PlasamenteBNRList(DateTime data); 
    }
    public class PlasamentBNRManager : IPlasamentBNRManager
    {
        private readonly IConfiguration _configuration;
        string fgdbApiUrl;
        string rusalUrl;
        public PlasamentBNRManager(IConfiguration configuration)
        {
            _configuration = configuration;
            fgdbApiUrl = _configuration["App:FgdbUrl"];
            rusalUrl = _configuration["App:RusalUrl"];
        }

        public List<PlasamentBNRDto> PlasamenteBNRList(DateTime data)
        {
            string dataStr = LazyMethods.DateToString(data);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(fgdbApiUrl+"/api/external/getPlasamenteStatisticaBnr/" + dataStr);

            List<PlasamentBNRDto> plasamenteBNRList = new List<PlasamentBNRDto>();
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using(Stream stream = response.GetResponseStream())
            {
                JsonSerializer serializer = new JsonSerializer();
                using(StreamReader reader = new StreamReader(stream))
                {
                    var responseBody = reader.ReadToEnd();
                    plasamenteBNRList = JsonConvert.DeserializeObject<List<PlasamentBNRDto>>(responseBody);
                }
            }

            return plasamenteBNRList;
        }
    }
}
