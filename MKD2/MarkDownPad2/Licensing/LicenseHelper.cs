using MarkdownPad2.i18n;
using MarkdownPad2.Properties;
using MarkdownPad2.Utilities;
using System;
using System.Windows;
using WPFCustomMessageBox;
namespace MarkdownPad2.Licensing
{
	internal class LicenseHelper
	{
		internal static bool ValidateLicense(string featureDescription, Window owner)
		{
			Settings @default = Settings.Default;
			LicenseEngine licenseEngine = new LicenseEngine();
			bool flag = licenseEngine.VerifyLicense(@default.App_LicenseKey, @default.App_LicenseEmail);
			if (!flag)
			{
				string localizedString = LocalizationProvider.GetLocalizedString("Pro_OneOfManyFeatures", false, "MarkdownPadStrings");
				string localizedString2 = LocalizationProvider.GetLocalizedString("Pro_LearnMore", false, "MarkdownPadStrings");
				string localizedString3 = LocalizationProvider.GetLocalizedString("Pro_OnlyAvailableInPro", false, "MarkdownPadStrings");
				MessageBoxResult messageBoxResult = CustomMessageBox.ShowYesNo(string.Concat(new string[]
				{
					featureDescription,
					" ",
					localizedString,
					StringUtilities.GetNewLines(2),
					localizedString2
				}), localizedString3, LocalizationProvider.GetLocalizedString("Yes", false, "MarkdownPadStrings"), LocalizationProvider.GetLocalizedString("No", false, "MarkdownPadStrings"), MessageBoxImage.Asterisk);
				if (messageBoxResult == MessageBoxResult.Yes)
				{
					UpgradeProWindow upgradeProWindow = new UpgradeProWindow
					{
						Owner = owner
					};
					upgradeProWindow.ShowDialog();
				}
			}
			return flag;
		}
	}
}
