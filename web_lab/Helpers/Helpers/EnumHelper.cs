using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Domain.Helpers
{
    public static class EnumHelper
    {
        public static IEnumerable<SelectListItem> ToSelectList<TEnum>()
        {
            var enumType = typeof(TEnum);

            return Enum.GetValues(enumType)
                       .Cast<TEnum>()
                       .Select(e => new SelectListItem
                       {
                           Value = e.ToString(),
                           Text = GetEnumDisplayName(e)
                       })
                       .ToList();
        }

        private static string GetEnumDisplayName<TEnum>(TEnum enumValue)
        {
            var fieldInfo = enumValue
                .GetType()
                .GetField(enumValue.ToString());
            var attribute = (DisplayAttribute)fieldInfo.GetCustomAttribute(typeof(DisplayAttribute));
            return attribute?.Name ?? enumValue.ToString();
        }
    }
}
