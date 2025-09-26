using FluentValidation;
using UrlShortener.Api.Application.DTOs;

namespace UrlShortener.Api.Application.Validators;

public class CreateShortUrlRequestValidator : AbstractValidator<CreateShortUrlRequest>
{
    public CreateShortUrlRequestValidator()
    {
        RuleFor(x => x.OriginalUrl)
            .NotEmpty().WithMessage("OriginalUrl is required.")
            .Must(BeValidHttpUrl).WithMessage("OriginalUrl must be a valid HTTP/HTTPS URL.");
    }

    private bool BeValidHttpUrl(string url)
        => Uri.TryCreate(url, UriKind.Absolute, out var uri) &&
           (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
}
