using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Markup;
namespace MarkdownPad2.UI
{
	public class EnumerationExtension : System.Windows.Markup.MarkupExtension
	{
		public class EnumerationMember
		{
			public string Description
			{
				get;
				set;
			}
			public object Value
			{
				get;
				set;
			}
		}
		private System.Type _enumType;
		public System.Type EnumType
		{
			get
			{
				return this._enumType;
			}
			private set
			{
				if (this._enumType == value)
				{
					return;
				}
				System.Type type = System.Nullable.GetUnderlyingType(value) ?? value;
				if (!type.IsEnum)
				{
					throw new System.ArgumentException("Type must be an Enum.");
				}
				this._enumType = value;
			}
		}
		public EnumerationExtension(System.Type enumType)
		{
			if (enumType == null)
			{
				throw new System.ArgumentNullException("enumType");
			}
			this.EnumType = enumType;
		}
		public override object ProvideValue(System.IServiceProvider serviceProvider)
		{
			System.Array values = System.Enum.GetValues(this.EnumType);
			return (
				from object enumValue in values
				select new EnumerationExtension.EnumerationMember
				{
					Value = enumValue,
					Description = this.GetDescription(enumValue)
				}).ToArray<EnumerationExtension.EnumerationMember>();
		}
		private string GetDescription(object enumValue)
		{
			DescriptionAttribute descriptionAttribute = this.EnumType.GetField(enumValue.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault<object>() as DescriptionAttribute;
			if (descriptionAttribute == null)
			{
				return enumValue.ToString();
			}
			return descriptionAttribute.Description;
		}
	}
}
