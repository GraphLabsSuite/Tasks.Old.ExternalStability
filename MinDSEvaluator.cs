using GraphLabs.Graphs;
using System.Collections.Generic;

namespace GraphLabs.Tasks.ExternalStability
{
    /// <summary>
    /// Вычисление минимального множества внешней устойчивости
    /// </summary>
    public class MinDSEvaluator
    {
        private List<List<Vertex>> MinDS;

        private int Delta;

        private int N;

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
                MinDS[0].Add(graph.Vertices[i]);
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
            State FirstStep = new State(graph);
            Process(FirstStep, graph);
            return MinDS;
        }

        /// <summary>
        /// Проверяет есть ли вершины, которые не могут быть покрыты красной в данной ситуации
        /// </summary>
        /// <param name="vertex"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public bool CanVertexBeCovered (Vertex vertex, State state)
        {
            List<Vertex> Neighbors;
            state.VertexNeighbors.TryGetValue(vertex, out Neighbors);
                foreach (Vertex vert in Neighbors)
                {
                int VertDomNum;
                state.VertexPossibleDominatingNumber.TryGetValue(vertex, out VertDomNum);
                    if (VertDomNum == 0)
                    {
                        return false;
                    }
                }
                return true;
            }
        /// <summary>
        /// Проверка на то, что все вершины ещё могут быть кем-либо доминируемы
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public bool CanVerticesBeCovered (State state)
        {
            foreach (KeyValuePair<Vertex, List<Vertex>> keyValue in state.VertexNeighbors) {
                    foreach (Vertex vert in keyValue.Value)
                    {
                    int VertDomNum;
                    state.VertexPossibleDominatingNumber.TryGetValue(keyValue.Key, out VertDomNum);
                    if (VertDomNum == 0)
                    {
                        return false;
                    }
                }
             }
             return true;
        }

        private int RecountNDominated (State state)
        {
            int Result = 0;
            foreach (KeyValuePair<Vertex, int> keyValue in state.VertexDominatedNumber)
            {
                if (keyValue.Value > 0)
                {
                    Result++;
                }
            }
            return Result;
        }

        private void BlueVertexRecount(State state, Vertex givenVertex)
        {
            foreach (KeyValuePair<Vertex, List<Vertex>> keyValue in state.VertexNeighbors)
            {
                if (keyValue.Key == givenVertex)
                {
                    foreach (Vertex vert in keyValue.Value)
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
        }

        private void RedVertexRecount(State state, Vertex givenVertex)
        {
            foreach (KeyValuePair<Vertex, List<Vertex>> keyValue in state.VertexNeighbors)
            {
                if (keyValue.Key == givenVertex)
                {
                    foreach (Vertex vert in keyValue.Value)
                    {
                        foreach (KeyValuePair<Vertex, int> keyValue2 in state.VertexPossibleDominatingNumber)
                        {
                            if (keyValue2.Key == vert)
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
                bool IsAllVerticesCovered = CanVerticesBeCovered(givenState);
                if (IsAllVerticesCovered == false)
                {
                    return;
                }
                else
                {
                    if (MinDS.Count > givenState.TempDS.Count)
                    {
                        MinDS.Clear();
                        MinDS.Add(givenState.TempDS);
                        return;
                    }
                    else if (MinDS.Count == givenState.TempDS.Count)
                    {
                        MinDS.Add(givenState.TempDS);
                        return;
                    }
                }
            } else
            {
                bool IsVertexCovered = CanVertexBeCovered(graph.Vertices[givenState.Level], givenState);
                if (IsVertexCovered == false)
                {
                    return;
                }
                else
                {
                    int N_extra = (1 - givenState.N_dominated) / (Delta + 1);
                    if ((N_extra + givenState.TempDS.Count) > MinDS.Count)
                    {
                        return;
                    }
                    else
                    {
                        State newState = (State)givenState.Clone();
                        newState.Level++;
                        Vertex givenVertex = graph.Vertices[givenState.Level];
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
