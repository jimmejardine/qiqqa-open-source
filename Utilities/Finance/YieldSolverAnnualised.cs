using System;

namespace Utilities.Finance
{
    public class YieldSolverAnnualised : YieldSolver
    {
        protected override double PV(double irr)
        {   
            double PV = 0;
            foreach (var cashflow in cashflows)
            {
                double discount_factor = Math.Pow(1.0 / (1.0 + irr), cashflow.years);
                PV += cashflow.amount * discount_factor;
            }

            return PV;
        }

        protected override double MinYield()
        {
            return -0.99999999;
        }
    }
}
