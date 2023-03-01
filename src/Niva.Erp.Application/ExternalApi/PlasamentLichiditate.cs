using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Niva.Erp.EntityFrameworkCore.Methods;
using Niva.Erp.Repositories.Conta.Lichiditate;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Niva.Erp.ExternalApi
{
    public interface IPlasamentLichiditateManager
    {
        public List<PlasamentLichiditateDto> PlasamenteLichiditateList(DateTime data);
    }
    public class PlasamentLichiditateManager : IPlasamentLichiditateManager
    {
        private readonly IConfiguration _configuration;
        string fgdbApiUrl;
        string rusalUrl;
        public PlasamentLichiditateManager(IConfiguration configuration)
        {
            _configuration = configuration;
            fgdbApiUrl = _configuration["App:FgdbUrl"];
            rusalUrl = _configuration["App:RusalUrl"];
        }

        public List<PlasamentLichiditateDto> PlasamenteLichiditateList(DateTime data)
        {
            string dataStr = LazyMethods.DateToString(data);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(fgdbApiUrl+"/api/external/getDetaliiPlasamente/" + dataStr);

            List<PlasamentLichiditateDto> plasamenteLichiditateList = new List<PlasamentLichiditateDto>();
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            {
                JsonSerializer serializer = new JsonSerializer();
                using (StreamReader reader = new StreamReader(stream))
                {
                    var responseBody = reader.ReadToEnd();
                    plasamenteLichiditateList = JsonConvert.DeserializeObject<List<PlasamentLichiditateDto>>(responseBody);
                }
            }

            return plasamenteLichiditateList;
        }
    }
}
