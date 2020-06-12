namespace ProtoR.Web.Resources
{
    /// <summary>
    /// Success response.
    /// </summary>
    /// <typeparam name="T">Data type.</typeparam>
    public class ResponseModel<T>
    {
        public ResponseModel(T data)
        {
            this.Data = data;
        }

        /// <summary>
        /// Response data.
        /// </summary>
        public T Data { get; set; }
    }
}
