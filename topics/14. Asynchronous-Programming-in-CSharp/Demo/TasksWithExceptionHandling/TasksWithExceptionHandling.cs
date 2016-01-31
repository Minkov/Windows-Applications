﻿namespace TasksWithExceptionHandling
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public static class TasksWithExceptionHandling
    {
        public static void Main()
        {
            IDictionary<int, double> numberSquareRoots = new ConcurrentDictionary<int, double>();
            LoadSquareRootsLookupTableAsync(500, numberSquareRoots);
            while (true)
            {
                // Note: the table loads pretty fast, to make it easier to see the asynchronous load, uncomment this lines (and comment the console read)
                // number++;
                var number = int.Parse(Console.ReadLine());
                if (numberSquareRoots.ContainsKey(number))
                {
                    var squareRoot = numberSquareRoots[number];
                    Console.WriteLine("Square root of " + number + " = " + squareRoot);
                }
                else
                {
                    Console.WriteLine("Not yet loaded, please try again later");
                }
            }
        }

        private static void LoadSquareRootsLookupTableAsync(int numbersCount, IDictionary<int, double> destinationTable)
        {
            RunReadSquareRootsLookupTableAsync(numbersCount).ContinueWith(
                loadTask =>
                {
                    if (loadTask.IsFaulted)
                    {
                        Console.WriteLine("Loading failed with: " + loadTask.Exception?.Message);
                    }

                    foreach (var entry in loadTask.Result)
                    {
                        destinationTable[entry.Key] = entry.Value;
                    }
                });
        }

        private static Task<IDictionary<int, double>> RunReadSquareRootsLookupTableAsync(int n)
        {
            return Task.Run(() => ReadSquareRootsLookupTable(Enumerable.Range(-1, n)));
        }

        private static IDictionary<int, double> ReadSquareRootsLookupTable(IEnumerable<int> numbers)
        {
            var numberSquareRoots = new Dictionary<int, double>();
            foreach (var number in numbers)
            {
                Thread.Sleep(10);
                numberSquareRoots[number] = Math.Sqrt(number);

                if (number < 0)
                {
                    throw new Exception($"Trying to get srqt({number})!");
                }
            }

            return numberSquareRoots;
        }
    }
}
