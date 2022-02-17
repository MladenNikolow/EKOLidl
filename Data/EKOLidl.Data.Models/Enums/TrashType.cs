using System.ComponentModel;

namespace EKOLidl.Data.Models.Enums
{
    public enum TrashType
    {
        Unknown = 0,

        [Description("Хартия")]
        Paper,

        [Description("Метал")]
        Metal,

        [Description("Пластмаса")]
        Plastic,

        [Description("Стъкло")]
        Glass,

        [Description("Дрехи")]
        Clothes,

        [Description("Други")]
        Other,
    }
}
