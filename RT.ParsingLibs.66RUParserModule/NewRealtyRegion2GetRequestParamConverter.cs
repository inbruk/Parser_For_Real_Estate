using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RT.ParsingLibs.WWW66RUParser
{
    /// <summary>
    /// этот кусок пока не используется, вытаскиваются по городам фильтрацией в новостройках
    /// </summary>
    internal class NewRealtyRegion2GetRequestParamConverter : ParserParam2GetRequestParamConverterBase
    {
        /// <summary>
        /// Осуществляет преобразование, в случае, если параметр неверный выбрасывает ArgumentOutOfRangeException
        /// </summary>
        /// <param name="parserParam">значение параметра Region парсера в случае новостройки или коттеджи </param>
        /// <returns>кусок GET Запроса</returns>
        public override String Convert(Int32 parserParam)
        {
            switch (parserParam)
            {
                case 66:   return Prefix + @"";   // Свердловская область (любой город)
                case 2931: return Prefix + @"1";  // г. Екатеринбург
                case 2971: return Prefix + @"7";  // г. Арамиль
                case 2944: return Prefix + @"3";  // г. Берёзовский	
                case 2941: return Prefix + @"2";  // г. Верхняя Пышма	
                case 3009: return Prefix + @"55"; // п. Двуреченск	
                case 2964: return Prefix + @"57"; // г. Среднеуральск	
                case 3013: return Prefix + @"305"; // пгт Верхнее Дуброво	
                case 2963: return Prefix + @"27"; // г. Сысерть	
                default: break;
            }

            throw new ArgumentOutOfRangeException("Неподдерживаемые значения Action");
        }
    }
}
