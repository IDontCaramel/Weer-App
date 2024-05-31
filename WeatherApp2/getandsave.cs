using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WeatherApp2
{
    public class WeatherData
    {
        public static LiveWeatherData LiveWeather { get; private set; }
        public static List<DailyForecast> WeeklyForecast { get; private set; }
        public static List<HourlyForecast> HourlyForecasts { get; private set; }

        public class LiveWeatherData
        {
            public string Place { get; set; }
            public long Timestamp { get; set; }
            public string Time { get; set; }
            public double Temp { get; set; }
            public double Gtemp { get; set; }
            public string Summary { get; set; }
            public int Lv { get; set; }
            public string WindDirection { get; set; }
            public double WindDirectionDegrees { get; set; }
            public double WindSpeedMetersPerSecond { get; set; }
            public int WindBft { get; set; }
            public double WindKnots { get; set; }
            public double WindKmh { get; set; }
            public double AirPressure { get; set; }
            public double AirPressureMmHg { get; set; }
            public double DewPoint { get; set; }
            public double Visibility { get; set; }
            public int Gr { get; set; }
            public string WeatherForecast { get; set; }
            public string Sup { get; set; }
            public string Sunrise { get; set; }
            public string Image { get; set; }
            public int Alarm { get; set; }
            public string Lkop { get; set; }
            public string Ltekst { get; set; }
            public string ProbabilityClear { get; set; }
            public string ProbabilityG { get; set; }
            public int ProbabilityGts { get; set; }
            public string ProbabilityGc { get; set; }
        }

        public class DailyForecast
        {
            public string Day { get; set; }
            public string Image { get; set; }
            public int MaxTemperature { get; set; }
            public int MinTemperature { get; set; }
            public int WindBft { get; set; }
            public int WindKmh { get; set; }
            public int WindKnots { get; set; }
            public int WindMetersPerSecond { get; set; }
            public int WindDirectionDegrees { get; set; }
            public string WindDirection { get; set; }
            public int PrecipitationPercentageDay { get; set; }
            public int SunPercentageDay { get; set; }
        }

        public class HourlyForecast
        {
            public string Hour { get; set; }
            public long Timestamp { get; set; }
            public string Image { get; set; }
            public int Temperature { get; set; }
            public int WindBft { get; set; }
            public int WindKmh { get; set; }
            public int WindKnots { get; set; }
            public int WindMetersPerSecond { get; set; }
            public int WindDirectionDegrees { get; set; }
            public string WindDirection { get; set; }
            public double Precipitation { get; set; }
            public int Gr { get; set; }
        }

        public class Api
        {
            public string Source { get; set; }
            public int MaxCount { get; set; }
            public int RemainingCount { get; set; }
        }

        public class Root
        {
            public List<LiveWeatherData> LiveWeather { get; set; }
            public List<DailyForecast> WeeklyForecast { get; set; }
            public List<HourlyForecast> HourlyForecasts { get; set; }
            public List<Api> Api { get; set; }
        }

        public static HourlyForecast GetHourlyForecastByHour(int hour)
        {
            return HourlyForecasts?.FirstOrDefault(forecast =>
            {
                DateTime forecastTime = DateTime.Parse(forecast.Hour);
                return forecastTime.Hour == hour;
            });
        }

        public static async Task<bool> FetchWeatherData(string apiKey, string place)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string url = $"https://weerlive.nl/api/weerlive_api_v2.php?key={apiKey}&locatie={place}";
                    HttpResponseMessage response = await client.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();
                        Root weatherRoot = JsonConvert.DeserializeObject<Root>(json);
                        if (weatherRoot != null && weatherRoot.LiveWeather.Count > 0)
                        {
                            LiveWeather = weatherRoot.LiveWeather[0];
                            WeeklyForecast = weatherRoot.WeeklyForecast;
                            HourlyForecasts = weatherRoot.HourlyForecasts;
                            return true; // Data fetched successfully
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Handle or log the exception as needed
                    Console.WriteLine("Error fetching weather data: " + ex.Message);
                }
                return false; // Request failed or data was invalid
            }
        }
    }
}
