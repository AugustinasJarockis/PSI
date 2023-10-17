using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace NOTEA.Extentions
{
    public static class EnumExtention
    {
        public static string ToDescription(this Enum value)
        {
            var attributes = (DescriptionAttribute[])value.GetType().GetField(value.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : value.ToString();
        }

        public static IEnumerable<SelectListItem> ToSelectList<T>(this Enum enumValue)
        {
            return
                Enum.GetValues(enumValue.GetType()).Cast<T>()
                      .Select(
                          x =>
                          new SelectListItem
                          {
                              Text = ((Enum)(object)x).ToDescription(),
                              Value = x.ToString(),
                              Selected = enumValue.Equals(x)
                          });
        }

        public static string GetDisplayName(this Enum enumValue)
        {
            return enumValue.GetType()
                            .GetMember(enumValue.ToString())
                            .First()
                            .GetCustomAttribute<DisplayAttribute>()
                            .GetName();
        }
    }
}
