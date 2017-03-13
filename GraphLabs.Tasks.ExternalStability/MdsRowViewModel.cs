using System;
using System.Collections.Generic;
using System.Windows;
using GraphLabs.Graphs;
using System.Collections.ObjectModel;
using System.Linq;


namespace GraphLabs.Tasks.ExternalStability
{
    /// <summary> ViewModel для отображения на панели МВУ </summary>
    public class MdsRowViewModel : DependencyObject
    {
        /// <summary> № </summary>
        public static readonly DependencyProperty NumberProperty = DependencyProperty.Register(
            nameof(Number),
            typeof(int),
            typeof(MdsRowViewModel),
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

        /// <summary>
        /// Множество вершин
        /// </summary>
        public IList<Vertex> VerticesSet;  

        /// <summary> Множество вершин для вывода </summary>
        public static readonly DependencyProperty VerticesViewProperty = DependencyProperty.Register(
            nameof(VerticesView),
            typeof(string),
            typeof(MdsRowViewModel),
            new PropertyMetadata(default(string)));
        
        /// <summary> Множество вершин </summary>
        public string VerticesView
        {
            get { return (string)GetValue(VerticesViewProperty); }
            private set { SetValue(VerticesViewProperty, value); }
        }


        /// <summary> Уже построена? </summary>
        public static DependencyProperty IsBuiltProperty = DependencyProperty.Register(
            nameof(IsBuilt),
            typeof(bool),
            typeof(MdsRowViewModel),
            new PropertyMetadata(default(bool)));

        /// <summary> Уже построена? </summary>
        public bool IsBuilt
        {
            get { return (bool)GetValue(IsBuiltProperty); }
            set { SetValue(IsBuiltProperty, value); }
        }

        /// <summary> Обнуляем счётчик количества </summary>
        static MdsRowViewModel()
        {
            _count = 0;
        }

        /// <summary>
        /// Конструктор-клонизатор
        /// </summary>
        /// <param name="prototype"></param>
        public MdsRowViewModel (MdsRowViewModel prototype)
        {
            this.VerticesSet = prototype.VerticesSet;
            this.IsBuilt = prototype.IsBuilt;
            this.Number = ++_count;
            this.VerticesView = prototype.VerticesView;
        }

        private const string SccNameDelimiter = ", ";     
        private static string BuildMdsName(IEnumerable<Vertex> vertices)
        {
            string str = " {" + string.Join(SccNameDelimiter, vertices.Select(v => v.Name).OrderBy(s => s))  + "}";
            return str;
         }

    /// <summary> Ctor. </summary>
    public MdsRowViewModel(IList<Vertex> vertices, int count)
        {
            Number = count;
            VerticesSet = new List<Vertex>(vertices);
            VerticesView = BuildMdsName(vertices);
            IsBuilt = false;
        }

        /// <summary> Ctor. </summary>
   public MdsRowViewModel(IList<Vertex> vertices)
        {
            Number = ++_count;
            VerticesSet = new List<Vertex>(vertices);
            VerticesView = BuildMdsName(vertices);
            IsBuilt = false;
        }

        /// <summary> Ctor. </summary>
        public MdsRowViewModel(IList<Vertex> vertices, bool flag)
        {
            if (flag) Number = ++_count;
            VerticesSet = new List<Vertex>(vertices);
            VerticesView = BuildMdsName(vertices);
            IsBuilt = false;
        }


    }
}
