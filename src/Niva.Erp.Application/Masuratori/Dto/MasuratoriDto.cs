using Niva.Erp.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Niva.Erp.Masuratori.Dto
{
    public class MasuratoriDto
    {
        public int Id { get; set; }
        public string Denumire { get; set; }
        public int TaraId { get; set; }
        public string TaraString { get; set; }
        public int JudetId { get; set; }
        public string JudetString { get; set; }
        public string Localitate { get; set; }
        public string Adresa { get; set; }
        public decimal Peak { get; set; }
        public decimal Average { get; set; }
        public decimal Latitudine { get; set; }
        public decimal Longitudine { get; set; }
        public Stare Stare { get; set; }
        public long OwnerId { get; set; }
        public string OwnerName { get; set; }
        public TipUtilizator? TipUtilizator { get; set; }
    }
}
