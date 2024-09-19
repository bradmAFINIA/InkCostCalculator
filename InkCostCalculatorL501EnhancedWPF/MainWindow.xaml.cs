using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using InkCostModel;
using NLog;

namespace InkCostCalculatorL501EnhancedWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        InkCostController inkCostController;
        public static Logger logger = LogManager.GetCurrentClassLogger();

        static void ExceptionMessage()
        {
            const string caption = "Critical Error";
            const string message = "Application exception.  Please contact Afinia Label Technical Support.";
            var result = System.Windows.Forms.MessageBox.Show(message, caption,
                                         MessageBoxButtons.OK,
                                         MessageBoxIcon.Exclamation);
            Environment.Exit(1);
        }

        static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            // Log the exception, display it
            Debug.WriteLine(e.Exception.Message);
            logger.Error($"{e.Exception.Message}");
            ExceptionMessage();
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // Log the exception, display it
            Debug.WriteLine((e.ExceptionObject as Exception).Message);
            logger.Error($"{(e.ExceptionObject as Exception).Message}");
            logger.Error(e.ExceptionObject as Exception, "Exception");
            ExceptionMessage();
        }

        public MainWindow()
        {
            System.Windows.Forms.Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            InitializeComponent();
            try
            {
                inkCostController = new InkCostController();
                //MainFrame.Content = new InkCostPage(inkCostController);
                MainFrame.Content = new InkCostHomePage(inkCostController);
                //this.DataContext = inkCostController.viewModel;
                logger.Debug("MainWindow Initialized");
            }
            catch (Exception ex)
            {
                logger.Debug("MainWindow");
                logger.Error($"{ex.Message}. {ex.InnerException}");
                ExceptionMessage();
            }
        }
        ~MainWindow()
        {
            try
            {
                inkCostController.StopWatcher();
            }
            catch { }
        }
    }
}
