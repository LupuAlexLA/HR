using System;
using System.Collections.Generic;
using System.Text;

namespace Niva.Erp.Conta.Prepayments
{
    public class PrepaymentsDurationSetupDto
    {
        public int Id { get; set; }

        public int PrepaymentTypeId { get; set; }

        public int PrepaymentDurationCalcId { get; set; }
    }

    public class PrepaymentsDecDeprecSetupDto
    {
        public int Id { get; set; }

        public int PrepaymentTypeId { get; set; }

        public int DecimalAmort { get; set; }
    }
}
