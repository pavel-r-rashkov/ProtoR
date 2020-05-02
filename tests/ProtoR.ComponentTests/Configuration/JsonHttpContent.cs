namespace ProtoR.ComponentTests.Configuration
{
    using System.Net.Http;
    using System.Text;
    using Newtonsoft.Json;

    public class JsonHttpContent : StringContent
    {
        public JsonHttpContent(object contents)
            : base(JsonConvert.SerializeObject(contents), Encoding.UTF8, "application/json")
        {
        }
    }
}
