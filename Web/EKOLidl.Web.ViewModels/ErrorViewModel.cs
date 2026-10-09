namespace EKOLidl.Web.ViewModels
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        /// <summary>
        /// Gets or sets the HTTP status code that led to the error page, when there is one.
        /// </summary>
        public int? StatusCode { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(this.RequestId);

        public bool IsNotFound => this.StatusCode == 404;
    }
}
