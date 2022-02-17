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
    internal class NewRealtySearchResultsTableParser
    {
        public ParseResponse Parse(String currUrl, Bind currBind, String requestLastPublicationId)
        {
            Boolean isReqIdExists = false;
            String firstPublicationId = null;
            String currPublicationId = null;

            // загрузим HtmlDocument
            HtmlDocument htmlDoc = CrawlerHelper.GetHtmlDocumentByUrl(currUrl);

            // найдем по полному урлу с параметрами его разные части
            int idxOfRu = currUrl.IndexOf("?");
            String commonPartOfUrl = currUrl.Remove(idxOfRu);

            idxOfRu = currUrl.IndexOf(".ru") + 3;
            String middlePartOfUrl = commonPartOfUrl.Remove(0, idxOfRu);

            idxOfRu = currUrl.IndexOf(".ru") + 3;
            String mainPartOfUrl = currUrl.Remove(idxOfRu);

            // общая инициализация ответа парсера
            ParseResponse result = new ParseResponse();
            result.Publications = new List<WebPublication>();

            // вытащим строки таблицы 
            HtmlNodeCollection liItems = htmlDoc.DocumentNode.SelectNodes(@"//ul[@class='b-doska-goods-list2']//li[@class='b-doska-goods-list2__item' or @class='b-doska-goods-list2__item hotels_current']");
            if (liItems != null)
            {
                // проходим, по CountAD строками или по всему списку
                int CountAD = 15; // временно, потом заменить на настоящий из библиотеки, найти его не удалось
                int countOfProcessedRows;
                if (requestLastPublicationId == null || requestLastPublicationId == "|")
                {
                    countOfProcessedRows = 1;
                }
                else
                {
                    if (CountAD != 0 && CountAD < liItems.Count)
                    {
                        countOfProcessedRows = CountAD;
                    }
                    else
                    {
                        countOfProcessedRows = liItems.Count;
                    }
                }

                OldRealtyAdvertismentPageParser oldRealtyParser = new OldRealtyAdvertismentPageParser();

                for (int i = 0, j = 0; (i < countOfProcessedRows) && (j < liItems.Count); j++)
                {
                    HtmlNode currLi = liItems[j];

                    HtmlNode currH4 = currLi.ChildNodes.SingleOrDefault(x => x.Name == "h4");
                    if (currH4 == null)
                    {
                        continue;
                    }

                    HtmlNode currAnchor = currH4.ChildNodes.SingleOrDefault(x => x.Name == "a");
                    if (currAnchor == null)
                    {
                        continue;
                    }

                    // если это 5к и больше рубрика по БД, то нужно игнорировать 4-к квартиры на сайте
                    if (currBind.RubricId == 1390 && currAnchor.InnerText.Contains("4-к.") == true)
                    {
                        continue;
                    }

                    // определим ссылку для парсинга конкретного объявления
                    String descriptionUrl = mainPartOfUrl + currAnchor.Attributes["href"].Value;

                    // определим ид последней обрабатываемой записи
                    currPublicationId = currAnchor.Attributes["href"].Value;
                    currPublicationId = currPublicationId.Replace(middlePartOfUrl, "");
                    currPublicationId = currPublicationId.Replace(".html", "");

                    if (firstPublicationId == null) firstPublicationId = currPublicationId; // i==0 не катит, так как могут быть пропуски

                    // выходим по достижении одинаковости идов, так как вытаскиваем до последнего вытащенного в прошлый раз (который первый по счету)
                    if (requestLastPublicationId == currPublicationId)
                    {
                        isReqIdExists = true;
                        break;
                    }

                    // создадим WebPublication из объявления на сайте 
                    WebPublication currWebPub = null;

                    // сюда попадаем только при поддерживаемом типе записи
                    try
                    {
                        //if (result.Publications.Count == 5)
                        //{
                        //    int kkk = 0;
                        //}

                        currWebPub = oldRealtyParser.Parse(descriptionUrl, mainPartOfUrl, currPublicationId, currBind);
                    }
                    catch
                    {
                        ; // если все таки при парсинге случились ошибки, то просто игнорируем такие строки
                        // СЮДА ХОРОШО БЫ ДОБАВИТЬ ЛОГИРОВАНИЕ ИЛИ ЕЩЕ ЧТО, ПОКА ИСПОЛЬЗУЕТСЯ ДЛЯ ТЕСТИРОВАНИЯ
                    }

                    // добвим публикацию в ответ
                    if (currWebPub != null)
                    {
                        result.Publications.Add(currWebPub);
                        i++; // наращиваем, только в случае, если все нормально допарсилось до конца и не было игнорируемых строк
                    }
                }
            }

            // общие параметры ответа парсера
            if (isReqIdExists == false && (requestLastPublicationId != null && requestLastPublicationId != "|"))
            {
                result.ResponseCode = ParseResponseCode.NotFoundId;
            }
            else
            {
                if (result.Publications.Count <= 0)
                {
                    result.ResponseCode = ParseResponseCode.ContentEmpty;
                }
                else
                {
                    result.ResponseCode = ParseResponseCode.Success;
                }
            }
            result.LastPublicationId = firstPublicationId;

            return result;
        }
    }
}
