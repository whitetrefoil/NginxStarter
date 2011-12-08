using System;
using System.Windows.Data;

namespace NginxStarterGUI.Converters
{
	public class CheckboxConverter : IValueConverter
	{
		#region IValueConverter Members

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if ((bool)value) return true;
			else return false;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if ((bool?)value == true) return true;
			else return false;
		}

		#endregion
	}
}
