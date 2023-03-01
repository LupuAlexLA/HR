using System.Threading.Tasks;
using Abp.Zero.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Niva.Erp.Authorization.Roles;
using Niva.Erp.Authorization.Users;
using Niva.Erp.EntityFrameworkCore.DbFunctions;
using Niva.Erp.Models.AutoOperation;
using Niva.Erp.Models.Buget;
using Niva.Erp.Models.Buget.BVC;
using Niva.Erp.Models.Conta;
using Niva.Erp.Models.Conta.ConfigurareRapoarte;
using Niva.Erp.Models.Conta.Lichiditate;
using Niva.Erp.Models.Conta.RegistruInventar;
using Niva.Erp.Models.Conta.SituatiiFinanciare;
using Niva.Erp.Models.Conta.TaxProfit;
using Niva.Erp.Models.Contracts;
using Niva.Erp.Models.Deconturi;
using Niva.Erp.Models.Economic;
using Niva.Erp.Models.Economic.Casierii;
using Niva.Erp.Models.Economic.Casierii.Cupiuri;
using Niva.Erp.Models.Emitenti;
using Niva.Erp.Models.Filedoc;
using Niva.Erp.Models.HR;
using Niva.Erp.Models.ImoAsset;
using Niva.Erp.Models.ImoAssets;
using Niva.Erp.Models.Imprumuturi;
using Niva.Erp.Models.InvObjects;
using Niva.Erp.Models.PrePayments;
using Niva.Erp.Models.SectoareBnr;
using Niva.Erp.Models.Setup;
using Niva.Erp.MultiTenancy;

namespace Niva.Erp.EntityFrameworkCore
{
    public class ErpDbContext : AbpZeroDbContext<Tenant, Role, User, ErpDbContext>
    {
        /* Define a DbSet for each entity of the application */

        public ErpDbContext(DbContextOptions<ErpDbContext> options)
            : base(options)
        {
        }

        public DbSet<OperatieGarantie> OperatieGarantie { get; set; }
        public DbSet<Dobanda> Dobanda { get; set; }
        public DbSet<ComisionV2> ComisionV2 { get; set; }
        public DbSet<OperatieDobandaComision> OperatieDobandaComision { get; set; }
        public DbSet<BVC_Cheltuieli> BVC_Cheltuieli { get; set; }
        public DbSet<TranzactiiFonduri> TranzactiiFonduri { get; set; }
        public DbSet<OperationDefinition> OperationDefinition { get; set; }
        public DbSet<OperationDefinitionDetails> OperationDefinitionDetails { get; set; }
        public DbSet<ImprumutState> ImprumutState { get; set; }
        public DbSet<Tragere> Tragere { get; set; }
        public DbSet<Contracts_State> Contracts_State { get; set; }
        public DbSet<DataComision> DateComision { get; set; }
        public DbSet<ExchangeRateForecast> ExchangeRateForecasts { get; set; }
        public DbSet<Comision> Comisioane { get; set; }
        public DbSet<DateDobanziReferinta> DateDobanziReferinta { get; set; }
        public DbSet<DobanziReferinta> DobanziReferinta { get; set; }
        public DbSet<Imprumut> Imprumuturi { get; set; }
        public DbSet<Garantie> Garantie { get; set; }
        public DbSet<GarantieTip> GarantieTip { get; set; }
        public DbSet<GarantieCeGaranteaza> GarantieCeGaranteaza { get; set; }
        public DbSet<Rata> Rate { get; set; }
        public DbSet<ImprumutTermen> ImprumutTermene { get; set; }
        public DbSet<ImprumutTip> ImprumutTipuri { get; set; }
        public DbSet<ImprumutTipDetaliu> ImprumutTipDetalii { get; set; }
        public DbSet<Operatie> Operatie { get; set; }
        public DbSet<Account> Account { get; set; }
        public DbSet<AccountHistory> AccountHistory { get; set; }
        public DbSet<AccountRelation> AccountRelation { get; set; }
        public DbSet<AccountTaxProperty> AccountTaxProperties { get; set; }
        public DbSet<DocumentType> DocumentType { get; set; }
        public DbSet<ExchangeRates> ExchangeRates { get; set; }
        public DbSet<Currency> Currency { get; set; }
        public DbSet<Region> Region { get; set; }
        public DbSet<BankAccount> BankAccount { get; set; }
        public DbSet<Country> Country { get; set; }
        public DbSet<Person> Persons { get; set; }
        public DbSet<LegalPerson> LegalPersons { get; set; }
        public DbSet<NaturalPerson> NaturalPersons { get; set; }
        public DbSet<Operation> Operations { get; set; }
        public DbSet<OperationDetails> OperationsDetails { get; set; }
        public DbSet<Balance> Balance { get; set; }
        public DbSet<BalanceDetails> BalanceDetails { get; set; }
        public DbSet<SavedBalanceViewDet> SavedBalanceViewDet { get; set; }
        public DbSet<SavedBalance> SavedBalance { get; set; }
        public DbSet<SavedBalanceDetails> SavedBalanceDetails { get; set; }
        public DbSet<SavedBalanceDetailsCurrency> SavedBalanceDetailsCurrencies { get; set; }
        public DbSet<ForeignOperation> ForeignOperation { get; set; }
        public DbSet<ForeignOperationsDetails> ForeignOperationsDetails { get; set; }
        public DbSet<ForeignOperationsAccounting> ForeignOperationsAccounting { get; set; }

        public DbSet<ForeignOperationDictionary> ForeignOperationDictionary { get; set; }
        public DbSet<AccountConfig> AccountConfig { get; set; }

        public DbSet<Decont> Decont { get; set; }
        public DbSet<DiurnaLegala> DiurnaLegala { get; set; }
        public DbSet<CotaTVA> CotaTVA { get; set; }

        public DbSet<DiurnaZi> DiurnaZi { get; set; }
        public DbSet<ZileLibere> ZileLibere { get; set; }

        #region Contracts
        public DbSet<Contracts> Contracts { get; set; }
        public DbSet<ContractsCategory> ContractsCategory { get; set; }
        public DbSet<ContractsInstalments> ContractsInstalments { get; set; }
        public DbSet<Invoices> Invoices { get; set; }
        public DbSet<InvoiceDetails> InvoiceDetails { get; set; }
        public DbSet<InvoiceElementsDetails> InvoiceElementsDetails { get; set; }
        public DbSet<InvoiceElementAccounts> InvoiceElementAccounts { get; set; }

        public DbSet<InvoiceElementsDetailsCategory> InvoiceElementsDetailsCategory { get; set; }

        public DbSet<ImoInventariere> ImoInventariere { get; set; }
        public DbSet<ImoInventariereDet> ImoInventariereDet { get; set; }

        public DbSet<InvObjectInventariere> InvObjectInventariere { get; set; }

        public DbSet<InvObjectInventariereDet> InvObjectInventariereDet { get; set; }

        #endregion

        #region Economic
        public DbSet<Disposition> Dispositions { get; set; }
        public DbSet<DispositionInvoice> DispositionInvoice { get; set; }
        public DbSet<PaymentOrders> PaymentOrders { get; set; }
        public DbSet<PaymentOrderInvoice> PaymentOrderInvoice { get; set; }
        public DbSet<Exchange> Exchange { get; set; }
        #endregion

        #region ImoAsset
        public DbSet<ImoAssetItem> ImoAssetItem { get; set; }
        public DbSet<ImoAssetClassCode> ImoAssetClassCode { get; set; }
        public DbSet<ImoAssetOper> ImoAssetOper { get; set; }
        public DbSet<ImoAssetOperDetail> ImoAssetOperDetail { get; set; }
        public DbSet<ImoAssetStorage> ImoAssetStorage { get; set; }
        public DbSet<ImoAssetStock> ImoAssetStock { get; set; }
        public DbSet<ImoAssetStockReserve> ImoAssetStockReserve { get; set; }
        public DbSet<ImoAssetStockModerniz> ImoAssetStockModerniz { get; set; }
        public DbSet<ImoAssetOperDocType> ImoAssetOperDocType { get; set; }
        public DbSet<ImoAssetSetup> ImoAssetSetup { get; set; }
        public DbSet<ImoAssetOperForType> ImoAssetOperForType { get; set; }
        #endregion

        #region InventoryObjects
        public DbSet<InvObjectItem> InvObjectItem { get; set; }
        public DbSet<InvCateg> InvCateg { get; set; }
        public DbSet<InvStorage> InvStorage { get; set; }
        public DbSet<InvObjectOperDocType> InvObjectOperDocType { get; set; }
        public DbSet<InvObjectOper> InvObjectOper { get; set; }
        public DbSet<InvObjectOperDetail> InvObjectOperDetail { get; set; }
        public DbSet<InvObjectStockReserve> InvObjectStockReserve { get; set; }
        public DbSet<InvObjectStock> InvObjectStock { get; set; }
        #endregion

        #region Prepayments
        public DbSet<Prepayment> Prepayment { get; set; }
        public DbSet<PrepaymentBalance> PrepaymentBalance { get; set; }
        public DbSet<PrepaymentDocType> PrepaymentDocType { get; set; }
        public DbSet<PrepaymentsDurationSetup> PrepaymentsDurationSetup { get; set; }
        public DbSet<PrepaymentsDecDeprecSetup> PrepaymentsDecDeprecSetup { get; set; }

        #endregion Prepayments

        #region AutoOperation
        public DbSet<AutoOperationConfig> AutoOperationConfig { get; set; }
        public DbSet<AutoOperationSearchConfig> AutoOperationSearchConfig { get; set; }
        public DbSet<AutoOperationOper> AutoOperationOper { get; set; }
        public DbSet<AutoOperationDetail> AutoOperationDetail { get; set; }
        public DbSet<OperGenerateCateg> OperGenerateCateg { get; set; }
        public DbSet<OperGenerateTipuri> OperGenerateTipuri { get; set; }
        public DbSet<OperGenerate> OperGenerate { get; set; }

        #endregion

        #region SitFinan

        public DbSet<SitFinan> SitFinan { get; set; }
        public DbSet<SitFinanRap> SitFinanRap { get; set; }
        public DbSet<SitFinanRapConfig> SitFinanRapConfig { get; set; }
        public DbSet<SitFinanRapConfigCol> SitFinanRapConfigCol { get; set; }
        public DbSet<SitFinanRapConfigNote> SitFinanRapConfigNote { get; set; }
        public DbSet<SitFinanRapConfigCorel> SitFinanRapConfigCorel { get; set; }
        public DbSet<SitFinanRapFluxConfig> SitFinanRapFluxConfig { get; set; }
        public DbSet<SitFinanCalc> SitFinanCalc { get; set; }
        public DbSet<SitFinanCalcDetails> SitFinanCalcDetails { get; set; }
        public DbSet<SitFinanCalcFormulaDet> SitFinanCalcFormulaDet { get; set; }
        public DbSet<SitFinanCalcNote> SitFinanCalcNote { get; set; }

        #endregion

        #region Configurare Rapoarte
        public DbSet<ReportInit> ReportInit { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<ReportConfig> ReportConfig { get; set; }
        #endregion

        #region FileDoc
        public DbSet<FileDocError> FileDocErrors { get; set; }
        #endregion FileDoc

        public DbSet<Issuer> Issuer { get; set; }

        public DbSet<ActivityType> ActivityTypes { get; set; }
        public DbSet<OperationTypes> OperationTypes { get; set; }

        public DbSet<Tokens> Tokens { get; set; }

        #region Registru Inventar
        //Registru Inventar
        public DbSet<RegInventar> RegInventar { get; set; }
        public DbSet<RegInventarExceptii> RegInventarExceptii { get; set; }
        public DbSet<RegInventarExceptiiEliminare> RegInventarExceptiiEliminare { get; set; }
        #endregion

        #region Cupiuri

        public DbSet<CupiuriInit> CupiuriInit { get; set; }
        public DbSet<CupiuriItem> CupiuriItem { get; set; }
        public DbSet<CupiuriDetails> CupiuriDetails { get; set; }
        #endregion

        #region BVC

        public DbSet<BVC_PAAP> BVC_PAAP { get; set; }
        public DbSet<BVC_PAAP_State> BVC_PAAP_State { get; set; }
        public DbSet<BVC_PAAP_AvailableSum> BVC_PAAP_AvailableSum { get; set; }
        public DbSet<BVC_PAAP_ApprovedYear> BVC_PAAP_ApprovedYear { get; set; }
        public DbSet<BVC_PAAP_InvoiceDetails> BVC_PAAP_InvoiceDetails { get; set; }
        public DbSet<BVC_PAAP_Referat> BVC_PAAP_Referat { get; set; }
        public DbSet<BVC_PAAPTranse> BVC_PAAPTranse { get; set; }

        // BVC
        public DbSet<BVC_Formular> BVC_Formular { get; set; }
        public DbSet<BVC_FormRand> BVC_FormRand { get; set; }
        public DbSet<BVC_FormRandDetails> BVC_FormRandDetails { get; set; }
        public DbSet<BVC_BugetPrev> BVC_BugetPrev { get; set; }
        public DbSet<BVC_BugetPrevRand> BVC_BugetPrevRand { get; set; }
        public DbSet<BVC_BugetPrevStatus> BVC_BugetPrevStatus { get; set; }
        public DbSet<BVC_BugetPrevRandValue> BVC_BugetPrevRandValue { get; set; }
        public DbSet<BVC_BugetPrevAutoValue> BVC_BugetPrevAutoValue { get; set; }
        public DbSet<BVC_BugetPrevContributie> BVC_BugetPrevContributie { get; set; }
        public DbSet<BVC_BugetPrevStatCalcul> BVC_BugetPrevStatCalcul { get; set; }
        public DbSet<BVC_BugetPrevSumeReinvest> BVC_BugetPrevSumeReinvest { get; set; }
        public DbSet<BVC_BugetPrevTitluriValab> BVC_BugetPrevTitluriValab { get; set; }
        public DbSet<BVC_DobandaReferinta> BVC_DobandaReferinta { get; set; }
        public DbSet<BVC_Realizat> BVC_Realizat { get; set; }
        public DbSet<BVC_RealizatRand> BVC_RealizatRand { get; set; }
        public DbSet<BVC_RealizatRandDetails> BVC_RealizatRandDetails { get; set; }
        public DbSet<BVC_BalRealizat> BVC_BalRealizat { get; set; }
        public DbSet<BVC_BalRealizatRand> BVC_BalRealizatRand { get; set; }
        public DbSet<BVC_BalRealizatRandDetails> BVC_BalRealizatRandDetails { get; set; }
        public DbSet<BVC_RealizatExceptii> BVC_RealizatExceptii { get; set; }
        public DbSet<BVC_VenitTitlu> BVC_VenitTitlu { get; set; }
        public DbSet<BVC_VenitTitluBVC> BVC_VenitTitluBVC { get; set; }
        public DbSet<BVC_VenitTitluCF> BVC_VenitTitluCF { get; set; }
        public DbSet<BVC_VenitTitluCFReinv> BVC_VenitTitluCFReinv { get; set; }
        public DbSet<BVC_VenitCheltuieli> BVC_VenitCheltuieli { get; set; }
        public DbSet<BVC_VenitProcRepartiz> BVC_VenitProcRepartiz { get; set; }
        public DbSet<BVC_VenitBugetPrelim> BVC_VenitBugetPrelim { get; set; }
        public DbSet<BVC_VenitTitluParams> BVC_VenitTitluParams { get; set; }
        public DbSet<BVC_PrevResurse> BVC_PrevResurse { get; set; }
        public DbSet<BVC_PrevResurseDetalii> BVC_PrevResurseDetalii { get; set; }
        public DbSet<BVC_PaapRedistribuire> BVC_PaapRedistribuire { get; set; }
        public DbSet<BVC_PaapRedistribuireDetalii> BVC_PaapRedistribuireDetalii { get; set; }
        public DbSet<Notificare> Notificare { get; set; }

        #endregion

        #region HR
        public DbSet<Departament> Departament { get; set; }
        public DbSet<SalariatiDepartamente> SalariatiDepartamente { get; set; }
        #endregion

        #region BNR
        public DbSet<BNR_AnexaDetail> BNR_AnexaDetails { get; set; }
        public DbSet<BNR_Anexa> BNR_Anexa { get; set; }
        public DbSet<BNR_Sector> BNR_Sector { get; set; }
        public DbSet<BNR_Conturi> BNR_Conturi { get; set; }
        public DbSet<BNR_Raportare> BNR_Raportare { get; set; }
        public DbSet<BNR_RaportareRand> BNR_RaportareRand { get; set; }
        public DbSet<BNR_RaportareRandDetail> BNR_RaportareRandDetail { get; set; }
        #endregion

        #region Lichiditate

        public DbSet<LichidConfig> LichidConfig { get; set; }
        public DbSet<LichidBenzi> LichidBenzi { get; set; }
        public DbSet<LichidCalc> LichidCalc { get; set; }
        public DbSet<LichidCalcDet> LichidCalcDet { get; set; }
        public DbSet<LichidCalcCurr> LichidCalcCurr { get; set; }
        public DbSet<LichidCalcCurrDet> LichidCalcCurrDet { get; set; }
        public DbSet<LichidBenziCurr> LichidBenziCurr { get; set; }

        #endregion

        public DbSet<SetupStergOperExterna> SetupStergOperExterna { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ...
            MyDbFunctions.Register(modelBuilder);
            // ...

            //preluate din vechi
            //modelBuilder.Properties<string>().Where(n => !n.Name.EndsWith("Lob")).Configure(s => s.HasMaxLength(2000));
            modelBuilder.Entity<ExchangeRates>().Property(f => f.Value).HasPrecision(18, 4);
            modelBuilder.Entity<Exchange>().Property(f => f.ExchangeRate).HasPrecision(18, 4);
            modelBuilder.Entity<SavedBalanceDetails>().Property(f => f.ExhangeRate).HasPrecision(18, 4);
            modelBuilder.Entity<TaxProfitPropertyDet>().Property(f => f.Value).HasPrecision(18, 8);
            modelBuilder.Entity<BVC_PrevResurseDetalii>().Property(f => f.CursValutar).HasPrecision(18, 4);



            //modelBuilder.Configurations.Add(new RoleEfConfig());
            //modelBuilder.Configurations.Add(new UserEfConfig());
            //modelBuilder.Configurations.Add(new PersonEfConfig());

            //modelBuilder.Configurations.Add(new NaturalPersonEfConfig());
            //modelBuilder.Configurations.Add(new AppClientsEfConfig());
            //modelBuilder.Configurations.Add(new ThirdPartyEfConfig());

            //modelBuilder.Configurations.Add(new BankConfig());

            //modelBuilder.Configurations.Add(new ImoAssetStockConfig());

            modelBuilder.Entity<SitFinanCalc>().Property(f => f.Val1).HasPrecision(22, 8);
            modelBuilder.Entity<SitFinanCalc>().Property(f => f.Val2).HasPrecision(22, 8);
            modelBuilder.Entity<SitFinanCalc>().Property(f => f.Val3).HasPrecision(22, 8);
            modelBuilder.Entity<SitFinanCalc>().Property(f => f.Val4).HasPrecision(22, 8);
            modelBuilder.Entity<SitFinanCalc>().Property(f => f.Val5).HasPrecision(22, 8);
            modelBuilder.Entity<SitFinanCalc>().Property(f => f.Val6).HasPrecision(22, 8);

            modelBuilder.Entity<SitFinanCalcDetails>().Property(f => f.Val).HasPrecision(22, 8);

            modelBuilder.Entity<PrepaymentBalance>().Property(f => f.MontlyCharge).HasPrecision(20, 4);
            modelBuilder.Entity<Prepayment>().Property(f => f.MontlyDepreciation).HasPrecision(20, 4);

            //EF CORE LATEST  TODO TEST expected behaviour
            modelBuilder.Entity<BankAccount>()
                .HasOne(s => s.Person)
                .WithMany(s => s.BankAccount)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Invoices>()
                .HasOne(s => s.Currency)
                .WithMany(s => s.Invoice)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Invoices>()
                .HasOne(s => s.MonedaFactura)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Invoices>().Property(f => f.CursValutar).HasPrecision(18, 4);

            modelBuilder.Entity<BankAccount>()
                .HasOne(s => s.Currency)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<OperationDetails>()
                .HasOne(s => s.Debit)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            // operatii contabile
            modelBuilder.Entity<OperationDetails>()
                .HasOne(s => s.Credit)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<OperationDetails>()
                .HasOne(s => s.Debit)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<OperationDetails>()
                .HasOne(s => s.Operation)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<OperationDetails>()
                .HasOne(s => s.InvoiceElementsDetails)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<OperationDetails>()
                .HasOne(s => s.InvoiceElementsDetailsCategory)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Operation>()
                .HasMany(s => s.OperationsDetails)
                .WithOne(s => s.Operation)
                .OnDelete(DeleteBehavior.Cascade);


            // sabloane operatii
            modelBuilder.Entity<OperationDefinitionDetails>()
                .HasOne(s => s.Debit)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<OperationDefinitionDetails>()
                .HasOne(s => s.Credit)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<OperationDefinitionDetails>()
                .HasOne(s => s.OperationDefinition)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<OperationDefinition>()
                .HasMany(s => s.OperationDefinitionDetails)
                .WithOne(s => s.OperationDefinition)
                .OnDelete(DeleteBehavior.Cascade);

            // foreign operations
            modelBuilder.Entity<ForeignOperation>()
                .HasMany(s => s.ForeignOperationsDetails)
                .WithOne(s => s.ForeignOperation)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SavedBalanceDetails>()
                .HasOne(s => s.Currency)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<SavedBalanceDetailsCurrency>()
                .HasOne(s => s.Currency)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<SavedBalanceViewDet>()
                .HasOne(s => s.Currency)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Tenant>()
                .HasOne(s => s.LegalPerson)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<PaymentOrders>()
                .HasOne(s => s.Beneficiary)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<PaymentOrders>()
                .HasOne(s => s.PayerBankAccount)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<PaymentOrders>()
                .HasOne(s => s.BenefAccount)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<PaymentOrders>()
                .HasOne(s => s.FgnOperationsDetail)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<PaymentOrders>().Ignore(s => s.PaymentOrderInvoices);

            modelBuilder.Entity<PaymentOrderInvoice>()
                .HasOne(s => s.Invoice)
                .WithMany(s => s.PaymentOrderInvoices).HasForeignKey(s => s.InvoiceId).IsRequired(true)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<PaymentOrderInvoice>()
               .HasOne(s => s.PaymentOrder)
               .WithMany(s => s.PaymentOrderInvoices).HasForeignKey(s => s.PaymentOrderId).IsRequired(true)
               .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<BalanceDetails>()
                .HasOne(s => s.Currency)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<BalanceDetails>()
                .HasOne(s => s.Account)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ForeignOperationsDetails>()
                .HasOne(s => s.ForeignOperation)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ForeignOperation>()
                .HasMany(s => s.ForeignOperationsDetails)
                .WithOne(s => s.ForeignOperation)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ImoAssetOperDetail>()
                .HasOne(s => s.ImoAssetOper)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ImoAssetOper>()
                .HasMany(s => s.OperDetails)
                .WithOne(s => s.ImoAssetOper)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ImoAssetOper>()
                .HasOne(s => s.PersonStoreIn)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ImoAssetOper>()
                .HasOne(s => s.PersonStoreOut)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            #region AutoOperation
            modelBuilder.Entity<AutoOperationDetail>()
                .HasOne(s => s.CreditAccount)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<AutoOperationDetail>()
                .HasOne(s => s.DebitAccount)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);
            #endregion

            #region InvObject
            modelBuilder.Entity<InvObjectOperDetail>()
                .HasOne(s => s.InvObjectOper)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<InvObjectOperDetail>()
                .HasOne(s => s.InvoiceDetail)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<InvObjectOper>()
                .HasMany(s => s.OperDetails)
                .WithOne(s => s.InvObjectOper)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<InvObjectOper>()
                .HasOne(s => s.PersonStoreOut)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<InvObjectOper>()
    .HasOne(s => s.PersonStoreIn)
    .WithMany()
    .OnDelete(DeleteBehavior.NoAction);
            #endregion

            modelBuilder.Entity<SitFinanCalc>()
                .HasOne(s => s.SitFinanRap)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Account>().HasMany(a => a.AnalyticAccounts).WithOne(a => a.SyntheticAccount).HasForeignKey(f => f.SyntheticAccountId).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Account>()
                .HasOne(s => s.BNR_Sector)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<AccountHistory>()
                .HasOne(s => s.BNR_Sector)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            // Dispozitii
            modelBuilder.Entity<Disposition>()
                .HasOne(s => s.Operation)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Disposition>().Ignore(s => s.DispositionInvoices);

            modelBuilder.Entity<DispositionInvoice>()
                .HasOne(s => s.Invoice)
                .WithMany(s => s.DispositionInvoices).HasForeignKey(s => s.InvoiceId).IsRequired(true)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<DispositionInvoice>()
                .HasOne(s => s.Disposition)
                .WithMany(s => s.DispositionInvoices).HasForeignKey(s => s.DispositionId).IsRequired(true)
                .OnDelete(DeleteBehavior.NoAction);

            // Tokens
            modelBuilder.Entity<Tokens>()
                .HasOne(s => s.User)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Operation>().HasOne(a => a.OperationParent).WithMany().OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Decont>()
                .Property(f => f.CurrencyId)
                .HasDefaultValue(1);

            modelBuilder.Entity<Decont>()
                .HasOne(s => s.DocumentType)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            //imprumuturi
            modelBuilder.Entity<Imprumut>().HasOne(a => a.LoanAccount).WithMany().OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Imprumut>().HasOne(a => a.PaymentAccount).WithMany().OnDelete(DeleteBehavior.NoAction);

            //modelBuilder.Entity<Imprumut>()
            //    .HasOne(s => s.Currency)
            //    .WithMany()
            //    .OnDelete(DeleteBehavior.NoAction);

            // Exchange
            modelBuilder.Entity<Exchange>()
                .HasOne(f => f.BankAccountLei)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Exchange>()
                .HasOne(f => f.BankAccountValuta)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<CupiuriItem>()
                .HasOne(s => s.CupiuriInit)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<SalariatiDepartamente>()
                .HasOne(s => s.Departament)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<SalariatiDepartamente>()
                .HasOne(s => s.Person)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            #region BVC
            modelBuilder.Entity<BVC_PAAP_InvoiceDetails>()
                .HasOne(s => s.InvoiceDetails)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<BVC_PAAPTranse>()
                .HasOne(s => s.BVC_PAAP)
                .WithMany(m => m.Transe).HasForeignKey(f => f.BVC_PAAPId).IsRequired(true)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BVC_PAAP>()
                .HasOne(s => s.CotaTVA)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<BVC_PAAP>()
                .Property(s => s.NrTranse)
                .HasDefaultValue(1);

            //modelBuilder.Entity<BVC_Formular>()
            //    .HasMany(s => s.FormRanduri)
            //    .WithOne(s => s.Formular)
            //    .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<BVC_FormRand>()
                .HasOne(s => s.Formular)
                .WithMany(m => m.FormRanduri).HasForeignKey(f => f.FormularId).IsRequired(true)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BVC_FormRand>().HasMany(a => a.RanduriChild).WithOne(a => a.RandParent).HasForeignKey(f => f.RandParentId).OnDelete(DeleteBehavior.NoAction);

            //modelBuilder.Entity<BVC_FormRand>()
            //    .HasMany(s => s.DetaliiRand)
            //    .WithOne(s => s.FormRand)
            //    .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<BVC_FormRandDetails>()
                .HasOne(s => s.FormRand)
                .WithMany(m => m.DetaliiRand).HasForeignKey(f => f.FormRandId).IsRequired(true)
                .OnDelete(DeleteBehavior.Restrict);

            //modelBuilder.Entity<BVC_BugetPrev>()
            //    .HasMany(s => s.PrevRanduri)
            //    .WithOne(s => s.BugetPrev).
            //    .OnDelete(DeleteBehavior.NoAction);

            //modelBuilder.Entity<BVC_BugetPrev>()
            //    .HasMany(s => s.StatusList)
            //    .WithOne(s => s.BugetPrev)
            //    .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<BVC_BugetPrevStatus>()
                .HasOne(s => s.BugetPrev)
                .WithMany(m => m.StatusList).HasForeignKey(f => f.BugetPrevId).IsRequired(true)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BVC_BugetPrevStatCalcul>()
                .HasOne(s => s.BugetPrev)
                .WithMany()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BVC_BugetPrevSumeReinvest>()
                .HasOne(s => s.BugetPrev)
                .WithMany()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BVC_BugetPrevTitluriValab>()
                .HasOne(s => s.BugetPrev)
                .WithMany()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BVC_PAAP_AvailableSum>()
                .HasOne(s => s.InvoiceElementsDetailsCategory)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<BVC_PAAP_AvailableSum>()
                .HasOne(s => s.Departament)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<BVC_BugetPrevRand>()
                .HasOne(s => s.BugetPrev)
                .WithMany(m => m.PrevRanduri).HasForeignKey(f => f.BugetPrevId).IsRequired(true)
                .OnDelete(DeleteBehavior.Cascade);

            //modelBuilder.Entity<BVC_BugetPrevRand>()
            //    .HasMany(s => s.ValueList)
            //    .WithOne(s => s.BugetPrevRand)
            //    .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<BVC_BugetPrevRandValue>()
                .HasOne(s => s.BugetPrevRand)
                .WithMany(m => m.ValueList).HasForeignKey(f => f.BugetPrevRandId).IsRequired(true)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BVC_BugetPrevContributie>()
                .HasOne(s => s.ActivityType)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<BVC_BugetPrevContributie>()
                .HasOne(s => s.BankAccount)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<BVC_BugetPrevContributie>()
                .HasOne(s => s.Currency)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<BVC_RealizatRand>()
                .HasOne(s => s.BVC_Realizat)
                .WithMany(m => m.BVC_RealizatRanduri).HasForeignKey(f => f.BVC_RealizatId).IsRequired(true)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BVC_RealizatRandDetails>()
                .HasOne(s => s.BVC_RealizatRand)
                .WithMany(m => m.BVC_RealizatRandDetails).HasForeignKey(f => f.BVC_RealizatRandId).IsRequired(true)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BVC_RealizatRandDetails>()
                .HasOne(s => s.Currency)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<BVC_BalRealizatRand>()
                .HasOne(s => s.BVC_BalRealizat)
                .WithMany(m => m.BVC_BalRealizatRanduri).HasForeignKey(f => f.BVC_BalRealizatId).IsRequired(true)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BVC_BalRealizatRandDetails>()
                .HasOne(s => s.BVC_BalRealizatRand)
                .WithMany(m => m.BVC_BalRealizatRandDetails).HasForeignKey(f => f.BVC_BalRealizatRandId).IsRequired(true)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BVC_BalRealizatRand>()
                .HasOne(s => s.ActivityType)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<BVC_VenitTitluBVC>()
                .HasOne(s => s.BVC_VenitTitlu)
                .WithMany(m => m.VenituriBVC).HasForeignKey(f => f.BVC_VenitTitluId).IsRequired(true)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BVC_VenitTitluCF>()
                .HasOne(s => s.BVC_VenitTitlu)
                .WithMany(m => m.VenituriCF).HasForeignKey(f => f.BVC_VenitTitluId).IsRequired(true)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BVC_VenitTitluCFReinv>()
                .HasOne(s => s.BVC_VenitTitluCF)
                .WithMany(m => m.BVC_VenitTitluCFReinv).HasForeignKey(f => f.BVC_VenitTitluCFId).IsRequired(true)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BVC_VenitTitluCFReinv>()
                .HasOne(s => s.Currency)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<BVC_VenitCheltuieli>()
                .HasOne(s => s.BVC_VenitTitluCFReinv)
                .WithMany(m => m.VenitCheltuieli).HasForeignKey(f => f.BVC_VenitTitluCFReinvId).IsRequired(true)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<BVC_VenitTitluParams>()
                .HasOne(s => s.Formular)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<BVC_VenitCheltuieli>()
                .HasOne(s => s.Formular)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<BVC_VenitProcRepartiz>()
                .HasOne(s => s.BVC_VenitBugetPrelim)
                .WithMany(m => m.VenitProcRepartiz).HasForeignKey(f => f.BVC_VenitBugetPrelimId).IsRequired(true)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<BVC_VenitBugetPrelim>()
                .HasOne(s => s.Formular)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<BVC_DobandaReferinta>()
                .HasOne(s => s.Currency)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<BVC_DobandaReferinta>()
                .HasOne(s => s.ActivityType)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<BVC_PrevResurseDetalii>()
                .HasOne(s => s.Currency)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<BVC_PrevResurse>()
                .HasOne(s => s.ActivityType)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<BVC_PrevResurse>()
                .HasOne(s => s.BugetPrev)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            #endregion

            #region BNR
            modelBuilder.Entity<BNR_AnexaDetail>()
                .HasOne(s => s.BNR_Anexa)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<BNR_Conturi>()
                .HasOne(s => s.BNR_Sector)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<BNR_Conturi>()
                .HasOne(s => s.BNR_AnexaDetail)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<BNR_Raportare>()
                .HasOne(s => s.BNR_Anexa)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<BNR_Raportare>()
                .HasOne(s => s.SavedBalance)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<BNR_RaportareRand>()
                .HasOne(s => s.BNR_AnexaDetail)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<BNR_RaportareRand>()
                .HasOne(s => s.BNR_Raportare)
                .WithMany(m => m.BNR_RaportareRanduri).HasForeignKey(f => f.BNR_RaportareId).IsRequired(true)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BNR_RaportareRand>()
                .HasOne(s => s.BNR_Sector)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);
            #endregion

            #region Lichiditate
            modelBuilder.Entity<LichidCalc>()
               .HasOne(s => s.SavedBalance)
               .WithMany()
               .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<LichidCalc>()
               .HasOne(s => s.LichidConfig)
               .WithMany()
               .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<LichidCalc>()
               .HasOne(s => s.LichidBenzi)
               .WithMany()
               .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<LichidCalcDet>()
                .HasOne(s => s.LichidCalc)
                .WithMany(m => m.LichidCalcDet).HasForeignKey(f => f.LichidCalcId).IsRequired(true)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<LichidCalcCurr>()
               .HasOne(s => s.SavedBalance)
               .WithMany()
               .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<LichidCalcCurr>()
               .HasOne(s => s.LichidConfig)
               .WithMany()
               .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<LichidCalcCurr>()
               .HasOne(s => s.LichidBenziCurr)
               .WithMany()
               .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<LichidCalcCurrDet>()
                .HasOne(s => s.LichidCalcCurr)
                .WithMany(m => m.LichidCalcCurrDet).HasForeignKey(f => f.LichidCalcCurrId).IsRequired(true)
                .OnDelete(DeleteBehavior.Cascade);

            #endregion

            modelBuilder.Entity<BVC_Cheltuieli>()
                .HasOne(s => s.Departament)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ImprumutTipDetaliu>()
                .HasOne(s => s.ImprumutTip)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ImprumutTipDetaliu>()
                .HasOne(s => s.ActivityType)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<BVC_PaapRedistribuire>()
                .HasMany(s => s.PaapRedistribuireDetalii)
                .WithOne(s => s.BVC_PaapRedistribuire)
                .HasForeignKey(f => f.BVC_PaapRedistribuireId)
                .OnDelete(DeleteBehavior.Cascade);


            //modelBuilder.Entity<BVC_PaapRedistribuireDetalii>()
            //    .HasOne(s => s.BVC_PaapRedistribuire)
            //    .WithMany()
            //    .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Notificare>()
                .HasOne(s => s.Departament)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        public override ValueTask DisposeAsync()
        {
            return base.DisposeAsync();
        }
    }

    public static class SqlServerModelBuilderExtensions
    {
        public static PropertyBuilder<decimal?> HasPrecision(this PropertyBuilder<decimal?> builder, int precision, int scale)
        {
            return builder.HasColumnType($"decimal({precision},{scale})");
        }

        public static PropertyBuilder<decimal> HasPrecision(this PropertyBuilder<decimal> builder, int precision, int scale)
        {
            return builder.HasColumnType($"decimal({precision},{scale})");
        }


    }


}
