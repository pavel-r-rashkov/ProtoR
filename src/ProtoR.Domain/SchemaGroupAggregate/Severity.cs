namespace ProtoR.Domain.SchemaGroupAggregate
{
    using ProtoR.Domain.SeedWork;

    public class Severity : Enumeration
    {
        protected Severity(int id, string name, bool isFatal)
            : base(id, name)
        {
            this.IsFatal = isFatal;
        }

        public static Severity Hidden { get; } = new Severity(1, "Hidden", false);

        public static Severity Info { get; } = new Severity(2, "Info", false);

        public static Severity Warning { get; } = new Severity(3, "Warning", false);

        public static Severity Error { get; } = new Severity(4, "Error", true);

        public bool IsFatal { get; }
    }
}
