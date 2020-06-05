namespace ProtoR.Web.Infrastructure.Identity
{
    public class AuthenticationConfiguration
    {
        public bool AuthenticationEnabled { get; set; }

        public string SigningCredentialPath { get; set; }

        public string SigningCredentialPassword { get; set; }
    }
}
