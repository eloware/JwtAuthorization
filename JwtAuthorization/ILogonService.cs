namespace Authorization {
    public interface ILogonService {
        public TUser? DecodeToken<TUser>(string token) where TUser : TokenData, new();
        public string EncodeToken<TUser>(TUser user);
    }
}