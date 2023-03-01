using Abp.EntityFrameworkCore;
using Niva.Erp.Models.Emitenti;
using Niva.Erp.Repositories.Emitenti;
using System.Linq;

namespace Niva.Erp.EntityFrameworkCore.Repositories.Emitenti
{
    public class IssuerRepository : ErpRepositoryBase<Issuer, int>, IIssuerRepository
    {
        public IssuerRepository(IDbContextProvider<ErpDbContext> dbContextProvider) : base(dbContextProvider)
        {

        }

        public void IssuerInsertOrUpdate(Issuer issuerObj)
        {
            var count = Context.Issuer.Count(f => f.LegalPersonId == issuerObj.LegalPersonId);
            Issuer issuer;
            if (count == 0)
            {
                issuer = new Issuer
                {
                    Bic = issuerObj.Bic,
                    IbanAbrv = issuerObj.IbanAbrv,
                    IssuerType = issuerObj.IssuerType,
                    LegalPersonId = issuerObj.LegalPersonId,
                    BNR_SectorId = issuerObj.BNR_SectorId
                };
                Context.Issuer.Add(issuer);
            }else
            {
                issuer = Context.Issuer.FirstOrDefault(f => f.LegalPersonId == issuerObj.LegalPersonId);
                issuer.Bic = issuerObj.Bic;
                issuer.IbanAbrv = issuerObj.IbanAbrv;
                issuer.IssuerType = issuerObj.IssuerType;
                issuer.BNR_SectorId = issuerObj.BNR_SectorId;
            }
            Context.SaveChanges();
        }
    }
}
