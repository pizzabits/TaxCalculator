using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxCalculator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            TaxManager manager = new TaxManager();
            Console.WriteLine("Input bruto salary: ");
            UInt32 salary;
            if (!UInt32.TryParse(Console.ReadLine(), out salary))
            {
                throw new ArgumentException("Parsing user input: bruto salary.");
            }
            Single sum = manager.CalcTax(salary);
            Console.WriteLine("Tax'd with {0} and the result salary is {1}", sum, salary - sum);
        }
    }
}
