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
using PricingLibrary.Computations;
using PricingLibrary.TimeHandler;
using MathNet.Numerics.Statistics;
using Portfolio.Library;

namespace ConsoleApp1
{

    public class Program
    {
        public static void Main(string[] args)
        {
            List<double> OptionPrices = new List<double>();
            string path = args[0];
            //string fileName = "share_5_strike_11.json";
            var testParameters = FileHandler.JsonHandler(path);
            var marketData = FileHandler.CsvHandlerInput(args[1], 5);
            /*Console.WriteLine($"{testParameters.BasketOption.UnderlyingShareIds[0]}");
            Console.WriteLine($"{marketData[1].PriceList["share_3"]}");*/
            var portfolio = PortfolioComputations.PortfolioValues(testParameters, marketData);
            for (int i = 0; i < portfolio.Count; i++)
            {
                Pricer pricer = new Pricer(testParameters);
                double TimeToMaturity = MathDateConverter.ConvertToMathDistance(marketData[i].Date, testParameters.BasketOption.Maturity);
                double[] Spots = PortfolioComputations.SpotsArray(marketData[i]);
                PricingResults PriceResult = pricer.Price(TimeToMaturity, Spots);
                OptionPrices.Add(PriceResult.Price);
                Console.WriteLine(PriceResult.Price + " " + portfolio[i]);
            }
            FileHandler.CsvHandlerOutput(args[2], "Portfolio Values", portfolio);
            FileHandler.CsvHandlerOutput(args[3], "Option Prices", OptionPrices);
            /*FileHandler.CsvHandlerOutput("C:\\Users\\localuser\\source\\repos\\systematic-strategies(1)\\PortfolioValues.csv", "Portfolio Values", portfolio);
            FileHandler.CsvHandlerOutput("C:\\Users\\localuser\\source\\repos\\systematic-strategies(1)\\OptionPrices.csv", "Option Prices", OptionPrices);*/

        }


    }
}