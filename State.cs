using System.Windows;
using GraphLabs.Graphs;
using System.Collections.Generic;
using System;

namespace GraphLabs.Tasks.ExternalStability
{
    enum Color
    {
        WHITE,
        BLUE,
        RED
    }

    /// <summary>
    /// Состояние графа в момент рекурсии
    /// </summary>
    public class State : ICloneable
    {
        public IDictionary<Vertex, Color> VertexColor {
            get
            {
                if (VertexColor.ContainsKey(key) == true)
                return VertexColor.TryGetValue(key, value);
            }
            private set
            {
                VertexColor.Add(key, value);
            }
        }
        public IDictionary<Vertex, List<Vertex>> VertexNeighbors {
            get
            {
                if (VertexNeighbors.ContainsKey(key) == true)
                    return VertexNeighbors.TryGetValue(key, value);
            }
            private set
            {
                VertexNeighbors.Add(key, value);
            }
        }
       public IDictionary<Vertex, int> VertexDominatedNumber {
            get
            {
                if (VertexDominatedNumber.ContainsKey(key) == true)
                    return VertexDominatedNumber.TryGetValue(key, value);
            }
            private set
            {
                 VertexDominatedNumber.Add(key, value);
            }
        }

        public IDictionary<Vertex, int> VertexPossibleDominatingNumber
        {
            get
            {
                if (VertexPossibleDominatingNumber.ContainsKey(key) == true)
                    return VertexPossibleDominatingNumber.TryGetValue(key, value);
            }
            private set
            {
                VertexPossibleDominatingNumber.Add(key, value);
            }
        }

        /// <summary>
        /// Текущее доминирующее множество
        /// </summary>
        public List<Vertex> TempDS;

        /// <summary>
        /// Количество доминируемых вершин
        /// </summary>
        public int N_dominated;

        /// <summary>
        /// Уровень рекурсии
        /// </summary>
        public int Level;

        /// <summary>
        /// Создаёт копию объекта для рекурсии
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return this.MemberwiseClone();
        }

        /// <summary>
        /// Инициализирует значения для алгоритма
        /// </summary>
        /// <param name="graph"></param>
        public State (UndirectedGraph graph)
        {
            TempDS = new List<Vertex>();
            Level = 0;
            foreach (Vertex vertex in graph.Vertices)
            {
                VertexColor.Add(vertex, Color.WHITE);
                VertexDominatedNumber.Add(vertex, 0);
                List<Vertex> TempNeighbors = null;
                for (int i = 0; i < graph.VerticesCount; i++)
                {
                    if (graph[graph.Vertices[i], vertex] != null) TempNeighbors.Add(graph.Vertices[i]);
                }
                VertexNeighbors.Add(vertex, TempNeighbors);
                VertexPossibleDominatingNumber.Add(vertex, TempNeighbors.Count);
            }
            N_dominated = 0;
        }
    }
}
