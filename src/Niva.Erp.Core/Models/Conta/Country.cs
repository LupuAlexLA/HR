using System.Collections.Generic;

namespace Niva.Erp.Models.Conta
{
	public class Country
	{
		public virtual int Id
		{
			get;
			set;
		}

		public virtual string CountryAbrv
		{
			get;
			set;
		}

		public virtual string CountryName
		{
			get;
			set;
		}

		public virtual IList<Region> Region
		{
			get;
			set;
		}

	}
}