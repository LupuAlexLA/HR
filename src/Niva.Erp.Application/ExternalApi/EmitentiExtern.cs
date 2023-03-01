using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Niva.Erp.ExternalApi
{
    public class IssuerExtern
    {
        public int Id { get; set; }
        public string DenumireBanca { get; set; }
        public string CodTara { get; set; }
        public string Bic { get; set; }
        public string Swift { get; set; }
        public string CodStatistica { get; set; }
    }

    public interface IEmitentiExternManager
    {
        public List<IssuerExtern> GetIssuerList();
    }

    public class EmitentiExternManager : IEmitentiExternManager
    {
        private readonly IConfiguration _configuration;
        string fgdbApiUrl;
        
        public EmitentiExternManager(IConfiguration configuration)
        {
            _configuration = configuration;
            fgdbApiUrl = _configuration["App:FgdbUrl"];
            
        }

        public List<IssuerExtern> GetIssuerList()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(fgdbApiUrl+@"/api/external/getBanciActive");

            List<IssuerExtern> issuerList = new List<IssuerExtern>();
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            {
                JsonSerializer serializer = new JsonSerializer();
                using (StreamReader reader = new StreamReader(stream))
                {
                    var responseBody = reader.ReadToEnd();
                    issuerList = JsonConvert.DeserializeObject<List<IssuerExtern>>(responseBody);
                }
            }
            return issuerList;
        }
    }
}
