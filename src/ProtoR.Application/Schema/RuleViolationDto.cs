namespace ProtoR.Application.Schema
{
    public class RuleViolationDto
    {
        public string RuleCode { get; set; }

        public int Severity { get; set; }

        public bool IsFatal { get; set; }

        public string Description { get; set; }

        public bool IsForwardIncompatible { get; set; }

        public bool IsBackwardIncompatible { get; set; }

        public int ConflictingVersion { get; set; }
    }
}
