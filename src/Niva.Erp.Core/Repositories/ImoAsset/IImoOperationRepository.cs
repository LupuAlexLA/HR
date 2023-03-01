using Abp.Domain.Repositories;
using Niva.Erp.Models.ImoAsset;
using System;
using System.Collections.Generic;
using System.Text;

namespace Niva.Erp.Repositories.ImoAsset
{
    public interface IImoOperationRepository : IRepository<ImoAssetOper, int>
    {
        DateTime UnprocessedDate();

        DateTime LastProcessedDate();

        DateTime LastProcessedDateAdd();

        void GestAssetComputing(DateTime operationDate);

        void GestAssetDelComputing(DateTime operationDate);

        void UpdateAssetOperation(ImoAssetOper operation);

        ImoAssetStock GetGestDetailForAsset(int assetId, DateTime gestDate);

        int NextDocumentNumber(int documentTypeId);

        // Calcul gestiune
        DateTime LastProcessedDateForAsset(int imoAssetId);

        void GestComputingForAsset(DateTime operationDate, int imoAssetId);
        void GestImoAssetsComputing(DateTime operationDate);
        void GestDelComputingForAsset(DateTime operationDate, int imoAssetId);

        List<ImoAssetOperDetail> GetImoAssetOperDetails(int imoAssetId, int appClientId);
    }
}
