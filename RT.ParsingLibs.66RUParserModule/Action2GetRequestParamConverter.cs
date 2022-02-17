using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RT.ParsingLibs.WWW66RUParser
{
    internal class Action2GetRequestParamConverter : ParserParam2GetRequestParamConverterBase
    {
        /// <summary>
        /// Осуществляет преобразование, в случае, если параметр неверный выбрасывает ArgumentOutOfRangeException
        /// </summary>
        /// <param name="parserParam">значение параметра Action парсера</param>
        /// <returns>кусок GET Запроса</returns>
        public override String Convert(Int32 parserParam)
        {
            switch (parserParam)
            {
                case 1: return @"action_type=buy&";   // купить = продают
                case 3: return @"action_type=lease&"; // снять = сдают в аренду
                case 5: return @"action_type=day&";   // аренда посуточно
                default: break;
            }

            throw new ArgumentOutOfRangeException("Неподдерживаемые значения Action");
        }
    }
}
