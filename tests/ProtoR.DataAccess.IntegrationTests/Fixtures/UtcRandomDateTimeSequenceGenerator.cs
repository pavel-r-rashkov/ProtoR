namespace ProtoR.DataAccess.IntegrationTests.Fixtures
{
    using System;
    using AutoFixture;
    using AutoFixture.Kernel;

    internal class UtcRandomDateTimeSequenceGenerator : ISpecimenBuilder
{
    private readonly ISpecimenBuilder innerRandomDateTimeSequenceGenerator;

    internal UtcRandomDateTimeSequenceGenerator()
    {
        this.innerRandomDateTimeSequenceGenerator = new RandomDateTimeSequenceGenerator();
    }

    public object Create(object request, ISpecimenContext context)
    {
        var result = this.innerRandomDateTimeSequenceGenerator.Create(request, context);

        if (result is NoSpecimen)
        {
            return result;
        }

        return ((DateTime)result).ToUniversalTime();
    }
}
}
