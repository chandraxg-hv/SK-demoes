using System;
using System.ComponentModel;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;

namespace ConsoleApp1
{
    public class WeatherPlugin
    {
        [KernelFunction("weather_data")]
        [Description("Get the weather data for a city ")]
        [return : Description("A list of weather for a city")]

        public async Task<string?> GetWeatherData (string location)
        {
            var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

            // Get configuration values
            string? api_Key = config["WEATHER_API_KEY"];
            if (string.IsNullOrEmpty(api_Key))
            {
                throw new InvalidOperationException("API key is missing in the configuration.");
            }

            string url = $"http://api.weatherapi.com/v1/current.json?key={api_Key}&q={location}&aqi=no";

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();
                    return responseBody;
                }

            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request error: {e.Message}");
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
                return null;
            }
        }

        [KernelFunction("weather_forecast")]
        [Description("Get the weather forecast for a city ")]
        [return : Description("A list of weather forecast for a city")]

        public async Task<string?> GetWeatherForecast (string location)
        {
            var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

            // Get configuration values
            string? api_Key = config["WEATHER_API_KEY"];
            if (string.IsNullOrEmpty(api_Key))
            {
                throw new InvalidOperationException("API key is missing in the configuration.");
            }

            string url = $"http://api.weatherapi.com/v1/forecast.json?key={api_Key}&q={location}&days=3&aqi=no&alerts=no";

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();
                    return responseBody;
                }

            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request error: {e.Message}");
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
                return null;
            }
        }
    }
}

