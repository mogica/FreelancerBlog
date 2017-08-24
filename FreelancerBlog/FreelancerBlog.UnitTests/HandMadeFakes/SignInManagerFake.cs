﻿using System.Security.Claims;
using System.Threading.Tasks;
using FreelancerBlog.Core.Domain;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace FreelancerBlog.UnitTests.HandMadeFakes
{
    public class SignInManagerFake : SignInManager<ApplicationUser>
    {
        private SignInResult _signInResult;
        private ExternalLoginInfo _externalLoginInfo;

        public SignInManagerFake(IHttpContextAccessor contextAccessor, SignInResult signInResult)
        : base(new UserManagerFake(isUserConfirmed: false),
              contextAccessor,
              new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>().Object,
              new Mock<IOptions<IdentityOptions>>().Object,
              new Mock<ILogger<SignInManager<ApplicationUser>>>().Object,
              new Mock<IAuthenticationSchemeProvider>().Object
            )
        {
            _signInResult = signInResult;
        }

        public SignInManagerFake(IHttpContextAccessor contextAccessor, SignInResult signInResult, ExternalLoginInfo externalLoginInfo, bool isSignIn = false)
        : base(new UserManagerFake(isUserConfirmed: false),
              contextAccessor,
              new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>().Object,
              new Mock<IOptions<IdentityOptions>>().Object,
              new Mock<ILogger<SignInManager<ApplicationUser>>>().Object,
            new Mock<IAuthenticationSchemeProvider>().Object)
        {
            _signInResult = signInResult;
            _externalLoginInfo = externalLoginInfo;
        }

        public override Task SignInAsync(ApplicationUser user, bool isPersistent, string authenticationMethod = null)
        {
            return Task.FromResult(0);
        }

        public override Task<SignInResult> PasswordSignInAsync(string userName, string password, bool isPersistent, bool lockoutOnFailure)
        {
            return Task.FromResult(_signInResult);
        }

        public override Task SignOutAsync()
        {
            return Task.FromResult(0);
        }

        public override Task<ExternalLoginInfo> GetExternalLoginInfoAsync(string expectedXsrf = null)
        {
            return Task.FromResult<ExternalLoginInfo>(_externalLoginInfo);
        }

        public override Task<SignInResult> ExternalLoginSignInAsync(string loginProvider, string providerKey, bool isPersistent)
        {
            return Task.FromResult(_signInResult);
        }

        public override bool IsSignedIn(ClaimsPrincipal principal)
        {
            return true;
        }
    }
}
