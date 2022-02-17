using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RT.ParsingLibs.E1RUParser
{
    /// <summary>
    /// Преобразователь одного измерения параметров парсера в часть строки GET Запроса
    /// </summary>
    internal interface IParserParam2GetRequestParamConverter
    {
        /// <summary>
        /// Осуществляет преобразование, в случае, если параметр неверный выбрасывает ArgumentOutOfRangeException
        /// </summary>
        /// <param name="parserParam">значение одного параметра парсера</param>
        /// <returns>кусок GET Запроса</returns>
        String Convert(Int32 parserParam);
    }
}
