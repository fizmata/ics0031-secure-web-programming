using Microsoft.AspNetCore.Identity;

namespace Domain
{
    public class Vigenere
    {
        public int Id { get; set; }
        
        public string Key { get; set; }
        public string PlainText { get; set; }
        public string CipherText { get; set; }

        public string UserId { get; set; }
        public IdentityUser User { get; set; }
    }
}