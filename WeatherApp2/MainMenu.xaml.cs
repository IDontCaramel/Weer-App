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
using System.Windows.Threading;




namespace WeatherApp2
{

    public partial class MainMenu : Window
    {



        public MainMenu()
        {
            InitializeComponent();

            delay = int.Parse(inputDelay.Text);
            LoadWeatherData();

            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = TimeSpan.FromMinutes(delay);
            dispatcherTimer.Start();
        }


        public int delay;

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

                var currentDay = WeatherData.GetDailyForecastByIndex(i + 1);

                DateTime currentDate = DateTime.Now.AddDays(i + 1);

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
                    tempLabel.Content = $"{WeatherData.GetHourlyForecastByHour(roundedHour + i).temp}°";

                if (timeLabel != null)
                    timeLabel.Content = $"{roundedHour + i}:00";
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

        private void delayUp_Click(object sender, RoutedEventArgs e)
        {
            delay++;
            inputDelay.Text = delay.ToString();
        }
        private void delayDown_Click(object sender, RoutedEventArgs e)
        {
            delay--;
            inputDelay.Text = delay.ToString();
        }

        private void inputDelay_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key < Key.D0 || e.Key > Key.D9)
            {
                e.Handled = true;
            }
        }

        private void inputDelay_LostFocus(object sender, RoutedEventArgs e)
        {
            delay = int.Parse(inputDelay.Text);


            if (delay < 15)
            {
                inputDelay.Text = "15";
            }
            else if (delay > 999)
            {
                inputDelay.Text = "999";
            }
        }





        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            LoadWeatherData();
        }





    }
}
