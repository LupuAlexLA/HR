using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Xml.Serialization;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Threading.BackgroundWorkers;
using Abp.Threading.Timers;
using Abp.Timing;
using Niva.Erp.Models.Conta;

namespace Niva.Erp.BackgroudWorkers.Bnr
{
	// using System.Xml.Serialization;
	// XmlSerializer serializer = new XmlSerializer(typeof(DataSet));
	// using (StringReader reader = new StringReader(xml))
	// {
	//    var test = (DataSet)serializer.Deserialize(reader);
	// }

	[XmlRoot(ElementName = "Header", Namespace = "http://www.bnr.ro/xsd")]
	public class Header
	{

		[XmlElement(ElementName = "Publisher", Namespace = "http://www.bnr.ro/xsd")]
		public string Publisher { get; set; }

		[XmlElement(ElementName = "PublishingDate", Namespace = "http://www.bnr.ro/xsd")]
		public DateTime PublishingDate { get; set; }

		[XmlElement(ElementName = "MessageType", Namespace = "http://www.bnr.ro/xsd")]
		public string MessageType { get; set; }
	}

	[XmlRoot(ElementName = "Rate", Namespace = "http://www.bnr.ro/xsd")]
	public class Rate
	{

		[XmlAttribute(AttributeName = "currency", Namespace = "")]
		public string Currency { get; set; }

		[XmlText]
		public decimal Text { get; set; }

		[XmlAttribute(AttributeName = "multiplier", Namespace = "")]
		public int Multiplier { get; set; }
	}

	[XmlRoot(ElementName = "Cube", Namespace = "http://www.bnr.ro/xsd")]
	public class Cube
	{

		[XmlElement(ElementName = "Rate", Namespace = "http://www.bnr.ro/xsd")]
		public List<Rate> Rate { get; set; }

		[XmlAttribute(AttributeName = "date", Namespace = "")]
		public DateTime Date { get; set; }

		[XmlText]
		public string Text { get; set; }
	}

	[XmlRoot(ElementName = "Body", Namespace = "http://www.bnr.ro/xsd")]
	public class Body
	{

		[XmlElement(ElementName = "Subject", Namespace = "http://www.bnr.ro/xsd")]
		public string Subject { get; set; }

		[XmlElement(ElementName = "OrigCurrency", Namespace = "http://www.bnr.ro/xsd")]
		public string OrigCurrency { get; set; }

		[XmlElement(ElementName = "Cube", Namespace = "http://www.bnr.ro/xsd")]
		public Cube Cube { get; set; }
	}

	[XmlRoot(ElementName = "DataSet", Namespace = "http://www.bnr.ro/xsd")]
	public class DataSet
	{

		[XmlElement(ElementName = "Header", Namespace = "http://www.bnr.ro/xsd")]
		public Header Header { get; set; }

		[XmlElement(ElementName = "Body", Namespace = "http://www.bnr.ro/xsd")]
		public Body Body { get; set; }

		[XmlAttribute(AttributeName = "xmlns", Namespace = "")]
		public string Xmlns { get; set; }

		[XmlAttribute(AttributeName = "xsi", Namespace = "http://www.w3.org/2000/xmlns/")]
		public string Xsi { get; set; }

		[XmlAttribute(AttributeName = "schemaLocation", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
		public string SchemaLocation { get; set; }

		[XmlText]
		public string Text { get; set; }
	}





	public class CursBnrBackgroundWorker : PeriodicBackgroundWorkerBase, ISingletonDependency
	{
		private readonly IRepository<ExchangeRates> _exchangeRateRepository;
		private readonly IRepository<Currency> _currencyRepository;

		public CursBnrBackgroundWorker(AbpTimer timer, IRepository<ExchangeRates> exchangeRateRepository, IRepository<Currency> currencyRepository)
			: base(timer)
		{
			_exchangeRateRepository = exchangeRateRepository;
			_currencyRepository = currencyRepository;	 
			Timer.Period = 720000;//3600000; //5 seconds (good for tests, but normally will be more)
		}

		[UnitOfWork]
		protected override void DoWork()
		{
			//ServicePointManager.ServerCertificateValidationCallback += (senderr, certificate, chain, sslPolicyErrors) =>
			//{
			//	return true;
			//};
			//ServicePointManager.Expect100Continue = true;
			//ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

			var today = DateTime.Now.Date;
			var delayHours = 7;
			var todayDelayed = today.AddHours(-delayHours);
			var ratesForToday = _exchangeRateRepository.GetAll().Where(w => w.ExchangeDate >= today).ToList();

			if (ratesForToday.Count == 0)
			{
				HttpWebRequest request = (HttpWebRequest)WebRequest.Create(@"https://www.bnr.ro/nbrfxrates.xml");
				request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate | DecompressionMethods.None;

				DataSet responseData = new DataSet();
				using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
				using (Stream stream = response.GetResponseStream())
				{
					XmlSerializer serializer = new XmlSerializer(typeof(DataSet));
					using (StreamReader reader = new StreamReader(stream))
					{
						var xml = reader.ReadToEnd();
						using (StringReader stringReader = new StringReader(xml))
						{
							responseData = (DataSet)serializer.Deserialize(stringReader);
						}
					}
				}

				var currencies = _currencyRepository.GetAll().ToList();
				foreach (var rate in responseData.Body.Cube.Rate)
				{
					var rateCurrency = currencies.SingleOrDefault(c => c.CurrencyCode == rate.Currency);
					if (rateCurrency != null)
					{
						var exchangeDate = responseData.Body.Cube.Date;
						var count = _exchangeRateRepository.GetAll().Count(f => f.ExchangeDate == exchangeDate && f.CurrencyId == rateCurrency.Id);
						if (count == 0)
						{
							var exchangeRate = new ExchangeRates() { CurrencyId = rateCurrency.Id, ExchangeDate = responseData.Body.Cube.Date, Value = rate.Text };
							if (rate.Multiplier != 0)
								exchangeRate.Value = exchangeRate.Value / 100;
							_exchangeRateRepository.Insert(exchangeRate);
						}
					}
				}
			}
			CurrentUnitOfWork.SaveChanges();
		}
	}
}


