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

namespace WeatherApp2
{
    /// <summary>
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class MainMenu : Window
    {
        public MainMenu()
        {
            InitializeComponent();
        }

        public void UpdateData()
        {
            var liveWeather = WeatherData.LiveWeather;
            var weeklyForecast = WeatherData.WeeklyForecast;
            var hourlyForecasts = WeatherData.HourlyForecasts;

            temp.Content = liveWeather.Temp.ToString();
            //tempMinMax.Content = $"{weeklyForecast(0).}}"

        }

        private async void LoadWeatherData()
        {
            string place = inputPlace.Text.Trim().ToLower();

            bool success = await WeatherData.FetchWeatherData("demo", place);

            if (success)
            {
                // Data was successfully fetched
                var forecastForHour9 = WeatherData.GetHourlyForecastByHour(9);

                // Use the forecast data as needed
                if (forecastForHour9 != null)
                {
                    Console.WriteLine($"Temperature at 9:00: {forecastForHour9.Temperature}°C");
                    Console.WriteLine($"Weather at 9:00: {forecastForHour9.Image}");
                }
                else
                {
                    Console.WriteLine("No forecast available for 9:00.");
                }
            }
            else
            {
                // Handle the failure
                Console.WriteLine("Failed to fetch weather data.");
            }
        }

        private void btnMap_Click(object sender, RoutedEventArgs e)
        {
            map mapWindow = new map();
            mapWindow.Show();
        }

    }
}
