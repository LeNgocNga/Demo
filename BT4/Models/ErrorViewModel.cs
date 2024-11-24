namespace Demo.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

    }

    public class RegisterViewModel
    {
        public TKhachHang KhachHang { get; set; }
        public TUser User { get; set; }
        public string UsernameError { get; set; } = string.Empty;
    }
}
