using Microsoft.AspNetCore.Identity;

namespace Domain
{
    public class Rsa
    {
        public int Id { get; set; }
        
        public int? N { get; set; }
        public int? D { get; set; }
        public int? E { get; set; }
        public string PlainText { get; set; }
        public string CipherText { get; set; }
        
        public string UserId { get; set; }
        public IdentityUser User { get; set; }
    }
}