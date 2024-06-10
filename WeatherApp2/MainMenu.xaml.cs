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

            inputDelay.MaxLength = 3; 

            //create timer
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


            temp.Content = $"{liveWeather.temp}°";
            tempMinMax.Content = $"{currentDay.min_temp}/{currentDay.max_temp}";


        }

        // updates the daily temps
        private void updateDaily()
        {
            for (int i = 0; i < 4; i++)
            {
                // find the labels by name using the FindName method
                Label dailyTemp = (Label)this.FindName("dailyTemp" + i);
                Label dailyRain = (Label)this.FindName("dailyRain" + i);
                Label dailyDay = (Label)this.FindName("dailyDay" + i);

                var currentDay = WeatherData.GetDailyForecastByIndex(i + 1);

                DateTime currentDate = DateTime.Now.AddDays(i + 1);

                // modify the labels
                if (dailyTemp != null)
                    dailyTemp.Content = $"{currentDay.min_temp}/{currentDay.max_temp}";

                if (dailyRain != null)
                    dailyRain.Content = $"%{currentDay.neersl_perc_dag}";

                if (dailyDay != null)
                    dailyDay.Content = currentDate.ToString("ddd"); ;
            }
        }

        // updates the hourly temps
        private void updateHourly()
        {
            DateTime currentTime = DateTime.Now;
            int roundedHour = currentTime.Minute >= 59 ? currentTime.Hour + 1 : currentTime.Hour;




            for (int i = 0; i < 8; i++)
            {
                // find the labels by name using the FindName method
                Label tempLabel = (Label)this.FindName("tempList" + i);
                Label timeLabel = (Label)this.FindName("timeList" + i);

                // Modify the labels
                if (tempLabel != null)
                    tempLabel.Content = $"{WeatherData.GetHourlyForecastByHour(roundedHour + i).temp}°";

                if (timeLabel != null)
                    timeLabel.Content = $"{roundedHour + i}:00";
            }



        }

        // makes a request to the api and starts the data update
        private async void LoadWeatherData()
        {

            string place = inputPlace.Text.Trim().ToLower();

            // makes api request
            bool success = await WeatherData.FetchWeatherData("3fb67a9d77", place);

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

        // opens map
        private void btnMap_Click(object sender, RoutedEventArgs e)
        {
            map mapWindow = new map();
            mapWindow.Closing += mapWindow_OnWindowClosing; // Subscribe to the Closing event
            mapWindow.Show();
        }



        // updates data when a place is chosen
        private void mapWindow_OnWindowClosing(object sender, CancelEventArgs e)
        {
            inputPlace.Text = map.City;
            LoadWeatherData();
        }

        
        private void delayUp_Click(object sender, RoutedEventArgs e)
        {
            if (delay <= 999) { delay++; }
            inputDelay.Text = delay.ToString();
            TimeSpan.FromMinutes(delay);
            checkDelay();
        }
        private void delayDown_Click(object sender, RoutedEventArgs e)
        {
            if (delay >= 15 ) { delay--; }
            inputDelay.Text = delay.ToString();
            TimeSpan.FromMinutes(delay);
            checkDelay();
        }

        // forces numbers
        private void inputDelay_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key < Key.D0 || e.Key > Key.D9)
            {
                e.Handled = true;
            }
        }

        

        // updates timer each tick
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            LoadWeatherData();
        }

        
        // checks if the delay is a valid input
        private void checkDelay()
        {
            try
            {
                delay = int.Parse(inputDelay.Text);
            }
            catch
            {

            }

            if (inputDelay.Text.Length > 3)
            {
                inputDelay.Text = "999";
            }

            if (delay < 15)
            {
                inputDelay.Text = "15";
            }
            else if (delay > 999)
            {
                inputDelay.Text = "999";
            }

            TimeSpan.FromMinutes(delay);
        }


        private void inputDelay_LostFocus(object sender, RoutedEventArgs e)
        {
            checkDelay();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            LoadWeatherData();
        }

        
    }
}
