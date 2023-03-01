using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Abp;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Domain.Uow;
using Niva.Erp.Models.Conta;

namespace Niva.Erp.Managers.Reporting
{
    //public class TestReportingItem
    //{
    //    public string Nume { get; set; }
    //    public string Prenume { get; set; }
    //}

    //public interface ITestReportManager :  IDomainService
    //{
    //    List<TestReportingItem> GetLegalPersons(int count);
    //}

    //public class TestReportManager : DomainService, ITestReportManager 
    //{
    //    IRepository<LegalPerson> personRepository { get; set; }
    //    public TestReportManager(IRepository<LegalPerson> legalPersonRepository)
    //    {
    //        personRepository = legalPersonRepository;
    //    }
    //    [UnitOfWork]
    //    public virtual List<TestReportingItem> GetLegalPersons(int count)
    //    {
    //        //personRepository = IocManager.Instance.Resolve<IRepository<LegalPerson>>();
    //        var ret = personRepository.GetAll().Select(s => new TestReportingItem { Nume = s.Name, Prenume = s.Id1 }).Take(count).ToList();
    //        return ret;
    //    }
    //}
}

