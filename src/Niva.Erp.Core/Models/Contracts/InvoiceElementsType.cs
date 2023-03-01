
namespace Niva.Erp.Models.Contracts
{
	using System;
	using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
	using System.Linq;
	using System.Text;

	public enum InvoiceElementsType : int
	{
		[Description("Mijloace fixe")]
		MijloaceFixe,
		[Description("Cheltuieli in avans")]
		CheltuieliInAvans,
		[Description("Obiecte de inventar")]
		ObiecteDeInventar,
		Altele,
		[Description("Venituri in avans")]
        VenituriInAvans
	}
}
