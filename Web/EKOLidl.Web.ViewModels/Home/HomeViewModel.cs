namespace EKOLidl.Web.ViewModels.Home
{
    using System.Collections.Generic;

    using EKOLidl.Data.Models;

    public class HomeViewModel
    {
        public IEnumerable<Event> Events { get; set; }

        public IEnumerable<Signal> Signals { get; set; }
    }
}
