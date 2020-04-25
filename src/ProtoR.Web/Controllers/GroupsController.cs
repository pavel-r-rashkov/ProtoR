namespace ProtoR.Web.Controllers
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
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
        private readonly IIgniteFactory igniteFactory;

        public GroupsController(
            IMediator mediator,
            IMapper mapper,
            IIgniteFactory igniteFactory)
            : base(mediator, mapper)
        {
            this.igniteFactory = igniteFactory;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GroupReadModel>>> Get()
        {
            IEnumerable<GroupDto> groupsDto = await this.Mediator.Send(new GetGroupsQuery());
            var groupResources = this.Mapper.Map<IEnumerable<GroupReadModel>>(groupsDto);

            return this.Ok(groupResources);
        }

        [HttpPost]
        public async Task<ActionResult<IEnumerable<GroupReadModel>>> Post(GroupWriteModel group)
        {
            var command = this.Mapper.Map<CreateGroupCommand>(group);
            await this.Mediator.Send(command);

            return this.CreatedAtAction(nameof(this.GetByName), new { GroupName = group.Name }, null);
        }

        [HttpGet]
        [Route("{GroupName}")]
        public async Task<ActionResult<IEnumerable<GroupReadModel>>> GetByName([FromRoute]GetByNameQuery query)
        {
            GroupDto groupDto = await this.Mediator.Send(query);
            var groupResource = this.Mapper.Map<GroupReadModel>(groupDto);

            return this.Ok(groupResource);
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

            var clusterService = this.igniteFactory
                .Instance()
                .GetServices()
                .GetServiceProxy<IClusterSingletonService>(nameof(IClusterSingletonService));

            var commandResult = await clusterService.AddSchema(command);

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
        public async Task<ActionResult<IEnumerable<GroupReadModel>>> GetConfiguration([FromRoute]GetByGroupNameQuery query)
        {
            ConfigurationDto configurationDto = await this.Mediator.Send(query);
            var configurationResource = this.Mapper.Map<ConfigurationReadModel>(configurationDto);

            return this.Ok(configurationResource);
        }
    }
}
