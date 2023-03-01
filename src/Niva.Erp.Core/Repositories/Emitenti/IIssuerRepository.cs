using Abp.Domain.Repositories;
using Niva.Erp.Models.Emitenti;

namespace Niva.Erp.Repositories.Emitenti
{
    public interface IIssuerRepository: IRepository<Issuer, int>
    {
        void IssuerInsertOrUpdate(Issuer issuerObj);
    }
}
