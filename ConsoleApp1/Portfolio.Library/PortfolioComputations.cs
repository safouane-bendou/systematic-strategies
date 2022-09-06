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
        public static double[] SpotsArray(Dictionary<string, double> PriceList)
        {
            int numberOfShares = PriceList.Count;
            double[] allSpots = new double[numberOfShares];
            for (int i = 0; i < PriceList.Count; i++)
            {
                double spot = PriceList.ElementAt(i).Value;
                allSpots[i] = spot;
            }
            return allSpots;
        }

        public static double riskyAssetsValue(Dictionary<string, double> composition, Dictionary<string, double> spots)
        {
            double sumOfQuantities = 0;
            int counter = 0;
            foreach (var shareId in composition.Keys)
            {
                sumOfQuantities += composition[shareId] * spots[shareId];
                counter++;
            }
            return sumOfQuantities;
        }

        public static void PortfolioValues(Portfolio portfolio, List<DataFeed> marketData, Pricer pricer, DateTime optionMaturity)
        {
            //premium computing
            DataFeed initialDataFeed = marketData[0];
            portfolio.UpdateCompo(initialDataFeed, optionMaturity, pricer);
            List<double> resultingPortfolio = new List<double>() { portfolio.Value };
            foreach (DataFeed dataFeed in marketData.Skip(1))
            {
                portfolio.UpdatingPortfolio(dataFeed);
                //Don't forget the rebalancing ; discarded for now : if Rebalancing()
                portfolio.UpdateCompo(dataFeed, optionMaturity, pricer);
            }
            //return portfolio;
        }
    }
}