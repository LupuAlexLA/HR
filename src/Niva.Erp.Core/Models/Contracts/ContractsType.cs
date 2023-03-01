
namespace Niva.Erp.Models.Contracts
{
	using System;
	using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
	using System.Linq;
	using System.Text;

	public enum ContractsType : int
	{
		Contract,
		[Description("Act aditional")]
		ActAditional,
		[Description("Contract cadru")]
		ContractCadru
	}
}
