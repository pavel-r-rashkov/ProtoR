namespace ProtoR.Infrastructure.DataAccess
{
    using Apache.Ignite.Core;
    using Autofac;

    public interface IIgniteFactory
    {
         void InitalizeIgnite();

         void SetAutoFacPlugin(ILifetimeScope scope);

         IIgnite Instance();
    }
}
