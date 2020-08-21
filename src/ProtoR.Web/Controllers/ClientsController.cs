namespace ProtoR.Web.Controllers
{
    using System.Threading.Tasks;
    using AutoMapper;
    using MediatR;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using ProtoR.Application.Client;
    using ProtoR.Application.Common;
    using ProtoR.Web.Infrastructure.Identity;
    using ProtoR.Web.Resources;
    using ProtoR.Web.Resources.ClientResource;

    public class ClientsController : BaseController
    {
        public ClientsController(
            IMediator mediator,
            IMapper mapper)
            : base(mediator, mapper)
        {
        }

        /// <summary>
        /// Get all clients.
        /// </summary>
        /// <returns>Clients.</returns>
        /// <response code="200">Client list.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet]
        [PermissionClaim(Permission.ClientRead)]
        public async Task<ActionResult<ResponseModel<PagedResult<ClientReadModel>>>> Get([FromQuery]GetClientsQuery query)
        {
            var clients = await this.Mediator.Send(query);
            var clientResources = this.Mapper.Map<PagedResult<ClientReadModel>>(clients);

            return this.Ok(new ResponseModel<PagedResult<ClientReadModel>>(clientResources));
        }

        /// <summary>
        /// Get client by ID.
        /// </summary>
        /// <returns>Client.</returns>
        /// <response code="200">Client with requested ID.</response>
        /// <response code="404">Client with the specified ID doesn't exist.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [HttpGet]
        [Route("{ClientId}")]
        [PermissionClaim(Permission.ClientRead)]
        public async Task<ActionResult<ResponseModel<ClientReadModel>>> Get([FromRoute]GetClientByIdQuery query)
        {
            var clients = await this.Mediator.Send(query);
            var clientResource = this.Mapper.Map<ClientReadModel>(clients);

            return this.Ok(new ResponseModel<ClientReadModel>(clientResource));
        }

        /// <summary>
        /// Create a new client.
        /// </summary>
        /// <returns>No content.</returns>
        /// <response code="201">Client created successfully.</response>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [HttpPost]
        [PermissionClaim(Permission.ClientWrite)]
        public async Task<ActionResult> Post(ClientWriteModel client)
        {
            var command = this.Mapper.Map<CreateClientCommand>(client);
            var clientId = await this.Mediator.Send(command);

            return this.CreatedAtAction(nameof(this.Get), new { ClientId = clientId }, null);
        }

        /// <summary>
        /// Update existing client.
        /// </summary>
        /// <returns>No content.</returns>
        /// <response code="204">Client updated successfully.</response>
        /// <response code="404">Client with the specified ID doesn't exist.</response>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [HttpPut]
        [Route("{Id}")]
        [PermissionClaim(Permission.ClientWrite)]
        public async Task<ActionResult> Put(ClientWriteModel client)
        {
            var command = this.Mapper.Map<UpdateClientCommand>(client);
            await this.Mediator.Send(command);

            return this.NoContent();
        }

        /// <summary>
        /// Delete a client.
        /// </summary>
        /// <returns>No conent.</returns>
        /// <response code="204">Client deleted successfully.</response>
        /// <response code="404">Client with the specified ID doesn't exist.</response>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [HttpDelete]
        [Route("{clientId}")]
        [PermissionClaim(Permission.ClientWrite)]
        public async Task<ActionResult> Delete(long clientId)
        {
            await this.Mediator.Send(new DeleteClientCommand { ClientId = clientId });

            return this.NoContent();
        }
    }
}
