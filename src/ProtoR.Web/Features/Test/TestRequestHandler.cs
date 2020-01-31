namespace Web.Features.Test
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;

    public class TestRequestHandler : IRequestHandler<TestRequest, TestResponse>
    {
        public async Task<TestResponse> Handle(TestRequest request, CancellationToken cancellationToken)
        {
            return await Task.FromResult(new TestResponse()
            {
                FullName = $"{request.FirstName} {request.LastName}",
            });
        }
    }
}
