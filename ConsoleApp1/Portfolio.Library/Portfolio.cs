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

       

    }
}
