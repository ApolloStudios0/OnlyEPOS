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

namespace OnlyEPOS.Startup
{
    public partial class EntranceLoadScreen : Page
    {
        public EntranceLoadScreen()
        {
            InitializeComponent();
            FinishedLoading();
        }

        async void FinishedLoading()
        {
            await Task.Delay(1000);
            OnlyText.Foreground = new SolidColorBrush(Color.FromRgb(249, 110, 22));
            await Task.Delay(1000);
            EPOSText.Foreground = new SolidColorBrush(Color.FromRgb(249, 110, 22));
        }
    }
}
