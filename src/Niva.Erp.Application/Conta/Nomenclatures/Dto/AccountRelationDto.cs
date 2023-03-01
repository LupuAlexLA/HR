using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niva.Conta.Nomenclatures
{
    public class AccountRelationDto
    {
        public virtual int Id { get; set; }

        public virtual string DebitRoot { get; set; }

        public virtual string CreditRoot { get; set; }
    }
}
