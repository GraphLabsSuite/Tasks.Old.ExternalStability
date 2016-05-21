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
        public List<Vertex> TempDs { get; private set; }

        /// <summary>
        /// Количество доминируемых вершин
        /// </summary>
        public int NDominated { get; set; }

        /// <summary>
        /// Уровень рекурсии
        /// </summary>
        public int Level { get; set; }

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
            TempDs = new List<Vertex>();
            VertexColor = new Dictionary<Vertex, StateColor>();
            VertexDominatedNumber = new Dictionary<Vertex, int>();
            VertexNeighbors = new Dictionary<Vertex, List<Vertex>>();
            VertexPossibleDominatingNumber = new Dictionary<Vertex, int>();
            Level = 0;
            foreach (var vertex in graph.Vertices)
            {
                VertexColor.Add(vertex, StateColor.WHITE);
                VertexDominatedNumber.Add(vertex, 0);
                var tempNeighbors = new List<Vertex>();
                for (int i = 0; i < graph.VerticesCount; i++)
                {
                    if (graph[graph.Vertices[i], vertex] != null) tempNeighbors.Add(graph.Vertices[i]);
                }
                VertexNeighbors.Add(vertex, tempNeighbors);
                VertexPossibleDominatingNumber.Add(vertex, tempNeighbors.Count + 1);
            }
            NDominated = 0;
        }

        private State(State prototype)
        {
            this.Level = prototype.Level;
            this.NDominated = prototype.NDominated;
            this.TempDs = new List<Vertex>(prototype.TempDs);
            this.VertexDominatedNumber = new Dictionary<Vertex, int>(prototype.VertexDominatedNumber);
            this.VertexColor = new Dictionary<Vertex, StateColor>(prototype.VertexColor);
            this.VertexNeighbors = new Dictionary<Vertex, List<Vertex>>(prototype.VertexNeighbors);
            this.VertexPossibleDominatingNumber = new Dictionary<Vertex, int>(prototype.VertexPossibleDominatingNumber);
            //...
        }
    }
}
