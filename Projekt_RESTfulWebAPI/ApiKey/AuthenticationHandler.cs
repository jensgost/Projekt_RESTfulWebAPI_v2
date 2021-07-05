using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Projekt_RESTfulWebAPI.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Projekt_RESTfulWebAPI.ApiKey
{
    public class AuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly OurDbContext _context;
        public AuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            OurDbContext context
            )
            : base(options, logger, encoder, clock)
        {
            _context = context;
        }

        public async Task<IdentityUser> GetUserByTokenAsync(string apiToken)
        {
            var user = await _context.ApiTokens.Where(t => t.Key.ToString() == apiToken)
                .Select(t => t.User)
                .FirstOrDefaultAsync();
            return user;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            string apiToken = Request.Query["apikey"];

            if (apiToken == null)
                return AuthenticateResult.Fail("No token");

            var user = await GetUserByTokenAsync(apiToken);

            if (user == null)
            {
                return AuthenticateResult.Fail("Invalid token");
            }

            var claims = new List<Claim> {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
            };

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
    }
}
