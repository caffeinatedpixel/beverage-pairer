using Microsoft.Data.DataView;
using Microsoft.ML;
using Microsoft.ML.Data;
using System;
using System.Collections.Generic;

// CS0649 compiler warning is disabled because some fields are only
// assigned to dynamically by ML.NET at runtime
#pragma warning disable CS0649

namespace BeveragePairer
{
    public class FindBeverage
    {
        public class FoodData
        {
            [LoadColumn(0)]
            public string Ingredients;

            [LoadColumn(1)]
            public string Beverage;
        }

        public class BeveragePrediction
        {
            [ColumnName("PredictedLabel")]
            public string PredictedLabels;
        }

        public static List<string> GetBeverage()
        {
            MLContext mlContext = new MLContext();
            IDataView trainingDataView = mlContext.Data.LoadFromTextFile<FoodData>(path: "Beverage-Food-Data.csv", hasHeader: false, separatorChar: ',');
           //use a multiclass linear regression algorithm to find the right beverages for the dish
            var pipeline = mlContext.Transforms.Conversion.MapValueToKey(inputColumnName: "Beverage", outputColumnName: "Label")
                .Append(mlContext.Transforms.Text.FeaturizeText(inputColumnName: "Ingredients", outputColumnName: "IngredientsFeaturized"))
                .Append(mlContext.Transforms.Concatenate("Features", "IngredientsFeaturized"))
                .AppendCacheCheckpoint(mlContext)
                .Append(mlContext.MulticlassClassification.Trainers.StochasticDualCoordinateAscent(DefaultColumnNames.Label, DefaultColumnNames.Features))
                .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));
                

            //train
            var model = pipeline.Fit(trainingDataView);

            List<string> beverages = new List<string>();
            foreach (string ingredient in
                DishData.SelectedDish.ingredients)
            {
                //make prediction
                var prediction = model.CreatePredictionEngine<FoodData, BeveragePrediction>(mlContext).Predict(
                    new FoodData()
                    {
                        Ingredients = ingredient
                    });
                beverages.Add(prediction.PredictedLabels); 
            }
            return beverages;
        }
    }
}