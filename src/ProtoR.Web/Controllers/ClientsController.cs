namespace ProtoR.Web.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoMapper;
    using MediatR;
    using Microsoft.AspNetCore.Mvc;
    using ProtoR.Application.Client;
    using ProtoR.Web.Infrastructure.Identity;
    using ProtoR.Web.Resources.ClientResource;

    public class ClientsController : BaseController
    {
        public ClientsController(
            IMediator mediator,
            IMapper mapper)
            : base(mediator, mapper)
        {
        }

        [HttpGet]
        [PermissionClaim(Permission.ClientRead)]
        public async Task<ActionResult<IEnumerable<ClientReadModel>>> Get([FromRoute]GetClientsQuery query)
        {
            var clients = await this.Mediator.Send(query);
            var clientResources = this.Mapper.Map<IEnumerable<ClientReadModel>>(clients);

            return this.Ok(clientResources);
        }

        [HttpGet]
        [Route("{ClientId}")]
        [PermissionClaim(Permission.ClientRead)]
        public async Task<ActionResult<ClientReadModel>> Get([FromRoute]GetClientByIdQuery query)
        {
            var clients = await this.Mediator.Send(query);
            var clientResource = this.Mapper.Map<ClientReadModel>(clients);

            return this.Ok(clientResource);
        }

        [HttpPost]
        [PermissionClaim(Permission.ClientWrite)]
        public async Task<ActionResult> Post(ClientWriteModel client)
        {
            var command = this.Mapper.Map<CreateClientCommand>(client);
            var clientId = await this.Mediator.Send(command);

            return this.CreatedAtAction(nameof(this.Get), new { ClientId = clientId }, null);
        }

        [HttpPut]
        [Route("{Id}")]
        [PermissionClaim(Permission.ClientWrite)]
        public async Task<ActionResult> Put(ClientWriteModel client)
        {
            var command = this.Mapper.Map<UpdateClientCommand>(client);
            await this.Mediator.Send(command);

            return this.NoContent();
        }

        [HttpDelete]
        [Route("{Id}")]
        [PermissionClaim(Permission.ClientWrite)]
        public async Task<ActionResult> Delete(long clientId)
        {
            await this.Mediator.Send(new DeleteClientCommand { ClientId = clientId });

            return this.NoContent();
        }
    }
}
