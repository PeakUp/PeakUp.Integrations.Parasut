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
    /// ContactAttributes
    /// </summary>
    [DataContract]
    public partial class ContactAttributes :  IEquatable<ContactAttributes>, IValidatableObject
    {
        /// <summary>
        /// Tip
        /// </summary>
        /// <value>Tip</value>
        [JsonConverter(typeof(StringEnumConverter))]
        public enum ContactTypeEnum
        {
            
            /// <summary>
            /// Enum Person for "person"
            /// </summary>
            [EnumMember(Value = "person")]
            Person,
            
            /// <summary>
            /// Enum Company for "company"
            /// </summary>
            [EnumMember(Value = "company")]
            Company
        }

        /// <summary>
        /// Gets or Sets AccountType
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enum AccountTypeEnum
        {
            
            /// <summary>
            /// Enum Customer for "customer"
            /// </summary>
            [EnumMember(Value = "customer")]
            Customer,
            
            /// <summary>
            /// Enum Supplier for "supplier"
            /// </summary>
            [EnumMember(Value = "supplier")]
            Supplier
        }

        /// <summary>
        /// Tip
        /// </summary>
        /// <value>Tip</value>
        [DataMember(Name="contact_type", EmitDefaultValue=false)]
        public ContactTypeEnum? ContactType { get; set; }
        /// <summary>
        /// Gets or Sets AccountType
        /// </summary>
        [DataMember(Name="account_type", EmitDefaultValue=false)]
        public AccountTypeEnum? AccountType { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="ContactAttributes" /> class.
        /// </summary>
        [JsonConstructorAttribute]
        protected ContactAttributes() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ContactAttributes" /> class.
        /// </summary>
        /// <param name="Email">E-posta.</param>
        /// <param name="Name">Müşteri/tedarikçi ismi (required).</param>
        /// <param name="ShortName">ShortName.</param>
        /// <param name="ContactType">Tip.</param>
        /// <param name="TaxOffice">Vergi dairesi.</param>
        /// <param name="TaxNumber">Vergi numarası/TC kimlik no.</param>
        /// <param name="District">İlçe.</param>
        /// <param name="City">İl.</param>
        /// <param name="Address">Address.</param>
        /// <param name="Phone">Phone.</param>
        /// <param name="Fax">Fax.</param>
        /// <param name="IsAbroad">IsAbroad.</param>
        /// <param name="Archived">Archived.</param>
        /// <param name="Iban">Iban.</param>
        /// <param name="AccountType">AccountType (required).</param>
        public ContactAttributes(string Email = default(string), string Name = default(string), string ShortName = default(string), ContactTypeEnum? ContactType = default(ContactTypeEnum?), string TaxOffice = default(string), string TaxNumber = default(string), string District = default(string), string City = default(string), string Address = default(string), string Phone = default(string), string Fax = default(string), bool? IsAbroad = default(bool?), bool? Archived = default(bool?), string Iban = default(string), AccountTypeEnum? AccountType = default(AccountTypeEnum?))
        {
            // to ensure "Name" is required (not null)
            if (Name == null)
            {
                throw new InvalidDataException("Name is a required property for ContactAttributes and cannot be null");
            }
            else
            {
                this.Name = Name;
            }
            // to ensure "AccountType" is required (not null)
            if (AccountType == null)
            {
                throw new InvalidDataException("AccountType is a required property for ContactAttributes and cannot be null");
            }
            else
            {
                this.AccountType = AccountType;
            }
            this.Email = Email;
            this.ShortName = ShortName;
            this.ContactType = ContactType;
            this.TaxOffice = TaxOffice;
            this.TaxNumber = TaxNumber;
            this.District = District;
            this.City = City;
            this.Address = Address;
            this.Phone = Phone;
            this.Fax = Fax;
            this.IsAbroad = IsAbroad;
            this.Archived = Archived;
            this.Iban = Iban;
        }
        
        /// <summary>
        /// Bakiye
        /// </summary>
        /// <value>Bakiye</value>
        [DataMember(Name="balance", EmitDefaultValue=false)]
        public decimal? Balance { get; private set; }
        /// <summary>
        /// TL Bakiye
        /// </summary>
        /// <value>TL Bakiye</value>
        [DataMember(Name="trl_balance", EmitDefaultValue=false)]
        public decimal? TrlBalance { get; private set; }
        /// <summary>
        /// USD Bakiye
        /// </summary>
        /// <value>USD Bakiye</value>
        [DataMember(Name="usd_balance", EmitDefaultValue=false)]
        public decimal? UsdBalance { get; private set; }
        /// <summary>
        /// EUR Bakiye
        /// </summary>
        /// <value>EUR Bakiye</value>
        [DataMember(Name="eur_balance", EmitDefaultValue=false)]
        public decimal? EurBalance { get; private set; }
        /// <summary>
        /// GBP Bakiye
        /// </summary>
        /// <value>GBP Bakiye</value>
        [DataMember(Name="gbp_balance", EmitDefaultValue=false)]
        public decimal? GbpBalance { get; private set; }
        /// <summary>
        /// E-posta
        /// </summary>
        /// <value>E-posta</value>
        [DataMember(Name="email", EmitDefaultValue=false)]
        public string Email { get; set; }
        /// <summary>
        /// Müşteri/tedarikçi ismi
        /// </summary>
        /// <value>Müşteri/tedarikçi ismi</value>
        [DataMember(Name="name", EmitDefaultValue=false)]
        public string Name { get; set; }
        /// <summary>
        /// Gets or Sets ShortName
        /// </summary>
        [DataMember(Name="short_name", EmitDefaultValue=false)]
        public string ShortName { get; set; }
        /// <summary>
        /// Vergi dairesi
        /// </summary>
        /// <value>Vergi dairesi</value>
        [DataMember(Name="tax_office", EmitDefaultValue=false)]
        public string TaxOffice { get; set; }
        /// <summary>
        /// Vergi numarası/TC kimlik no
        /// </summary>
        /// <value>Vergi numarası/TC kimlik no</value>
        [DataMember(Name="tax_number", EmitDefaultValue=false)]
        public string TaxNumber { get; set; }
        /// <summary>
        /// İlçe
        /// </summary>
        /// <value>İlçe</value>
        [DataMember(Name="district", EmitDefaultValue=false)]
        public string District { get; set; }
        /// <summary>
        /// İl
        /// </summary>
        /// <value>İl</value>
        [DataMember(Name="city", EmitDefaultValue=false)]
        public string City { get; set; }
        /// <summary>
        /// Gets or Sets Address
        /// </summary>
        [DataMember(Name="address", EmitDefaultValue=false)]
        public string Address { get; set; }
        /// <summary>
        /// Gets or Sets Phone
        /// </summary>
        [DataMember(Name="phone", EmitDefaultValue=false)]
        public string Phone { get; set; }
        /// <summary>
        /// Gets or Sets Fax
        /// </summary>
        [DataMember(Name="fax", EmitDefaultValue=false)]
        public string Fax { get; set; }
        /// <summary>
        /// Gets or Sets IsAbroad
        /// </summary>
        [DataMember(Name="is_abroad", EmitDefaultValue=false)]
        public bool? IsAbroad { get; set; }
        /// <summary>
        /// Gets or Sets Archived
        /// </summary>
        [DataMember(Name="archived", EmitDefaultValue=false)]
        public bool? Archived { get; set; }
        /// <summary>
        /// Gets or Sets Iban
        /// </summary>
        [DataMember(Name="iban", EmitDefaultValue=false)]
        public string Iban { get; set; }
        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class ContactAttributes {\n");
            sb.Append("  Balance: ").Append(Balance).Append("\n");
            sb.Append("  TrlBalance: ").Append(TrlBalance).Append("\n");
            sb.Append("  UsdBalance: ").Append(UsdBalance).Append("\n");
            sb.Append("  EurBalance: ").Append(EurBalance).Append("\n");
            sb.Append("  GbpBalance: ").Append(GbpBalance).Append("\n");
            sb.Append("  Email: ").Append(Email).Append("\n");
            sb.Append("  Name: ").Append(Name).Append("\n");
            sb.Append("  ShortName: ").Append(ShortName).Append("\n");
            sb.Append("  ContactType: ").Append(ContactType).Append("\n");
            sb.Append("  TaxOffice: ").Append(TaxOffice).Append("\n");
            sb.Append("  TaxNumber: ").Append(TaxNumber).Append("\n");
            sb.Append("  District: ").Append(District).Append("\n");
            sb.Append("  City: ").Append(City).Append("\n");
            sb.Append("  Address: ").Append(Address).Append("\n");
            sb.Append("  Phone: ").Append(Phone).Append("\n");
            sb.Append("  Fax: ").Append(Fax).Append("\n");
            sb.Append("  IsAbroad: ").Append(IsAbroad).Append("\n");
            sb.Append("  Archived: ").Append(Archived).Append("\n");
            sb.Append("  Iban: ").Append(Iban).Append("\n");
            sb.Append("  AccountType: ").Append(AccountType).Append("\n");
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
            return this.Equals(obj as ContactAttributes);
        }

        /// <summary>
        /// Returns true if ContactAttributes instances are equal
        /// </summary>
        /// <param name="other">Instance of ContactAttributes to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(ContactAttributes other)
        {
            // credit: http://stackoverflow.com/a/10454552/677735
            if (other == null)
                return false;

            return 
                (
                    this.Balance == other.Balance ||
                    this.Balance != null &&
                    this.Balance.Equals(other.Balance)
                ) && 
                (
                    this.TrlBalance == other.TrlBalance ||
                    this.TrlBalance != null &&
                    this.TrlBalance.Equals(other.TrlBalance)
                ) && 
                (
                    this.UsdBalance == other.UsdBalance ||
                    this.UsdBalance != null &&
                    this.UsdBalance.Equals(other.UsdBalance)
                ) && 
                (
                    this.EurBalance == other.EurBalance ||
                    this.EurBalance != null &&
                    this.EurBalance.Equals(other.EurBalance)
                ) && 
                (
                    this.GbpBalance == other.GbpBalance ||
                    this.GbpBalance != null &&
                    this.GbpBalance.Equals(other.GbpBalance)
                ) && 
                (
                    this.Email == other.Email ||
                    this.Email != null &&
                    this.Email.Equals(other.Email)
                ) && 
                (
                    this.Name == other.Name ||
                    this.Name != null &&
                    this.Name.Equals(other.Name)
                ) && 
                (
                    this.ShortName == other.ShortName ||
                    this.ShortName != null &&
                    this.ShortName.Equals(other.ShortName)
                ) && 
                (
                    this.ContactType == other.ContactType ||
                    this.ContactType != null &&
                    this.ContactType.Equals(other.ContactType)
                ) && 
                (
                    this.TaxOffice == other.TaxOffice ||
                    this.TaxOffice != null &&
                    this.TaxOffice.Equals(other.TaxOffice)
                ) && 
                (
                    this.TaxNumber == other.TaxNumber ||
                    this.TaxNumber != null &&
                    this.TaxNumber.Equals(other.TaxNumber)
                ) && 
                (
                    this.District == other.District ||
                    this.District != null &&
                    this.District.Equals(other.District)
                ) && 
                (
                    this.City == other.City ||
                    this.City != null &&
                    this.City.Equals(other.City)
                ) && 
                (
                    this.Address == other.Address ||
                    this.Address != null &&
                    this.Address.Equals(other.Address)
                ) && 
                (
                    this.Phone == other.Phone ||
                    this.Phone != null &&
                    this.Phone.Equals(other.Phone)
                ) && 
                (
                    this.Fax == other.Fax ||
                    this.Fax != null &&
                    this.Fax.Equals(other.Fax)
                ) && 
                (
                    this.IsAbroad == other.IsAbroad ||
                    this.IsAbroad != null &&
                    this.IsAbroad.Equals(other.IsAbroad)
                ) && 
                (
                    this.Archived == other.Archived ||
                    this.Archived != null &&
                    this.Archived.Equals(other.Archived)
                ) && 
                (
                    this.Iban == other.Iban ||
                    this.Iban != null &&
                    this.Iban.Equals(other.Iban)
                ) && 
                (
                    this.AccountType == other.AccountType ||
                    this.AccountType != null &&
                    this.AccountType.Equals(other.AccountType)
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
                if (this.Balance != null)
                    hash = hash * 59 + this.Balance.GetHashCode();
                if (this.TrlBalance != null)
                    hash = hash * 59 + this.TrlBalance.GetHashCode();
                if (this.UsdBalance != null)
                    hash = hash * 59 + this.UsdBalance.GetHashCode();
                if (this.EurBalance != null)
                    hash = hash * 59 + this.EurBalance.GetHashCode();
                if (this.GbpBalance != null)
                    hash = hash * 59 + this.GbpBalance.GetHashCode();
                if (this.Email != null)
                    hash = hash * 59 + this.Email.GetHashCode();
                if (this.Name != null)
                    hash = hash * 59 + this.Name.GetHashCode();
                if (this.ShortName != null)
                    hash = hash * 59 + this.ShortName.GetHashCode();
                if (this.ContactType != null)
                    hash = hash * 59 + this.ContactType.GetHashCode();
                if (this.TaxOffice != null)
                    hash = hash * 59 + this.TaxOffice.GetHashCode();
                if (this.TaxNumber != null)
                    hash = hash * 59 + this.TaxNumber.GetHashCode();
                if (this.District != null)
                    hash = hash * 59 + this.District.GetHashCode();
                if (this.City != null)
                    hash = hash * 59 + this.City.GetHashCode();
                if (this.Address != null)
                    hash = hash * 59 + this.Address.GetHashCode();
                if (this.Phone != null)
                    hash = hash * 59 + this.Phone.GetHashCode();
                if (this.Fax != null)
                    hash = hash * 59 + this.Fax.GetHashCode();
                if (this.IsAbroad != null)
                    hash = hash * 59 + this.IsAbroad.GetHashCode();
                if (this.Archived != null)
                    hash = hash * 59 + this.Archived.GetHashCode();
                if (this.Iban != null)
                    hash = hash * 59 + this.Iban.GetHashCode();
                if (this.AccountType != null)
                    hash = hash * 59 + this.AccountType.GetHashCode();
                return hash;
            }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        { 
            yield break;
        }
    }

}
