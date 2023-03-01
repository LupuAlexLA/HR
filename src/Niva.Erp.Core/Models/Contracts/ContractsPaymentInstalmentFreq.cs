 
namespace Niva.Erp.Models.Contracts
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
	using System.Linq;
	using System.Text;

	public enum ContractsPaymentInstalmentFreq : int
	{
		Lunar,
		Semestrial,
		Trimestrial,
		Ocazional,
	}
}
