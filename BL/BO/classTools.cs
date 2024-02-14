

using System.Reflection;
using System.Text;

namespace BO;

public static class classTools
{
   public static string ToStringProperty<T>(this T obj)
    {
        if (obj == null)
        {
            return "null object";
        }

        StringBuilder stringBuilder = new StringBuilder();
        Type type = typeof(T);
        PropertyInfo[] properties = type.GetProperties();

        stringBuilder.Append($"{type.Name} properties:\n");

        foreach (PropertyInfo property in properties)
        {
            object value = property.GetValue(obj) ?? "null property";
            stringBuilder.Append($"{property.Name}: {value ?? "null"}\n");
        }

        return stringBuilder.ToString();
    }
}
