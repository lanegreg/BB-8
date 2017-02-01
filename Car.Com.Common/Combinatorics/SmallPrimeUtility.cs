using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Car.Com.Common.Combinatorics
{
	public class SmallPrimeUtility
	{
		private SmallPrimeUtility() { }

		public static List<int> Factor(int i)
		{
			var primeIndex = 0;
			var prime = PrimeTable[primeIndex];
			var factors = new List<int>();
			while (i > 1)
			{
				if (i%prime == 0)
				{
					factors.Add(prime);
					i /= prime;
				}
				else
				{
					++primeIndex;
					prime = PrimeTable[primeIndex];
				}
			}
			return factors;
		}

		public static List<int> MultiplyPrimeFactors(IList<int> lhs, IList<int> rhs)
		{
			var product = lhs.ToList();
			product.AddRange(rhs);
			product.Sort();

			return product;
		}

		public static List<int> DividePrimeFactors(IList<int> numerator, IList<int> denominator)
		{
			var product = numerator.ToList();
			
			foreach (var prime in denominator)
				product.Remove(prime);
			
			return product;
		}

		public static long EvaluatePrimeFactors(IList<int> value)
		{
			return value.Aggregate<int, long>(1, (current, prime) => current*prime);
		}

		static SmallPrimeUtility()
		{
			CalculatePrimes();
		}

		private static void CalculatePrimes()
		{
			var sieve = new BitArray(65536, true);
			for (var possiblePrime = 2; possiblePrime <= 256; ++possiblePrime)
				if (sieve[possiblePrime])
					for (var nonPrime = 2*possiblePrime; nonPrime < 65536; nonPrime += possiblePrime)
						sieve[nonPrime] = false;

			_myPrimes = new List<int>();
			for (var i = 2; i < 65536; ++i)
				if (sieve[i])
					_myPrimes.Add(i);
		}

		public static IList<int> PrimeTable
		{
			get { return _myPrimes; }
		}

		private static List<int> _myPrimes = new List<int>();
	}
}