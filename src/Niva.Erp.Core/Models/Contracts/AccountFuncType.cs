
using System.ComponentModel;

namespace Niva.Erp.Models.Contracts
{
 
 

	public enum AccountFuncType : int
	{
		Regular,
        [Description("Pozitie schimb cheltuieli")]
		PozitieSchimbCheltuieli,
        [Description("Contravaloare pozitie schimb cheltuieli")]
		ContravaloarePozitieSchimbCheltuieli,
        [Description("Obiecte de inventar")]
        ObiecteDeInventar,
        [Description("Mijloace fixe")]
        MijloaceFixe,
        [Description("Cheltuieli in avans")]
        CheltuieliInAvans,
        [Description("Venituri in avans")]
        VenituriInAvans,
        [Description("Cont bancar")]
        ContBancar,
        Sponsorizare,
        Protocol,
        Casierie,
        Stocuri,
        [Description("Profit sau pierdere")]
        ProfitSauPierdere,
        [Description("Pozitie schimb operatiuni")]
        PozitieSchimbOperatiuni,
        [Description("Contravaloare pozitie schimb operatiuni")]
        ContravaloarePozitieSchimbOperatiuni,
        [Description("Decont casa")]
        DecontCasa,
        [Description("Decont card")]
        DecontCard,
        [Description("Cheltuieli cu deplasarile")]
        CheltuieliCuDeplasarile,
        [Description("Cheltuieli diferente curs valutar operatiuni")]
        CheltuieliDiferenteCursValutarOperatiuni,
        [Description("Cheltuieli diferente curs valutar cheltuieli")]
        CheltuieliDiferenteCursValutarCheltuieli,
        [Description("Venituri diferente curs valutar operatiuni")]
        VenituriDiferenteCursValutarOperatiuni,
        [Description("Venituri diferente curs valutar cheltuieli")]
        VenituriDiferenteCursValutarCheltuieli,
        [Description("Pozitie schimb imprumuturi")]
        PozitieSchimbImprumuturi,
        [Description("Contravaloare pozitie imprumuturi")]
        ContravaloarePozitieImprumuturi,
        [Description("Venituri - cheltuieli diferente curs valutar imprumuturi")]
        VenituriCheltuieliDiferenteCursValutarImprumuturi
    }
}
