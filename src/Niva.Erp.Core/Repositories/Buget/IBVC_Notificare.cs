using Abp.Domain.Repositories;
using Niva.Erp.Models.Buget;
using System.Collections.Generic;

namespace Niva.Erp.Repositories.Buget
{
    public interface IBVC_Notificare : IRepository<Notificare, int>
    {
        void SaveNotificarePaapPrimeste(BVC_PaapRedistribuire paapRedistribuire);
        void SaveNotificarePaapPierde(BVC_PaapRedistribuireDetalii paapRedistribuireDetaliu);
    }
}
