using System.Windows;
using GraphLabs.Graphs;
using System.Collections.ObjectModel;

namespace GraphLabs.Tasks.ExternalStability
{
    /// <summary> ViewModel КСС для отображения на панели при построении конденсата </summary>
    public class SccRowViewModel : DependencyObject
    {
        /// <summary> № </summary>
        public static readonly DependencyProperty NumberProperty = DependencyProperty.Register(
            nameof(Number),
            typeof(int),
            typeof(SccRowViewModel),
            new PropertyMetadata(default(int)));
        
        /// <summary> № </summary>
        public int Number
        {
            get { return (int)GetValue(NumberProperty); }
            set { SetValue(NumberProperty, value); }
        }

        /// <summary>
        /// Счётчик количества
        /// </summary>
        private static int _count;

        /// <summary> Множество вершин </summary>
        public static readonly DependencyProperty VerticesSetProperty = DependencyProperty.Register(
            nameof(VerticesSet),
            typeof(ObservableCollection<IVertex>),
            typeof(SccRowViewModel),
            new PropertyMetadata(default(string)));
        
        /// <summary> Множество вершин </summary>
        public ObservableCollection<IVertex> VerticesSet
        {
            get { return (ObservableCollection<IVertex>)GetValue(VerticesSetProperty); }
            set { SetValue(VerticesSetProperty, value); }
        }


        /// <summary> Уже построена? </summary>
        public static DependencyProperty IsBuiltProperty = DependencyProperty.Register(
            nameof(IsBuilt),
            typeof(bool),
            typeof(SccRowViewModel),
            new PropertyMetadata(default(bool)));

        /// <summary> Уже построена? </summary>
        public bool IsBuilt
        {
            get { return (bool)GetValue(IsBuiltProperty); }
            set { SetValue(IsBuiltProperty, value); }
        }

        /// <summary> Обнуляем счётчик количества </summary>
        static SccRowViewModel()
        {
            _count = 0;
        }

        /// <summary> Ctor. </summary>
        public SccRowViewModel(ObservableCollection<IVertex> vertices)
        {
            Number = ++_count;
            VerticesSet = vertices;
            IsBuilt = false;
        }
        

    }
}
