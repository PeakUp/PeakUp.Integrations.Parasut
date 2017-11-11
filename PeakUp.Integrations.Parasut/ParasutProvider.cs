using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PeakUp.Integrations.Parasut.Client;
using RestSharp;

namespace PeakUp.Integrations.Parasut
{
    using Helper;
    using Model;
    using Utilities;

    public class ParasutProvider
    {
        public ParasutClient Client { get; protected set; }
        public string Url { get; set; }
        public ParasutProvider(string url = null, string version = null, string clientId = null, string clientSecret = null, string username = null, string password = null)
        {
            Client = new ParasutClient(url, version, clientId, clientSecret, username, password);
            Url = Client.Url;
        }

        Parasut.Model.InlineResponse2002 RawAccounts(string companyId, int page = 1, int count = 25) => Client.Contacts.ListContacts(companyId: companyId.ToInt(), pageNumber: page, pageSize: count);
        /// <summary>
        /// Past transactions uç noktası v1'de olmasına rağmen v4'te olmadığı için v1 uç noktasına istek yapar.
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="accountId"></param>
        /// <param name="page"></param>
        /// <param name="count"></param>
        /// <returns>Dynamic cevap. Şablon aşağıdaki gibi ; 
        /// {
        ///  "id": 11867951,
        ///  "date": "2017-05-14",
        ///  "description": "CSP",
        ///  "item_id": 3364921,
        ///  "contact": null,
        ///  "transaction_type": "sales_invoice",
        ///  "debit_credit": "debit",
        ///  "amount": "166.66",
        ///  "currency": "USD",
        ///  "other_amount": null,
        ///  "other_currency": null,
        ///  "amount_in_trl": "597.66",
        ///  "cancelled": false
        ///}
        /// </returns>
        dynamic RawInvoices(string companyId, string accountId, int page = 1, int count = 15) => new RestClient($"{Url}/v1").With(c => c.AddDefaultHeader("Authorization", $"Bearer {Client.Configuration.AccessToken}")).ExecuteDynamic(
                new RestRequest("{company_id}/contacts/{account_id}/past_transactions", Method.GET)
                    .AddUrlSegment("company_id", companyId)
                    .AddUrlSegment("account_id", accountId)
                    .AddQueryParameter("page", page.ToString())
                    .AddQueryParameter("per_page", count.ToString())
            ).Data;



        ParasutInvoice InvoiceFromDynamic(dynamic item) => new ParasutInvoice
        {
            Id = item.item_id.ToString(),
            Date = DateTime.ParseExact(item.date, "yyyy-MM-dd", null),
            Description = item.description,
            Type = item.transaction_type,
            Amount = StringExtensions.ToDouble(item?.amount),
            Currency = item.currency,
            OtherAmount = StringExtensions.ToDouble(item?.other_amount),
            AmountInTRL = StringExtensions.ToDouble(item?.amount_in_trl),
            Cancelled = item.cancelled,
            OtherCurrency = item.other_currency,
            ExchangeRate = GetExchangeRate(DateTime.ParseExact(item.date, "yyyy-MM-dd", null), item.currency)
        };

        /*
         [
             [
               "Date",
               "Currency Code",
               "Unit",
               "Currency Name",
               "Forex Selling"
             ],
             // [1]
             [ 
               "6/14/2017",
               "USD",
               1,
               "US DOLLAR",
               3.5199 // [4]
             ]
         ]
        */
        private double GetExchangeRate(DateTime date, string currency)
        {
            var data = new RestClient("http://peakupexchangerates.azurewebsites.net/").ExecuteDynamic(
                new RestRequest("api/Crawler", Method.POST).AddJsonBody(new
                {
                    BeginDate = date,
                    EndDate = date,
                    SelectedCodes = new[] { currency },
                    SelectedTypes = new[] { "Forex Selling" }
                })
            ).Data;
            return data.Count > 1 && data[1].Count > 4 ? data[1][4] : 0;
        }

        //void RawInvoices(string accountId, int page = 1, int count = 15) => client
        public IEnumerable<InlineResponse2001Included> Companies() => Client.Home.ShowMe().Included.Where(x => x.Type == InlineResponse2001Included.TypeEnum.Companies);

        public IEnumerable<InlineResponse2002Data> Accounts(string companyId)
        {
            Parasut.Model.InlineResponse2002 response = null;
            while (response == null || response.Meta.CurrentPage != response.Meta.TotalPages)
            {
                response = RawAccounts(companyId, (response?.Meta?.CurrentPage ?? 0) + 1);
                foreach (var item in response.Data)
                    yield return item;
            }
        }



        public IEnumerable<ParasutInvoice> Invoices(string companyId, string accountId)
        {
            dynamic response = null;
            int page = 1;
            while (response == null || page != response.meta.total_count)
            {
                response = RawInvoices(companyId, accountId, page);
                foreach (dynamic item in response.items)
                    yield return InvoiceFromDynamic(item);
                page++;
            }
        }

        public IPaged<InlineResponse2002Data> Accounts(string companyId, int page, int count = 15)
        {
            var raw = RawAccounts(companyId, page, count);
            return new Paged<InlineResponse2002Data>
            {
                CurrentPage = raw.Meta.CurrentPage ?? 1,
                TotalCount = raw.Meta.TotalCount ?? 1,
                TotalPages = raw.Meta.TotalPages ?? 1,
                Items = raw.Data
            };
        }



        public IPaged<ParasutInvoice> Invoices(string companyId, string accountId, int page, int count = 15)
        {
            var raw = RawInvoices(companyId, accountId, page, count);
            var paged = new Paged<ParasutInvoice>
            {
                CurrentPage = page,
                TotalCount = (int)raw.meta.total_count,
                TotalPages = (int)raw.meta.page_count
            };
            var items = new List<ParasutInvoice>();
            foreach (var item in raw?.items)
                items.Add(InvoiceFromDynamic(item));
            paged.Items = items;
            return paged;
        }

        // TODO: Account ve Invoice için tüm listeyi çekmeden yap.
        public InlineResponse2002Data Account(string companyId, string accountId) => Accounts(companyId).FirstOrDefault(x => x.Id == accountId);
        public ParasutInvoice Invoice(string companyId, string accountId, string invoiceId) => Invoices(companyId, accountId).FirstOrDefault(x => x.Id == invoiceId);
    }
}
