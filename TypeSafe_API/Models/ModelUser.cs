namespace BilakLk_API.Models
{
    public class ModelUser
    {
        public int? Id { get; set; }
        public string? AuthToken { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? ContactNumber { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? UserStatus { get; set; }

    }
}
