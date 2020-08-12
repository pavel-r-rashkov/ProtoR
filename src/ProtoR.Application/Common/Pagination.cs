namespace ProtoR.Application.Common
{
    public class Pagination
    {
        public Pagination()
        {
        }

        public Pagination(int page, int size)
        {
            this.Page = page;
            this.Size = size;
        }

        public int Page { get; set; }

        public int Size { get; set; }

        public static Pagination Default()
        {
            return new Pagination(1, 10);
        }
    }
}
