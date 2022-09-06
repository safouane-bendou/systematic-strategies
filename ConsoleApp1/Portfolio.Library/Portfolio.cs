using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PricingLibrary.Computations;
using PricingLibrary.DataClasses;
using PricingLibrary.MarketDataFeed;
using PricingLibrary.TimeHandler;


using PortfolioLibrary;


namespace PortfolioLibrary
{
    public class Portfolio
    {
        public Dictionary<string, double> Composition { get; set; }
        public double Value { get; set; }
        public DateTime CurrentDate { get; set; }


        public Portfolio(Dictionary<string, double> composition, double value, DateTime currentDate)
        {
            Composition = composition;
            Value = value;
            CurrentDate = currentDate;
        }


        //update que les deltas
        public void UpdateCompo(Dictionary<string, double> newComposition)
        {
            Composition = newComposition;
        }

        //update les autres
        public void UpdatingPortfolio(DataFeed dataFeed, Dictionary<string, double> assets)
        {
            double capitalisationRiskFree = RiskFreeRateProvider.GetRiskFreeRateAccruedValue(CurrentDate, dataFeed.Date);
            Value = (Value - PortfolioComputations.RiskyAssetsValue(Composition, assets)) * capitalisationRiskFree + PortfolioComputations.RiskyAssetsValue(Composition, dataFeed.PriceList);
            CurrentDate = dataFeed.Date;
        }

        
    }
}
