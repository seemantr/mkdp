using System;
namespace MarkdownPad2.Updater
{
	public enum UpdateFrequency
	{
		Always,
		Daily = 24,
		Weekly = 168,
		BiWeekly = 336,
		Monthly = 720,
		Never = 2147483647
	}
}
