namespace ProtoR.Web.Controllers
{
    using AutoMapper;
    using MediatR;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize("ProtoRClientPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        public BaseController(
            IMediator mediator,
            IMapper mapper)
        {
            this.Mediator = mediator;
            this.Mapper = mapper;
        }

        protected IMediator Mediator { get; private set; }

        protected IMapper Mapper { get; private set; }
    }
}