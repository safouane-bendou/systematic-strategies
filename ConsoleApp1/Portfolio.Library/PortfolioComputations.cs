using PricingLibrary.Computations;
using PricingLibrary.DataClasses;
using PricingLibrary.MarketDataFeed;
using PricingLibrary.TimeHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portfolio.Library
{
    public class PortfolioComputations
    {
        public static double[] SpotsArray(DataFeed dataFeed)
        {
            int numberOfShares = dataFeed.PriceList.Count;
            double[] allSpots = new double[numberOfShares];
            for (int i = 0; i < dataFeed.PriceList.Count; i++)
            {
                double spot = dataFeed.PriceList.ElementAt(i).Value;
                allSpots[i] = spot;
            }
            return allSpots;
        }

        public static bool RebalancingTime(DateTime date)
        {
            return date.DayOfWeek == DayOfWeek.Monday;
        }

        public static double riskyAssetsValue(Dictionary<string, double> sharesDelta, double[] spots)
        {
            double sumOfQuantities = 0;
            int counter = 0;
            foreach (var delta in sharesDelta.Values)
            {
                sumOfQuantities += delta * spots[counter];
                counter++;
            }
            return sumOfQuantities;
        }

        public static Dictionary<string, double> UpdateCompo(TestParameters testParameters, DataFeed dataFeed)
        {
            Pricer pricer = new Pricer(testParameters);
            double timeToMaturity = MathDateConverter.ConvertToMathDistance(dataFeed.Date, testParameters.BasketOption.Maturity);
            PricingResults priceResult = pricer.Price(timeToMaturity, SpotsArray(dataFeed));
            Dictionary<string, double> resultedUpdate = new Dictionary<string, double>();
            int counter = 0;
            foreach(var shareId in dataFeed.PriceList.Keys)
            {
                resultedUpdate.Add(shareId, priceResult.Deltas[counter]);
                counter++;
            }
            return resultedUpdate;
        }        
    }
}