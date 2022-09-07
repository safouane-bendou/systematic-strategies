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
using PortfolioLibrary;

namespace ConsoleApp1
{

    public class Program
    {
        public static void Main(string[] args)
        {
            //string paramPath = "C:\\Users\\sbend\\Desktop\\ensimag\\c#\\systematic-strategies\\TestParameters\\share_5_strike_9.json";
            //string marketPath = "C:\\Users\\sbend\\Desktop\\ensimag\\c#\\systematic-strategies\\MarketData\\data_share_5_2.csv";
            //var marketData = FileHandler.CsvHandlerInput(path, 5);

            List<double> optionPrices = new List<double>();
            string path = args[0];
            
            var testParameters = FileHandler.JsonHandler(path);
            int numberOfShares = testParameters.BasketOption.UnderlyingShareIds.Count();
            var marketData = FileHandler.CsvHandlerInput(args[1], numberOfShares);
            List<double> portfolioValues = PortfolioComputations.PortfolioValues(marketData, testParameters);
            Pricer pricer = new Pricer(testParameters);
            List<string> allDates = new List<string>();
            for (int i = 0; i < portfolioValues.Count; i++)
            {
                double TimeToMaturity = MathDateConverter.ConvertToMathDistance(marketData[i].Date, testParameters.BasketOption.Maturity);
                double[] Spots = PortfolioComputations.SpotsArray(marketData[i].PriceList);
                allDates.Add(marketData[i].Date.ToString());
                PricingResults PriceResult = pricer.Price(TimeToMaturity, Spots);
                optionPrices.Add(PriceResult.Price);
                Console.WriteLine(allDates[i] + " " + PriceResult.Price + " " + portfolioValues[i]);
            }
            FileHandler.CsvHandlerOutput(args[2], "Dates" + "," + "Portfolio Values", allDates, portfolioValues);
            FileHandler.CsvHandlerOutput(args[3], "Dates" + "," + "Option Prices", allDates, optionPrices);
            /*FileHandler.CsvHandlerOutput("C:\\Users\\localuser\\source\\repos\\systematic-strategies(1)\\PortfolioValues.csv", "Portfolio Values", portfolio);
            FileHandler.CsvHandlerOutput("C:\\Users\\localuser\\source\\repos\\systematic-strategies(1)\\OptionPrices.csv", "Option Prices", OptionPrices);*/

        }


    }
}