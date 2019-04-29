using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BeveragePairer
{

    class DishData
    {
        public static DishMetadata GetDishList(string encodedRequest)
        {
            //HTTP request to recipepuppy to get dishes based on typed in value
            using (WebResponse wr = WebRequest.Create("http://www.recipepuppy.com/api/?q=" + encodedRequest).GetResponse())
            {
                using (StreamReader sr = new StreamReader(wr.GetResponseStream()))
                {
                    var result = sr.ReadToEnd();
                    DishMetadata json = JsonConvert.DeserializeObject<DishMetadata>(result);
                    return json;
                }
            }
        }
        //public property to access the selected dish from the main process
        public static Dish SelectedDish {get; set;}    
    }
}
