using Abp.EntityFrameworkCore;
using Niva.Erp.Models.ImoAsset;
using Niva.Erp.Repositories.ImoAsset;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Niva.Erp.EntityFrameworkCore.Repositories.ImoAsset
{
    public class ImoAssetRepository : ErpRepositoryBase<ImoAssetItem, int>, IImoAssetRepository
    {
        public ImoAssetRepository(IDbContextProvider<ErpDbContext> context) : base(context)
        {

        }

        public IQueryable<ImoAssetItem> GetAssets()
        {
            throw new NotImplementedException();
        }

        public void UpdateAsset(ImoAssetItem asset)
        {
            var _assetDb = Context.ImoAssetItem.FirstOrDefault(f => f.Id == asset.Id);
            Context.Entry(_assetDb).CurrentValues.SetValues(asset);
            Context.SaveChanges();
        }
    }
}
