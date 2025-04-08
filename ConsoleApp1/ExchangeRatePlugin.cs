using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;

namespace ConsoleApp1
{
    public class ExchangeRatePlugin
    {
        [KernelFunction("exchangerate_data")]
        [Description("Get the exchange rate data for currency ")]
        [return : Description("A list of exchange rate data for currency")]

        public async Task<string?> GetExchangeRateAsync (string basecurrency, List<string> listcurrency)
        {
            // Convert the list of currencies to a comma-separated string
            string currency = string.Join(",", listcurrency);

            // Create a configuration builder to read the appsettings.json file
            var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

            // Get configuration values
            string? api_Key = config["EXCHANERATEIO_API_KEY"];
            if (string.IsNullOrEmpty(api_Key))
            {
                throw new InvalidOperationException("API key is missing in the configuration.");
            }

            //check if the base currency is in the list of symbols
            //call exchangerates symbols api
            string symbolsUrl = $"https://api.exchangeratesapi.io/v1/symbols?access_key={api_Key}&base={basecurrency}";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(symbolsUrl);
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();
                    //check if the base currency is in the list of symbols
                    if (!responseBody.Contains(basecurrency))
                    {
                        throw new InvalidOperationException($"Base currency {basecurrency} is not valid.");
                    }
                    else
                    {
                        return responseBody;
                    }
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

 /*            //convert the listcurrency from a list to a comma separate string of each list item
            string listcurrencycurrency = string.Join(",", listcurrency);

            //Get the exchanage rate between base and listcurrency


            string url = $"https://api.exchangeratesapi.io/v1/latest?access_key={api_Key}&base=USD&symbols={string.Join(",", listcurrency)}";

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
            } */
        }
    }
}


