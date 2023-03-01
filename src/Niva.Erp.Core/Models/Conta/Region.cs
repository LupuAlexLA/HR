using Abp.Domain.Entities;
using System.Collections.Generic;

namespace Niva.Erp.Models.Conta
{
	public class Region : Entity
	{
		 
		public virtual string RegionName
		{
			get;
			set;
		}

		public virtual string RegionAbrv
		{
			get;
			set;
		}

		public virtual IList<Locality> Locality
		{
			get;
			set;
		}

		public virtual Country Country
		{
			get;
			set;
		}
	}
}