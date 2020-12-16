using HtmlAgilityPack;
using ProxyPool.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ProxyPool.Spiders
{
    public sealed class KuaiDaiLiSpider
    {
        private const string HIGH_PROXY_INDEX_URL = "https://www.kuaidaili.com/free/inha/";
        private const string NORMAL_PROXY_INDEX_URL = "https://www.kuaidaili.com/free/intr/";
        private static HttpClient _httpClient = new HttpClient();

        public async IAsyncEnumerable<ProxyInfo> GetProxyInfos()
        {
            await foreach (var item in Crawling(NORMAL_PROXY_INDEX_URL))
            {
                yield return item;
            }

            await foreach (var item in Crawling(HIGH_PROXY_INDEX_URL))
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
            var trNodes = doc?.DocumentNode.SelectNodes("//div[@id='list']/table/tbody/tr");
            return trNodes?
                .Where(c =>
                {
                    var lastUpdateTime = c.SelectSingleNode("./td[@data-title='最后验证时间']")?.InnerText;
                    if (string.IsNullOrWhiteSpace(lastUpdateTime))
                        return false;

                    return Convert.ToDateTime(lastUpdateTime).Date >= DateTime.Now.Date;
                })
                .Select(c =>
                {
                    int.TryParse(c.SelectSingleNode("./td[@data-title='PORT']")?.InnerText, out int port);
                    return new ProxyInfo
                    {
                        IP = c.SelectSingleNode("./td[@data-title='IP']")?.InnerText,
                        Port = port,
                        AnonymousDegree = c.SelectSingleNode("./td[@data-title='匿名度']")?.InnerText switch
                                            {
                                                "高匿名" => AnonymousDegree.High,
                                                "透明" => AnonymousDegree.None,
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
