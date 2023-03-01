using Abp.EntityFramework;
using Abp.EntityFrameworkCore;
using Niva.Erp.EntityFrameworkCore;
using Niva.Erp.EntityFrameworkCore.Repositories;
using Niva.Erp.Models.Conta;
using Niva.Erp.Repositories.Conta.Nomenclatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niva.EntityFramework.Repositories.Nomenclatures
{

    public class DocumentTypeRepository : ErpRepositoryBase<DocumentType, int>, IDocumentTypeRepository
    {
        public DocumentTypeRepository(IDbContextProvider<ErpDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public DocumentType GetDocumentTypeByShortName(string documentTypeShortName)
        {
            DocumentType documentType = new DocumentType();
            documentType = DocumentTypeList().FirstOrDefault(p => p.TypeNameShort == documentTypeShortName);
            return documentType;
        }

        public IQueryable<DocumentType> DocumentTypeList()
        { 
                var x = Context.DocumentType
                     
                    .OrderBy(f => f.TypeName);
                return x;
            
        }
        public DocumentType GetDocumentTypeById(int? documentTypeId)
        {
            DocumentType documentType = new DocumentType();
            documentType = Context.DocumentType.FirstOrDefault(p => p.Id == documentTypeId);
            return documentType;
        }
    }
}
