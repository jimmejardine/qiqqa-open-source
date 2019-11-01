using System;

namespace Utilities.Mathematics.Statistics.Distributions
{
    // NB: This class is a direct translation of Graeme West's c++ code
    // Up for review
    // *** XXX REVIEW

    public class GenzMultivariateClosed
    {
        private const double epsilon = 1E-15;
        private const double invsqr2pi = 0.398942280401433;

        #region --- Test ------------------------------------------------------------------------

#if TEST
		public static void TestHarness()
		{
			Console.WriteLine("{0} is {1}", cumnorm(0.4)*cumnorm(0.6), bivarcumnorm(0.4, 0.6, 0));
		}
#endif

        #endregion

        public static double cumnorm(double x)
        {
            double cn;

            double xabs = Math.Abs(x);
            if (xabs > 37)
            {
                cn = 0;
            }
            else
            {
                double exponential = Math.Exp(-Math.Pow(xabs, 2) / 2);
                if (xabs < 7.07106781186547)
                {
                    double build = 3.52624965998911E-02 * xabs + 0.700383064443688;
                    build = build * xabs + 6.37396220353165;
                    build = build * xabs + 33.912866078383;
                    build = build * xabs + 112.079291497871;
                    build = build * xabs + 221.213596169931;
                    build = build * xabs + 220.206867912376;
                    cn = exponential * build;
                    build = 8.83883476483184E-02 * xabs + 1.75566716318264;
                    build = build * xabs + 16.064177579207;
                    build = build * xabs + 86.7807322029461;
                    build = build * xabs + 296.564248779674;
                    build = build * xabs + 637.333633378831;
                    build = build * xabs + 793.826512519948;
                    build = build * xabs + 440.413735824752;
                    cn = cn / build;
                }
                else
                {
                    double build = xabs + 0.65;
                    build = xabs + 4.0 / build;
                    build = xabs + 3.0 / build;
                    build = xabs + 2.0 / build;
                    build = xabs + 1.0 / build;
                    cn = exponential / build / 2.506628274631;
                }
            }

            if (x > 0)
            {
                cn = 1 - cn;
            }
            return cn;
        }



        public static double bivarcumnorm(double x, double y, double correlation)
        {
            double[,] XX = new double[11, 4];
            double[,] W = new double[11, 4];

            W[1, 1] = 0.17132449237917;
            XX[1, 1] = -0.932469514203152;
            W[2, 1] = 0.360761573048138;
            XX[2, 1] = -0.661209386466265;
            W[3, 1] = 0.46791393457269;
            XX[3, 1] = -0.238619186083197;
            W[1, 2] = 4.71753363865118E-02;
            XX[1, 2] = -0.981560634246719;
            W[2, 2] = 0.106939325995318;
            XX[2, 2] = -0.904117256370475;
            W[3, 2] = 0.160078328543346;
            XX[3, 2] = -0.769902674194305;
            W[4, 2] = 0.203167426723066;
            XX[4, 2] = -0.587317954286617;
            W[5, 2] = 0.233492536538355;
            XX[5, 2] = -0.36783149899818;
            W[6, 2] = 0.249147045813403;
            XX[6, 2] = -0.125233408511469;
            W[1, 3] = 1.76140071391521E-02;
            XX[1, 3] = -0.993128599185095;
            W[2, 3] = 4.06014298003869E-02;
            XX[2, 3] = -0.963971927277914;
            W[3, 3] = 6.26720483341091E-02;
            XX[3, 3] = -0.912234428251326;
            W[4, 3] = 8.32767415767048E-02;
            XX[4, 3] = -0.839116971822219;
            W[5, 3] = 0.10193011981724;
            XX[5, 3] = -0.746331906460151;
            W[6, 3] = 0.118194531961518;
            XX[6, 3] = -0.636053680726515;
            W[7, 3] = 0.131688638449177;
            XX[7, 3] = -0.510867001950827;
            W[8, 3] = 0.142096109318382;
            XX[8, 3] = -0.37370608871542;
            W[9, 3] = 0.149172986472604;
            XX[9, 3] = -0.227785851141645;
            W[10, 3] = 0.152753387130726;
            XX[10, 3] = -7.65265211334973E-02;

            int NG;
            int LG;

            if (Math.Abs(correlation) < 0.3)
            {
                NG = 1;
                LG = 3;
            }
            else if (Math.Abs(correlation) < 0.75)
            {
                NG = 2;
                LG = 6;
            }
            else
            {
                NG = 3;
                LG = 10;
            }

            double h = -x;
            double k = -y;
            double hk = h * k;
            double BVN = 0;

            if (Math.Abs(correlation) < 0.925)
            {
                if (Math.Abs(correlation) > 0)
                {
                    double hs = (h * h + k * k) / 2;
                    double asr = Math.Asin(correlation);
                    for (int i = 1; i <= LG; ++i)
                    {
                        for (int j = -1; j <= 1; j = j + 2)
                        {
                            double sn = Math.Sin(asr * (j * XX[i, NG] + 1) / 2);
                            BVN = BVN + W[i, NG] * Math.Exp((sn * hk - hs) / (1 - sn * sn));
                        }
                    }
                    BVN = BVN * asr / (4 * Math.PI);
                }
                BVN = BVN + cumnorm(-h) * cumnorm(-k);
            }
            else
            {
                if (correlation < 0)
                {
                    k *= -1;
                    hk *= -1;
                }
                if (Math.Abs(correlation) < 1)
                {
                    double Ass = (1 - correlation) * (1 + correlation);
                    double a = Math.Sqrt(Ass);
                    double bs = Math.Pow(h - k, 2);
                    double c = (4 - hk) / 8;
                    double d = (12 - hk) / 16;
                    double asr = -(bs / Ass + hk) / 2;
                    if (asr > -100)
                    {
                        BVN = a * Math.Exp(asr) * (1 - c * (bs - Ass) * (1 - d * bs / 5) / 3 + c * d * Ass * Ass / 5);
                    }
                    if (-hk < 100)
                    {
                        double B = Math.Sqrt(bs);
                        BVN = BVN - Math.Exp(-hk / 2) * Math.Sqrt(2 * Math.PI) * cumnorm(-B / a) * B * (1 - c * bs * (1 - d * bs / 5) / 3);
                    }
                    a /= 2;



                    for (int i = 1; i <= LG; ++i)
                    {
                        for (int j = -1; i <= 1; j += 2)
                        {
                            double xs = Math.Pow(a * (j * XX[i, NG] + 1), 2);
                            double rs = Math.Sqrt(1 - xs);
                            asr = -(bs / xs + hk) / 2;
                            if (asr > -100)
                            {
                                BVN = BVN + a * W[i, NG] * Math.Exp(asr) * (Math.Exp(-hk * (1 - rs) / (2 * (1 + rs))) / rs - (1 + c * xs * (1 + d * xs)));
                            }
                        }
                    }
                    BVN *= -1 / (2 * Math.PI);
                }
                if (correlation > 0)
                {
                    BVN = BVN + cumnorm(-Math.Max(h, k));
                }
                else
                {
                    BVN *= -1;
                    if (k > h)
                    {
                        BVN = BVN + cumnorm(k) - cumnorm(h);
                    }
                }
            }
            return BVN;
        }

        private static double pntgnd(double ba, double bb, double bc, double ra, double rb, double r, double rr)
        {
            //Computes Plackett formula integrand
            double pnt = 0;

            double dt = rr * (rr - Math.Pow(ra - rb, 2) - 2 * ra * rb * (1 - r));
            if (dt > 0)
            {
                double bt = (bc * rr + ba * (r * rb - ra) + bb * (r * ra - rb)) / Math.Sqrt(dt);
                double ft = Math.Pow(ba - r * bb, 2) / rr + Math.Pow(bb, 2);
                if (bt > -10 && ft < 100)
                {
                    pnt = Math.Exp(-ft / 2);
                    if (bt < 10)
                    {
                        pnt = pnt * cumnorm(bt);
                    }
                }
            }

            return pnt;
        }

        private static double Tvtmfn(double x, double l1, double l2, double l3, double correl21, double correl31, double correl32)
        {
            //Computes Plackett formula integrands
            double TV = 0;
            double rua = Math.Asin(correl21);
            double rub = Math.Asin(correl31);
            double[] y = new double[5];
            y[1] = Math.Sin(rua * x);
            y[2] = Math.Pow(Math.Cos(rua * x), 2);
            y[3] = Math.Sin(rub * x);
            y[4] = Math.Pow(Math.Cos(rub * x), 2);
            if (Math.Abs(rua) > 0)
            {
                TV = rua * pntgnd(l1, l2, l3, y[3], correl32, y[1], y[2]);
            }
            if (Math.Abs(rub) > 0)
            {
                TV = TV + rub * pntgnd(l1, l3, l2, y[1], correl32, y[3], y[4]);
            }
            return TV;
        }

        private static void Kronrod(double lower, double upper, double l1, double l2, double l3, double correl21, double correl31, double correl32, double[] WG, double[] WGK, double[] XGK, out double Kronrodint, out double Kronroderr)
        {
            //Kronrod Rule on interval [lower,upper]
            double Halflength = (upper - lower) / 2;
            double Centre = (lower + upper) / 2;
            double fc = Tvtmfn(Centre, l1, l2, l3, correl21, correl31, correl32);
            double Resltg = fc * WG[0];
            double Resltk = fc * WGK[0];
            int vector;
            for (vector = 1; vector <= 11; ++vector)
            {
                double Abscis = Halflength * XGK[vector];
                double FunSum = Tvtmfn(Centre - Abscis, l1, l2, l3, correl21, correl31, correl32) + Tvtmfn(Centre + Abscis, l1, l2, l3, correl21, correl31, correl32);
                Resltk = Resltk + WGK[vector] * FunSum;
                if (0 == vector % 2)
                {
                    Resltg = Resltg + WG[vector / 2] * FunSum;
                }
            }
            Kronrodint = Resltk * Halflength;
            Kronroderr = Math.Abs(Resltg - Resltk) * Halflength;
        }

        public static double trivarcumnorm(double x, double y, double z, double correl12, double correl13, double correl23)
        {
            double[] WG = new double[6];
            double[] WGK = new double[12];
            double[] XGK = new double[12];

            WG[0] = 0.272925086777901;
            WG[1] = 5.56685671161745E-02;
            WG[2] = 0.125580369464905;
            WG[3] = 0.186290210927735;
            WG[4] = 0.233193764591991;
            WG[5] = 0.262804544510248;
            WGK[0] = 0.136577794711118;
            WGK[1] = 9.76544104596129E-03;
            WGK[2] = 2.71565546821044E-02;
            WGK[3] = 4.58293785644267E-02;
            WGK[4] = 6.30974247503748E-02;
            WGK[5] = 7.86645719322276E-02;
            WGK[6] = 9.29530985969007E-02;
            WGK[7] = 0.105872074481389;
            WGK[8] = 0.116739502461047;
            WGK[9] = 0.125158799100319;
            WGK[10] = 0.131280684229806;
            WGK[11] = 0.135193572799885;
            XGK[0] = 0.0;
            XGK[1] = 0.996369613889543;
            XGK[2] = 0.978228658146057;
            XGK[3] = 0.941677108578068;
            XGK[4] = 0.887062599768095;
            XGK[5] = 0.816057456656221;
            XGK[6] = 0.730152005574049;
            XGK[7] = 0.630599520161965;
            XGK[8] = 0.519096129206812;
            XGK[9] = 0.397944140952378;
            XGK[10] = 0.269543155952345;
            XGK[11] = 0.136113000799362;


            double TVCN;
            double[] limit = new double[4];
            limit[1] = x;
            limit[2] = y;
            limit[3] = z;
            double[,] correlation = new double[4, 4];
            correlation[2, 1] = correl12;
            correlation[3, 1] = correl13;
            correlation[3, 2] = correl23;

            if (Math.Abs(correlation[2, 1]) > Math.Abs(correlation[3, 1]))
            {
                limit[2] = z;
                limit[3] = y;
                correlation[2, 1] = correl13;
                correlation[3, 1] = correl12;
            }
            if (Math.Abs(correlation[3, 1]) > Math.Abs(correlation[3, 2]))
            {
                limit[1] = limit[2];
                limit[2] = x;
                correlation[3, 2] = correlation[3, 1];
                correlation[3, 1] = correl23;
            }
            TVCN = 0;

            if (Math.Abs(limit[1]) + Math.Abs(limit[2]) + Math.Abs(limit[3]) < epsilon)
            {
                TVCN = (1 + (Math.Asin(correlation[2, 1]) + Math.Asin(correlation[3, 1]) + Math.Asin(correlation[3, 2])) / Math.Asin(1.0)) / 8;
            }
            else if (Math.Abs(correlation[2, 1]) + Math.Abs(correlation[3, 1]) < epsilon)
            {
                TVCN = cumnorm(limit[1]) * bivarcumnorm(limit[2], limit[3], correlation[3, 2]);
            }
            else if (Math.Abs(correlation[3, 1]) + Math.Abs(correlation[3, 2]) < epsilon)
            {
                TVCN = cumnorm(limit[3]) * bivarcumnorm(limit[1], limit[2], correlation[2, 1]);
            }
            else if (Math.Abs(correlation[2, 1]) + Math.Abs(correlation[3, 2]) < epsilon)
            {
                TVCN = cumnorm(limit[2]) * bivarcumnorm(limit[1], limit[3], correlation[3, 1]);
            }
            else if (1 - correlation[3, 2] < epsilon)
            {
                TVCN = bivarcumnorm(limit[1], Math.Min(limit[2], limit[3]), correlation[2, 1]);
            }
            else if (correlation[3, 2] + 1 < epsilon)
            {
                if (limit[2] > -limit[3])
                {
                    TVCN = bivarcumnorm(limit[1], limit[2], correlation[2, 1]) - bivarcumnorm(limit[1], -limit[3], correlation[2, 1]);
                }
                else
                {
                    TVCN = 0;
                }
            }
            else
            {
                TVCN = bivarcumnorm(limit[2], limit[3], correlation[3, 2]) * cumnorm(limit[1]);
                double[] A = new double[10];
                double[] B = new double[10];
                double[] kf = new double[10];
                double[] ke = new double[10];
                int k = 1;
                int j = 1;
                A[1] = 0.0;
                B[1] = 1.0;
                double Kronrodint;
                double Kronroderr;
                double Err;
                double Fin;
                int i;

                do
                {
                    j = ++j;
                    B[j] = B[k];
                    A[j] = (A[k] + B[k]) / 2;
                    B[k] = A[j];
                    Kronrod(A[k], B[k], limit[1], limit[2], limit[3], correlation[2, 1], correlation[3, 1], correlation[3, 2], WG, WGK, XGK, out Kronrodint, out Kronroderr);
                    kf[k] = Kronrodint;
                    ke[k] = Kronroderr;
                    Kronrod(A[j], B[j], limit[1], limit[2], limit[3], correlation[2, 1], correlation[3, 1], correlation[3, 2], WG, WGK, XGK, out Kronrodint, out Kronroderr);
                    kf[j] = Kronrodint;
                    ke[j] = Kronroderr;
                    Err = 0;
                    Fin = 0;
                    for (i = 1; i <= j; ++i)
                    {
                        if (ke[i] > ke[k])
                        {
                            k = i;
                        }
                        Fin = Fin + kf[i];
                        Err = Err + Math.Pow(ke[i], 2);
                    }
                    Err = Math.Sqrt(Err);
                }
                while (4 * Err > epsilon && j < 10);
                TVCN = TVCN + Fin / (4 * Math.Asin(1.0));
            }
            return TVCN;
        }

    }
}
