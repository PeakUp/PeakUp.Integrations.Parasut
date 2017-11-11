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
    /// InlineResponse200Attributes
    /// </summary>
    [DataContract]
    public partial class InlineResponse200Attributes :  IEquatable<InlineResponse200Attributes>, IValidatableObject
    {
        /// <summary>
        /// Döviz cinsi
        /// </summary>
        /// <value>Döviz cinsi</value>
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
        /// Hesap tipi
        /// </summary>
        /// <value>Hesap tipi</value>
        [JsonConverter(typeof(StringEnumConverter))]
        public enum AccountTypeEnum
        {
            
            /// <summary>
            /// Enum Cash for "cash"
            /// </summary>
            [EnumMember(Value = "cash")]
            Cash,
            
            /// <summary>
            /// Enum Bank for "bank"
            /// </summary>
            [EnumMember(Value = "bank")]
            Bank,
            
            /// <summary>
            /// Enum Sys for "sys"
            /// </summary>
            [EnumMember(Value = "sys")]
            Sys
        }

        /// <summary>
        /// Döviz cinsi
        /// </summary>
        /// <value>Döviz cinsi</value>
        [DataMember(Name="currency", EmitDefaultValue=false)]
        public CurrencyEnum? Currency { get; set; }
        /// <summary>
        /// Hesap tipi
        /// </summary>
        /// <value>Hesap tipi</value>
        [DataMember(Name="account_type", EmitDefaultValue=false)]
        public AccountTypeEnum? AccountType { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="InlineResponse200Attributes" /> class.
        /// </summary>
        [JsonConstructorAttribute]
        protected InlineResponse200Attributes() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="InlineResponse200Attributes" /> class.
        /// </summary>
        /// <param name="Name">Hesap ismi (required).</param>
        /// <param name="Currency">Döviz cinsi.</param>
        /// <param name="AccountType">Hesap tipi.</param>
        /// <param name="BankName">Banka ismi.</param>
        /// <param name="BankBranch">Banka şubesi.</param>
        /// <param name="BankAccountNo">Banka hesap no.</param>
        /// <param name="Iban">IBAN.</param>
        /// <param name="Archived">Archived.</param>
        public InlineResponse200Attributes(string Name = default(string), CurrencyEnum? Currency = default(CurrencyEnum?), AccountTypeEnum? AccountType = default(AccountTypeEnum?), string BankName = default(string), string BankBranch = default(string), string BankAccountNo = default(string), string Iban = default(string), bool? Archived = default(bool?))
        {
            // to ensure "Name" is required (not null)
            if (Name == null)
            {
                throw new InvalidDataException("Name is a required property for InlineResponse200Attributes and cannot be null");
            }
            else
            {
                this.Name = Name;
            }
            this.Currency = Currency;
            this.AccountType = AccountType;
            this.BankName = BankName;
            this.BankBranch = BankBranch;
            this.BankAccountNo = BankAccountNo;
            this.Iban = Iban;
            this.Archived = Archived;
        }
        
        /// <summary>
        /// Gets or Sets UsedFor
        /// </summary>
        [DataMember(Name="used_for", EmitDefaultValue=false)]
        public string UsedFor { get; private set; }
        /// <summary>
        /// Gets or Sets LastUsedAt
        /// </summary>
        [DataMember(Name="last_used_at", EmitDefaultValue=false)]
        public DateTime? LastUsedAt { get; private set; }
        /// <summary>
        /// Bakiye
        /// </summary>
        /// <value>Bakiye</value>
        [DataMember(Name="balance", EmitDefaultValue=false)]
        public decimal? Balance { get; private set; }
        /// <summary>
        /// Gets or Sets LastAdjustmentDate
        /// </summary>
        [DataMember(Name="last_adjustment_date", EmitDefaultValue=false)]
        public DateTime? LastAdjustmentDate { get; private set; }
        /// <summary>
        /// Gets or Sets BankIntegrationType
        /// </summary>
        [DataMember(Name="bank_integration_type", EmitDefaultValue=false)]
        public string BankIntegrationType { get; private set; }
        /// <summary>
        /// Gets or Sets AssociateEmail
        /// </summary>
        [DataMember(Name="associate_email", EmitDefaultValue=false)]
        public string AssociateEmail { get; private set; }
        /// <summary>
        /// Hesap ismi
        /// </summary>
        /// <value>Hesap ismi</value>
        [DataMember(Name="name", EmitDefaultValue=false)]
        public string Name { get; set; }
        /// <summary>
        /// Banka ismi
        /// </summary>
        /// <value>Banka ismi</value>
        [DataMember(Name="bank_name", EmitDefaultValue=false)]
        public string BankName { get; set; }
        /// <summary>
        /// Banka şubesi
        /// </summary>
        /// <value>Banka şubesi</value>
        [DataMember(Name="bank_branch", EmitDefaultValue=false)]
        public string BankBranch { get; set; }
        /// <summary>
        /// Banka hesap no
        /// </summary>
        /// <value>Banka hesap no</value>
        [DataMember(Name="bank_account_no", EmitDefaultValue=false)]
        public string BankAccountNo { get; set; }
        /// <summary>
        /// IBAN
        /// </summary>
        /// <value>IBAN</value>
        [DataMember(Name="iban", EmitDefaultValue=false)]
        public string Iban { get; set; }
        /// <summary>
        /// Gets or Sets Archived
        /// </summary>
        [DataMember(Name="archived", EmitDefaultValue=false)]
        public bool? Archived { get; set; }
        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class InlineResponse200Attributes {\n");
            sb.Append("  UsedFor: ").Append(UsedFor).Append("\n");
            sb.Append("  LastUsedAt: ").Append(LastUsedAt).Append("\n");
            sb.Append("  Balance: ").Append(Balance).Append("\n");
            sb.Append("  LastAdjustmentDate: ").Append(LastAdjustmentDate).Append("\n");
            sb.Append("  BankIntegrationType: ").Append(BankIntegrationType).Append("\n");
            sb.Append("  AssociateEmail: ").Append(AssociateEmail).Append("\n");
            sb.Append("  Name: ").Append(Name).Append("\n");
            sb.Append("  Currency: ").Append(Currency).Append("\n");
            sb.Append("  AccountType: ").Append(AccountType).Append("\n");
            sb.Append("  BankName: ").Append(BankName).Append("\n");
            sb.Append("  BankBranch: ").Append(BankBranch).Append("\n");
            sb.Append("  BankAccountNo: ").Append(BankAccountNo).Append("\n");
            sb.Append("  Iban: ").Append(Iban).Append("\n");
            sb.Append("  Archived: ").Append(Archived).Append("\n");
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
            return this.Equals(obj as InlineResponse200Attributes);
        }

        /// <summary>
        /// Returns true if InlineResponse200Attributes instances are equal
        /// </summary>
        /// <param name="other">Instance of InlineResponse200Attributes to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(InlineResponse200Attributes other)
        {
            // credit: http://stackoverflow.com/a/10454552/677735
            if (other == null)
                return false;

            return 
                (
                    this.UsedFor == other.UsedFor ||
                    this.UsedFor != null &&
                    this.UsedFor.Equals(other.UsedFor)
                ) && 
                (
                    this.LastUsedAt == other.LastUsedAt ||
                    this.LastUsedAt != null &&
                    this.LastUsedAt.Equals(other.LastUsedAt)
                ) && 
                (
                    this.Balance == other.Balance ||
                    this.Balance != null &&
                    this.Balance.Equals(other.Balance)
                ) && 
                (
                    this.LastAdjustmentDate == other.LastAdjustmentDate ||
                    this.LastAdjustmentDate != null &&
                    this.LastAdjustmentDate.Equals(other.LastAdjustmentDate)
                ) && 
                (
                    this.BankIntegrationType == other.BankIntegrationType ||
                    this.BankIntegrationType != null &&
                    this.BankIntegrationType.Equals(other.BankIntegrationType)
                ) && 
                (
                    this.AssociateEmail == other.AssociateEmail ||
                    this.AssociateEmail != null &&
                    this.AssociateEmail.Equals(other.AssociateEmail)
                ) && 
                (
                    this.Name == other.Name ||
                    this.Name != null &&
                    this.Name.Equals(other.Name)
                ) && 
                (
                    this.Currency == other.Currency ||
                    this.Currency != null &&
                    this.Currency.Equals(other.Currency)
                ) && 
                (
                    this.AccountType == other.AccountType ||
                    this.AccountType != null &&
                    this.AccountType.Equals(other.AccountType)
                ) && 
                (
                    this.BankName == other.BankName ||
                    this.BankName != null &&
                    this.BankName.Equals(other.BankName)
                ) && 
                (
                    this.BankBranch == other.BankBranch ||
                    this.BankBranch != null &&
                    this.BankBranch.Equals(other.BankBranch)
                ) && 
                (
                    this.BankAccountNo == other.BankAccountNo ||
                    this.BankAccountNo != null &&
                    this.BankAccountNo.Equals(other.BankAccountNo)
                ) && 
                (
                    this.Iban == other.Iban ||
                    this.Iban != null &&
                    this.Iban.Equals(other.Iban)
                ) && 
                (
                    this.Archived == other.Archived ||
                    this.Archived != null &&
                    this.Archived.Equals(other.Archived)
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
                if (this.UsedFor != null)
                    hash = hash * 59 + this.UsedFor.GetHashCode();
                if (this.LastUsedAt != null)
                    hash = hash * 59 + this.LastUsedAt.GetHashCode();
                if (this.Balance != null)
                    hash = hash * 59 + this.Balance.GetHashCode();
                if (this.LastAdjustmentDate != null)
                    hash = hash * 59 + this.LastAdjustmentDate.GetHashCode();
                if (this.BankIntegrationType != null)
                    hash = hash * 59 + this.BankIntegrationType.GetHashCode();
                if (this.AssociateEmail != null)
                    hash = hash * 59 + this.AssociateEmail.GetHashCode();
                if (this.Name != null)
                    hash = hash * 59 + this.Name.GetHashCode();
                if (this.Currency != null)
                    hash = hash * 59 + this.Currency.GetHashCode();
                if (this.AccountType != null)
                    hash = hash * 59 + this.AccountType.GetHashCode();
                if (this.BankName != null)
                    hash = hash * 59 + this.BankName.GetHashCode();
                if (this.BankBranch != null)
                    hash = hash * 59 + this.BankBranch.GetHashCode();
                if (this.BankAccountNo != null)
                    hash = hash * 59 + this.BankAccountNo.GetHashCode();
                if (this.Iban != null)
                    hash = hash * 59 + this.Iban.GetHashCode();
                if (this.Archived != null)
                    hash = hash * 59 + this.Archived.GetHashCode();
                return hash;
            }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        { 
            yield break;
        }
    }

}
