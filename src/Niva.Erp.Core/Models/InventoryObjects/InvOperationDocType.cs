﻿using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Niva.Erp.Models.Conta;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niva.Erp.Models.InventoryObjects
{
    public class InvOperationDocType : AuditedEntity<int>,  IMustHaveTenant
    {
        public InvOperationType OperationType { get; set; }

        [ForeignKey("DocumentType")]
        public int DocumentTypeId { get; set; }
        public DocumentType DocumentType { get; set; }

        public bool AppOperation { get; set; } // se foloseste din interfata  in modulul Operatiuni
        public int TenantId { get; set; }
    }
}
