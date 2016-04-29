using System.Windows;
using GraphLabs.Graphs;
using System.Collections.Generic;

namespace GraphLabs.Tasks.ExternalStability
{
    /// <summary>
    /// ViewModel вершин графа для алгоритма нахождения минимального множества внешней устойчивости.
    /// </summary>
    public partial class MinDSVertexViewModel : IVertex
    {

        /// <summary> Номер вершины </summary>
        public IVertex Vertex;

        /// <summary>
        /// Цвет вершины
        /// </summary>
        public int VerticeColor;

        /// <summary>
        /// Количество красных вершин, имеющих связь с данной
        /// </summary>
        public int DominatedNumber;

        /// <summary>
        /// Общее количество связей
        /// </summary>
        public int PossibleDominationNumber;

        string IVertex.Name
        {
            get
            {
                return Vertex.Name;
            }
        }

        /// <summary> Ctor. </summary>
        public MinDSVertexViewModel(IVertex vert, IGraph graph)
        {

            Vertex = vert;
            DominatedNumber = 0;
            PossibleDominationNumber = 1;
            foreach (var vertex in graph.Vertices)
            {
                var directEdge = graph[Vertex, vertex];
                if (directEdge != null)
                {
                    PossibleDominationNumber++;
                }
            }
        }

        /// <summary>
        /// Изменение какого-либо свойства
        /// </summary>
        /// <param name="color"></param>
        /// <param name="d_num"></param>
        /// <param name="pd_num"></param>
        public void ChangeVerticeProperties(int color, int d_num, int pd_num)
        {
            this.VerticeColor = color;
            this.DominatedNumber = d_num;
            this.PossibleDominationNumber = pd_num;
            }

        public IVertex Rename(string newName)
        {
            return Vertex.Rename(newName);
        }

        public object Clone()
        {
            return Vertex.Clone();
        }

        public bool Equals(IVertex other)
        {
            return Vertex.Equals(other);
        }
    }
}
}
