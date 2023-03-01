using Abp.Application.Services;
using Niva.Conta.Nomenclatures;
using Niva.Erp.EntityFrameworkCore.Methods;
using Niva.Erp.Models.AutoOperation;
using Niva.Erp.Models.Buget;
using Niva.Erp.Models.Conta;
using Niva.Erp.Models.Conta.Enums;
using Niva.Erp.Models.Conta.Lichiditate;
using Niva.Erp.Models.Conta.SituatiiFinanciare;
using Niva.Erp.Models.Conta.TaxProfit;
using Niva.Erp.Models.Contracts;
using Niva.Erp.Models.Deconturi;
using Niva.Erp.Models.Economic.Casierii;
using Niva.Erp.Models.Emitenti;
using Niva.Erp.Models.ImoAsset;
using Niva.Erp.Models.Imprumuturi;
using Niva.Erp.Models.InvObjects;
using Niva.Erp.Models.PrePayments;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Niva.Erp.Conta.Nomenclatures
{
    public interface IEnumAppService : IApplicationService
    {
        GetImoAssetOperTypeOutput ImoAssetOperTypeList(); // Mij Fixe
        GetInvOperationTypeOutput InvOperationTypeList(); // Obiecte de inventar        
        List<PrepaymentOperationTypeDto> PrepaymentsOperationTypeList(); // Cheltuieli / Venituri in avans
        List<EnumTypeDto> TaxProfitOperationTypeList(); // Impozit pe profit
        List<EnumTypeDto> AccountTypeList(); // Tipuri conturi contabile
        List<EnumTypeDto> AccountFuncTypeList(); // Functii conturi contabile
        List<EnumTypeDto> AccountTaxStatusList(); // impozabil / neimpozabil
        List<EnumTypeDto> AutoOperationTypeList(); // Categorii operatii contari automate - calcul
        List<EnumTypeDto> AutoOperationTypeListConfig();  // Categorii operatii contari automate - configurare
        List<EnumTypeDto> ImoAssetStockElementList(); // Elemente stoc mijloace fixe
        List<EnumTypeDto> InvObjectStockElementList(); // Elemente stoc obiecte de inventar
        List<EnumTypeDto> PrepaymentStockElementList(); // Elemente stoc chelt in avans
        List<EnumTypeDto> TaxProfitElementList(); // Elemente impozit pe profit
        List<EnumTypeDto> ValueSignList(); // pozitiv (1) sau negativ (-1)
        // InvoiceElementsType
        List<EnumTypeDto> InvoiceElementsTypeList();
        List<EnumTypeDto> AccountDivConfigPersTypeList();
        List<EnumTypeDto> AccountDivConfigResidenceTypeList();
        List<EnumTypeDto> AccountDivConfigTypeList();
        List<EnumTypeDto> PrepaymentsOperTypeList();
        List<EnumTypeDto> TaxProfitPropertyElemList();
        List<EnumTypeDto> TaxProfitPickConditionList();
        List<EnumTypeDto> TaxProfitConfigSourceList();
        List<EnumTypeDto> TaxProfitDeducType();
        List<EnumTypeDto> PrepaymentDurationCalcList();
        List<EnumTypeDto> ImoAssetTypeList();
        List<EnumTypeDto> IssuerTypeList();

        List<EnumTypeDto> DecontTypeList(); // tip decont - CASA sau CARD
        List<EnumTypeDto> CategoryTypeList(); // tip categorie (cheltuieli, Venituri, PlatiIncasari)
        List<EnumTypeDto> DiurnaTypeList(); // tip diurna (interna, externa)
        List<EnumTypeDto> DictionaryTypeList(); // tip expresie dictionar (incasare, plata, toate)
        List<EnumTypeDto> ScopeDeplasareList(); // scopul deplasarii (Lichidare, Deplasare)
        List<EnumTypeDto> ImprumuturiTipDurataList(); // ImprumuturiTipDurata
        List<EnumTypeDto> TitluriDePlasamentOperTypeList();
        List<EnumTypeDto> TitluriDePlasamentElementList();
        List<EnumTypeDto> DepoziteBancareOperTypeList();
        List<EnumTypeDto> DepoziteBancareElementList();
        List<EnumTypeDto> RepoDepoDateInPensiuneLivrataOperTypeList();
        List<EnumTypeDto> RepoDepoDateInPensiuneLivrataElementList();
        List<EnumTypeDto> RepoDepoPrimiteInPensiuneLivrataOperTypeList();
        List<EnumTypeDto> RepoDepoPrimiteInPensiuneLivrataElementList();
        List<EnumTypeDto> CertificateDepozitOperTypeList();
        List<EnumTypeDto> CertificateDepozitElementList();
        List<EnumTypeDto> ContributiiOperTypeList();
        List<EnumTypeDto> ContributiiElementList();
        List<EnumTypeDto> AjustariDeprecierePlasamenteOperTypeList();
        List<EnumTypeDto> AjustariDeprecierePlasamenteElementList();
        List<EnumTypeDto> ReclasificariOperTypeList();
        List<EnumTypeDto> ReclasificariElementList();
        List<EnumTypeDto> ImprumuturiOperTypeList();
        List<EnumTypeDto> ImprumuturiElementList();

        List<EnumTypeDto> ExchangeOperTypeList(); // tip operatie (cumpar lei/ cumpar valuta)
        List<EnumTypeDto> ExchangeTypeList(); // operatiune/cheltuiala

        List<EnumTypeDto> SursaFinantareList(); // lista surse de finantare
        List<EnumTypeDto> ModalitateDerulareList(); // modalitate derulare planificare achizitie
        List<EnumTypeDto> ObiecteTranzactieList(); // lista obiectelor de tranzactie  
        List<EnumTypeDto> ContractsStatusList(); // lista status contracte
        List<EnumTypeDto> TipPerioadaSoldList();

        List<EnumTypeDto> BvcTipList(); //lista tipuri BVC (BVC, Cashflow)
        List<EnumTypeDto> BVC_RowTypeList(); // lista tipuri rand 
        List<EnumTypeDto> BVC_RowTypeIncome(); //lista tipuri venit
        List<EnumTypeDto> BVC_StatusList(); // lista status buget prevazut
        List<EnumTypeDto> BVC_PlasamentTypeList(); // lista tip plasament
        List<EnumTypeDto> PreliminatCalculTypeList(); // 

        List<EnumTypeDto> SitFinanRowModCalc(); // Lista tip mod calcul situatii financiare
        List<EnumTypeDto> TipTragere();
        List<EnumTypeDto> ImprumuturiStare();
        List<EnumTypeDto> LichidCalcTypeList();
        List<EnumTypeDto> GarantieTipPrimitaData();

        List<EnumImprumutTipDetaliuDescriereDto> ImprumutTipDetaliuDescriere();
        List<EnumTypeDto> OperatieGarantieTipEnum();
    }

    public class GetImoAssetOperTypeOutput
    {
        public List<ImoAssetOperTypeDto> GetImoAssetOperType { get; set; }
    }

    public class GetInvOperationTypeOutput
    {
        public List<InvOperationTypeDto> GetInvOperationType { get; set; }
    }

    public static class Template
    {
        public static List<EnumTypeDto> EnumList<T>()
        {
            var list = new List<EnumTypeDto>();
            var _operType = Enum.GetValues(typeof(T)).Cast<T>().ToList();

            foreach (var e in Enum.GetValues(typeof(T)))
            {
                var item = new EnumTypeDto { Id = (int)e, Name = e.ToString() };
                list.Add(item);
            }
            return list;
        }
    }



    public class EnumAppService : ErpAppServiceBase, IEnumAppService
    {
        public EnumAppService()
        {

        }

        // Mijloace Fixe
        public GetImoAssetOperTypeOutput ImoAssetOperTypeList()
        {
            var list = new List<ImoAssetOperTypeDto>();
            var _operType = Enum.GetValues(typeof(ImoAssetOperType)).Cast<ImoAssetOperType>().ToList();

            foreach (var e in _operType)
            {
                var item = new ImoAssetOperTypeDto { Id = (int)e, Name = LazyMethods.EnumValueToDescription(e) };
                list.Add(item);
            }
            var ret = new GetImoAssetOperTypeOutput { GetImoAssetOperType = list };
            return ret;
        }

        // Obiecte de Inventar
        public GetInvOperationTypeOutput InvOperationTypeList()
        {
            var list = new List<InvOperationTypeDto>();
            var _operType = Enum.GetValues(typeof(InvObjectOperType)).Cast<InvObjectOperType>().ToList();

            foreach (var e in _operType/*Enum.GetValues(typeof(InvObjectOperType))*/)
            {
                var item = new InvOperationTypeDto { Id = (int)e, Name = LazyMethods.EnumValueToDescription(e) };
                list.Add(item);
            }
            var ret = new GetInvOperationTypeOutput { GetInvOperationType = list };
            return ret;
        }

        // Cheltuieli / Venituri in avans
        public List<PrepaymentOperationTypeDto> PrepaymentsOperationTypeList()
        {
            var list = new List<PrepaymentOperationTypeDto>();
            var _operType = Enum.GetValues(typeof(PrepaymentOperType)).Cast<PrepaymentOperType>().ToList();

            foreach (var e in _operType/*Enum.GetValues(typeof(PrepaymentOperType))*/)
            {
                var item = new PrepaymentOperationTypeDto { Id = (int)e, Name = LazyMethods.EnumValueToDescription(e) };
                list.Add(item);
            }
            return list;
        }

        // Impozit pe profit
        public List<EnumTypeDto> TaxProfitOperationTypeList()
        {
            var list = new List<EnumTypeDto>();
            var _operType = Enum.GetValues(typeof(TaxProfitOperType)).Cast<TaxProfitOperType>().ToList();

            foreach (var e in Enum.GetValues(typeof(TaxProfitOperType)))
            {
                var item = new EnumTypeDto { Id = (int)e, Name = e.ToString() };
                list.Add(item);
            }
            return list;
        }


        // Tipuri conturi contabile
        public List<EnumTypeDto> AccountTypeList()
        {
            var list = new List<EnumTypeDto>();
            var _operType = Enum.GetValues(typeof(AccountTypes)).Cast<AccountTypes>().ToList();

            foreach (var e in Enum.GetValues(typeof(AccountTypes)))
            {
                var item = new EnumTypeDto { Id = (int)e, Name = e.ToString() };
                list.Add(item);
            }
            var ret = list;
            return ret;
        }

        // Functii conturi contabile
        public List<EnumTypeDto> AccountFuncTypeList() // Functii conturi contabile
        {
            var list = new List<EnumTypeDto>();
            var _operType = Enum.GetValues(typeof(AccountFuncType)).Cast<AccountFuncType>().ToList();

            foreach (var e in _operType)
            {
                var item = new EnumTypeDto { Id = (int)e, Name = LazyMethods.EnumValueToDescription(e) };
                list.Add(item);
            }
            var ret = list;
            return ret;
        }

        // Categorii operatii contari automate
        public List<EnumTypeDto> AutoOperationTypeList()
        {
            var list = new List<EnumTypeDto>();
            var _operType = Enum.GetValues(typeof(AutoOperationType)).Cast<AutoOperationType>().ToList();

            foreach (var e in _operType)
            {
                var item = new EnumTypeDto { Id = (int)e, Name = LazyMethods.EnumValueToDescription(e) };
                if (item.Id != 4)
                {
                    list.Add(item);
                }
            }
            var ret = list;
            return ret;
        }

        // Categorii operatii contari automate
        public List<EnumTypeDto> AutoOperationTypeListConfig()
        {
            var list = new List<EnumTypeDto>();
            var _operType = Enum.GetValues(typeof(AutoOperationType)).Cast<AutoOperationType>().ToList();

            foreach (var e in _operType)
            {
                var item = new EnumTypeDto { Id = (int)e, Name = LazyMethods.EnumValueToDescription(e) };
                list.Add(item);
            }
            var ret = list;
            return ret;
        }

        // Elemente stoc mijloace fixe
        public List<EnumTypeDto> ImoAssetStockElementList()
        {
            var list = new List<EnumTypeDto>();
            var _operType = Enum.GetValues(typeof(ImoAssetStockElement)).Cast<ImoAssetStockElement>().ToList();

            foreach (var e in _operType)
            {
                var item = new EnumTypeDto { Id = (int)e, Name = LazyMethods.EnumValueToDescription(e) };
                list.Add(item);
            }
            var ret = list;
            return ret;
        }

        // Elemente stoc obiecte de inventar
        public List<EnumTypeDto> InvObjectStockElementList()
        {
            var list = new List<EnumTypeDto>();
            var _operType = Enum.GetValues(typeof(InvObjectStockElement)).Cast<InvObjectStockElement>().ToList();

            foreach (var e in _operType)
            {
                var item = new EnumTypeDto { Id = (int)e, Name = LazyMethods.EnumValueToDescription(e) };
                list.Add(item);
            }
            var ret = list;
            return ret;
        }

        // Elemente stoc chelt in avans
        public List<EnumTypeDto> PrepaymentStockElementList()
        {
            var list = new List<EnumTypeDto>();
            var _operType = Enum.GetValues(typeof(PrepaymentStockElement)).Cast<PrepaymentStockElement>().ToList();

            foreach (var e in _operType)
            {
                var item = new EnumTypeDto { Id = (int)e, Name = LazyMethods.EnumValueToDescription(e) };
                list.Add(item);
            }
            var ret = list;
            return ret;
        }

        // Elemente impozit pe profit
        public List<EnumTypeDto> TaxProfitElementList()
        {
            var list = new List<EnumTypeDto>();
            var _operType = Enum.GetValues(typeof(TaxProfitElement)).Cast<TaxProfitElement>().ToList();

            foreach (var e in _operType)
            {
                var item = new EnumTypeDto { Id = (int)e, Name = LazyMethods.EnumValueToDescription(e) };
                list.Add(item);
            }
            var ret = list;
            return ret;
        }

        // pozitiv (1) sau negativ (-1)
        public List<EnumTypeDto> ValueSignList()
        {
            var list = new List<EnumTypeDto>();
            var _operType = Enum.GetValues(typeof(ValueSign)).Cast<ValueSign>().ToList();

            foreach (var e in Enum.GetValues(typeof(ValueSign)))
            {
                var item = new EnumTypeDto { Id = (int)e, Name = e.ToString() };
                list.Add(item);
            }
            var ret = list;
            return ret;
        }

        // InvoiceElementsType
        public List<EnumTypeDto> InvoiceElementsTypeList()
        {
            var list = new List<EnumTypeDto>();
            var _operType = Enum.GetValues(typeof(InvoiceElementsType)).Cast<InvoiceElementsType>().ToList();

            foreach (var e in _operType)
            {
                var item = new EnumTypeDto { Id = (int)e, Name = LazyMethods.EnumValueToDescription(e) };
                list.Add(item);
            }
            var ret = list;
            return ret;
        }

        // AccountDivConfigPersType
        public List<EnumTypeDto> AccountDivConfigPersTypeList()
        {
            var list = new List<EnumTypeDto>();
            var _operType = Enum.GetValues(typeof(AccountDivConfigPersType)).Cast<AccountDivConfigPersType>().ToList();

            foreach (var e in Enum.GetValues(typeof(AccountDivConfigPersType)))
            {
                var item = new EnumTypeDto { Id = (int)e, Name = e.ToString() };
                list.Add(item);
            }
            var ret = list;
            return ret;
        }

        // AccountDivConfigResidenceType
        public List<EnumTypeDto> AccountDivConfigResidenceTypeList()
        {
            var list = new List<EnumTypeDto>();
            var _operType = Enum.GetValues(typeof(AccountDivConfigResidenceType)).Cast<AccountDivConfigResidenceType>().ToList();

            foreach (var e in Enum.GetValues(typeof(AccountDivConfigResidenceType)))
            {
                var item = new EnumTypeDto { Id = (int)e, Name = e.ToString() };
                list.Add(item);
            }
            var ret = list;
            return ret;
        }

        // AccountDivConfigType
        public List<EnumTypeDto> AccountDivConfigTypeList()
        {
            var list = new List<EnumTypeDto>();
            var _operType = Enum.GetValues(typeof(AccountDivConfigType)).Cast<AccountDivConfigType>().ToList();

            foreach (var e in _operType)
            {
                var item = new EnumTypeDto { Id = (int)e, Name = LazyMethods.EnumValueToDescription(e) };
                list.Add(item);
            }
            var ret = list;
            return ret;
        }

        public List<EnumTypeDto> PrepaymentsOperTypeList()
        {
            var list = new List<EnumTypeDto>();
            var _operType = Enum.GetValues(typeof(PrepaymentOperType)).Cast<PrepaymentOperType>().ToList();

            foreach (var e in _operType)
            {
                var item = new EnumTypeDto { Id = (int)e, Name = LazyMethods.EnumValueToDescription(e) };
                list.Add(item);
            }
            var ret = list;
            return ret;
        }

        public List<EnumTypeDto> TaxProfitPropertyElemList()
        {
            var list = new List<EnumTypeDto>();
            var _operType = Enum.GetValues(typeof(TaxProfitPropertyElem)).Cast<TaxProfitPropertyElem>().ToList();

            foreach (var e in _operType)
            {
                var item = new EnumTypeDto { Id = (int)e, Name = LazyMethods.EnumValueToDescription(e) };
                list.Add(item);
            }
            var ret = list;
            return ret;
        }

        public List<EnumTypeDto> TaxProfitPickConditionList()
        {
            var list = new List<EnumTypeDto>();
            var _operType = Enum.GetValues(typeof(TaxProfitPickCondition)).Cast<TaxProfitPickCondition>().ToList();

            foreach (var e in Enum.GetValues(typeof(TaxProfitPickCondition)))
            {
                var item = new EnumTypeDto { Id = (int)e, Name = e.ToString() };
                list.Add(item);
            }
            var ret = list;
            return ret;
        }

        public List<EnumTypeDto> TaxProfitConfigSourceList()
        {
            var list = new List<EnumTypeDto>();
            var _operType = Enum.GetValues(typeof(TaxProfitConfigSource)).Cast<TaxProfitConfigSource>().ToList();

            foreach (var e in _operType)
            {
                var item = new EnumTypeDto { Id = (int)e, Name = LazyMethods.EnumValueToDescription(e) };
                list.Add(item);
            }
            var ret = list;
            return ret;
        }

        public List<EnumTypeDto> AccountTaxStatusList() // impozabil / neimpozabil
        {
            var list = new List<EnumTypeDto>();
            var _operType = Enum.GetValues(typeof(TaxStatus)).Cast<TaxStatus>().ToList();

            foreach (var e in Enum.GetValues(typeof(TaxStatus)))
            {
                var item = new EnumTypeDto { Id = (int)e, Name = e.ToString() };
                list.Add(item);
            }
            var ret = list;
            return ret;
        }

        public List<EnumTypeDto> PrepaymentDurationCalcList()
        {
            var list = new List<EnumTypeDto>();
            var _operType = Enum.GetValues(typeof(PrepaymentDurationCalc)).Cast<PrepaymentDurationCalc>().ToList();

            foreach (var e in Enum.GetValues(typeof(PrepaymentDurationCalc)))
            {
                var item = new EnumTypeDto { Id = (int)e, Name = e.ToString() };
                list.Add(item);
            }
            var ret = list;
            return ret;
        }

        public List<EnumTypeDto> ImoAssetTypeList()
        {
            var list = new List<EnumTypeDto>();
            var _operType = Enum.GetValues(typeof(ImoAssetType)).Cast<ImoAssetType>().ToList();

            foreach (var e in _operType)
            {
                var item = new EnumTypeDto { Id = (int)e, Name = LazyMethods.EnumValueToDescription(e) };
                list.Add(item);
            }
            var ret = list;
            return ret;
        }

        public List<EnumTypeDto> IssuerTypeList()
        {
            var list = new List<EnumTypeDto>();
            var _operType = Enum.GetValues(typeof(IssuerType)).Cast<IssuerType>().ToList();

            foreach (var e in _operType)
            {
                var item = new EnumTypeDto { Id = (int)e, Name = LazyMethods.EnumValueToDescription(e) };
                list.Add(item);
            }
            var ret = list;
            return ret;
        }

        public List<EnumTypeDto> TaxProfitDeducType()
        {
            var list = new List<EnumTypeDto>();
            var _operType = Enum.GetValues(typeof(AccountTaxPropertyType)).Cast<AccountTaxPropertyType>().ToList();

            foreach (var e in Enum.GetValues(typeof(AccountTaxPropertyType)))
            {
                var item = new EnumTypeDto { Id = (int)e, Name = e.ToString() };
                list.Add(item);
            }

            var ret = list;
            return ret;
        }

        public List<EnumTypeDto> DecontTypeList()
        {
            var list = new List<EnumTypeDto>();
            var _operType = Enum.GetValues(typeof(DecontType)).Cast<DecontType>().ToList();

            foreach (var e in _operType)
            {
                var item = new EnumTypeDto { Id = (int)e, Name = LazyMethods.EnumValueToDescription(e) };
                list.Add(item);
            }

            var ret = list;
            return ret;
        }

        public List<EnumTypeDto> CategoryTypeList()
        {
            var list = new List<EnumTypeDto>();
            var _operType = Enum.GetValues(typeof(CategoryType)).Cast<CategoryType>().ToList();

            foreach (var e in _operType)
            {
                var item = new EnumTypeDto { Id = (int)e, Name = LazyMethods.EnumValueToDescription(e) };
                list.Add(item);
            }

            var ret = list;
            return list;
        }

        public List<EnumTypeDto> DiurnaTypeList()
        {
            var list = new List<EnumTypeDto>();
            var _operType = Enum.GetValues(typeof(DiurnaType)).Cast<DiurnaType>().ToList();

            foreach (var e in Enum.GetValues(typeof(DiurnaType)))
            {
                var item = new EnumTypeDto { Id = (int)e, Name = e.ToString() };
                list.Add(item);
            }

            var ret = list;
            return ret;
        }

        public List<EnumTypeDto> DictionaryTypeList()
        {
            var list = new List<EnumTypeDto>();
            var _operType = Enum.GetValues(typeof(FODictionaryType)).Cast<FODictionaryType>().ToList();

            foreach (var e in Enum.GetValues(typeof(FODictionaryType)))
            {
                var item = new EnumTypeDto { Id = (int)e, Name = e.ToString() };
                list.Add(item);
            }

            var ret = list;
            return ret;
        }

        public List<EnumTypeDto> ScopeDeplasareList()
        {
            var list = new List<EnumTypeDto>();
            var _operType = Enum.GetValues(typeof(ScopDeplasareType)).Cast<ScopDeplasareType>().ToList();

            foreach (var e in _operType)
            {
                var item = new EnumTypeDto { Id = (int)e, Name = LazyMethods.EnumValueToDescription(e) };
                list.Add(item);
            }

            var ret = list;
            return ret;
        }

        public List<EnumTypeDto> ImprumuturiTipDurataList()
        {
            var list = new List<EnumTypeDto>();
            var _operType = Enum.GetValues(typeof(ImprumuturiTipDurata)).Cast<ImprumuturiTipDurata>().ToList();

            foreach (var e in Enum.GetValues(typeof(ImprumuturiTipDurata)))
            {
                var item = new EnumTypeDto { Id = (int)e, Name = e.ToString() };
                list.Add(item);
            }

            var ret = list;
            return ret;
        }

        public List<EnumTypeDto> TitluriDePlasamentOperTypeList()
        {
            var list = new List<EnumTypeDto>();
            var _operType = Enum.GetValues(typeof(TitluriDePlasamentOperType)).Cast<TitluriDePlasamentOperType>().ToList();

            foreach (var e in _operType)
            {
                var item = new EnumTypeDto { Id = (int)e, Name = LazyMethods.EnumValueToDescription(e) };
                list.Add(item);
            }
            var ret = list;
            return ret;
        }

        public List<EnumTypeDto> TitluriDePlasamentElementList()
        {
            var list = new List<EnumTypeDto>();
            var _operType = Enum.GetValues(typeof(TitluriDePlasamentElement)).Cast<TitluriDePlasamentElement>().ToList();

            foreach (var e in _operType)
            {
                var item = new EnumTypeDto { Id = (int)e, Name = LazyMethods.EnumValueToDescription(e) };
                list.Add(item);
            }
            var ret = list;
            return ret;
        }

        public List<EnumTypeDto> DepoziteBancareOperTypeList()
        {
            var list = new List<EnumTypeDto>();
            var _operType = Enum.GetValues(typeof(DepoziteBancareOperType)).Cast<DepoziteBancareOperType>().ToList();

            foreach (var e in _operType)
            {
                var item = new EnumTypeDto { Id = (int)e, Name = LazyMethods.EnumValueToDescription(e) };
                list.Add(item);
            }
            var ret = list;
            return ret;
        }

        public List<EnumTypeDto> DepoziteBancareElementList()
        {
            var list = new List<EnumTypeDto>();
            var _operType = Enum.GetValues(typeof(DepoziteBancareElement)).Cast<DepoziteBancareElement>().ToList();

            foreach (var e in _operType)
            {
                var item = new EnumTypeDto { Id = (int)e, Name = LazyMethods.EnumValueToDescription(e) };
                list.Add(item);
            }
            var ret = list;
            return ret;
        }

        public List<EnumTypeDto> RepoDepoDateInPensiuneLivrataOperTypeList()
        {
            var list = new List<EnumTypeDto>();
            var _operType = Enum.GetValues(typeof(RepoDepoDateInPensiuneLivrataOperType)).Cast<RepoDepoDateInPensiuneLivrataOperType>().ToList();

            foreach (var e in _operType)
            {
                var item = new EnumTypeDto { Id = (int)e, Name = LazyMethods.EnumValueToDescription(e) };
                list.Add(item);
            }
            var ret = list;
            return ret;
        }

        public List<EnumTypeDto> RepoDepoDateInPensiuneLivrataElementList()
        {
            var list = new List<EnumTypeDto>();
            var _operType = Enum.GetValues(typeof(RepoDepoDateInPensiuneLivrataElement)).Cast<RepoDepoDateInPensiuneLivrataElement>().ToList();

            foreach (var e in _operType)
            {
                var item = new EnumTypeDto { Id = (int)e, Name = LazyMethods.EnumValueToDescription(e) };
                list.Add(item);
            }
            var ret = list;
            return ret;
        }

        public List<EnumTypeDto> RepoDepoPrimiteInPensiuneLivrataOperTypeList()
        {
            var list = new List<EnumTypeDto>();
            var _operType = Enum.GetValues(typeof(RepoDepoPrimiteInPensiuneLivrataOperType)).Cast<RepoDepoPrimiteInPensiuneLivrataOperType>().ToList();

            foreach (var e in _operType)
            {
                var item = new EnumTypeDto { Id = (int)e, Name = LazyMethods.EnumValueToDescription(e) };
                list.Add(item);
            }
            var ret = list;
            return ret;
        }

        public List<EnumTypeDto> RepoDepoPrimiteInPensiuneLivrataElementList()
        {
            var list = new List<EnumTypeDto>();
            var _operType = Enum.GetValues(typeof(RepoDepoPrimiteInPensiuneLivrataElement)).Cast<RepoDepoPrimiteInPensiuneLivrataElement>().ToList();

            foreach (var e in _operType)
            {
                var item = new EnumTypeDto { Id = (int)e, Name = LazyMethods.EnumValueToDescription(e) };
                list.Add(item);
            }
            var ret = list;
            return ret;
        }

        public List<EnumTypeDto> CertificateDepozitOperTypeList()
        {
            var list = new List<EnumTypeDto>();
            var _operType = Enum.GetValues(typeof(CertificateDepozitOperType)).Cast<CertificateDepozitOperType>().ToList();

            foreach (var e in _operType)
            {
                var item = new EnumTypeDto { Id = (int)e, Name = LazyMethods.EnumValueToDescription(e) };
                list.Add(item);
            }
            var ret = list;
            return ret;
        }

        public List<EnumTypeDto> CertificateDepozitElementList()
        {
            var list = new List<EnumTypeDto>();
            var _operType = Enum.GetValues(typeof(CertificateDepozitElement)).Cast<CertificateDepozitElement>().ToList();

            foreach (var e in _operType)
            {
                var item = new EnumTypeDto { Id = (int)e, Name = LazyMethods.EnumValueToDescription(e) };
                list.Add(item);
            }
            var ret = list;
            return ret;
        }

        public List<EnumTypeDto> ContributiiOperTypeList()
        {
            var list = new List<EnumTypeDto>();
            var _operType = Enum.GetValues(typeof(ContributiiOperType)).Cast<ContributiiOperType>().ToList();

            foreach (var e in _operType)
            {
                var item = new EnumTypeDto { Id = (int)e, Name = LazyMethods.EnumValueToDescription(e) };
                list.Add(item);
            }
            var ret = list;
            return ret;
        }

        public List<EnumTypeDto> ContributiiElementList()
        {
            var list = new List<EnumTypeDto>();
            var _operType = Enum.GetValues(typeof(ContributiiElement)).Cast<ContributiiElement>().ToList();

            foreach (var e in Enum.GetValues(typeof(ContributiiElement)))
            {
                var item = new EnumTypeDto { Id = (int)e, Name = e.ToString() };
                list.Add(item);
            }
            var ret = list;
            return ret;
        }

        public List<EnumTypeDto> AjustariDeprecierePlasamenteOperTypeList()
        {
            var list = new List<EnumTypeDto>();
            var _operType = Enum.GetValues(typeof(AjustariDeprecierePlasamenteOperType)).Cast<AjustariDeprecierePlasamenteOperType>().ToList();

            foreach (var e in _operType)
            {
                var item = new EnumTypeDto { Id = (int)e, Name = LazyMethods.EnumValueToDescription(e) };
                list.Add(item);
            }
            var ret = list;
            return ret;
        }

        public List<EnumTypeDto> AjustariDeprecierePlasamenteElementList()
        {
            var list = new List<EnumTypeDto>();
            var _operType = Enum.GetValues(typeof(AjustariDeprecierePlasamenteElement)).Cast<AjustariDeprecierePlasamenteElement>().ToList();

            foreach (var e in _operType)
            {
                var item = new EnumTypeDto { Id = (int)e, Name = LazyMethods.EnumValueToDescription(e) };
                list.Add(item);
            }
            var ret = list;
            return ret;
        }

        public List<EnumTypeDto> ReclasificariOperTypeList()
        {
            return Template.EnumList<ReclasificariOperType>();
        }

        public List<EnumTypeDto> ReclasificariElementList()
        {
            return Template.EnumList<ReclasificariElement>();
        }

        public List<EnumTypeDto> ImprumuturiOperTypeList()
        {
            return Template.EnumList<ImprumuturiOperType>();
        }

        public List<EnumTypeDto> ImprumuturiElementList()
        {
            return Template.EnumList<ImprumuturiElement>();
        }

        public List<EnumTypeDto> ExchangeOperTypeList()
        {
            var list = new List<EnumTypeDto>();
            var _operType = Enum.GetValues(typeof(ExchangeOperType)).Cast<ExchangeOperType>().ToList();

            foreach (var e in _operType)
            {
                var item = new EnumTypeDto { Id = (int)e, Name = LazyMethods.EnumValueToDescription(e) };
                list.Add(item);
            }

            var ret = list;
            return ret;
        }

        public List<EnumTypeDto> ExchangeTypeList()
        {
            var list = new List<EnumTypeDto>();
            var _operType = Enum.GetValues(typeof(ExchangeType)).Cast<ExchangeType>().ToList();

            foreach (var e in Enum.GetValues(typeof(ExchangeType)))
            {
                var item = new EnumTypeDto { Id = (int)e, Name = e.ToString() };
                list.Add(item);
            }

            var ret = list;
            return ret;
        }

        public List<EnumTypeDto> SursaFinantareList()
        {
            var list = new List<EnumTypeDto>();
            var _operType = Enum.GetValues(typeof(SursaFinantare)).Cast<SursaFinantare>().ToList();

            foreach (var e in Enum.GetValues(typeof(SursaFinantare)))
            {
                var item = new EnumTypeDto { Id = (int)e, Name = e.ToString() };
                list.Add(item);
            }
            return list;
        }

        public List<EnumTypeDto> ModalitateDerulareList()
        {
            var list = new List<EnumTypeDto>();
            var _operType = Enum.GetValues(typeof(ModalitateDerulare)).Cast<ModalitateDerulare>().ToList();

            foreach (var e in Enum.GetValues(typeof(ModalitateDerulare)))
            {
                var item = new EnumTypeDto { Id = (int)e, Name = e.ToString() };
                list.Add(item);
            }
            return list;
        }

        public List<EnumTypeDto> ObiecteTranzactieList()
        {
            var list = new List<EnumTypeDto>();
            var _operType = Enum.GetValues(typeof(ObiectTranzactie)).Cast<ObiectTranzactie>().ToList();

            foreach (var e in _operType)
            {
                var item = new EnumTypeDto { Id = (int)e, Name = LazyMethods.EnumValueToDescription(e) };
                list.Add(item);
            }
            return list;
        }

        public List<EnumTypeDto> ContractsStatusList()
        {
            var list = new List<EnumTypeDto>();
            var _operType = Enum.GetValues(typeof(ContractsStatus)).Cast<ContractsStatus>().ToList();

            foreach (var e in _operType)
            {
                var item = new EnumTypeDto { Id = (int)e, Name = LazyMethods.EnumValueToDescription(e) };
                list.Add(item);
            }
            return list;
        }

        public List<EnumTypeDto> TipPerioadaSoldList()
        {
            var list = new List<EnumTypeDto>();
            var _operType = Enum.GetValues(typeof(TipPerioadaSold)).Cast<TipPerioadaSold>().ToList();

            foreach (var e in Enum.GetValues(typeof(TipPerioadaSold)))
            {
                var item = new EnumTypeDto { Id = (int)e, Name = e.ToString() };
                list.Add(item);
            }
            return list;
        }

        public List<EnumTypeDto> BvcTipList()
        {
            var list = new List<EnumTypeDto>();

            foreach (var e in Enum.GetValues(typeof(BVC_Tip)))
            {
                var item = new EnumTypeDto { Id = (int)e, Name = e.ToString() };
                list.Add(item);
            }
            return list;
        }

        public List<EnumTypeDto> BVC_RowTypeList()
        {
            var list = new List<EnumTypeDto>();

            foreach (var e in Enum.GetValues(typeof(BVC_RowType)))
            {
                var item = new EnumTypeDto { Id = (int)e, Name = e.ToString() };
                list.Add(item);
            }
            return list;
        }

        public List<EnumTypeDto> BVC_RowTypeIncome()
        {
            var list = new List<EnumTypeDto>();
            var _operType = Enum.GetValues(typeof(BVC_RowTypeIncome)).Cast<BVC_RowTypeIncome>().ToList();

            foreach (var e in _operType)
            {
                var item = new EnumTypeDto { Id = (int)e, Name = LazyMethods.EnumValueToDescription(e) };
                list.Add(item);
            }
            return list;
        }

        public List<EnumTypeDto> BVC_StatusList()
        {
            var list = new List<EnumTypeDto>();
            var _operType = Enum.GetValues(typeof(BVC_Status)).Cast<BVC_Status>().ToList();

            foreach (var e in _operType)
            {
                var item = new EnumTypeDto { Id = (int)e, Name = LazyMethods.EnumValueToDescription(e) };
                list.Add(item);
            }
            return list;
        }

        public List<EnumTypeDto> BVC_PlasamentTypeList()
        {
            var list = new List<EnumTypeDto>();
            var _operType = Enum.GetValues(typeof(BVC_PlasamentType)).Cast<BVC_PlasamentType>().ToList();

            foreach (var e in _operType)
            {
                var item = new EnumTypeDto { Id = (int)e, Name = LazyMethods.EnumValueToDescription(e) };
                list.Add(item);
            }
            return list;
        }

        public List<EnumTypeDto> BVC_RowTypeList2()
        {
            return Template.EnumList<BVC_RowType>();
        }

        public List<EnumTypeDto> BVC_RowTypeSalarizare()
        {
            return Template.EnumList<BVC_RowTypeSalarizare>();
        }

        public List<EnumTypeDto> PreliminatCalculTypeList()
        {
            var list = new List<EnumTypeDto>();
            var _operType = Enum.GetValues(typeof(PreliminatCalculType)).Cast<PreliminatCalculType>().ToList();

            foreach (var e in _operType)
            {
                var item = new EnumTypeDto { Id = (int)e, Name = LazyMethods.EnumValueToDescription(e) };
                list.Add(item);
            }
            return list;
        }

        public List<EnumTypeDto> SitFinanRowModCalc()
        {
            var list = new List<EnumTypeDto>();
            var _operType = Enum.GetValues(typeof(SitFinanRowModCalc)).Cast<SitFinanRowModCalc>().ToList();

            foreach (var e in Enum.GetValues(typeof(SitFinanRowModCalc)))
            {
                var item = new EnumTypeDto { Id = (int)e, Name = e.ToString() };
                list.Add(item);
            }
            return list;
        }

        public List<EnumTypeDto> BVC_BugetPrevContributieTipIncasare()
        {
            var list = new List<EnumTypeDto>();
            var _operType = Enum.GetValues(typeof(BVC_BugetPrevContributieTipIncasare)).Cast<BVC_BugetPrevContributieTipIncasare>().ToList();

            foreach (var e in _operType)
            {
                var item = new EnumTypeDto { Id = (int)e, Name = LazyMethods.EnumValueToDescription(e) };
                list.Add(item);
            }
            return list;
        }

        public List<EnumTypeDto> TipTragere()
        {
            return Template.EnumList<TipTragere>();
        }

        public List<EnumTypeDto> ImprumuturiStare()
        {
            return Template.EnumList<ImprumuturiStare>();
        }

        public List<EnumTypeDto> LichidCalcTypeList()
        {
            var list = new List<EnumTypeDto>();
            var _operType = Enum.GetValues(typeof(LichidCalcType)).Cast<LichidCalcType>().ToList();

            foreach (var e in _operType)
            {
                var item = new EnumTypeDto { Id = (int)e, Name = LazyMethods.EnumValueToDescription(e) };
                list.Add(item);
            }
            return list;
        }

        public List<EnumImprumutTipDetaliuDescriereDto> ImprumutTipDetaliuDescriere()
        {
            var list = new List<EnumImprumutTipDetaliuDescriereDto>();
            var _operType = Enum.GetValues(typeof(ImprumuturiTipDetaliuEnum)).Cast<ImprumuturiTipDetaliuEnum>().ToList();

            foreach (var e in _operType)
            {
                var item = new EnumImprumutTipDetaliuDescriereDto { Id = e.ToString(), Name = LazyMethods.EnumValueToDescription(e) };
                list.Add(item);
            }
            return list;
        }

        public List<EnumTypeDto> GarantieTipPrimitaData()
        {
            return Template.EnumList<TipGarantiePrimitaDataEnum>(); 
        }

        public List<EnumTypeDto> OperatieGarantieTipEnum()
        {
            return Template.EnumList<TipOperatieGarantieEnum>();
        }
    }

}
