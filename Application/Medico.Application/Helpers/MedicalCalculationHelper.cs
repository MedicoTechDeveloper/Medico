using System;

namespace Medico.Application.Helpers
{
    public static class MedicalCalculationHelper
    {
        public static double CalculateBmi(double height, double weight)
        {
            return Math.Round(((weight * 703 / (height * height))), 0);
        }
    }
}