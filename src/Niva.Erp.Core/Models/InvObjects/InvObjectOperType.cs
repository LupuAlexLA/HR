using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System.ComponentModel;

namespace Niva.Erp.Models.InvObjects
{
    public enum InvObjectOperType : int
    {
        Reevaluare,
        Modernizare,
        Casare,
        Vanzare,
        [Description("Bon miscare")]
        BonMiscare,
        Transfer,
        Intrare,
        Modificare,
        Iesire,
        [Description("Dare in consum")]
        DareInConsum
    }

}