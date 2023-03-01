using Abp.Domain.Repositories;
using Niva.Erp.Models.ImoAsset;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Niva.Erp.Repositories.ImoAsset
{
    public interface IImoAssetRepository : IRepository<ImoAssetItem, int>
    {
        IQueryable<ImoAssetItem> GetAssets();

        void UpdateAsset(ImoAssetItem asset);
    }
}
