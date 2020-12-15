using System;

namespace ValorantEngine
{
    public class Maths
    {
        public static double getSpeed()
        {
            Random random = new Random();
            double randomSpeed = random.NextDouble() * 0.05;
            return randomSpeed;
        }
    }
}
