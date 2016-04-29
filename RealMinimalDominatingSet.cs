using System.Collections.Generic;
using GraphLabs.Graphs;
using GraphLabs.CommonUI.Controls.ViewModels;
using System.Collections.ObjectModel;



namespace GraphLabs.Tasks.ExternalStability
{
    /// <summary>
    /// Минимальное множество внешней устойчивости
    /// </summary>
    public class RealMinimalDominatingSet
    {
        /// <summary>
        /// Список минимальных множеств внешней устойчивости
        /// </summary>
        public List<IList<MinDSVertexViewModel>> MinDS;

        /// <summary>
        /// Размер минимального множества внешней устойчивости
        /// </summary>
        public int MinSize;

        /// <summary>
        /// Функция поиска минимальных множеств внешней устойчивости
        /// </summary>
       public void FindAllMinDS(IGraph graph, ObservableCollection<MatrixRowViewModel<string>> matrix)
        {
            var FinderStep = new TemporalDS();
            FinderStep.FindMinDS(0, graph, matrix, MinDS, MinSize);
        }
    }

}