namespace EKOLidl.Web.ViewModels.Signals
{
    using System;
    using System.Collections.Generic;

    using EKOLidl.Data.Models;

    public class SignalsViewModel
    {
        public IEnumerable<Signal> Signals { get; set; }

        public int Page { get; set; }

        public int Count { get; set; }

        public int SignalsPerPage { get; set; }

        public bool IsCreateSignalContext { get; set; }

        public bool HasPreviousPage => this.Page > 1;

        public int PreviousPageNumber => this.Page - 1;

        public int PagesCount => (int)Math.Ceiling((double)this.Count / this.SignalsPerPage);

        public bool HasNextPage => this.Page < this.PagesCount;

        public int NextPageNumber => this.Page + 1;
    }
}
