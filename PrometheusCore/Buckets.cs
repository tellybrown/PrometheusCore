using System.Collections.Generic;

namespace PrometheusCore
{
    public static class Buckets
    {
        public static readonly double[] Default = { .005, .01, .025, .05, .075, .1, .25, .5, .75, 1, 2.5, 5, 7.5, 10 };
      
        public static double[] Linear(double start, double increment, int count)
        {
            List<double> ret = new List<double>();

            for (int i = 0; i < count; i++)
            {
                ret.Add(start);
                start += increment;
            }

            return ret.ToArray();
        }
        public static double[] Exponential(double start, double factor, int count)
        {
            List<double> ret = new List<double>();
            for (int i = 0; i < count; i++)
            {
                ret.Add(start);
                start *= factor;
            }

            return ret.ToArray();
        }
    }
}
