using Authorization;

namespace JwtAuthorization.Test.Models {

    public class User: TokenData {
        [JwtTokenProperty("User")]
        public string Username { get; set; }
        [JwtTokenProperty]
        public string Name { get; set; }

        [JwtTokenProperty]
        public string Role { get; set; }
        [JwtTokenProperty]
        public string NicName { get; set; }
        [JwtTokenProperty]
        public string Phone { get; set; }
        [JwtTokenProperty("mail")]
        public string MailAddress { get; set; }
        
    }
}
