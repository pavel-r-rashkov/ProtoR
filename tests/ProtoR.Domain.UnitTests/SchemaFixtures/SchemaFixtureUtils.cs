namespace ProtoR.Domain.UnitTests.SchemaFixtures
{
    using System;
    using System.IO;

    public static class SchemaFixtureUtils
    {
        private static readonly string ProtoBufSchemasLocation = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "SchemaFixtures",
            "ProtoBuf");

        public static string GetProtoBuf(string name)
        {
            string path = Path.Combine(ProtoBufSchemasLocation, $"{name}.proto");
            return File.ReadAllText(path);
        }
    }
}
