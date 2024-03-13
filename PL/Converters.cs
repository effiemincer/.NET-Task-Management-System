using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace PL; // Declaring the namespace for the PL (Presentation Layer) class.

// Class to convert an ID to content (string) for display purposes.
class ConvertIdToContent : IValueConverter
{
    // Converts an ID to content based on its value.
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // If the value is 0, returns "Add", otherwise returns "Update".
        return (int)value == 0 ? "Add" : "Update";
    }

    // Not implemented because this converter is for one-way conversion.
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
