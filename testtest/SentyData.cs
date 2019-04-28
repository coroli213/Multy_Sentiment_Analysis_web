using Microsoft.ML.Data;

namespace testtest
{
    public class SentimentData
    {
        [LoadColumn(0)]
        public string Tone { get; set; }
        [LoadColumn(1)]
        public string Sentiment { get; set; }
    }

    public class SentimentPrediction
    {
        [ColumnName("PredictedLabel")]
        public string Sentiment;
    }
}

