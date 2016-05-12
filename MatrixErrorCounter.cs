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
    public partial class MatrixErrorCounter : ExternalStabilityViewModel
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

                UserActionsManager.RegisterMistake(mistake, k);
        }

        /// <summary>
        /// Проверка правильности заполнения матрицы смежности
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public int CountOfErrorsMatrix(ObservableCollection<MatrixRowViewModel<string>> m)
        {
            var counter = 0;

            for (var i = 0; i < m.Count; i++)
                for (var j = 0; j < m.Count; j++)
                {
                    var studentInput = (m[i][j + 1] ?? "").Trim();
                    var directEdge = GivenGraph[GivenGraph.Vertices[i], GivenGraph.Vertices[j]];

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
        /// <returns></returns>
        public int CountOfErrorsMatrixforAlgorithm(ObservableCollection<MatrixRowViewModel<string>> m)
        {
            int counter = 0;

            for (int i = 0; i < GivenGraph.VerticesCount; i++)
            {
                for (int j = 0; j < GivenGraph.VerticesCount; j++)
                {
                    var directEdge = GivenGraph[GivenGraph.Vertices[i], GivenGraph.Vertices[j]];

                    if ((i != j) && (m[i][j + 1] == "1") && (directEdge == null))
                    {
                        counter++;
                    }
                    if ((i != j) && (m[i][j + 1] != "1") && (directEdge != null))
                    {
                        counter++;
                    }
                    if ((i == j) && (m[i][j + 1] != "1"))
                    {
                        counter++;
                    }
                }
            }

            return counter;
        }

    }
}
