using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.UI;
using Niva.Erp.Conta.TranzactiiDto.Dto;
using Niva.Erp.Models.Conta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Niva.Erp.Conta.TranzactiiFonduriAppService
{

    public interface ITranzactiiFonduriAppService : IApplicationService
    {
        List<TranzactiiFonduriDto> TranzactiiFonduriList(DateTime _dataEnd, DateTime _dataStart, string _debit, string _credit, string _explicatie);
    }



    public class TranzactiiFonduriAppService : ErpAppServiceBase , ITranzactiiFonduriAppService
    {
        IRepository<TranzactiiFonduri> _trazactiiFonduriRepository { get; set; }

        public TranzactiiFonduriAppService(IRepository<TranzactiiFonduri> trazactiiFonduriRepository)
        {
            _trazactiiFonduriRepository = trazactiiFonduriRepository;
        }


        public List<TranzactiiFonduriDto> TranzactiiFonduriList(DateTime _dataEnd, DateTime _dataStart, string _debit, string _credit, string _explicatie)
        {
            try
            {
                var ret = _trazactiiFonduriRepository.GetAll().ToList().Where( i => i.Data <= _dataEnd && i.Data >= _dataStart); 

                if(!String.IsNullOrEmpty(_debit))
                {
                    ret = ret.Where(i => i.Debit.IndexOf(_debit) >= 0);
                }

                if(!String.IsNullOrEmpty(_credit))
                {
                    ret = ret.Where(i => i.Credit.IndexOf(_credit) >= 0);
                }

                if (!String.IsNullOrEmpty(_explicatie))
                {
                    ret = ret.Where(i => i.Explicatie.Contains(_explicatie));
                }


                return ObjectMapper.Map<List<TranzactiiFonduriDto>>(ret).ToList();
            }
            catch (Exception e)
            {
                throw new UserFriendlyException("Eroare");
            }
            

        }





    }
}
