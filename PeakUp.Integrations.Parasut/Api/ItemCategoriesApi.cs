/* 
 * Paraşüt - API V4
 *
 * # GİRİŞ  ## API Hakkında  - API V4 test aşamasındadır.  - API geliştirmesinde çoğunlukla JSONAPI (http://jsonapi.org/) standartlarına uymaya çalıştık.  - Dökümantasyon oluşturulmasında ise OpenAPI-Swagger 2.0 kullandık.  - API hizmetimizin `BASE_URL`i `https://api.parasut.com` şeklindedir.  - V4 endpointlerine ulaşmak için `https://api.parasut.com/v4` şeklinde kullanabilirsiniz.  ## Genel Bilgiler  - API metodlarına erişmek için baz URL olarak `https://api.parasut.com/v4/firma_no` adresi kullanılır.   - Bu yapıda kullanılan `firma_no` parametresi bilgisine erişilmek istenin firmanın Paraşüt veritabanındaki kayıt numarasıdır.   - Örneğin 115 numaralı firmanın müşteri/tedarikçi listesine erişmek için `https://api.parasut.com/v4/115/contacts` adresi kullanılır. - İstekleri gönderirken `Content-Type` header'ı olarak `application/json` veya `application/vnd.api+json` göndermelisiniz. - Yeni bir kayıt oluştururken **ilgili** kaydın `ID` parametresini boş göndermeli veya hiç göndermemelisiniz.   - Örnek: Satış faturası oluştururken `data->id` boş olmalı, ama `relationships->contact->data->id` dolu olmalı, çünkü gönderdiğiniz müşterinizin ID'si daha önceden elinizde bulunmalıdır. - API endpointlerine ulaşmak için, aldığınız `access_token`'ı sorgulara `Authorization` header'ı olarak `Bearer access_token` şeklinde göndermelisiniz.  # Authentication  <!- - ReDoc-Inject: <security-definitions> - ->  Paraşüt API kimlik doğrulama için oAuth2 kullanmaktadır. Bu protokolü destekleyen istemci kütüphanelerini kullanarak oturum açabilir ve API'yi kullanabilirsiniz.  Kimlik doğrulama işleminin başarılı olması durumunda bir adet kimlik jetonu (authentication token) ve bir adet de yenileme jetonu (refresh token) gönderilecektir. Kimlik jetonu 2 saat süreyle geçerlidir ve her istekte http başlık bilgilerinin içerisinde gönderilmelidir. Bu sürenin sonunda kimlik jetonu geçerliliğini yitirecektir ve yenileme jetonu kullanılarak tekrar üretilmesi gerekmektedir.  ## access_token almak:  access_token almanız için iki farklı seçenek bulunmaktadır.  Kullanım şeklinize bağlı olarak iki yöntemden birini tercih etmelisiniz.  ### 1. grant_type=authorization_code  1. Kullanıcıyı şu adrese yönlendirin:    ```   BASE_URL/oauth/authorize?client_id=CLIENT_ID&redirect_uri=REDIRECT_URL&response_type=code   ```  2. Oturum açmışsa ve uygulamayı kabul ederse, kullanıcı sizin tanımladığınız REDIRECT_URL'e şu şekilde gelmesi gerekiyor:   `REDIRECT_URL?code=xxxxxxx`  3. Burada size gelen \"code\" parametresi ile access token almalısınız.  ```bash curl -F grant_type=authorization_code \\ -F client_id=CLIENT_ID \\ -F client_secret=CLIENT_SECRET \\ -F code=RETURNED_CODE \\ -F redirect_uri=REDIRECT_URL \\ -X POST BASE_URL/oauth/token ```  ### 2. grant_type=password  access_token almanız için aşağıdaki url'de size özel alanları doldurarak POST ile ulaşmanız gerekmektedir.  ```bash BASE_URL/oauth/token?client_id=CLIENT_ID&client_secret=CLIENT_SECRET&username=YOUREMAIL&password=YOURPASSWORD&grant_type=password&redirect_uri=urn:ietf:wg:oauth:2.0:oob ```  ### Sonuç  Her iki yöntem sonucunda size aşağıdaki gibi bir sonuç dönecektir:  ```json {  \"access_token\": \"XYZXYZXYZ\",  \"token_type\": \"bearer\",  \"expires_in\": 7200,  \"refresh_token\": \"ABCABCABC\" } ```  Burada dönen `access_token`'ı API endpointlerine ulaşmak için gönderdiğiniz sorgulara `Authorization` header'ı olarak `Bearer XYZXYZXYZ` şeklinde eklemeniz gerekiyor.   #### Refresh token ile yeni access_token alma örneği:  `access_token` geçerliliğini 2 saat içerisinde yitirdiği için `refresh_token` ile yeni token alabilirsiniz.  ```bash curl -F grant_type=refresh_token \\ -F client_id=CLIENT_ID \\ -F client_secret=CLIENT_SECRET \\ -F refresh_token=REFRESH_TOKEN \\ -X POST BASE_URL/oauth/token ```  `refresh_token` ile yeni bir `access_token` alırken aynı zamanda yeni bir `refresh_token` da almaktasınız. Dolayısıyla, daha sonra yeniden bir `access_token` alma isteğinizde size dönen yeni `refresh_token`ı kullanmalısınız.  # SIK KULLANILAN İŞLEMLER  ## Kullanıcı Bilgisi  access_token aldığınız kullanıcının genel bilgilerini görmek için [/me](/#operation/showMe) adresini kullanabilirsiniz.  ## Satış Faturası Oluşturma  Satış faturası oluşturmak için adım adım şunlara ihtiyacınız vardır:  - Müşteri ID'si   - [Müşteri listesi](/#operation/listContacts)   - [Müşteri oluşturma](/#operation/createContact) - Ürün ID'leri   - [Müşteri listesi](/#operation/listContacts)   - [Müşteri oluşturma](/#operation/createContact)  Yukarıdaki bilgilere sahip olduktan sonra [Satış Faturası Oluşturma](/#operation/createSalesInvoice) adımına geçebilirsiniz.   `relationships`:   - `details` kısmına fatura kalemlerini `array` şeklinde girmelisiniz.   - `contact` kısmına müşteri bilgisini girmelisiniz.  ## Satış Faturasına Tahsilat Ekleme  [Tahsilat ekleme](/#operation/paySalesInvoice) kısmındaki ilgili alanları doldurarak satış faturasına tahsilat ekleyebilirsiniz.  ## Satış Faturasının Tahsilatını Silme  Bir satış faturasının tahsilatını silmek aslında o tahsilatı yaratan para akışı - işlemi silmek demektir.   [Bir işlemi silmek](/#operation/deleteTransaction) için o işlemin ID'sini bilmek gerekir. Bir satış faturasına ait tahsilatları almak için [show](/#operation/showSalesInvoice) endpoint'ine istek atarken `?include=payments` parametresini de eklemelisiniz. Tahsilatların yanında işlemleri de almak isterseniz `?include=payments.tx` göndermelisiniz.   Daha sonra ilgili işlem-transaction ID'sini [işlem silme](/#operation/deleteTransaction) endpoint'inde kullanabilirsiniz.  ## Satış Faturası Resmileştirme  Satış faturasını [oluşturduktan](/#section/Basic-Actions/Satis-Faturasi-Olusturma) sonra e-Arşiv ve e-Fatura şeklinde resmileştirmek için adım adım şunları yapmalısınız:  1. Müşterinizin e-Fatura kullanıcısı olup olmadığını öğrenmelisiniz.   - Müşterinizin [e-Fatura gelen kutusu](/#operation/listEInvoiceInboxes) var ise e-Fatura kullanıcısıdır. 2. Eğer müşteriniz e-Fatura kullanıcısı ise:   - [e-Fatura oluşturmalısınız](/#operation/createEInvoice) 3. Eğer müşteriniz e-Fatura kullanıcısı değil ise:   - [e-Arşiv oluşturmalısınız](/#operation/createEArchive) 4. Her iki istek sonucunda da size takip edebileceğiniz bir işlem id'si dönecektir.   Bu işlemi [sorgulama](/#tag/TrackableJobs) adresi ile belirli aralıklarla sorgulayıp, durum kontrolü yapmalısınız.    - \"data->attributes->status\": \"running\" -> işlemin hala sürdüğü anlamına gelir.   - \"data->attributes->status\": \"error\" -> bir hata olduğu anlamına gelir.     - \"data->attributes->errors\": ile hataları görebilirsiniz.   - \"data->attributes->status\": \"done\" -> başarılı bir işlem anlamına gelir.  5. Resmileştirme detayları    Faturanız ile birlikte, faturanızın e-Arşiv/e-Fatura detaylarını görmek için [fatura bilgilerine](/#operation/showSalesInvoice) ulaşırken `?include=active_e_document` parametresini eklemeniz yeterlidir.  6. Başarılı resmileştirilen e-Arşiv    E-arşiv faturalarınızı başarılı bir şekilde resmileştirdikten sonra müşterinize PDF ile ulaştırabilirsiniz.   [e-Arşiv PDF](/#operation/showEArchivePdf) adresinden size dönen PDF'i indirip müşterinize gönderebilirsiniz. Unutmayın, bu URL 1 saat geçerli bir adrestir. Bu yüzden bu linki direk olarak müşterinizle <u>paylaşmamalısınız</u>. İndirip müşterinize ayrıca göndermelisiniz.  7. Başarılı resmileştirilen e-Fatura    Eğer e-fatura olarak başarılı bir şekilde resmileştirdiyseniz, yapmanız gereken başka bir işlem yoktur. Karşı taraf da e-fatura kullancısı olduğu için faturanız karşı tarafa otomatik olarak iletilecektir.   Eğer isterseniz yine de [e-Fatura PDF](/#operation/showEInvoicePdf) adresinden size dönen PDF'i indirip müşterinize gönderebilirsiniz. 
 *
 * OpenAPI spec version: 4.0.0
 * 
 * Generated by: https://github.com/swagger-api/swagger-codegen.git
 */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RestSharp;
using PeakUp.Integrations.Parasut.Client;
using PeakUp.Integrations.Parasut.Model;

namespace PeakUp.Integrations.Parasut.Api
{
    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
    public interface IItemCategoriesApi : IApiAccessor
    {
        #region Synchronous Operations
        /// <summary>
        /// Create
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="PeakUp.Integrations.Parasut.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="itemCategoryForm"></param>
        /// <param name="companyId">Firma ID</param>
        /// <param name="include">Response ile birlikte geri dönmesini istediğiniz ilişkiler - *Available: parent_category, subcategories* (optional)</param>
        /// <returns>InlineResponse2017</returns>
        InlineResponse2017 CreateItemCategory (ItemCategoryForm itemCategoryForm, int? companyId, string include = null);

        /// <summary>
        /// Create
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="PeakUp.Integrations.Parasut.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="itemCategoryForm"></param>
        /// <param name="companyId">Firma ID</param>
        /// <param name="include">Response ile birlikte geri dönmesini istediğiniz ilişkiler - *Available: parent_category, subcategories* (optional)</param>
        /// <returns>ApiResponse of InlineResponse2017</returns>
        ApiResponse<InlineResponse2017> CreateItemCategoryWithHttpInfo (ItemCategoryForm itemCategoryForm, int? companyId, string include = null);
        /// <summary>
        /// Delete
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="PeakUp.Integrations.Parasut.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="companyId">Firma ID</param>
        /// <param name="id">Kategori ID</param>
        /// <returns>Object</returns>
        Object DeleteItemCategory (int? companyId, int? id);

        /// <summary>
        /// Delete
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="PeakUp.Integrations.Parasut.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="companyId">Firma ID</param>
        /// <param name="id">Kategori ID</param>
        /// <returns>ApiResponse of Object</returns>
        ApiResponse<Object> DeleteItemCategoryWithHttpInfo (int? companyId, int? id);
        /// <summary>
        /// Index
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="PeakUp.Integrations.Parasut.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="companyId">Firma ID</param>
        /// <param name="filterName"> (optional)</param>
        /// <param name="filterCategoryType"> (optional)</param>
        /// <param name="sort">Sortable parameters - *Available: id, name, category_type* (optional, default to id)</param>
        /// <param name="pageNumber">Page Number (optional, default to 1)</param>
        /// <param name="pageSize">Page Size (optional, default to 15)</param>
        /// <param name="include">Response ile birlikte geri dönmesini istediğiniz ilişkiler - *Available: parent_category, subcategories* (optional)</param>
        /// <returns>InlineResponse2008</returns>
        InlineResponse2008 ListItemCategories (int? companyId, string filterName = null, string filterCategoryType = null, string sort = null, int? pageNumber = null, int? pageSize = null, string include = null);

        /// <summary>
        /// Index
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="PeakUp.Integrations.Parasut.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="companyId">Firma ID</param>
        /// <param name="filterName"> (optional)</param>
        /// <param name="filterCategoryType"> (optional)</param>
        /// <param name="sort">Sortable parameters - *Available: id, name, category_type* (optional, default to id)</param>
        /// <param name="pageNumber">Page Number (optional, default to 1)</param>
        /// <param name="pageSize">Page Size (optional, default to 15)</param>
        /// <param name="include">Response ile birlikte geri dönmesini istediğiniz ilişkiler - *Available: parent_category, subcategories* (optional)</param>
        /// <returns>ApiResponse of InlineResponse2008</returns>
        ApiResponse<InlineResponse2008> ListItemCategoriesWithHttpInfo (int? companyId, string filterName = null, string filterCategoryType = null, string sort = null, int? pageNumber = null, int? pageSize = null, string include = null);
        /// <summary>
        /// Kategori bilgisini gösterir.
        /// </summary>
        /// <remarks>
        /// Ayrıntısını görmek istediğiniz kategorinin id&#39;sini göndererek gerekli bilgilere ulaşabilirsiniz.
        /// </remarks>
        /// <exception cref="PeakUp.Integrations.Parasut.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="companyId">Firma ID</param>
        /// <param name="id">Kategori ID</param>
        /// <param name="include">Response ile birlikte geri dönmesini istediğiniz ilişkiler - *Available: parent_category, subcategories* (optional)</param>
        /// <returns>InlineResponse2017</returns>
        InlineResponse2017 ShowItemCategory (int? companyId, int? id, string include = null);

        /// <summary>
        /// Kategori bilgisini gösterir.
        /// </summary>
        /// <remarks>
        /// Ayrıntısını görmek istediğiniz kategorinin id&#39;sini göndererek gerekli bilgilere ulaşabilirsiniz.
        /// </remarks>
        /// <exception cref="PeakUp.Integrations.Parasut.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="companyId">Firma ID</param>
        /// <param name="id">Kategori ID</param>
        /// <param name="include">Response ile birlikte geri dönmesini istediğiniz ilişkiler - *Available: parent_category, subcategories* (optional)</param>
        /// <returns>ApiResponse of InlineResponse2017</returns>
        ApiResponse<InlineResponse2017> ShowItemCategoryWithHttpInfo (int? companyId, int? id, string include = null);
        /// <summary>
        /// Edit
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="PeakUp.Integrations.Parasut.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="itemCategoryForm"></param>
        /// <param name="companyId">Firma ID</param>
        /// <param name="id">Kategori ID</param>
        /// <param name="include">Response ile birlikte geri dönmesini istediğiniz ilişkiler - *Available: parent_category, subcategories* (optional)</param>
        /// <returns>InlineResponse2017</returns>
        InlineResponse2017 UpdateItemCategory (ItemCategoryForm1 itemCategoryForm, int? companyId, int? id, string include = null);

        /// <summary>
        /// Edit
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="PeakUp.Integrations.Parasut.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="itemCategoryForm"></param>
        /// <param name="companyId">Firma ID</param>
        /// <param name="id">Kategori ID</param>
        /// <param name="include">Response ile birlikte geri dönmesini istediğiniz ilişkiler - *Available: parent_category, subcategories* (optional)</param>
        /// <returns>ApiResponse of InlineResponse2017</returns>
        ApiResponse<InlineResponse2017> UpdateItemCategoryWithHttpInfo (ItemCategoryForm1 itemCategoryForm, int? companyId, int? id, string include = null);
        #endregion Synchronous Operations
        #region Asynchronous Operations
        /// <summary>
        /// Create
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="PeakUp.Integrations.Parasut.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="itemCategoryForm"></param>
        /// <param name="companyId">Firma ID</param>
        /// <param name="include">Response ile birlikte geri dönmesini istediğiniz ilişkiler - *Available: parent_category, subcategories* (optional)</param>
        /// <returns>Task of InlineResponse2017</returns>
        System.Threading.Tasks.Task<InlineResponse2017> CreateItemCategoryAsync (ItemCategoryForm itemCategoryForm, int? companyId, string include = null);

        /// <summary>
        /// Create
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="PeakUp.Integrations.Parasut.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="itemCategoryForm"></param>
        /// <param name="companyId">Firma ID</param>
        /// <param name="include">Response ile birlikte geri dönmesini istediğiniz ilişkiler - *Available: parent_category, subcategories* (optional)</param>
        /// <returns>Task of ApiResponse (InlineResponse2017)</returns>
        System.Threading.Tasks.Task<ApiResponse<InlineResponse2017>> CreateItemCategoryAsyncWithHttpInfo (ItemCategoryForm itemCategoryForm, int? companyId, string include = null);
        /// <summary>
        /// Delete
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="PeakUp.Integrations.Parasut.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="companyId">Firma ID</param>
        /// <param name="id">Kategori ID</param>
        /// <returns>Task of Object</returns>
        System.Threading.Tasks.Task<Object> DeleteItemCategoryAsync (int? companyId, int? id);

        /// <summary>
        /// Delete
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="PeakUp.Integrations.Parasut.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="companyId">Firma ID</param>
        /// <param name="id">Kategori ID</param>
        /// <returns>Task of ApiResponse (Object)</returns>
        System.Threading.Tasks.Task<ApiResponse<Object>> DeleteItemCategoryAsyncWithHttpInfo (int? companyId, int? id);
        /// <summary>
        /// Index
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="PeakUp.Integrations.Parasut.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="companyId">Firma ID</param>
        /// <param name="filterName"> (optional)</param>
        /// <param name="filterCategoryType"> (optional)</param>
        /// <param name="sort">Sortable parameters - *Available: id, name, category_type* (optional, default to id)</param>
        /// <param name="pageNumber">Page Number (optional, default to 1)</param>
        /// <param name="pageSize">Page Size (optional, default to 15)</param>
        /// <param name="include">Response ile birlikte geri dönmesini istediğiniz ilişkiler - *Available: parent_category, subcategories* (optional)</param>
        /// <returns>Task of InlineResponse2008</returns>
        System.Threading.Tasks.Task<InlineResponse2008> ListItemCategoriesAsync (int? companyId, string filterName = null, string filterCategoryType = null, string sort = null, int? pageNumber = null, int? pageSize = null, string include = null);

        /// <summary>
        /// Index
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="PeakUp.Integrations.Parasut.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="companyId">Firma ID</param>
        /// <param name="filterName"> (optional)</param>
        /// <param name="filterCategoryType"> (optional)</param>
        /// <param name="sort">Sortable parameters - *Available: id, name, category_type* (optional, default to id)</param>
        /// <param name="pageNumber">Page Number (optional, default to 1)</param>
        /// <param name="pageSize">Page Size (optional, default to 15)</param>
        /// <param name="include">Response ile birlikte geri dönmesini istediğiniz ilişkiler - *Available: parent_category, subcategories* (optional)</param>
        /// <returns>Task of ApiResponse (InlineResponse2008)</returns>
        System.Threading.Tasks.Task<ApiResponse<InlineResponse2008>> ListItemCategoriesAsyncWithHttpInfo (int? companyId, string filterName = null, string filterCategoryType = null, string sort = null, int? pageNumber = null, int? pageSize = null, string include = null);
        /// <summary>
        /// Kategori bilgisini gösterir.
        /// </summary>
        /// <remarks>
        /// Ayrıntısını görmek istediğiniz kategorinin id&#39;sini göndererek gerekli bilgilere ulaşabilirsiniz.
        /// </remarks>
        /// <exception cref="PeakUp.Integrations.Parasut.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="companyId">Firma ID</param>
        /// <param name="id">Kategori ID</param>
        /// <param name="include">Response ile birlikte geri dönmesini istediğiniz ilişkiler - *Available: parent_category, subcategories* (optional)</param>
        /// <returns>Task of InlineResponse2017</returns>
        System.Threading.Tasks.Task<InlineResponse2017> ShowItemCategoryAsync (int? companyId, int? id, string include = null);

        /// <summary>
        /// Kategori bilgisini gösterir.
        /// </summary>
        /// <remarks>
        /// Ayrıntısını görmek istediğiniz kategorinin id&#39;sini göndererek gerekli bilgilere ulaşabilirsiniz.
        /// </remarks>
        /// <exception cref="PeakUp.Integrations.Parasut.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="companyId">Firma ID</param>
        /// <param name="id">Kategori ID</param>
        /// <param name="include">Response ile birlikte geri dönmesini istediğiniz ilişkiler - *Available: parent_category, subcategories* (optional)</param>
        /// <returns>Task of ApiResponse (InlineResponse2017)</returns>
        System.Threading.Tasks.Task<ApiResponse<InlineResponse2017>> ShowItemCategoryAsyncWithHttpInfo (int? companyId, int? id, string include = null);
        /// <summary>
        /// Edit
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="PeakUp.Integrations.Parasut.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="itemCategoryForm"></param>
        /// <param name="companyId">Firma ID</param>
        /// <param name="id">Kategori ID</param>
        /// <param name="include">Response ile birlikte geri dönmesini istediğiniz ilişkiler - *Available: parent_category, subcategories* (optional)</param>
        /// <returns>Task of InlineResponse2017</returns>
        System.Threading.Tasks.Task<InlineResponse2017> UpdateItemCategoryAsync (ItemCategoryForm1 itemCategoryForm, int? companyId, int? id, string include = null);

        /// <summary>
        /// Edit
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="PeakUp.Integrations.Parasut.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="itemCategoryForm"></param>
        /// <param name="companyId">Firma ID</param>
        /// <param name="id">Kategori ID</param>
        /// <param name="include">Response ile birlikte geri dönmesini istediğiniz ilişkiler - *Available: parent_category, subcategories* (optional)</param>
        /// <returns>Task of ApiResponse (InlineResponse2017)</returns>
        System.Threading.Tasks.Task<ApiResponse<InlineResponse2017>> UpdateItemCategoryAsyncWithHttpInfo (ItemCategoryForm1 itemCategoryForm, int? companyId, int? id, string include = null);
        #endregion Asynchronous Operations
    }

    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
    public partial class ItemCategoriesApi : IItemCategoriesApi
    {
        private PeakUp.Integrations.Parasut.Client.ExceptionFactory _exceptionFactory = (name, response) => null;

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemCategoriesApi"/> class.
        /// </summary>
        /// <returns></returns>
        public ItemCategoriesApi(String basePath)
        {
            this.Configuration = new Configuration(new ApiClient(basePath));

            ExceptionFactory = PeakUp.Integrations.Parasut.Client.Configuration.DefaultExceptionFactory;

            // ensure API client has configuration ready
            if (Configuration.ApiClient.Configuration == null)
            {
                this.Configuration.ApiClient.Configuration = this.Configuration;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemCategoriesApi"/> class
        /// using Configuration object
        /// </summary>
        /// <param name="configuration">An instance of Configuration</param>
        /// <returns></returns>
        public ItemCategoriesApi(Configuration configuration = null)
        {
            if (configuration == null) // use the default one in Configuration
                this.Configuration = Configuration.Default;
            else
                this.Configuration = configuration;

            ExceptionFactory = PeakUp.Integrations.Parasut.Client.Configuration.DefaultExceptionFactory;

            // ensure API client has configuration ready
            if (Configuration.ApiClient.Configuration == null)
            {
                this.Configuration.ApiClient.Configuration = this.Configuration;
            }
        }

        /// <summary>
        /// Gets the base path of the API client.
        /// </summary>
        /// <value>The base path</value>
        public String GetBasePath()
        {
            return this.Configuration.ApiClient.RestClient.BaseUrl.ToString();
        }

        /// <summary>
        /// Sets the base path of the API client.
        /// </summary>
        /// <value>The base path</value>
        [Obsolete("SetBasePath is deprecated, please do 'Configuration.ApiClient = new ApiClient(\"http://new-path\")' instead.")]
        public void SetBasePath(String basePath)
        {
            // do nothing
        }

        /// <summary>
        /// Gets or sets the configuration object
        /// </summary>
        /// <value>An instance of the Configuration</value>
        public Configuration Configuration {get; set;}

        /// <summary>
        /// Provides a factory method hook for the creation of exceptions.
        /// </summary>
        public PeakUp.Integrations.Parasut.Client.ExceptionFactory ExceptionFactory
        {
            get
            {
                if (_exceptionFactory != null && _exceptionFactory.GetInvocationList().Length > 1)
                {
                    throw new InvalidOperationException("Multicast delegate for ExceptionFactory is unsupported.");
                }
                return _exceptionFactory;
            }
            set { _exceptionFactory = value; }
        }

        /// <summary>
        /// Gets the default header.
        /// </summary>
        /// <returns>Dictionary of HTTP header</returns>
        [Obsolete("DefaultHeader is deprecated, please use Configuration.DefaultHeader instead.")]
        public Dictionary<String, String> DefaultHeader()
        {
            return this.Configuration.DefaultHeader;
        }

        /// <summary>
        /// Add default header.
        /// </summary>
        /// <param name="key">Header field name.</param>
        /// <param name="value">Header field value.</param>
        /// <returns></returns>
        [Obsolete("AddDefaultHeader is deprecated, please use Configuration.AddDefaultHeader instead.")]
        public void AddDefaultHeader(string key, string value)
        {
            this.Configuration.AddDefaultHeader(key, value);
        }

        /// <summary>
        /// Create 
        /// </summary>
        /// <exception cref="PeakUp.Integrations.Parasut.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="itemCategoryForm"></param>
        /// <param name="companyId">Firma ID</param>
        /// <param name="include">Response ile birlikte geri dönmesini istediğiniz ilişkiler - *Available: parent_category, subcategories* (optional)</param>
        /// <returns>InlineResponse2017</returns>
        public InlineResponse2017 CreateItemCategory (ItemCategoryForm itemCategoryForm, int? companyId, string include = null)
        {
             ApiResponse<InlineResponse2017> localVarResponse = CreateItemCategoryWithHttpInfo(itemCategoryForm, companyId, include);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Create 
        /// </summary>
        /// <exception cref="PeakUp.Integrations.Parasut.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="itemCategoryForm"></param>
        /// <param name="companyId">Firma ID</param>
        /// <param name="include">Response ile birlikte geri dönmesini istediğiniz ilişkiler - *Available: parent_category, subcategories* (optional)</param>
        /// <returns>ApiResponse of InlineResponse2017</returns>
        public ApiResponse< InlineResponse2017 > CreateItemCategoryWithHttpInfo (ItemCategoryForm itemCategoryForm, int? companyId, string include = null)
        {
            // verify the required parameter 'itemCategoryForm' is set
            if (itemCategoryForm == null)
                throw new ApiException(400, "Missing required parameter 'itemCategoryForm' when calling ItemCategoriesApi->CreateItemCategory");
            // verify the required parameter 'companyId' is set
            if (companyId == null)
                throw new ApiException(400, "Missing required parameter 'companyId' when calling ItemCategoriesApi->CreateItemCategory");

            var localVarPath = "/{company_id}/item_categories";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new Dictionary<String, String>();
            var localVarHeaderParams = new Dictionary<String, String>(Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/vnd.api+json"
            };
            String localVarHttpContentType = Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/vnd.api+json"
            };
            String localVarHttpHeaderAccept = Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            // set "format" to json by default
            // e.g. /pet/{petId}.{format} becomes /pet/{petId}.json
            localVarPathParams.Add("format", "json");
            if (companyId != null) localVarPathParams.Add("company_id", Configuration.ApiClient.ParameterToString(companyId)); // path parameter
            if (include != null) localVarQueryParams.Add("include", Configuration.ApiClient.ParameterToString(include)); // query parameter
            if (itemCategoryForm != null && itemCategoryForm.GetType() != typeof(byte[]))
            {
                localVarPostBody = Configuration.ApiClient.Serialize(itemCategoryForm); // http body (model) parameter
            }
            else
            {
                localVarPostBody = itemCategoryForm; // byte array
            }

            // authentication (parasut_auth) required
            // oauth required
            if (!String.IsNullOrEmpty(Configuration.AccessToken))
            {
                localVarHeaderParams["Authorization"] = "Bearer " + Configuration.AccessToken;
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) Configuration.ApiClient.CallApi(localVarPath,
                Method.POST, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("CreateItemCategory", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<InlineResponse2017>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Name, x => x.Value.ToString()),
                (InlineResponse2017) Configuration.ApiClient.Deserialize(localVarResponse, typeof(InlineResponse2017)));
            
        }

        /// <summary>
        /// Create 
        /// </summary>
        /// <exception cref="PeakUp.Integrations.Parasut.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="itemCategoryForm"></param>
        /// <param name="companyId">Firma ID</param>
        /// <param name="include">Response ile birlikte geri dönmesini istediğiniz ilişkiler - *Available: parent_category, subcategories* (optional)</param>
        /// <returns>Task of InlineResponse2017</returns>
        public async System.Threading.Tasks.Task<InlineResponse2017> CreateItemCategoryAsync (ItemCategoryForm itemCategoryForm, int? companyId, string include = null)
        {
             ApiResponse<InlineResponse2017> localVarResponse = await CreateItemCategoryAsyncWithHttpInfo(itemCategoryForm, companyId, include);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Create 
        /// </summary>
        /// <exception cref="PeakUp.Integrations.Parasut.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="itemCategoryForm"></param>
        /// <param name="companyId">Firma ID</param>
        /// <param name="include">Response ile birlikte geri dönmesini istediğiniz ilişkiler - *Available: parent_category, subcategories* (optional)</param>
        /// <returns>Task of ApiResponse (InlineResponse2017)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<InlineResponse2017>> CreateItemCategoryAsyncWithHttpInfo (ItemCategoryForm itemCategoryForm, int? companyId, string include = null)
        {
            // verify the required parameter 'itemCategoryForm' is set
            if (itemCategoryForm == null)
                throw new ApiException(400, "Missing required parameter 'itemCategoryForm' when calling ItemCategoriesApi->CreateItemCategory");
            // verify the required parameter 'companyId' is set
            if (companyId == null)
                throw new ApiException(400, "Missing required parameter 'companyId' when calling ItemCategoriesApi->CreateItemCategory");

            var localVarPath = "/{company_id}/item_categories";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new Dictionary<String, String>();
            var localVarHeaderParams = new Dictionary<String, String>(Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/vnd.api+json"
            };
            String localVarHttpContentType = Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/vnd.api+json"
            };
            String localVarHttpHeaderAccept = Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            // set "format" to json by default
            // e.g. /pet/{petId}.{format} becomes /pet/{petId}.json
            localVarPathParams.Add("format", "json");
            if (companyId != null) localVarPathParams.Add("company_id", Configuration.ApiClient.ParameterToString(companyId)); // path parameter
            if (include != null) localVarQueryParams.Add("include", Configuration.ApiClient.ParameterToString(include)); // query parameter
            if (itemCategoryForm != null && itemCategoryForm.GetType() != typeof(byte[]))
            {
                localVarPostBody = Configuration.ApiClient.Serialize(itemCategoryForm); // http body (model) parameter
            }
            else
            {
                localVarPostBody = itemCategoryForm; // byte array
            }

            // authentication (parasut_auth) required
            // oauth required
            if (!String.IsNullOrEmpty(Configuration.AccessToken))
            {
                localVarHeaderParams["Authorization"] = "Bearer " + Configuration.AccessToken;
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.POST, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("CreateItemCategory", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<InlineResponse2017>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Name, x => x.Value.ToString()),
                (InlineResponse2017) Configuration.ApiClient.Deserialize(localVarResponse, typeof(InlineResponse2017)));
            
        }

        /// <summary>
        /// Delete 
        /// </summary>
        /// <exception cref="PeakUp.Integrations.Parasut.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="companyId">Firma ID</param>
        /// <param name="id">Kategori ID</param>
        /// <returns>Object</returns>
        public Object DeleteItemCategory (int? companyId, int? id)
        {
             ApiResponse<Object> localVarResponse = DeleteItemCategoryWithHttpInfo(companyId, id);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Delete 
        /// </summary>
        /// <exception cref="PeakUp.Integrations.Parasut.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="companyId">Firma ID</param>
        /// <param name="id">Kategori ID</param>
        /// <returns>ApiResponse of Object</returns>
        public ApiResponse< Object > DeleteItemCategoryWithHttpInfo (int? companyId, int? id)
        {
            // verify the required parameter 'companyId' is set
            if (companyId == null)
                throw new ApiException(400, "Missing required parameter 'companyId' when calling ItemCategoriesApi->DeleteItemCategory");
            // verify the required parameter 'id' is set
            if (id == null)
                throw new ApiException(400, "Missing required parameter 'id' when calling ItemCategoriesApi->DeleteItemCategory");

            var localVarPath = "/{company_id}/item_categories/{id}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new Dictionary<String, String>();
            var localVarHeaderParams = new Dictionary<String, String>(Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/vnd.api+json"
            };
            String localVarHttpContentType = Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/vnd.api+json"
            };
            String localVarHttpHeaderAccept = Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            // set "format" to json by default
            // e.g. /pet/{petId}.{format} becomes /pet/{petId}.json
            localVarPathParams.Add("format", "json");
            if (companyId != null) localVarPathParams.Add("company_id", Configuration.ApiClient.ParameterToString(companyId)); // path parameter
            if (id != null) localVarPathParams.Add("id", Configuration.ApiClient.ParameterToString(id)); // path parameter

            // authentication (parasut_auth) required
            // oauth required
            if (!String.IsNullOrEmpty(Configuration.AccessToken))
            {
                localVarHeaderParams["Authorization"] = "Bearer " + Configuration.AccessToken;
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) Configuration.ApiClient.CallApi(localVarPath,
                Method.DELETE, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("DeleteItemCategory", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<Object>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Name, x => x.Value.ToString()),
                (Object) Configuration.ApiClient.Deserialize(localVarResponse, typeof(Object)));
            
        }

        /// <summary>
        /// Delete 
        /// </summary>
        /// <exception cref="PeakUp.Integrations.Parasut.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="companyId">Firma ID</param>
        /// <param name="id">Kategori ID</param>
        /// <returns>Task of Object</returns>
        public async System.Threading.Tasks.Task<Object> DeleteItemCategoryAsync (int? companyId, int? id)
        {
             ApiResponse<Object> localVarResponse = await DeleteItemCategoryAsyncWithHttpInfo(companyId, id);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Delete 
        /// </summary>
        /// <exception cref="PeakUp.Integrations.Parasut.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="companyId">Firma ID</param>
        /// <param name="id">Kategori ID</param>
        /// <returns>Task of ApiResponse (Object)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<Object>> DeleteItemCategoryAsyncWithHttpInfo (int? companyId, int? id)
        {
            // verify the required parameter 'companyId' is set
            if (companyId == null)
                throw new ApiException(400, "Missing required parameter 'companyId' when calling ItemCategoriesApi->DeleteItemCategory");
            // verify the required parameter 'id' is set
            if (id == null)
                throw new ApiException(400, "Missing required parameter 'id' when calling ItemCategoriesApi->DeleteItemCategory");

            var localVarPath = "/{company_id}/item_categories/{id}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new Dictionary<String, String>();
            var localVarHeaderParams = new Dictionary<String, String>(Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/vnd.api+json"
            };
            String localVarHttpContentType = Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/vnd.api+json"
            };
            String localVarHttpHeaderAccept = Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            // set "format" to json by default
            // e.g. /pet/{petId}.{format} becomes /pet/{petId}.json
            localVarPathParams.Add("format", "json");
            if (companyId != null) localVarPathParams.Add("company_id", Configuration.ApiClient.ParameterToString(companyId)); // path parameter
            if (id != null) localVarPathParams.Add("id", Configuration.ApiClient.ParameterToString(id)); // path parameter

            // authentication (parasut_auth) required
            // oauth required
            if (!String.IsNullOrEmpty(Configuration.AccessToken))
            {
                localVarHeaderParams["Authorization"] = "Bearer " + Configuration.AccessToken;
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.DELETE, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("DeleteItemCategory", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<Object>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Name, x => x.Value.ToString()),
                (Object) Configuration.ApiClient.Deserialize(localVarResponse, typeof(Object)));
            
        }

        /// <summary>
        /// Index 
        /// </summary>
        /// <exception cref="PeakUp.Integrations.Parasut.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="companyId">Firma ID</param>
        /// <param name="filterName"> (optional)</param>
        /// <param name="filterCategoryType"> (optional)</param>
        /// <param name="sort">Sortable parameters - *Available: id, name, category_type* (optional, default to id)</param>
        /// <param name="pageNumber">Page Number (optional, default to 1)</param>
        /// <param name="pageSize">Page Size (optional, default to 15)</param>
        /// <param name="include">Response ile birlikte geri dönmesini istediğiniz ilişkiler - *Available: parent_category, subcategories* (optional)</param>
        /// <returns>InlineResponse2008</returns>
        public InlineResponse2008 ListItemCategories (int? companyId, string filterName = null, string filterCategoryType = null, string sort = null, int? pageNumber = null, int? pageSize = null, string include = null)
        {
             ApiResponse<InlineResponse2008> localVarResponse = ListItemCategoriesWithHttpInfo(companyId, filterName, filterCategoryType, sort, pageNumber, pageSize, include);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Index 
        /// </summary>
        /// <exception cref="PeakUp.Integrations.Parasut.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="companyId">Firma ID</param>
        /// <param name="filterName"> (optional)</param>
        /// <param name="filterCategoryType"> (optional)</param>
        /// <param name="sort">Sortable parameters - *Available: id, name, category_type* (optional, default to id)</param>
        /// <param name="pageNumber">Page Number (optional, default to 1)</param>
        /// <param name="pageSize">Page Size (optional, default to 15)</param>
        /// <param name="include">Response ile birlikte geri dönmesini istediğiniz ilişkiler - *Available: parent_category, subcategories* (optional)</param>
        /// <returns>ApiResponse of InlineResponse2008</returns>
        public ApiResponse< InlineResponse2008 > ListItemCategoriesWithHttpInfo (int? companyId, string filterName = null, string filterCategoryType = null, string sort = null, int? pageNumber = null, int? pageSize = null, string include = null)
        {
            // verify the required parameter 'companyId' is set
            if (companyId == null)
                throw new ApiException(400, "Missing required parameter 'companyId' when calling ItemCategoriesApi->ListItemCategories");

            var localVarPath = "/{company_id}/item_categories";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new Dictionary<String, String>();
            var localVarHeaderParams = new Dictionary<String, String>(Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/vnd.api+json"
            };
            String localVarHttpContentType = Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/vnd.api+json"
            };
            String localVarHttpHeaderAccept = Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            // set "format" to json by default
            // e.g. /pet/{petId}.{format} becomes /pet/{petId}.json
            localVarPathParams.Add("format", "json");
            if (companyId != null) localVarPathParams.Add("company_id", Configuration.ApiClient.ParameterToString(companyId)); // path parameter
            if (filterName != null) localVarQueryParams.Add("filter[name]", Configuration.ApiClient.ParameterToString(filterName)); // query parameter
            if (filterCategoryType != null) localVarQueryParams.Add("filter[category_type]", Configuration.ApiClient.ParameterToString(filterCategoryType)); // query parameter
            if (sort != null) localVarQueryParams.Add("sort", Configuration.ApiClient.ParameterToString(sort)); // query parameter
            if (pageNumber != null) localVarQueryParams.Add("page[number]", Configuration.ApiClient.ParameterToString(pageNumber)); // query parameter
            if (pageSize != null) localVarQueryParams.Add("page[size]", Configuration.ApiClient.ParameterToString(pageSize)); // query parameter
            if (include != null) localVarQueryParams.Add("include", Configuration.ApiClient.ParameterToString(include)); // query parameter

            // authentication (parasut_auth) required
            // oauth required
            if (!String.IsNullOrEmpty(Configuration.AccessToken))
            {
                localVarHeaderParams["Authorization"] = "Bearer " + Configuration.AccessToken;
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) Configuration.ApiClient.CallApi(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("ListItemCategories", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<InlineResponse2008>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Name, x => x.Value.ToString()),
                (InlineResponse2008) Configuration.ApiClient.Deserialize(localVarResponse, typeof(InlineResponse2008)));
            
        }

        /// <summary>
        /// Index 
        /// </summary>
        /// <exception cref="PeakUp.Integrations.Parasut.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="companyId">Firma ID</param>
        /// <param name="filterName"> (optional)</param>
        /// <param name="filterCategoryType"> (optional)</param>
        /// <param name="sort">Sortable parameters - *Available: id, name, category_type* (optional, default to id)</param>
        /// <param name="pageNumber">Page Number (optional, default to 1)</param>
        /// <param name="pageSize">Page Size (optional, default to 15)</param>
        /// <param name="include">Response ile birlikte geri dönmesini istediğiniz ilişkiler - *Available: parent_category, subcategories* (optional)</param>
        /// <returns>Task of InlineResponse2008</returns>
        public async System.Threading.Tasks.Task<InlineResponse2008> ListItemCategoriesAsync (int? companyId, string filterName = null, string filterCategoryType = null, string sort = null, int? pageNumber = null, int? pageSize = null, string include = null)
        {
             ApiResponse<InlineResponse2008> localVarResponse = await ListItemCategoriesAsyncWithHttpInfo(companyId, filterName, filterCategoryType, sort, pageNumber, pageSize, include);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Index 
        /// </summary>
        /// <exception cref="PeakUp.Integrations.Parasut.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="companyId">Firma ID</param>
        /// <param name="filterName"> (optional)</param>
        /// <param name="filterCategoryType"> (optional)</param>
        /// <param name="sort">Sortable parameters - *Available: id, name, category_type* (optional, default to id)</param>
        /// <param name="pageNumber">Page Number (optional, default to 1)</param>
        /// <param name="pageSize">Page Size (optional, default to 15)</param>
        /// <param name="include">Response ile birlikte geri dönmesini istediğiniz ilişkiler - *Available: parent_category, subcategories* (optional)</param>
        /// <returns>Task of ApiResponse (InlineResponse2008)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<InlineResponse2008>> ListItemCategoriesAsyncWithHttpInfo (int? companyId, string filterName = null, string filterCategoryType = null, string sort = null, int? pageNumber = null, int? pageSize = null, string include = null)
        {
            // verify the required parameter 'companyId' is set
            if (companyId == null)
                throw new ApiException(400, "Missing required parameter 'companyId' when calling ItemCategoriesApi->ListItemCategories");

            var localVarPath = "/{company_id}/item_categories";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new Dictionary<String, String>();
            var localVarHeaderParams = new Dictionary<String, String>(Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/vnd.api+json"
            };
            String localVarHttpContentType = Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/vnd.api+json"
            };
            String localVarHttpHeaderAccept = Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            // set "format" to json by default
            // e.g. /pet/{petId}.{format} becomes /pet/{petId}.json
            localVarPathParams.Add("format", "json");
            if (companyId != null) localVarPathParams.Add("company_id", Configuration.ApiClient.ParameterToString(companyId)); // path parameter
            if (filterName != null) localVarQueryParams.Add("filter[name]", Configuration.ApiClient.ParameterToString(filterName)); // query parameter
            if (filterCategoryType != null) localVarQueryParams.Add("filter[category_type]", Configuration.ApiClient.ParameterToString(filterCategoryType)); // query parameter
            if (sort != null) localVarQueryParams.Add("sort", Configuration.ApiClient.ParameterToString(sort)); // query parameter
            if (pageNumber != null) localVarQueryParams.Add("page[number]", Configuration.ApiClient.ParameterToString(pageNumber)); // query parameter
            if (pageSize != null) localVarQueryParams.Add("page[size]", Configuration.ApiClient.ParameterToString(pageSize)); // query parameter
            if (include != null) localVarQueryParams.Add("include", Configuration.ApiClient.ParameterToString(include)); // query parameter

            // authentication (parasut_auth) required
            // oauth required
            if (!String.IsNullOrEmpty(Configuration.AccessToken))
            {
                localVarHeaderParams["Authorization"] = "Bearer " + Configuration.AccessToken;
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("ListItemCategories", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<InlineResponse2008>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Name, x => x.Value.ToString()),
                (InlineResponse2008) Configuration.ApiClient.Deserialize(localVarResponse, typeof(InlineResponse2008)));
            
        }

        /// <summary>
        /// Kategori bilgisini gösterir. Ayrıntısını görmek istediğiniz kategorinin id&#39;sini göndererek gerekli bilgilere ulaşabilirsiniz.
        /// </summary>
        /// <exception cref="PeakUp.Integrations.Parasut.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="companyId">Firma ID</param>
        /// <param name="id">Kategori ID</param>
        /// <param name="include">Response ile birlikte geri dönmesini istediğiniz ilişkiler - *Available: parent_category, subcategories* (optional)</param>
        /// <returns>InlineResponse2017</returns>
        public InlineResponse2017 ShowItemCategory (int? companyId, int? id, string include = null)
        {
             ApiResponse<InlineResponse2017> localVarResponse = ShowItemCategoryWithHttpInfo(companyId, id, include);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Kategori bilgisini gösterir. Ayrıntısını görmek istediğiniz kategorinin id&#39;sini göndererek gerekli bilgilere ulaşabilirsiniz.
        /// </summary>
        /// <exception cref="PeakUp.Integrations.Parasut.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="companyId">Firma ID</param>
        /// <param name="id">Kategori ID</param>
        /// <param name="include">Response ile birlikte geri dönmesini istediğiniz ilişkiler - *Available: parent_category, subcategories* (optional)</param>
        /// <returns>ApiResponse of InlineResponse2017</returns>
        public ApiResponse< InlineResponse2017 > ShowItemCategoryWithHttpInfo (int? companyId, int? id, string include = null)
        {
            // verify the required parameter 'companyId' is set
            if (companyId == null)
                throw new ApiException(400, "Missing required parameter 'companyId' when calling ItemCategoriesApi->ShowItemCategory");
            // verify the required parameter 'id' is set
            if (id == null)
                throw new ApiException(400, "Missing required parameter 'id' when calling ItemCategoriesApi->ShowItemCategory");

            var localVarPath = "/{company_id}/item_categories/{id}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new Dictionary<String, String>();
            var localVarHeaderParams = new Dictionary<String, String>(Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/vnd.api+json"
            };
            String localVarHttpContentType = Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/vnd.api+json"
            };
            String localVarHttpHeaderAccept = Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            // set "format" to json by default
            // e.g. /pet/{petId}.{format} becomes /pet/{petId}.json
            localVarPathParams.Add("format", "json");
            if (companyId != null) localVarPathParams.Add("company_id", Configuration.ApiClient.ParameterToString(companyId)); // path parameter
            if (id != null) localVarPathParams.Add("id", Configuration.ApiClient.ParameterToString(id)); // path parameter
            if (include != null) localVarQueryParams.Add("include", Configuration.ApiClient.ParameterToString(include)); // query parameter

            // authentication (parasut_auth) required
            // oauth required
            if (!String.IsNullOrEmpty(Configuration.AccessToken))
            {
                localVarHeaderParams["Authorization"] = "Bearer " + Configuration.AccessToken;
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) Configuration.ApiClient.CallApi(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("ShowItemCategory", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<InlineResponse2017>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Name, x => x.Value.ToString()),
                (InlineResponse2017) Configuration.ApiClient.Deserialize(localVarResponse, typeof(InlineResponse2017)));
            
        }

        /// <summary>
        /// Kategori bilgisini gösterir. Ayrıntısını görmek istediğiniz kategorinin id&#39;sini göndererek gerekli bilgilere ulaşabilirsiniz.
        /// </summary>
        /// <exception cref="PeakUp.Integrations.Parasut.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="companyId">Firma ID</param>
        /// <param name="id">Kategori ID</param>
        /// <param name="include">Response ile birlikte geri dönmesini istediğiniz ilişkiler - *Available: parent_category, subcategories* (optional)</param>
        /// <returns>Task of InlineResponse2017</returns>
        public async System.Threading.Tasks.Task<InlineResponse2017> ShowItemCategoryAsync (int? companyId, int? id, string include = null)
        {
             ApiResponse<InlineResponse2017> localVarResponse = await ShowItemCategoryAsyncWithHttpInfo(companyId, id, include);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Kategori bilgisini gösterir. Ayrıntısını görmek istediğiniz kategorinin id&#39;sini göndererek gerekli bilgilere ulaşabilirsiniz.
        /// </summary>
        /// <exception cref="PeakUp.Integrations.Parasut.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="companyId">Firma ID</param>
        /// <param name="id">Kategori ID</param>
        /// <param name="include">Response ile birlikte geri dönmesini istediğiniz ilişkiler - *Available: parent_category, subcategories* (optional)</param>
        /// <returns>Task of ApiResponse (InlineResponse2017)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<InlineResponse2017>> ShowItemCategoryAsyncWithHttpInfo (int? companyId, int? id, string include = null)
        {
            // verify the required parameter 'companyId' is set
            if (companyId == null)
                throw new ApiException(400, "Missing required parameter 'companyId' when calling ItemCategoriesApi->ShowItemCategory");
            // verify the required parameter 'id' is set
            if (id == null)
                throw new ApiException(400, "Missing required parameter 'id' when calling ItemCategoriesApi->ShowItemCategory");

            var localVarPath = "/{company_id}/item_categories/{id}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new Dictionary<String, String>();
            var localVarHeaderParams = new Dictionary<String, String>(Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/vnd.api+json"
            };
            String localVarHttpContentType = Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/vnd.api+json"
            };
            String localVarHttpHeaderAccept = Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            // set "format" to json by default
            // e.g. /pet/{petId}.{format} becomes /pet/{petId}.json
            localVarPathParams.Add("format", "json");
            if (companyId != null) localVarPathParams.Add("company_id", Configuration.ApiClient.ParameterToString(companyId)); // path parameter
            if (id != null) localVarPathParams.Add("id", Configuration.ApiClient.ParameterToString(id)); // path parameter
            if (include != null) localVarQueryParams.Add("include", Configuration.ApiClient.ParameterToString(include)); // query parameter

            // authentication (parasut_auth) required
            // oauth required
            if (!String.IsNullOrEmpty(Configuration.AccessToken))
            {
                localVarHeaderParams["Authorization"] = "Bearer " + Configuration.AccessToken;
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("ShowItemCategory", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<InlineResponse2017>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Name, x => x.Value.ToString()),
                (InlineResponse2017) Configuration.ApiClient.Deserialize(localVarResponse, typeof(InlineResponse2017)));
            
        }

        /// <summary>
        /// Edit 
        /// </summary>
        /// <exception cref="PeakUp.Integrations.Parasut.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="itemCategoryForm"></param>
        /// <param name="companyId">Firma ID</param>
        /// <param name="id">Kategori ID</param>
        /// <param name="include">Response ile birlikte geri dönmesini istediğiniz ilişkiler - *Available: parent_category, subcategories* (optional)</param>
        /// <returns>InlineResponse2017</returns>
        public InlineResponse2017 UpdateItemCategory (ItemCategoryForm1 itemCategoryForm, int? companyId, int? id, string include = null)
        {
             ApiResponse<InlineResponse2017> localVarResponse = UpdateItemCategoryWithHttpInfo(itemCategoryForm, companyId, id, include);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Edit 
        /// </summary>
        /// <exception cref="PeakUp.Integrations.Parasut.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="itemCategoryForm"></param>
        /// <param name="companyId">Firma ID</param>
        /// <param name="id">Kategori ID</param>
        /// <param name="include">Response ile birlikte geri dönmesini istediğiniz ilişkiler - *Available: parent_category, subcategories* (optional)</param>
        /// <returns>ApiResponse of InlineResponse2017</returns>
        public ApiResponse< InlineResponse2017 > UpdateItemCategoryWithHttpInfo (ItemCategoryForm1 itemCategoryForm, int? companyId, int? id, string include = null)
        {
            // verify the required parameter 'itemCategoryForm' is set
            if (itemCategoryForm == null)
                throw new ApiException(400, "Missing required parameter 'itemCategoryForm' when calling ItemCategoriesApi->UpdateItemCategory");
            // verify the required parameter 'companyId' is set
            if (companyId == null)
                throw new ApiException(400, "Missing required parameter 'companyId' when calling ItemCategoriesApi->UpdateItemCategory");
            // verify the required parameter 'id' is set
            if (id == null)
                throw new ApiException(400, "Missing required parameter 'id' when calling ItemCategoriesApi->UpdateItemCategory");

            var localVarPath = "/{company_id}/item_categories/{id}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new Dictionary<String, String>();
            var localVarHeaderParams = new Dictionary<String, String>(Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/vnd.api+json"
            };
            String localVarHttpContentType = Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/vnd.api+json"
            };
            String localVarHttpHeaderAccept = Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            // set "format" to json by default
            // e.g. /pet/{petId}.{format} becomes /pet/{petId}.json
            localVarPathParams.Add("format", "json");
            if (companyId != null) localVarPathParams.Add("company_id", Configuration.ApiClient.ParameterToString(companyId)); // path parameter
            if (id != null) localVarPathParams.Add("id", Configuration.ApiClient.ParameterToString(id)); // path parameter
            if (include != null) localVarQueryParams.Add("include", Configuration.ApiClient.ParameterToString(include)); // query parameter
            if (itemCategoryForm != null && itemCategoryForm.GetType() != typeof(byte[]))
            {
                localVarPostBody = Configuration.ApiClient.Serialize(itemCategoryForm); // http body (model) parameter
            }
            else
            {
                localVarPostBody = itemCategoryForm; // byte array
            }

            // authentication (parasut_auth) required
            // oauth required
            if (!String.IsNullOrEmpty(Configuration.AccessToken))
            {
                localVarHeaderParams["Authorization"] = "Bearer " + Configuration.AccessToken;
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) Configuration.ApiClient.CallApi(localVarPath,
                Method.PUT, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("UpdateItemCategory", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<InlineResponse2017>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Name, x => x.Value.ToString()),
                (InlineResponse2017) Configuration.ApiClient.Deserialize(localVarResponse, typeof(InlineResponse2017)));
            
        }

        /// <summary>
        /// Edit 
        /// </summary>
        /// <exception cref="PeakUp.Integrations.Parasut.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="itemCategoryForm"></param>
        /// <param name="companyId">Firma ID</param>
        /// <param name="id">Kategori ID</param>
        /// <param name="include">Response ile birlikte geri dönmesini istediğiniz ilişkiler - *Available: parent_category, subcategories* (optional)</param>
        /// <returns>Task of InlineResponse2017</returns>
        public async System.Threading.Tasks.Task<InlineResponse2017> UpdateItemCategoryAsync (ItemCategoryForm1 itemCategoryForm, int? companyId, int? id, string include = null)
        {
             ApiResponse<InlineResponse2017> localVarResponse = await UpdateItemCategoryAsyncWithHttpInfo(itemCategoryForm, companyId, id, include);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Edit 
        /// </summary>
        /// <exception cref="PeakUp.Integrations.Parasut.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="itemCategoryForm"></param>
        /// <param name="companyId">Firma ID</param>
        /// <param name="id">Kategori ID</param>
        /// <param name="include">Response ile birlikte geri dönmesini istediğiniz ilişkiler - *Available: parent_category, subcategories* (optional)</param>
        /// <returns>Task of ApiResponse (InlineResponse2017)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<InlineResponse2017>> UpdateItemCategoryAsyncWithHttpInfo (ItemCategoryForm1 itemCategoryForm, int? companyId, int? id, string include = null)
        {
            // verify the required parameter 'itemCategoryForm' is set
            if (itemCategoryForm == null)
                throw new ApiException(400, "Missing required parameter 'itemCategoryForm' when calling ItemCategoriesApi->UpdateItemCategory");
            // verify the required parameter 'companyId' is set
            if (companyId == null)
                throw new ApiException(400, "Missing required parameter 'companyId' when calling ItemCategoriesApi->UpdateItemCategory");
            // verify the required parameter 'id' is set
            if (id == null)
                throw new ApiException(400, "Missing required parameter 'id' when calling ItemCategoriesApi->UpdateItemCategory");

            var localVarPath = "/{company_id}/item_categories/{id}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new Dictionary<String, String>();
            var localVarHeaderParams = new Dictionary<String, String>(Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/vnd.api+json"
            };
            String localVarHttpContentType = Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/vnd.api+json"
            };
            String localVarHttpHeaderAccept = Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            // set "format" to json by default
            // e.g. /pet/{petId}.{format} becomes /pet/{petId}.json
            localVarPathParams.Add("format", "json");
            if (companyId != null) localVarPathParams.Add("company_id", Configuration.ApiClient.ParameterToString(companyId)); // path parameter
            if (id != null) localVarPathParams.Add("id", Configuration.ApiClient.ParameterToString(id)); // path parameter
            if (include != null) localVarQueryParams.Add("include", Configuration.ApiClient.ParameterToString(include)); // query parameter
            if (itemCategoryForm != null && itemCategoryForm.GetType() != typeof(byte[]))
            {
                localVarPostBody = Configuration.ApiClient.Serialize(itemCategoryForm); // http body (model) parameter
            }
            else
            {
                localVarPostBody = itemCategoryForm; // byte array
            }

            // authentication (parasut_auth) required
            // oauth required
            if (!String.IsNullOrEmpty(Configuration.AccessToken))
            {
                localVarHeaderParams["Authorization"] = "Bearer " + Configuration.AccessToken;
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.PUT, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("UpdateItemCategory", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<InlineResponse2017>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Name, x => x.Value.ToString()),
                (InlineResponse2017) Configuration.ApiClient.Deserialize(localVarResponse, typeof(InlineResponse2017)));
            
        }

    }
}
