namespace ProtoR.Web.Infrastructure
{
    public class TlsConfiguration
    {
        public string CertificateLocation { get; set; }

        public string Password { get; set; }

        public bool ForceHttps { get; set; }
    }
}
