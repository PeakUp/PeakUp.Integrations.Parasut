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
    /// SalesInvoiceDetailAttributes
    /// </summary>
    [DataContract]
    public partial class SalesInvoiceDetailAttributes :  IEquatable<SalesInvoiceDetailAttributes>, IValidatableObject
    {
        /// <summary>
        /// İndirim türü
        /// </summary>
        /// <value>İndirim türü</value>
        [JsonConverter(typeof(StringEnumConverter))]
        public enum DiscountTypeEnum
        {
            
            /// <summary>
            /// Enum Percentage for "amount percentage"
            /// </summary>
            [EnumMember(Value = "amount percentage")]
            Percentage
        }

        /// <summary>
        /// ÖTV tipi
        /// </summary>
        /// <value>ÖTV tipi</value>
        [JsonConverter(typeof(StringEnumConverter))]
        public enum ExciseDutyTypeEnum
        {
            
            /// <summary>
            /// Enum Percentage for "amount percentage"
            /// </summary>
            [EnumMember(Value = "amount percentage")]
            Percentage
        }

        /// <summary>
        /// İndirim türü
        /// </summary>
        /// <value>İndirim türü</value>
        [DataMember(Name="discount_type", EmitDefaultValue=false)]
        public DiscountTypeEnum? DiscountType { get; set; }
        /// <summary>
        /// ÖTV tipi
        /// </summary>
        /// <value>ÖTV tipi</value>
        [DataMember(Name="excise_duty_type", EmitDefaultValue=false)]
        public ExciseDutyTypeEnum? ExciseDutyType { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="SalesInvoiceDetailAttributes" /> class.
        /// </summary>
        [JsonConstructorAttribute]
        protected SalesInvoiceDetailAttributes() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="SalesInvoiceDetailAttributes" /> class.
        /// </summary>
        /// <param name="Quantity">Miktar (required).</param>
        /// <param name="UnitPrice">Birim fiyatı (required).</param>
        /// <param name="VatRate">KDV oranı (required).</param>
        /// <param name="DiscountType">İndirim türü.</param>
        /// <param name="DiscountValue">DiscountValue.</param>
        /// <param name="ExciseDutyType">ÖTV tipi.</param>
        /// <param name="ExciseDutyValue">ExciseDutyValue.</param>
        /// <param name="CommunicationsTaxRate">ÖİV oranı.</param>
        /// <param name="Description">Açıklama.</param>
        public SalesInvoiceDetailAttributes(decimal? Quantity = default(decimal?), decimal? UnitPrice = default(decimal?), decimal? VatRate = default(decimal?), DiscountTypeEnum? DiscountType = default(DiscountTypeEnum?), decimal? DiscountValue = default(decimal?), ExciseDutyTypeEnum? ExciseDutyType = default(ExciseDutyTypeEnum?), decimal? ExciseDutyValue = default(decimal?), decimal? CommunicationsTaxRate = default(decimal?), string Description = default(string))
        {
            // to ensure "Quantity" is required (not null)
            if (Quantity == null)
            {
                throw new InvalidDataException("Quantity is a required property for SalesInvoiceDetailAttributes and cannot be null");
            }
            else
            {
                this.Quantity = Quantity;
            }
            // to ensure "UnitPrice" is required (not null)
            if (UnitPrice == null)
            {
                throw new InvalidDataException("UnitPrice is a required property for SalesInvoiceDetailAttributes and cannot be null");
            }
            else
            {
                this.UnitPrice = UnitPrice;
            }
            // to ensure "VatRate" is required (not null)
            if (VatRate == null)
            {
                throw new InvalidDataException("VatRate is a required property for SalesInvoiceDetailAttributes and cannot be null");
            }
            else
            {
                this.VatRate = VatRate;
            }
            this.DiscountType = DiscountType;
            this.DiscountValue = DiscountValue;
            this.ExciseDutyType = ExciseDutyType;
            this.ExciseDutyValue = ExciseDutyValue;
            this.CommunicationsTaxRate = CommunicationsTaxRate;
            this.Description = Description;
        }
        
        /// <summary>
        /// Ürün/hizmet net tutarı
        /// </summary>
        /// <value>Ürün/hizmet net tutarı</value>
        [DataMember(Name="net_total", EmitDefaultValue=false)]
        public decimal? NetTotal { get; private set; }
        /// <summary>
        /// Miktar
        /// </summary>
        /// <value>Miktar</value>
        [DataMember(Name="quantity", EmitDefaultValue=false)]
        public decimal? Quantity { get; set; }
        /// <summary>
        /// Birim fiyatı
        /// </summary>
        /// <value>Birim fiyatı</value>
        [DataMember(Name="unit_price", EmitDefaultValue=false)]
        public decimal? UnitPrice { get; set; }
        /// <summary>
        /// KDV oranı
        /// </summary>
        /// <value>KDV oranı</value>
        [DataMember(Name="vat_rate", EmitDefaultValue=false)]
        public decimal? VatRate { get; set; }
        /// <summary>
        /// Gets or Sets DiscountValue
        /// </summary>
        [DataMember(Name="discount_value", EmitDefaultValue=false)]
        public decimal? DiscountValue { get; set; }
        /// <summary>
        /// Gets or Sets ExciseDutyValue
        /// </summary>
        [DataMember(Name="excise_duty_value", EmitDefaultValue=false)]
        public decimal? ExciseDutyValue { get; set; }
        /// <summary>
        /// ÖİV oranı
        /// </summary>
        /// <value>ÖİV oranı</value>
        [DataMember(Name="communications_tax_rate", EmitDefaultValue=false)]
        public decimal? CommunicationsTaxRate { get; set; }
        /// <summary>
        /// Açıklama
        /// </summary>
        /// <value>Açıklama</value>
        [DataMember(Name="description", EmitDefaultValue=false)]
        public string Description { get; set; }
        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class SalesInvoiceDetailAttributes {\n");
            sb.Append("  NetTotal: ").Append(NetTotal).Append("\n");
            sb.Append("  Quantity: ").Append(Quantity).Append("\n");
            sb.Append("  UnitPrice: ").Append(UnitPrice).Append("\n");
            sb.Append("  VatRate: ").Append(VatRate).Append("\n");
            sb.Append("  DiscountType: ").Append(DiscountType).Append("\n");
            sb.Append("  DiscountValue: ").Append(DiscountValue).Append("\n");
            sb.Append("  ExciseDutyType: ").Append(ExciseDutyType).Append("\n");
            sb.Append("  ExciseDutyValue: ").Append(ExciseDutyValue).Append("\n");
            sb.Append("  CommunicationsTaxRate: ").Append(CommunicationsTaxRate).Append("\n");
            sb.Append("  Description: ").Append(Description).Append("\n");
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
            return this.Equals(obj as SalesInvoiceDetailAttributes);
        }

        /// <summary>
        /// Returns true if SalesInvoiceDetailAttributes instances are equal
        /// </summary>
        /// <param name="other">Instance of SalesInvoiceDetailAttributes to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(SalesInvoiceDetailAttributes other)
        {
            // credit: http://stackoverflow.com/a/10454552/677735
            if (other == null)
                return false;

            return 
                (
                    this.NetTotal == other.NetTotal ||
                    this.NetTotal != null &&
                    this.NetTotal.Equals(other.NetTotal)
                ) && 
                (
                    this.Quantity == other.Quantity ||
                    this.Quantity != null &&
                    this.Quantity.Equals(other.Quantity)
                ) && 
                (
                    this.UnitPrice == other.UnitPrice ||
                    this.UnitPrice != null &&
                    this.UnitPrice.Equals(other.UnitPrice)
                ) && 
                (
                    this.VatRate == other.VatRate ||
                    this.VatRate != null &&
                    this.VatRate.Equals(other.VatRate)
                ) && 
                (
                    this.DiscountType == other.DiscountType ||
                    this.DiscountType != null &&
                    this.DiscountType.Equals(other.DiscountType)
                ) && 
                (
                    this.DiscountValue == other.DiscountValue ||
                    this.DiscountValue != null &&
                    this.DiscountValue.Equals(other.DiscountValue)
                ) && 
                (
                    this.ExciseDutyType == other.ExciseDutyType ||
                    this.ExciseDutyType != null &&
                    this.ExciseDutyType.Equals(other.ExciseDutyType)
                ) && 
                (
                    this.ExciseDutyValue == other.ExciseDutyValue ||
                    this.ExciseDutyValue != null &&
                    this.ExciseDutyValue.Equals(other.ExciseDutyValue)
                ) && 
                (
                    this.CommunicationsTaxRate == other.CommunicationsTaxRate ||
                    this.CommunicationsTaxRate != null &&
                    this.CommunicationsTaxRate.Equals(other.CommunicationsTaxRate)
                ) && 
                (
                    this.Description == other.Description ||
                    this.Description != null &&
                    this.Description.Equals(other.Description)
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
                if (this.NetTotal != null)
                    hash = hash * 59 + this.NetTotal.GetHashCode();
                if (this.Quantity != null)
                    hash = hash * 59 + this.Quantity.GetHashCode();
                if (this.UnitPrice != null)
                    hash = hash * 59 + this.UnitPrice.GetHashCode();
                if (this.VatRate != null)
                    hash = hash * 59 + this.VatRate.GetHashCode();
                if (this.DiscountType != null)
                    hash = hash * 59 + this.DiscountType.GetHashCode();
                if (this.DiscountValue != null)
                    hash = hash * 59 + this.DiscountValue.GetHashCode();
                if (this.ExciseDutyType != null)
                    hash = hash * 59 + this.ExciseDutyType.GetHashCode();
                if (this.ExciseDutyValue != null)
                    hash = hash * 59 + this.ExciseDutyValue.GetHashCode();
                if (this.CommunicationsTaxRate != null)
                    hash = hash * 59 + this.CommunicationsTaxRate.GetHashCode();
                if (this.Description != null)
                    hash = hash * 59 + this.Description.GetHashCode();
                return hash;
            }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        { 
            yield break;
        }
    }

}
