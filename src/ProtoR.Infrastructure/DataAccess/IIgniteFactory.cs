namespace ProtoR.Infrastructure.DataAccess
{
    using Apache.Ignite.Core;

    public interface IIgniteFactory
    {
         void InitalizeIgnite();

         IIgnite Instance();
    }
}
