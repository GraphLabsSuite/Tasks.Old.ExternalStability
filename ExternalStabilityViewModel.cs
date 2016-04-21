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
    /// <summary> ViewModel для ExternalStability </summary>
    public partial class  ExternalStabilityViewModel : TaskViewModelBase<ExternalStability>
    { 
        /// <summary> Текущее состояние </summary>
    private enum State
        {
            /// <summary> Пусто </summary>
            Nothing,
            /// <summary> Добавление вершин в множество ES </summary>
            SetDES,
        }
        /// <summary> Текущее подзадание </summary>
        private enum Task
        {
            /// <summary>Заполнение матрицы </summary>
            t11,
            /// <summary> Изменение матрицы для алгоритма нахождения ES </summary>
            t12,
            /// <summary> Добавление множеств ES </summary>
            t2,
            /// <summary> Вывод о числе внешней устойчивости </summary>
            t3,
            /// <summary>Задание выполнено</summary>
            end
        }

        /// <summary> Текущее задание </summary>
        private Task _task;

        /// <summary>
        /// Требуемое число нахождения множеств внешней устойчивости
        /// </summary>
        private short _countOfSes = 5;

        /// <summary> Текущее состояние </summary>
        private State _state;

        /// <summary> Допустимые версии генератора, с помощью которого сгенерирован вариант </summary>
        private readonly Version[] _allowedGeneratorVersions = new[] {  new Version(1, 0) };


        private readonly Color defaultBorderColor     = Color.FromArgb(255, 50, 133, 144);
        private readonly Color defaultBackgroundColor = Color.FromArgb(250, 207, 207, 207);
       

        #region Public свойства вьюмодели

        /// <summary> Идёт загрузка данных? </summary>
        public static readonly DependencyProperty IsLoadingDataProperty = DependencyProperty.Register(
            nameof(IsLoadingData), 
            typeof(bool), 
            typeof(ExternalStabilityViewModel), 
            new PropertyMetadata(false));

        /// <summary> Идёт загрузка данных? </summary>
        public bool IsLoadingData
        {
            get { return (bool)GetValue(IsLoadingDataProperty); }
            private set { SetValue(IsLoadingDataProperty, value); }
        }

        /// <summary> Разрешено перемещение вершин? </summary>
        public static readonly DependencyProperty IsMouseVerticesMovingEnabledProperty = DependencyProperty.Register(
            nameof(IsMouseVerticesMovingEnabled),
            typeof(bool), 
            typeof(ExternalStabilityViewModel),
            new PropertyMetadata(false));

        /// <summary> Разрешено перемещение вершин? </summary>
        public bool IsMouseVerticesMovingEnabled
        {
            get { return (bool)GetValue(IsMouseVerticesMovingEnabledProperty); }
            set { SetValue(IsMouseVerticesMovingEnabledProperty, value); }
        }


        /// <summary> Команды панели инструментов</summary>
        public static readonly DependencyProperty ToolBarCommandsProperty = DependencyProperty.Register(
            nameof(ToolBarCommands), 
            typeof(ObservableCollection<ToolBarCommandBase>), 
            typeof(ExternalStabilityViewModel), 
            new PropertyMetadata(default(ObservableCollection<ToolBarCommandBase>)));

        /// <summary> Команды панели инструментов </summary>
        public ObservableCollection<ToolBarCommandBase> ToolBarCommands
        {
            get { return (ObservableCollection<ToolBarCommandBase>)GetValue(ToolBarCommandsProperty); }
            set { SetValue(ToolBarCommandsProperty, value); }
        }

        /// <summary> Выданный в задании граф </summary>
        public static readonly DependencyProperty GivenGraphProperty = DependencyProperty.Register(
           nameof(GivenGraph), 
            typeof(IGraph), 
            typeof(ExternalStabilityViewModel), 
            new PropertyMetadata(default(IGraph)));

        /// <summary> Выданный в задании граф </summary>
        public IGraph GivenGraph
        {
            get { return (IGraph)GetValue(GivenGraphProperty); }
            set { SetValue(GivenGraphProperty, value); }
        }

        /// <summary> Загрузка silvelight-модуля выполнена </summary>
        public static readonly DependencyProperty OnLoadedCmdProperty = DependencyProperty.Register(
            nameof(OnLoadedCmd), 
            typeof(ICommand), 
            typeof(ExternalStabilityViewModel), 
            new PropertyMetadata(default(ICommand)));

        /// <summary> Загрузка silvelight-модуля выполнена </summary>
        public ICommand OnLoadedCmd
        {
            get { return (ICommand)GetValue(OnLoadedCmdProperty); }
            set { SetValue(OnLoadedCmdProperty, value); }
        }

        /// <summary> Клик по вершине </summary>
        public static readonly DependencyProperty VertexClickCmdProperty = DependencyProperty.Register(
            nameof(VertexClickCmd), 
            typeof(ICommand), 
            typeof(ExternalStabilityViewModel), 
            new PropertyMetadata(default(ICommand)));

        /// <summary> Клик по вершине </summary>
        public ICommand VertexClickCmd
        {
            get { return (ICommand)GetValue(VertexClickCmdProperty); }
            set { SetValue(VertexClickCmdProperty, value); }
        }

        /// <summary>
        /// Множество внешней устойчивости, выбираемое студентом
        /// </summary>
        public static readonly DependencyProperty SetDESProperty = DependencyProperty.Register(
            nameof(SetDES),
            typeof(ObservableCollection<IVertex>),
            typeof(ExternalStabilityViewModel),
            new PropertyMetadata(default(ObservableCollection<IVertex>)));
        
        /// <summary>
        /// Множесто внешней устойчивости, выбираемое студентом
        /// </summary>
        public ObservableCollection<IVertex> SetDES
        {
            get { return (ObservableCollection<IVertex>)GetValue(SetDESProperty); }
            set { SetValue(SetDESProperty, value); }
        }

        /// <summary> Заполняемая студентом матрица </summary>
        public static readonly DependencyProperty MatrixProperty = DependencyProperty.Register(
            nameof(Matrix),
            typeof(ObservableCollection<MatrixRowViewModel<string>>),
            typeof(ExternalStabilityViewModel),
            new PropertyMetadata(default(ObservableCollection<MatrixRowViewModel<string>>)));

        /// <summary> Заполняемая студентом матрица </summary>
        public ObservableCollection<MatrixRowViewModel<string>> Matrix
        {
            get { return (ObservableCollection<MatrixRowViewModel<string>>)GetValue(MatrixProperty); }
            set { SetValue(MatrixProperty, value); }
        }
        
        /// <summary> Вершины из визуализатора </summary>
        public static DependencyProperty VertVisColProperty = DependencyProperty.Register(
            nameof(VertVisCol),
            typeof(ReadOnlyCollection<Graphs.UIComponents.Visualization.Vertex>),
            typeof(ExternalStabilityViewModel),
            new PropertyMetadata(default(ReadOnlyCollection<Graphs.UIComponents.Visualization.Vertex>)));

        /// <summary> Вершины из визуализатора </summary>
        public ReadOnlyCollection<Graphs.UIComponents.Visualization.Vertex> VertVisCol
        {
            get { return (ReadOnlyCollection<Graphs.UIComponents.Visualization.Vertex>)GetValue(VertVisColProperty); }
            set { SetValue(VertVisColProperty, value); }
        }

        /// <summary> Рёбра из визуализатора </summary>
        public static DependencyProperty EdgeVisColProperty = DependencyProperty.Register(
            nameof(EdgeVisCol),
            typeof(ReadOnlyCollection<Graphs.UIComponents.Visualization.Edge>),
            typeof(ExternalStabilityViewModel),
            new PropertyMetadata(default(ReadOnlyCollection<Graphs.UIComponents.Visualization.Edge>)));

        /// <summary> Рёбра из визуализатора </summary>
        public ReadOnlyCollection<Graphs.UIComponents.Visualization.Edge> EdgeVisCol
        {
            get { return (ReadOnlyCollection<Graphs.UIComponents.Visualization.Edge>)GetValue(EdgeVisColProperty); }
            set { SetValue(EdgeVisColProperty, value); }
        }

        /// <summary> Строки КСС для боковой панели в режиме "Конденсат" </summary>
        public static readonly DependencyProperty SccRowsProperty = DependencyProperty.Register(
            nameof(SccRows), 
            typeof(IList<SccRowViewModel>),
            typeof(ExternalStabilityViewModel),
            new PropertyMetadata(default(SccRowViewModel)));

        /// <summary> Строки КСС для боковой панели в режиме "Конденсат" </summary>
        public IList<SccRowViewModel> SccRows
        {
            get { return (IList<SccRowViewModel>)GetValue(SccRowsProperty); }
            set { SetValue(SccRowsProperty, value); }
        }
        #endregion


        /// <summary> Допустимые версии генератора </summary>
        protected override Version[] AllowedGeneratorVersions
        {
            get { return _allowedGeneratorVersions; }
        }

        /// <summary> Инициализация </summary>
        protected override void OnInitialized()
        {
            base.OnInitialized();

            UserActionsManager.PropertyChanged += (sender, args) => HandlePropertyChanged(args);
            VariantProvider.PropertyChanged += (sender, args) => HandlePropertyChanged(args);
            InitToolBarCommands();
            SubscribeToViewEvents();
        }

        private void SubscribeToViewEvents()
        {
            VertexClickCmd = new DelegateCommand(
                            o => OnVertexClick((IVertex) o),
                            o => true);
            //View.VertexClicked += (sender, args) => OnVertexClick(args.Control);
            View.VertexClicked += (sender, args) => VertexClickCmd.Execute(args.Control);
            View.Loaded += (sender, args) => StartVariantDownload();
            
        }

        /// <summary> Начать загрузку варианта </summary>
        public void StartVariantDownload()
        {
            VariantProvider.DownloadVariantAsync();
            //OnTaskLoadingComlete(e);
        }

        /// <summary> Клик по вершине </summary>
        public void OnVertexClick(IVertex vertex)
        {
            UserActionsManager.RegisterInfo(string.Format("Клик по вершине [{0}]", vertex.Name));
        }

        private void HandlePropertyChanged(PropertyChangedEventArgs args)
        {
            if (args.PropertyName == ExpressionUtility.NameForMember((IUiBlockerAsyncProcessor p) => p.IsBusy))
            {
                // Нас могли дёрнуть из другого потока, поэтому доступ к UI - через Dispatcher.
                Dispatcher.BeginInvoke(RecalculateIsLoadingData);
            }
        }

        private void RecalculateIsLoadingData()
        {
            IsLoadingData = VariantProvider.IsBusy || UserActionsManager.IsBusy;
        }

        /// <summary> Задание загружено </summary>
        /// <param name="e"></param>
        protected override void OnTaskLoadingComlete(VariantDownloadedEventArgs e)
        {
            // Мы вызваны из другого потока. Поэтому работаем с UI-элементами через Dispatcher.
            Dispatcher.BeginInvoke(() => {
                GivenGraph = GraphSerializer.Deserialize(e.Data);
                SccRows = new List<SccRowViewModel>();
                SetDES = new ObservableCollection<IVertex>();
                IsMouseVerticesMovingEnabled = true;

                _task = Task.t11;

                var matrix = new ObservableCollection<MatrixRowViewModel<string>>();
                for (var i = 0; i < GivenGraph.VerticesCount; ++i)
                {
                    var row = new ObservableCollection<string> { i.ToString() };
                    for (var j = 0; j < GivenGraph.VerticesCount; ++j)
                        row.Add("0");

                    row.CollectionChanged += RowChanged;
                    matrix.Add(new MatrixRowViewModel<string>(row));
                }
                Matrix = matrix;
            });
        }
        
        private ObservableCollection<string> _changedCollection;
        private NotifyCollectionChangedEventArgs _cellChangedArgs;
        private void RowChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                _cellChangedArgs = e;
                _changedCollection = (ObservableCollection<string>)sender;
            }

       
        
        /// <summary> Проверка матрицы </summary>
        public void checkMatrix()
        {
            int counter = CountOfErrorsMatrix(Matrix);
            if (counter > 0)
            {
                short k = (short) ((short) counter * 3);
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

                UserActionsManager.RegisterMistake(mistake,k);
            }
            else
            {
                MessageBox.Show("Задание 1.1 пройдено.\n Вы перешли к заданию 1.2.\n Ознакомьтесь со справкой.<?>");
                _task = Task.t12;
            }
        }

        /// <summary> Проверка матрицы для алгоритма </summary>

        public void checkMatrixforAghorithm()
        {
            int counter = CountOfErrorsMatrixforAlgorithm(Matrix);

            if (counter > 0)
            {
                short k = (short)((short)counter * 3);
                string mistake = null;
                switch (counter)
                {
                    case 1:
                        mistake = "Найдена " + counter.ToString() + " ошибка";
                        break;
                    case 2: case 3: case 4:
                        mistake = "Найдено " + counter.ToString() + " ошибки";
                        break;
                    default:
                        mistake = "Найдено " + counter.ToString() + " ошибок";
                        break;
                }
               
                UserActionsManager.RegisterMistake(mistake, k);
            }
            else
            {
                MessageBox.Show("Задание 1.2 пройдено.\n Вы перешли к заданию 2.\n Ознакомьтесь со справкой.<?>");
                for (int i = 0; i < GivenGraph.VerticesCount; ++i)
                {
                    Matrix[i].IsEnabled = false;
                }
                _task = Task.t2;
            }
        }

        /// <summary> Добавление вершины в список </summary>
        public void SelectRMouseClick(IVertex clickedVertex)
        {
            var vertex = GivenGraph.Vertices.Single(clickedVertex.Equals);
            

            // Если вершину уже добавили - то удаляем.
            if (SetDES.Contains(vertex))
            {
                VertVisCol[Convert.ToInt32(vertex.Name)].Background = new SolidColorBrush(defaultBackgroundColor);
                Matrix[Convert.ToInt32(vertex.Name)].Background = new SolidColorBrush(Color.FromArgb(250, 239, 240, 250));
                VertVisCol[Convert.ToInt32(vertex.Name)].BorderBrush = new SolidColorBrush(defaultBorderColor);
                
                SetDES.Remove(vertex);

                foreach (var edge in EdgeVisCol)
                {
                    if ((edge.Vertex1.Name == vertex.Name) || (edge.Vertex2.Name == vertex.Name))
                        edge.Stroke = new SolidColorBrush(defaultBorderColor);
                }

                foreach (var vert in SetDES)
                {
                    foreach (var edge in EdgeVisCol)
                    {
                        if ((edge.Vertex1.Name == vert.Name) || (edge.Vertex2.Name == vert.Name))
                            edge.Stroke = new SolidColorBrush(Color.FromArgb(255, 243, 117, 117));
                    }
                }
                return;

            }

            SetDES.Add(vertex);
            Matrix[Convert.ToInt32(vertex.Name)].Background = new SolidColorBrush(Color.FromArgb(250, 230, 207, 207));

            VertVisCol[Convert.ToInt32(vertex.Name)].Background = new SolidColorBrush(Color.FromArgb(250, 230, 207, 207));
            VertVisCol[Convert.ToInt32(vertex.Name)].BorderBrush = new SolidColorBrush(Color.FromArgb(255,243,117,117));

            foreach (var edge in EdgeVisCol)
            {
                if ((edge.Vertex1.Name == vertex.Name) || (edge.Vertex2.Name == vertex.Name))
                    edge.Stroke = new SolidColorBrush(Color.FromArgb(255,243,117,117));
            }

            UserActionsManager.RegisterInfo(string.Format("Выбрана вершина: {0}", clickedVertex.Name));
        }


        /// <summary>
        /// Проверка правильности заполнения матрицы смежности
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        private int CountOfErrorsMatrix(ObservableCollection<MatrixRowViewModel<string>> m)
        {
            var counter = 0;

            for (var i = 0; i < GivenGraph.VerticesCount; i++)
                for (var j = 0; j < GivenGraph.VerticesCount; j++)
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
        private int CountOfErrorsMatrixforAlgorithm(ObservableCollection<MatrixRowViewModel<string>> m)
        {
            int counter = 0;

            for (int i = 0; i < GivenGraph.VerticesCount; i++)
            {
                for (int j = 0; j < GivenGraph.VerticesCount; j++)
                {
                    var directEdge = GivenGraph[GivenGraph.Vertices[i], GivenGraph.Vertices[j]];

                    if ((i != j) && (m[i][j+1] == "1") && (directEdge == null))
                    {
                        counter++;
                    }
                    if ((i != j) && (m[i][j+1] != "1") && (directEdge != null))
                    {
                        counter++;
                    }
                    if ((i == j) && (m[i][j+1] != "1"))
                    {
                        counter++;
                    }
                }
            }

            return counter;
        }

        /// <summary>
        /// Проверка выбранного множества вершин на соответствие множеству внешней устойчивости
        /// </summary>
        /// <returns></returns>
        private bool isExternalStability()
        {
            var isExternal = true;
            var isAdded = false;
            var extendedSetofVertex = new Collection<IVertex>();

            foreach (var vertex in SetDES)
            {
                extendedSetofVertex.Add(vertex);
            }

            //Добавляем вершины, соседние с выбранными в расширенный набор вершин
            foreach (var vertex in SetDES)
            {
                foreach (var edge in GivenGraph.Edges)
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
            foreach (var vertex in GivenGraph.Vertices)
            {
                if (!extendedSetofVertex.Contains(vertex))
                {
                    isExternal = false;
                }
            }

            if (isExternal)
            {
                --_countOfSes;

                //Выбрано достаточное количество множеств внешней устойчивости
                if (_countOfSes == 0)
                {
                    MessageBox.Show("Задание 2 пройдено.\n Вы перешли к заданию 3.\n Ознакомьтесь со справкой.<?>");
                    _task = Task.t3;
                }
                UserActionsManager.RegisterInfo(string.Format(@"Множество добавлено. Осталось {0} множеств(о).",_countOfSes));



                var sccStr = new SccRowViewModel(BuildSccName(SetDES));

                //Поиск выбранного множества в списке всех множеств ???
                foreach (var sccRow in SccRows)
                {
                    if (sccRow.VerticesSet == sccStr.VerticesSet)
                    {
                        isAdded = true;
                        break;
                    }

                }

                //Проверка на уже добавленность выбранного множества
                if (isAdded)
                {
                    MessageBox.Show("Множество "+sccStr.VerticesSet+" уже добавлено.");
                    UserActionsManager.RegisterMistake("Множество " + sccStr.VerticesSet + " уже добавлено.",2);
                }
                else
                {
                    SccRows.Add(sccStr);
                }

                //Очищаем текущее множество выбранных вершин
                SetDES.Clear();

                //Визуальное изменение выбранных элементов
                foreach (var vertex in VertVisCol)
                {
                    vertex.BorderBrush = new SolidColorBrush(defaultBorderColor);
                    vertex.Background = new SolidColorBrush(defaultBackgroundColor);
                    Matrix[Convert.ToInt32(vertex.Name)].Background = new SolidColorBrush(Color.FromArgb(250, 239, 240, 250));
                }


                foreach (var edge in EdgeVisCol)
                {
                    edge.Stroke = new SolidColorBrush(defaultBorderColor);
                }

            }
            else
            {
                UserActionsManager.RegisterMistake("Это множество вершин не является внешне устойчивым.",10);
            }
            return true;
        }

        private const string SCC_NAME_DELIMITER = ", ";

        //Строковое представление выбранного множества ???
        private string BuildSccName(IEnumerable<IVertex> vertices)
        {
            return string.Join(SCC_NAME_DELIMITER,
                               vertices.Select(v => v.Name).OrderBy(s => s));
        }

        /// <summary>
        /// Проверка выбранного множества на соответствие минимальному множеству внешней устойчивости
        /// </summary>
        public void isthreedown()
        {
            var isAllMinimal = true;

            foreach (var sccRow in SccRows)
            {
                if ( (sccRow.IsBuilt == true) && (sccRow.VerticesSet.Length > 6))
                {
                   
                    isAllMinimal = false;
                }

                if ((sccRow.IsBuilt == false) && (sccRow.VerticesSet.Length < 7))
                {
                    isAllMinimal = false;
                }
            }
            if (isAllMinimal)
            {
                MessageBox.Show("Задание выполнено.");
                _task = Task.end;
            }
            else
            {
                UserActionsManager.RegisterMistake("Выбранные множество/а не являются минимальными./Выбранны не все множества", 10);
            }
        }


    }
}
