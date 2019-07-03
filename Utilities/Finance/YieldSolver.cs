using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities.Collections;

namespace Utilities.Finance
{
    public abstract class YieldSolver
    {
        public class Cashflow
        {
            public double years;
            public double amount;

            public override string ToString()
            {
                return String.Format("{0:F4} \t{1}", years, amount);
            }
        }

        protected List<Cashflow> cashflows = new List<Cashflow>();

        public void AddCashflow(double years, double amount)
        {
            cashflows.Add(new Cashflow { years = years, amount = amount });
        }

        protected abstract double MinYield();

        public double SolveIRR()
        {
            // Start with bounds of 100% irr
            double irr_min = -0.99999999;
            double irr_max = +0.99999999;
            double pv_min = 0;
            double pv_max = 0;

            // Grow our bounds until we bracket zero in our PV
            for (int i = 0; i < 10; ++i)
            {
                pv_min = PV(irr_min);
                pv_max = PV(irr_max);
                if (pv_min * pv_max <= 0) break;

                irr_min *= 2;
                irr_max *= 2;

                if (irr_min < MinYield()) irr_min = MinYield();

                // We should not have to search this far - something is wrong...
                if (i > 6) return Double.NaN;
            }

            // Make sure our min and max irr are the correct way round w.r.t to their PV
            {
                if (pv_min > pv_max)
                {
                    Swap.swap(ref irr_min, ref irr_max);
                    Swap.swap(ref pv_min, ref pv_max);
                }
            }

            return SolveIRR(irr_min, pv_min, irr_max, pv_max, 0);
        }

        private static readonly double EPSILON = 0.0001;

        private double SolveIRR(double irr_min, double pv_min, double irr_max, double pv_max, int iteration)
        {
            if (iteration > 20) return Double.NaN;

            double irr = (irr_min + irr_max) / 2.0;
            if (irr_min - irr_max < EPSILON) return irr;
            
            double pv = PV(irr);
            if (0 == pv) return irr;

            if (false) { }
            else if (pv_min * pv < 0) return SolveIRR(irr_min, pv_min, irr, pv, iteration + 1);
            else if (pv_max * pv < 0) return SolveIRR(irr, pv, irr_max, pv_max, iteration + 1);

            // Should never get here
            return Double.NaN;
        }

        protected abstract double PV(double irr);

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var cashflow in cashflows)
            {
                sb.AppendLine(String.Format("{0}", cashflow));
            }
            return sb.ToString();
        }


        public SummingDictionary<double> GetTallies()
        {
            SummingDictionary<double> tallies = new SummingDictionary<double>();
            foreach (var cashflow in cashflows)
            {
                tallies.Add(cashflow.years, cashflow.amount);
            }
            return tallies;
        }

        public double WAL()
        {
            SummingDictionary<double> tallies = GetTallies();

            double num = 0;
            double den = 0;
            foreach (var tally in tallies)
            {
                if (tally.Value > 0)
                {
                    num += tally.Key * tally.Value;
                    den += tally.Value;
                }
            }

            return num / den;
        }
    }
}
