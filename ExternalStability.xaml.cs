using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using GraphLabs.CommonUI;
using GraphLabs.CommonUI.Controls.ViewModels;
using GraphLabs.Graphs.UIComponents.Visualization;

namespace GraphLabs.Tasks.ExternalStability
{
    /// <summary> Поиск компонент сильной связности, построение конденсата </summary>
    public partial class ExternalStability : TaskViewBase
    {
        #region Команды

        /// <summary> Клик по вершине </summary>
        public static readonly DependencyProperty VertexClickCommandProperty = DependencyProperty.Register(
            "VertexClickCommand", 
            typeof(ICommand), 
            typeof(ExternalStability), 
            new PropertyMetadata(default(ICommand)));

        /// <summary> Клик по вершине </summary>
        public ICommand VertexClickCommand
        {
            get { return (ICommand)GetValue(VertexClickCommandProperty); }
            set { SetValue(VertexClickCommandProperty, value); }
        }

        /// <summary> Загрузка silvelight-модуля выполнена </summary>
        public static readonly DependencyProperty OnLoadedCommandProperty =
            DependencyProperty.Register("OnLoadedCommand", typeof(ICommand), typeof(ExternalStability), new PropertyMetadata(default(ICommand)));

        /// <summary> Загрузка silvelight-модуля выполнена </summary>
        public ICommand OnLoadedCommand
        {
            get { return (ICommand)GetValue(OnLoadedCommandProperty); }
            set { SetValue(OnLoadedCommandProperty, value); }
        }

        #endregion
        /// <summary> Вершины из визуализатора </summary>
        public static DependencyProperty VertVisProperty =
            DependencyProperty.Register("VertVis", 
                                        typeof(ReadOnlyCollection<Vertex>), 
                                        typeof(ExternalStability), 
                                        new PropertyMetadata(default(ReadOnlyCollection<Vertex>)));


        /// <summary> Вершины из визуализатора </summary>
        public ReadOnlyCollection<Vertex> VertVis
        {
            get { return (ReadOnlyCollection<Vertex>)GetValue(VertVisProperty); }
            set { SetValue(VertVisProperty, value); }
        }

        /// <summary> Рёбра из визуализатора </summary>
        public static DependencyProperty EdgeVisProperty =
            DependencyProperty.Register("EdgeVis", 
                                        typeof(ReadOnlyCollection<Edge>), 
                                        typeof(ExternalStability), 
                                        new PropertyMetadata(default(ReadOnlyCollection<Edge>)));

        /// <summary> Рёбра из визуализатора </summary>
        public ReadOnlyCollection<Edge> EdgeVis
        {
            get { return (ReadOnlyCollection<Edge>)GetValue(EdgeVisProperty); }
            set { SetValue(EdgeVisProperty, value); }
        }

        /// <summary> Рёбра из визуализатора </summary>
        public static DependencyProperty MatrixVisProperty =
            DependencyProperty.Register("MatrixVis",
                                        typeof(ReadOnlyCollection<MatrixRowViewModel<string>>),
                                        typeof(ExternalStability),
                                        new PropertyMetadata(default(ReadOnlyCollection<MatrixRowViewModel<string>>)));

        /// <summary> Рёбра из визуализатора </summary>
        public ReadOnlyCollection<MatrixRowViewModel<string>> MatrixVis
        {
            get { return (ReadOnlyCollection<MatrixRowViewModel<string>>) GetValue(MatrixVisProperty); }
            set { SetValue(MatrixVisProperty, value); }
        }
        
        /// <summary> Ctor. </summary>
        public ExternalStability()
        {
            InitializeComponent();
            
            // Куча Binding'ов (в реальных заданиях)
            SetBinding(VertexClickCommandProperty, new Binding("VertexClickCmd"));
            SetBinding(OnLoadedCommandProperty, new Binding("OnLoadedCmd"));
            SetBinding(VertVisProperty, new Binding("VertVisCol") { Mode = BindingMode.TwoWay });
            SetBinding(EdgeVisProperty, new Binding("EdgeVisCol") { Mode = BindingMode.TwoWay });
            
        }


        /// <summary> Клик по вершине </summary>
        public event EventHandler<VertexClickEventArgs> VertexClicked;

        private void OnVertexClicked(VertexClickEventArgs e)
        {
            var handler = VertexClicked;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void OnVertexClick(object sender, VertexClickEventArgs e)
        {
            OnVertexClicked(e);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (OnLoadedCommand != null)
            {
                OnLoadedCommand.Execute(null);

            }
            VertVis = Visualizer.Vertices;
            EdgeVis = Visualizer.Edges;

        }

        private void GraphVisualizer_Loaded()
        {

        }

    }
}
