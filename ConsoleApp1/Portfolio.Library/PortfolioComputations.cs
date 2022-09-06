using PricingLibrary.Computations;
using PricingLibrary.DataClasses;
using PricingLibrary.MarketDataFeed;
using PricingLibrary.TimeHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioLibrary
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

        public static double RiskyAssetsValue(Dictionary<string, double> composition, Dictionary<string, double> spots)
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

        public static Dictionary<string, double> ComputeNewComposition(DataFeed dataFeed, DateTime optionMaturity, Pricer pricer)
        {
            Dictionary<string, double> newComposition = new Dictionary<string, double>();
            double timeToMaturity = MathDateConverter.ConvertToMathDistance(dataFeed.Date, optionMaturity);
            PricingResults priceResult = pricer.Price(timeToMaturity, PortfolioComputations.SpotsArray(dataFeed.PriceList));
            int count = 0;
            foreach (var shareId in dataFeed.PriceList.Keys)
            {
                newComposition[shareId] = priceResult.Deltas[count];
                count++;
            }
            return newComposition;
        }

        public static double ComputePremium(DataFeed dataFeed, DateTime optionMaturity, Pricer pricer)
        {
            double timeToMaturity = MathDateConverter.ConvertToMathDistance(dataFeed.Date, optionMaturity);
            PricingResults priceResult = pricer.Price(timeToMaturity, PortfolioComputations.SpotsArray(dataFeed.PriceList));
            return priceResult.Price;
        }

        public static List<double> PortfolioValues(List<DataFeed> marketData, DateTime optionMaturity, Pricer pricer)
        {
            //creating a brand new portfolio, no need to update when just getting started
            DataFeed initialDataFeed = marketData[0];
            Dictionary<string, double> assets = initialDataFeed.PriceList;
            Dictionary<string, double> newComposition = ComputeNewComposition(initialDataFeed, optionMaturity, pricer);
            double premium = ComputePremium(initialDataFeed, optionMaturity, pricer);
            Portfolio portfolio = new Portfolio(newComposition, premium, initialDataFeed.Date);
            List<double> resultingPortfolioValues = new List<double>() { portfolio.Value };
            
            
            foreach (DataFeed dataFeed in marketData.Skip(1))
            {
                portfolio.UpdatingPortfolio(dataFeed, assets);
                //Don't forget the rebalancing ; discarded for now : if Rebalancing()
                portfolio.UpdateCompo(ComputeNewComposition(dataFeed, optionMaturity, pricer));

                resultingPortfolioValues.Add(portfolio.Value);
                assets = dataFeed.PriceList;

            }
            return resultingPortfolioValues;
        }
    }
}