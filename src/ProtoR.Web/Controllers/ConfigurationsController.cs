namespace ProtoR.Web.Controllers
{
    using System.Threading.Tasks;
    using AutoMapper;
    using MediatR;
    using Microsoft.AspNetCore.Mvc;
    using ProtoR.Application.Configuration;
    using ProtoR.Web.Resources.ConfigurationResource;

    public class ConfigurationsController : BaseController
    {
        public ConfigurationsController(
            IMediator mediator,
            IMapper mapper)
            : base(mediator, mapper)
        {
        }

        [HttpGet]
        [Route("{ConfigurationId}")]
        public async Task<ActionResult<ConfigurationReadModel>> Get([FromRoute]GetByIdQuery query)
        {
            ConfigurationDto configurationDto = await this.Mediator.Send(query);
            var configurationResource = this.Mapper.Map<ConfigurationReadModel>(configurationDto);

            return this.Ok(configurationResource);
        }

        [HttpPut]
        [Route("{ConfigurationId}")]
        public async Task<ActionResult> Get(ConfigurationWriteModel configuration)
        {
            var updateCommand = this.Mapper.Map<UpdateConfigurationCommand>(configuration);
            await this.Mediator.Send(updateCommand);

            return this.Ok();
        }
    }
}
