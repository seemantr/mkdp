using MarkdownPad2.i18n;
using MarkdownPad2.Licensing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
namespace MarkdownPad2.Markdown
{
	public static class MarkdownProcessorProvider
	{
		public static System.Collections.Generic.Dictionary<MarkdownProcessorType, IMarkdownProcessor> MarkdownProcessorMap
		{
			get
			{
				return new System.Collections.Generic.Dictionary<MarkdownProcessorType, IMarkdownProcessor>
				{

					{
						MarkdownProcessorType.Markdown,
						new MarkdownProcessor()
					},

					{
						MarkdownProcessorType.MarkdownClassic,
						new MarkdownClassicProcessor()
					},

					{
						MarkdownProcessorType.MarkdownExtraMode,
						new MarkdownExtraProcessor()
					},

					{
						MarkdownProcessorType.GithubFlavoredMarkdown,
						new GitHubFlavoredMarkdownProcessor()
					},

					{
						MarkdownProcessorType.GitHubFlavoredMarkdownOffline,
						new GitHubFlavoredMarkdownOffline()
					}
				};
			}
		}
		public static System.Collections.Generic.List<IMarkdownProcessor> AvailableMarkdownProcessors
		{
			get
			{
				return (
					from pair in MarkdownProcessorProvider.MarkdownProcessorMap
					select pair.Value).ToList<IMarkdownProcessor>();
			}
		}
		public static MarkdownProcessorType ReverseLookup(IMarkdownProcessor processor)
		{
			System.Collections.Generic.Dictionary<IMarkdownProcessor, MarkdownProcessorType> dictionary = MarkdownProcessorProvider.MarkdownProcessorMap.ToDictionary((System.Collections.Generic.KeyValuePair<MarkdownProcessorType, IMarkdownProcessor> x) => x.Value, (System.Collections.Generic.KeyValuePair<MarkdownProcessorType, IMarkdownProcessor> x) => x.Key);
			return dictionary[processor];
		}
		public static MarkdownProcessorType ReverseLookupBad(IMarkdownProcessor processor)
		{
			MarkdownProcessorType result;
			if (processor is MarkdownProcessor)
			{
				result = MarkdownProcessorType.Markdown;
			}
			else
			{
				if (processor is MarkdownClassicProcessor)
				{
					result = MarkdownProcessorType.MarkdownClassic;
				}
				else
				{
					if (processor is MarkdownExtraProcessor)
					{
						result = MarkdownProcessorType.MarkdownExtraMode;
					}
					else
					{
						if (processor is GitHubFlavoredMarkdownProcessor)
						{
							result = MarkdownProcessorType.GithubFlavoredMarkdown;
						}
						else
						{
							if (!(processor is GitHubFlavoredMarkdownOffline))
							{
								throw new System.NotImplementedException("Missing processor type enum in ReverseLookup");
							}
							result = MarkdownProcessorType.GitHubFlavoredMarkdownOffline;
						}
					}
				}
			}
			return result;
		}
		public static void PopulateComboBoxWithMarkdownProcessors(ComboBox comboBox, MarkdownProcessorType selectedProcessor, Window window = null)
		{
			comboBox.ItemsSource = MarkdownProcessorProvider.AvailableMarkdownProcessors;
			comboBox.SelectedItem = MarkdownProcessorProvider.MarkdownProcessorMap[selectedProcessor];
			comboBox.SelectionChanged += delegate(object sender, SelectionChangedEventArgs args)
			{
				object obj = args.AddedItems[0];
				string text = string.Empty;
				if (obj is GitHubFlavoredMarkdownProcessor || obj is GitHubFlavoredMarkdownOffline)
				{
					text = LocalizationProvider.GetLocalizedString("Pro_GfmSupport", false, "MarkdownPadStrings");
				}
				else
				{
					if (obj is MarkdownExtraProcessor)
					{
						text = LocalizationProvider.GetLocalizedString("Pro_MarkdownTables", false, "MarkdownPadStrings");
					}
				}
				if (!string.IsNullOrEmpty(text) && !LicenseHelper.ValidateLicense(text, window))
				{
					comboBox.SelectedItem = args.RemovedItems[0];
					args.Handled = true;
				}
			};
		}
	}
}
