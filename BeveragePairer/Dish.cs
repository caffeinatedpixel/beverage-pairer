using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeveragePairer
{
    public class DishMetadata
    {
        public string title { get; set; }
        public string version { get; set; }
        public string href { get; set; }
        [JsonProperty("results")]
        public List<Dish> Dishes { get; set; }
    }

    public class Dish
    {
        public string title { get; set; }
        public string href { get; set; }

        [JsonConverter(typeof(IngredientConverter))]
        public string[] ingredients { get; set; }
        public string thumbnail { get; set; }
    }


    public class Ingredient
    {
        public string ingredient { get; set; }
    }

    public class IngredientConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            if (objectType == typeof(string))
            {
                return true;
            }

            return false;
        }

        public override object ReadJson(JsonReader reader, Type objectType
            , object existingValue, JsonSerializer serializer)
        {
            string token = (string)JToken.Load(reader);
            string[] model = token.Split(',');
            return model;
        }

        public override void WriteJson(JsonWriter writer, object value
            , JsonSerializer serializer)
        {
            // we dont need to serialize json
            throw new NotImplementedException();
        }
    }
}
