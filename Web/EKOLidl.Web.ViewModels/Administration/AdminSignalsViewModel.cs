namespace EKOLidl.Web.ViewModels.Administration
{
    using System;
    using System.Collections.Generic;

    using EKOLidl.Data.Models;

    public class AdminSignalsViewModel
    {
        public IEnumerable<Signal> Signals { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the list shows published (true) or pending (false) signals.
        /// </summary>
        public bool ShowsApproved { get; set; }

        public int PendingCount { get; set; }

        public int ApprovedCount { get; set; }

        public int Page { get; set; }

        public int SignalsPerPage { get; set; }

        public int PagesCount => (int)Math.Ceiling((double)(this.ShowsApproved ? this.ApprovedCount : this.PendingCount) / this.SignalsPerPage);
    }
}
