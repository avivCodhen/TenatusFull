using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Tenatus.API.Components.AlgoTrading.Models;
using Tenatus.API.Components.SignalR;
using Tenatus.API.Components.SignalR.Services;
using Tenatus.API.Util;

namespace Tenatus.API.Components.AlgoTrading.Services.Scrapping
{
    public class YahooStockDataReader : IStockDataReader
    {
        private readonly string _stock;
        private readonly SignalRService _signalRService;
        private ChromeDriver _driver;

        public YahooStockDataReader(string stock, SignalRService signalRService)
        {
            _stock = stock;
            _signalRService = signalRService;
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments("--headless");

            _driver = new ChromeDriver(AppConstants.FilePath, chromeOptions);
            
            _driver.Navigate().GoToUrl($"https://finance.yahoo.com/quote/{_stock}");
        }

        public Task<List<StockData>> WriteStocksValue()
        {
            var stocksData = new List<StockData>();
            var now = DateTime.Now;
            while (DateTime.Now < now.AddHours(2))
            {
                var stockVal = _driver.FindElementById("quote-header-info")
                    .FindElement(By.CssSelector("span[data-reactid='32']")).Text;

                stocksData.Add(new StockData() {Time = DateTime.Now, CurrentPrice = Convert.ToDecimal(stockVal)});
                Console.WriteLine(stockVal);
                Thread.Sleep(1000);
            }

            var jsonStr = JsonConvert.SerializeObject(stocksData.ToArray());
            System.IO.File.WriteAllText($@"c:\Stocks\Tenatus\{_stock}-value.json", jsonStr);
            return Task.FromResult(stocksData);
        }

        public async Task<StockData> ReadStockValue()
        {
            try
            {
                var stockVal = _driver.FindElementById("quote-header-info")
                    .FindElement(By.CssSelector("span[data-reactid='32']")).Text;
                var data = new StockData()
                    {Time = DateTime.Now, CurrentPrice = Convert.ToDecimal(stockVal), Stock = _stock};
                await _signalRService.SendMessageToAll("transferStockData", data);
                return data;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public void Dispose()
        {
            _driver.Dispose();
        }
    }
}