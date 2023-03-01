using AutoMapper;
using Niva.Erp.Conta.Nomenclatures;
using Niva.Erp.Models.Conta;

namespace Niva.Conta.Nomenclatures
{
    public class DocumentTypeListDDDto
    {
        public int Id { get; set; }

        public virtual string TypeName { get; set; }

        public virtual string TypeNameShort { get; set; }

        public virtual string Name { get; set; }
    }

    public class DocumentTypeEditDto
    {
        public int Id { get; set; }

        public virtual string TypeName { get; set; }

        public virtual string TypeNameShort { get; set; }

        public bool Editable { get; set; }

        public bool AutoNumber { get; set; }

        public bool ClosingMonth { get; set; }

        public int AppClientId { get; set; }
    }

    public class DocumentTypeMapProfile: Profile
    {
        public DocumentTypeMapProfile()
        {
            CreateMap<DocumentTypeEditDto, DocumentType>()
                .ForMember(t => t.TenantId, opts => opts.MapFrom(d => d.AppClientId));
            CreateMap<DocumentType, DocumentTypeEditDto>()
                .ForMember(t => t.AppClientId, opts => opts.MapFrom(d => d.TenantId));

            CreateMap<DocumentTypeListDDDto, DocumentType>();
            CreateMap<DocumentType, DocumentTypeListDDDto>()
                .ForMember(t => t.Name, opts => opts.MapFrom(d => d.TypeName));
        }
    }

}
