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
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace WeatherApp2
{
    /// <summary>
    /// Interaction logic for map.xaml
    /// </summary>
    public partial class map : Window
    {
        public map()
        {
            InitializeComponent();
        }

        private void nlImage_Mouse_Down(object sender, MouseButtonEventArgs e)
        {
            Point clickPoint = e.GetPosition(nlImage);
            MessageBox.Show(clickPoint.ToString());
        }
    }
}
