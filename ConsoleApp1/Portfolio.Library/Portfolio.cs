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
        public Dictionary<string, double> Composition { get; set; }
        public double Value { get; set; }
        public DateTime CurrentDate { get; set; }


        public Portfolio(Dictionary<string, double> composition, double value, DateTime currentDate)
        {
            Composition = composition;
            Value = value;
            CurrentDate = currentDate;
        }

        public void UpdatingPortfolio(DataFeed dataFeed)
        {
            double capitalisationRiskFree = RiskFreeRateProvider.GetRiskFreeRateAccruedValue(CurrentDate, dataFeed.Date);
            Value = (Value - PortfolioComputations.riskyAssetsValue(Composition, dataFeed.PriceList)) * capitalisationRiskFree + PortfolioComputations.riskyAssetsValue(Composition, dataFeed.PriceList);
            CurrentDate = dataFeed.Date;
        }

        public void UpdateCompo(DataFeed dataFeed, Pricer pricer, DateTime optionMaturity)
        {
            double timeToMaturity = MathDateConverter.ConvertToMathDistance(dataFeed.Date, optionMaturity);
            PricingResults priceResult = pricer.Price(timeToMaturity, PortfolioComputations.SpotsArray(dataFeed.PriceList));
            Dictionary<string, double> resultedUpdate = new Dictionary<string, double>();
            int count = 0;
            foreach (var shareId in dataFeed.PriceList.Keys)
            {
                Composition[shareId] = priceResult.Deltas[count];
                count++;
            }
        }
    }
}
