using Abp.Application.Services;
using Abp.UI;
using Niva.Erp.Buget.Dto;
using Niva.Erp.EntityFrameworkCore.Methods;
using Niva.Erp.Models.Buget;
using Niva.Erp.Repositories.Buget;
using Niva.Erp.Repositories.Conta.Nomenclatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niva.Erp.Buget
{
    public interface INotificareAppService : IApplicationService
    {
        List<NotificareDto> GetNotificareList();
        void UpdateStareNotificare(int notificareId);   
    }
    public class NotificareAppService : ErpAppServiceBase, INotificareAppService
    {
        private IBVC_Notificare _bvcNotificare;
        IPersonRepository _personRepository;

        public NotificareAppService(IBVC_Notificare bvcNotificare, IPersonRepository personRepository)
        {
            _bvcNotificare = bvcNotificare;
            _personRepository = personRepository;
        }
        public List<NotificareDto> GetNotificareList()
        {
            long? userId = AbpSession.UserId;
            var dept = _personRepository.GetUserDeptId(userId.Value);
            var notificareListDto = new List<NotificareDto>();

            if (dept != null)
            {
                var notificareList = _bvcNotificare.GetAll().Where(f => f.DepartamentId == dept.Id && f.StareNotificare == StareNotificare.Necitita).ToList();
                notificareListDto = ObjectMapper.Map<List<NotificareDto>>(notificareList);
            }
            return notificareListDto;
        }

        public void UpdateStareNotificare(int notificareId)
        {
            try
            {
                var notificare = _bvcNotificare.FirstOrDefault(f => f.Id == notificareId);
                notificare.StareNotificare = StareNotificare.Citita;
                notificare.DataVizualizare = LazyMethods.Now();
                CurrentUnitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Eroare", ex.Message);
            }
        }
    }
}
