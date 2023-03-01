using System;
using System.Collections.Generic;
using System.Text;

namespace Niva.Erp.Models.Conta.Enums
{
    public enum State : int
    {
        Active,
        Inactive,
    }

    public enum InternalOperType : int
    {
        Insert,
        Update,
        Delete
    }
    public enum AccountTypes : int
    {
        Active,
        Passive,
        Bifunctional,
    }
   
    public enum TaxStatus : int
    {
        NA,
        Impozabil,
        Neimpozabil,
    }

    public enum TypeofAccount
    {
        Synthetic,
        Analytic
    }

    public enum InvOperationType : int
    {
        NIR,
        DareInConsum,
        Transfer
    }
}
