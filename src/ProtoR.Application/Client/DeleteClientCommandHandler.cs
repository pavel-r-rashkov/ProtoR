namespace ProtoR.Application.Client
{
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using ProtoR.Domain.ClientAggregate;
    using ProtoR.Domain.Exceptions;
    using ProtoR.Domain.SeedWork;

    public class DeleteClientCommandHandler : AsyncRequestHandler<DeleteClientCommand>
    {
        private readonly IClientRepository clientRepository;
        private readonly IUnitOfWork unitOfWork;

        public DeleteClientCommandHandler(
            IClientRepository clientRepository,
            IUnitOfWork unitOfWork)
        {
            this.clientRepository = clientRepository;
            this.unitOfWork = unitOfWork;
        }

        protected override async Task Handle(DeleteClientCommand request, CancellationToken cancellationToken)
        {
            var client = await this.clientRepository.GetById(request.ClientId);

            if (client == null)
            {
                throw new EntityNotFoundException<Client>(request.ClientId);
            }

            await this.clientRepository.Delete(request.ClientId);
            await this.unitOfWork.SaveChanges();
        }
    }
}
