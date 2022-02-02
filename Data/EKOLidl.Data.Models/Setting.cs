namespace EKOLidl.Data.Models
{
    using EKOLidl.Data.Common.Models;

    public class Setting : BaseDeletableModel<int>
    {
        public string Name { get; set; }

        public string Value { get; set; }
    }
}
