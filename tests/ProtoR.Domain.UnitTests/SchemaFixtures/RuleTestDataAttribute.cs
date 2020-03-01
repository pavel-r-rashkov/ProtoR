namespace ProtoR.Domain.UnitTests.SchemaFixtures
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using ProtoR.Domain.SchemaGroupAggregate.Schemas;
    using Xunit;
    using Version = ProtoR.Domain.SchemaGroupAggregate.Schemas.Version;

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class RuleTestDataAttribute : ClassDataAttribute
    {
        private string ruleName;
        private string schemaName;

        public RuleTestDataAttribute(string ruleName, string schemaName)
            : base(null)
        {
            this.ruleName = ruleName;
            this.schemaName = schemaName;
        }

        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            string firstContents = SchemaFixtureUtils.GetProtoBuf(Path.Combine(this.ruleName, $"{this.schemaName}V1"));
            string secondContents = SchemaFixtureUtils.GetProtoBuf(Path.Combine(this.ruleName, $"{this.schemaName}V2"));

            var schemaFactory = new ProtoBufSchemaFactory();
            ProtoBufSchema firstSchema = schemaFactory.CreateNew(Version.Initial, firstContents);
            ProtoBufSchema secondSchema = schemaFactory.CreateNew(Version.Initial.Next(), secondContents);

            yield return new[] { firstSchema, secondSchema };
        }
    }
}
