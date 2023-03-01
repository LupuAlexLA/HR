using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niva.Erp.Buget.Dto
{
    public class NotificareDto
    {
        public int NotificareId { get; set; }
        public virtual int? DepartamentId { get; set; }
        public virtual int? IdPersonal { get; set; }
        public int StareNotificareId { get; set; }
        public string Mesaj { get; set; }
        public int UserVizualizareId { get; set; }  
        public DateTime DataVizualizare { get; set; }
        public int TenantId { get; set; }
    }
}
