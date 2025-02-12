﻿using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System;
using System.Drawing.Printing;
using System.Xml.Linq;
using System.Text.Json.Nodes;
using System.Diagnostics.Eventing.Reader;
using HtmlAgilityPack;
using System.ComponentModel;
using System.Globalization;

public class GetStocksService
{
    private readonly HttpClient _httpClient;

    public GetStocksService()
    {
        _httpClient = new HttpClient();
    }

    #pragma warning disable CS8603 // 可能有 Null 參考傳回

    public async Task<string> getExDividendNoticeForm(int limitDays, bool isCashDividend = false)
    {
        try
        {
            var apiUrl = "https://kgiweb.moneydj.com/b2brwdCommon/jsondata/63/56/6c/twstockdata.xdjjson?x=afterhours-bulletin0001&revision=2018_07_31_1\r\n";
            var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);

            // Add standard headers
            //request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
            //request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            //request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
            //request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("br"));
            //request.Headers.AcceptLanguage.Add(new StringWithQualityHeaderValue("en-US", 0.9));
            //request.Headers.AcceptLanguage.Add(new StringWithQualityHeaderValue("zh-TW", 0.8));
            //request.Headers.AcceptLanguage.Add(new StringWithQualityHeaderValue("zh", 0.7));
            ////request.Headers.CacheControl = new CacheControlHeaderValue { NoCache = true };
            //request.Headers.Add("Cookie", "BID=CA4AAE15-0D0B-4C0D-BCB1-25DC11E09166; twk_idm_key=0BqUILyNhLa4ONJ4SIVah; client_fingerprint=ee08beec5cce0a966f9667469f9c7f9b5c64f67910e22198439121665f6b7422; cf_clearance=MrS1qSsUnDBygYFC.1VQdLs622.NccDUNydgm2gEKcc-1701328000-0-1-cf5244fa.961f637.98531b1a-0.2.1701328000; TawkConnectionTime=0; twk_uuid_630dbec937898912e9661d8a=%7B%22uuid%22%3A%221.70gsc8OZvmWaovjYZw82SRAW6RMMvXJp6DaiLMMYetvd1iARFoIKfxsIav5s3XgEcXFuS6rcbgEqMlzaVU4Ur84xWMLbruMz2SUJ6Ooz6Yz2yZZdzKVC%22%2C%22version%22%3A3%2C%22domain%22%3A%22wantgoo.com%22%2C%22ts%22%3A1701331318119%7D");
            ////request.Headers.Add("Pragma", "no-cache");
            //request.Headers.Referrer = new Uri("https://www.wantgoo.com/stock/calendar/dividend-right");
            //request.Headers.Add("Sec-Ch-Ua", "\"Google Chrome\";v=\"119\", \"Chromium\";v=\"119\", \"Not?A_Brand\";v=\"24\"");
            //request.Headers.Add("Sec-Ch-Ua-Mobile", "?0");
            //request.Headers.Add("Sec-Ch-Ua-Platform", "\"Windows\"");
            //request.Headers.Add("Sec-Fetch-Dest", "empty");
            //request.Headers.Add("Sec-Fetch-Mode", "cors");
            //request.Headers.Add("Sec-Fetch-Site", "same-origin");
            //request.Headers.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/119.0.0.0 Safari/537.36");
            //request.Headers.Add("X-Client-Signature", "975c97355589017eff3a53331570215d0d1a682011ac8119a351e5e607504cb0");
            //request.Headers.Add("X-Requested-With", "XMLHttpRequest");
            // Send the request
            HttpResponseMessage response = await _httpClient.SendAsync(request);
            //HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);
            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                var jsonObject = JObject.Parse(responseBody);
                var originalResult = jsonObject["ResultSet"]?["Result"] as JArray ?? new JArray();
                DateTime currentDate = DateTime.Now;
                var filteredData = new List<JToken>();
                JArray tempData = new JArray();
                foreach (JObject item in originalResult)
                {

                    var jsonData = new JObject();
                    if (DateTime.TryParse(item["V9"]?.ToString(), out DateTime dateValue))
                    {
                        double daysDifference = (dateValue - currentDate).TotalDays;
                        if (daysDifference > 0 && daysDifference <= limitDays)
                        {
                            jsonData.Add("除權息日期", item["V9"]);
                            jsonData.Add("股票名稱", item["V3"]);
                            jsonData.Add("除息(現金股利)", item["V4"]);
                            if (!isCashDividend)
                            {
                                jsonData.Add("除權(股票股利)", item["V7"]);
                            }
                            //tempData.Add(new JObject(
                            //    new JProperty("除權息日期", item["V9"]),
                            //));
                            tempData.Add(jsonData);
                            //filteredData.Add(item);
                        }

                    }
                }
                if (isCashDividend)
                {
                    tempData = new JArray(tempData.Where(item => !string.IsNullOrWhiteSpace(item["除息(現金股利)"]?.ToString())));
                }
                else
                {
                }
                string filteredJson = JsonConvert.SerializeObject(tempData, Formatting.Indented);

                return filteredJson;
            }
            else
            {
                //Console.WriteLine("HTTP请求失败，状态码：" + response.StatusCode);
                return "HTTP请求失败，状态码：" + response.StatusCode;
            }
        }
        catch (Exception ex)
        {
            //Console.WriteLine("发生异常：" + ex.Message);
            return "发生异常：" + ex.Message;
        }
    }

    public async Task<string> getFiveLevelsOfStockInformation(string stockCode)
    {
        try
        {
            //String interpolation using $
            var apiUrl = $"https://mis.twse.com.tw/stock/api/getStockInfo.jsp?ex_ch=tse_" +
                $"{stockCode}.tw&json=1&delay=0&_=1701445552510";


            var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
            HttpResponseMessage response = await _httpClient.SendAsync(request);
            //// 发送HTTP GET请求
            //HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);
            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                responseBody = responseBody.Replace("\n", "").Replace("\r", "");

                return responseBody;
            }
            else
            {
                //Console.WriteLine("HTTP请求失败，状态码：" + response.StatusCode);
                return "HTTP请求失败，状态码：" + response.StatusCode;
            }
        }
        catch (Exception ex)
        {
            //Console.WriteLine("发生异常：" + ex.Message);
            return "发生异常：" + ex.Message;
        }
    }

    public async Task<string> getThreeMajorInstitutionalInvestors()
    {
        //JToken value;

        try
        {
            var _LatestOpeningDate = await getTheLatestOpeningDate();

            var apiUrl = $"https://wwwc.twse.com.tw/rwd/zh/fund/T86?date={_LatestOpeningDate}&selectType=ALL&response=json&_=1704631325883";


            var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
            HttpResponseMessage response = await _httpClient.SendAsync(request);
            //// 发送HTTP GET请求
            //HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);
            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                //responseBody = responseBody.Replace("\n", "").Replace("\r", "");
                var jsonObject = JObject.Parse(responseBody);
                var originalResult = jsonObject["data"] as JArray ?? new JArray();
                if (originalResult.Count > 0)
                {
                    var zz = originalResult[0][5];
                    var sortedResult = originalResult
               .Select(item => item as JArray)
                    .OrderByDescending(item =>
                    {
                        var container = item as JArray;
                        if (container != null)
                        {
                            var valueString = container[5].ToString(); // 将 JValue 转换为字符串
                            int value;

                            // 尝试将带有千位分隔符的字符串转换为整数
                            if (int.TryParse(valueString, NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out value))
                            {
                                return value;
                            }
                        }
                        return 0; // 如果无法获取值或转换失败，则返回默认值
                    })
               .ToArray();
                    var top100Items = sortedResult.Take(10).ToList();
                    string jsonResult = JsonConvert.SerializeObject(top100Items);
                    return jsonResult;
                }
                else
                {
                    return "HTTP请求失败，状态码：" + response.StatusCode;
                }

            }
            else
            {
                //Console.WriteLine("HTTP请求失败，状态码：" + response.StatusCode);
                return "HTTP请求失败，状态码：" + response.StatusCode;
            }
        }
        catch (Exception ex)
        {
            //Console.WriteLine("发生异常：" + ex.Message);
            return "发生异常：" + ex.Message;
        }
    }

    public async Task<string> getTheLatestOpeningDate()
    {
        try
        {
            string _responseClosingDates = await getStockMarketOpeningAndClosingDates(false);
            JArray jsonArray = JArray.Parse(_responseClosingDates);
            // 转换为DateTime对象的列表
            List<DateTime> dates = jsonArray.Select(dateString => DateTime.Parse(dateString.ToString())).ToList();

            //if (dates.Contains(now))
            //{
            //    Console.WriteLine("DateTime.Now 在列表中.");
            //}
            //else
            //{
            //    Console.WriteLine("DateTime.Now 不在列表中.");
            //}
            var currentDate = DateTime.Now;
            if (currentDate.Hour < 20)
            {
                currentDate = currentDate.AddDays(-1);
            }
            // 循环递减日期，直到找到不是周六的日期
            while (currentDate.DayOfWeek == DayOfWeek.Saturday || currentDate.DayOfWeek == DayOfWeek.Sunday || dates.Contains(currentDate.Date))
            {

                currentDate = currentDate.AddDays(-1);

            }
            var _yyyyMMdd = currentDate.ToString("yyyyMMdd");
            return _yyyyMMdd; 
        }
        catch (Exception ex)
        {
            return "发生异常：" + ex.Message;
        }
    }

    public async Task<string> getStockMarketOpeningAndClosingDates(bool requestAllData = false)
    {
        try
        {
            var apiUrl = $"https://www.twse.com.tw/rwd/zh/holidaySchedule/holidaySchedule?response=json&_=1704633406324";

            var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
            HttpResponseMessage response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                responseBody = responseBody.Replace("\n", "").Replace("\r", "");

                var jsonObject = JObject.Parse(responseBody);
                var originalResult = jsonObject["data"] as JArray ?? new JArray();
                if (originalResult.Count > 0 && !requestAllData)
                {
                    var dates = originalResult.Select(item => item[0].ToString()).ToArray();
                    string jsonResult = JsonConvert.SerializeObject(dates);

                    return jsonResult;
                }
                else
                {
                    // 'originalResult' 为空，处理空数组的情况
                    string jsonResult = JsonConvert.SerializeObject(jsonObject);

                    return jsonResult;
                }

            }
            else
            {
                return null; 
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return null;
        }
    }

    public async Task<string> getQuoteTimeSalesStore()
    {
        var httpClient = new HttpClient();
        var url = "https://tw.stock.yahoo.com/quote/3231.TW/time-sales"; // 替换成你的 URL

        try
        {
            var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var htmlContent = await response.Content.ReadAsStringAsync();

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(htmlContent);

            // 以下是如何提取特定元素的例子
            // 例如，提取所有的 h1 标签
            var h1Tags = htmlDoc.DocumentNode.SelectNodes("//h1");
            if (h1Tags != null)
            {
                foreach (var tag in h1Tags)
                {
                    Console.WriteLine(tag.InnerText);
                }
            }

            // 你可以根据需要调整 XPath 查询
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine($"请求错误: {e.Message}");
        }
        return "HTTP请求失败，状态码：";
    }


}