using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RT.ParsingLibs.WWW66RUParser
{
    internal class LivingRegion2GetRequestParamConverter : ParserParam2GetRequestParamConverterBase
    {
        /// <summary>
        /// Осуществляет преобразование, в случае, если параметр неверный выбрасывает ArgumentOutOfRangeException
        /// </summary>
        /// <param name="parserParam">значение параметра Region парсера в случае "жилой недвижимости" </param>
        /// <returns>кусок GET Запроса</returns>
        public override String Convert(Int32 parserParam)
        {
            switch (parserParam)
            {
                case 66:   return @"location=";  // Свердловская область (любой город)
                case 2931: return @"location=0"; // г. Екатеринбург
                default: break;
            }

            throw new ArgumentOutOfRangeException("Неподдерживаемые значения Action");
        }
    }
}
