using System.Collections.Generic;
using GraphLabs.Graphs;
using GraphLabs.CommonUI.Controls.ViewModels;


namespace GraphLabs.Tasks.ExternalStability
{

    /// <summary>
    /// Алгоритм нахождения минимального множества внешней устойчивости
    /// </summary>
    public partial class TemporalDS
    { 
        /// <summary>
        /// Текущее множество внешней устойчивости
        /// </summary>
        public IList<MinDSVertexViewModel> TempDS;

        /// <summary>
        /// Максимальное число связей у вершины в графе
        /// </summary>
        public int Delta;

        /// <summary>
        /// Число доминируемых вершин
        /// </summary>
        public int DominatedNumberinGraph;

        /// <summary>
        /// Ещё не определившаяся вершина
        /// </summary>
        const int WHITE = 0;
        /// <summary>
        /// Вершина в формирующемся доминирующем множестве
        /// </summary>
        const int RED = 1;
        /// <summary>
        /// Вершина, выгнанная из формирования доминирующего множества
        /// </summary>
        const int BLUE = 2;



        /// <summary>
        /// Текущий размер множества внешней устойчивости
        /// </summary>
        public int TempSize;

        /// <summary>
        /// Граф с расширенными вершинами
        /// </summary>
        public IGraph<MinDSVertexViewModel, IEdge> MGraph { get; private set; }




        private void InitialiseForMinDS(IGraph graph, List<IList<MinDSVertexViewModel>> MinDS, int MinSize)
        {
            TempDS = new List<MinDSVertexViewModel>();
            var MEdges = graph.Edges;
            var NumVert = graph.VerticesCount;
            MGraph = graph;
            foreach (IVertex vertex in MGraph.Vertices)
            {
                var Vertex = new MinDSVertexViewModel(vertex, MGraph);
            }
            TempSize = 0;
            Delta = 0;
            DominatedNumberinGraph = 0;
            MinSize = MGraph.VerticesCount;
            for (int i = 0; i < MGraph.VerticesCount; i++)
            {
                MinDS[0][i] = new MinDSVertexViewModel(MGraph.Vertices[i], MGraph);
                int TempDelta = 0;
                for (int j = 0; j < MGraph.VerticesCount; j++)
                {
                    if (MGraph[MGraph.Vertices[i], MGraph.Vertices[j]] != null)
                    {
                        TempDelta++;
                    }
                }
                if (TempDelta > Delta)
                {
                    Delta = TempDelta;
                }
            }
        }

        /// <summary>
        /// Функция нахождения минимального множества внешней устойчивости
        /// </summary>
        /// <param name="level"></param>
        /// <param name="graph"></param>
        /// <param name="matrix"></param>
        /// <param name="MinDS"></param>
        /// <returns></returns>
        public int FindMinDS(int level, IGraph<MinDSVertexViewModel, TEdge> graph, IList<MatrixRowViewModel<string>> matrix, List<IList<MinDSVertexViewModel>> MinDS, int MinSize)
        {
            if (level == 0)
            {
                InitialiseForMinDS(graph, MinDS, MinSize);
            }
            if (level == graph.VerticesCount)
            {
                if (TempSize < MinSize)
                {
                    MinDS[0] = TempDS;
                    MinSize = TempSize;
                    return 0;
                }
                else if (TempSize == MinSize)
                {
                    MinDS[MinDS.Count] = TempDS;
                    return 0;
                }
            }
            else
            {
                for (int i = 0; i < graph.VerticesCount; i++)
                {
                    if (graph.Vertices[i].PossibleDominationNumber == 0) return 1;
                }

                var UnDominatedNumber = graph.VerticesCount - DominatedNumberinGraph;
                var n_extra = UnDominatedNumber / (Delta + 1);
                if ((n_extra + TempSize) > MinSize)
                {
                    return 2;
                }

                bool flag = true;
                for (int i = 0; i < graph.VerticesCount; i++)
                {
                    if (graph.Vertices[i].DominatedNumber == 0) flag = false;
                }
                if (flag)
                {
                    if (TempSize < MinSize)
                    {
                        MinDS[0] = TempDS;
                        MinSize = TempSize;
                        return 0;
                    }
                    else if (TempSize == MinSize)
                    {
                        MinDS[MinDS.Count] = TempDS;
                        return 0;
                    }
                }

                var u = level;
                graph.Vertices[u].VerticeColor = BLUE;
                for (int i = 0; i < matrix.Count; i++)
                {
                    if (matrix[u][i + 1] == "1")
                    {
                        graph.Vertices[i].PossibleDominationNumber++; //Why?
                        graph.Vertices[i].DominatedNumber--;
                    }
                }
                TempDS.Add(graph.Vertices[u]);
                level++;
                var BlueDaughter = new TemporalDS(); //Нужно передать по значению текущий объект
                BlueDaughter.FindMinDS(level, graph, matrix, MinDS, MinSize);
                for (int i = 0; i < matrix.Count; i++)
                {
                    if (matrix[u][i + 1] == "1")
                    {
                        graph.Vertices[i].DominatedNumber++;
                    }
                }
                DominatedNumberinGraph = graph.Vertices[u].DominatedNumber;
                var RedDaughter = new TemporalDS();
                RedDaughter.FindMinDS(level, graph, matrix, MinDS, MinSize);
                return 0;
            }


            return 0;
        }
    }
}
