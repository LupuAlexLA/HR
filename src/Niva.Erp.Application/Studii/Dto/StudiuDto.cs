using Niva.Erp.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Niva.Erp.Studii.Dto
{
    public class StudiuDto
    {
        public int Id { get; set; }
        public byte[] Poza { get; set; }
        public string Titlu { get; set; }
        public string Text { get; set; }
        public string Link { get; set; }
        public Stare Stare { get; set; }
        public long OwnerId { get; set; }
        public string OwnerName { get; set; }
        public TipUtilizator? TipUtilizator { get; set; }
    }
}
