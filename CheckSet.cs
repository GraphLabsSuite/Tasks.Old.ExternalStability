using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using GraphLabs.Graphs;

namespace GraphLabs.Tasks.ExternalStability
{
    /// <summary>
    /// Проверки на доминирующее и минимальное доминирующее множества
    /// </summary>
    public class CheckSet
    {
        /// <summary>
        /// Проверка, является ли множество доминирующим
        /// </summary>
        /// <param name="vSet"></param>
        /// <param name="givenGraph"></param>
        /// <returns></returns>
        public bool IsExternalStability(IList<Vertex> vSet, UndirectedGraph givenGraph)
        {
            var extendedSetofVertex = new Collection<Vertex>();

            foreach (var vertex in vSet)
            {
                extendedSetofVertex.Add(vertex);
            }

            //Добавляем вершины, соседние с выбранными в расширенный набор вершин
            foreach (var vertex in vSet)
            {
                foreach (var edge in givenGraph.Edges)
                {
                    if ((edge.Vertex1 == vertex) && !extendedSetofVertex.Contains(edge.Vertex2))
                    {
                        extendedSetofVertex.Add(edge.Vertex2);
                    }
                    if ((edge.Vertex2 == vertex) && !extendedSetofVertex.Contains(edge.Vertex1))
                    {
                        extendedSetofVertex.Add(edge.Vertex1);
                    }

                }
                // GivenGraph.Vertices.Where(e => externalMultiplicity.Contains(e.Vertex1));
            }

            //Проверяем, есть ли вершины в графе, не являющиеся соседними с выбранным множеством
            foreach (var vertex in givenGraph.Vertices)
            {
                if (!extendedSetofVertex.Contains(vertex))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        ///  Является ли множество минимальным доминирующим
        /// </summary>
        /// <param name="setDES"></param>
        /// <param name="givenGraph"></param>
        /// <returns></returns>
        public bool IsMinimal(IList<Vertex> setDES, UndirectedGraph givenGraph)
        {
            var leadFlag = true;
            var flag = true;
            for (var i = 0; i < setDES.Count; i++)
            {
                var newSet = new SccRowViewModel(setDES, false);
                newSet.VerticesSet.RemoveAt(i);
                flag = IsExternalStability(newSet.VerticesSet, givenGraph);
                if (flag) leadFlag = false;
            }
            if (leadFlag) return true;
            return false;
        }
    }
}
