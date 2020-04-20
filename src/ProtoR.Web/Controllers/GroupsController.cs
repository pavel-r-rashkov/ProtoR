namespace ProtoR.Web.Controllers
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Threading.Tasks;
    using AutoMapper;
    using MediatR;
    using Microsoft.AspNetCore.Mvc;
    using ProtoR.Application.Configuration;
    using ProtoR.Application.Group;
    using ProtoR.Application.Schema;
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
            string version = await this.Mediator.Send(command);

            return this.CreatedAtAction(nameof(this.GetSchema), new { schema.GroupName, Version = version });
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
