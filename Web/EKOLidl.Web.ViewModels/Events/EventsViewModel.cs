namespace EKOLidl.Web.ViewModels.Events
{
    using System;
    using System.Collections.Generic;

    using EKOLidl.Data.Models;

    public class EventsViewModel
    {
        public IEnumerable<Event> Events { get; set; }

        public int Page { get; set; }

        public int Count { get; set; }

        public int EventsPerPage { get; set; }

        public bool HasPreviousPage => this.Page > 1;

        public int PreviousPageNumber => this.Page - 1;

        public int PagesCount => (int)Math.Ceiling((double)this.Count / this.EventsPerPage);

        public bool HasNextPage => this.Page < this.PagesCount;

        public int NextPageNumber => this.Page + 1;
    }
}
