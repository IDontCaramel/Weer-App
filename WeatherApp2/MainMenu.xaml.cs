using Newtonsoft.Json.Linq;
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
using static WeatherData;
using System.Reflection;
using System.ComponentModel;



namespace WeatherApp2
{
    
    public partial class MainMenu : Window
    {

        public MainMenu()
        {
            InitializeComponent();

            LoadWeatherData();
        }

        public void updateMenu()
        {
            var liveWeather = WeatherData.LiveWeather;
            DailyForecast currentDay = WeatherData.GetDailyForecastByIndex(0);


            temp.Content = $"{liveWeather.temp}";
            tempMinMax.Content = $"{currentDay.min_temp}/{currentDay.max_temp}";


        }


        private void updateDaily()
        {
            for (int i = 0; i < 4; i++)
            {
                // Find the labels by name using the FindName method
                Label dailyTemp = (Label)this.FindName("dailyTemp" + i);
                Label dailyRain = (Label)this.FindName("dailyRain" + i);
                Label dailyDay = (Label)this.FindName("dailyDay" + i);

                var currentDay = WeatherData.GetDailyForecastByIndex(i+1);

                DateTime currentDate = DateTime.Now.AddDays(i+1);

                // Modify the labels
                if (dailyTemp != null)
                    dailyTemp.Content = $"{currentDay.min_temp}/{currentDay.max_temp}";

                if (dailyRain != null)
                    dailyRain.Content = $"%{currentDay.neersl_perc_dag}";

                if (dailyDay != null)
                    dailyDay.Content = currentDate.ToString("ddd"); ;
            }
        }

        private void updateHourly()
        {
            DateTime currentTime = DateTime.Now;
            int roundedHour = currentTime.Minute >= 59 ? currentTime.Hour + 1 : currentTime.Hour;




            for (int i = 0; i < 8; i++)
            {
                // Find the labels by name using the FindName method
                Label tempLabel = (Label)this.FindName("tempList" + i);
                Label timeLabel = (Label)this.FindName("timeList" + i);

                // Modify the labels
                if (tempLabel != null)
                    tempLabel.Content = $"{WeatherData.GetHourlyForecastByHour(roundedHour+i).temp}°";

                if (timeLabel != null)
                    timeLabel.Content =  $"{roundedHour+ i}:00";
            }



        }


        private async void LoadWeatherData()
        {

            string place = inputPlace.Text.Trim().ToLower();

            bool success = await WeatherData.FetchWeatherData("demo", place);

            if (success)
            {
                updateMenu();
                updateHourly();
                updateDaily();
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
            mapWindow.Closing += mapWindow_OnWindowClosing; // Subscribe to the Closing event
            mapWindow.Show();
        }

        private void mapWindow_OnWindowClosing(object sender, CancelEventArgs e)
        {
            inputPlace.Text = map.City;
            LoadWeatherData();
        }

    }
}
