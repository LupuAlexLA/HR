using System;
using System.Collections.Generic;
using System.Text;

namespace Niva.Erp.Conta.AutoOperation
{
    public class GetAccountDto
    {
        public string Symbol { get; set; }

        public string AccountName { get; set; }
    }

    public class GetAccountListDto
    {
        public string SyntheticAccount { get; set; }

        public string Symbol { get; set; }
    }

    public class GetSoldAccounttDto
    {
        public string Message { get; set; }

        public decimal Sold { get; set; }
    }

    public class ResultMessageIdDto
    {
        public string Message { get; set; }

        public int? Id { get; set; }
    }

    public class ResultMessageValueDto
    {
        public string Message { get; set; }

        public decimal? Value { get; set; }
    }

    public class OperationAutoDirectDto
    {
        public string OperationDate { get; set; }
        public string DocumentNr { get; set; }
        public string DocumentDate { get; set; }
        public string Currency { get; set; }
        public string DocumentType { get; set; }

        public List<OperationAutoDetailDirectDto> Details { get; set; }
    }

    public class OperationAutoDetailDirectDto
    {
        public string Debit { get; set; }
        public string Credit { get; set; }
        public decimal Value { get; set; }
        public string Explicatii { get; set; }

    }

    public class OperationAutoDto
    {
        public string OperationDate { get; set; }
        public string DocumentNr { get; set; }
        public string DocumentDate { get; set; }
        public string Currency { get; set; }
        public string ActivityType { get; set; }
        public int OperType { get; set; }
        public int OperationType { get; set; }
        public List<OperationAutoDetailDto> Details { get; set; }
    }

    public class OperationAutoDetailDto
    {
        public int ValueType { get; set; }
        public decimal Value { get; set; }
        public string Bank { get; set; }
        public string Explicatii { get; set; }
        public bool Storno { get; set; }
    }

    public class ResultMessageStringDto
    {
        public string Message { get; set; }

        public string Value { get; set; }
    }

    public class ResultSitResurseDto
    {
        public decimal FGcuProfitCurent { get; set; }
        public decimal FRcuProfitCurent { get; set; }
        public string Message { get; set; }
    }
}
