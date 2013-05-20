using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxCalculator
{
    public struct TaxStep : IComparable
    {
        private UInt32 _lowerBound;
        private UInt32 _upperBound;
        private Byte _taxPercent;
        private UInt32 _fullTax;
        public UInt32 LowerBound { get { return _lowerBound; } }   
        public UInt32 UpperBound { get { return _upperBound; } }
        public Byte TaxPercent { get { return _taxPercent; } }
        public UInt32 FullTax { get { return _fullTax; } }

        public override int GetHashCode()
        {
            return (int) (LowerBound / TaxManager.MajorStepsBound);
        }

        public TaxStep(UInt32 lower, UInt32 higher, Byte percent)
        {
            // higher MIGHT BE 0, considered as infinity or, assuming that no one earns more than 2^32, UInt32.MaxValue.
            if (lower >= higher && higher != 0)
            {
                throw new ConfigurationErrorsException("Tax step's upper bound must be larger than its lower bound.");
            }
            else if (lower < 1)
            {
                throw new ConfigurationErrorsException("Tax step's lower bound must be larger than 0.");
            }
            else if (percent < 0 || percent > 100)
            {
                throw new ConfigurationErrorsException("Tax step's percent must be between 0 and 100 (but not smaller or larger than those, respectfuly).");
            }

            _lowerBound = lower - 1;
            _upperBound = higher;
            _taxPercent = percent;

            if (_upperBound != 0)
            {
                _fullTax = (_upperBound - _lowerBound) * _taxPercent / 100;
            }
            else
            {
                _fullTax = 0;
            }
        }

        public Single CalcStepForSalary(UInt32 salary)
        {
            if (UpperBound == 0 || salary < UpperBound)
            {   //upper bound is either "infinity" or higher than the salary, calculate the relative part
                return ((salary - LowerBound) * TaxPercent) / 100.0f;
            }
            
            //upper bound is smaller than the salary, return the entire tax step.
            return this.FullTax;
        }

        public int CompareTo(object obj)
        {
            TaxStep other = (TaxStep)obj;
            return (int)(this.LowerBound - other.LowerBound);
        }
    }
}