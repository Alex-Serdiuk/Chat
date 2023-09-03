using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace Client.Converters
{
    // A class that implements IMultiValueConverter to check if all text boxes are filled
    public class AllFilledConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            //// Check if all values are booleans and false
            //foreach (object value in values)
            //{
            //    if (!(value is bool) || (bool)value)
            //    {
            //        return false;
            //    }
            //}
            //// Return true if all values are false
            //return true;


            //foreach (var value in values)
            //{
            //    if (string.IsNullOrEmpty(value as string))
            //    {
            //        return false; // Якщо хоча б один TextBox пустий, повертаємо false
            //    }
            //}

            //return true; // Всі TextBox-и мають текст, повертаємо true
            TextBox textBox = values[0] as TextBox;
            PasswordBox passwordBox = values[1] as PasswordBox;

            if (textBox == null || passwordBox == null)
                return false;

            string text = textBox.Text;
            string password = passwordBox.Password;

            // Перевірка, чи текст і пароль не є порожніми або null
            return !string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(password);


        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            // Not implemented
            throw new NotImplementedException();
        }
    }
}
