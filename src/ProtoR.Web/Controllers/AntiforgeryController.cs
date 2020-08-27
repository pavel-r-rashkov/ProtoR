namespace ProtoR.Web.Controllers
{
    using Microsoft.AspNetCore.Antiforgery;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;

    [Route("api/[controller]")]
    [ApiController]
    public class AntiforgeryController : Controller
    {
        private readonly IAntiforgery antiforgery;
        private readonly AntiforgeryOptions antiforgeryOptions;

        public AntiforgeryController(
            IAntiforgery antiforgery,
            IOptions<AntiforgeryOptions> antiforgeryOptions)
        {
            this.antiforgery = antiforgery;
            this.antiforgeryOptions = antiforgeryOptions.Value;
        }

        /// <summary>
        /// Generate antiforgery cookie.
        /// </summary>
        /// <returns>Antiforgery cookie.</returns>
        /// <response code="204">No content.</response>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpGet]
        [IgnoreAntiforgeryToken]
        public IActionResult GenerateAntiForgeryTokens()
        {
            var tokens = this.antiforgery.GetAndStoreTokens(this.HttpContext);

            this.Response.Cookies.Append(
                this.antiforgeryOptions.Cookie.Name,
                tokens.RequestToken,
                new CookieOptions
                {
                    HttpOnly = false,
                });

            return this.NoContent();
        }
    }
}
