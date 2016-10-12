using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using GraphLabs.Common;
using GraphLabs.CommonUI;
using GraphLabs.CommonUI.Controls.ViewModels;
using GraphLabs.Graphs;
using GraphLabs.Graphs.DataTransferObjects.Converters;
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
            SetEs,
        }
        /// <summary> Текущее подзадание </summary>
    private enum Task
        {
            /// <summary>Заполнение матрицы </summary>
            TaskAdjacencyMatrix,
            /// <summary> Изменение матрицы для алгоритма нахождения ES </summary>
            TaskModifiedAdjMatrix,
            /// <summary> Добавление множеств ES </summary>
            TaskSelectDomSets,
            /// <summary> Вывод о числе внешней устойчивости </summary>
            TaskFindMinDomSets,
            /// <summary>Задание выполнено</summary>
            TaskEnd
        }

        /// <summary> Текущее задание </summary>
        private Task _task;

        /// <summary>
        /// Требуемое число нахождения множеств внешней устойчивости
        /// </summary>
        private int _countOfSes;

        private int _dsCount;

        /// <summary> Текущее состояние </summary>
        private State _state;

        /// <summary> Допустимые версии генератора, с помощью которого сгенерирован вариант </summary>
        private readonly Version[] _allowedGeneratorVersions = new[] {  new Version(1, 0) };


        private readonly Color _defaultBorderColor     = Color.FromArgb(255, 50, 133, 144);
        private readonly Color _defaultBackgroundColor = Color.FromArgb(250, 207, 207, 207);
       

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
            typeof(UndirectedGraph), 
            typeof(ExternalStabilityViewModel), 
            new PropertyMetadata(default(IGraph)));

        /// <summary> Выданный в задании граф </summary>
        public UndirectedGraph GivenGraph
        {
            get { return (UndirectedGraph)GetValue(GivenGraphProperty); }
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
        public static readonly DependencyProperty DomSetProperty = DependencyProperty.Register(
            nameof(DomSet),
            typeof(ObservableCollection<Vertex>),
            typeof(ExternalStabilityViewModel),
            new PropertyMetadata(default(ObservableCollection<Vertex>)));
        
        /// <summary>
        /// Множесто внешней устойчивости, выбираемое студентом
        /// </summary>
        public ObservableCollection<Vertex> DomSet
        {
            get { return (ObservableCollection<Vertex>)GetValue(DomSetProperty); }
            set { SetValue(DomSetProperty, value); }
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
        public static readonly DependencyProperty MdsRowsProperty = DependencyProperty.Register(
            nameof(MdsRows), 
            typeof(IList<MdsRowViewModel>),
            typeof(ExternalStabilityViewModel),
            new PropertyMetadata(default(MdsRowViewModel)));

        /// <summary> Строки КСС для боковой панели в режиме "Конденсат" </summary>
        public IList<MdsRowViewModel> MdsRows
        {
            get { return (IList<MdsRowViewModel>)GetValue(MdsRowsProperty); }
            set { SetValue(MdsRowsProperty, value); }
        }

        /// <summary>
        /// Реальное совокупность наименьших доминирующих множеств
        /// </summary>
        public IList<MdsRowViewModel> RealMdsRows;
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
                            o => OnVertexClick((Vertex) o),
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
        public void OnVertexClick(Vertex vertex)
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
            Dispatcher.BeginInvoke(
                () =>
                {
                    try
                    {
                        GivenGraph = (UndirectedGraph) VariantSerializer.Deserialize(e.Data)[0];
                    }
                    catch (InvalidCastException ex)
                    {
                        throw new InvalidCastException("Входной граф должен быть неориентированным.", ex);
                    }

                    MdsRows = new ObservableCollection<MdsRowViewModel>();
                    DomSet = new ObservableCollection<Vertex>();
                    IsMouseVerticesMovingEnabled = true;
                    var minDsCount = new MinDSEvaluator(GivenGraph);
                    minDsCount.Evaluate(GivenGraph, true);
                    RealMdsRows = new List<MdsRowViewModel>();
                    foreach (var minDs in minDsCount.MinDs)
                    {
                        var tempScc = new MdsRowViewModel(minDs);
                        RealMdsRows.Add(tempScc);
                    }
                    var minimalDsCount = new MinDSEvaluator(GivenGraph);
                    minimalDsCount.Evaluate(GivenGraph, false);
                    var tempCount = 0;
                    foreach (var minDs in minimalDsCount.MinDs)
                    {
                        tempCount++;
                    }
                    _task = Task.TaskAdjacencyMatrix;
                    _dsCount = tempCount - 1;
                    _countOfSes = _dsCount;

                    var matrix = new ObservableCollection<MatrixRowViewModel<string>>();
                    for (var i = 0; i < GivenGraph.VerticesCount; ++i)
                    {
                        var row = new ObservableCollection<string> {i.ToString()};
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
        private void CheckMatrix()
        {
            var counter = new MatrixErrorCounter();
            var amount = counter.CountOfErrorsMatrix(Matrix, GivenGraph);
            if (amount > 0)
            {
                short k = (short) ((short) amount*3);
                string mistake = null;
                switch (amount)
                {
                    case 1:
                        mistake = "Найдена " + amount + " ошибка";
                        break;
                    case 2:
                    case 3:
                    case 4:
                        mistake = "Найдено " + amount + " ошибки";
                        break;
                    default:
                        mistake = "Найдено " + amount + " ошибок";
                        break;
                }

                if (UserActionsManager.Score > k) UserActionsManager.RegisterMistake(mistake, k);
                else if (UserActionsManager.Score > 0)
                {
                    UserActionsManager.RegisterMistake(mistake,(short) UserActionsManager.Score);
                }

            }
            else
            {
                MessageBox.Show("Задание 1.1 пройдено.\n Вы перешли к заданию 1.2.\n Ознакомьтесь со справкой.<?>");
                _task = Task.TaskModifiedAdjMatrix;
            }
        }

        /// <summary> Проверка матрицы для алгоритма </summary>

        private void CheckMatrixforAghorithm()
        {
            var counter = new MatrixErrorCounter();
            int amount = counter.CountOfErrorsMatrixforAlgorithm(Matrix, GivenGraph);
            if (amount > 0)
            {
                short k = (short)((short)amount * 3);
                string mistake = null;
                switch (amount)
                {
                    case 1:
                        mistake = "Найдена " + amount + " ошибка";
                        break;
                    case 2:
                    case 3:
                    case 4:
                        mistake = "Найдено " + amount + " ошибки";
                        break;
                    default:
                        mistake = "Найдено " + amount + " ошибок";
                        break;
                }

                if (UserActionsManager.Score > k) UserActionsManager.RegisterMistake(mistake, k);
                else if (UserActionsManager.Score > 0)
                {
                    UserActionsManager.RegisterMistake(mistake, (short)UserActionsManager.Score);
                }
            }
            else
            {
                MessageBox.Show("Задание 1.2 пройдено.\n Вы перешли к заданию 2.\n Ознакомьтесь со справкой.<?>");
                for (var i = 0; i < GivenGraph.VerticesCount; ++i)
                {
                    Matrix[i].IsEnabled = false;
                }
                _task = Task.TaskSelectDomSets;
            }
        }

        /// <summary> Добавление вершины в список </summary>
        public void SelectRMouseClick(IVertex clickedVertex)
        {
            var vertex = GivenGraph.Vertices.Single(clickedVertex.Equals);
            

            // Если вершину уже добавили - то удаляем.
            if (DomSet.Contains(vertex))
            {
                VertVisCol[Convert.ToInt32(vertex.Name)].Background = new SolidColorBrush(_defaultBackgroundColor);
                Matrix[Convert.ToInt32(vertex.Name)].Background = new SolidColorBrush(Color.FromArgb(250, 239, 240, 250));
                VertVisCol[Convert.ToInt32(vertex.Name)].BorderBrush = new SolidColorBrush(_defaultBorderColor);
                
                DomSet.Remove(vertex);

                foreach (var edge in EdgeVisCol)
                {
                    if ((edge.Vertex1.Name == vertex.Name) || (edge.Vertex2.Name == vertex.Name))
                        edge.Stroke = new SolidColorBrush(_defaultBorderColor);
                }

                foreach (var vert in DomSet)
                {
                    foreach (var edge in EdgeVisCol)
                    {
                        if ((edge.Vertex1.Name == vert.Name) || (edge.Vertex2.Name == vert.Name))
                            edge.Stroke = new SolidColorBrush(Color.FromArgb(255, 243, 117, 117));
                    }
                }
                return;

            }

            DomSet.Add(vertex);
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
        /// Проверка выбранного множества вершин на соответствие множеству внешней устойчивости
        /// </summary>
        /// <returns></returns>
        private bool ValidateSet()
        {
            var isAdded = false;
            var setChecker = new CheckSet();
            bool isExternal = setChecker.IsExternalStability(DomSet, GivenGraph);
            var sccStr = new MdsRowViewModel(DomSet);
            var isMinimal = setChecker.IsMinimal(DomSet, GivenGraph);
            if (isExternal)
            {
                if (isMinimal)
                {
                    --_countOfSes;
                    //Выбрано достаточное количество множеств внешней устойчивости
                    if (_countOfSes == 0)
                    {
                        MessageBox.Show("Задание 2 пройдено.\n Вы перешли к заданию 3.\n Ознакомьтесь со справкой.<?>");
                        _task = Task.TaskFindMinDomSets;
                    }
                    if (UserActionsManager.Score > _countOfSes)
                        UserActionsManager.RegisterInfo(string.Format(@"Множество добавлено. Осталось {0} множеств(о).",
                            _countOfSes));
                    else if (UserActionsManager.Score > 0)
                    {
                        UserActionsManager.RegisterInfo(string.Format(@"Множество добавлено. Осталось {0} множеств(о).",
                            UserActionsManager.Score));
                    }
                    //Поиск выбранного множества в списке всех множеств ???
                    foreach (var sccRow in MdsRows)
                    {
                        if (sccRow.VerticesView == sccStr.VerticesView)
                        {
                            isAdded = true;
                            break;
                        }

                    }
                    //Проверка на уже добавленность выбранного множества
                    if (isAdded)
                    {
                        _countOfSes++;
                        MessageBox.Show("Множество " + sccStr.VerticesView + " уже добавлено.");
                        if (UserActionsManager.Score > 2)
                            UserActionsManager.RegisterMistake("Множество " + sccStr.VerticesView + " уже добавлено.", 2);
                        else if (UserActionsManager.Score > 0)
                        {
                            UserActionsManager.RegisterMistake("Множество " + sccStr.VerticesView + " уже добавлено.",
                                (short) UserActionsManager.Score);
                        }
                    }
                    else
                    {
                        MdsRows.Add(sccStr);
                    }

                    //Очищаем текущее множество выбранных вершин
                    DomSet.Clear();

                    //Визуальное изменение выбранных элементов
                    foreach (var vertex in VertVisCol)
                    {
                        vertex.BorderBrush = new SolidColorBrush(_defaultBorderColor);
                        vertex.Background = new SolidColorBrush(_defaultBackgroundColor);
                        Matrix[Convert.ToInt32(vertex.Name)].Background =
                            new SolidColorBrush(Color.FromArgb(250, 239, 240, 250));
                    }


                    foreach (var edge in EdgeVisCol)
                    {
                        edge.Stroke = new SolidColorBrush(_defaultBorderColor);
                    }
                }
                else
                {
                    if (UserActionsManager.Score > 10)
                        UserActionsManager.RegisterMistake("Это множество вершин не является минимальным.", 10);
                    else if (UserActionsManager.Score > 0)
                    {
                        UserActionsManager.RegisterMistake("Это множество вершин не является минимальным.",
                            (short)UserActionsManager.Score);
                    }
                }
            }
            else
            {
                if (UserActionsManager.Score > 10)
                    UserActionsManager.RegisterMistake("Это множество вершин не является внешне устойчивым.", 10);
                else if (UserActionsManager.Score > 0)
                {
                    UserActionsManager.RegisterMistake("Это множество вершин не является внешне устойчивым.",
                        (short) UserActionsManager.Score);
                }
            }
            return true;
        }

        /// <summary>
        /// Проверка выбранного множества на соответствие наименьшему множеству внешней устойчивости
        /// </summary>
        private void IsMinDS()
        {
 
            var numofChosen = 0;
            foreach (var sccRow in MdsRows)
            {
                if (sccRow.IsBuilt)
                {
                    numofChosen++;
                }
            }
            foreach (var realSccRow in RealMdsRows)
            {   
                foreach (var sccRow in MdsRows)
                {
                    if (sccRow.IsBuilt)
                    {
                        var tempSize = 0;
                        for (int i = 0; i < realSccRow.VerticesSet.Count; i++)
                        {
                            for (int j = 0; j < sccRow.VerticesSet.Count; j++)
                            {
                                if (realSccRow.VerticesSet[i].Name == sccRow.VerticesSet[j].Name) tempSize++;
                                if ((tempSize == realSccRow.VerticesSet.Count) && (realSccRow.VerticesSet.Count == sccRow.VerticesSet.Count))
                                {
                                    realSccRow.IsBuilt = true;
                                }
                            }
                        }
                    }
                }
            }
            var flag = true;
            var flag2 = true;
            var k = "";
            var numOfBuilt = 0;
            var m = 0;
            foreach (var realSccRow in RealMdsRows)
            {
                if (realSccRow.IsBuilt)
                {
                    numOfBuilt++;
                }
                else
                {
                    flag = false;
                    k = k + realSccRow.VerticesView;
                    m = m + 5;
                }
            }
            if (numOfBuilt != numofChosen)
            {
                flag2 = false;
            }

            if (flag && flag2)
            {
                MessageBox.Show("Задание выполнено. Нажмите ещё раз кнопку ОК для выхода.");
                _task = Task.TaskEnd;
            }
            else if (!flag2)
            {
                k = "Выбранные множества не являются минимальными. Количество множеств не совпадает";
                if (UserActionsManager.Score > 5) UserActionsManager.RegisterMistake(k, 5);
                else if (UserActionsManager.Score > 0)
                {
                    UserActionsManager.RegisterMistake(k, (short) UserActionsManager.Score);
                }
                MessageBox.Show("Неправильно выбраны множества. Повторите выполнение 2 и 3 заданий.");
                _task = Task.TaskSelectDomSets;
                MdsRows = new ObservableCollection<MdsRowViewModel>();
                DomSet = new ObservableCollection<Vertex>();
                _countOfSes = _dsCount;
            }
            else
            {
                k = "Выбранные множества не являются минимальными. Выбраны не все множества";
                if (UserActionsManager.Score > m) UserActionsManager.RegisterMistake(k, (short) m);
                else if (UserActionsManager.Score > 0)
                {
                    UserActionsManager.RegisterMistake(k, (short) UserActionsManager.Score);
                }
                MessageBox.Show("Неправильно выбраны множества. Повторите выполнение 2 и 3 заданий.");
                _task = Task.TaskSelectDomSets;
                MdsRows = new ObservableCollection<MdsRowViewModel>();
                DomSet = new ObservableCollection<Vertex>();
                _countOfSes = _dsCount;
            }
        }
    }
}
