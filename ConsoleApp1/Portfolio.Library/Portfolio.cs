using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PricingLibrary.DataClasses;
using PricingLibrary.MarketDataFeed;

using Portfolio.Library;


namespace Portfolio.Library
{
    public class PortfolioInit
    {
        Dictionary<string, double> sharesDelta;
        double capitalisationRiskFree = 1;
        double premium;

        public List<double> PortfolioValues(TestParameters testParameters, List<DataFeed> marketData)
        {
            //premium computing
            DataFeed initialDataFeed = marketData[0];
            DateTime previousDate = initialDataFeed.Date;
            double[] initialSpots = PortfolioComputations.SpotsArray(initialDataFeed);
            sharesDelta = PortfolioComputations.UpdateCompo(testParameters, initialDataFeed);
            List<double> portfolio = new List<double>() { premium };
            //portfolio.Add(premium);
            foreach (DataFeed dataFeed in marketData)
            {
                if (!dataFeed.Equals(initialDataFeed))
                {
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
            }
            return portfolio;
        }
     
    }
}
