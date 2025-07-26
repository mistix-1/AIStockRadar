namespace AIStockRadar.Models
{
    public class User
    {
        public int UserId { get; set; }  // Primary Key
        public string Email { get; set; }
        public string PasswordHash { get; set; }
    }
}
