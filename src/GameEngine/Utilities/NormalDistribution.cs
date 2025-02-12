using System;

namespace GameEngine.Utilities
{
    static class NormalDistribution
    {
        static readonly Random random = new Random();

        //Box-Muller transformation
        // https://mathworld.wolfram.com/Box-MullerTransformation.html
        public static double NextGaussian()
        {
            double u1 = 1.0 - random.NextDouble();
            double u2 = 1.0 - random.NextDouble();
            return Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
        }
    }
}
