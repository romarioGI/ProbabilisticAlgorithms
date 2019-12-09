using System;
using System.Collections.Generic;
using System.Linq;
using ThreeColorGraphLib;

namespace ThreeColorGraphConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var edges = new List<(int, int)>();
            Console.WriteLine("Введите количество рёбер");
            var m = int.Parse(Console.ReadLine());
            Console.WriteLine("Введите {0} пар чисел - рёбра", m);
            for (var _ = 0; _ < m; _++)
            {
                var e = Console.ReadLine().Split(' ').Select(int.Parse).ToArray();
                edges.Add((e[0], e[1]));
            }

            Console.WriteLine("Введите количество итераций алгоритма");
            var it = int.Parse(Console.ReadLine());

            var ans = ThreeColorGraphAlgorithm.GetColors(edges, it);

            if (ans is null)
                Console.WriteLine("Решение не найдено");
            else
                foreach (var p in ans.OrderBy(x => x.Key))
                {
                    Console.WriteLine("вершина с номером {0} в цвет {1}", p.Key, p.Value);
                }

            Console.ReadKey();
        }
    }
}
