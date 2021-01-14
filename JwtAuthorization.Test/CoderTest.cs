using System;
using System.Threading;
using FluentAssertions;
using JwtAuthorization.Test.Models;
using NUnit.Framework;

namespace JwtAuthorization.Test {
    [TestFixture]
    public class CoderTest {

        private ILogonService _logonService;

        [SetUp]
        public void Setup() {
            _logonService = new LogonService("your-256-bit-secret", TimeSpan.FromMinutes(1));
        }

        [Test]
        public void EncodeToken_Test() {
            var user = new User {
                Name = "Klaus Fischer",
                Phone = "0800 testMyApp",
                Role = "Developer",
                Username = "admin",
                MailAddress = "admin@admins.io",
                NicName = "kfman"
            };
            var token = _logonService.EncodeToken(user);
            Console.WriteLine(token);
            token.Should().NotBeEmpty();

            var decoded = _logonService.DecodeToken<User>(token);
            decoded.Name.Should().Be("Klaus Fischer");
            decoded.Phone.Should().Be("0800 testMyApp");
            decoded.Role.Should().Be("Developer");
            decoded.Username.Should().Be("admin");
            decoded.MailAddress.Should().Be("admin@admins.io");
            decoded.NicName.Should().Be("kfman");
            decoded.NotValidBefore.Should().BeOnOrBefore(DateTime.Now);
            decoded.ExpirationTime.Should().BeOnOrAfter(DateTime.Now);
            decoded.IsCorrupt.Should().BeFalse();
            decoded.Valid.Should().BeTrue($"{decoded.NotValidBefore} <= {DateTime.Now} <= {decoded.ExpirationTime}");
            
        }

        [Test]
        public void DecodeTokenWithResult() {
            var token =
                "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJVc2VyIjoiYWRtaW4iLCJOYW1lIjoiS2xhdXMgRmlzY2hlciIsIlJvbGUiOiJEZXZlbG9wZXIiLCJOaWNOYW1lIjoia2ZtYW4iLCJQaG9uZSI6IjA4MDAgdGVzdE15QXBwIiwibWFpbCI6ImFkbWluQGFkbWlucy5pbyIsIm5iZiI6MTYxMDYyMDI3MywiZXhwIjoxNjEwNjIwMzMzLCJpYXQiOjE2MTA2MjAyNzN9.KsX4Bfr3Y0IkIDyuYNs0B2YDjP55Dva5lq9ZKp_NPV4";

            var user = _logonService.DecodeToken<User>(token);

            user.Name.Should().Be("Klaus Fischer");
            user.Phone.Should().Be("0800 testMyApp");
            user.Role.Should().Be("Developer");
            user.Username.Should().Be("admin");
            user.MailAddress.Should().Be("admin@admins.io");
            user.NicName.Should().Be("kfman");
            user.IsCorrupt.Should().BeFalse();
            user.Valid.Should().BeFalse();
        }

        [Test]
        public void DecodeCorruptToken() {
            var token =
                "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJVc2VyIjoiYWRtaW4iLCJOYW1lIjoiS2xhdXMgRmlzY2hlciIsIlJvbGUiOiJEZXZlbG9wZXIiLCJOaWNOYW1lIjoia2ZtYW4iLCJQaG9uZSI6IjA4MDAgdGVzdE15QXBwIiwibWFpbCI6ImFkbWluQGFkbWlucy5pbyIsIm5iZiI6MTYxMDYyMDI3MywiZXhwIjoxNjEwNjIwMzMzLCJpYXQiOjE2MTA2MjAyNzN9.KsX4Bfr3Y0IkIDyuYOs0B2YDjP58Dva5lq9ZKp_NPV4";

            var user = _logonService.DecodeToken<User>(token);

            user.Name.Should().Be("Klaus Fischer");
            user.Phone.Should().Be("0800 testMyApp");
            user.Role.Should().Be("Developer");
            user.Username.Should().Be("admin");
            user.MailAddress.Should().Be("admin@admins.io");
            user.NicName.Should().Be("kfman");
            user.IsCorrupt.Should().BeTrue();
            user.Valid.Should().BeFalse();
        }

    }
}