using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace NoteaAPI.Extentions
{
    public static class EnumExtension
    {
        public static string? GetDisplayName(this Enum enumValue)
        {
            return enumValue?.GetType()
                            ?.GetMember(enumValue.ToString())
                            ?.First()
                            ?.GetCustomAttribute<DisplayAttribute>()
                            ?.GetName();
        }
    }
}
