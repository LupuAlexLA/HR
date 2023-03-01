using Niva.Erp.Repositories.Buget;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Niva.Erp.Models.Buget.BVC
{
    public class BugetRaportare
    {
        public int AnBVC { get; set; }
        public string Titlu { get; set; }
        public string AppClientName { get; set; }
        public List<BugetRaportareDetails> RaportareDetails { get; set; }
    }

    
}
