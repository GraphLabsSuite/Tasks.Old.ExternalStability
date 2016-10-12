using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Autofac.Core;
using GraphLabs.CommonUI;
using GraphLabs.CommonUI.Controls.ViewModels;
using GraphLabs.Graphs;
using GraphLabs.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Windows;


using Vertex = GraphLabs.Graphs.Vertex;

namespace GraphLabs.Tasks.ExternalStability
{
    public partial class ExternalStabilityViewModel
    {
        private const string ImageResourcesPath = @"/GraphLabs.Tasks.ExternalStability;component/Images/";

        private Uri GetImageUri(string imageFileName)
        {
            return new Uri(ImageResourcesPath + imageFileName, UriKind.Relative);
        }

        private void InitToolBarCommands()
        {
            ToolBarCommands = new ObservableCollection<ToolBarCommandBase>();

            // Выбор множества External stability
            var selectDsCommand = new ToolBarToggleCommand(
                () =>
                {
                    if (_task == Task.TaskSelectDomSets)
                    {
                        VertexClickCmd = new DelegateCommand(
                            o => SelectRMouseClick((IVertex) o),
                            o => true);
                        _state = State.SetEs;
                        IsMouseVerticesMovingEnabled = false;
                    }
                    
                },

                () =>
                {
                    _state = State.Nothing;
                    IsMouseVerticesMovingEnabled = true;
                },

                () => _state == State.Nothing,
                () => true
              )
            {
                Image = new BitmapImage(GetImageUri("es.png")),
                Description = "Выбор множества внешней устойчивости"
            };

            // Вызов окна со справкой
            var helpM = new ToolBarInstantCommand(
                () => MessageBox.Show
                    (
                        "Лабораторная работа \"Устойчивость графов \"\n "
                        +
                        "Задание \"Множество внешней устойчивости\"\n"
                        +
                        "Цель: найти число внешней устойчивости графа\n"
                        +
                        "\n"
                        +
                        "для перехода к следующему заданию нажмите ОК\n"
                        +
                        "Для изменения матрицы необходимо изменить значение в ячейке и нажать \"Enter\"\n"
                        +
                        "Либо дважды кликнуть мышью по ячейке матрицы\n"
                        +
                        "\n"
                        +
                        "Задания:\n"
                        +
                        "1.1 Заполнить матрицу смежности\n"
                        +
                        "1.2 Измените матрицу смежности под выполнение алгоритма красно-синих вершин\n"
                        +
                        "2.Выделите несколько доминирующих множеств графа\n (выделение множества доступно по кнопке <ES>\nзакрытие множества происходит по кнопке <{}>)\n"
                        +
                        "3.Определить число внешней устойчивости (пометить соответствующее множество вершин)"
                    ),
                () => _state == State.Nothing
                )
            {
                Image = new BitmapImage(GetImageUri("help.png")),
                Description = "справка"
            };

            // Проверка задания
            var checkButton = new ToolBarInstantCommand(
                () =>
                {
                    switch (_task)
                    {
                        case Task.TaskAdjacencyMatrix:
                            CheckMatrix();
                            break;
                        case Task.TaskModifiedAdjMatrix:
                            CheckMatrixforAghorithm();
                            break;
                        case Task.TaskSelectDomSets:
                            MessageBox.Show(string.Format(
                                "Необходимо найти еще {0} множеств(о) внешней устойчивости", _countOfSes));
                            break;
                        case Task.TaskFindMinDomSets:
                            IsMinDS();
                            break;
                        case Task.TaskEnd:
                            UserActionsManager.ReportThatTaskFinished();
                            break;
                    }
                },
                () => _state == State.Nothing
                )
            {
                Image = new BitmapImage(GetImageUri("ok.png")),
                Description = "Проверить матрицу"
            };

            // Проверка множества и его сохранение
            var addSetofES = new ToolBarInstantCommand(
                () =>
                {
                    if (_task == Task.TaskSelectDomSets)
                    ValidateSet();
                },
                () => _state == State.Nothing
                )
            {
                Image = new BitmapImage(GetImageUri("add.png")),
                Description = "Добавить множество"
            };

            // Проверка задания
            var debugButton = new ToolBarInstantCommand(
                () =>
                {
                    var counter = new MatrixErrorCounter();
                    if (_task == Task.TaskAdjacencyMatrix)
                        counter.FillInMatrix(Matrix, GivenGraph);
                    if (_task == Task.TaskModifiedAdjMatrix)
                        counter.ModifyMatrix(Matrix);
                },
                () => _state == State.Nothing
                )
            {
                Image = new BitmapImage(GetImageUri("ok.png")),
                Description = "Автозаполнение"
            };

            ToolBarCommands.Add(checkButton);
            ToolBarCommands.Add(selectDsCommand);
            ToolBarCommands.Add(addSetofES);
            ToolBarCommands.Add(helpM);
            ToolBarCommands.Add(debugButton);



        }
    }
}
