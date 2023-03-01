 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niva.Erp.Models.AutoOperation;

namespace Niva.Erp.Models.Conta
{
    public class AccountDivConfigDto
    {
        public int Id { get; set; }

        public AccountDivConfigType AccountType { get; set; }

        public string AccountTypeStr { get; set; }

        public AccountDivConfigPersType? PersType { get; set; }

        public string PersTypeStr { get; set; }

        public AccountDivConfigResidenceType? ResidenceType { get; set; }

        public string ResidenceTypeStr { get; set; }

        public int? DivYear { get; set; }

        public int AccountId { get; set; }

        public string AccountName { get; set; }

        public int AppClientId { get; set; }
    }
}
