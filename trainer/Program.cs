using Microsoft.Data.DataView;
using Microsoft.ML;
using Microsoft.ML.Data;
using System;

// CS0649 compiler warning is disabled because some fields are only
// assigned to dynamically by ML.NET at runtime
#pragma warning disable CS0649

namespace myMLApp
{
    class Program
    {
        // STEP 1: Define your data structures
        // IrisData is used to provide training data, and as
        // input for prediction operations
        // - First 4 properties are inputs/features used to predict the label
        // - Label is what you are predicting, and is only set when training
        public class FoodData
        {
            [LoadColumn(0)]
            public string Ingredients;

            [LoadColumn(1)]
            public string Beverage;
        }

        // IrisPrediction is the result returned from prediction operations
        public class BeveragePrediction
        {
            [ColumnName("PredictedLabel")]
            public string PredictedLabels;
        }

        static void Main(string[] args)
        {
            // STEP 2: Create a ML.NET environment
            MLContext mlContext = new MLContext();

            // If working in Visual Studio, make sure the 'Copy to Output Directory'
            // property of iris-data.txt is set to 'Copy always'
            IDataView trainingDataView = mlContext.Data.LoadFromTextFile<FoodData>(path: "Beverage-Food-Data.csv", hasHeader: false, separatorChar: ',');

            // STEP 3: Transform your data and add a learner
            // Assign numeric values to text in the "Label" column, because only
            // numbers can be processed during model training.
            // Add a learning algorithm to the pipeline. e.g.(What type of iris is this?)
            // Convert the Label back into original text (after converting to number in step 3)
            var pipeline = mlContext.Transforms.Conversion.MapValueToKey(inputColumnName: "Beverage", outputColumnName: "Label")
                .Append(mlContext.Transforms.Text.FeaturizeText(inputColumnName: "Ingredients", outputColumnName: "IngredientsFeaturized"))
                .Append(mlContext.Transforms.Concatenate("Features", "IngredientsFeaturized"))
                .AppendCacheCheckpoint(mlContext)
                .Append(mlContext.MulticlassClassification.Trainers.StochasticDualCoordinateAscent(DefaultColumnNames.Label, DefaultColumnNames.Features))
                    .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));
                // .Append(mlContext.MulticlassClassification.Trainers.StochasticDualCoordinateAscent(labelColumnName: "Label", featureColumnName: "Features"))
                // .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

            // STEP 4: Train your model based on the data set
            var model = pipeline.Fit(trainingDataView);

            // STEP 5: Use your model to make a prediction
            // You can change these numbers to test different predictions
            var prediction = model.CreatePredictionEngine<FoodData, BeveragePrediction>(mlContext).Predict(
                new FoodData()
                {
                    Ingredients="beef wellington"
                });

            Console.WriteLine($"Predicted beverage is: {prediction.PredictedLabels}");

            Console.WriteLine("Press any key to exit....");
            Console.ReadLine();
        }
    }
}