using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Microsoft.Extensions.Configuration;
using Niva.Erp.Models.Filedoc;
using Niva.Erp.Repositories.Economic;
using System;
using System.Collections.Generic;
using System.Linq; 
using System.Threading.Tasks; 

namespace Niva.Erp.ExternalApi
{
    public class Emitent
    {
        public string Denumire { get; set; }
        public string Cui { get; set; }
    }

    public class AtasamentDTO
    {
        public string Denumire { get; set; }
        public int Id { get; set; }
    }

    public class AtasamentFileDTO
    {
        public byte[] Content { get; set; }
        public string FileName { get; set; }
    }

    public class FileDocErrorDto : EntityDto<int> 
    {
        public int DocumentId { get; set; }
        public string DocumentNr { get; set; }
        public string MesajEroare { get; set; }
        public bool Rezolvat { get; set; }
        public DateTime LastErrorDate { get; set; }
        public DateTime? RezolvatDate { get; set; }
    }

    public interface IFileDocExternAppService : IApplicationService
    {
        Task<List<AtasamentDTO>> GetAtasamenteAsync(int documentId);
        Task<AtasamentFileDTO> GetAtasamentFileAsync(int attachementId);
        List<FileDocErrorDto> GetImportErrors();
        string GetFileDocViewUrl();
    }
    public class FileDocExternAppService : ErpAppServiceBase, IFileDocExternAppService
    {
        private readonly IConfiguration _configuration;
        private readonly FileDocApi fileDocApi;
        private readonly IRepository<FileDocError> fileDocErrorRepository;
        IInvoiceRepository invoiceRepository;

        string apiUrl;
        string viewUrl;
        public FileDocExternAppService(IConfiguration  configuration, IInvoiceRepository invoiceRepository, FileDocApi fileDocApi, IRepository<FileDocError> fileDocErrorRepository) : base()
        {
            apiUrl = configuration["App:FileDocApiUrl"];
            viewUrl = configuration["App:FileDocUrl"];
            this.invoiceRepository = invoiceRepository;
            this.fileDocErrorRepository = fileDocErrorRepository;
            this.fileDocApi = fileDocApi;
        }
        public async Task<List<AtasamentDTO>> GetAtasamenteAsync(int documentId)
        {
            var res = await fileDocApi.GetAtasamente(documentId);
            return res.Select(s => new AtasamentDTO { Denumire = s.numeAtasament, Id = s.ID }).ToList();

        }

        public async Task<AtasamentFileDTO> GetAtasamentFileAsync(int attachementId)
        {
            var res = await fileDocApi.DownloadAtasament(attachementId);
            var ret = new AtasamentFileDTO();
            ret.FileName = res.numefisier;
            ret.Content = res.continut;
            return ret;
        }

        public string GetFileDocViewUrl()
        {
            return viewUrl;
        }

        public List<FileDocErrorDto> GetImportErrors()
        {
            var res = fileDocErrorRepository.GetAll().Where(w => w.Rezolvat == false).OrderByDescending(o => o.LastErrorDate);
            var ret = ObjectMapper.Map<List<FileDocErrorDto>>(res);
            return ret;
        }
    }
     
}
