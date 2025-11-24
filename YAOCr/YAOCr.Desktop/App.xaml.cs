using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using WinUIEx;
using YAOCr.Bootstrap;
using YAOCr.Core.Plugins;
using YAOCr.Views;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace YAOCr {
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application {
        private Window? m_window;

        // Needed to expose the MainWindow for theme switching
        public Window MainWin => m_window;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App() {
            this.InitializeComponent();

            RegisterServices();
            
            LoadPlugins();
        }

        private static void RegisterServices() {
            Ioc.Default.ConfigureServices(
                        new ServiceCollection()
                            .RegisterConfiguration()
                            .RegisterViewModels()
                            .RegisterProviders()
                            .RegisterServices()
                            .BuildServiceProvider());
        }

        private static void LoadPlugins() {
            PluginsLoader.Initialize(Ioc.Default.GetService<IConfiguration>());
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs args) {
            m_window = new MainWindow();

            m_window.CenterOnScreen();
            m_window.Activate();
        }

        
    }
}
