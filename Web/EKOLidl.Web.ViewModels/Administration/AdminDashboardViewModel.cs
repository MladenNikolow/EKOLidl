namespace EKOLidl.Web.ViewModels.Administration
{
    using System.Collections.Generic;

    using EKOLidl.Data.Models;

    public class AdminDashboardViewModel
    {
        public int EventsCount { get; set; }

        public int PendingSignalsCount { get; set; }

        public int ApprovedSignalsCount { get; set; }

        /// <summary>
        /// Gets or sets the newest signals waiting for moderation.
        /// </summary>
        public IEnumerable<Signal> LatestPending { get; set; }
    }
}
