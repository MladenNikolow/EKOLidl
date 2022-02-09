namespace EKOLidl.Web.ViewModels
{
    using System;

    public class BaseViewModel
    {
        public string Id { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }
    }
}
