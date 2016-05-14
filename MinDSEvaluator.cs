using GraphLabs.Graphs;
using System.Collections.Generic;

namespace GraphLabs.Tasks.ExternalStability
{
    /// <summary>
    /// Вычисление минимального множества внешней устойчивости
    /// </summary>
    public class MinDSEvaluator
    {
        /// <summary>
        /// Минимальные доминирующие множества
        /// </summary>
        public List<List<Vertex>> MinDs;

        /// <summary>
        /// Максимальное число связей на вершину в графе
        /// </summary>
        private readonly int Delta;

        /// <summary>
        /// Константа - количество вершин графа
        /// </summary>
        private readonly int N;

        /// <summary>
        /// Создаёт начальную версию MinDS
        /// </summary>
        /// <param name="graph"></param>
        public MinDSEvaluator(UndirectedGraph graph)
        {
            Delta = 0;
            N = graph.VerticesCount;
            for (int i = 0; i < graph.VerticesCount; i++)
            {
                MinDs[0].Add(graph.Vertices[i]);
                int TempDelta = 0;
                for (int j = 0; j < graph.VerticesCount; j++)
                {
                    if (graph[graph.Vertices[i], graph.Vertices[j]] != null) TempDelta++;
                }
                if (TempDelta > Delta) Delta = TempDelta;
            }
        }

        /// <summary>
        /// Публичный метод вычисления минимального доминирующего множества
        /// </summary>
        /// <param name="graph"></param>
        /// <returns></returns>
        public List<List<Vertex>> Evaluate(UndirectedGraph graph)
        {
            State firstStep = new State(graph);
            Process(firstStep, graph);
            return MinDs;
        }

        /// <summary>
        /// Проверяет есть ли вершины, которые не могут быть покрыты красной в данной ситуации
        /// </summary>
        /// <param name="vertex"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        private bool CanVertexBeCovered (Vertex vertex, State state)
        {
            List<Vertex> neighbors;
            state.VertexNeighbors.TryGetValue(vertex, out neighbors);
                int vertDomNum;
                state.VertexPossibleDominatingNumber.TryGetValue(vertex, out vertDomNum);
                    if (vertDomNum == 0)
                    {
                        return false;
                    }
                return true;
            }
        /// <summary>
        /// Проверка на то, что все вершины ещё могут быть кем-либо доминируемы
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        private bool CanVerticesBeCovered (State state)
        {
            foreach (KeyValuePair<Vertex, List<Vertex>> keyValue in state.VertexNeighbors) {
                int vertDomNum;
                state.VertexPossibleDominatingNumber.TryGetValue(keyValue.Key, out vertDomNum);
                if (vertDomNum == 0)
                {
                    return false;
                }
             }
             return true;
        }

        private int RecountNDominated (State state)
        {
            int result = 0;
            foreach (KeyValuePair<Vertex, int> keyValue in state.VertexDominatedNumber)
            {
                if (keyValue.Value > 0)
                {
                    result++;
                }
            }
            return result;
        }

        private void BlueVertexRecount(State state, Vertex givenVertex)
        {
            List<Vertex> neighbors;
            state.VertexNeighbors.TryGetValue(givenVertex, out neighbors);
            int temp3;
            state.VertexPossibleDominatingNumber.TryGetValue(givenVertex, out temp3);
            temp3--;
            state.VertexPossibleDominatingNumber.Remove(givenVertex);
            state.VertexPossibleDominatingNumber.Add(givenVertex, temp3);
            if (neighbors != null)
            {
                foreach (var vert in neighbors)
                {
                    foreach (KeyValuePair<Vertex, int> keyValue2 in state.VertexPossibleDominatingNumber)
                    {
                        if (keyValue2.Key == vert)
                        {
                            int temp;
                            state.VertexPossibleDominatingNumber.TryGetValue(vert, out temp);
                            temp--;
                            state.VertexPossibleDominatingNumber.Remove(vert);
                            state.VertexPossibleDominatingNumber.Add(vert, temp);
                        }
                    }
                }
            }
        }

        private void RedVertexRecount(State state, Vertex givenVertex)
        {
            int temp3;
            state.VertexPossibleDominatingNumber.TryGetValue(givenVertex, out temp3);
            temp3++;
            state.VertexPossibleDominatingNumber.Remove(givenVertex);
            state.VertexPossibleDominatingNumber.Add(givenVertex, temp3);
            int temp4;
            state.VertexDominatedNumber.TryGetValue(givenVertex, out temp4);
            temp4++;
            state.VertexDominatedNumber.Remove(givenVertex);
            state.VertexDominatedNumber.Add(givenVertex, temp4);
            foreach (KeyValuePair<Vertex, List<Vertex>> keyValue in state.VertexNeighbors)
            {
                if (keyValue.Key == givenVertex)
                {
                    foreach (var vert in keyValue.Value)
                    {
                        foreach (KeyValuePair<Vertex, int> keyValue2 in state.VertexPossibleDominatingNumber)
                        {
                            if (keyValue2.Key.Equals(vert))
                            {
                                int temp;
                                state.VertexPossibleDominatingNumber.TryGetValue(vert, out temp);
                                temp++;
                                state.VertexPossibleDominatingNumber.Remove(vert);
                                state.VertexPossibleDominatingNumber.Add(vert, temp);
                                int temp2;
                                state.VertexDominatedNumber.TryGetValue(vert, out temp2);
                                temp2++;
                                state.VertexDominatedNumber.Remove(vert);
                                state.VertexDominatedNumber.Add(vert, temp2);
                            }
                        }
                    }
                }
            }
            state.N_dominated = RecountNDominated(state);
        }

        private void Process(State givenState, UndirectedGraph graph)
        {
            if (givenState.Level == N)
            {
                var isAllVerticesCovered = CanVerticesBeCovered(givenState);
                if (isAllVerticesCovered == false)
                {
                    return;
                }
                else
                {
                    if (MinDs.Count > givenState.TempDS.Count)
                    {
                        MinDs.Clear();
                        MinDs.Add(givenState.TempDS);
                        return;
                    }
                    if (MinDs.Count == givenState.TempDS.Count)
                    {
                        MinDs.Add(givenState.TempDS);
                        return;
                    }
                }
            } else
            {
                var isVertexCovered = CanVertexBeCovered(graph.Vertices[givenState.Level], givenState);
                if (isVertexCovered == false)
                {
                    return;
                }
                else
                {
                    var nExtra = (1 - givenState.N_dominated) / (Delta + 1);
                    if ((nExtra + givenState.TempDS.Count) > MinDs.Count)
                    {
                        return;
                    }
                    else
                    {
                        var newState = givenState.Clone();
                        newState.Level++;
                        var givenVertex = graph.Vertices[givenState.Level];
                        newState.VertexColor.Remove(givenVertex);
                        newState.VertexColor.Add(givenVertex, StateColor.BLUE);
                        BlueVertexRecount(givenState, givenVertex);
                        Process(newState, graph);
                        newState.VertexColor.Remove(givenVertex);
                        newState.VertexColor.Add(givenVertex, StateColor.RED);
                        RedVertexRecount(givenState, givenVertex);
                        Process(newState, graph);
                        return;
                    }
                }
            }
        }
    }
}
