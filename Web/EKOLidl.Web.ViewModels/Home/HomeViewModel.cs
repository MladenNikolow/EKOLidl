namespace EKOLidl.Web.ViewModels.Home
{
    using System.Collections.Generic;

    using EKOLidl.Data.Models;

    public class HomeViewModel
    {
        public List<Event> Events { get; set; }

        public List<Signal> Signals { get; set; }
    }
}
