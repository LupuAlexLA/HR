using Abp.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Niva.Erp.Conta.AutoOperation;
using Niva.Erp.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Niva.Erp.Web.Host.Controllers
{
    [AllowAnonymous]    
    public class AutomatController : AbpController
    {
        // http://localhost:21122/automat/ContaOperationSave

        IAutomatAppService _automatService;

        public AutomatController(IAutomatAppService automatService) : base()
        {
            _automatService = automatService;
        }

        [HttpGet]
        [AllowAnonymous]
        public int ValidateToken(string tokenId)
        {
            int rez = _automatService.ValidateToken(tokenId);
            return rez;
        }

        [HttpGet]
        [AllowAnonymous]
        public int FindAccount(string tokenId, string partialSymbol, string externalCode, string accountFuncType, int exactAccount)
        {
            var ret = _automatService.FindAccount(tokenId, partialSymbol, externalCode, accountFuncType, exactAccount);
            return ret;
        }

        // GET - search for an account => return 0 if yes, 1 if not
        [HttpGet]
        [AllowAnonymous]
        public JsonResult GetAccount(string tokenId, string partialSymbol, string externalCode, string accountFuncType, int exactAccount)
        {
            var rez = _automatService.GetAccount(tokenId, partialSymbol, externalCode, accountFuncType, exactAccount);
            var ret = Json(rez);
            return ret;
        }

        [HttpGet]
        [AllowAnonymous]
        public string GetNewAccount(string tokenId, string partialSymbol, string accountName, string externalCode, int accountFuncType, int nrOfFigures)
        {
            var rez = _automatService.GetNewAccount(tokenId, partialSymbol, accountName, externalCode, accountFuncType, nrOfFigures);
            return rez;
        }

        [HttpGet]
        [AllowAnonymous]
        public string SaveNewAccount(string tokenId, string symbol, string synthetic, string accountName, string externalCode, string thirdPartyCif, string currency, int accountFuncType)
        {
            string message = _automatService.SaveNewAccount(tokenId, symbol, synthetic, accountName, externalCode, thirdPartyCif, currency, accountFuncType);
            return message;
        }

        // GET - VerifyClosedMonth
        [HttpGet]
        [AllowAnonymous]
        public string VerifyClosedMonth(string tokenId, string operationDate)
        {
            string message = _automatService.VerifyClosedMonth(tokenId, operationDate);
            return message;
        }

        // Add operation header
        [HttpGet]
        [AllowAnonymous]
        public JsonResult AddOperation(string tokenId, string operationDate, string documentNr, string documentDate, int closingMonth, int operationStatus, string currency, string documentType)
        {
            int idOperation = 0;
            var rez = new ResultMessageIdDto();
            string message = "OK";
            try
            {
                var addOperResult = _automatService.AddOperation(tokenId, operationDate, documentNr, documentDate, closingMonth, operationStatus, currency, documentType);
                rez.Message = addOperResult.Message; rez.Id = addOperResult.Id;
                return Json(rez);
            }
            catch (Exception ex)
            {
                message = ex.Message;
                rez.Message = message;
            }
            return Json(rez);
        }

        // Add operation detail
        [HttpGet]
        [AllowAnonymous]
        public JsonResult AddOperationDetail(string tokenId, int operationId, string debitSymbol, string creditSymbol, decimal value, decimal valueCurrency, string details)
        {
            string message = "OK";
            try
            {
                message = _automatService.AddOperationDetail(tokenId, operationId, debitSymbol, creditSymbol, value, valueCurrency, details.Replace("$", " "));
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            var rez = new { rezMessage = message };
            return Json(rez);
        }

        // delete operation
        [HttpGet]
        [AllowAnonymous]
        public JsonResult DeleteOperation(string tokenId, int idOperation)
        {
            string message = "OK";
            try
            {
                message = _automatService.DeleteOperation(tokenId, idOperation);
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            var rez = new { rezMessage = message };
            return Json(rez);
        }

        [HttpGet]
        [AllowAnonymous]
        public JsonResult GetAccounts(string tokenId, string synthetic, string includeSynthetic)
        {
            string message = "OK";
            try
            {
                var x = _automatService.GetAccounts(tokenId, synthetic, includeSynthetic);
                return Json(x);
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return Json(message);
        }

        [HttpGet]
        [AllowAnonymous]
        public JsonResult GetSoldAccount(string tokenId, string account, string soldDate)
        {
            decimal sold = 0;
            string message = "OK";
            try
            {
                var format = new System.Globalization.CultureInfo("fr-FR", true);
                DateTime _soldDate = DateTime.Parse(soldDate, format);
                message = _automatService.GetSoldAccount(tokenId, account, _soldDate, out sold);
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            var rez = new { rezMessage = message, sold = sold };
            return Json(rez);
        }

        [HttpGet]
        [AllowAnonymous]
        public string SaveExchangeRate(string currencyCode, string exchangeDate, decimal value)
        {
            string message = "OK";
            var format = new System.Globalization.CultureInfo("fr-FR", true);
            DateTime _exchangeDate = DateTime.Parse(exchangeDate, format);

            message = _automatService.ExchangeRatesAddModify(currencyCode, _exchangeDate, value);

            return message;
        }

        [HttpGet]
        [AllowAnonymous]
        public string AddModifyThirdParty(string tokenId, string personType, string id1, string id2, string name, string lastName, string address, string countryCode, string regionCode, string locality)
        {
            string message = _automatService.AddModifyThirdParty(tokenId, personType, id1, id2, name, lastName, address, countryCode, regionCode, locality);
            return message;
        }

        [HttpGet]
        [AllowAnonymous]
        public string AddModifyBank(string tokenId, string personType, string id1, string id2, string name, string lastName, string address, string countryCode, string regionCode, string locality, string bic, string radIban)
        {
            string message = _automatService.AddModifyBank(tokenId, personType, id1, id2, name, lastName, address, countryCode, regionCode, locality, bic, radIban);
            return message;
        }

        [HttpGet]
        [AllowAnonymous]
        public string AddModifyBankAccount(string tokenId, string thirdParty, string bank, string currency, string iban)
        {
            string message = _automatService.AddModifyBankAccount(tokenId, thirdParty, bank, currency, iban);
            return message;
        }

        // Add operation header
        [HttpGet]
        [AllowAnonymous]
        public JsonResult ContaOperationDelete(int idOperation)
        {
            var rez = new ResultMessageIdDto();
            string message = "OK";
            try
            {
                message = _automatService.ContaOperationDelete(idOperation);
                rez.Message = message;
                return Json(rez);
            }
            catch (Exception ex)
            {
                message = ex.Message;
                rez.Message = message;
            }
            return Json(rez);
        }

        // Add operation header
        [HttpGet]
        [AllowAnonymous]
        public JsonResult ContaOperationSave(string operationDate, string documentNr, string documentDate, string currency, string activityType, int operType, int operationType)
        {
            var rez = new ResultMessageIdDto();
            string message = "OK";
            try
            {
                var addOperResult = _automatService.ContaOperationSaveCtl(operationDate, documentNr, documentDate, currency, activityType, operType, operationType);
                rez.Message = addOperResult.Message; rez.Id = addOperResult.Id;
                return Json(rez);
            }
            catch (Exception ex)
            {
                message = ex.Message;
                rez.Message = message;
            }
            return Json(rez);
        }

        // Add operation header
        [HttpGet]
        [AllowAnonymous]
        public JsonResult ContaOperationDetailSave(int idOperation, string activityType, int operType, int operationType, int valueType, decimal value, string bankAccount, string explicatii)
        {
            var rez = new ResultMessageIdDto();
            string message = "OK";
            try
            {
                message = _automatService.ContaOperationDetailSaveCtl(idOperation, activityType, operType, operationType, valueType, value, bankAccount, explicatii);
                rez.Message = message;
                return Json(rez);
            }
            catch (Exception ex)
            {
                message = ex.Message;
                rez.Message = message;
            }
            return Json(rez);
        }

        // Add operation header
        [HttpGet]
        [AllowAnonymous]
        public JsonResult ContaOperationEndSave(int idOperation)
        {
            var rez = new ResultMessageIdDto();
            string message = "OK";
            try
            {
                message = _automatService.ContaOperationEndSave(idOperation);
                rez.Message = message;
                return Json(rez);
            }
            catch (Exception ex)
            {
                message = ex.Message;
                rez.Message = message;
            }
            return Json(rez);
        }

        // Add operation header
        [HttpGet]
        [AllowAnonymous]

        public JsonResult GetExchangeRate(string currency, string exchangeDate, string tipRet) // tipRet = OPER sau REEV
        {
            var rez = new ResultMessageValueDto();
            string message = "OK";
            try
            {
                decimal exchangeRate = 0;
                var result = _automatService.GetExchangeRateCtl(currency, exchangeDate, tipRet, out exchangeRate);
                rez.Message = result; rez.Value = exchangeRate;
                return Json(rez);
            }
            catch (Exception ex)
            {
                message = ex.Message;
                rez.Message = message;
            }
            return Json(rez);
        }

        

    }
}
