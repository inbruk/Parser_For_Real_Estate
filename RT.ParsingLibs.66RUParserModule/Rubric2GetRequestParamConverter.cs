using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RT.ParsingLibs.WWW66RUParser
{
    internal class Rubric2GetRequestParamConverter : ParserParam2GetRequestParamConverterBase
    {
        /// <summary>
        /// Осуществляет преобразование, в случае, если параметр неверный выбрасывает ArgumentOutOfRangeException
        /// </summary>
        /// <param name="parserParam">значение параметра Rubric парсера</param>
        /// <returns>кусок GET Запроса</returns>
        public override String Convert(Int32 parserParam)
        {
            switch (parserParam)
            {
                case 1383:                           // Жилая - то же самое, что и вторичный рвнок
                case 1384: return @"doska/live/?sort=newest-first&object_type=kv&";   // вторичный рынок﻿

                case 1385: return @"doska/live/?object_type=room&sort=newest-first&"; // комната вторичный рынок﻿	

                case 1386: return @"doska/live/?object_type=kv&rooms=1&sort=newest-first&"; // 1- комнатная квартира	вторичный рынок﻿

                case 1387: return @"doska/live/?object_type=kv&rooms=2&sort=newest-first&"; // 2- комнатная квартира	вторичный рынок﻿

                case 1388: return @"doska/live/?object_type=kv&rooms=3&sort=newest-first&"; // 3- комнатная квартира	вторичный рынок﻿

                case 1389:                                                                  // 4- комнатная квартира	вторичный рынок﻿
                case 1390: return @"doska/live/?object_type=kv&rooms=4&sort=newest-first&"; // 5- комнатная квартира и более вторичный рынок﻿ ТУТ НАДО ФИЛЬТРОВАТЬ !!!

                //case 1392: return @"new/";   // рубрика Новостройки	

                case 4220: return @"doska/zagorod/?sort=newest-first&"; //	Участок сад дача

                case 1403: return @"doska/com/?sort=newest-first&"; //	Коммерческая	

                case 1404: return @"doska/com/?object_type=6&sort=newest-first&"; //   Офисы	Коммерческая	

                case 1405: return @"doska/com/?object_type=7&sort=newest-first&|doska/com/?object_type=8&sort=newest-first&"; //	Производственно-складское помещение	Коммерческая	

                case 1406: return @"doska/com/?object_type=5&sort=newest-first&"; //	Торговая площадь	Коммерческая
	
﻿                case 4221:                                           //	Гараж и стоянка - то же самое, что и Гараж
                 case 4223: return @"doska/live/?object_type=garage&sort=newest-first&"; // 	Гараж	
                
                default: break;
            }

            throw new ArgumentOutOfRangeException("Неподдерживаемые значения Action");
        }
    }
}
