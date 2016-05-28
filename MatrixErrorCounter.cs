using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Security;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using GraphLabs.Common;
using GraphLabs.Common.UserActionsRegistrator;
using GraphLabs.CommonUI;
using GraphLabs.CommonUI.Controls.ViewModels;
using GraphLabs.Graphs;
using GraphLabs.Graphs.DataTransferObjects.Converters;
using GraphLabs.Utils;
using GraphLabs.Utils.Services;
using GraphLabs.Common.Utils;

namespace GraphLabs.Tasks.ExternalStability
{
    /// <summary>
    /// Определение числа ошибок при обработке правильности заполнения матрицы
    /// </summary>
    public class MatrixErrorCounter
    {

        /// <summary>
        /// Расчёт ошибок и вывод
        /// </summary>
        /// <param name="counter"></param>
        public void ShowMatrixErrors(int counter)
        {
                short k = (short)((short)counter * 3);
                string mistake = null; 
                switch (counter)
                {
                    case 1:
                        mistake = "Найдена " + counter.ToString() + " ошибка";
                        break;
                    case 2:
                    case 3:
                    case 4:
                        mistake = "Найдено " + counter.ToString() + " ошибки";
                        break;
                    default:
                        mistake = "Найдено " + counter.ToString() + " ошибок";
                        break;
                }

                //UserActionsManager.RegisterMistake(mistake, k);
        }

        /// <summary>
        /// Проверка правильности заполнения матрицы смежности
        /// </summary>
        /// <param name="m"></param>
        /// <param name="givenGraph"></param>
        /// <returns></returns>
        public int CountOfErrorsMatrix(ObservableCollection<MatrixRowViewModel<string>> m, UndirectedGraph givenGraph)
        {
            var counter = 0;

            for (var i = 0; i < m.Count; i++)
                for (var j = 0; j < m.Count; j++)
                {
                    var studentInput = (m[i][j + 1] ?? "").Trim();
                    var directEdge = givenGraph[givenGraph.Vertices[i], givenGraph.Vertices[j]];

                    if (directEdge != null && studentInput != "1"
                        ||
                        directEdge == null && studentInput == "1")
                    {
                        counter++;
                    }
                }

            return counter;
        }

        /// <summary>
        /// Проверка правильности заполнения модифицированной матрицы смежности
        /// </summary>
        /// <param name="m"></param>
        /// <param name="givenGraph"></param>
        /// <returns></returns>
        public int CountOfErrorsMatrixforAlgorithm(ObservableCollection<MatrixRowViewModel<string>> m, UndirectedGraph givenGraph)
        {
            int counter = 0;
            for (int i = 0; i < givenGraph.VerticesCount; i++)
            {
                for (int j = 0; j < givenGraph.VerticesCount; j++)
                {
                    var directEdge = givenGraph[givenGraph.Vertices[i], givenGraph.Vertices[j]];
                    var studentInput = (m[i][j + 1] ?? "").Trim();
                    if ((i != j) && (studentInput == "1") && (directEdge == null))
                    {
                        counter++;
                    }
                    if ((i != j) && (studentInput != "1") && (directEdge != null))
                    {
                        counter++;
                    }
                    if ((i == j) && (studentInput != "1"))
                    {
                        counter++;
                    }
                }
            }

            return counter;
        }

        /// <summary>
        /// Автозаполнение матрицы
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="givenGraph"></param>
        public void FillInMatrix(ObservableCollection<MatrixRowViewModel<string>> matrix, UndirectedGraph givenGraph)
        {
            for (int i = 0; i < givenGraph.VerticesCount; i++)
            {
                for (int j = 0; j < givenGraph.VerticesCount; j++)
                {
                    var directEdge = givenGraph[givenGraph.Vertices[i], givenGraph.Vertices[j]];
                    if (directEdge != null)
                    {
                        matrix[i][j + 1] = "1";
                    }
                }
            }
        }

        /// <summary>
        /// Автомодификация матрицы
        /// </summary>
        /// <param name="matrix"></param>
        public void ModifyMatrix(ObservableCollection<MatrixRowViewModel<string>> matrix)
        {
            for (int i = 0; i < matrix.Count; i++)
            {
                for (int j = 0; j < matrix.Count; j++)
                {
                    if (i == j)
                    {
                        matrix[i][j + 1] = "1";
                    }
                }
            }
        }

    }
}
