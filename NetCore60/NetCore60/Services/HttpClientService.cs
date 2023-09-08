﻿using System;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading.Tasks;

public class HttpClientService
{
    private readonly HttpClient _httpClient;

    public HttpClientService()
    {
        _httpClient = new HttpClient();
    }

    #pragma warning disable CS8603 // 可能有 Null 參考傳回
    public async Task<string> GetNordVPNDataAsync(string ipAddress)
    {
        try
        {
            // 设置请求的URL
            string apiUrl = $"https://nordvpn.com/wp-admin/admin-ajax.php?action=get_user_info_data&ip={ipAddress}";

            // 发送HTTP GET请求
            HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);

            // 检查响应状态码
            if (response.IsSuccessStatusCode)
            {
                // 从响应中读取内容
                string responseBody = await response.Content.ReadAsStringAsync();

                // 返回响应数据
                return responseBody;
            }
            else
            {
                Console.WriteLine("HTTP请求失败，状态码：" + response.StatusCode);
                //return null;
                return "HTTP请求失败，状态码：";
            }
        }
        catch (Exception ex)
        {
            //Console.WriteLine("发生异常：" + ex.Message);
            return "发生异常：" + ex.Message;
        }
    }

    public async Task<string> GetLocalPublicIpAddressAsync()
    {
        try
        {
            // 使用 "ipify" 的API来获取公共IP地址
            string apiUrl = "https://api64.ipify.org?format=text";

            HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                string publicIpAddress = await response.Content.ReadAsStringAsync();
                return publicIpAddress;
            }
            else
            {
                return "Unable to retrieve public IP address";
            }
        }
        catch (Exception ex)
        {
            return "Error: " + ex.Message;
        }
    }
}
