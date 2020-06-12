namespace ProtoR.Web.Infrastructure
{
    public static class StringExtensions
    {
        public static string ToCamelCase(this string input)
        {
            return char.ToLowerInvariant(input[0]) + (input.Length > 1 ? input.Substring(1) : string.Empty);
        }
    }
}
