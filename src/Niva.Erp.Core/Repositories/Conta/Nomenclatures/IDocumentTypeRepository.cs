using Abp.Domain.Repositories;
using Niva.Erp.Models.Conta;
using System.Linq;

namespace Niva.Erp.Repositories.Conta.Nomenclatures
{
    public interface IDocumentTypeRepository : IRepository<DocumentType, int>
    {
        DocumentType GetDocumentTypeByShortName(string documentTypeShortName);
        IQueryable<DocumentType> DocumentTypeList();
        DocumentType GetDocumentTypeById(int? documentTypeId);
    }
}