using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Net;
using System.Text;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using RT.Crawler;
using RT.ParsingLibs.Models;
using RT.ParsingLibs.Requests;
using RT.ParsingLibs.Responses;

using HtmlAgilityPack;

namespace RT.ParsingLibs.WWW66RUParser
{
    // Парсер недвижимости с сайта http://66.ru/realty/
    [Export(typeof(IParsingModule))]
    [ExportMetadata("Name", "WWW66RUParserModule")]
    public class WWW66RUParserModule : IParsingModule
    {
        private const String requestUrl = @"http://66.ru/realty/";

        /// <summary>
        /// Получить информацию о разработчике
        /// </summary>
        /// <returns>Информация о разработчике</returns>
        public AboutResponse About()
        {
            return new AboutResponse()
            {
                Info = "uytutyu Korn",
                Contacts = "vab@royot.com",
                CopyRight = "All reserved"
            };
        }

        /// <summary>
        /// Получить список ИД действий, поддерживаемых библиотекой
        /// </summary>
        /// <returns>Коллекция ИД действий</returns>
        public IList<int> KeysActions()
        {
            return new List<int>()
            {
                1, // купить = продают
                3, // снять = сдают в аренду
                5  // аренда посуточно
            };
        }

        /// <summary>
        /// Получить список ИД рубрик, поддерживаемых библиотекой
        /// </summary>
        /// <returns>Коллекция ИД рубрик</returns>
        public IList<int> KeysRubrics()
        {
            return new List<int>()
            {
                1383, // Жилая
                1384, // вторичный рынок﻿
                1385, // комната вторичный рынок﻿	
                1386, // 1- комнатная квартира	вторичный рынок﻿
                1387, // 2- комнатная квартира	вторичный рынок﻿
                1388, // 3- комнатная квартира	вторичный рынок﻿
                1389, // 4- комнатная квартира	вторичный рынок﻿
                1390, // 5- комнатная квартира и более	вторичный рынок﻿

                //1392, // Новостройки
                
                4220, //	Участок, сад, дача
                1403, //	Коммерческая	
                1404, //    Офисы	Коммерческая	
                1405, //	Производственно-складское помещение	Коммерческая	
                1406, //	Торговая площадь	Коммерческая	
﻿                4221, //	Гараж и стоянка
                4223  // 	Гараж	
            };
        }

        /// <summary>
        /// Получить список ИД регионов, поддерживаемых библиотекой
        /// </summary>
        /// <returns>Коллекция ИД регионов</returns>
        public IList<int> KeysRegions()
        {
            return new List<int>()
            {
                 66,   // Свердловская область (любой город)
                 2931, // 	г. Екатеринбург

                 // Эти делаются фильтрацией и только в новостройках
                 2971, //	г. Арамиль
                 2944, //   г. Берёзовский
                 2941, //	г. Верхняя Пышма
                 3009, //	п. Двуреченск
                 2964, //	г. Среднеуральск
                 3013, //	пгт Верхнее Дуброво	
                 2963, //	г. Сысерть
            };
        }

        private String Generate1UrlLowlevel(Bind bind, Int32 posInRubric, Boolean useAction, Boolean isLivingRealtyRegions)
        {
            Rubric2GetRequestParamConverter rubConv = new Rubric2GetRequestParamConverter();
            Action2GetRequestParamConverter actConv = new Action2GetRequestParamConverter();
            LivingRegion2GetRequestParamConverter livRegConv = new LivingRegion2GetRequestParamConverter();
            NewRealtyRegion2GetRequestParamConverter newRealRegConv = new NewRealtyRegion2GetRequestParamConverter();
            
            StringBuilder resSB = new StringBuilder(1000);
            resSB.Append( requestUrl );

            String rubricFull = rubConv.Convert(bind.RubricId);
            String[] rubricArray = rubricFull.Split('|');
            String rubricPart = rubricArray[posInRubric];
            resSB.Append(rubricPart);

            if (useAction == true)
            {
                resSB.Append(actConv.Convert(bind.ActionId));
            }

            if (isLivingRealtyRegions==true)
            {
                resSB.Append( livRegConv.Convert(bind.RegionId) );
            }
            else
            {
                resSB.Append( newRealRegConv.Convert(bind.RegionId) );
            }

            String result = resSB.ToString();
            return result;
        }

        /// <summary>
        /// Получить коллекцию полных url-ов для выполнения парсинга заданного Bind 
        /// Если параметры неверные выбрасывает ArgumentOutOfRangeException
        /// </summary>
        /// <param name="bind"> параметры запроса на парсинг</param>
        /// <returns> Коллекция полных url-ов для выполнения парсинга заданного Bind </returns>
        public IList<string> Sources(Bind bind)
        {

            switch (bind.RubricId)
            {
                case 1383: // Жилая - то же самое, что и вторичный рвнок
                case 1384: // вторичный рынок﻿                  
                case 1385: // комната вторичный рынок﻿	
                case 1386: // 1- комнатная квартира	вторичный рынок﻿
                case 1387: // 2- комнатная квартира	вторичный рынок﻿
                case 1388: // 3- комнатная квартира	вторичный рынок﻿
                case 1389: // 4- комнатная квартира	вторичный рынок﻿
                case 1390: // 5- комнатная квартира и более вторичный рынок﻿ тут надо фильтровать
                    return new List<String>()
                    {
                        Generate1UrlLowlevel(bind, 0, true, true)
                    };
                break;

                //// ТУТ ВЫБИРАЕТСЯ ОДИНАКОВО, НУЖНО ФИЛЬТРОВАТЬ ПО ТИПУ И РЕГИОНУ ПРИ ВЫБОРКЕ !!!
                //case 1392: // рубрика Новостройки	нужно включать флаг дом сдан (что не соотв всем новостройкам)
                //    if (bind.ActionId == 1 ) // остальные виды действия не поддерживаются (съем и съем посуточный)
                //    {
                //        return new List<String>()
                //        {
                //            Generate1UrlLowlevel(bind, 0, false, false)
                //        };
                //    }
                //break;

                case 4220: // Участок сад дача
                case 1403: // Коммерческая	
                case 1404: // Офисы	Коммерческая	
                case 1406: // Торговая площадь	Коммерческая
                    if (bind.ActionId == 1 || bind.ActionId == 3) // остальные виды действия не поддерживаются (съем посуточный)
                    {
                        return new List<String>()
                        {
                            Generate1UrlLowlevel(bind, 0, true, true)
                        };
                    }
                break;

                case 1405: //	Производственно-складское помещение	Коммерческая	
                    if (bind.ActionId == 1 || bind.ActionId == 3) // остальные виды действия не поддерживаются (съем посуточный)
                    {
                        return new List<String>()
                        {
                            Generate1UrlLowlevel(bind, 0, true, true),
                            Generate1UrlLowlevel(bind, 1, true, true)
                        };
                    }
                break;

﻿                case 4221: // Гараж и стоянка - то же самое, что и Гараж
                case 4223: // Гараж	
                    if (bind.ActionId == 1 || bind.ActionId == 3) // остальные виды действия не поддерживаются (съем посуточный)
                    {
                        return new List<String>()
                        {
                            Generate1UrlLowlevel(bind, 0, true, true)
                        };
                    }
                break;

                default: break;
            }

            throw new ArgumentOutOfRangeException("Неподдерживаемые комбинации параметров в bind");
        }

        /// <summary>
        /// Задача на парсинг сайта http://66.ru/realty/
        /// Внимание ! Разделительный символ в идах и урлах это '|'. Их количество и там и там должно совпадать.
        /// </summary>
        /// <param name="request">параметры запроса на парсинг см. тип ParseRequest </param>
        /// <returns>Ответ от парсера</returns>
        public async Task<ParseResponse> Result(ParseRequest request)
        {
            // вытащим иды, в случае если их много (в нужном случае заменим null на "|")
            String ids = request.LastPublicationId;
            if( ids == null)
            {
                if (request.RubricId == 1405)
                {
                    ids = "|";
                }
                else
                {
                    ids = "";
                }
            }

            String[] ids_Array = ids.Split('|');

            // пустые строки в списке идов заменим на null
            for(int i=0; i<ids_Array.Length; i++)
            {
                if( ids_Array[i]==String.Empty )
                {
                    ids_Array[i] = null;
                }
            }

            // создадим корректный URL по параметрам парсера
            Bind currBind = new Bind((int)request.RubricId, (int)request.ActionId, (int)request.RegionId);
            IList<String> currUrls = await Task.Run(() => Sources(currBind));

            // парсим таблицу с результатами поиска на сайте по каждому из url-ов, внутри делая дополнительные запросы для парсинга деталей
            ParseResponse parResp = new ParseResponse();
            parResp.ModuleName = "WWW66RUParser";
            parResp.ResponseCode = ParseResponseCode.Success;
            parResp.Publications = new List<WebPublication>();
            parResp.LastPublicationId = null;
            for(int i=0; i<currUrls.Count; i++)
            {
                String currUrl = currUrls[i];
                String currId = ids_Array[i]; 
                ParseResponse currParResp = null;

                //if (request.RubricId >= 1392 && request.RubricId <= 1398)
                //{
                //    currParResp = await Task.Run(() => (new NewRealtySearchResultsTableParser()).Parse(currUrl, currBind, request.LastPublicationId));
                //}
                //else
                //{
                    currParResp = await Task.Run(() => (new OldRealtySearchResultsTableParser()).Parse(currUrl, currBind, request.LastPublicationId));
                //}

                foreach (WebPublication currWebPub in currParResp.Publications)
                {
                    parResp.Publications.Add(currWebPub);
                }

                if( i>=1 )
                {
                    if (parResp.LastPublicationId == null)
                    {
                        parResp.LastPublicationId = "|";
                    }
                    else
                    {
                        parResp.LastPublicationId = parResp.LastPublicationId + "|";
                    }
                }

                if( currParResp.LastPublicationId!=null )
                {
                    if( parResp.LastPublicationId==null )
                    {
                        parResp.LastPublicationId = currParResp.LastPublicationId;
                    }
                    else
                    {
                        parResp.LastPublicationId = parResp.LastPublicationId + currParResp.LastPublicationId;
                    }
                }

                if( currParResp.ResponseCode!= ParseResponseCode.Success )
                {
                    parResp.ResponseCode = currParResp.ResponseCode;
                }                
            }            

            return parResp;
        }
    }
}
