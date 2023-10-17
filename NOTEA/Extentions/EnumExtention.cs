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

        public static string? GetDisplayName(this Enum enumValue)
        {
            return enumValue?.GetType()
                            ?.GetMember(enumValue.ToString())
                            ?.First()
                            ?.GetCustomAttribute<DisplayAttribute>()
                            ?.GetName();
        }

        public static IEnumerable<SelectListItem> GetSelectList<T>()
        {
            var groupDictionary = new Dictionary<string, SelectListGroup>();

            var enumType = typeof(T);
            var fields = from field in enumType.GetFields()
                         where field.IsLiteral
                         select field;

            foreach (var field in fields)
            {
                var display = field.GetCustomAttribute<DisplayAttribute>(false);
                var description = field.GetCustomAttribute<DescriptionAttribute>(false);
                var group = field.GetCustomAttribute<CategoryAttribute>(false);

                var text = display?.GetName() ?? display?.GetShortName() ?? display?.GetDescription() ?? display?.GetPrompt() ?? description?.Description ?? field.Name;
                var value = field.Name;
                var groupName = display?.GetGroupName() ?? group?.Category ?? string.Empty;
                if (!groupDictionary.ContainsKey(groupName)) { groupDictionary.Add(groupName, new SelectListGroup { Name = groupName }); }

                yield return new SelectListItem
                {
                    Text = text,
                    Value = value,
                    Group = groupDictionary[groupName],
                };
            }
        }
    }
}
