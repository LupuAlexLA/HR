using Abp.EntityFrameworkCore;
using Niva.Erp.Models.Buget;
using Niva.Erp.Repositories.Buget;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Niva.Erp.EntityFrameworkCore.Repositories.Buget
{
    public class BVC_Notificare : ErpRepositoryBase<Notificare, int>, IBVC_Notificare
    {
        public BVC_Notificare(IDbContextProvider<ErpDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }
        public void SaveNotificarePaapPierde(BVC_PaapRedistribuireDetalii paapRedistribuireDetaliu)
        {
            var appClientId = 1;
            
                var salariatDepartament = Context.SalariatiDepartamente
                    .FirstOrDefault(x => x.DepartamentId == paapRedistribuireDetaliu.PaapCarePierde.DepartamentId);
                var person = Context.Persons
                    .FirstOrDefault(x => x.Id == salariatDepartament.PersonId);
                var notificare = new Notificare()
                {
                    DepartamentId = salariatDepartament.DepartamentId,
                    IdPersonal = person.IdPersonal,
                    StareNotificare = StareNotificare.Necitita,
                    TenantId = appClientId,
                    Mesaj = $"Departamentului {paapRedistribuireDetaliu.PaapCarePierde.Departament.Name} i-a fost redistribuita suma de {paapRedistribuireDetaliu.SumaPierduta} lei de la achizitia {paapRedistribuireDetaliu.PaapCarePierde.Descriere}"
                };

                Context.Notificare.Add(notificare);
                Context.SaveChanges();
        }

        public void SaveNotificarePaapPrimeste(BVC_PaapRedistribuire paapRedistribuire)
        {
            var salariatDepartament = Context.SalariatiDepartamente
                    .FirstOrDefault(x => x.DepartamentId == paapRedistribuire.PaapCarePrimeste.DepartamentId);
            var person = Context.Persons.FirstOrDefault(x => x.Id == salariatDepartament.PersonId);
            var notificare = new Notificare()
            {
                DepartamentId = salariatDepartament.DepartamentId,
                IdPersonal = person.IdPersonal,
                StareNotificare = StareNotificare.Necitita,
                TenantId = paapRedistribuire.TenantId,
                Mesaj = $"Departamentului {paapRedistribuire.PaapCarePrimeste.Departament.Name} i se aloca suma de {paapRedistribuire.SumaPlatita} lei pentru achizitia {paapRedistribuire.PaapCarePrimeste.Descriere}"
            };

            Context.Notificare.Add(notificare);
            Context.SaveChanges();
        }
    }
}
