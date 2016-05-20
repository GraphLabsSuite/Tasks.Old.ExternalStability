﻿using GraphLabs.Graphs;
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
        private readonly int _delta;

        /// <summary>
        /// Константа - количество вершин графа
        /// </summary>
        private readonly int _n;

        /// <summary>
        /// Создаёт начальную версию MinDS
        /// </summary>
        /// <param name="graph"></param>
        public MinDSEvaluator(UndirectedGraph graph)
        {
            MinDs = new List<List<Vertex>>();
            _delta = 0;
            _n = graph.VerticesCount;
            var tempDs = new List<Vertex>();
            for (var i = 0; i < _n; i++)
            {
                tempDs.Add(graph.Vertices[i]);
                var tempDelta = 0;
                for (var j = 0; j < _n; j++)
                {
                    if (graph[graph.Vertices[i], graph.Vertices[j]] != null) tempDelta++;
                }
                if (tempDelta > _delta) _delta = tempDelta;
            }
            MinDs.Add(tempDs);
        }

        /// <summary>
        /// Публичный метод вычисления минимального доминирующего множества
        /// </summary>
        /// <param name="graph"></param>
        /// <returns></returns>
        public List<List<Vertex>> Evaluate(UndirectedGraph graph)
        {
            var firstStep = new State(graph);
            Process(firstStep, graph);
            return MinDs;
        }

        /// <summary>
        /// Проверяет есть ли вершины, которые не могут быть покрыты красной в данной ситуации
        /// </summary>
        /// <param name="vertex"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        private bool CanVertexBeCovered(Vertex vertex, State state)
        {
            List<Vertex> neighbors;
            state.VertexNeighbors.TryGetValue(vertex, out neighbors);
            int vertDomNum;
            state.VertexPossibleDominatingNumber.TryGetValue(vertex, out vertDomNum);
            if (vertDomNum == 0)
            {
                return false;
            }
            if (neighbors != null)
            {
                foreach (var neigh in neighbors)
                {
                    int vertNeighDomNum;
                    state.VertexPossibleDominatingNumber.TryGetValue(neigh, out vertNeighDomNum);
                    if (vertNeighDomNum == 0)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Проверка на то, что все вершины ещё могут быть кем-либо доминируемы
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        private bool CanVerticesBeCovered(State state)
        {
            foreach (KeyValuePair<Vertex, int> keyValue in state.VertexPossibleDominatingNumber)
            {
                int vertDomNum;
                state.VertexPossibleDominatingNumber.TryGetValue(keyValue.Key, out vertDomNum);
                if (vertDomNum == 0)
                {
                    return false;
                }
            }
            return true;
        }

        private int RecountNDominated(State state)
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
                    int temp;
                    state.VertexPossibleDominatingNumber.TryGetValue(vert, out temp);
                    temp--;
                    state.VertexPossibleDominatingNumber.Remove(vert);
                    state.VertexPossibleDominatingNumber.Add(vert, temp);
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
            foreach (var keyValue in state.VertexNeighbors)
            {
                if (keyValue.Key.Equals(givenVertex))
                {
                    foreach (var vert in keyValue.Value)
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
            state.NDominated = RecountNDominated(state);
        }

        private void Process(State givenState, UndirectedGraph graph)
        {
            if (givenState.Level == _n)
            {
                var isAllVerticesCovered = CanVerticesBeCovered(givenState);
                if (isAllVerticesCovered == false)
                {
                    return;
                }
                else
                {
                    if (MinDs.Count > givenState.TempDs.Count)
                    {
                        MinDs.Clear();
                        MinDs.Add(givenState.TempDs);
                        return;
                    }
                    if (MinDs.Count == givenState.TempDs.Count)
                    {
                        MinDs.Add(givenState.TempDs);
                        return;
                    }
                }
            }
            else
            {
                var givenVertex = graph.Vertices[givenState.Level];
                givenState.VertexColor.Remove(givenVertex);
                givenState.VertexColor.Add(givenVertex, StateColor.BLUE);
                BlueVertexRecount(givenState, givenVertex);
                var isVertexCovered = CanVertexBeCovered(givenVertex, givenState);
                if (isVertexCovered)
                {
                    var newState = givenState.Clone();
                    newState.Level++;
                    Process(newState, graph);
                }
                givenState.VertexColor.Remove(givenVertex);
                givenState.VertexColor.Add(givenVertex, StateColor.RED);
                RedVertexRecount(givenState, givenVertex);
                if (givenState.NDominated == _n)
                {
                    if (MinDs.Count > givenState.TempDs.Count)
                    {
                        MinDs.Clear();
                        MinDs.Add(givenState.TempDs);
                        return;
                    }
                    if (MinDs.Count == givenState.TempDs.Count)
                    {
                        MinDs.Add(givenState.TempDs);
                        return;
                    }
                }
                else
                {
                    var nExtra = (_n - givenState.NDominated) / (_delta + 1);
                    if ((nExtra + givenState.TempDs.Count) > MinDs.Count)
                    {
                        return;
                    }
                    else
                    {
                        givenState.TempDs.Add(givenVertex);
                        var newState = givenState.Clone();
                        newState.Level++;
                        Process(newState, graph);
                        return;
                    }
                }

            }
        }
    }
}
