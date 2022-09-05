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
        public static void Main()
        {
            string path = "C:\\Users\\localuser\\Desktop\\c#\\systematic-strategies";
            string fileName = "TestParameters\\share_5_strike_9.json";
            var testParameters = FileHandler.JsonHandler(path, fileName);
            var marketData = FileHandler.CsvHandler("C:\\Users\\localuser\\Desktop\\c#\\systematic-strategies", "MarketData\\data_share_5_2.csv", 5);
            //Console.WriteLine($"{testParameters.BasketOption.UnderlyingShareIds[0]}");
            //Console.WriteLine($"{marketData[1].PriceList["share_3"]}");
            var portfolio = PortfolioComputations.PortfolioValues(testParameters, marketData);
            for (int i=0; i < portfolio.Count; i++)
            {
                Pricer pricer = new Pricer(testParameters);
                double TimeToMaturity = MathDateConverter.ConvertToMathDistance(marketData[i].Date, testParameters.BasketOption.Maturity);
                double[] Spots = PortfolioComputations.SpotsArray(marketData[i]);
                PricingResults PriceResult = pricer.Price(TimeToMaturity, Spots);
                Console.WriteLine(PriceResult.Price + " " + portfolio[i]);
                
            }
            
        }


    }
}

