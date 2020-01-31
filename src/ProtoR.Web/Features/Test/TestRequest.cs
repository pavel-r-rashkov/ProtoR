namespace Web.Features.Test
{
    using MediatR;

    public class TestRequest : IRequest<TestResponse>
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}
