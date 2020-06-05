namespace ProtoR.Web.Controllers
{
    using System;
    using System.Globalization;
    using System.Threading.Tasks;
    using IdentityServer4.Events;
    using IdentityServer4.Models;
    using IdentityServer4.Services;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using ProtoR.Domain.UserAggregate;
    using ProtoR.Web.Infrastructure.Identity;

    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly IIdentityServerInteractionService interaction;
        private readonly SignInManager<User> signInManager;
        private readonly IEventService events;
        private readonly UserManager<User> userManager;

        public AccountController(
            IIdentityServerInteractionService interaction,
            SignInManager<User> signInManager,
            IEventService events,
            UserManager<User> userManager)
        {
            this.interaction = interaction;
            this.signInManager = signInManager;
            this.events = events;
            this.userManager = userManager;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            var loginModel = new LoginViewModel
            {
                ReturnUrl = returnUrl,
            };

            return this.View(loginModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginInputModel model)
        {
            // check if we are in the context of an authorization request
            var context = await this.interaction.GetAuthorizationContextAsync(model.ReturnUrl);

            // the user clicked the "cancel" button
            if (model.Button != "login")
            {
                if (context != null)
                {
                    // if the user cancels, send a result back into IdentityServer as if they
                    // denied the consent (even if this client does not require consent).
                    // this will send back an access denied OIDC error response to the client.
                    await this.interaction.GrantConsentAsync(context, ConsentResponse.Denied);

                    return this.Redirect(model.ReturnUrl);
                }
                else
                {
                    // since we don't have a valid context, then we just go back to the home page
                    return this.Redirect("~/");
                }
            }

            if (this.ModelState.IsValid)
            {
                var result = await this.signInManager.PasswordSignInAsync(model.Username, model.Password, false, false);

                if (result.Succeeded)
                {
                    var user = await this.userManager.FindByNameAsync(model.Username);
                    await this.events.RaiseAsync(new UserLoginSuccessEvent(
                        user.UserName,
                        user.Id.ToString(CultureInfo.InvariantCulture),
                        user.UserName,
                        clientId: context?.ClientId));

                    if (context != null)
                    {
                        return this.Redirect(model.ReturnUrl);
                    }

                    if (this.Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return this.Redirect(model.ReturnUrl);
                    }
                    else if (string.IsNullOrEmpty(model.ReturnUrl))
                    {
                        return this.Redirect("~/");
                    }
                    else
                    {
                        // user might have clicked on a malicious link - should be logged
                        throw new Exception("invalid return URL");
                    }
                }

                await this.events.RaiseAsync(new UserLoginFailureEvent(model.Username, "invalid credentials", clientId: context?.ClientId));
                this.ModelState.AddModelError(string.Empty, "Invalid username or password");
            }

            // something went wrong, show form with error
            var loginModel = new LoginViewModel
            {
                ReturnUrl = model.ReturnUrl,
            };

            return this.View(loginModel);
        }
    }
}
