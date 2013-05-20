using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxCalculator
{
    public class TaxManager
    {
        public static UInt32 MajorStepsBound;
        private static String _taxPrefix;
        private static TaxStep[] _taxSteps;
        private static int _steps;

        public TaxManager()
        {
            InitializeConfiguration();

            Array.Sort(_taxSteps); //sort the steps ascending to be able to calculate the progressive sum

            UInt32 lower;
            UInt32 upper;

            lower = upper = 0;

            for (int i = 0; i < _steps; i++)
            {
                if (_taxSteps[i].LowerBound < upper)
                {
                    throw new ConfigurationErrorsException(String.Format("tax step lower bound {0} must be at least as large as the last upper bound {1}", _taxSteps[i].LowerBound, upper));
                }
                lower = _taxSteps[i].LowerBound;
                upper = _taxSteps[i].UpperBound;
            }
        }

        private static void InitializeConfiguration()
        {
            //parse configuration
            string dividor = ConfigurationManager.AppSettings.Get("MajorStepsBound");
            _taxPrefix = ConfigurationManager.AppSettings.Get("TaxPrefix");

            if (dividor == null || _taxPrefix == null)
            {
                throw new ConfigurationErrorsException("MajorStepsBound and TaxPrefix keys must exist in the configuration.");
            }
            if (UInt32.TryParse(dividor, out MajorStepsBound) != true)
            {
                throw new ConfigurationErrorsException("MajorStepsBound's value must be an UInt32 number.");
            }

            foreach (string key in ConfigurationManager.AppSettings.AllKeys)
            {
                if (key.StartsWith(_taxPrefix))
                {
                    _steps++;
                }
            }
            _taxSteps = new TaxStep[_steps]; //allocate a sufficient amount of steps

            int i = 0;
            UInt32 lower;
            UInt32 upper;
            Byte percent;

            foreach (string key in ConfigurationManager.AppSettings.AllKeys) //parse each tax step from the configuration
            {
                if (key.StartsWith(_taxPrefix))
                {
                    String[] parts = key.Substring(_taxPrefix.Length).Split(',');
                    if (parts.Length > 2)
                    {
                        throw new ConfigurationErrorsException("Tax key could contain at most two numbers (lower then higher) separated by comma.");
                    }

                    if (UInt32.TryParse(parts[0], out lower) != true || UInt32.TryParse(parts[1], out upper) != true)
                    {
                        throw new ConfigurationErrorsException("Failed parsing tax lower and upper bound.");
                    }

                    if (Byte.TryParse(ConfigurationManager.AppSettings[key], out percent) != true)
                    {
                        throw new ConfigurationErrorsException(String.Format("Failed parsing tax percent for {0} step.", key));
                    }

                    _taxSteps[i] = new TaxStep(lower, upper, percent);
                    i++;
                }
            }
        }

        public Single CalcTax(UInt32 brutoSalary)
        {
            Single sum = 0;

            for (int i = 0; i < _steps; i++)
            {
                if (_taxSteps[i].LowerBound > brutoSalary)
                    break;

                sum += _taxSteps[i].CalcStepForSalary(brutoSalary);
            }
            return sum;
        }
    }
}