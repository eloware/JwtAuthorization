using System;

namespace JwtAuthorization {
    
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class JwtTokenProperty : Attribute {
        public string? Type { get; }

        public JwtTokenProperty(string type) {
            Type = type;
        }

        public JwtTokenProperty() {
            Type = null;
        }
    }
}