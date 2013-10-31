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
    /// Interaction logic for videopage.xaml
    /// </summary>
    public partial class videopage : Page
    {
        public videopage()
        {
            InitializeComponent();
        }

        private void MediaTimeline_Completed(object sender, EventArgs e)
        {
            this.rev_exercise_avi.Visibility = Visibility.Collapsed;
            _mainFrame.Source = new Uri("MainMenu.xaml", UriKind.Relative);
        }
    }
}
