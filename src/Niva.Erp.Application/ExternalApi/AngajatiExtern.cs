using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;

namespace Niva.Erp.ExternalApi
{
    public class Angajati
    {
        public int Id { get; set; }
        public string Angajat { get; set; }
        public string Nume { get; set; }
        public string Prenume { get; set; }
        public string Cnp { get; set; }
        public string CI { get; set; }
        public int IdCompartiment { get; set; }
        public string Compartiment { get; set; }

    }

    public interface IAngajatiExternManager
    {
        public List<Angajati> GetAngajatiList();

        public decimal NrMediuSalariatiPerioada(DateTime data);

        public decimal NrMediuSalariatiLuna(DateTime data);
    }

    public class AngajatiExternManager : IAngajatiExternManager
    {
        private readonly IConfiguration _configuration; string rusalUrl;
        public AngajatiExternManager(IConfiguration configuration)
        {
            _configuration = configuration;
             
            rusalUrl = _configuration["App:RusalUrl"];
        }

        public List<Angajati> GetAngajatiList()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(rusalUrl+@"/api/rusal/angajati");

            List<Angajati> angajatList = new List<Angajati>();
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse()) 
            using (Stream stream = response.GetResponseStream())
            {
                JsonSerializer serializer = new JsonSerializer();
                using (StreamReader reader = new StreamReader(stream))
                {
                    var responseBody = reader.ReadToEnd();
                    angajatList = JsonConvert.DeserializeObject<List<Angajati>>(responseBody);
                }
            }
            return angajatList;
        }

        public decimal NrMediuSalariatiPerioada(DateTime data)
        {
            decimal ret = 0;
            string _data = data.Year.ToString() + "-" + data.Month.ToString() + "-" + data.Day.ToString();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(rusalUrl + @"/api/rusal/numarmediusalariatipanaladata?data=" + _data);

            List<Angajati> angajatList = new List<Angajati>();
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    var responseBody = reader.ReadToEnd();
                    ret = decimal.Parse(responseBody, CultureInfo.InvariantCulture);
                }
            }
            return ret;
        }

        public decimal NrMediuSalariatiLuna(DateTime data)
        {
            decimal ret = 0;
            string _data = data.Year.ToString() + "-" + data.Month.ToString() + "-" + data.Day.ToString();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(rusalUrl + @"/api/rusal/numarsalariatiladata?data=" + _data);

            List<Angajati> angajatList = new List<Angajati>();
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    var responseBody = reader.ReadToEnd();
                    ret = decimal.Parse(responseBody);
                }
            }
            return ret;
        }
    }
}
