using System.ComponentModel.DataAnnotations;

namespace Niva.Erp.Conta.InvObjects.Dto
{
    public class InvObjectStorageDto
    {
        public int Id { get; set; }

        [StringLength(1000)]
        public string StorageName { get; set; }

        public bool CentralStorage { get; set; }

        public int TenantId { get; set; }
    }

    public class InvObjectStorageAddDto
    {
        public int Id { get; set; }

        [StringLength(1000)]
        public string Name { get; set; }
    }
}
