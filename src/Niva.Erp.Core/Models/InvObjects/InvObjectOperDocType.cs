using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Niva.Erp.Models.Conta;
using Niva.Erp.Models.Conta.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Niva.Erp.Models.InvObjects
{
    public class InvObjectOperDocType : AuditedEntity<int>, IMustHaveTenant
    {
        public InvObjectOperType OperType { get; set; }
        [ForeignKey("DocumentType")]
        public int DocumentTypeId { get; set; }
        public DocumentType DocumentType { get; set; }
        public bool AppOperation { get; set; }
        public int TenantId { get; set; }
    }
}
