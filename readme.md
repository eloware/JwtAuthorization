# JwtAuthorization

## Installation

Add the following lines to your **ConfigureServices** methode
````c#
services.AddJwtAuthentication(Environment.GetEnvironmentVariable("JwtSecret") ?? "JwtSecurityTokenTemplate");
````

And make sure to add 
````c#
app.UseAuthorization();
````

below the **UseAuthentication()** in your Configure methode

## Usage

### Token
Your JwtClaimOwner e.g. User must inherit from TokenData to store validation time stamps.

To create a JWT string call the
````c#
public string EncodeToken<TUser>(TUser user)
````
function of the LogonService

### User class

As an example of a user class take a look at this code

```c#
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
```

If the JwtTokenProperty attribute is created without a constructor parameter, the property name is used as Claim.Type