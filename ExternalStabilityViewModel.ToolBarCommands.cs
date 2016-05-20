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
            var selectDESCommand = new ToolBarToggleCommand(
                () =>
                {
                    if (_task == Task.t2)
                    {
                        VertexClickCmd = new DelegateCommand(
                            o => SelectRMouseClick((IVertex) o),
                            o => true);
                        _state = State.SetDes;
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
                        "\n"
                        +
                        "Задания:\n"
                        +
                        "1.1 Заполнить матрицу смежности\n"
                        +
                        "1.2 Измените матрицу смежности под выполнение алгоритма Петрика\n"
                        +
                        "2.Выделите несколько множеств внешней устойчивости с помощью графа\n (выделение множества доступно по кнопке <ES>\nзакрытие множества происходит по кнопке <{}>)\n"
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
                        case Task.t11:
                            CheckMatrix();
                            break;
                        case Task.t12:
                            CheckMatrixforAghorithm();
                            break;
                        case Task.t2:
                            MessageBox.Show(string.Format(
                                "Необходимо найти еще {0} множеств(о)\n внешней устойчивости", _countOfSes));
                            break;
                        case Task.t3:
                            IsMinDS();
                            break;
                        case Task.end:
                            UserActionsManager.ReportThatTaskFinished();
                            TransferToNextTask();
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
                    if (_task == Task.t2)
                    IsExternalStability();
                },
                () => _state == State.Nothing
                )
            {
                Image = new BitmapImage(GetImageUri("add.png")),
                Description = "Добавить множество"
            };

            ToolBarCommands.Add(checkButton);
            ToolBarCommands.Add(selectDESCommand);
            ToolBarCommands.Add(addSetofES);
            ToolBarCommands.Add(helpM);

            
        }
    }
}
