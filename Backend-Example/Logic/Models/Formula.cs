namespace Backend_Example.Logic.Classes
{
    public class Formula
    {
        public double CalculateFormula(double x, double mS)
        {
            double lD = mS % 10 + 1;
            double term1 =
                Math.Cos(0.345 * x)
                * Math.Sin(1 / ((mS + 1) / 52) * x)
                * Math.Pow(Math.Sin(0.1134 * x), 2)
                * 0.2;
            double term2 = Math.Abs(Math.Pow(Math.Cos(0.356 * x), 3));
            double term3 =
                Math.Abs(Math.Sin(1.0 / ((mS + 1) / 1.23) * x) * (mS / 30))
                + Math.Sin(176 * x) * 0.345
                + Math.Cos(1320 * x) * (0.15165 + lD / 70)
                + Math.Cos(820 * x) * (0.0461644 + lD / 50)
                + Math.Cos((620 + mS * 2) * x) * 0.1134
                + Math.Cos((260 + lD * 5) * x) * 0.09456
                + Math.Sin(1760 * x) * 0.0345
                + Math.Cos(13200 * x) * (0.015165 + lD / 69)
                + Math.Cos(8200 * x) * (0.0461644 + lD / 93)
                + Math.Cos((620 + mS * 20) * x) * 0.01134
                + Math.Cos((260 + lD * 50) * x) * 0.09456;
            /*Multiple waves a day*/
            double term4 =
                Math.Sin(1.0 / ((mS + 1) / 42) * x * 1.4)
                + Math.Cos(10.94 * x) * 0.75
                + Math.Cos(15.43 * x) * 0.435
                + Math.Cos(100 * x) * 0.193
                + Math.Sin(23 * x) * 0.5275;
            double term5 =
                Math.Sin(0.00229 * x) * (lD + 1) / 85
                + Math.Cos(0.003343 * x) * (lD + 1) / 13
                + Math.Sin(0.00505 * x) * (lD + 1) / 19
                + Math.Cos(0.00343 * x) * (lD + 1) / 63;
            double term6 =
                Math.Cos(0.00291 * x) * 21
                + Math.Cos(0.0012 * x) * (10.654 + lD)
                + Math.Cos(0.000826 * x) * (30 - mS / 10)
                + Math.Cos(0.001176 * x) * (10 + mS / 10)
                + Math.Abs(Math.Cos(1.3438 * x) * 1.254)
                + Math.Abs(Math.Cos(1.9438 * x) * 2.154)
                + Math.Abs(Math.Cos(1.5438 * x) * 1.054)
                + Math.Abs(Math.Cos(1.2438 * x) * 1.954)
                + Math.Abs(Math.Cos(1.0438 * x) * 2.654);
            double term7 = mS / 1589.0 * x;
            double term8 =
                Math.Sin((0.19 + mS / 300) * x) * lD * -0.9
                + Math.Cos((0.07343 + mS / 500) * x) * mS / 15
                + Math.Sin((0.105 + mS / 1000) * x) * lD
                + Math.Cos((0.02343 + mS / 2000) * x) * lD;

            return term1 + term2 + term3 + term4 + term5 + term6 + term7 + term8 + 0.3 * mS;
        }
    }
}
