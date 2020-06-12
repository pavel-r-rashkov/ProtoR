namespace ProtoR.Web.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using MediatR;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
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
        /// <response code="401">User or client is not authenticated.</response>
        /// <response code="403">
        /// "GroupRead" permission is missing or user/client doesn't have access to the category associated with this group.
        /// </response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status403Forbidden)]
        [HttpGet]
        [PermissionClaim(Permission.GroupRead)]
        public async Task<ActionResult<ResponseModel<IEnumerable<GroupReadModel>>>> Get()
        {
            IEnumerable<GroupDto> groupsDto = await this.Mediator.Send(new GetGroupsQuery());
            var groupResources = this.Mapper.Map<IEnumerable<GroupReadModel>>(groupsDto);

            return this.Ok(new ResponseModel<IEnumerable<GroupReadModel>>(groupResources));
        }

        /// <summary>
        /// Create new group.
        /// </summary>
        /// <returns>No content.</returns>
        /// <response code="201">Group created successfully.</response>
        /// <response code="400">Group data is invalid or a group with the same name already exists.</response>
        /// <response code="401">User or client is not authenticated.</response>
        /// <response code="403">
        /// "GroupWrite" permission is missing or user/client doesn't have access to the category associated with this group.
        /// </response>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status403Forbidden)]
        [HttpPost]
        [PermissionClaim(Permission.GroupWrite)]
        public async Task<ActionResult> Post(GroupWriteModel group)
        {
            var command = this.Mapper.Map<CreateGroupCommand>(group);
            var created = await this.Mediator.Send(command);

            return this.CreatedAtAction(nameof(this.GetByName), new { group.GroupName }, null);
        }

        /// <summary>
        /// Update existing configuration.
        /// </summary>
        /// <returns>No content.</returns>
        /// <response code="204">Group updated successfully.</response>
        /// <response code="400">Group data is invalid.</response>
        /// <response code="401">User or client is not authenticated.</response>
        /// <response code="403">
        /// "GroupWrite" permission is missing or user/client doesn't have access to the category associated with this group.
        /// </response>
        /// <response code="404">Group with the specified ID doesn't exist.</response>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [HttpPut]
        [Route("{GroupName}")]
        [PermissionClaim(Permission.GroupWrite)]
        public async Task<ActionResult> Put(GroupPutModel group)
        {
            var command = this.Mapper.Map<UpdateGroupCommand>(group);
            await this.Mediator.Send(command);

            return this.NoContent();
        }

        /// <summary>
        /// Get group by name.
        /// </summary>
        /// <returns>Group.</returns>
        /// <response code="200">Group with requested name.</response>
        /// <response code="401">User or client is not authenticated.</response>
        /// <response code="403">
        /// "GroupRead" permission is missing or user/client doesn't have access to the category associated with this group.
        /// </response>
        /// <response code="404">Group with specified name doesn't exist.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [HttpGet]
        [Route("{GroupName}")]
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
        /// <response code="401">User or client is not authenticated.</response>
        /// <response code="403">
        /// "GroupWrite" permission is missing or user/client doesn't have access to the category associated with this group.
        /// </response>
        /// <response code="404">Group with specified name doesn't exist.</response>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [HttpDelete]
        [Route("{GroupName}")]
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
        /// <response code="401">User or client is not authenticated.</response>
        /// <response code="403">
        /// "SchemaRead" permission is missing or user/client doesn't have access to the category associated with this group.
        /// </response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status403Forbidden)]
        [HttpGet]
        [Route("{GroupName}/Schemas")]
        [PermissionClaim(Permission.SchemaRead)]
        public async Task<ActionResult<ResponseModel<IEnumerable<SchemaReadModel>>>> GetSchemas([FromRoute]GetGroupSchemasQuery query)
        {
            IEnumerable<SchemaDto> schemasDto = await this.Mediator.Send(query);
            var schemaResources = this.Mapper.Map<IEnumerable<SchemaReadModel>>(schemasDto);

            return this.Ok(new ResponseModel<IEnumerable<SchemaReadModel>>(schemaResources));
        }

        /// <summary>
        /// Get schema by version.
        /// </summary>
        /// <returns>Schema.</returns>
        /// <response code="200">Schema with requested version.</response>
        /// <response code="400">Version is invalid.</response>
        /// <response code="401">User or client is not authenticated.</response>
        /// <response code="403">
        /// "SchemaRead" permission is missing or user/client doesn't have access to the category associated with this group.
        /// </response>
        /// <response code="404">Schema with specified version doesn't exist.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [HttpGet]
        [Route("{GroupName}/Schemas/{Version}")]
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
        /// <response code="400">Schema is invalid or doesn't pass group rule restrictions.</response>
        /// <response code="401">User or client is not authenticated.</response>
        /// <response code="403">
        /// "SchemaWrite" permission is missing or user/client doesn't have access to the category associated with this group.
        /// </response>
        /// <response code="404">Group with specified name doesn't exist.</response>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [HttpPost]
        [Route("{GroupName}/Schemas")]
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
                    schema.GroupName,
                    Version = commandResult.NewVersion,
                },
                commandResult.RuleViolations);
        }

        /// <summary>
        /// Test schema compatibility.
        /// </summary>
        /// <returns>Validation result.</returns>
        /// <response code="200">Schema validation result.</response>
        /// <response code="400">Schema is invalid.</response>
        /// <response code="401">User or client is not authenticated.</response>
        /// <response code="403">
        /// "SchemaRead" permission is missing or user/client doesn't have access to the category associated with this group.
        /// </response>
        /// <response code="404">Group with specified name doesn't exist.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [HttpPost]
        [Route("{GroupName}/SchemaTest")]
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
        /// <response code="401">User or client is not authenticated.</response>
        /// <response code="403">
        /// "ConfigurationRead" permission is missing or user/client doesn't have access to the category associated with this configuration.
        /// </response>
        /// <response code="404">Group with specified name doesn't exist.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [HttpGet]
        [Route("{GroupName}/Configuration")]
        [PermissionClaim(Permission.ConfigurationRead)]
        public async Task<ActionResult<ResponseModel<ConfigurationReadModel>>> GetConfiguration([FromRoute]GetByGroupNameQuery query)
        {
            ConfigurationDto configurationDto = await this.Mediator.Send(query);
            var configurationResource = this.Mapper.Map<ConfigurationReadModel>(configurationDto);

            return this.Ok(new ResponseModel<ConfigurationReadModel>(configurationResource));
        }
    }
}
