using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PricingLibrary.RebalancingOracleDescriptions;
namespace PortfolioLibrary
{
    internal interface IRebalanceOracle
    {
        bool Rebalance(DateTime Date, DateTime IitialDate);
    }

    public class RegularOracle : IRebalanceOracle
    {
        int Period { get; set; }
        RebalancingOracleType Type { get; set; }

        public RegularOracle(int period, RebalancingOracleType type)
        {
            Period = period;
            Type = type;
        }

        public bool Rebalance(DateTime Date, DateTime InitialDate)
        {

            if ((Date - InitialDate).TotalDays % Period == 0)
            {
                return true;
            }
            return false;
        }
    }
    public class WeeklyOracle : IRebalanceOracle
    {
        DayOfWeek Day { get; set; }
        RebalancingOracleType Type { get; set; }

        public WeeklyOracle(DayOfWeek day, RebalancingOracleType type)
        {
            Day = day;
            Type = type;
        }

        public bool Rebalance(DateTime Date, DateTime InitialDate)
        {
            if (Date.DayOfWeek == Day)
            {
                return true;
            }
            return false;
        }
    }
}

