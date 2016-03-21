using System.Collections.Generic;
using System.Windows.Controls;
using GraphLabs.CommonUI;
using GraphLabs.CommonUI.Configuration;
using GraphLabs.Tasks.ExternalStability.Configuration;

namespace GraphLabs.Tasks.ExternalStability
{
    /// <summary> TaskTemplate app </summary>
    public partial class App : TaskApplicationBase
    {
       
        /// <summary> Получить конфигураторы сервисов </summary>
        private static IEnumerable<IDependencyResolverConfigurator> GetConfigurators()
        {
            // Wcf-сервисы
            yield return GetWcfServicesConfigurator();

            // Построитель View - сделано так, потому что в Xaml Silverlight нельзя подсунуть Generic
            yield return new ViewBuilderConfigurator<ViewBuilder<ExternalStability, ExternalStabilityViewModel>>();

            yield return new CommonItemsConfigurator();
        }

        private static IDependencyResolverConfigurator GetWcfServicesConfigurator()
        {
            return Current.IsRunningOutOfBrowser
                ? (IDependencyResolverConfigurator)new MockedWcfServicesConfigurator()
                {
                    GettingVariantDelay = 500
                }
                : (IDependencyResolverConfigurator)new WcfServicesConfigurator();
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        public App() : base(GetConfigurators())
        {
            InitializeComponent();
        }
    }
}
