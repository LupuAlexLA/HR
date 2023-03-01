using Niva.Erp.Models.InvObjects;
using System;
using System.Collections.Generic;

namespace Niva.Erp.Conta.InvObjects.Dto
{

    public class InventoryDDList
    {
        public int Id { get; set; }
        public string InvObjectDate { get; set; }
    }
    public class InvObjectOperListDto
    {
        public DateTime DataStart { get; set; }

        public DateTime DataEnd { get; set; }

        public List<InvObjectOperListDetailDto> ListDetail { get; set; }
    }

    public class InvObjectOperListDetailDto
    {
        public int Id { get; set; }

        public string OperationType { get; set; }
        public int OperationTypeId { get; set; }

        public DateTime OperationDate { get; set; }

        public string DocumentType { get; set; }

        public int DocumentNr { get; set; }

        public DateTime DocumentDate { get; set; }

        public bool Processed { get; set; }

        public bool ShowReportBtn
        {
            get
            {
                if (OperationTypeId == (int)InvObjectOperType.BonMiscare || OperationTypeId == (int)InvObjectOperType.Transfer || OperationTypeId == (int)InvObjectOperType.DareInConsum)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }


    public class InvObjectOperEditDto
    {
        public int Id { get; set; }

        public int? OperationTypeId { get; set; }

        public string OperationType { get; set; }

        public DateTime OperationDate { get; set; }

        public string OperationDateStr { get; set; }

        public int DocumentNr { get; set; }

        public DateTime DocumentDate { get; set; }

        public string DocumentDateStr { get; set; }

        public int? DocumentTypeId { get; set; }

        public string DocumentType { get; set; }

        public InvObjectOperType InvObjectOperType { get; set; }

        public int? InvObjectsStoreInId { get; set; }

        public string InvObjectsStoreIn { get; set; }

        public int? InvObjectsStoreOutId { get; set; }

        public string InvObjectsStoreOut { get; set; }

        public int? PersonStoreInId { get; set; }
        public string PersonStoreInName { get; set; }

        public int? PersonStoreOutId { get; set; }
        public string PersonStoreOutName { get; set; }

        public int? InvoiceId { get; set; }

        public bool ShowForm1 { get; set; }

        public bool ShowForm2 { get; set; }

        public bool ShowForm3 { get; set; }

        public bool FinishAdd { get; set; }

        public bool ShowStorage { get; set; }

        public bool ShowValues { get; set; }

        public bool ShowModifValues { get; set; } // la true afisez doar campurile cu diferenta de valoare, la false afisez valorile intregi si valorile modif sunt disabled

        public List<InvObjectOperDetailEditDto> Details { get; set; }
    }

    public class InvObjectOperDetailEditDto
    {
        public int Id { get; set; }

        public int? InvObjectItemId { get; set; }
        public int SelectedInvObjectItemId { get; set; }
        public string SelectedInvObjectItem { get; set; }   
        public bool IsSelectedInvObjectItem { get; set; }

        public string InvObjectItem { get; set; }

        public int Quantity { get; set; }
        public Decimal InvValueOld { get; set; }

        public Decimal FiscalValueOld { get; set; }

        public Decimal InvValueModif { get; set; }


        public int IdOrd { get; set; }

        public int? InvoiceDetailId { get; set; }

    }
}
