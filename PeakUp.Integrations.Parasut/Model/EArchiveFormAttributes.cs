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
    /// EArchiveFormAttributes
    /// </summary>
    [DataContract]
    public partial class EArchiveFormAttributes :  IEquatable<EArchiveFormAttributes>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EArchiveFormAttributes" /> class.
        /// </summary>
        /// <param name="VatWithholdingCode">Tevkifat oranına ait vergi kodu. - *Bu dosyada ilgili kodları bulabilirsiniz: https://www.efatura.gov.tr/dosyalar/kilavuzlar/UBLTR_1.2_Kilavuzlar.zip*.</param>
        /// <param name="VatExemptionReasonCode">KDV&#39;si %0 olan hizmet ve ürünlerin KDV muafiyet sebebi kodu. - *Bu dosyada ilgili kodları bulabilirsiniz: https://www.efatura.gov.tr/dosyalar/kilavuzlar/UBLTR_1.2_Kilavuzlar.zip*.</param>
        /// <param name="VatExemptionReason">Eğer KDV muafiyet sebebi kodu 250 veya 350 ise KDV muafiyet sebebi açıklaması..</param>
        /// <param name="Note">Fatura notu.</param>
        /// <param name="ExciseDutyCodes">ÖTV kodları - *Özel tüketim vergisi faturadaki her kalem için ayrıdır. ÖTV uygulanan her ürün için ÖTV kodu göndermeniz gerekmektedir.*.</param>
        /// <param name="InternetSale">InternetSale.</param>
        /// <param name="Shipment">Shipment.</param>
        public EArchiveFormAttributes(string VatWithholdingCode = default(string), string VatExemptionReasonCode = default(string), string VatExemptionReason = default(string), string Note = default(string), List<CompanyIdeArchivesDataAttributesExciseDutyCodes> ExciseDutyCodes = default(List<CompanyIdeArchivesDataAttributesExciseDutyCodes>), CompanyIdeArchivesDataAttributesInternetSale InternetSale = default(CompanyIdeArchivesDataAttributesInternetSale), CompanyIdeArchivesDataAttributesShipment Shipment = default(CompanyIdeArchivesDataAttributesShipment))
        {
            this.VatWithholdingCode = VatWithholdingCode;
            this.VatExemptionReasonCode = VatExemptionReasonCode;
            this.VatExemptionReason = VatExemptionReason;
            this.Note = Note;
            this.ExciseDutyCodes = ExciseDutyCodes;
            this.InternetSale = InternetSale;
            this.Shipment = Shipment;
        }
        
        /// <summary>
        /// Tevkifat oranına ait vergi kodu. - *Bu dosyada ilgili kodları bulabilirsiniz: https://www.efatura.gov.tr/dosyalar/kilavuzlar/UBLTR_1.2_Kilavuzlar.zip*
        /// </summary>
        /// <value>Tevkifat oranına ait vergi kodu. - *Bu dosyada ilgili kodları bulabilirsiniz: https://www.efatura.gov.tr/dosyalar/kilavuzlar/UBLTR_1.2_Kilavuzlar.zip*</value>
        [DataMember(Name="vat_withholding_code", EmitDefaultValue=false)]
        public string VatWithholdingCode { get; set; }
        /// <summary>
        /// KDV&#39;si %0 olan hizmet ve ürünlerin KDV muafiyet sebebi kodu. - *Bu dosyada ilgili kodları bulabilirsiniz: https://www.efatura.gov.tr/dosyalar/kilavuzlar/UBLTR_1.2_Kilavuzlar.zip*
        /// </summary>
        /// <value>KDV&#39;si %0 olan hizmet ve ürünlerin KDV muafiyet sebebi kodu. - *Bu dosyada ilgili kodları bulabilirsiniz: https://www.efatura.gov.tr/dosyalar/kilavuzlar/UBLTR_1.2_Kilavuzlar.zip*</value>
        [DataMember(Name="vat_exemption_reason_code", EmitDefaultValue=false)]
        public string VatExemptionReasonCode { get; set; }
        /// <summary>
        /// Eğer KDV muafiyet sebebi kodu 250 veya 350 ise KDV muafiyet sebebi açıklaması.
        /// </summary>
        /// <value>Eğer KDV muafiyet sebebi kodu 250 veya 350 ise KDV muafiyet sebebi açıklaması.</value>
        [DataMember(Name="vat_exemption_reason", EmitDefaultValue=false)]
        public string VatExemptionReason { get; set; }
        /// <summary>
        /// Fatura notu
        /// </summary>
        /// <value>Fatura notu</value>
        [DataMember(Name="note", EmitDefaultValue=false)]
        public string Note { get; set; }
        /// <summary>
        /// ÖTV kodları - *Özel tüketim vergisi faturadaki her kalem için ayrıdır. ÖTV uygulanan her ürün için ÖTV kodu göndermeniz gerekmektedir.*
        /// </summary>
        /// <value>ÖTV kodları - *Özel tüketim vergisi faturadaki her kalem için ayrıdır. ÖTV uygulanan her ürün için ÖTV kodu göndermeniz gerekmektedir.*</value>
        [DataMember(Name="excise_duty_codes", EmitDefaultValue=false)]
        public List<CompanyIdeArchivesDataAttributesExciseDutyCodes> ExciseDutyCodes { get; set; }
        /// <summary>
        /// Gets or Sets InternetSale
        /// </summary>
        [DataMember(Name="internet_sale", EmitDefaultValue=false)]
        public CompanyIdeArchivesDataAttributesInternetSale InternetSale { get; set; }
        /// <summary>
        /// Gets or Sets Shipment
        /// </summary>
        [DataMember(Name="shipment", EmitDefaultValue=false)]
        public CompanyIdeArchivesDataAttributesShipment Shipment { get; set; }
        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class EArchiveFormAttributes {\n");
            sb.Append("  VatWithholdingCode: ").Append(VatWithholdingCode).Append("\n");
            sb.Append("  VatExemptionReasonCode: ").Append(VatExemptionReasonCode).Append("\n");
            sb.Append("  VatExemptionReason: ").Append(VatExemptionReason).Append("\n");
            sb.Append("  Note: ").Append(Note).Append("\n");
            sb.Append("  ExciseDutyCodes: ").Append(ExciseDutyCodes).Append("\n");
            sb.Append("  InternetSale: ").Append(InternetSale).Append("\n");
            sb.Append("  Shipment: ").Append(Shipment).Append("\n");
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
            return this.Equals(obj as EArchiveFormAttributes);
        }

        /// <summary>
        /// Returns true if EArchiveFormAttributes instances are equal
        /// </summary>
        /// <param name="other">Instance of EArchiveFormAttributes to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(EArchiveFormAttributes other)
        {
            // credit: http://stackoverflow.com/a/10454552/677735
            if (other == null)
                return false;

            return 
                (
                    this.VatWithholdingCode == other.VatWithholdingCode ||
                    this.VatWithholdingCode != null &&
                    this.VatWithholdingCode.Equals(other.VatWithholdingCode)
                ) && 
                (
                    this.VatExemptionReasonCode == other.VatExemptionReasonCode ||
                    this.VatExemptionReasonCode != null &&
                    this.VatExemptionReasonCode.Equals(other.VatExemptionReasonCode)
                ) && 
                (
                    this.VatExemptionReason == other.VatExemptionReason ||
                    this.VatExemptionReason != null &&
                    this.VatExemptionReason.Equals(other.VatExemptionReason)
                ) && 
                (
                    this.Note == other.Note ||
                    this.Note != null &&
                    this.Note.Equals(other.Note)
                ) && 
                (
                    this.ExciseDutyCodes == other.ExciseDutyCodes ||
                    this.ExciseDutyCodes != null &&
                    this.ExciseDutyCodes.SequenceEqual(other.ExciseDutyCodes)
                ) && 
                (
                    this.InternetSale == other.InternetSale ||
                    this.InternetSale != null &&
                    this.InternetSale.Equals(other.InternetSale)
                ) && 
                (
                    this.Shipment == other.Shipment ||
                    this.Shipment != null &&
                    this.Shipment.Equals(other.Shipment)
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
                if (this.VatWithholdingCode != null)
                    hash = hash * 59 + this.VatWithholdingCode.GetHashCode();
                if (this.VatExemptionReasonCode != null)
                    hash = hash * 59 + this.VatExemptionReasonCode.GetHashCode();
                if (this.VatExemptionReason != null)
                    hash = hash * 59 + this.VatExemptionReason.GetHashCode();
                if (this.Note != null)
                    hash = hash * 59 + this.Note.GetHashCode();
                if (this.ExciseDutyCodes != null)
                    hash = hash * 59 + this.ExciseDutyCodes.GetHashCode();
                if (this.InternetSale != null)
                    hash = hash * 59 + this.InternetSale.GetHashCode();
                if (this.Shipment != null)
                    hash = hash * 59 + this.Shipment.GetHashCode();
                return hash;
            }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        { 
            yield break;
        }
    }

}
