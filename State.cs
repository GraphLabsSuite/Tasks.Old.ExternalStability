using System.Windows;
using GraphLabs.Graphs;
using System.Collections.Generic;
using System;

namespace GraphLabs.Tasks.ExternalStability
{
    /// <summary>
    /// Цвета вершин
    /// </summary>
    public enum StateColor
    {
        /// <summary>
        /// Белый
        /// </summary>
        WHITE,
        /// <summary>
        /// Синий
        /// </summary>
        BLUE,
        /// <summary>
        /// Красный
        /// </summary>
        RED
    }

    /// <summary>
    /// Состояние графа в момент рекурсии
    /// </summary>
    public class State
    {
        /// <summary>
        /// Цвета вершин
        /// </summary>
        public IDictionary<Vertex, StateColor> VertexColor
        {
            get;
            private set;
        }


        /// <summary>
        /// Соседи вершин
        /// </summary>
        public IDictionary<Vertex, List<Vertex>> VertexNeighbors { get;private set; }

        /// <summary>
        /// Число доминантов
        /// </summary>
       public IDictionary<Vertex, int> VertexDominatedNumber
        {
            get;
            private set;
        }

        /// <summary>
        /// Возможное число доминантов
        /// </summary>
        public IDictionary<Vertex, int> VertexPossibleDominatingNumber { get; private set; }

        /// <summary>
        /// Текущее доминирующее множество
        /// </summary>
        public List<Vertex> TempDS { get; private set; }

        /// <summary>
        /// Количество доминируемых вершин
        /// </summary>
        public int N_dominated { get; private set; }

        /// <summary>
        /// Уровень рекурсии
        /// </summary>
        public int Level { get; private set; }

        /// <summary>
        /// Создаёт копию объекта для рекурсии
        /// </summary>
        /// <returns></returns>
        public State Clone()
        {
            return new State(this);
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
                VertexColor.Add(vertex, StateColor.WHITE);
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

        private State(State prototype)
        {
            this.Level = prototype.Level;
            this.N_dominated = prototype.N_dominated;
            this.TempDS = prototype.TempDS;
            this.VertexColor = new Dictionary<Vertex, StateColor>(prototype.VertexColor);
            //...
        }
    }
}
