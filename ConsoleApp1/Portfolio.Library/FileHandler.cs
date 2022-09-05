using PricingLibrary.DataClasses;
using PricingLibrary.MarketDataFeed;
using PricingLibrary.RebalancingOracleDescriptions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Portfolio.Library
{
    public class FileHandler
    {
        public static TestParameters JsonHandler(string path, string fileName)
        {
            string jsonString = File.ReadAllText(path + "\\" + fileName);
            var options = new JsonSerializerOptions
            {
                Converters = { new RebalancingOracleDescriptionConverter() }
            };
            var testParameters = JsonSerializer.Deserialize<TestParameters>(jsonString, options);
            return testParameters!;
        }



        public static List<DataFeed> CsvHandler(string path, string csvFile, int numberOfShares)
        {
            string[] rawCsv = System.IO.File.ReadAllLines(path + "\\" + csvFile);
            var allDataFeeds = new List<DataFeed>();
            for (int i = 1; i < rawCsv.Length / numberOfShares; i += numberOfShares)
            {
                string[] rowData = rawCsv[i].Split(',');
                DateTime currentDate = Convert.ToDateTime(rowData[1] + " AM", new CultureInfo("en-US"));
                Dictionary<string, double> priceList = new Dictionary<string, double>();
                for (int j = 0; j < numberOfShares; j++)
                {
                    rowData = rawCsv[i + j].Split(',');
                    NumberFormatInfo provider = new NumberFormatInfo();
                    provider.NumberDecimalSeparator = ".";
                    priceList.Add(rowData[0], Convert.ToDouble(rowData[2], provider));
                }
                DataFeed blockDataFeed = new DataFeed(currentDate, priceList);

                allDataFeeds.Add(blockDataFeed);
            }
            return allDataFeeds;
        }
    }
}
