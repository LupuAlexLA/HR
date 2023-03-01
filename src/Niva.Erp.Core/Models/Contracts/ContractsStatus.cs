
namespace Niva.Erp.Models.Contracts
{
	using System;
	using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
	using System.Linq;
	using System.Text;

	public enum ContractsStatus : int
	{
		[Description("In vigoare")]
		InVigoare,
		Reziliat,
		Finalizat,
		Preliminat
	}
}
