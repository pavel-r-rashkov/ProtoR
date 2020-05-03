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
    using ProtoR.Web.Resources.ConfigurationResource;
    using ProtoR.Web.Resources.GroupResource;
    using ProtoR.Web.Resources.SchemaResource;

    public class GroupsController : BaseController
    {
        public GroupsController(
            IMediator mediator,
            IMapper mapper)
            : base(mediator, mapper)
        {
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GroupReadModel>>> Get()
        {
            IEnumerable<GroupDto> groupsDto = await this.Mediator.Send(new GetGroupsQuery());
            var groupResources = this.Mapper.Map<IEnumerable<GroupReadModel>>(groupsDto);

            return this.Ok(groupResources);
        }

        [HttpPost]
        public async Task<ActionResult> Post(GroupWriteModel group)
        {
            var command = this.Mapper.Map<CreateGroupCommand>(group);
            var created = await this.Mediator.Send(command);

            if (!created)
            {
                return this.BadRequest($"Group with name {group.Name} already exists");
            }

            return this.CreatedAtAction(nameof(this.GetByName), new { GroupName = group.Name }, null);
        }

        [HttpGet]
        [Route("{GroupName}")]
        public async Task<ActionResult<GroupReadModel>> GetByName([FromRoute]GetByNameQuery query)
        {
            GroupDto groupDto = await this.Mediator.Send(query);
            var groupResource = this.Mapper.Map<GroupReadModel>(groupDto);

            return this.Ok(groupResource);
        }

        [HttpDelete]
        [Route("{GroupName}")]
        public async Task<ActionResult> Delete(DeleteGroupCommand command)
        {
            await this.Mediator.Send(command);

            return this.NoContent();
        }

        [HttpGet]
        [Route("{GroupName}/Schemas")]
        public async Task<ActionResult<IEnumerable<SchemaReadModel>>> GetSchemas([FromRoute]GetGroupSchemasQuery query)
        {
            IEnumerable<SchemaDto> schemasDto = await this.Mediator.Send(query);
            var schemaResources = this.Mapper.Map<IEnumerable<SchemaReadModel>>(schemasDto);

            return this.Ok(schemaResources);
        }

        [HttpGet]
        [Route("{GroupName}/Schemas/{Version}")]
        public async Task<ActionResult<SchemaReadModel>> GetSchema([FromRoute]GetByVersionQuery query)
        {
            SchemaDto schemaDto = await this.Mediator.Send(query);
            var schemaResource = this.Mapper.Map<SchemaReadModel>(schemaDto);

            return this.Ok(schemaResource);
        }

        [HttpPost]
        [Route("{GroupName}/Schemas")]
        public async Task<ActionResult> PostSchema(SchemaWriteModel schema)
        {
            var command = this.Mapper.Map<CreateSchemaCommand>(schema);
            var commandResult = await this.Mediator.Send(command);

            if (!string.IsNullOrWhiteSpace(commandResult.SchemaParseErrors))
            {
                return this.BadRequest(commandResult.SchemaParseErrors);
            }

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

        [HttpGet]
        [Route("{GroupName}/Configuration")]
        public async Task<ActionResult<ConfigurationReadModel>> GetConfiguration([FromRoute]GetByGroupNameQuery query)
        {
            ConfigurationDto configurationDto = await this.Mediator.Send(query);
            var configurationResource = this.Mapper.Map<ConfigurationReadModel>(configurationDto);

            return this.Ok(configurationResource);
        }
    }
}
