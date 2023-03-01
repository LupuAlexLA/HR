using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using Niva.Conta.Nomenclatures;
using Niva.Erp.Models.Conta;
using Niva.Erp.Repositories.Conta.AutoOperation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Niva.Erp.Conta.Nomenclatures
{
    public interface IDocumentTypeAppService : IApplicationService
    {
        GetDocumentTypeOutput DocumentTypeList();
        GetDocumentTypeOutput DocumentTypeListByStr(string documentName);
        void SaveDocumentType(DocumentTypeEditDto doc);
        DocumentTypeEditDto GetDocTypeById(int id);
        void DeleteDocumentType(int idDoc);
        int GetDocumentTypeId(string documentShortName);
        string NextDocumentNumber(DocumentTypeEditDto documentType, DateTime operationDate);
    }

    public class GetDocumentTypeOutput
    {
        public List<DocumentTypeListDDDto> GetDocumentType { get; set; }
    }

    public class DocumentTypeAppService : ErpAppServiceBase, IDocumentTypeAppService
    {
        IRepository<DocumentType> _documentTypeRepository;
        IAutoOperationRepository _autoOperationRepository;

        public DocumentTypeAppService(IRepository<DocumentType> documentTypeRepository, IAutoOperationRepository autoOperationRepository)
        {
            _documentTypeRepository = documentTypeRepository;
            _autoOperationRepository = autoOperationRepository;
        }
      
        //[AbpAuthorize("Admin.Conta.TipDoc.Acces")]
        public GetDocumentTypeOutput DocumentTypeList()
        {

            var _documentTypes = _documentTypeRepository.GetAll()

                                 .ToList()
                                 .OrderBy(f => f.TypeName);

            var ret = new GetDocumentTypeOutput { GetDocumentType = ObjectMapper.Map<List<DocumentTypeListDDDto>>(_documentTypes) };
            return ret;
        }
     
        //[AbpAuthorize("Admin.Conta.TipDoc.Acces")]
        public GetDocumentTypeOutput DocumentTypeListByStr(string documentName)
        {
            var ret = new GetDocumentTypeOutput { GetDocumentType = null };

            if (documentName.Length >= 2)
            {

                var _documentTypes = _documentTypeRepository.GetAll()
                                     .ToList()
                                     .Where(f => (f.TypeName + f.TypeNameShort).ToUpper().IndexOf(documentName.ToUpper()) >= 0)
                                     .OrderBy(f => f.TypeName);

                ret = new GetDocumentTypeOutput { GetDocumentType = ObjectMapper.Map<List<DocumentTypeListDDDto>>(_documentTypes) };
            }
            return ret;
        }
   
        //[AbpAuthorize("Admin.Conta.TipDoc.Acces")]
        public DocumentTypeEditDto GetDocTypeById(int id)
        {
            var _doc = _documentTypeRepository.FirstOrDefault(f => f.Id == id);
            var ret = ObjectMapper.Map<DocumentTypeEditDto>(_doc);
            return ret;
        }
      
        //[AbpAuthorize("Admin.Conta.TipDoc.Acces")]
        public void SaveDocumentType(DocumentTypeEditDto doc)
        {
            try
            {
                // get current tenant
                var appClient = GetCurrentTenant();
                var _doc = ObjectMapper.Map<DocumentType>(doc);


                if (_doc.Id == 0)
                {
                    _doc.TenantId = appClient.Id;
                    _doc.Editable = true;
                    _documentTypeRepository.Insert(_doc);
                }
                else
                {
                    _documentTypeRepository.Update(_doc);
                }

            }
            catch (Exception ex)
            {
                throw new Abp.UI.UserFriendlyException("Eroare", ex.Message);
            }
        }
    
        //[AbpAuthorize("Admin.Conta.TipDoc.Acces")]
        public void DeleteDocumentType(int idDoc)
        {
            var _doc = _documentTypeRepository.FirstOrDefault(f => f.Id == idDoc);
            _documentTypeRepository.Delete(_doc);
        }
        
        //[AbpAuthorize("Admin.Conta.TipDoc.Acces")]
        public int GetDocumentTypeId(string documentShortName)
        {
            var _docId = _documentTypeRepository.GetAll().FirstOrDefault(f => f.TypeNameShort == documentShortName).Id;
            return _docId;
        }

        public string NextDocumentNumber(DocumentTypeEditDto documentType, DateTime operationDate)  
        {
            try
            {
                var docType = ObjectMapper.Map<DocumentType>(documentType);
                var nextDocNumber = _autoOperationRepository.GetDocumentNextNumber(docType, operationDate);
                return nextDocNumber;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }
    }
}