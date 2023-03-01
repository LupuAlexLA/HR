
namespace Niva.Erp.Models.ImoAsset
{
    using Abp.Domain.Entities;
    using Abp.Domain.Entities.Auditing;
    using System.ComponentModel;

    public enum ImoAssetOperType : int
    {
        Reevaluare,
        [Description("Intrare in conservare")]
        IntrareInConservare,
        [Description("Iesire din conservare")]
        IesireDinConservare,
        Modernizare,
        Casare,
        Vanzare,
        [Description("Punere in functiune")]
        PunereInFunctiune,
        [Description("Bon miscare")]
        BonMiscare,
        Transfer,
        [Description("Amortizare lunara")]
        AmortizareLunara,
        Intrare,
        Modificare,
        Iesire,
        [Description("Modificare conturi fara inregistrare nota contabila")]
        ModificareConturiFaraInregistrareNotaContabila,
        [Description("Modificare conturi cu inregistrare nota contabila")]
        ModificareConturiCuInregistrareNotaContabila
    }

    public class ImoAssetOperForType : AuditedEntity<int>, IMustHaveTenant
    {
        public ImoAssetType ImoAssetType { get; set; }

        public ImoAssetOperType ImoAssetOperType { get; set; }


        public int TenantId { get; set; }
    }
}
