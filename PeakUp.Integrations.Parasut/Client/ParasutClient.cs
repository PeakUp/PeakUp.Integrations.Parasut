using PeakUp.Integrations.Parasut.Api;
using RestSharp;
using RestSharp.Deserializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeakUp.Integrations.Parasut.Client
{
    public class ParasutClient : ApiClient
    {
        public string Url { get; set; }
        public string Version { get; set; }
        public string CompanyId { get; set; }
        protected string ClientId { get; set; }
        protected string ClientSecret { get; set; }
        protected string Username { get; set; }
        protected string Password { get; set; }
        
        public ApiHomeApi Home { get; set; } = new ApiHomeApi();
        public AccountsApi Accounts { get; set; } = new AccountsApi();
        public BankFeesApi BankFees { get; set; } = new BankFeesApi();
        public ContactsApi Contacts { get; set; } = new ContactsApi();
        public EArchivesApi EArchives { get; set; } = new EArchivesApi();
        public EInvoiceInboxesApi EInvoiceInboxes { get; set; } = new EInvoiceInboxesApi();
        public EInvoicesApi EInvoices { get; set; } = new EInvoicesApi();
        public EmployeesApi Employees { get; set; } = new EmployeesApi();
        public ItemCategoriesApi ItemCategories { get; set; } = new ItemCategoriesApi();
        public ProductsApi Products { get; set; } = new ProductsApi();
        public PurchaseBillsApi PurchaseBills { get; set; } = new PurchaseBillsApi();
        public SalariesApi Salaries { get; set; } = new SalariesApi();
        public SalesInvoicesApi SalesInvoices { get; set; } = new SalesInvoicesApi();
        public TagsApi Tags { get; set; } = new TagsApi();
        public TaxesApi Taxes { get; set; } = new TaxesApi();
        public TrackableJobsApi TrackableJobs { get; set; } = new TrackableJobsApi();
        public TransactionsApi Transactions { get; set; } = new TransactionsApi();


        public ParasutClient(string url = null, string version = null, string companyId = null, string clientId = null, string clientSecret = null, string username = null, string password = null) : base(ParasutApiSettings.BaseUrl(url, version))
        {
            Url = ParasutApiSettings.Url(url);
            Version = ParasutApiSettings.Version(version);
            CompanyId = ParasutApiSettings.CompanyId(companyId);
            ClientId = ParasutApiSettings.ClientId(clientId);
            ClientSecret = ParasutApiSettings.ClientSecret(clientSecret);
            Username = ParasutApiSettings.Username(username);
            Password = ParasutApiSettings.Password(password);

            Configuration.AccessToken = GetParasutAccessToken();
        }

        string GetParasutAccessToken() => GetParasutAccessToken(ClientId, ClientSecret, Username, Password);

        string GetParasutAccessToken(string clientId, string clientSecret, string username, string password) =>
            new RestClient(Url).Execute<OAuthTokenResponse>(new RestRequest("oauth/token", Method.POST)
                .AddQueryParameter("client_id", clientId)
                .AddQueryParameter("client_secret", clientSecret)
                .AddQueryParameter("username", username)
                .AddQueryParameter("password", password)
                .AddQueryParameter("grant_type", "password")
                .AddQueryParameter("redirect_uri", "urn:ietf:wg:oauth:2.0:oob")
            )?.Data?.AccessToken;


    }

    public class OAuthTokenResponse
    {
        [DeserializeAs(Name = "access_token")]
        public string AccessToken { get; set; }

        [DeserializeAs(Name = "token_type")]
        public string TokenType { get; set; }

        [DeserializeAs(Name = "expires_in")]
        public int ExpiresIn { get; set; }

        [DeserializeAs(Name = "refresh_token")]
        public string RefreshToken { get; set; }
    }
}
