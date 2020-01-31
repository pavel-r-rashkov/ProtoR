namespace Web.Features.Test
{
    using System.Threading.Tasks;
    using AutoMapper;
    using MediatR;
    using Microsoft.AspNetCore.Mvc;
    using Serilog;

    /// <summary>
    /// Verifies that the swagger documentation generator works as expected.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IMediator mediator;
        private readonly IMapper mapper;

        public TestController(
            IMediator mediator,
            IMapper mapper)
        {
            this.mediator = mediator;
            this.mapper = mapper;
        }

        /// <summary>
        /// Retrieves test data.
        /// </summary>
        /// <returns>The test data.</returns>
        [HttpGet]
        public async Task<ActionResult<TestResponse>> Get([FromQuery]TestRequest request)
        {
            Log.Logger.Warning("=============== Some warining");
            return await this.mediator.Send(request);
        }
    }
}
