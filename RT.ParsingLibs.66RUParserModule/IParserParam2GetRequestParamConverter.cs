using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RT.ParsingLibs.WWW66RUParser
{
    /// <summary>
    /// Преобразователь одного измерения параметров парсера в часть строки GET Запроса
    /// </summary>
    internal abstract class ParserParam2GetRequestParamConverterBase
    {
        /// <summary>
        /// префикс для всех получаемых таким образом параметров, в большинстве случаев не задан и не используется
        /// </summary>
        public String Prefix { get; set; }

        /// <summary>
        /// Осуществляет преобразование, в случае, если параметр неверный выбрасывает ArgumentOutOfRangeException
        /// </summary>
        /// <param name="parserParam">значение одного параметра парсера</param>
        /// <returns>кусок GET Запроса</returns>
        public abstract String Convert(Int32 parserParam);
    }
}
