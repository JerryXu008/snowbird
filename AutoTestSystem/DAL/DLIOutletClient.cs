using AutoTestSystem.Model;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static AutoTestSystem.BLL.Bd;
namespace AutoTestSystem.DAL
{
    public class DLIOutletClient
    {
        WebSwitchConInfo connectionInfo;
        CookieContainer cookies { get; set; }
        public string logPath { get; set; }
        public HttpClient client { get; set; }
        public DLIOutletClient(WebSwitchConInfo connectionInf)
        {
            this.connectionInfo = connectionInf;
            cookies = new CookieContainer();

            client = GetClient(cookies);
        }

        public readonly object wLock = new object();     //!互斥锁        
        private HttpClient GetClient(CookieContainer cookies)
        {
            var handler = new HttpClientHandler() { CookieContainer = cookies, UseCookies = true };
            var client = new HttpClient(handler);

            client.DefaultRequestHeaders.Add("User-Agent", "DLIOutletClient");
            client.DefaultRequestHeaders.Add("Accept", "text/html, application/xhtml+xml, image/jxr, */*");
            client.DefaultRequestHeaders.Add("Pragma", "no-cache");
            client.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
            //client.DefaultRequestHeaders.Add("Keep-Alive", "timeout=1296000");
            string strLoginPassword = string.Format("{0}:{1}", "admin", "1234");
            byte[] bytLoginPassword = System.Text.Encoding.UTF8.GetBytes(strLoginPassword);
            string strLoginPasswordEncoded = Convert.ToBase64String(bytLoginPassword);
            client.DefaultRequestHeaders.Add("Authorization", $"Basic {strLoginPasswordEncoded}");
            return client;
        }

        //public async Task<bool> GetSwitchInfo(int outletIndex)
        //{
        //    var url = GetUrl(connectionInfo, string.Format(CultureInfo.InvariantCulture, URLs.CycleN, outletIndex.ToString()));
        //    var body = await GetStringSafeAsync(url);
        //    return body.Contains("true") ? true : false;
        //}

        public async Task<bool> CycleOutlet(int outletIndex)
        {
            var url = GetUrl(connectionInfo, string.Format(CultureInfo.InvariantCulture, URLs.CycleN, outletIndex.ToString()));

            var body = await GetStringSafeAsync(url);

            if (body.Contains("URL=/index.htm"))
            {
                loggerInfo($"PowerCycle Outlet {outletIndex} PASS!");
                return true;
            }
            else
            {
                loggerInfo($"PowerCycle Outlet {outletIndex} FAIL!");
                loggerInfo("CycleOutlet body:" + body);
                return false;
            }
        }

        public async Task<bool> SetOutlet(int outletIndex, bool desiredState)
        {
            var url = GetUrl(connectionInfo, string.Format(CultureInfo.InvariantCulture, URLs.OutletNToState, outletIndex.ToString(), desiredState ? "ON" : "OFF"));

            var body = await GetStringSafeAsync(url);
            if (body.Contains("URL=/index.htm"))
            {
                loggerInfo($"Set Outlet {outletIndex} {(desiredState ? "ON" : "OFF")} PASS!");
                return true;
            }
            else
            {
                loggerInfo($"Set Outlet {outletIndex} {(desiredState ? "ON" : "OFF")} FAIL!");
                loggerInfo("SetOutlet body:" + body);
                return false;
            }
        }

        //public async Task<SwitchInfo> ConnectAndGetSwitchInfoAsync()
        //{
        //    //await ConnectAsync();
        //    loggerInfo("WebSwitchPower connect!");
        //    return await GetSwitchInfo();
        //}


        //public async Task ConnectAsync()
        //{
        //    var client = GetClient(cookies);

        //    Uri loginUrl = null;
        //    try
        //    {
        //        var body = await GetStringSafeAsync(GetUrl(connectionInfo));
        //        var h = new HtmlDocument();
        //        h.LoadHtml(body);

        //        var challengeNode = FindNode(h.DocumentNode, "name", "Challenge");
        //        var challenge = challengeNode.Attributes["value"].Value;

        //        var username = connectionInfo.Username;
        //        var password = challenge + connectionInfo.Username + connectionInfo.Password + challenge;

        //        var md5 = System.Security.Cryptography.MD5.Create();
        //        var hash = md5.ComputeHash(password.ToByteArray());

        //        var sb = new StringBuilder();
        //        for (int x = 0; x < 16; x++)
        //        {
        //            sb.Append(hash[x].ToString("x2"));
        //        }
        //        password = sb.ToString();

        //        var content = new FormUrlEncodedContent(new[]
        //        {
        //           new KeyValuePair<string, string>("Username", username),
        //           new KeyValuePair<string, string>("Password", password),
        //        });

        //        loginUrl = GetUrl(connectionInfo, URLs.Login);
        //        //loggerInfo("WebPsLoginUrl:"+ loginUrl+",content:"+ content);
        //        var result = await client.PostAsync(loginUrl, content);

        //        if (!result.IsSuccessStatusCode)
        //        {
        //            var responseBody = await result.Content.GetStringSafeAsync();
        //            loggerInfo(responseBody);
        //            throw new Exception($"Failed to connect WebPowerSwitch.Response code: {result.StatusCode}");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        loggerInfo(ex.ToString());
        //        //throw;
        //    }
        //}

        public async Task<SwitchInfo> GetSwitchInfo()
        {
            var responseBody = await GetStringSafeAsync(GetUrl(connectionInfo, URLs.Index));

            try
            {
                var switchInfo = new SwitchInfo();

                var controllerName = ParseControllerName(responseBody);

                var newOutlets = ParseRelayName(responseBody);

                switchInfo.Name = controllerName;
                switchInfo.Outlets = new OutletInfo[newOutlets.Count];
                int index = 0;
                foreach (var item in newOutlets)
                {
                    switchInfo.Outlets[index++] = new OutletInfo() { Index = item.Index, Name = item.Name, IsOn = item.IsOn };
                }
                return switchInfo;
            }
            catch (Exception ex)
            {
                loggerInfo(ex.ToString());
            }
            return null;
        }

        private IList<OutletInfo> ParseRelayName(string responseBody)
        {
            try
            {
                var d = new HtmlAgilityPack.HtmlDocument();
                d.LoadHtml(responseBody);

                var outlets = new List<OutletInfo>();
                for (int x = 0; x < 8; x++)
                {
                    var outletNumber = d.DocumentNode.SelectSingleNode(string.Format("/html/body/table/tr/td[2]/table[2]/tr[{0}]/td[1]", x + 3));
                    var outletName = d.DocumentNode.SelectSingleNode(string.Format("/html/body/table/tr/td[2]/table[2]/tr[{0}]/td[2]", x + 3));
                    var outletState = d.DocumentNode.SelectSingleNode(string.Format("/html/body/table/tr/td[2]/table[2]/tr[{0}]/td[3]/b/font", x + 3));

                    if (outletName == null || outletState == null)
                    {
                        outletNumber = d.DocumentNode.SelectSingleNode(string.Format("/html/body/font/table/tr/td[2]/table[2]/tr[{0}]/td[1]", x + 3));
                        outletName = d.DocumentNode.SelectSingleNode(string.Format("/html/body/font/table/tr/td[2]/table[2]/tr[{0}]/td[2]", x + 3));
                        outletState = d.DocumentNode.SelectSingleNode(string.Format("/html/body/font/table/tr/td[2]/table[2]/tr[{0}]/td[3]/b/font", x + 3));
                        if (outletName == null || outletState == null)
                        {
                            loggerInfo("ParseRelayName:刷新失败，XPath节点InnerText为空");
                        }
                    }

                    if ((outletName != null || outletState != null)
                        && (outletNumber.InnerText.Trim() != "Logout" && outletName.InnerText.Trim() != "Help"))
                    {
                        outlets.Add(new OutletInfo() { Index = int.Parse(outletNumber.InnerText.Trim()), Name = outletName.InnerText.Trim(), IsOn = IsSwitchOn(outletState.InnerText) });
                    }
                }
                return outlets;
            }
            catch (Exception ex)
            {
                loggerInfo("ParseRelayName:" + ex.ToString());
                throw;
            }
        }

        private string ParseControllerName(string responseBody)
        {
            var targetString = "Controller:";
            var start = responseBody.IndexOf(targetString);

            if (start > 0)
            {
                var stop = responseBody.IndexOf("<", start);

                var name = responseBody.Substring(start + targetString.Length, stop - (start + targetString.Length));

                return name.Trim();
            }
            else
            {
                targetString = "<title>";
                start = responseBody.IndexOf(targetString);
                var stop = responseBody.IndexOf("</title>", start);

                var name = responseBody.Substring(start + targetString.Length, stop - (start + targetString.Length));

                return name.Trim();
            }
        }


        private bool IsSwitchOn(string innerText)
        {
            return !innerText.Contains("OFF");
        }

        //private Uri GetUrl(WebSwitchConInfo connectionInfo)
        //{
        //    if (connectionInfo.Port != 0)
        //    {
        //        return GetUrl(string.Format("{0}:{1}", connectionInfo.IPAddress, connectionInfo.Port), string.Empty);
        //    }
        //    else
        //    {
        //        return GetUrl(connectionInfo.IPAddress, string.Empty);
        //    }
        //}

        private Uri GetUrl(WebSwitchConInfo connectionInfo, string uri)
        {
            if (connectionInfo.Port != 0)
            {
                return GetUrl(string.Format("{0}:{1}", connectionInfo.IPAddress, connectionInfo.Port), uri);
            }
            else
            {
                return GetUrl(connectionInfo.IPAddress, uri);
            }
        }


        //private Uri GetUrl(string ip)
        //{
        //    return GetUrl(ip, string.Empty);
        //}

        private Uri GetUrl(string ip, string uri)
        {
            string s = string.Format("http://{0}/{1}", ip, uri);
            return new Uri(s);
        }

        private HtmlNode FindNode(HtmlNode h, string attributeName, string value)
        {
            try
            {
                if (h.Attributes[attributeName] != null && h.Attributes[attributeName].Value.Contains(value))
                {
                    return h;
                }

                foreach (var item in h.ChildNodes)
                {
                    var foo = FindNode(item, attributeName, value);

                    if (foo != null)
                        return foo;
                }
            }
            catch (Exception ex)
            {
                loggerInfo(ex.ToString());
                //throw;
            }
            return null;
        }

        public async Task<string> GetStringSafeAsync(Uri uri)
        {
            int retry = 0;
            var str = "";

            while (retry < 10)
            {
                try
                {
                    retry++;
                    var bytes = await client.GetByteArrayAsync(uri);
                    str = System.Text.Encoding.UTF8.GetString(bytes);
                    return str;
                }
                catch (Exception ex)
                {
                    loggerInfo($"HttpClient请求异常次数{retry},Retey. {ex.ToString()}");
                    Thread.Sleep(2000);
                }
            }
            return str;
        }


    }

    public class OutletInfo
    {
        public int Index { get; set; }
        public string Name { get; set; }
        public bool IsOn { get; set; }
    }

    public class SwitchInfo
    {
        public string Name { get; set; }
        public OutletInfo[] Outlets { get; set; }
    }

    public class WebSwitchConInfo
    {
        public string IPAddress { get; set; }
        public int Port { get; set; } = 80;
        public string Username { get; set; }
        public string Password { get; set; }
        public string LogPath { get; set; }
    }

    public static class URLs
    {
        public static string Login = "login.tgi";
        public static string Index = "index.htm";
        public static string OutletNToState = "outlet?{0}={1}";
        public static string CycleN = "outlet?{0}=CCL";
    }
}