using System;
using System.Collections.Generic;
using System.Linq;

namespace ThreeColorGraphLib
{
    public static class ThreeColorGraphAlgorithm
    {
        private static readonly Random Rd = new Random((int) DateTime.Now.Ticks);

        public static Dictionary<int, int> GetColors(List<(int, int)> edges, int iterationsNumber)
        {
            var vertexes = new HashSet<int>();
            foreach (var (x, y) in edges)
            {
                vertexes.Add(x);
                vertexes.Add(y);
            }

            for (var i = 0; i < iterationsNumber; i++)
            {
                var banColor = GetBanColor(vertexes);
                var color = GetColor(edges, banColor);

                if (color is null == false)
                    return color;
            }

            return null;
        }

        private static Dictionary<int, int> GetBanColor(IEnumerable<int> vertexes)
        {
            return vertexes.ToDictionary(x => x, x => Rd.Next(0, 3));
        }

        private static Dictionary<int, int> GetColor(IEnumerable<(int, int)> edges, Dictionary<int, int> banColor)
        {
            var graph = Get2SatGraph(edges, banColor);
            var graphT = graph.SelectMany(x => x.Value.Select(y => (y, x.Key))).GroupBy(x => x.Item1)
                .ToDictionary(x => x.Key, x => x.Select(y => y.Item2).ToList());

            var topSort = TopSort(graph).Reverse();
            var compsNumber = ComponentsNumber(graphT, topSort);

            return CompsToColors(compsNumber)?.ToDictionary(x => x.Key, x => (x.Value + 1 + banColor[x.Key]) % 3);
        }

        private static Dictionary<(int, int), List<(int, int)>> Get2SatGraph(IEnumerable<(int, int)> edges,
            Dictionary<int, int> banColor)
        {
            var vertexesColors = banColor.ToDictionary(x => x.Key, x => new[] {(x.Value + 1) % 3, (x.Value + 2) % 3});
            var graph = vertexesColors.SelectMany(x => (new[] {0, 1}).Select(y => (x.Key, y)))
                .ToDictionary(x => x, x => new List<(int, int)>());

            foreach (var (u, v) in edges)
            {
                for (var i = 0; i < 2; i++)
                {
                    for (var j = 0; j < 2; j++)
                    {
                        if (vertexesColors[u][i] == vertexesColors[v][j])
                        {
                            graph[(u, i)].Add((v, j ^ 1));
                            graph[(v, j)].Add((u, i ^ 1));
                        }
                    }
                }
            }

            return graph;
        }

        private static IEnumerable<(int, int)> TopSort(Dictionary<(int, int), List<(int, int)>> graph)
        {
            var used = new HashSet<(int, int)>();
            foreach (var u in graph.Keys)
            {
                if (used.Contains(u))
                    continue;
                foreach (var v in TopSort(u, graph, used))
                    yield return v;
            }
        }

        private static IEnumerable<(int, int)> TopSort((int, int) u, Dictionary<(int, int), List<(int, int)>> graph,
            HashSet<(int, int)> used)
        {
            used.Add(u);
            foreach (var v in graph[u])
            {
                if (used.Contains(v))
                    continue;
                foreach (var k in TopSort(v, graph, used))
                    yield return k;
            }

            yield return u;
        }

        private static Dictionary<(int, int), int> ComponentsNumber(Dictionary<(int, int), List<(int, int)>> graph,
            IEnumerable<(int, int)> order)
        {
            var curNumber = 0;
            var result = new Dictionary<(int, int), int>();
            foreach (var u in order)
            {
                if (result.ContainsKey(u))
                    continue;
                SetComponentNumber(u, result, curNumber++, graph);
            }

            return result;
        }

        private static void SetComponentNumber((int, int) u, Dictionary<(int, int), int> nums, int curNum,
            Dictionary<(int, int), List<(int, int)>> graph)
        {
            nums.Add(u, curNum);
            if (graph.ContainsKey(u))
            {
                foreach (var v in graph[u])
                {
                    if (nums.ContainsKey(v))
                        continue;
                    SetComponentNumber(v, nums, curNum, graph);
                }
            }
        }

        private static Dictionary<int, int> CompsToColors(Dictionary<(int, int), int> comps)
        {
            var res = new Dictionary<int, int>();
            foreach (var u in comps.Keys)
            {
                var v = (u.Item1, u.Item2 ^ 1);
                if (comps[u] == comps[v])
                    return null;
                if (comps[u] > comps[v])
                    res.Add(u.Item1, u.Item2);
            }

            return res;
        }
    }
}