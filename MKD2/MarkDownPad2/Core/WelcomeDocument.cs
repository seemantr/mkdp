using MarkdownPad2.i18n;
using System;
using System.Text;
namespace MarkdownPad2.Core
{
	internal class WelcomeDocument
	{
		public static string Document
		{
			get
			{
				System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
				stringBuilder.AppendLine(LocalizationProvider.GetLocalizedString("WelcomeDoc_Intro", false, "MarkdownPadStrings"));
				stringBuilder.AppendLine();
				stringBuilder.AppendLine(LocalizationProvider.GetLocalizedString("WelcomeDoc_BuiltForMarkdownWithExamples", false, "MarkdownPadStrings"));
				stringBuilder.AppendLine();
				stringBuilder.AppendLine(LocalizationProvider.GetLocalizedString("WelcomeDoc_LivePreview", false, "MarkdownPadStrings"));
				stringBuilder.AppendLine();
				stringBuilder.AppendLine(LocalizationProvider.GetLocalizedString("WelcomeDoc_Customize", false, "MarkdownPadStrings"));
				stringBuilder.AppendLine();
				stringBuilder.AppendLine(LocalizationProvider.GetLocalizedString("WelcomeDoc_AdvancedFeatures", false, "MarkdownPadStrings"));
				return stringBuilder.ToString();
			}
		}
	}
}
