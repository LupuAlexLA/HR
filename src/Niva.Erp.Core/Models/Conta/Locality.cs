namespace Niva.Erp.Models.Conta
{
	public class Locality
	{
		public virtual int Id
		{
			get;
			set;
		}

		public virtual string Name
		{
			get;
			set;
		}

		public virtual Region Region
		{
			get;
			set;
		}

	}
}