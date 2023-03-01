using Abp.Application.Services;
using Abp.Domain.Repositories;
using Niva.Erp.Imprumuturi.Dto;
using Niva.Erp.Managers.Imprumut;
using Niva.Erp.Models.Conta;
using Niva.Erp.Models.Imprumuturi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Niva.Erp.Imprumuturi
{

    public interface IOperatieComisionDobandaAppService : IApplicationService
    {
        OperatieDobandaComisionDtoList OperatieComisionDobandaByImprumutIdList(int ImprumutId);
        void OperatieComisionDobandaSave(OperatieDobandaComisionDto OperatieDto);
        void OperatieDeleteId(int OperatieId);
    }

    public class OperatieComisionDobandaAppService : ErpAppServiceBase, IOperatieComisionDobandaAppService
    {

        IRepository<OperatieDobandaComision> _OperatieDobandaComisionRepository;
        IRepository<Imprumut> _ImprumutRepository;
        IRepository<ComisionV2> _ComisionRepository;
        IRepository<Operation> _NotaContabilaRepository;
        IRepository<OperationDetails> _NotaContabilaDetailRepository;
        ImprumutOperationManager _imprumutOperationManager;

        public OperatieComisionDobandaAppService(IRepository<Imprumut> ImprumutRepository, IRepository<OperationDetails> NotaContabilaDetailRepository, IRepository<ComisionV2> ComisionRepository, IRepository<Operation> NotaContabilaRepository ,ImprumutOperationManager imprumutOperationManager,IRepository<OperatieDobandaComision> OperatieDobandaComisionRepository)
        {
            _ImprumutRepository = ImprumutRepository;
            _NotaContabilaDetailRepository = NotaContabilaDetailRepository;
            _ComisionRepository = ComisionRepository;
            _NotaContabilaRepository = NotaContabilaRepository;
            _imprumutOperationManager = imprumutOperationManager;
            _OperatieDobandaComisionRepository = OperatieDobandaComisionRepository;
        }
    
        public OperatieDobandaComisionDtoList OperatieComisionDobandaByImprumutIdList(int ImprumutId)
        {

            var operatieDobandaComision = _OperatieDobandaComisionRepository.GetAll().Where(f => f.ImprumutId == ImprumutId && f.State == Models.Conta.Enums.State.Active).OrderBy(f => f.Id).ToList();
            var operatieComision = operatieDobandaComision.Where(f => f.TipOperatieDobandaComision == TipOperatieDobandaComision.comision && f.ImprumutId == ImprumutId && f.State == Models.Conta.Enums.State.Active).OrderBy(f => f.Id).ToList();
            var operatieDobanda = operatieDobandaComision.Where(f => f.TipOperatieDobandaComision == TipOperatieDobandaComision.dobanda && f.ImprumutId == ImprumutId && f.State == Models.Conta.Enums.State.Active).OrderBy(f => f.Id).ToList();
            

            return new OperatieDobandaComisionDtoList { OperatieComision = ObjectMapper.Map<List<OperatieDobandaComisionDto>>(operatieComision),
                OperatieDobanda = ObjectMapper.Map<List<OperatieDobandaComisionDto>>(operatieDobanda),
                OperatieDobandaComision = ObjectMapper.Map<List<OperatieDobandaComisionDto>>(operatieDobandaComision)
            };
        }

        public void OperatieComisionDobandaSave(OperatieDobandaComisionDto OperatieDto)
        {
            int localCurrencyId = GetCurrentTenant().LocalCurrencyId.Value;
            var Operatie = ObjectMapper.Map<OperatieDobandaComision>(OperatieDto);

            if (Operatie.Id != 0)
            {
                _OperatieDobandaComisionRepository.Update(Operatie);
                CurrentUnitOfWork.SaveChanges();
            }
            else
            {

                _OperatieDobandaComisionRepository.Insert(Operatie);
                CurrentUnitOfWork.SaveChanges();

                var imprumut = _ImprumutRepository.Get((int)Operatie.ImprumutId);

                var comisionCheck = _ComisionRepository.GetAll().Where(f => f.TipComision == TipComisionV2.Periodic && f.State == Models.Conta.Enums.State.Active).Count();



                // se verifica in plus tipul de operatie (probabil ramas de la o implementare veche) deoarece functia OperatieToConta/OperatieToContaCredit verifica in interior tipul de operatie
                // de scos in viitor

                if (comisionCheck > 0 && Operatie.TipOperatieDobandaComision == TipOperatieDobandaComision.comision)
                {
                    var operId = 0;

                    if (imprumut.TipCreditare == TipCreditare.LinieDeCredit)
                    {
                        operId = _imprumutOperationManager.OperatieToConta(Operatie, localCurrencyId, OperatieDto.OperGenerateId);
                    }
                    else
                    {
                        operId = _imprumutOperationManager.OperatieToContaCredit(Operatie, localCurrencyId, OperatieDto.OperGenerateId);
                    }

                    var nota = _NotaContabilaRepository.Get(operId);
                    var notaDetalii = _NotaContabilaDetailRepository.GetAll().Where(f => f.OperationId == operId);

                    foreach (var detaliu in notaDetalii)
                    {
                        if (detaliu.Value == 0)
                        {
                            nota.State = Models.Conta.Enums.State.Inactive;
                        }
                    }

                    Operatie.OperationId = operId;
                }

                if(Operatie.TipOperatieDobandaComision == TipOperatieDobandaComision.dobanda)
                {
                    int operId = 0;

                    if (imprumut.TipCreditare == TipCreditare.LinieDeCredit)
                    {
                        operId = _imprumutOperationManager.OperatieToConta(Operatie, localCurrencyId, OperatieDto.OperGenerateId);
                    }
                    else if (imprumut.TipCreditare == TipCreditare.Credit)
                    {
                        operId = _imprumutOperationManager.OperatieToContaCredit(Operatie, localCurrencyId, OperatieDto.OperGenerateId);
                    }
                    
                    
                    var nota = _NotaContabilaRepository.Get(operId);
                    var notaDetalii = _NotaContabilaDetailRepository.GetAll().Where(f => f.OperationId == operId);

                    foreach(var detaliu in notaDetalii)
                    {
                        if(detaliu.Value == 0)
                        {
                            nota.State = Models.Conta.Enums.State.Inactive;
                        }
                    }
                    Operatie.OperationId = operId;
                }
                
                
            }

        }

        public void OperatieDeleteId(int OperatieId)
        {
            var operatie = _OperatieDobandaComisionRepository.Get(OperatieId);
            if (operatie.OperationId != null)
            {
                var nota = _NotaContabilaRepository.Get((int)operatie.OperationId);
                var notaChildrends = _NotaContabilaRepository.GetAll().Where(f => f.OperationParentId == nota.Id && f.State == Models.Conta.Enums.State.Active);

                foreach (var value in notaChildrends)
                {
                    value.State = Models.Conta.Enums.State.Inactive;
                }

                nota.State = Models.Conta.Enums.State.Inactive;                
            }
            operatie.State = Models.Conta.Enums.State.Inactive;
            CurrentUnitOfWork.SaveChanges();
        }


    }
}
