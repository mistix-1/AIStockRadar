namespace AIStockRadar.Models
{
    public class AiRecommendationViewModel
    {
        public string Ticker { get; set; }
        public double Confidence { get; set; }
        public double Volatility { get; set; }
        public string RiskLevel { get; set; }
        public string ErrorMessage { get; set; }
    }
}