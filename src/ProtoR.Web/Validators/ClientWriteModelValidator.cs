namespace ProtoR.Web.Validators
{
    using System.Linq;
    using System.Text.RegularExpressions;
    using FluentValidation;
    using ProtoR.Web.Resources.ClientResource;

    public class ClientWriteModelValidator : AbstractValidator<ClientWriteModel>
    {
        public ClientWriteModelValidator()
        {
            this.RuleFor(r => r.ClientId)
                .MaximumLength(500)
                .NotNull()
                .NotEmpty();

            this.RuleFor(r => r.ClientName)
                .MaximumLength(500)
                .NotNull()
                .NotEmpty();

            this.RuleFor(r => r.Secret)
                .MaximumLength(500)
                .MinimumLength(14);

            this.RuleFor(r => r.GrantTypes)
                .NotEmpty()
                .NotNull()
                .ForEach(g => g.Matches(@"^(client_credentials|authorization_code)$", RegexOptions.IgnoreCase))
                .WithMessage("Only \"client_credentials\" and \"authorization_code\" are supported");

            this.When(c => c.GrantTypes.Contains("client_credentials"), () =>
            {
                this.RuleFor(r => r.Secret)
                    .NotNull()
                    .NotEmpty();
            });

            this.RuleFor(r => r.RedirectUris)
                .ForEach(g => g.SetValidator(new UriValidator()));

            this.RuleFor(r => r.PostLogoutRedirectUris)
                .ForEach(g => g.SetValidator(new UriValidator()));

            this.RuleFor(r => r.AllowedCorsOrigins)
                .ForEach(g => g.SetValidator(new UriValidator()));

            this.When(c => c.GrantTypes.Contains("authorization_code"), () =>
            {
                this.RuleFor(r => r.RedirectUris)
                    .NotEmpty()
                    .NotNull();

                this.RuleFor(r => r.PostLogoutRedirectUris)
                    .NotEmpty()
                    .NotNull();

                this.RuleFor(r => r.AllowedCorsOrigins)
                    .NotEmpty()
                    .NotNull();
            });
        }
    }
}
