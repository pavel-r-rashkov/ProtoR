namespace ProtoR.Web.Controllers
{
    using System.Threading.Tasks;
    using AutoMapper;
    using MediatR;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using ProtoR.Application.Configuration;
    using ProtoR.Web.Infrastructure.Identity;
    using ProtoR.Web.Resources;
    using ProtoR.Web.Resources.ConfigurationResource;

    public class ConfigurationsController : BaseController
    {
        public ConfigurationsController(
            IMediator mediator,
            IMapper mapper)
            : base(mediator, mapper)
        {
        }

        /// <summary>
        /// Get configuration by ID.
        /// </summary>
        /// <returns>Configuration.</returns>
        /// <response code="200">Configuration with requested ID.</response>
        /// <response code="400">Configuration ID is invalid.</response>
        /// <response code="404">Configuration with the specified ID doesn't exist.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [HttpGet]
        [Route("{ConfigurationId}")]
        [PermissionClaim(Permission.ConfigurationRead)]
        public async Task<ActionResult<ResponseModel<ConfigurationReadModel>>> Get([FromRoute]GetByIdQuery query)
        {
            ConfigurationDto configurationDto = await this.Mediator.Send(query);
            var configurationResource = this.Mapper.Map<ConfigurationReadModel>(configurationDto);

            return this.Ok(new ResponseModel<ConfigurationReadModel>(configurationResource));
        }

        /// <summary>
        /// Update existing configuration.
        /// </summary>
        /// <returns>No content.</returns>
        /// <response code="204">Configuration updated successfully.</response>
        /// <response code="404">Configuration with the specified ID doesn't exist.</response>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [HttpPut]
        [Route("{ConfigurationId}")]
        [PermissionClaim(Permission.ConfigurationWrite)]
        public async Task<ActionResult> Get(ConfigurationWriteModel configuration)
        {
            var updateCommand = this.Mapper.Map<UpdateConfigurationCommand>(configuration);
            await this.Mediator.Send(updateCommand);

            return this.NoContent();
        }
    }
}
