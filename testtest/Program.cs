using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.DataView;
using Microsoft.ML;
using Microsoft.ML.Data;

namespace testtest
{
    public class Program
    {
        public static string _appPath => Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]);
        public static string _ENBiTrainDataPath => Path.Combine(_appPath, "..", "..", "..", "Data", "EnglishBinaryTrainer.tsv");
        public static string _RUBiTrainDataPath => Path.Combine(_appPath, "..", "..", "..", "Data", "RussianBinaryTrainer.tsv");
        public static string _RUMuTrainDataPath => Path.Combine(_appPath, "..", "..", "..", "Data", "RussianTrisomTrainer.tsv");
        public static string _modelPath         => Path.Combine(_appPath, "..", "..", "..", "Models", "SentyModel.zip");

    //    https://modely.file.core.windows.net/savedmodel

        public static MLContext mlContext = new MLContext(seed: 0);
        public static ITransformer trainedModel;
        public static IDataView trainingDataView;

        public static void Main(string[] args)
        {
          
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IEstimator<ITransformer> ProcessData_()
        {
            Console.WriteLine($"=============== Обработка данных ===============");

            var pipeline = mlContext.Transforms.Conversion.MapValueToKey(inputColumnName: "Sentiment", outputColumnName: "Label")
                            .Append(mlContext.Transforms.Text.FeaturizeText(inputColumnName: "Tone", outputColumnName: "ToneFeaturized"))
                            .Append(mlContext.Transforms.Concatenate("Features", "ToneFeaturized"))
                            .AppendCacheCheckpoint(mlContext);

            Console.WriteLine($"=============== Данные обработны ===============");

            return pipeline;
        }

        public static ITransformer BuildAndTrainModel_(IDataView trainingDataView, IEstimator<ITransformer> pipeline)
        {
            var trainingPipeline = pipeline.Append(mlContext.MulticlassClassification.Trainers.NaiveBayes(DefaultColumnNames.Label, DefaultColumnNames.Features))
                                           .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

            Console.WriteLine($"=============== Начало обучения модели:     {DateTime.Now.TimeOfDay.ToString()} ===============");

            trainedModel = trainingPipeline.Fit(trainingDataView);

            Console.WriteLine($"=============== Завершение обучения модели: {DateTime.Now.TimeOfDay.ToString()} ===============");

            return trainedModel;
        }

        public static string UseModelWithSingleItem(string input)
        {
            PredictionEngine<SentimentData, SentimentPrediction> predictionFunction = trainedModel.CreatePredictionEngine<SentimentData, SentimentPrediction>(mlContext);

            var resultprediction = predictionFunction.Predict(new SentimentData { Tone = input });

            if (resultprediction.Sentiment == "1")
                return "Позитивная";
            else if (resultprediction.Sentiment == "0")
                return "Нормальная";
            else
                return "Негативная";
        }

        public static void UseLoadedModelWithBatchItems()
        {
            //данные для тестирования модели, 
            IEnumerable<SentimentData> sentiments_ = new[]
            {
                new SentimentData
                {
                    Tone = "turtle walks around the aquarium"
                },
                new SentimentData
                {
                    Tone = "peach is friends with grapes"
                },
                new SentimentData
                {
                    Tone = "this tree has been growing for 20 years"
                },
                new SentimentData
                {
                    Tone = "The service was meh."
                },
                new SentimentData
                {
                    Tone = "The turkey and roast beef were bland."
                },
                new SentimentData
                {
                    Tone = "If you want a sandwich just go to any Firehouse!!!!! "
                },
                new SentimentData
                {
                    Tone = "The burger is good beef, cooked just right."
                },
                new SentimentData
                {
                    Tone = "pasha came at first lesson and he look not goog"
                },
                new SentimentData
                {
                    Tone = "me too seat here"
                },
                new SentimentData
                {
                    Tone = "i eat soup in the bar"
                },
                new SentimentData
                {
                    Tone = "johny seaat in class and learn his lessons"
                },
                new SentimentData
                {
                    Tone = "i go home and see the forest"
                },
                new SentimentData
                {
                    Tone = "i just waked up at 7 o'clock am and wash my mouth"
                },
                new SentimentData
                {
                    Tone = "The food, amazing."
                },
                new SentimentData
                {
                    Tone = "Poor service, the waiter made me feel like I was stupid every time he came to the table."
                },
                new SentimentData
                {
                    Tone = "The portion was huge!"
                },
                new SentimentData
                {
                    Tone = "Loved it...friendly servers, great food, wonderful and imaginative menu.	"
                },
                new SentimentData
                {
                    Tone = "The Heart Attack Grill in downtown Vegas is an absolutely flat-lined excuse for a restaurant."
                },
                new SentimentData
                {
                    Tone = "At least 40min passed in between us ordering and the food arriving, and it wasn't that busy."
                },
                new SentimentData
                {
                    Tone = "This was a horrible meal"
                },
                new SentimentData
                {
                    Tone = "I love this spaghetti."
                }
            };//блок английских текстов
            IEnumerable<SentimentData> sentiments = new[]{
                new SentimentData
                {
                    Tone = "Объясняй им новую тему вместо меня и я ставлю тебе пять.  Мне пизда."
                },
                new SentimentData
                {
                    Tone = "Люблю макароны"
                },
                new SentimentData
                {
                    Tone = "Ненавижу это место"
                },
                new SentimentData
                {
                    Tone = "Он Филдин, ты жи сам говорил"
                },
                new SentimentData
                {
                    Tone = "Я люблю эти макароны"
                },
               new SentimentData
                {
                    Tone = "Это было ужасное мясо"
                }

            };//блок русских    текстов

            IDataView sentimentStreamingDataView = mlContext.Data.LoadFromEnumerable(sentiments_);

            IDataView predictions = trainedModel.Transform(sentimentStreamingDataView);

            IEnumerable<SentimentPrediction> predictedResults = mlContext.Data.CreateEnumerable<SentimentPrediction>(predictions, reuseRowObject: false);

            Console.WriteLine();
            Console.WriteLine("=============== Тест прогнозирования модели ===============");
            Console.WriteLine();

            IEnumerable<(SentimentData sentiment, SentimentPrediction prediction)> sentimentsAndPredictions = sentiments_.Zip(predictedResults, (sentiment, prediction) => (sentiment, prediction));

            foreach ((SentimentData sentiment, SentimentPrediction prediction) item in sentimentsAndPredictions)
            {
                Console.WriteLine($"Prediction: {item.prediction.Sentiment}  | Текст: {item.sentiment.Tone}");
            }
            Console.WriteLine();
            Console.WriteLine("=============== Завершение теста ===============");

        }

        public static void SaveModelAsFile()
        {
            using (var fs = new FileStream(_modelPath, FileMode.Create, FileAccess.Write, FileShare.Write))
                mlContext.Model.Save(trainedModel, fs);
            Console.WriteLine("The model is saved to {0}", _modelPath);
        }

        public static void LoadModelFromFile()
        {
            using (var stream = new FileStream(_modelPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                trainedModel = mlContext.Model.Load(stream);
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();

    }
}
