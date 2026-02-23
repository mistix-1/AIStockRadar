namespace AIStockRadar.Models
{
    public class AiRecommendationViewModel
    {
        public string Ticker { get; set; }
        public string Confidence { get; set; }
        public string Volatility { get; set; }
        public string ErrorMessage { get; set; }
    }
}