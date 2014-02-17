using MarkdownPad2.i18n;
using System;
using System.Collections.Generic;
namespace MarkdownPad2.Updater
{
	internal static class UpdateProvider
	{
		public static System.Collections.Generic.Dictionary<UpdateFrequency, string> UpdateFrequencyMap
		{
			get
			{
				return new System.Collections.Generic.Dictionary<UpdateFrequency, string>
				{

					{
						UpdateFrequency.Always,
						LocalizationProvider.GetLocalizedString("UpdateFrequency_Always", false, "MarkdownPadStrings")
					},

					{
						UpdateFrequency.Daily,
						LocalizationProvider.GetLocalizedString("UpdateFrequency_Daily", false, "MarkdownPadStrings")
					},

					{
						UpdateFrequency.Weekly,
						LocalizationProvider.GetLocalizedString("UpdateFrequency_Weekly", false, "MarkdownPadStrings")
					},

					{
						UpdateFrequency.BiWeekly,
						LocalizationProvider.GetLocalizedString("UpdateFrequency_BiWeekly", false, "MarkdownPadStrings")
					},

					{
						UpdateFrequency.Monthly,
						LocalizationProvider.GetLocalizedString("UpdateFrequency_Monthly", false, "MarkdownPadStrings")
					},

					{
						UpdateFrequency.Never,
						LocalizationProvider.GetLocalizedString("UpdateFrequency_Never", false, "MarkdownPadStrings")
					}
				};
			}
		}
	}
}
