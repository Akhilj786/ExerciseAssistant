using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private const int MinimumScreenWidth = 1920;
        private const int MinimumScreenHeight = 1080;
        

        public MainWindow()
        {
            InitializeComponent();
             this.WindowState = System.Windows.WindowState.Maximized;
            this.WindowStyle = System.Windows.WindowStyle.None;

            if (Generics.loadingStatus == 0)
            {
               // _mainFrame.Source = new Uri("Page1.xaml", UriKind.Relative);
                _mainFrame.Source = new Uri("videopage.xaml", UriKind.Relative);
                Generics.loadingStatus = 1;
            }
       else
                _mainFrame.Source = new Uri("MainMenu.xaml", UriKind.Relative);
        }
           /* try
            {
                InitializeComponent();

                Loaded += MainWindow_Loaded;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
       
        }*/
    }

   /*id MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // get the main screen size
            //double height = SystemParameters.PrimaryScreenHeight;
            //double width = SystemParameters.PrimaryScreenWidth;

            // if the main screen is less than 1920 x 1080 then warn the user it is not the optimal experience 
           /* if ((width < MinimumScreenWidth) || (height < MinimumScreenHeight))
            {
                MessageBoxResult continueResult = MessageBox.Show(Properties.Resources.SuboptimalScreenResolutionMessage, Properties.Resources.SuboptimalScreenResolutionTitle, MessageBoxButton.YesNo);
                if (continueResult == MessageBoxResult.No)
                {
                    this.Close();
                }
            } */
          /*  this.WindowState = System.Windows.WindowState.Maximized;
            this.WindowStyle = System.Windows.WindowStyle.None;

            if (Generics.loadingStatus == 0)
            {
                _mainFrame.Source = new Uri("Page1.xaml", UriKind.Relative);
                Generics.loadingStatus = 1;
            }
       else
                _mainFrame.Source = new Uri("MainMenu.xaml", UriKind.Relative);
        }*/
        
 
    }
