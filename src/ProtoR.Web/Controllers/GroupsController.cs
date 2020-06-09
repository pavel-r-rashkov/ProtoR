namespace ProtoR.Web.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using MediatR;
    using Microsoft.AspNetCore.Mvc;
    using ProtoR.Application.Configuration;
    using ProtoR.Application.Group;
    using ProtoR.Application.Schema;
    using ProtoR.Infrastructure.DataAccess;
    using ProtoR.Web.Infrastructure.Identity;
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

        [HttpGet]
        [PermissionClaim(Permission.GroupRead)]
        public async Task<ActionResult<IEnumerable<GroupReadModel>>> Get()
        {
            IEnumerable<GroupDto> groupsDto = await this.Mediator.Send(new GetGroupsQuery());
            var groupResources = this.Mapper.Map<IEnumerable<GroupReadModel>>(groupsDto);

            return this.Ok(groupResources);
        }

        [HttpPost]
        [PermissionClaim(Permission.GroupWrite)]
        public async Task<ActionResult> Post(GroupWriteModel group)
        {
            var command = this.Mapper.Map<CreateGroupCommand>(group);
            var created = await this.Mediator.Send(command);

            if (!created)
            {
                return this.BadRequest($"Group with name {group.GroupName} already exists");
            }

            return this.CreatedAtAction(nameof(this.GetByName), new { group.GroupName }, null);
        }

        [HttpPut]
        [Route("{GroupName}")]
        [PermissionClaim(Permission.GroupWrite)]
        public async Task<ActionResult> Put(GroupPutModel group)
        {
            var command = this.Mapper.Map<UpdateGroupCommand>(group);
            await this.Mediator.Send(command);

            return this.NoContent();
        }

        [HttpGet]
        [Route("{GroupName}")]
        [PermissionClaim(Permission.GroupRead)]
        public async Task<ActionResult<GroupReadModel>> GetByName([FromRoute]GetByNameQuery query)
        {
            GroupDto groupDto = await this.Mediator.Send(query);
            var groupResource = this.Mapper.Map<GroupReadModel>(groupDto);

            return this.Ok(groupResource);
        }

        [HttpDelete]
        [Route("{GroupName}")]
        [PermissionClaim(Permission.GroupWrite)]
        public async Task<ActionResult> Delete(DeleteGroupCommand command)
        {
            await this.Mediator.Send(command);

            return this.NoContent();
        }

        [HttpGet]
        [Route("{GroupName}/Schemas")]
        [PermissionClaim(Permission.SchemaRead)]
        public async Task<ActionResult<IEnumerable<SchemaReadModel>>> GetSchemas([FromRoute]GetGroupSchemasQuery query)
        {
            IEnumerable<SchemaDto> schemasDto = await this.Mediator.Send(query);
            var schemaResources = this.Mapper.Map<IEnumerable<SchemaReadModel>>(schemasDto);

            return this.Ok(schemaResources);
        }

        [HttpGet]
        [Route("{GroupName}/Schemas/{Version}")]
        [PermissionClaim(Permission.SchemaRead)]
        public async Task<ActionResult<SchemaReadModel>> GetSchema([FromRoute]GetByVersionQuery query)
        {
            SchemaDto schemaDto = await this.Mediator.Send(query);
            var schemaResource = this.Mapper.Map<SchemaReadModel>(schemaDto);

            return this.Ok(schemaResource);
        }

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

        [HttpPost]
        [Route("{GroupName}/SchemaTest")]
        [PermissionClaim(Permission.SchemaRead)]
        public async Task<ActionResult> ValidationCheck(SchemaWriteModel schema)
        {
            var command = this.Mapper.Map<ValidateSchemaCommand>(schema);
            var commandResult = await this.Mediator.Send(command);

            return this.Ok(commandResult.RuleViolations);
        }

        [HttpGet]
        [Route("{GroupName}/Configuration")]
        [PermissionClaim(Permission.ConfigurationRead)]
        public async Task<ActionResult<ConfigurationReadModel>> GetConfiguration([FromRoute]GetByGroupNameQuery query)
        {
            ConfigurationDto configurationDto = await this.Mediator.Send(query);
            var configurationResource = this.Mapper.Map<ConfigurationReadModel>(configurationDto);

            return this.Ok(configurationResource);
        }
    }
}
