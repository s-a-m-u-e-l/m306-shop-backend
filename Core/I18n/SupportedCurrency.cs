namespace Core.I18n
{
    public sealed class SupportedCurrency
    {
        /// <summary>
        /// Currency title, e.g. Euro
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Currency code, e.g. EUR
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Whether this currency is the default one
        /// </summary>
        public bool IsDefault { get; set; }
    }
}