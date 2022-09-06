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

namespace PortfolioLibrary
{
    public class FileHandler
    {
        public static TestParameters JsonHandler(string path)
        {
            string jsonString = File.ReadAllText(path);
            var options = new JsonSerializerOptions
            {
                Converters = { new RebalancingOracleDescriptionConverter() }
            };
            var testParameters = JsonSerializer.Deserialize<TestParameters>(jsonString, options);
            return testParameters!;
        }



        public static List<DataFeed> CsvHandlerInput(string path, int numberOfShares)
        {
            string[] rawCsv = System.IO.File.ReadAllLines(path);
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


        public static void CsvHandlerOutput(string filePath, string kindOfOutput, List<double> values)
        {
            List<String> strings = new List<String>();
            strings.Add(kindOfOutput);
            foreach (double value in values)
            {
                // Apply formatting to the string if necessary
                strings.Add(value.ToString().Replace(',', '.'));
            }

            File.WriteAllLines(filePath, strings, Encoding.UTF8);
        }
    }
}
