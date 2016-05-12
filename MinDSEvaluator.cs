using GraphLabs.Graphs;
using System.Collections.Generic;
using System;
using State.cs;

namespace GraphLabs.Tasks.ExternalStability
{
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
            foreach (KeyValuePair<Vertex,List<Vertex>> keyValue in state.VertexNeighbors)
                if (keyValue.Key == vertex)
                {
                    foreach (Vertex vert in keyValue.Value)
                    {
                        foreach (KeyValuePair<Vertex, int> keyValue2 in state.VertexDominatedNumber)
                        {
                            if ((keyValue2.Key == vert) && (keyValue2.Value == 0))
                            {
                                return false;
                            }
                        }
                    }
                    return true;
                } else
                {
                    return false;
                }
        }

        public bool CanVerticesBeCovered (State state)
        {
            foreach (KeyValuePair<Vertex, List<Vertex>> keyValue in state.VertexNeighbors)
                    foreach (Vertex vert in keyValue.Value)
                    {
                        foreach (KeyValuePair<Vertex, int> keyValue2 in state.VertexDominatedNumber)
                        {
                            if ((keyValue2.Key == vert) && (keyValue2.Value == 0))
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
            foreach (KeyValuePair<Vertex, bool> keyValue in state.VertexDominatedNumber)
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
                            if (keyValue2.Value == vert)
                            {
                                temp = state.VertexPossibleDominatingNumber.get(vert);
                                temp--;
                                state.VertexPossibleDominatingNumber.set(vert, temp);
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
                            if (keyValue2.Value == vert)
                            {
                                temp = state.VertexPossibleDominatingNumber.get(vert);
                                temp++;
                                state.VertexPossibleDominatingNumber.set(vert, temp);
                                temp2 = state.VertexDominatedNumber.get(vert);
                                temp2++;
                                state.VertexPossibleDominatingNumber.set(vert, temp2);
                            }
                        }
                    }
                }
            }
            state.N_dominated.set(RecountNDominated(state));
        }

        private void Process(State givenState, UnDirectedGraph graph)
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
                        MinDS.Clear;
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
                        State newState = givenState.Clone();
                        newState.Level++;
                        Vertex givenVertex = graph.Vertices[givenState.Level];
                        newState.VertexColor.set(givenVertex, givenState.Color.BLUE);
                        BlueVertexRecount(givenState, givenVertex);
                        Process(newState, graph);
                        newState.VertexColor.set(givenVertex, givenState.Color.RED);
                        RedVertexRecount(givenState, givenVertex);
                        Process(newState, graph);
                        return;
                    }
                }
            }
        }
    }
}
