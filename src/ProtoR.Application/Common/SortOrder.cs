namespace ProtoR.Application.Common
{
    using System.Collections.Generic;

    public class SortOrder
    {
        public string PropertyName { get; set; }

        public SortDirection Direction { get; set; }

        public static IEnumerable<SortOrder> Default(string propertyName)
        {
            return new SortOrder[]
            {
                new SortOrder { PropertyName = propertyName, Direction = SortDirection.Ascending },
            };
        }
    }
}
