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
using HtmlAPDocument = HtmlAgilityPack.HtmlDocument;

namespace RT.ParsingLibs.E1RUParser
{
    internal class E1RUSearchResultsTableParser 
    {
        public ParseResponse Parse(String currUrl, Bind currBind, String requestLastPublicationId)
        {
            Boolean isReqIdExists = false;
            String firstPublicationId = null;
            String currPublicationId = null;

            // загрузим HtmlDocument 
            HtmlAPDocument htmlDoc = CrawlerHelper.GetHtmlDocumentByUrl(currUrl);

            // найдем по полному урлу с параметрами его основную часть
            int idxOfRu = currUrl.IndexOf(".ru");
            String currMainPartOfUrl = currUrl.Remove(idxOfRu + 3);   

            // теперь определим является ли текущий запрос тем в котором нужно фильтровать таунхаусы
            Boolean IsHouseCottageWithoutTownhouseRequest = currUrl.Contains("formId=69");

            // общая инициализация ответа парсера
            ParseResponse result = new ParseResponse();
            result.Publications = new List<WebPublication>();            

            // вытащим строки таблицы TR
            HtmlNodeCollection tableRows = htmlDoc.DocumentNode.SelectNodes(@"//table[@class='re-search-result-table']//tbody//tr");
            if (tableRows != null)
            {

                // проходим, по CountAD строками или по всему списку
                int CountAD = 15; // временно, потом заменить на настоящий из библиотеки, найти его не удалось
                int countOfProcessedRows;
                if (requestLastPublicationId == null)
                {
                    countOfProcessedRows = 1;
                }
                else
                {
                    if (CountAD != 0 && CountAD < tableRows.Count)
                    {
                        countOfProcessedRows = CountAD;
                    }
                    else
                    {
                        countOfProcessedRows = tableRows.Count;
                    }
                }
                for (int i = 0; i < countOfProcessedRows; i++)
                {
                    HtmlNode currRow = tableRows[i];
                    List<HtmlNode> tds = currRow.ChildNodes.Where(x => x.Name == "td").ToList();

                    HtmlNode spanTypeNode = tds[1].ChildNodes.SingleOrDefault(x => x.Name == "span");
                    if (spanTypeNode == null) continue;
                    String type = spanTypeNode.InnerText;

                    HtmlNode anchorNode = tds[0].ChildNodes.SingleOrDefault(x => x.Name == "a");
                    if (anchorNode == null) continue;
                    String descriptionUrl = currMainPartOfUrl + anchorNode.Attributes["href"].Value;

                    // определим ид последней обрабатываемой записи
                    Int32 lastPublicationIdIndex = descriptionUrl.IndexOf("view/") + 5;
                    currPublicationId = descriptionUrl.Remove(0, lastPublicationIdIndex);

                    if (firstPublicationId == null) firstPublicationId = currPublicationId; // i==0 не катит, так как могут быть пропуски

                    // выходим по достижении одинаковости идов, так как вытаскиваем до последнего вытащенного в прошлый раз (который первый по счету)
                    if (requestLastPublicationId == currPublicationId)
                    {
                        isReqIdExists = true;
                        break;
                    }

                    // создадим WebPublication из объявления на сайте 
                    WebPublication currWebPub = null;
                    CommonRealtyAdvertismentPageParser commonRealtyParser = new CommonRealtyAdvertismentPageParser();
                    switch (type)
                    {
                        case "ком":  // Комната Вторичный рынок
                        case "1":  // 1- комнатная квартира Вторичный рынок
                        case "2":  // 2- комнатная квартира Вторичный рынок
                        case "3":  // 3- комнатная квартира Вторичный рынок
                        case "4":  // 4- комнатная квартира Вторичный рынок
                        case "5":  // 5- комнатная квартира Вторичный рынок

                        //	Дом, Коттедж ... (без таунхауса)
                        case "Дом":
                        case "Часть дома":
                        case "Коттедж":
                        case "Часть коттеджа":

                        //	Участок, сад, дача
                        case "Дачи":
                        case "Садовый участок":

                        //	Коммерческая
                        case "Офисное помещение":   // Офисы 
                        case "Производственно-складское помещение":  // Производственно-складское помещение 
                        case "Торговые площади":    //	Торговая площадь 
                        case "Коммерческая земля":  //	Земля 

                        //	Гараж и стоянка
                        case "Капитальный гараж":    // Гараж - Гараж и стоянка (Капитальный гараж на сайте)
                        case "Парковочное место":    // Машиноместо в паркинге - Гараж и стоянка
                        case "Металлический гараж":  // Тент-пенал - Гараж и стоянка (Металлический гараж)
                        case "Овощехранилище":       // Другое - Гараж и стоянка (Овощехранилище)
                        case "Земельный участок":    //	Земля Коммерческая

                            // сюда попадаем только при поддерживаемом типе записи
                            try
                            {
                                //if (result.Publications.Count == 5)
                                //{
                                //    int kkk = 0;
                                //}
                                currWebPub = commonRealtyParser.Parse(descriptionUrl, currMainPartOfUrl, currPublicationId, currBind); // lastPublicationId == currPublicationId
                            }
                            catch
                            {
                                break; // если все таки при парсинге случились ошибки, то просто игнорируем такие строки
                                // СЮДА ХОРОШО БЫ ДОБАВИТЬ ЛОГИРОВАНИЕ ИЛИ ЕЩЕ ЧТО, ПОКА ИСПОЛЬЗУЕТСЯ ДЛЯ ТЕСТИРОВАНИЯ
                            }
                            break;

                        case "Таунхаус": // Таунхаус
                            if (IsHouseCottageWithoutTownhouseRequest == false)
                            {
                                try
                                {
                                    currWebPub = commonRealtyParser.Parse(descriptionUrl, currMainPartOfUrl, currPublicationId, currBind); // lastPublicationId == currPublicationId
                                }
                                catch
                                {
                                    break; // если все таки при парсинге случились ошибки, то просто игнорируем такие строки
                                    // СЮДА ХОРОШО БЫ ДОБАВИТЬ ЛОГИРОВАНИЕ ИЛИ ЕЩЕ ЧТО, ПОКА ИСПОЛЬЗУЕТСЯ ДЛЯ ТЕСТИРОВАНИЯ
                                }
                            }
                            break;

                        default: break; // неподдерживаемые типы просто игнорируем
                    }

                    // добвим публикацию в ответ
                    if (currWebPub != null)
                    {
                        result.Publications.Add(currWebPub);
                    }
                }
            }

            // общие параметры ответа парсера
            if (isReqIdExists == false && requestLastPublicationId != null)
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
            result.ModuleName = "E1RUParser";

            return result;
        }
    }
}
