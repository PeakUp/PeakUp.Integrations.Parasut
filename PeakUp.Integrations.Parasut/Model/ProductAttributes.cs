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
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;

namespace PeakUp.Integrations.Parasut.Model
{
    /// <summary>
    /// ProductAttributes
    /// </summary>
    [DataContract]
    public partial class ProductAttributes :  IEquatable<ProductAttributes>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductAttributes" /> class.
        /// </summary>
        [JsonConstructorAttribute]
        protected ProductAttributes() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductAttributes" /> class.
        /// </summary>
        /// <param name="Code">Ürün/hizmet kodu.</param>
        /// <param name="Name">Ürün/hizmet ismi (required).</param>
        /// <param name="VatRate">KDV oranı.</param>
        /// <param name="SalesExciseDuty">Satış ÖTV.</param>
        /// <param name="SalesExciseDutyType">Satış ÖTV tipi.</param>
        /// <param name="PurchaseExciseDuty">Alış ÖTV.</param>
        /// <param name="PurchaseExciseDutyType">Alış ÖTV tipi.</param>
        /// <param name="Unit">Birim.</param>
        /// <param name="CommunicationsTaxRate">ÖİV oranı.</param>
        /// <param name="Archived">Archived.</param>
        /// <param name="ListPrice">Satış fiyatı.</param>
        /// <param name="Currency">Satış döviz.</param>
        /// <param name="BuyingPrice">Alış fiyatı.</param>
        /// <param name="BuyingCurrency">Alış döviz.</param>
        /// <param name="InventoryTracking">InventoryTracking.</param>
        /// <param name="InitialStockCount">Başlangıç Stok Miktarı.</param>
        public ProductAttributes(string Code = default(string), string Name = default(string), decimal? VatRate = default(decimal?), decimal? SalesExciseDuty = default(decimal?), string SalesExciseDutyType = default(string), decimal? PurchaseExciseDuty = default(decimal?), string PurchaseExciseDutyType = default(string), string Unit = default(string), decimal? CommunicationsTaxRate = default(decimal?), bool? Archived = default(bool?), decimal? ListPrice = default(decimal?), string Currency = default(string), decimal? BuyingPrice = default(decimal?), string BuyingCurrency = default(string), bool? InventoryTracking = default(bool?), decimal? InitialStockCount = default(decimal?))
        {
            // to ensure "Name" is required (not null)
            if (Name == null)
            {
                throw new InvalidDataException("Name is a required property for ProductAttributes and cannot be null");
            }
            else
            {
                this.Name = Name;
            }
            this.Code = Code;
            this.VatRate = VatRate;
            this.SalesExciseDuty = SalesExciseDuty;
            this.SalesExciseDutyType = SalesExciseDutyType;
            this.PurchaseExciseDuty = PurchaseExciseDuty;
            this.PurchaseExciseDutyType = PurchaseExciseDutyType;
            this.Unit = Unit;
            this.CommunicationsTaxRate = CommunicationsTaxRate;
            this.Archived = Archived;
            this.ListPrice = ListPrice;
            this.Currency = Currency;
            this.BuyingPrice = BuyingPrice;
            this.BuyingCurrency = BuyingCurrency;
            this.InventoryTracking = InventoryTracking;
            this.InitialStockCount = InitialStockCount;
        }
        
        /// <summary>
        /// Gets or Sets SalesExciseDutyCode
        /// </summary>
        [DataMember(Name="sales_excise_duty_code", EmitDefaultValue=false)]
        public string SalesExciseDutyCode { get; private set; }
        /// <summary>
        /// Gets or Sets SalesInvoiceDetailsCount
        /// </summary>
        [DataMember(Name="sales_invoice_details_count", EmitDefaultValue=false)]
        public int? SalesInvoiceDetailsCount { get; private set; }
        /// <summary>
        /// Gets or Sets PurchaseInvoiceDetailsCount
        /// </summary>
        [DataMember(Name="purchase_invoice_details_count", EmitDefaultValue=false)]
        public int? PurchaseInvoiceDetailsCount { get; private set; }
        /// <summary>
        /// Gets or Sets ListPriceInTrl
        /// </summary>
        [DataMember(Name="list_price_in_trl", EmitDefaultValue=false)]
        public decimal? ListPriceInTrl { get; private set; }
        /// <summary>
        /// Gets or Sets BuyingPriceInTrl
        /// </summary>
        [DataMember(Name="buying_price_in_trl", EmitDefaultValue=false)]
        public decimal? BuyingPriceInTrl { get; private set; }
        /// <summary>
        /// Stok Miktarı
        /// </summary>
        /// <value>Stok Miktarı</value>
        [DataMember(Name="stock_count", EmitDefaultValue=false)]
        public decimal? StockCount { get; private set; }
        /// <summary>
        /// Ürün/hizmet kodu
        /// </summary>
        /// <value>Ürün/hizmet kodu</value>
        [DataMember(Name="code", EmitDefaultValue=false)]
        public string Code { get; set; }
        /// <summary>
        /// Ürün/hizmet ismi
        /// </summary>
        /// <value>Ürün/hizmet ismi</value>
        [DataMember(Name="name", EmitDefaultValue=false)]
        public string Name { get; set; }
        /// <summary>
        /// KDV oranı
        /// </summary>
        /// <value>KDV oranı</value>
        [DataMember(Name="vat_rate", EmitDefaultValue=false)]
        public decimal? VatRate { get; set; }
        /// <summary>
        /// Satış ÖTV
        /// </summary>
        /// <value>Satış ÖTV</value>
        [DataMember(Name="sales_excise_duty", EmitDefaultValue=false)]
        public decimal? SalesExciseDuty { get; set; }
        /// <summary>
        /// Satış ÖTV tipi
        /// </summary>
        /// <value>Satış ÖTV tipi</value>
        [DataMember(Name="sales_excise_duty_type", EmitDefaultValue=false)]
        public string SalesExciseDutyType { get; set; }
        /// <summary>
        /// Alış ÖTV
        /// </summary>
        /// <value>Alış ÖTV</value>
        [DataMember(Name="purchase_excise_duty", EmitDefaultValue=false)]
        public decimal? PurchaseExciseDuty { get; set; }
        /// <summary>
        /// Alış ÖTV tipi
        /// </summary>
        /// <value>Alış ÖTV tipi</value>
        [DataMember(Name="purchase_excise_duty_type", EmitDefaultValue=false)]
        public string PurchaseExciseDutyType { get; set; }
        /// <summary>
        /// Birim
        /// </summary>
        /// <value>Birim</value>
        [DataMember(Name="unit", EmitDefaultValue=false)]
        public string Unit { get; set; }
        /// <summary>
        /// ÖİV oranı
        /// </summary>
        /// <value>ÖİV oranı</value>
        [DataMember(Name="communications_tax_rate", EmitDefaultValue=false)]
        public decimal? CommunicationsTaxRate { get; set; }
        /// <summary>
        /// Gets or Sets Archived
        /// </summary>
        [DataMember(Name="archived", EmitDefaultValue=false)]
        public bool? Archived { get; set; }
        /// <summary>
        /// Satış fiyatı
        /// </summary>
        /// <value>Satış fiyatı</value>
        [DataMember(Name="list_price", EmitDefaultValue=false)]
        public decimal? ListPrice { get; set; }
        /// <summary>
        /// Satış döviz
        /// </summary>
        /// <value>Satış döviz</value>
        [DataMember(Name="currency", EmitDefaultValue=false)]
        public string Currency { get; set; }
        /// <summary>
        /// Alış fiyatı
        /// </summary>
        /// <value>Alış fiyatı</value>
        [DataMember(Name="buying_price", EmitDefaultValue=false)]
        public decimal? BuyingPrice { get; set; }
        /// <summary>
        /// Alış döviz
        /// </summary>
        /// <value>Alış döviz</value>
        [DataMember(Name="buying_currency", EmitDefaultValue=false)]
        public string BuyingCurrency { get; set; }
        /// <summary>
        /// Gets or Sets InventoryTracking
        /// </summary>
        [DataMember(Name="inventory_tracking", EmitDefaultValue=false)]
        public bool? InventoryTracking { get; set; }
        /// <summary>
        /// Başlangıç Stok Miktarı
        /// </summary>
        /// <value>Başlangıç Stok Miktarı</value>
        [DataMember(Name="initial_stock_count", EmitDefaultValue=false)]
        public decimal? InitialStockCount { get; set; }
        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class ProductAttributes {\n");
            sb.Append("  SalesExciseDutyCode: ").Append(SalesExciseDutyCode).Append("\n");
            sb.Append("  SalesInvoiceDetailsCount: ").Append(SalesInvoiceDetailsCount).Append("\n");
            sb.Append("  PurchaseInvoiceDetailsCount: ").Append(PurchaseInvoiceDetailsCount).Append("\n");
            sb.Append("  ListPriceInTrl: ").Append(ListPriceInTrl).Append("\n");
            sb.Append("  BuyingPriceInTrl: ").Append(BuyingPriceInTrl).Append("\n");
            sb.Append("  StockCount: ").Append(StockCount).Append("\n");
            sb.Append("  Code: ").Append(Code).Append("\n");
            sb.Append("  Name: ").Append(Name).Append("\n");
            sb.Append("  VatRate: ").Append(VatRate).Append("\n");
            sb.Append("  SalesExciseDuty: ").Append(SalesExciseDuty).Append("\n");
            sb.Append("  SalesExciseDutyType: ").Append(SalesExciseDutyType).Append("\n");
            sb.Append("  PurchaseExciseDuty: ").Append(PurchaseExciseDuty).Append("\n");
            sb.Append("  PurchaseExciseDutyType: ").Append(PurchaseExciseDutyType).Append("\n");
            sb.Append("  Unit: ").Append(Unit).Append("\n");
            sb.Append("  CommunicationsTaxRate: ").Append(CommunicationsTaxRate).Append("\n");
            sb.Append("  Archived: ").Append(Archived).Append("\n");
            sb.Append("  ListPrice: ").Append(ListPrice).Append("\n");
            sb.Append("  Currency: ").Append(Currency).Append("\n");
            sb.Append("  BuyingPrice: ").Append(BuyingPrice).Append("\n");
            sb.Append("  BuyingCurrency: ").Append(BuyingCurrency).Append("\n");
            sb.Append("  InventoryTracking: ").Append(InventoryTracking).Append("\n");
            sb.Append("  InitialStockCount: ").Append(InitialStockCount).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }
  
        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        /// <summary>
        /// Returns true if objects are equal
        /// </summary>
        /// <param name="obj">Object to be compared</param>
        /// <returns>Boolean</returns>
        public override bool Equals(object obj)
        {
            // credit: http://stackoverflow.com/a/10454552/677735
            return this.Equals(obj as ProductAttributes);
        }

        /// <summary>
        /// Returns true if ProductAttributes instances are equal
        /// </summary>
        /// <param name="other">Instance of ProductAttributes to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(ProductAttributes other)
        {
            // credit: http://stackoverflow.com/a/10454552/677735
            if (other == null)
                return false;

            return 
                (
                    this.SalesExciseDutyCode == other.SalesExciseDutyCode ||
                    this.SalesExciseDutyCode != null &&
                    this.SalesExciseDutyCode.Equals(other.SalesExciseDutyCode)
                ) && 
                (
                    this.SalesInvoiceDetailsCount == other.SalesInvoiceDetailsCount ||
                    this.SalesInvoiceDetailsCount != null &&
                    this.SalesInvoiceDetailsCount.Equals(other.SalesInvoiceDetailsCount)
                ) && 
                (
                    this.PurchaseInvoiceDetailsCount == other.PurchaseInvoiceDetailsCount ||
                    this.PurchaseInvoiceDetailsCount != null &&
                    this.PurchaseInvoiceDetailsCount.Equals(other.PurchaseInvoiceDetailsCount)
                ) && 
                (
                    this.ListPriceInTrl == other.ListPriceInTrl ||
                    this.ListPriceInTrl != null &&
                    this.ListPriceInTrl.Equals(other.ListPriceInTrl)
                ) && 
                (
                    this.BuyingPriceInTrl == other.BuyingPriceInTrl ||
                    this.BuyingPriceInTrl != null &&
                    this.BuyingPriceInTrl.Equals(other.BuyingPriceInTrl)
                ) && 
                (
                    this.StockCount == other.StockCount ||
                    this.StockCount != null &&
                    this.StockCount.Equals(other.StockCount)
                ) && 
                (
                    this.Code == other.Code ||
                    this.Code != null &&
                    this.Code.Equals(other.Code)
                ) && 
                (
                    this.Name == other.Name ||
                    this.Name != null &&
                    this.Name.Equals(other.Name)
                ) && 
                (
                    this.VatRate == other.VatRate ||
                    this.VatRate != null &&
                    this.VatRate.Equals(other.VatRate)
                ) && 
                (
                    this.SalesExciseDuty == other.SalesExciseDuty ||
                    this.SalesExciseDuty != null &&
                    this.SalesExciseDuty.Equals(other.SalesExciseDuty)
                ) && 
                (
                    this.SalesExciseDutyType == other.SalesExciseDutyType ||
                    this.SalesExciseDutyType != null &&
                    this.SalesExciseDutyType.Equals(other.SalesExciseDutyType)
                ) && 
                (
                    this.PurchaseExciseDuty == other.PurchaseExciseDuty ||
                    this.PurchaseExciseDuty != null &&
                    this.PurchaseExciseDuty.Equals(other.PurchaseExciseDuty)
                ) && 
                (
                    this.PurchaseExciseDutyType == other.PurchaseExciseDutyType ||
                    this.PurchaseExciseDutyType != null &&
                    this.PurchaseExciseDutyType.Equals(other.PurchaseExciseDutyType)
                ) && 
                (
                    this.Unit == other.Unit ||
                    this.Unit != null &&
                    this.Unit.Equals(other.Unit)
                ) && 
                (
                    this.CommunicationsTaxRate == other.CommunicationsTaxRate ||
                    this.CommunicationsTaxRate != null &&
                    this.CommunicationsTaxRate.Equals(other.CommunicationsTaxRate)
                ) && 
                (
                    this.Archived == other.Archived ||
                    this.Archived != null &&
                    this.Archived.Equals(other.Archived)
                ) && 
                (
                    this.ListPrice == other.ListPrice ||
                    this.ListPrice != null &&
                    this.ListPrice.Equals(other.ListPrice)
                ) && 
                (
                    this.Currency == other.Currency ||
                    this.Currency != null &&
                    this.Currency.Equals(other.Currency)
                ) && 
                (
                    this.BuyingPrice == other.BuyingPrice ||
                    this.BuyingPrice != null &&
                    this.BuyingPrice.Equals(other.BuyingPrice)
                ) && 
                (
                    this.BuyingCurrency == other.BuyingCurrency ||
                    this.BuyingCurrency != null &&
                    this.BuyingCurrency.Equals(other.BuyingCurrency)
                ) && 
                (
                    this.InventoryTracking == other.InventoryTracking ||
                    this.InventoryTracking != null &&
                    this.InventoryTracking.Equals(other.InventoryTracking)
                ) && 
                (
                    this.InitialStockCount == other.InitialStockCount ||
                    this.InitialStockCount != null &&
                    this.InitialStockCount.Equals(other.InitialStockCount)
                );
        }

        /// <summary>
        /// Gets the hash code
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            // credit: http://stackoverflow.com/a/263416/677735
            unchecked // Overflow is fine, just wrap
            {
                int hash = 41;
                // Suitable nullity checks etc, of course :)
                if (this.SalesExciseDutyCode != null)
                    hash = hash * 59 + this.SalesExciseDutyCode.GetHashCode();
                if (this.SalesInvoiceDetailsCount != null)
                    hash = hash * 59 + this.SalesInvoiceDetailsCount.GetHashCode();
                if (this.PurchaseInvoiceDetailsCount != null)
                    hash = hash * 59 + this.PurchaseInvoiceDetailsCount.GetHashCode();
                if (this.ListPriceInTrl != null)
                    hash = hash * 59 + this.ListPriceInTrl.GetHashCode();
                if (this.BuyingPriceInTrl != null)
                    hash = hash * 59 + this.BuyingPriceInTrl.GetHashCode();
                if (this.StockCount != null)
                    hash = hash * 59 + this.StockCount.GetHashCode();
                if (this.Code != null)
                    hash = hash * 59 + this.Code.GetHashCode();
                if (this.Name != null)
                    hash = hash * 59 + this.Name.GetHashCode();
                if (this.VatRate != null)
                    hash = hash * 59 + this.VatRate.GetHashCode();
                if (this.SalesExciseDuty != null)
                    hash = hash * 59 + this.SalesExciseDuty.GetHashCode();
                if (this.SalesExciseDutyType != null)
                    hash = hash * 59 + this.SalesExciseDutyType.GetHashCode();
                if (this.PurchaseExciseDuty != null)
                    hash = hash * 59 + this.PurchaseExciseDuty.GetHashCode();
                if (this.PurchaseExciseDutyType != null)
                    hash = hash * 59 + this.PurchaseExciseDutyType.GetHashCode();
                if (this.Unit != null)
                    hash = hash * 59 + this.Unit.GetHashCode();
                if (this.CommunicationsTaxRate != null)
                    hash = hash * 59 + this.CommunicationsTaxRate.GetHashCode();
                if (this.Archived != null)
                    hash = hash * 59 + this.Archived.GetHashCode();
                if (this.ListPrice != null)
                    hash = hash * 59 + this.ListPrice.GetHashCode();
                if (this.Currency != null)
                    hash = hash * 59 + this.Currency.GetHashCode();
                if (this.BuyingPrice != null)
                    hash = hash * 59 + this.BuyingPrice.GetHashCode();
                if (this.BuyingCurrency != null)
                    hash = hash * 59 + this.BuyingCurrency.GetHashCode();
                if (this.InventoryTracking != null)
                    hash = hash * 59 + this.InventoryTracking.GetHashCode();
                if (this.InitialStockCount != null)
                    hash = hash * 59 + this.InitialStockCount.GetHashCode();
                return hash;
            }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        { 
            yield break;
        }
    }

}
