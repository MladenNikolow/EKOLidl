namespace EKOLidl.Data.Models.Signals
{
    using System.ComponentModel;

    /// <summary>
    /// Enum describing the trash in the signal.
    /// </summary>
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
