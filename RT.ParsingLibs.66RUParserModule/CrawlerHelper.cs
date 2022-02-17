using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Linq;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using RT.Crawler;
using RT.ParsingLibs.Models;
using RT.ParsingLibs.Requests;
using RT.ParsingLibs.Responses;

using HtmlAgilityPack;

namespace RT.ParsingLibs.WWW66RUParser
{
    internal static class CrawlerHelper
    {
        private static WebCrawler _currWebCrawler;

        private static WebCrawler CurrWebCrawler
        {
            get
            {
                if (_currWebCrawler == null)
                {
                    _currWebCrawler = new WebCrawler();
                }

                return _currWebCrawler;
            }
        }

        public static HtmlDocument GetHtmlDocumentByUrl(String urlStr)
        {
            WebRequest request = WebRequest.Create(urlStr);
            WebCrawler currWebCrawler = new WebCrawler();

            var crawlerTask = currWebCrawler.GetResponse(request);
            WebResponse webResponse = crawlerTask.Result;

            Encoding enc = Encoding.GetEncoding("utf-8");
            StreamReader streamReader = new StreamReader(webResponse.GetResponseStream(), enc);
            String content = streamReader.ReadToEnd();
            streamReader.Close();

            webResponse.Close();

            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(content);

            return htmlDoc;
        }
    }
}
