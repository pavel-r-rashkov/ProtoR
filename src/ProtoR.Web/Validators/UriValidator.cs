namespace ProtoR.Web.Validators
{
    using System;
    using FluentValidation;

    public class UriValidator : AbstractValidator<string>
    {
        public UriValidator()
        {
            this.When(u => u != null, () =>
            {
                this.RuleFor(u => u)
                    .Must(u =>
                    {
                        if (Uri.TryCreate(u, UriKind.Absolute, out var validatedUri))
                        {
                            return validatedUri.Scheme == Uri.UriSchemeHttp
                                || validatedUri.Scheme == Uri.UriSchemeHttps;
                        }

                        return false;
                    });
            });
        }
    }
}
