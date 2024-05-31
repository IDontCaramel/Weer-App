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
        public class ReferencePoint
        {
            public double scrX { get; set; }
            public double scrY { get; set; }
            public float lat { get; set; }
            public float lng { get; set; }
            public GlobalPosition pos { get; set; }
        }

        public class GlobalPosition
        {
            public double x { get; set; }
            public double y { get; set; }
            public double perX { get; set; }
            public double perY { get; set; }
        }

        public class CoordinateProjection
        {
            private const double radius = 6371; // Earth Radius in Km

            // This function converts lat and lng coordinates to GLOBAL X and Y positions
            public static GlobalPosition LatlngToGlobalXY(double lat, double lng, ReferencePoint p0, ReferencePoint p1)
            {
                // Calculates x based on cos of average of the latitudes
                double x = radius * lng * Math.Cos((p0.lat + p1.lat) / 2);
                // Calculates y based on latitude
                double y = radius * lat;
                return new GlobalPosition { x = x, y = y };
            }

            // This function converts lat and lng coordinates to SCREEN X and Y positions
            public static (double, double) LatlngToScreenXY(double lat, double lng, ReferencePoint p0, ReferencePoint p1)
            {
                // Calculate global X and Y for projection point
                GlobalPosition pos = LatlngToGlobalXY(lat, lng, p0, p1);
                // Calculate the percentage of Global X position in relation to total global width
                pos.perX = ((pos.x - p0.pos.x) / (p1.pos.x - p0.pos.x));
                // Calculate the percentage of Global Y position in relation to total global height
                pos.perY = ((pos.y - p0.pos.y) / (p1.pos.y - p0.pos.y));

                // Returns the screen position based on reference points
                double x = p0.scrX + (p1.scrX - p0.scrX) * pos.perX;
                double y = p0.scrY + (p1.scrY - p0.scrY) * pos.perY;
                return (x, y);
            }

            // This function converts SCREEN X and Y positions to lat and lng coordinates
            public static (double, double) ScreenXYToLatlng(double screenX, double screenY, ReferencePoint p0, ReferencePoint p1)
            {
                double perX = (screenX - p0.scrX) / (p1.scrX - p0.scrX);
                double perY = (screenY - p0.scrY) / (p1.scrY - p0.scrY);

                double globalX = p0.pos.x + perX * (p1.pos.x - p0.pos.x);
                double globalY = p0.pos.y + perY * (p1.pos.y - p0.pos.y);

                double lat = globalY / radius;
                double lng = globalX / (radius * Math.Cos((p0.lat + p1.lat) / 2));

                return (lat, lng);
            }
        }
        private void nlImage_Mouse_Down(object sender, MouseButtonEventArgs e)
        {
            Point clickPoint = e.GetPosition(nlImage);
            MessageBox.Show(clickPoint.ToString());
            planet(clickPoint.X, clickPoint.Y);
        }
        private void planet(double x , double y)
        {
            var p0 = new ReferencePoint
            {
                scrX = 320,       // Minimum X position on screen
                scrY = 480,        // Minimum Y position on screen
                lat = 52.3295182f,   // Latitude
                lng = 4.9227935f    // Longitude
            };

            // Bottom-right reference point
            var p1 = new ReferencePoint
            {
                scrX = 461,         // Maximum X position on screen
                scrY = 919,       // Maximum Y position on screen
                lat = 50.8234882f,   // Latitude
                lng = 5.706143f   // Longitude
            };

            // Calculate global X and Y for top-left reference point
            p0.pos = CoordinateProjection.LatlngToGlobalXY(p0.lat, p0.lng, p0, p1);
            // Calculate global X and Y for bottom-right reference point
            p1.pos = CoordinateProjection.LatlngToGlobalXY(p1.lat, p1.lng, p0, p1);

            // Usage example
            /*var screenX = 150;
            var screenY = 100;*/
            var latlng = CoordinateProjection.ScreenXYToLatlng(x, y, p0, p1);
            MessageBox.Show($"Latitude: {latlng.Item1}, Longitude: {latlng.Item2}");
        }
    }
}
