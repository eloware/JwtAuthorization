using System;

namespace Authorization {
    public class TokenData {
        public bool IsCorrupt { get; set; } = true;
        public DateTime? NotValidBefore { get; set; }
        public DateTime? ExpirationTime { get; set; }
        public DateTime? IssuedAt { get; set; }
        public bool Valid => !IsCorrupt && (DateTime.Now >= NotValidBefore) && (DateTime.Now <= ExpirationTime);
    }
}
