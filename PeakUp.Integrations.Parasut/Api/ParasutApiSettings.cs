using PeakUp.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeakUp.Integrations.Parasut.Api
{
    public static class ParasutApiSettings
    {
        public const string CompanyIdKey = "peakup.integrations.parasut.api.companyId";
        public const string UrlKey = "peakup.integrations.parasut.api.url";
        public const string VersionKey = "peakup.integrations.parasut.api.version";
        public const string ClientIdKey = "peakup.integrations.parasut.api.clientId";
        public const string ClientSecretKey = "peakup.integrations.parasut.api.clientSecret";
        public const string UsernameKey = "peakup.integrations.parasut.api.username";
        public const string PasswordKey = "peakup.integrations.parasut.api.password";
        public const string TokenType = "Bearer";

        static string Setting(string key) => ConfigurationManager.AppSettings[key];
        static string Setting(string key, string defaultValue) => Setting(key).As(value => string.IsNullOrEmpty(value) ? defaultValue : value);

        public static string CompanyId(string defaultValue = null) => Setting(CompanyIdKey, defaultValue);
        public static string Url(string defaultValue = null) => Setting(UrlKey, defaultValue);
        public static string Version(string defaultValue = null) => Setting(VersionKey, defaultValue);
        public static string ClientId(string defaultValue = null) => Setting(ClientIdKey, defaultValue);
        public static string ClientSecret(string defaulValue = null) => Setting(ClientSecretKey, defaulValue);
        public static string Username(string defaulValue = null) => Setting(UsernameKey, defaulValue);
        public static string Password(string defaulValue = null) => Setting(PasswordKey, defaulValue);
        public static string BaseUrl(string urlDefaultValue = null, string versionDefaultValue = null) => new Uri(new Uri(Url(urlDefaultValue)), Version(versionDefaultValue)).ToString();
    }
}
