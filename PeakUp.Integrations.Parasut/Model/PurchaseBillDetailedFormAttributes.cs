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
    /// PurchaseBillDetailedFormAttributes
    /// </summary>
    [DataContract]
    public partial class PurchaseBillDetailedFormAttributes :  IEquatable<PurchaseBillDetailedFormAttributes>, IValidatableObject
    {
        /// <summary>
        /// Gets or Sets ItemType
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enum ItemTypeEnum
        {
            
            /// <summary>
            /// Enum Purchasebill for "purchase_bill"
            /// </summary>
            [EnumMember(Value = "purchase_bill")]
            Purchasebill,
            
            /// <summary>
            /// Enum Refund for "refund"
            /// </summary>
            [EnumMember(Value = "refund")]
            Refund
        }

        /// <summary>
        /// Gets or Sets Currency
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enum CurrencyEnum
        {
            
            /// <summary>
            /// Enum TRL for "TRL"
            /// </summary>
            [EnumMember(Value = "TRL")]
            TRL,
            
            /// <summary>
            /// Enum USD for "USD"
            /// </summary>
            [EnumMember(Value = "USD")]
            USD,
            
            /// <summary>
            /// Enum EUR for "EUR"
            /// </summary>
            [EnumMember(Value = "EUR")]
            EUR,
            
            /// <summary>
            /// Enum GBP for "GBP"
            /// </summary>
            [EnumMember(Value = "GBP")]
            GBP
        }

        /// <summary>
        /// Gets or Sets InvoiceDiscountType
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enum InvoiceDiscountTypeEnum
        {
            
            /// <summary>
            /// Enum Percentage for "amount percentage"
            /// </summary>
            [EnumMember(Value = "amount percentage")]
            Percentage
        }

        /// <summary>
        /// Gets or Sets ItemType
        /// </summary>
        [DataMember(Name="item_type", EmitDefaultValue=false)]
        public ItemTypeEnum? ItemType { get; set; }
        /// <summary>
        /// Gets or Sets Currency
        /// </summary>
        [DataMember(Name="currency", EmitDefaultValue=false)]
        public CurrencyEnum? Currency { get; set; }
        /// <summary>
        /// Gets or Sets InvoiceDiscountType
        /// </summary>
        [DataMember(Name="invoice_discount_type", EmitDefaultValue=false)]
        public InvoiceDiscountTypeEnum? InvoiceDiscountType { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="PurchaseBillDetailedFormAttributes" /> class.
        /// </summary>
        [JsonConstructorAttribute]
        protected PurchaseBillDetailedFormAttributes() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="PurchaseBillDetailedFormAttributes" /> class.
        /// </summary>
        /// <param name="ItemType">ItemType (required).</param>
        /// <param name="Description">Description.</param>
        /// <param name="IssueDate">IssueDate (required).</param>
        /// <param name="DueDate">DueDate (required).</param>
        /// <param name="InvoiceNo">InvoiceNo.</param>
        /// <param name="Currency">Currency (required).</param>
        /// <param name="ExchangeRate">ExchangeRate.</param>
        /// <param name="WithholdingRate">WithholdingRate.</param>
        /// <param name="VatWithholdingRate">VatWithholdingRate.</param>
        /// <param name="InvoiceDiscount">InvoiceDiscount.</param>
        /// <param name="InvoiceDiscountType">InvoiceDiscountType.</param>
        public PurchaseBillDetailedFormAttributes(ItemTypeEnum? ItemType = default(ItemTypeEnum?), string Description = default(string), DateTime? IssueDate = default(DateTime?), DateTime? DueDate = default(DateTime?), string InvoiceNo = default(string), CurrencyEnum? Currency = default(CurrencyEnum?), decimal? ExchangeRate = default(decimal?), decimal? WithholdingRate = default(decimal?), decimal? VatWithholdingRate = default(decimal?), decimal? InvoiceDiscount = default(decimal?), InvoiceDiscountTypeEnum? InvoiceDiscountType = default(InvoiceDiscountTypeEnum?))
        {
            // to ensure "ItemType" is required (not null)
            if (ItemType == null)
            {
                throw new InvalidDataException("ItemType is a required property for PurchaseBillDetailedFormAttributes and cannot be null");
            }
            else
            {
                this.ItemType = ItemType;
            }
            // to ensure "IssueDate" is required (not null)
            if (IssueDate == null)
            {
                throw new InvalidDataException("IssueDate is a required property for PurchaseBillDetailedFormAttributes and cannot be null");
            }
            else
            {
                this.IssueDate = IssueDate;
            }
            // to ensure "DueDate" is required (not null)
            if (DueDate == null)
            {
                throw new InvalidDataException("DueDate is a required property for PurchaseBillDetailedFormAttributes and cannot be null");
            }
            else
            {
                this.DueDate = DueDate;
            }
            // to ensure "Currency" is required (not null)
            if (Currency == null)
            {
                throw new InvalidDataException("Currency is a required property for PurchaseBillDetailedFormAttributes and cannot be null");
            }
            else
            {
                this.Currency = Currency;
            }
            this.Description = Description;
            this.InvoiceNo = InvoiceNo;
            this.ExchangeRate = ExchangeRate;
            this.WithholdingRate = WithholdingRate;
            this.VatWithholdingRate = VatWithholdingRate;
            this.InvoiceDiscount = InvoiceDiscount;
            this.InvoiceDiscountType = InvoiceDiscountType;
        }
        
        /// <summary>
        /// Gets or Sets Description
        /// </summary>
        [DataMember(Name="description", EmitDefaultValue=false)]
        public string Description { get; set; }
        /// <summary>
        /// Gets or Sets IssueDate
        /// </summary>
        [DataMember(Name="issue_date", EmitDefaultValue=false)]
        public DateTime? IssueDate { get; set; }
        /// <summary>
        /// Gets or Sets DueDate
        /// </summary>
        [DataMember(Name="due_date", EmitDefaultValue=false)]
        public DateTime? DueDate { get; set; }
        /// <summary>
        /// Gets or Sets InvoiceNo
        /// </summary>
        [DataMember(Name="invoice_no", EmitDefaultValue=false)]
        public string InvoiceNo { get; set; }
        /// <summary>
        /// Gets or Sets ExchangeRate
        /// </summary>
        [DataMember(Name="exchange_rate", EmitDefaultValue=false)]
        public decimal? ExchangeRate { get; set; }
        /// <summary>
        /// Gets or Sets WithholdingRate
        /// </summary>
        [DataMember(Name="withholding_rate", EmitDefaultValue=false)]
        public decimal? WithholdingRate { get; set; }
        /// <summary>
        /// Gets or Sets VatWithholdingRate
        /// </summary>
        [DataMember(Name="vat_withholding_rate", EmitDefaultValue=false)]
        public decimal? VatWithholdingRate { get; set; }
        /// <summary>
        /// Gets or Sets InvoiceDiscount
        /// </summary>
        [DataMember(Name="invoice_discount", EmitDefaultValue=false)]
        public decimal? InvoiceDiscount { get; set; }
        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class PurchaseBillDetailedFormAttributes {\n");
            sb.Append("  ItemType: ").Append(ItemType).Append("\n");
            sb.Append("  Description: ").Append(Description).Append("\n");
            sb.Append("  IssueDate: ").Append(IssueDate).Append("\n");
            sb.Append("  DueDate: ").Append(DueDate).Append("\n");
            sb.Append("  InvoiceNo: ").Append(InvoiceNo).Append("\n");
            sb.Append("  Currency: ").Append(Currency).Append("\n");
            sb.Append("  ExchangeRate: ").Append(ExchangeRate).Append("\n");
            sb.Append("  WithholdingRate: ").Append(WithholdingRate).Append("\n");
            sb.Append("  VatWithholdingRate: ").Append(VatWithholdingRate).Append("\n");
            sb.Append("  InvoiceDiscount: ").Append(InvoiceDiscount).Append("\n");
            sb.Append("  InvoiceDiscountType: ").Append(InvoiceDiscountType).Append("\n");
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
            return this.Equals(obj as PurchaseBillDetailedFormAttributes);
        }

        /// <summary>
        /// Returns true if PurchaseBillDetailedFormAttributes instances are equal
        /// </summary>
        /// <param name="other">Instance of PurchaseBillDetailedFormAttributes to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(PurchaseBillDetailedFormAttributes other)
        {
            // credit: http://stackoverflow.com/a/10454552/677735
            if (other == null)
                return false;

            return 
                (
                    this.ItemType == other.ItemType ||
                    this.ItemType != null &&
                    this.ItemType.Equals(other.ItemType)
                ) && 
                (
                    this.Description == other.Description ||
                    this.Description != null &&
                    this.Description.Equals(other.Description)
                ) && 
                (
                    this.IssueDate == other.IssueDate ||
                    this.IssueDate != null &&
                    this.IssueDate.Equals(other.IssueDate)
                ) && 
                (
                    this.DueDate == other.DueDate ||
                    this.DueDate != null &&
                    this.DueDate.Equals(other.DueDate)
                ) && 
                (
                    this.InvoiceNo == other.InvoiceNo ||
                    this.InvoiceNo != null &&
                    this.InvoiceNo.Equals(other.InvoiceNo)
                ) && 
                (
                    this.Currency == other.Currency ||
                    this.Currency != null &&
                    this.Currency.Equals(other.Currency)
                ) && 
                (
                    this.ExchangeRate == other.ExchangeRate ||
                    this.ExchangeRate != null &&
                    this.ExchangeRate.Equals(other.ExchangeRate)
                ) && 
                (
                    this.WithholdingRate == other.WithholdingRate ||
                    this.WithholdingRate != null &&
                    this.WithholdingRate.Equals(other.WithholdingRate)
                ) && 
                (
                    this.VatWithholdingRate == other.VatWithholdingRate ||
                    this.VatWithholdingRate != null &&
                    this.VatWithholdingRate.Equals(other.VatWithholdingRate)
                ) && 
                (
                    this.InvoiceDiscount == other.InvoiceDiscount ||
                    this.InvoiceDiscount != null &&
                    this.InvoiceDiscount.Equals(other.InvoiceDiscount)
                ) && 
                (
                    this.InvoiceDiscountType == other.InvoiceDiscountType ||
                    this.InvoiceDiscountType != null &&
                    this.InvoiceDiscountType.Equals(other.InvoiceDiscountType)
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
                if (this.ItemType != null)
                    hash = hash * 59 + this.ItemType.GetHashCode();
                if (this.Description != null)
                    hash = hash * 59 + this.Description.GetHashCode();
                if (this.IssueDate != null)
                    hash = hash * 59 + this.IssueDate.GetHashCode();
                if (this.DueDate != null)
                    hash = hash * 59 + this.DueDate.GetHashCode();
                if (this.InvoiceNo != null)
                    hash = hash * 59 + this.InvoiceNo.GetHashCode();
                if (this.Currency != null)
                    hash = hash * 59 + this.Currency.GetHashCode();
                if (this.ExchangeRate != null)
                    hash = hash * 59 + this.ExchangeRate.GetHashCode();
                if (this.WithholdingRate != null)
                    hash = hash * 59 + this.WithholdingRate.GetHashCode();
                if (this.VatWithholdingRate != null)
                    hash = hash * 59 + this.VatWithholdingRate.GetHashCode();
                if (this.InvoiceDiscount != null)
                    hash = hash * 59 + this.InvoiceDiscount.GetHashCode();
                if (this.InvoiceDiscountType != null)
                    hash = hash * 59 + this.InvoiceDiscountType.GetHashCode();
                return hash;
            }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        { 
            yield break;
        }
    }

}
