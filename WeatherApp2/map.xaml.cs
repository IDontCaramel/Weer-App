using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace WeatherApp2
{
    public partial class map : Window
    {
        public static string City { get; set; }
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

            this.Close();
        }

        private void planet(double x, double y)
        {
            var p0 = new ReferencePoint
            {
                scrX = 309,       // Minimum X position on screen
                scrY = 465,        // Minimum Y position on screen
                lat = 52.3295182f,   // Latitude
                lng = 4.9227935f    // Longitude
            };

            // Bottom-right reference point
            var p1 = new ReferencePoint
            {
                scrX = 446,         // Maximum X position on screen
                scrY = 888,       // Maximum Y position on screen
                lat = 50.8234882f,   // Latitude
                lng = 5.706143f   // Longitude
            };

            // Calculate global X and Y for top-left reference point
            p0.pos = CoordinateProjection.LatlngToGlobalXY(p0.lat, p0.lng, p0, p1);
            // Calculate global X and Y for bottom-right reference point
            p1.pos = CoordinateProjection.LatlngToGlobalXY(p1.lat, p1.lng, p0, p1);

            // Calculate lat and lng based on screen coordinates
            var latlng = CoordinateProjection.ScreenXYToLatlng(x, y, p0, p1);
          /*  MessageBox.Show($"Latitude: {latlng.Item1}, Longitude: {latlng.Item2}");*/

            // Find the closest location
            var closestCity = FindClosestCity(latlng.Item1, latlng.Item2);
            MessageBox.Show($"Closest City: {closestCity.Key}, Coordinates: ({closestCity.Value.X}, {closestCity.Value.Y}, lattuide: {latlng.Item1}, Longitude: {latlng.Item2})");
            City = closestCity.Key;
        }

        // Haversine distance calculation
        private double HaversineDistance(double lat1, double lon1, double lat2, double lon2)
        {
            double R = 6371; // Radius of the Earth in km
            double dLat = (lat2 - lat1) * Math.PI / 180;
            double dLon = (lon2 - lon1) * Math.PI / 180;

            lat1 = lat1 * Math.PI / 180;
            lat2 = lat2 * Math.PI / 180;

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(lat1) * Math.Cos(lat2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return R * c;
        }

        private KeyValuePair<string, Point> FindClosestCity(double lat, double lng)
        {
            var locations = new Dictionary<string, Point>
{
    {"Delfzijl", new Point(53.3333, 6.9167)},
    {"Winschoten", new Point(53.1436, 7.0343)},
    {"Stadskanaal", new Point(52.9917, 6.9504)},
    {"Hoogezand", new Point(53.1620, 6.7587)},
    {"Groningen", new Point(53.2194, 6.5665)},
    {"Leek", new Point(53.1617, 6.3761)},
    {"Lauwersoog", new Point(53.3958, 6.2044)},
    {"Schiermonnikoog", new Point(53.4844, 6.1589)},
    {"Nes", new Point(53.4446, 5.7625)},
    {"Holwerd", new Point(53.3636, 5.9119)},
    {"Dokkum", new Point(53.3242, 5.9972)},
    {"Leeuwarden", new Point(53.2014, 5.7997)},
    {"Harlingen", new Point(53.1744, 5.4225)},
    {"Sneek", new Point(53.0333, 5.6585)},
    {"Stavoren", new Point(52.8865, 5.3628)},
    {"Lemmer", new Point(52.8460, 5.7068)},
    {"Heerenveen", new Point(52.9600, 5.9197)},
    {"Drachten", new Point(53.1127, 6.0987)},
    {"West-Terschelling", new Point(53.3575, 5.2223)},
    {"Oost-Vlieland", new Point(53.3000, 5.0667)},
    {"Assen", new Point(52.9920, 6.5625)},
    {"Emmen", new Point(52.7812, 6.8997)},
    {"Hoogeveen", new Point(52.7225, 6.4736)},
    {"Coevorden", new Point(52.6600, 6.7406)},
    {"Meppel", new Point(52.6950, 6.1922)},
    {"Steenwijk", new Point(52.7862, 6.1193)},
    {"Kampen", new Point(52.5550, 5.9143)},
    {"Zwolle", new Point(52.5168, 6.0830)},
    {"Raalte", new Point(52.3844, 6.2750)},
    {"Deventer", new Point(52.2553, 6.1633)},
    {"Almelo", new Point(52.3566, 6.6621)},
    {"Oldenzaal", new Point(52.3138, 6.9293)},
    {"Hengelo", new Point(52.2653, 6.7939)},
    {"Enschede", new Point(52.2215, 6.8937)},
    {"Emmeloord", new Point(52.7108, 5.7486)},
    {"Dronten", new Point(52.5250, 5.7189)},
    {"Lelystad", new Point(52.5186, 5.4714)},
    {"Almere", new Point(52.3508, 5.2647)},
    {"Den Burg", new Point(53.0500, 4.7986)},
    {"Den Helder", new Point(52.9581, 4.7597)},
    {"Hoorn", new Point(52.6415, 5.0606)},
    {"Enkhuizen", new Point(52.7047, 5.2919)},
    {"Alkmaar", new Point(52.6324, 4.7534)},
    {"Zaanstad", new Point(52.4578, 4.7516)},
    {"Purmerend", new Point(52.5050, 4.9597)},
    {"Amsterdam", new Point(52.3676, 4.9041)},
    {"Haarlem", new Point(52.3874, 4.6462)},
    {"Hoofddorp", new Point(52.3025, 4.6889)},
    {"Hilversum", new Point(52.2292, 5.1669)},
    {"Utrecht", new Point(52.0907, 5.1214)},
    {"Amersfoort", new Point(52.1560, 5.3878)},
    {"Veenendaal", new Point(52.0286, 5.5583)},
    {"Nieuwegein", new Point(52.0298, 5.0806)},
    {"Culemborg", new Point(51.9573, 5.2271)},
    {"Tiel", new Point(51.8877, 5.4286)},
    {"Nijmegen", new Point(51.8425, 5.8528)},
    {"Arnhem", new Point(51.9850, 5.8987)},
    {"Doetinchem", new Point(51.9650, 6.2881)},
    {"Zutphen", new Point(52.1400, 6.2014)},
    {"Apeldoorn", new Point(52.2112, 5.9699)},
    {"Harderwijk", new Point(52.3417, 5.6206)},
    {"Winterswijk", new Point(51.9725, 6.7194)},
    {"Leiden", new Point(52.1601, 4.4970)},
    {"Alphen aan den Rijn", new Point(52.1292, 4.6556)},
    {"Zoetermeer", new Point(52.0575, 4.4931)},
    {"Gouda", new Point(52.0167, 4.7083)},
    {"Delft", new Point(52.0116, 4.3571)},
    {"Rotterdam", new Point(51.9225, 4.4792)},
    {"Den Haag", new Point(52.0705, 4.3007)},
    {"Spijkenisse", new Point(51.8450, 4.3292)},
    {"Dordrecht", new Point(51.8133, 4.6900)},
    {"Zierikzee", new Point(51.6525, 3.9158)},
    {"Middelburg", new Point(51.4983, 3.6100)},
    {"Goes", new Point(51.5047, 3.8881)},
    {"Terneuzen", new Point(51.3350, 3.8278)},
    {"Vlissingen", new Point(51.4511, 3.5736)},
    {"Bergen op Zoom", new Point(51.4950, 4.2875)},
    {"Roosendaal", new Point(51.5300, 4.4650)},
    {"Breda", new Point(51.5719, 4.7683)},
    {"Tilburg", new Point(51.5555, 5.0913)},
    {"Eindhoven", new Point(51.4416, 5.4697)},
    {"Helmond", new Point(51.4817, 5.6619)},
    {"Valkenswaard", new Point(51.3500, 5.4600)},
    {"Oss", new Point(51.7656, 5.5183)},
    {"Venray", new Point(51.5250, 5.9750)},
    {"Venlo", new Point(51.3700, 6.1736)},
    {"Weert", new Point(51.2533, 5.7061)},
    {"Roermond", new Point(51.1947, 5.9875)},
    {"Sittard", new Point(51.0000, 5.8833)},
    {"Maastricht", new Point(50.8514, 5.6900)},
    {"Heerlen", new Point(50.8833, 5.9819)},
    {"Kerkrade", new Point(50.8658, 6.0625)},
    {"Geleen", new Point(50.9744, 5.8294)},
};


            string closestCity = null;
            double minDistance = double.MaxValue;

            foreach (var location in locations)
            {
                double distance = HaversineDistance(lat, lng, location.Value.X, location.Value.Y);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestCity = location.Key;
                }
            }

            return new KeyValuePair<string, Point>(closestCity, locations[closestCity]);
        }
    }
}
