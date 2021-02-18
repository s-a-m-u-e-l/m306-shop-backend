namespace API.Helpers
{
    public sealed class FilterParams
    {
        /// <summary>
        /// Specifies the column to filter by
        /// </summary>
        public string FilterBy { get; set; }

        /// <summary>
        /// Specifies the filter query
        /// </summary>
        public string FilterQuery { get; set; }
    }
}