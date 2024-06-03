using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

public class WeatherData
{
    public static LiveWeer LiveWeather { get; private set; }
    public static List<DailyForecast> WeeklyForecast { get; private set; }
    public static List<HourlyForecast> HourlyForecasts { get; private set; }

    public class LiveWeer
    {
        public string plaats { get; set; }
        public long timestamp { get; set; }
        public string time { get; set; }
        public double temp { get; set; }
        public double gtemp { get; set; }
        public string samenv { get; set; }
        public int lv { get; set; }
        public string windr { get; set; }
        public double windrgr { get; set; }
        public double windms { get; set; }
        public int windbft { get; set; }
        public double windknp { get; set; }
        public double windkmh { get; set; }
        public double luchtd { get; set; }
        public double ldmmhg { get; set; }
        public double dauwp { get; set; }
        public double zicht { get; set; }
        public int gr { get; set; }
        public string verw { get; set; }
        public string sup { get; set; }
        public string sunder { get; set; }
        public string image { get; set; }
        public int alarm { get; set; }
        public string lkop { get; set; }
        public string ltekst { get; set; }
        public string wrschklr { get; set; }
        public string wrsch_g { get; set; }
        public int wrsch_gts { get; set; }
        public string wrsch_gc { get; set; }
    }

    public class DailyForecast
    {
        public string dag { get; set; }
        public string image { get; set; }
        public int max_temp { get; set; }
        public int min_temp { get; set; }
        public int windbft { get; set; }
        public int windkmh { get; set; }
        public int windknp { get; set; }
        public int windms { get; set; }
        public int windrgr { get; set; }
        public string windr { get; set; }
        public int neersl_perc_dag { get; set; }
        public int zond_perc_dag { get; set; }
    }

    public class HourlyForecast
    {
        public string uur { get; set; }
        public long timestamp { get; set; }
        public string image { get; set; }
        public int temp { get; set; }
        public int windbft { get; set; }
        public int windkmh { get; set; }
        public int windknp { get; set; }
        public int windms { get; set; }
        public int windrgr { get; set; }
        public string windr { get; set; }
        public double neersl { get; set; }
        public int gr { get; set; }
    }

    public class Api
    {
        public string bron { get; set; }
        public int max_verz { get; set; }
        public int rest_verz { get; set; }
    }

    public class Root
    {
        public List<LiveWeer> liveweer { get; set; }
        public List<DailyForecast> wk_verw { get; set; }
        public List<HourlyForecast> uur_verw { get; set; }
        public List<Api> api { get; set; }
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
                    if (weatherRoot != null && weatherRoot.liveweer.Count > 0)
                    {
                        LiveWeather = weatherRoot.liveweer[0];
                        WeeklyForecast = weatherRoot.wk_verw;
                        HourlyForecasts = weatherRoot.uur_verw;
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

    public static DailyForecast GetDailyForecastByIndex(int index)
    {
        if (WeeklyForecast != null && index >= 0 && index < WeeklyForecast.Count)
        {
            return WeeklyForecast[index];
        }
        return null;
    }

    public static HourlyForecast GetHourlyForecastByHour(int hour)
    {
        return HourlyForecasts?.FirstOrDefault(forecast =>
        {
            DateTime forecastTime = DateTimeOffset.FromUnixTimeSeconds(forecast.timestamp).DateTime;
            return forecastTime.Hour == hour;
        });
    }
}
