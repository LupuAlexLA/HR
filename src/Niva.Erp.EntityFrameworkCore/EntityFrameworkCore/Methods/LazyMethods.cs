using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace Niva.Erp.EntityFrameworkCore.Methods
{
    public class LazyMethods
    {
        public static string LinkGetPlasamenteBuget()
        {
            return "http://192.168.15.24:5000/api/external/getPlasamenteBuget/";
        }

        public class ListForEnum
        {
            public string Id { get; set; }
            public string Value { get; set; }
        }
        public static List<ListForEnum> ReturnEnumList(Type enumType)
        {
            var x = Enum.GetNames(enumType).Select(t => new ListForEnum { Id = t, Value = t }).ToList();
            return x;
        }

        public static string ReturnOptionLabelForDD()
        {
            return "Not Selected";
        }

        public static string ReturnDateFormat()
        {
            return "dd/MM/yyyy";
        }

        public static string DateToString(DateTime date)
        {
            return date.Day.ToString().PadLeft(2, '0') + "." + date.Month.ToString().PadLeft(2, '0') + "." + date.Year.ToString();
        }

        public static string GetUniqueKey(int maxSize)
        {
            char[] chars = new char[60];
            chars =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
            byte[] data = new byte[1];
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetNonZeroBytes(data);
                data = new byte[maxSize];
                crypto.GetNonZeroBytes(data);
            }
            StringBuilder result = new StringBuilder(maxSize);
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }
            return result.ToString();
        }

        public static DateTime FirstDayNextMonth(DateTime date)
        {
            var ret = new DateTime(date.Year + (date.Month == 12 ? 1 : 0), (date.Month == 12 ? 1 : (date.Month + 1)), 1);
            return ret;
        }

        public static DateTime LastDayOfMonth(DateTime date)
        {
            DateTime firstOfNextMonth = new DateTime(date.Year, date.Month, 1).AddMonths(1);
            DateTime ret = firstOfNextMonth.AddDays(-1);

            return ret;
        }

        public static String GetSynthetic(string symbol)
        {
            var list = symbol.Split('.');
            return list[0];
        }

        public static String GetAnalythic(string symbol)
        {
            var list = symbol.Split('.');
            if (list.Count() == 1)
                return null;
            else
            {
                string vAnalythic = symbol.Substring(symbol.IndexOf('.') + 1);
                return vAnalythic;
            }
        }

        //cifre_to_litere
        public static string CifreToLitere(decimal numar)
        {
            string v_result;

            v_result = "";

            try
            {
                if (numar < 0)
                {
                    throw new Exception("Eroare generare numarul " + numar + " in litere: numarul trebuie sa fie > 0.");
                }
                else if (numar == 1)
                {
                    v_result = "(una)";
                }
                else
                {
                    v_result = RevSplitTranConc(numar);
                }
            }
            catch (Exception e)
            {
                throw new Exception("Eroare generare numarul " + numar + " in litere: " + e.Message);
            }

            return v_result;
        }

        // Rev_Split_Tran_Conc
        private static string RevSplitTranConc(decimal numar)
        {
            string reverse, rezultat, rezultat1, rezultat2, rezultat3, rezultat4;
            string x, x1, x2, x3, x4;
            int n;

            reverse = "";
            rezultat = "";
            rezultat1 = "";
            rezultat2 = "";
            rezultat3 = "";
            rezultat4 = "";
            x = numar.ToString();
            x = x.Replace(",", ".");
            n = x.Length;


            for (int i = n; i > 0; i--)
            {
                reverse += x.Substring(i - 1, 1);
            }
            //return reverse;

            if (n <= 3)
            {
                rezultat = szu(reverse, "u", "i", ""); //clasa unitatilor
            }
            if (n > 3 & n <= 6)
            {  //clasa miilor
                x1 = reverse.Substring(0, 3);
                x2 = reverse.Substring(3);
                rezultat1 = szu(x1, "u", "i", "");
                if (x2 != "1")
                {
                    rezultat2 = szu(x2, "a", "ua", "de") + "mii";
                }
                else
                {
                    rezultat2 = "omie";
                }
                rezultat = rezultat2 + rezultat1;
            }
            else if (n > 6 & n <= 9) //clasa milioanelor
            {
                x1 = reverse.Substring(0, 3);
                x2 = reverse.Substring(3, 3);
                x3 = reverse.Substring(6);
                rezultat1 = szu(x1, "u", "i", "");
                if (x2 != "100")
                {
                    if (x2 != "000")
                    {
                        rezultat2 = szu(x2, "a", "ua", "de") + "mii";
                    }
                }
                else
                {
                    rezultat2 = "omie";
                }
                if (x3 != "1")
                {
                    rezultat3 = szu(x3, "u", "ua", "de") + "milioane";
                }
                else
                {
                    rezultat3 = "unmilion";
                }
                rezultat = rezultat3 + rezultat2 + rezultat1;
            }
            else if (n > 9 & n <= 12) //clasa miliardelor
            {
                x1 = reverse.Substring(0, 3);
                x2 = reverse.Substring(3, 3);
                x3 = reverse.Substring(6, 3);
                x4 = reverse.Substring(9);
                rezultat1 = szu(x1, "u", "i", "");
                if (x2 != "100")
                {
                    if (x2 != "000")
                    {
                        rezultat2 = szu(x2, "a", "ua", "de") + "mii";
                    }
                }
                else
                {
                    rezultat2 = "omie";
                }
                if (x3 != "100")
                {
                    if (x3 != "000")
                    {
                        rezultat3 = szu(x3, "u", "ua", "de") + "milioane";
                    }
                }
                else
                {
                    rezultat4 = "unmilion";
                }
                if (x4 != "1")
                {
                    rezultat4 = szu(x4, "u", "ua", "de") + "miliarde";
                }
                else
                {
                    rezultat4 = "unmilard";
                }

                rezultat = rezultat4 + rezultat3 + rezultat2 + rezultat1;
            }

            return rezultat;
        }

        // szu
        private static string szu(string x, string sg, string pl, string prep)
        {
            string rezultat;
            int n, u, z, s, zu;

            rezultat = "";

            n = x.Length;
            u = int.Parse(x.Substring(0, 1)); //unitati
            if (n > 1)
            {
                z = int.Parse(x.Substring(1, 1));
            }
            else
            {
                z = 0;
            }
            if (n > 2)
            {
                s = int.Parse(x.Substring(2, 1)); //zeci si sute
            }
            else
            {
                s = 0;
            }

            //Array a = new Array{"", "un", "doua", "trei", "patru", "cinci", "sase", "sapte", "opt", "noua", "zece", "unsprezece", "doisprezece", "treisprezece", "paisprezece", "cincisprezece", "saisprezece", "saptesprezece", "optsprezece", "nouasprezece"};
            string[] a = { "", "un", "doua", "trei", "patru", "cinci", "sase", "sapte", "opt", "noua", "zece", "unsprezece", "doisprezece", "treisprezece", "paisprezece", "cincisprezece", "saisprezece", "saptesprezece", "optsprezece", "nouasprezece" };
            if (s == 1)
            {
                rezultat = "osuta";
            }
            if (s >= 2)
            {
                rezultat = a[s] + "sute";
            }
            if (z >= 2)
            {
                a[6] = "sai";

                rezultat = rezultat + a[z] + "zeci";
                a[6] = "sase";
                a[1] += sg;
                a[2] = "do" + pl;
                if (u > 0)
                {
                    rezultat += "si" + a[u] + "";
                }

                rezultat += prep;
            }
            else
            {
                if (z == 1)
                {
                    zu = int.Parse("1" + u);
                }
                else
                {
                    zu = u;
                }
                rezultat = rezultat + a[zu] + "";
            }

            return rezultat;
        }

        public static string GetErrMessage(Exception ex)
        {
            string err = "";

            while (ex != null)
            {
                err += ex.Message;
                ex = ex.InnerException;
            }
            return err;
        }

        public static DateTime Now()
        {
            var date = DateTime.Now;
            return new DateTime(date.Year, date.Month, date.Day);
        }

        public static int MonthsBetween(DateTime startDate, DateTime endDate)
        {
            int rez = (endDate.Year - startDate.Year) * 12 + endDate.Month - startDate.Month;
            return rez;
        }

        public static string ReturnStringFromEnum(Type enumType, List<string> tipRanduriList)
        {
            string enumValue = "";
            foreach (var item in Enum.GetValues(enumType))
            {
                if (tipRanduriList.Contains(((int)item).ToString()))
                {
                    enumValue += item.ToString() + ",";
                }
            }

            return enumValue;
        }

        public static DateTime GetQuarterByDate(DateTime date)
        {
            DateTime quarter = new DateTime();
            if (date.Month <= 3)
            {
                quarter = LazyMethods.LastDayOfMonth(new DateTime(date.Year, 3, 1));
            }
            else if (date.Month > 3 && date.Month <= 6)
            {
                quarter = LazyMethods.LastDayOfMonth(new DateTime(date.Year, 6, 1));
            }
            else if (date.Month > 6 && date.Month <= 9)
            {
                quarter = LazyMethods.LastDayOfMonth(new DateTime(date.Year, 9, 1));
            }
            else if (date.Month > 9 && date.Month <= 12)
            {
                quarter = LazyMethods.LastDayOfMonth(new DateTime(date.Year, 12, 1));
            }
            return quarter;
        }

        public static string EnumValueToDescription(Enum value)
        {
            FieldInfo field = value.GetType().GetField(value.ToString());
            DescriptionAttribute attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
            return attribute == null ? value.ToString() : attribute.Description;
        }

        public static DateTime MinDate(DateTime startDate, DateTime endDate)
        {
            return startDate < endDate ? startDate : endDate;
        }

        public static DateTime MaxDate(DateTime startDate, DateTime endDate)
        {
            return startDate > endDate ? startDate : endDate;
        }

    }
}
