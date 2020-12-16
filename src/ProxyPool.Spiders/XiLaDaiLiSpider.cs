using HtmlAgilityPack;
using ProxyPool.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ProxyPool.Spiders
{
    public sealed class XiLaDaiLiSpider
    {
        private static HttpClient _httpClient = new HttpClient();

        public async IAsyncEnumerable<ProxyInfo> GetProxyInfos()
        {
            await foreach (var item in Crawling("http://www.xiladaili.com/gaoni/"))
            {
                yield return item;
            }

            await foreach(var item in Crawling("http://www.xiladaili.com/http/"))
            {
                yield return item;
            }

            await foreach(var item in Crawling("http://www.xiladaili.com/https/"))
            {
                yield return item;
            }
        }

        private async IAsyncEnumerable<ProxyInfo> Crawling(string url)
        {
            var index = 1;
            while (true)
            {
                var html = await SendRequestAsync($"{url}{index}/");
                if (string.IsNullOrWhiteSpace(html))
                    yield break;

                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(html);
                var records = ParseProxyInfos(doc);
                if (records == null)
                    yield break;

                foreach (var item in records)
                {
                    yield return item;
                }

                index++;
            }
        }

        private IEnumerable<ProxyInfo> ParseProxyInfos(HtmlDocument doc)
        {
            var trNodes = doc.DocumentNode.SelectNodes("//table[@class='fl-table']/tbody/tr");
            return trNodes?
                .Where(c =>
                {
                    var lastUpdateTime = c.SelectSingleNode("./td[last()-1]")?.InnerText;
                    if (string.IsNullOrWhiteSpace(lastUpdateTime))
                        return false;

                    return Convert.ToDateTime(lastUpdateTime).Date >= DateTime.Now.Date;
                })
                .Select(c =>
                {
                    var address = c.SelectSingleNode("./td[1]")?.InnerText ?? ":0";
                    return new ProxyInfo
                    {
                        IP = address.Split(":")[0],
                        Port = int.Parse(address.Split(":")[1]),
                        AnonymousDegree = c.SelectSingleNode("./td[3]")?.InnerText switch
                        {
                            "高匿代理服务" => AnonymousDegree.High,
                            "透明代理服务" => AnonymousDegree.None,
                            _ => AnonymousDegree.UnKnown
                        }
                    };
                });
        }

        private async Task<string> SendRequestAsync(string url)
        {
            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Get, url);
            var response = await _httpClient.SendAsync(message);
            return await response?.Content?.ReadAsStringAsync();
        }
    }
}
