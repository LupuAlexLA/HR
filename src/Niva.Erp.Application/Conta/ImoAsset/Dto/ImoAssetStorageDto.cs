using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Niva.Erp.Conta.ImoAsset
{
    public class ImoAssetStorageDto
    {
        public int Id { get; set; }

        [StringLength(1000)]
        public string StorageName { get; set; }

        public bool CentralStorage { get; set; }

        public int AppClientId { get; set; }
    }

    public class ImoAssetStorageDDDto
    {
        public int Id { get; set; }

        [StringLength(1000)]
        public string Name { get; set; }

    }
}
