using Abp.Authorization;
using Abp.Localization;
using Abp.MultiTenancy;

namespace Niva.Erp.Authorization
{
    public class ErpAuthorizationProvider : AuthorizationProvider
    {
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            context.CreatePermission(PermissionNames.Pages_Users, L("Users"));
            context.CreatePermission(PermissionNames.Pages_Roles, L("Roles"));
            context.CreatePermission(PermissionNames.Pages_Tenants, L("Tenants"), multiTenancySides: MultiTenancySides.Host);


            context.CreatePermission(PermissionNames.GeneralDateSocietateAcces, L("Vizualizare date societate"));
            context.CreatePermission(PermissionNames.GeneralDateSocietateModificare, L("Modificare date societate"));
            context.CreatePermission(PermissionNames.GeneralEmitentiAcces, L("Vizualizare lista emitenti si detaliile unui emitent"));
            context.CreatePermission(PermissionNames.GeneralEmitentiModificare, L("Adaugare / modificare / sterge detalii emitent"));
            context.CreatePermission(PermissionNames.GeneralPersoaneAcces, L("Vizualizare lista persoane/ detaliile unei persoane /conturi bancare ale persoanei"));
            context.CreatePermission(PermissionNames.GeneralPersoaneModificare, L("Adaugare/modificare/stergere persoana; adaugare/modificare/stergere cont bancar"));
            context.CreatePermission(PermissionNames.GeneralListaConturiAcces, L("Vizualizare lista conturi bancare FGDB"));
            context.CreatePermission(PermissionNames.GeneralListaConturiModificare, L("Adaugare / modificare / stergere cont bancar"));
            context.CreatePermission(PermissionNames.GeneralContracteAcces, L("Vizualizare lista contracte / detaliile unui contract"));
            context.CreatePermission(PermissionNames.GeneralContracteModificare, L("Adaugare / modificare / stergere contract, schimbare stare contract"));
            context.CreatePermission(PermissionNames.GeneralCursValutarAcces, L("Vizualizare curs valutar"));
            context.CreatePermission(PermissionNames.GeneralCursValutarModificare, L("Adaugare / modificare / stergere curs valutar"));
            context.CreatePermission(PermissionNames.BugetBVCConfigurare, L("Configurare formulare BVC si CashFlow"));
            context.CreatePermission(PermissionNames.BugetBVCPreliminat, L("Calcul buget preliminat"));
            context.CreatePermission(PermissionNames.BugetBVCParametriiAcces, L("Acces configurare parametrii BVC"));
            context.CreatePermission(PermissionNames.BugetBVCPrevazutConfigValori, L("Configurare alocare venituri / incasari / cheltuieli provenite din gestiuni la departamentele FGDB"));
            context.CreatePermission(PermissionNames.BugetBVCPrevazutContribAlteIncasari, L("Adaugare incasari previzionate din contributii, creante, etc"));
            context.CreatePermission(PermissionNames.BugetBVCPrevazutDobanziReferinta, L("Adaugare dobanzi de referinta previzionate"));
            context.CreatePermission(PermissionNames.BugetBVCParametriiCheltuieli, L("Adaugare cheltuieli estimate"));
            context.CreatePermission(PermissionNames.BugetBVCPrevazutAcces, L("Vizualizare bugete prevazute"));
            context.CreatePermission(PermissionNames.BugetBVCPrevazutModificare, L("Adaugare buget, modificare sume calculate"));
            context.CreatePermission(PermissionNames.BugetBVCPrevazutValidare, L("Validare buget prevazut"));
            context.CreatePermission(PermissionNames.BugetBVCPrevazutSchimbareStare, L("Schimbare stare BVC prevazut dupa validare"));
            context.CreatePermission(PermissionNames.BugetBVCVenituriAcces, L("Vizualizare configurare venituri / incasari"));
            context.CreatePermission(PermissionNames.BugetBVCVenituriModificare, L("Adaugare / modificare / stergere venituri si incasari. Modificare sume reinvestite. Aplicare venituri / incasari in buget"));
            context.CreatePermission(PermissionNames.BugetBVCRealizatAcces, L("Vizualizare buget realizat"));
            context.CreatePermission(PermissionNames.BugetBVCRealizatModificare, L("Adaugare / stergere calcul buget realizat"));
            context.CreatePermission(PermissionNames.BugetBVCRapoarte, L("Vizualizare rapoarte buget"));
            context.CreatePermission(PermissionNames.BugetPAAPAcces, L("Vizualizare lista PAAP si detaliile unei achizitii"));
            context.CreatePermission(PermissionNames.BugetPAAPModificare, L("Adaugare / modificare / stergere PAAP, modificare cote TVA, adaugare referate, Amanare PAAP"));
            context.CreatePermission(PermissionNames.BugetPAAPAprobare, L("Aproba / revoca aprobarea pentru PAAP, anuleaza o achizitie"));
            context.CreatePermission(PermissionNames.BugetPAAPSuper, L("Utilizatorul are dreptul sa inregistreze si sa vizualizeze PAAP pentru toate compartimentele"));
            context.CreatePermission(PermissionNames.BugetPAAPAlocareFacturi, L("Utilizatorul are dreptul sa aloce facturi la o achizitie"));
            context.CreatePermission(PermissionNames.BugetCursValutarEstimatAcces, L("Vizualizare lista curs valutar estimat"));
            context.CreatePermission(PermissionNames.BugetCursValutarEstimatModificare, L("Adaugare / modificare / stergere curs valutar estimat"));
            context.CreatePermission(PermissionNames.CasierieOPAcces, L("Vizualizare lista OP-uri"));
            context.CreatePermission(PermissionNames.CasierieOPModificare, L("Adaugare / modificare / stergere OP"));
            context.CreatePermission(PermissionNames.CasierieOPValidare, L("Validare OP-uri"));
            context.CreatePermission(PermissionNames.CasierieNumerarDispozitiiAcces, L("Vizualizare lista dispozitii casierie"));
            context.CreatePermission(PermissionNames.CasierieNumerarDispozitiiModificare, L("Adaugare / modificare / stergere dispozitii"));
            context.CreatePermission(PermissionNames.CasierieNumerarDepunRetragAcces, L("Vizualizare depuneri / retrageri"));
            context.CreatePermission(PermissionNames.CasierieNumerarDepunRetragModificare, L("Adaugare / modificare / stergere depunere/retragere"));
            context.CreatePermission(PermissionNames.CasierieNumerarCupiuriAcces, L("Vizualizare cupiuri"));
            context.CreatePermission(PermissionNames.CasierieNumerarCupiuriModificare, L("Adaugare / modificare / stergere cupiuri"));
            context.CreatePermission(PermissionNames.CasierieNumerarSoldInitialAcces, L("Vizualizare sold initial"));
            context.CreatePermission(PermissionNames.CasierieNumerarSoldInitialModificare, L("Adaugare / modificare / stergere sold initial"));
            context.CreatePermission(PermissionNames.CasierieSchimbValutarAcces, L("Vizualizare schimb valutar"));
            context.CreatePermission(PermissionNames.CasierieSchimbValutarModificare, L("Adaugare / modificare / stergere schimb valutar"));
            context.CreatePermission(PermissionNames.CasierieRapoarteAcces, L("Acces rapoarte Casierie "));
            context.CreatePermission(PermissionNames.ContaDocumenteAcces, L("Vizualizare lista documente primite sau emise"));
            context.CreatePermission(PermissionNames.ContaDocumenteModificare, L("Adaugare / modificare / stergere documente, adaugare documente din deconturi"));
            context.CreatePermission(PermissionNames.ContaDeconturiAcces, L("Vizualizare deconturi"));
            context.CreatePermission(PermissionNames.ContaDeconturiModificare, L("Adaugare / modificare / stergere deconturi"));
            context.CreatePermission(PermissionNames.ContaOperContabOperContabAcces, L("Vizualizare operatii contabile si detaliile unei note contabile"));
            context.CreatePermission(PermissionNames.ContaOperContabOperContabModificare, L("Adaugare / modificare / stergere operatie contabila"));
            context.CreatePermission(PermissionNames.ContaOperContabOperContabStergOperExter, L("Stergere operatii contabile generate din alte module"));
            context.CreatePermission(PermissionNames.ContaOperContabExtrasAcces, L("Vizualizare Operatii generate din importul extrasului de cont"));
            context.CreatePermission(PermissionNames.ContaOperContabExtrasModificare, L("Import extras, sterge extras, generare note contabile, vizualizare si editare dictionar"));
            context.CreatePermission(PermissionNames.ContaMFIntrariAcces, L("Vizualizare intrari mijloace fixe si detaliile unui MF"));
            context.CreatePermission(PermissionNames.ContaMFIntrariModificare, L("Adaugare intrare, editare, stergere, dare in folosinta MF"));
            context.CreatePermission(PermissionNames.ContaMFOperatiiAcces, L("Vizualizare operatii mijloace fixe"));
            context.CreatePermission(PermissionNames.ContaMFOperatiiModificare, L("Adaugare/modificare/stergere operatie mijloace fixe"));
            context.CreatePermission(PermissionNames.ContaMFInventarAcces, L("Vizualizare inventariere MF"));
            context.CreatePermission(PermissionNames.ContaMFInventarModificare, L("Adaugare / modificare / stergere inventariere MF"));
            context.CreatePermission(PermissionNames.ContaMFRapoarteAcces, L("Rapoarte MF"));
            context.CreatePermission(PermissionNames.ContaObInventarIntrariAcces, L("Vizualizare intrari obiecte de inventar si detaliile unui obiect"));
            context.CreatePermission(PermissionNames.ContaObInventarIntrariModificare, L("Adaugare intrare, editare, stergere obiecte de inventar"));
            context.CreatePermission(PermissionNames.ContaObInventarOperatiiAcces, L("Vizualizare operatii obiecte de inventar"));
            context.CreatePermission(PermissionNames.ContaObInventarOperatiiModificare, L("Adaugare/modificare/stergere operatie obiecte de inventar"));
            context.CreatePermission(PermissionNames.ContaObInventarInventarAcces, L("Vizualizare inventariere obiecte de inventar"));
            context.CreatePermission(PermissionNames.ContaObInventarInventarModificare, L("Adaugare / modificare / stergere inventariere obiecte de inventar"));
            context.CreatePermission(PermissionNames.ContaObInventarRapoarteAcces, L("Rapoarte Obiecte de inventar"));
            context.CreatePermission(PermissionNames.ContaCheltAvansCheltAcces, L("Vizualizare cheltuieli in avans si detaliile unei cheltuieli"));
            context.CreatePermission(PermissionNames.ContaCheltAvansCheltModificare, L("Adaugare / modificare / stergere cheltuiala"));
            context.CreatePermission(PermissionNames.ContaCheltAvansRapoarteAcces, L("Rapoarte cheltuieli in avans"));
            context.CreatePermission(PermissionNames.ContaVenitAvansVenituriAcces, L("Vizualizare venituri in avans si detaliile unui venit"));
            context.CreatePermission(PermissionNames.ContaVenitAvansVenituriModificare, L("Adaugare / modificare / stergere venituri"));
            context.CreatePermission(PermissionNames.ContaVenitAvansVenituriRapoarte, L("Rapoarte venituri in avans"));
            context.CreatePermission(PermissionNames.ContaBalantaBalanteAcces, L("Vizualizare balante calculate"));
            context.CreatePermission(PermissionNames.ContaBalantaBalanteModificare, L("Calcul si stergere balanta"));
            context.CreatePermission(PermissionNames.ContaBalantaSfarsitLunaAcces, L("Vizualizare note generate pentru sfarsitul de luna"));
            context.CreatePermission(PermissionNames.ContaBalantaSfarsitLunaModificare, L("Generare si stergere note pentru sfarsitul de luna"));
            context.CreatePermission(PermissionNames.ContaBalantaBalanteSalvateAcces, L("Vizualizare balanta salvata"));
            context.CreatePermission(PermissionNames.ContaBalantaBalanteSalvateModificare, L("Salvare balanta calculata si stergere balanta salvata"));
            context.CreatePermission(PermissionNames.ContaSitFinanCalculAcces, L("Calcul situatii financiare"));
            context.CreatePermission(PermissionNames.ContaSitFinanConfigAcces, L("Configurare situatii financiare"));
            context.CreatePermission(PermissionNames.ContaSitFinanRapoarteAcces, L("Vizualizare rapoarte situatii financiare"));
            context.CreatePermission(PermissionNames.ContaBNRConturiAcces, L("Adaugare calcul pozitii din contabilitate si sectorizarea conturilor"));
            context.CreatePermission(PermissionNames.ContaBNRConfigurareAcces, L("Modificare formule de calcul pentru pozitiile din contabilitate"));
            context.CreatePermission(PermissionNames.ContaBNRRaportareAcces, L("Calcul Raportare BNR"));
            context.CreatePermission(PermissionNames.ContaLichConfigurareAcces, L("Modificare formule de calcul"));
            context.CreatePermission(PermissionNames.ContaLichCalculAcces, L("Calcul si vizualizare rapoarte"));
            context.CreatePermission(PermissionNames.ContaRapoarteRapoarteAcces, L("Vizualizare rapoarte"));
            context.CreatePermission(PermissionNames.ContaRapoarteRapoarteBalanta, L("Vizualizare raport Balanta"));
            context.CreatePermission(PermissionNames.ContaRapoarteRapoarteBalantaValuta, L("Vizualizare raport Balanta in valuta"));
            context.CreatePermission(PermissionNames.ContaRapoarteRapoarteFisaCont, L("Vizualizare raport Fisa de cont"));
            context.CreatePermission(PermissionNames.ContaRapoarteRapoarteRegistruJurnal, L("Vizualizare raport Registru Jurnal"));
            context.CreatePermission(PermissionNames.ContaRapoarteRapoarteRegistruInventar, L("Vizualizare raport Registru inventar"));
            context.CreatePermission(PermissionNames.ContaRapoarteRapoarteSolduriConturiCurente, L("Vizualizare raport Solduri Conturi Curente"));
            context.CreatePermission(PermissionNames.ContaRapoarteRapoarteSoldFurnizoriDebitori, L("Vizualizare raport Sold Furnizori Debitori"));
            context.CreatePermission(PermissionNames.ContaRapoarteRapoarteCursValutarBNR, L("Vizualizare raport Curs valutar BNR"));
            context.CreatePermission(PermissionNames.ContaRapoarteRapoarteDepoziteBancare, L("Vizualizare raport Depozite bancare"));
            context.CreatePermission(PermissionNames.ContaRapoarteRapoarteFacturi, L("Vizualizare raport Facturi"));
            context.CreatePermission(PermissionNames.ContaRapoarteRapoarteConfigurabileAcces, L("Vizualizare rapoarte configurabile"));
            context.CreatePermission(PermissionNames.ImprumuturiAcces, L("Acces modul Imprumuturi"));
            context.CreatePermission(PermissionNames.AdministrareContracteBeneficiariAcces, L("Modificare nomenclator beneficiari contracte"));
            context.CreatePermission(PermissionNames.AdministrareMFNomenclatoareAcces, L("Modificare nomenclatoare MF"));
            context.CreatePermission(PermissionNames.AdministrareMFSetupAcces, L("Setup MF"));
            context.CreatePermission(PermissionNames.AdministrareMFCalculGestiuneAcces, L("Calcul gestiune MF"));
            context.CreatePermission(PermissionNames.AdministrareObInventarNomenclatoareAcces, L("Modificare nomenclatoare obiecte de inventar"));
            context.CreatePermission(PermissionNames.AdministrareObInventarCalculGestiuneAcces, L("Calcul gestiune obiecte de inventar"));
            context.CreatePermission(PermissionNames.AdministrareCheltAvansTipuriDocAcces, L("Modificare nomenclator tipuri documente"));
            context.CreatePermission(PermissionNames.AdministrareCheltAvansCalculGestiuneAcces, L("Calcul gestiune cheltuieli in avans"));
            context.CreatePermission(PermissionNames.AdministrareCheltAvansSetupAcces, L("Setup cheltuieli in avans"));
            context.CreatePermission(PermissionNames.AdministrareVenitAvansTipuriDocAcces, L("Modificare nomenclator tipuri documente"));
            context.CreatePermission(PermissionNames.AdministrareVenitAvansCalculGestiuneAcces, L("Calcul gestiune venituri in avans"));
            context.CreatePermission(PermissionNames.AdministrareVenitAvansSetupAcces, L("Setup venituri in avans"));
            context.CreatePermission(PermissionNames.AdminContaPlanConturiAcces, L("Vizualizare plan de conturi"));
            context.CreatePermission(PermissionNames.AdminContaPlanConturiModificare, L("Adaugare / modificare / stergere conturi si relatii conturi"));
            context.CreatePermission(PermissionNames.AdminContaTipDocAcces, L("Vizualizare si modificare nomenclator tipuri documente"));
            context.CreatePermission(PermissionNames.AdminContaTipOperContabAcces, L("Vizualizare si modificare tipuri operatii contabile"));
            context.CreatePermission(PermissionNames.AdminContaMonografiiAcces, L("Configurare monografii operatii contabile"));
            context.CreatePermission(PermissionNames.AdminContaConfigConturiAcces, L("Configurare generare conturi contabile"));
            context.CreatePermission(PermissionNames.AdminContaCategElemFacturaAcces, L("Definire categorii elemente facturi"));
            context.CreatePermission(PermissionNames.AdminContaElemFacturaAcces, L("Definire elemente facturi"));
            context.CreatePermission(PermissionNames.AdminContaTipActivitateAcces, L("Definire tipuri de activitate (fonduri)"));
            context.CreatePermission(PermissionNames.AdminContaContariAutomateAcces, L("Generare manuala contari mf, obiecte de inventar"));
            context.CreatePermission(PermissionNames.AdminContaFacturiAcces, L("Generare manuala facturi"));
            context.CreatePermission(PermissionNames.AdminContaDiurnaZiAcces, L("Nomenclator diurna zilnica"));
            context.CreatePermission(PermissionNames.AdminContaDiurnaLegalaAcces, L("Nomenclator diurna legala"));
            context.CreatePermission(PermissionNames.AdminContaConfigRapoarteAcces, L("Configurare rapoarte bazate pe solduri si rulaje"));
            context.CreatePermission(PermissionNames.AdminContaCotaTVAAcces, L("Modificare Cota TVA"));
            context.CreatePermission(PermissionNames.AdminContaSectoareBNRAcces, L("Nomenclator Sectoare BNR"));
            context.CreatePermission(PermissionNames.AdminContaFileDocErrors, L("Vizualizare lista erori import documente din FileDoc"));
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, ErpConsts.LocalizationSourceName);
        }
    }
}
