using System;
using System.Text;
namespace MarkdownPad2.Utilities
{
	public class HttpUtilities
	{
		public static string JavaScriptStringEncode(string value)
		{
			return HttpUtilities.JavaScriptStringEncode(value, false);
		}
		public static string JavaScriptStringEncode(string value, bool addDoubleQuotes)
		{
			if (string.IsNullOrEmpty(value))
			{
				if (!addDoubleQuotes)
				{
					return string.Empty;
				}
				return "\"\"";
			}
			else
			{
				int length = value.Length;
				bool flag = false;
				for (int i = 0; i < length; i++)
				{
					char c = value[i];
					if ((c >= '\0' && c <= '\u001f') || c == '"' || c == '\'' || c == '<' || c == '>' || c == '\\')
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
					if (addDoubleQuotes)
					{
						stringBuilder.Append('"');
					}
					for (int j = 0; j < length; j++)
					{
						char c = value[j];
						if ((c >= '\0' && c <= '\a') || (c == '\v' || (c >= '\u000e' && c <= '\u001f')) || c == '\'' || c == '<' || c == '>')
						{
							stringBuilder.AppendFormat("\\u{0:x4}", (int)c);
						}
						else
						{
							int num = (int)c;
							switch (num)
							{
							case 8:
								stringBuilder.Append("\\b");
								goto IL_17A;
							case 9:
								stringBuilder.Append("\\t");
								goto IL_17A;
							case 10:
								stringBuilder.Append("\\n");
								goto IL_17A;
							case 11:
								break;
							case 12:
								stringBuilder.Append("\\f");
								goto IL_17A;
							case 13:
								stringBuilder.Append("\\r");
								goto IL_17A;
							default:
								if (num == 34)
								{
									stringBuilder.Append("\\\"");
									goto IL_17A;
								}
								if (num == 92)
								{
									stringBuilder.Append("\\\\");
									goto IL_17A;
								}
								break;
							}
							stringBuilder.Append(c);
						}
						IL_17A:;
					}
					if (addDoubleQuotes)
					{
						stringBuilder.Append('"');
					}
					return stringBuilder.ToString();
				}
				if (!addDoubleQuotes)
				{
					return value;
				}
				return "\"" + value + "\"";
			}
		}
	}
}
