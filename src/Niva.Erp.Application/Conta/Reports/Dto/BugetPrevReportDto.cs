using Niva.Erp.Buget.Dto;
using System;
using System.Collections.Generic;

namespace Niva.Erp.Conta.Reports.Dto
{
    public class BugetPrevReportDto
    {
        public string AppClientName { get; set; }
        public string DepartmentName { get; set; }
        public List<BugePrevAllDepartmentByMonth> BugetPrevAllDepMonths { get; set; }
        public List<BugePrevDepartmentByMonth> BugetPrevDepMonths { get; set; }
    }

    public class BugePrevDepartmentByMonth
    {
        public DateTime MonthName { get; set; }
        public int Id { get; set; }
        public string Description { get; set; }
        public string ActivityType { get; set; }
        public decimal ValueActivity { get; set; }
        public int OrderView { get; set; }
        public decimal Value { get; set; }
    }

    public class BugePrevAllDepartmentByMonth
    {
        public DateTime MonthName { get; set; }
        public int Id { get; set; }
        public string Description { get; set; }
        public decimal Value { get; set; }
        public string ActivityType { get; set; }
        public decimal ValueActivity { get; set; }
        public int OrderView { get; set; }
    }

    public class BVC_PrevResurseModel
    {
        public int An { get; set; }
        public string AppClientId1 { get; set; }
        public string AppClientId2 { get; set; }
        public string AppClientName { get; set; }
        public List<BVC_PrevResurseDto> PrevResurse { get; set; }
    }
}