using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ActiveObject.NET45
{
    public class PrimeActiveObject
    {
        public Task<IEnumerable<int>> CalculateAsync(int limit)
        {
            //var future = Task<IEnumerable<int>>.Factory.StartNew(() => Calculate(limit)); // More complex syntax than constructor + Start()
            var future = new Task<IEnumerable<int>>(() => Calculate(limit));
            future.Start();
            return future;
        }

        public IEnumerable<int> Calculate(int limit)
        {
            var primes = new List<int>();
            if (limit >= 2)
            {
                primes.Add(2);
                if (limit >= 3)
                {
                    for (var candidate = 3; candidate <= limit; candidate++)
                    {
                        var isPrime = IsPrime(candidate);

                        if (isPrime)
                        {
                            primes.Add(candidate);
                        }
                    }
                }
            }
            return primes;
        }


        private bool IsPrime(int candidate)
        {
            for (int divisor = 2; divisor <= candidate / 2; divisor++)
            {
                var dividend = candidate / (double)divisor;
                if (Math.Floor(dividend).Equals(dividend))
                {
                    return false;
                }
            }
            return true;
        }


    }

}
