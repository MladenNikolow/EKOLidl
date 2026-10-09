namespace EKOLidl.Data.Models.Signals
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;

    public static class TrashTypeExtensions
    {
        /// <summary>
        /// Gets the Bulgarian display name from the enum value's <see cref="DescriptionAttribute"/>.
        /// </summary>
        /// <param name="trashType">The trash type.</param>
        /// <returns>The display name, or the enum name when no description is set.</returns>
        public static string GetDescription(this TrashType trashType)
        {
            FieldInfo field = typeof(TrashType).GetField(trashType.ToString());
            DescriptionAttribute attribute = field?.GetCustomAttribute<DescriptionAttribute>();

            return attribute?.Description ?? trashType.ToString();
        }

        /// <summary>
        /// Gets the trash types a reporter can pick, keyed by their numeric value.
        /// </summary>
        /// <returns>Value/description pairs, without <see cref="TrashType.Unknown"/>.</returns>
        public static List<KeyValuePair<int, string>> GetSelectableItems()
        {
            return Enum.GetValues<TrashType>()
                .Where(x => x != TrashType.Unknown)
                .Select(x => new KeyValuePair<int, string>((int)x, x.GetDescription()))
                .ToList();
        }
    }
}
