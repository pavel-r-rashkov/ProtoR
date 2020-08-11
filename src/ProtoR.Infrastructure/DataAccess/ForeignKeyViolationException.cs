namespace ProtoR.Infrastructure.DataAccess
{
    using System;

    public class ForeignKeyViolationException : Exception
    {
        public ForeignKeyViolationException()
        {
        }

        public ForeignKeyViolationException(string message)
            : base(message)
        {
        }

        public ForeignKeyViolationException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
