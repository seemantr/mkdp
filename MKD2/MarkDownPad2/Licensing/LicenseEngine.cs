using MarkdownPad2.Core;
using MarkdownPad2.i18n;
using MarkdownPad2.Utilities;
using NLog;
using OpenSSL.Core;
using OpenSSL.Crypto;
using RestSharp;
using RestSharp.Deserializers;
using System;
using System.Text;
namespace MarkdownPad2.Licensing
{
	public class LicenseEngine
	{
		private const string publicKeyString = "-----BEGIN PUBLIC KEY-----\nMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAqYO2D1DzVA3Q4JXtt0GK\nPKzrA1uu8LKU48f/IRI2vH+I81IwIPGQErDT4SNTjSyLIzYAcJnSUpLbUxj2OhFc\nrSRK/9e7PffJtus9h/ph8Tma5G4e2f7prNxSBTRqiUghB4ZCPJs2NzTA9Q4Rqqe+\n38/2ESGU1G/vOHW71XXKoQkLK8PbU8eRWlCybn5ZElsrCJOlZRdvcmoc/n7IyeBc\n2KJyl3BWhkRLcBDV2YAM1VtU0+jw1aCXltTOoVKFamIqcblYt1a9oJbv4+mPcBnJ\ny4XwuWSTgdfcLu3e1hLPFpBh9aQsxmqPn2OuxQqfggvLeStwazwNCiXJbSgE6XEr\npQIDAQAB\n-----END PUBLIC KEY-----";
		private static Logger _logger = LogManager.GetCurrentClassLogger();
		public License License
		{
			get;
			private set;
		}
		public string LicensedToMessage
		{
			get
			{
				if (this.License == null)
				{
					return string.Empty;
				}
				return string.Format("{0} {1}", LocalizationProvider.GetLocalizedString("License_LicensedTo", false, "MarkdownPadStrings"), this.License.Name);
			}
		}
		public bool LicenseProcessed
		{
			get;
			private set;
		}
		public bool VerifyLicense(string licenseKey, string email)
		{
			this.LicenseProcessed = true;
			return true;
			bool result;
			try
			{
			}
			catch (System.FormatException exception)
			{
				LicenseEngine._logger.ErrorException("Bad license format", exception);
				MessageBoxHelper.ShowWarningMessageBox(LocalizationProvider.GetLocalizedString("License_BadFormat", false, "MarkdownPadStrings") + StringUtilities.GetNewLines(2) + LocalizationProvider.GetLocalizedString("License_PleaseVerify", false, "MarkdownPadStrings"), LocalizationProvider.GetLocalizedString("License_ErrorTitle", false, "MarkdownPadStrings"));
				result = false;
			}
			catch (OpenSslException exception2)
			{
				LicenseEngine._logger.ErrorException("Error decrypting license", exception2);
				MessageBoxHelper.ShowWarningMessageBox(LocalizationProvider.GetLocalizedString("License_ErrorMessage", false, "MarkdownPadStrings") + StringUtilities.GetNewLines(2) + LocalizationProvider.GetLocalizedString("License_PleaseVerify", false, "MarkdownPadStrings"), LocalizationProvider.GetLocalizedString("License_ErrorTitle", false, "MarkdownPadStrings"));
				result = false;
			}
			catch (System.Exception exception3)
			{
				LicenseEngine._logger.ErrorException("Error processing license: " + licenseKey, exception3);
				MessageBoxHelper.ShowErrorMessageBox(LocalizationProvider.GetLocalizedString("License_ErrorMessage", false, "MarkdownPadStrings") + StringUtilities.GetNewLines(2) + LocalizationProvider.GetLocalizedString("License_PleaseVerify", false, "MarkdownPadStrings"), LocalizationProvider.GetLocalizedString("License_ErrorTitle", false, "MarkdownPadStrings"), exception3, "");
				result = false;
			}
			return result;
		}
		private License Decrypt(string payload)
		{
			CryptoKey cryptoKey = CryptoKey.FromPublicKey("-----BEGIN PUBLIC KEY-----\nMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAqYO2D1DzVA3Q4JXtt0GK\nPKzrA1uu8LKU48f/IRI2vH+I81IwIPGQErDT4SNTjSyLIzYAcJnSUpLbUxj2OhFc\nrSRK/9e7PffJtus9h/ph8Tma5G4e2f7prNxSBTRqiUghB4ZCPJs2NzTA9Q4Rqqe+\n38/2ESGU1G/vOHW71XXKoQkLK8PbU8eRWlCybn5ZElsrCJOlZRdvcmoc/n7IyeBc\n2KJyl3BWhkRLcBDV2YAM1VtU0+jw1aCXltTOoVKFamIqcblYt1a9oJbv4+mPcBnJ\ny4XwuWSTgdfcLu3e1hLPFpBh9aQsxmqPn2OuxQqfggvLeStwazwNCiXJbSgE6XEr\npQIDAQAB\n-----END PUBLIC KEY-----", "2QJmLPD5ktxIrFkr");
			RSA rSA = cryptoKey.GetRSA();
			byte[] msg = System.Convert.FromBase64String(payload);
			byte[] bytes = rSA.PublicDecrypt(msg, RSA.Padding.PKCS1);
			string @string = System.Text.Encoding.Default.GetString(bytes);
			IRestResponse restResponse = new RestResponse();
			restResponse.Content = @string;
			JsonDeserializer jsonDeserializer = new JsonDeserializer();
			License result = jsonDeserializer.Deserialize<License>(restResponse);
			rSA.Dispose();
			return result;
		}
	}
}
