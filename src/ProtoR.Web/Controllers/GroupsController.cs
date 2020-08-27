namespace ProtoR.Web.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using MediatR;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using ProtoR.Application.Common;
    using ProtoR.Application.Configuration;
    using ProtoR.Application.Group;
    using ProtoR.Application.Schema;
    using ProtoR.Infrastructure.DataAccess;
    using ProtoR.Web.Infrastructure.Identity;
    using ProtoR.Web.Resources;
    using ProtoR.Web.Resources.ConfigurationResource;
    using ProtoR.Web.Resources.GroupResource;
    using ProtoR.Web.Resources.SchemaResource;

    public class GroupsController : BaseController
    {
        private readonly IClusterSingletonService clusterSingletonService;

        public GroupsController(
            IMediator mediator,
            IMapper mapper,
            IClusterSingletonService clusterSingletonService)
            : base(mediator, mapper)
        {
            this.clusterSingletonService = clusterSingletonService;
        }

        /// <summary>
        /// Get all groups.
        /// </summary>
        /// <returns>Groups.</returns>
        /// <response code="200">Group list.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet]
        [PermissionClaim(Permission.GroupRead)]
        public async Task<ActionResult<ResponseModel<PagedResult<GroupReadModel>>>> Get([FromQuery]GetGroupsQuery query)
        {
            var groupsDto = await this.Mediator.Send(query);
            var groupResources = this.Mapper.Map<PagedResult<GroupReadModel>>(groupsDto);

            return this.Ok(new ResponseModel<PagedResult<GroupReadModel>>(groupResources));
        }

        /// <summary>
        /// Create new group.
        /// </summary>
        /// <returns>No content.</returns>
        /// <response code="201">Group created successfully.</response>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [HttpPost]
        [PermissionClaim(Permission.GroupWrite)]
        public async Task<ActionResult> Post(GroupWriteModel group)
        {
            var command = this.Mapper.Map<CreateGroupCommand>(group);
            var created = await this.Mediator.Send(command);

            return this.CreatedAtAction(nameof(this.GetByName), new { group.Name }, null);
        }

        /// <summary>
        /// Get group by name.
        /// </summary>
        /// <returns>Group.</returns>
        /// <response code="200">Group with requested name.</response>
        /// <response code="404">Group with specified name doesn't exist.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [HttpGet]
        [Route("{Name}")]
        [PermissionClaim(Permission.GroupRead)]
        public async Task<ActionResult<ResponseModel<GroupReadModel>>> GetByName([FromRoute]GetByNameQuery query)
        {
            GroupDto groupDto = await this.Mediator.Send(query);
            var groupResource = this.Mapper.Map<GroupReadModel>(groupDto);

            return this.Ok(new ResponseModel<GroupReadModel>(groupResource));
        }

        /// <summary>
        /// Delete group.
        /// </summary>
        /// <returns>No content.</returns>
        /// <response code="204">Group deleted successfully.</response>
        /// <response code="404">Group with specified name doesn't exist.</response>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [HttpDelete]
        [Route("{Name}")]
        [PermissionClaim(Permission.GroupWrite)]
        public async Task<ActionResult> Delete(DeleteGroupCommand command)
        {
            await this.Mediator.Send(command);

            return this.NoContent();
        }

        /// <summary>
        /// Get group schemas.
        /// </summary>
        /// <returns>Group schemas.</returns>
        /// <response code="200">Schemas list.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet]
        [Route("{Name}/Schemas")]
        [PermissionClaim(Permission.SchemaRead)]
        public async Task<ActionResult<ResponseModel<PagedResult<SchemaReadModel>>>> GetSchemas([FromRoute]GetGroupSchemasQuery query)
        {
            var schemasDto = await this.Mediator.Send(query);
            var schemaResources = this.Mapper.Map<PagedResult<SchemaReadModel>>(schemasDto);

            return this.Ok(new ResponseModel<PagedResult<SchemaReadModel>>(schemaResources));
        }

        /// <summary>
        /// Get schema by version.
        /// </summary>
        /// <returns>Schema.</returns>
        /// <response code="200">Schema with requested version.</response>
        /// <response code="404">Schema with specified version doesn't exist.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [HttpGet]
        [Route("{Name}/Schemas/{Version}")]
        [PermissionClaim(Permission.SchemaRead)]
        public async Task<ActionResult<ResponseModel<SchemaReadModel>>> GetSchema([FromRoute]GetByVersionQuery query)
        {
            SchemaDto schemaDto = await this.Mediator.Send(query);
            var schemaResource = this.Mapper.Map<SchemaReadModel>(schemaDto);

            return this.Ok(new ResponseModel<SchemaReadModel>(schemaResource));
        }

        /// <summary>
        /// Add new schema to a group.
        /// </summary>
        /// <returns>No content.</returns>
        /// <response code="201">Schema was added successfully.</response>
        /// <response code="404">Group with specified name doesn't exist.</response>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [HttpPost]
        [Route("{Name}/Schemas")]
        [PermissionClaim(Permission.SchemaWrite)]
        public async Task<ActionResult> PostSchema(SchemaWriteModel schema)
        {
            var command = this.Mapper.Map<CreateSchemaCommand>(schema);
            var commandResult = await this.clusterSingletonService.AddSchema(command);

            if (commandResult.RuleViolations.Any(v => v.IsFatal))
            {
                return this.BadRequest(commandResult.RuleViolations);
            }

            return this.CreatedAtAction(
                nameof(this.GetSchema),
                new
                {
                    schema.Name,
                    Version = commandResult.NewVersion,
                },
                commandResult.RuleViolations);
        }

        /// <summary>
        /// Test schema compatibility.
        /// </summary>
        /// <returns>Validation result.</returns>
        /// <response code="200">Schema validation result.</response>
        /// <response code="404">Group with specified name doesn't exist.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [HttpPost]
        [Route("{Name}/SchemaTest")]
        [PermissionClaim(Permission.SchemaRead)]
        public async Task<ActionResult<ResponseModel<IEnumerable<RuleViolationDto>>>> ValidationCheck(SchemaWriteModel schema)
        {
            var command = this.Mapper.Map<ValidateSchemaCommand>(schema);
            var commandResult = await this.Mediator.Send(command);

            return this.Ok(new ResponseModel<IEnumerable<RuleViolationDto>>(commandResult.RuleViolations));
        }

        /// <summary>
        /// Get configuration associated with a group.
        /// </summary>
        /// <returns>Configuration.</returns>
        /// <response code="200">Configuration associated with requested group.</response>
        /// <response code="404">Group with specified name doesn't exist.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [HttpGet]
        [Route("{Name}/Configuration")]
        [PermissionClaim(Permission.ConfigurationRead)]
        public async Task<ActionResult<ResponseModel<ConfigurationReadModel>>> GetConfiguration([FromRoute]GetByGroupNameQuery query)
        {
            ConfigurationDto configurationDto = await this.Mediator.Send(query);
            var configurationResource = this.Mapper.Map<ConfigurationReadModel>(configurationDto);

            return this.Ok(new ResponseModel<ConfigurationReadModel>(configurationResource));
        }
    }
}
