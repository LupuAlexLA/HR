using Abp.Domain.Services;
using Abp.UI;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;


// Eroare atunci cand interfata este injectata in alt AppService (de rezolvat) Se poate implementa ca metoda in orice AppService 

namespace Niva.Erp.ExternalApi

{
    public class FirmaCuiData
    {
        public int cui { get; set; }
        public string data { get; set; }
        
    }

    public class DateFirmaAnaf
    {
        public List<found> found { get; set; }
    }
    public class found
    {
        public int cui { get; set; }
        public string denumire { get; set; }
        public string data { get; set; }
        public string adresa { get; set; }
        public string nrRegCom { get; set; }
        public bool scpTVA { get; set; }
        public string data_inceput_ScpTVA { get; set; }
        public bool statusTvaIncasare { get; set; }
    }

    public interface IAnafAPI
    {
        Task<found> AnafAPIAsync(int cui);
    }

    public class AnafAPI : DomainService, IAnafAPI
    {
        public AnafAPI()
        { }

        public async Task<found> AnafAPIAsync(int cui)
        {
            try
            {
                var payload = new FirmaCuiData
                {
                    cui = cui,
                    data = DateTime.Now.ToString("yyyy'-'MM'-'dd")
                };


                string stringPayload = System.Text.Json.JsonSerializer.Serialize(payload);
                stringPayload = "[" + stringPayload + "]";


                var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");

                var httpClient = new HttpClient();
                var httpResponse = await httpClient.PostAsync("https://webservicesp.anaf.ro/PlatitorTvaRest/api/v6/ws/tva", httpContent);
                var response = await httpResponse.Content.ReadAsStringAsync();
                return System.Text.Json.JsonSerializer.Deserialize<DateFirmaAnaf>(response).found[0];
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }
    }
}

