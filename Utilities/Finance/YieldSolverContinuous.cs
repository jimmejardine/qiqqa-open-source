using System;

namespace Utilities.Finance
{
    public class YieldSolverContinuous : YieldSolver
    {
        protected override double PV(double irr)
        {
            double irr_neg = -irr;

            double PV = 0;
            foreach (var cashflow in cashflows)
            {
                PV += cashflow.amount * Math.Exp(cashflow.years * irr_neg);
            }

            return PV;
        }

        protected override double MinYield()
        {
            return Double.MinValue;
        }
    }
}

