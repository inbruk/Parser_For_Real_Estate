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
using HtmlAPDocument = HtmlAgilityPack.HtmlDocument;

namespace RT.ParsingLibs.E1RUParser
{
    // Парсер недвижимости с сайта e1.ru
    [Export(typeof(IParsingModule))]
    [ExportMetadata("Name", "E1RUParsingModule")]
    public class E1RUParsingModule : IParsingModule
    {
        private const String requestUrl = @"http://e1.ru/";
        
        /// <summary>
        /// Получить названия ресурсов, обрабатываемая библиотекой
        /// Если параметры неверные выбрасывает ArgumentOutOfRangeException
        /// </summary>
        /// <param name="bind">Бинд запроса</param>
        /// <returns> Коллекция названий ресурсов (сайтов)</returns>
        public IList<string> Sources(Bind bind)
        {
            SellRubric2GetRequestParamConverter sellConv = new SellRubric2GetRequestParamConverter();
            RentRubric2GetRequestParamConverter rentConv = new RentRubric2GetRequestParamConverter();

            StringBuilder resUrl = new StringBuilder(256);
            if( bind.ActionId==1 ) // купить = продают
            {
                resUrl.Append( sellConv.Convert(bind.RubricId) );
            }
            else if( bind.ActionId==3 ) // снять = сдают в аренду
            {
                resUrl.Append( rentConv.Convert(bind.RubricId) );
            }
            else
            {
                throw new ArgumentOutOfRangeException("Неподдерживаемые значения ActionId");
            }

            resUrl.Append( (new Region2GetRequestParamConverter()).Convert(bind.RegionId) );

            // включим сортировку в прошлое по дате последней модификации
            resUrl.Append("&by=_orderDate&order=DESC");

            return new List<String>() 
            {
                resUrl.ToString()
            };
        }

        /// <summary>
        /// Получить список ИД действий, обрабатываемая библиотекой
        /// </summary>
        /// <returns>Коллекция ИД действий</returns>
        public IList<int> KeysActions()
        {
            return new List<int>() 
            {
                1, // купить = продают
                3  // снять = сдают в аренду
            };
        }

        /// <summary>
        /// Получить список ИД рубрик, обрабатываемая библиотекой
        /// </summary>
        /// <returns>Коллекция ИД рубрик</returns>
        public IList<int> KeysRubrics()
        {
            return new List<int>() 
            {
                1383, //    Жилая

                1384, //	Вторичный рынок
                1385, //	комната Вторичный рынок
                1386, //	1- комнатная квартира Вторичный рынок
                1387, //	2- комнатная квартира Вторичный рынок
                1388, //	3- комнатная квартира Вторичный рынок
                1389, //	4- комнатная квартира Вторичный рынок
                1390, //	5- комнатная квартира и более Вторичный рынок

                1392, //	Новостройки
                1393, //	1- комнатная квартира Новостройки
                1394, //	2- комнатная квартира Новостройки
                1395, //	3- комнатная квартира Новостройки
                1396, //	4- комнатная квартира Новостройки
                1397, //	5- комнатная квартира и более Новостройки

                1399, //	Таунхаус
                1400, //	Дом, Коттедж
                
                4220, //	Участок, сад, дача

                1403, //	Коммерческая
                1404, //	Офисы
                1405, //	Производственно-складское помещение
                1406, //	Торговая площадь
                1407, //	Земля                

                4221, //	Гараж и стоянка
                4223, //	Гараж
                4224, //	Машиноместо в паркинге
                4226, //	Тент-пенал
                4227  //	Другое
            };
        }

        /// <summary>
        /// Получить список ИД регионов, обрабатываемая библиотекой
        /// </summary>
        /// <returns>Коллекция ИД регионов</returns>
        public IList<int> KeysRegions()
        {
            return new List<int>() 
            {
                66,   // Свердловская область (любой город)

                2944, //    г. Берёзовский
                2953, //	г. Артёмовский

                2931, // 	г. Екатеринбург
                2932, // 	г. Нижний Тагил
                2933, // 	г. Каменск-Уральский
                2934, // 	г. Первоуральск
                2935, // 	г. Серов
                2936, // 	г. Новоуральск
                2937, // 	г. Асбест
                2938, // 	г. Полевской
                2939, // 	г. Краснотурьинск
                2940, // 	г. Ревда
                2941, // 	г. Верхняя Пышма
                2942, // 	г. Лесной
                2943, // 	г. Верхняя Салда
                2945, // 	г. Качканар
                2946, // 	г. Алапаевск
                2947, // 	г. Ирбит
                2948, // 	г. Красноуфимск
                2949, // 	г. Реж
                2950, // 	г. Тавда
                2951, // 	г. Сухой Лог
                2952, // 	г. Кушва
                2954, // 	г. Богданович
                2955, // 	г. Североуральск
                2956, // 	г. Карпинск
                2957, // 	г. Камышлов
                2958, // 	г. Красноуральск
                2959, // 	г. Заречный
                2960, // 	г. Невьянск
                2961, // 	г. Нижняя Тура
                2962, // 	г. Кировград
                2963, // 	г. Сысерть
                2964, // 	г. Среднеуральск
                2965, // 	г. Талица
                2966, // 	г. Туринск
                2967, // 	г. Нижняя Салда
                2968, // 	пгт Рефтинский
                2969, // 	г. Ивдель
                2970, // 	г. Дегтярск
                2971, // 	г. Арамиль
                2972, // 	пгт Арти
                2973, // 	г. Новая Ляля
                2974, // 	п. Буланаш
                2975, // 	пгт Белоярский
                2976, // 	г. Верхний Тагил
                2977, // 	г. Нижние Серги
                2979, // 	п. Троицкий
                2981, // 	пгт Бисерть
                2982, // 	г. Верхняя Тура
                2983, // 	пгт Пышма
                2984, // 	г. Волчанск
                2985, // 	пгт Малышева
                2987, // 	г. Михайловск
                2989, // 	п. Еланский
                2991, // 	г. Верхотурье
                2992, // 	п. Черемухово
                2993, // 	п. Большой Исток
                2995, // 	пгт Шаля
                2996, // 	пгт Верхние Серги
                2997, // 	пгт Тугулым
                2998, // 	с. Байкалово
                3000, // 	с. Туринская Слобода
                3001, // 	п. Билимбай
                3002, // 	п. Цементный
                3004, // 	пгт Верх-Нейвинский
                3005, // 	п. Новоуткинск
                3006, // 	п. Монетный
                3008, // 	п. Бобровский
                3009, // 	п. Двуреченск
                3010, // 	пгт Ачит
                3011, // 	п. Красногвардейский
                3013, // 	пгт Верхнее Дуброво
                3018, // 	пгт Горноуральский
                3022, // 	п. Шабровский
                3023, // 	пгт Атиг
                3028, // 	пгт Пелым
                3030, // 	п. Совхозный
                3031, // 	п. Шамары
                3032, // 	п. Левиха
                3033, // 	п. Северка
                3035, // 	п. Исеть
                3037, // 	п. Кузино
                3038, // 	пгт Староуткинск
                3039, // 	п. Уфимский
                3043  // 	п. Садовый
            };
        }

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
        /// Задача на парсинг сайта http://e1.ru/
        /// </summary>
        /// <param name="request">параметры запроса на парсинг см. тип ParseRequest </param>
        /// <returns>Ответ от парсера</returns>
        public async Task<ParseResponse> Result(ParseRequest request)
        {
            // создадим корректный URL по параметрам парсера
            Bind currBind = new Bind((int)request.RubricId, (int)request.ActionId, (int)request.RegionId);
            String currUrl = await Task.Run(() => Sources(currBind)[0]);

            // парсим таблицу с результатами поиска на сайте, внутри делая дополнительные запросы для парсинга деталей
            ParseResponse parResp =  await Task.Run(() => (new E1RUSearchResultsTableParser()).Parse(currUrl, currBind, request.LastPublicationId));

            return parResp;
        }
    }
}
