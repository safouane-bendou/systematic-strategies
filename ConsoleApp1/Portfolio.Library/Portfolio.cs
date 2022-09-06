using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PricingLibrary.Computations;
using PricingLibrary.DataClasses;
using PricingLibrary.MarketDataFeed;
using PricingLibrary.TimeHandler;


using Portfolio.Library;


namespace Portfolio.Library
{
    public class Portfolio
    {
        Dictionary<string, double> sharesDelta { get; set; }
        double capitalisationRiskFree { get; set; }
        double premium { get; set; }

        public List<double> PortfolioValues(TestParameters testParameters, List<DataFeed> marketData)
        {
            //premium computing
            DataFeed initialDataFeed = marketData[0];
            DateTime previousDate = initialDataFeed.Date;
            double[] initialSpots = PortfolioComputations.SpotsArray(initialDataFeed);
            sharesDelta = PortfolioComputations.UpdateCompo(testParameters, initialDataFeed);
            List<double> portfolio = new List<double>() { premium };
            //portfolio.Add(premium);
            foreach (DataFeed dataFeed in marketData.Skip(1))
            {
                //if (!dataFeed.Equals(initialDataFeed))
                double[] spots = PortfolioComputations.SpotsArray(dataFeed);
                capitalisationRiskFree = RiskFreeRateProvider.GetRiskFreeRateAccruedValue(previousDate, dataFeed.Date);
                double portfolioValue = (premium - PortfolioComputations.riskyAssetsValue(sharesDelta, initialSpots)) * capitalisationRiskFree + PortfolioComputations.riskyAssetsValue(sharesDelta, spots);

                //Don't forget the rebalancing ; discarded for now : if Rebalancing(dataFeed.Date)
                sharesDelta = PortfolioComputations.UpdateCompo(testParameters, dataFeed);
                //rebalancing done

                initialSpots = spots;
                previousDate = dataFeed.Date;
                premium = portfolioValue;
                portfolio.Add(portfolioValue);
            }
            return portfolio;
        }


        public static Dictionary<string, double> UpdateCompo(TestParameters testParameters, DataFeed dataFeed)
        {
            Pricer pricer = new Pricer(testParameters);
            double timeToMaturity = MathDateConverter.ConvertToMathDistance(dataFeed.Date, testParameters.BasketOption.Maturity);
            PricingResults priceResult = pricer.Price(timeToMaturity, SpotsArray(dataFeed));
            Dictionary<string, double> resultedUpdate = new Dictionary<string, double>();
            int counter = 0;
            foreach (var shareId in dataFeed.PriceList.Keys)
            {
                resultedUpdate.Add(shareId, priceResult.Deltas[counter]);
                counter++;
            }
            return resultedUpdate;
        }

    }
}
