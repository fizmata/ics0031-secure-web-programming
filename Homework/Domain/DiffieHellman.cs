using Microsoft.AspNetCore.Identity;

namespace Domain
{
    public class DiffieHellman
    {
        public int Id { get; set; }

        public int PrivateKeyA { get; set; }
        public int? PrivateKeyB { get; set; }
        public int? P { get; set; }
        public int? G { get; set; }
        public int? PublicKeyA { get; set; }
        public int? PublicKeyB { get; set; }
        public int? SymmetricKey { get; set; }

        public string UserId { get; set; }
        public IdentityUser User { get; set; }
    }
}