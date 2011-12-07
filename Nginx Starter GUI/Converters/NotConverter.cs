using System;
using System.Windows.Data;

namespace NginxStarterGUI.Converters
{
	[ValueConversion(typeof(bool), typeof(bool))]
	public class NotConverter : IValueConverter
	{
		#region IValueConverter Members

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return !(bool)value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return this.Convert(value, targetType, parameter, culture);
		}

		#endregion
	}
}
