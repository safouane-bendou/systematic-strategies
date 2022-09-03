using System.Text.Json;
using PricingLibrary;
using PricingLibrary.DataClasses;
using PricingLibrary.RebalancingOracleDescriptions;

using CsvHelper;
using System.IO;
using System.Globalization;
using System.Linq;
using System.Dynamic;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using PricingLibrary.MarketDataFeed;

namespace DeserializeExtra
{

    public class FilesHandler
    {


        public static TestParameters JsonHandler(string path, string fileName)
        {
            string jsonString = File.ReadAllText(path + "\\" + fileName);
            var options = new JsonSerializerOptions
            {
                Converters = { new RebalancingOracleDescriptionConverter() }
            };
            var handler = JsonSerializer.Deserialize<TestParameters>(jsonString, options);
            return handler;
        }


        public static double CsvStringToDouble(string value)
        {
            double doubleValue = Convert.ToDouble(value);
            return (doubleValue);
        }

        public static List<DataFeed> CsvHandler(string path, string csvFile, int numberOfShares)
        {
            string[] rawCsv = System.IO.File.ReadAllLines(path + "\\" + csvFile);
            var allDataFeeds = new List<DataFeed>();
            for (int i = 1; i < rawCsv.Length / numberOfShares; i += numberOfShares)
            {
                string[] rowData = rawCsv[i].Split(',');
                DateTime currentDate = Convert.ToDateTime(rowData[1] + " AM");
                Dictionary<string, double> priceList = new Dictionary<string, double>();
                for (int j = 1; j < numberOfShares; j++)
                {
                    rowData = rawCsv[i + j].Split(',');
                    priceList.Add(rowData[0], Convert.ToDouble(rowData[2]));
                }
                DataFeed blockDataFeed = new DataFeed(currentDate, priceList);

                allDataFeeds.Add(blockDataFeed);
            }

            return allDataFeeds;
        }
        public static void Main()
        {
            string path = "C:\\Users\\sbend\\Desktop\\ensimag\\c#\\systematic-strategies";
            string fileName = "TestParameters\\share_5_strike_11.json";
            var jsonResult = JsonHandler(path, fileName);
            var dataFeedList = CsvHandler("C:\\Users\\sbend\\Desktop\\ensimag\\c#\\systematic-strategies", "MarketData\\data_share_5_2.csv", 5);
            Console.WriteLine($"{jsonResult.BasketOption.Maturity}");
            Console.WriteLine($"{dataFeedList[0].Date}");
        }

            
    }
}

