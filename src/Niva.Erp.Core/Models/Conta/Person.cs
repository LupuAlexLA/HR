using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Niva.Erp.Authorization.Users;
using Niva.Erp.Models.Emitenti;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Niva.Erp.Models.Conta
{
    public abstract class Person : AuditedEntity<int>, IMustHaveTenant //poate MayHaveTenant
    {
        [StringLength(1000)]
        public virtual string Id1 { get; set; }

        [StringLength(1000)]
        public virtual string Id2 { get; set; }

        [StringLength(1000)]
        public virtual string AddressStreet { get; set; }

        [StringLength(1000)]
        public virtual string AddressNo { get; set; }

        [StringLength(1000)]
        public virtual string AddressBlock { get; set; }

        [StringLength(1000)]
        public virtual string AddressFloor { get; set; }

        [StringLength(1000)]
        public virtual string AddressApartment { get; set; }

        [StringLength(1000)]
        public virtual string AddressZipCode { get; set; }

        [StringLength(1000)]
        public virtual string AddressLocality { get; set; }

        [ForeignKey("AddressRegion")]
        public int? AddressRegionId { get; set; }
        public virtual Region AddressRegion { get; set; }

        [ForeignKey("AddressCountry")]
        public int? AddressCountryId { get; set; }
        public virtual Country AddressCountry { get; set; }
        public virtual string FullName { get; }
        public virtual string FullNameLastFirst { get; }

        public virtual IList<BankAccount> BankAccount { get; set; }

        public bool IsEmployee { get; set; }

        public int? IdPersonal { get; set; }
        public int? IdBanci { get; set; }
        public int TenantId { get; set; }
    }
    public partial class NaturalPerson : Person
    {
        public virtual string FirstName
        {
            get;
            set;
        }

        public virtual string LastName
        {
            get;
            set;
        }

        public virtual User User
        {
            get;
            set;
        }
        //TODO uncomment
        //public virtual UserRegistrationRequest UserRegistrationRequest
        //{
        //    get;
        //    set;
        //}
        public override string FullName
        {
            get { return FirstName + " " + LastName; }
        }

        public override string FullNameLastFirst
        {
            get { return LastName + " " + FirstName; }
        }

    }

    public partial class LegalPerson : Person
    {
        public virtual string Name
        {
            get;
            set;
        }

        public virtual Issuer Bank
        {
            get;
            set;
        }
        public override string FullName
        {
            get { return Name; }
        }

        public override string FullNameLastFirst
        {
            get { return Name; }
        }

        public bool IsVATPayer { get; set; }
        public DateTime? StartDateVATPayment { get; set; }

        public bool IsVATCollector { get; set; }
        public DateTime? VATCollectedStartDate { get; set; }
    }
}