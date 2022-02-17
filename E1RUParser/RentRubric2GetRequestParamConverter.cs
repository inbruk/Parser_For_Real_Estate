using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RT.ParsingLibs.E1RUParser
{
    internal class RentRubric2GetRequestParamConverter : IParserParam2GetRequestParamConverter
    {
        /// <summary>
        /// Осуществляет преобразование, в случае, если параметр неверный выбрасывает ArgumentOutOfRangeException
        /// </summary>
        /// <param name="parserParam">значение параметра Rubric парсера</param>
        /// <returns>кусок GET Запроса</returns>
        public String Convert(Int32 parserParam)
        {
            switch(parserParam)
            {
                case 1383: return @"http://arenda.e1.ru/rent_available/?formId=7&intOption2="; // Жилая
                case 1384: return @"http://arenda.e1.ru/rent_available/?formId=7&intOption2="; // Вторичный рынок квартиры и комнаты

                case 1385: return @"http://arenda.e1.ru/rent_available/?formId=3&flat[365]=1&intOption2="; // Комната Вторичный рынок
                case 1386: return @"http://arenda.e1.ru/rent_available/?formId=5&flat[366]=1&intOption2="; // 1- комнатная квартира Вторичный рынок
                case 1387: return @"http://arenda.e1.ru/rent_available/?formId=5&flat[367]=1&intOption2="; // 2- комнатная квартира Вторичный рынок
                case 1388: return @"http://arenda.e1.ru/rent_available/?formId=5&flat[368]=1&intOption2="; // 3- комнатная квартира Вторичный рынок
                case 1389: return @"http://arenda.e1.ru/rent_available/?formId=5&flat[369]=1&intOption2="; // 4- комнатная квартира Вторичный рынок
                case 1390: return @"http://arenda.e1.ru/rent_available/?formId=5&flat[370]=1&intOption2="; // 5- комнатная квартира Вторичный рынок

                case 1399: return @"http://cottage.e1.ru/rent_available/?formId=91&flat[511]=1&intOption2="; //	Таунхаус
                case 1400: return @"http://cottage.e1.ru/rent_available/?formId=71&intOption2="; //	Дом, Коттедж

                case 4220: return @"http://dacha.e1.ru/rent_available/?formId=95&intOption14=";    //	Участок, сад, дача

                case 1403: return @"http://kn.e1.ru/rent_available/?formId=15&intOption2="; //	Коммерческая
                case 1404: return @"http://kn.e1.ru/rent_available/?formId=19&flat[461]=1&intOption2="; //	Офисы Коммерческая
                case 1405: return @"http://kn.e1.ru/rent_available/?formId=39&flat[466]=1&intOption2="; //	Производственно-складское помещение Коммерческая
                case 1406: return @"http://kn.e1.ru/rent_available/?formId=27&flat[463]=1&intOption2="; //	Торговая площадь Коммерческая

                case 1407: return @"http://land.e1.ru/rent_available/?formId=63&flat[493]=1&intOption14="; // Земля Коммерческая = Земля Коммерческая 
                // case 1407: return @"http://land.e1.ru/rent_available/?formId=55&intOption14="; // Земля Коммерческая = Земля на сайте

                case 4221: return @"http://garages.e1.ru/rent_available/?formId=99&intOption2=";  //	Гараж и стоянка
                case 4223: return @"http://garages.e1.ru/rent_available/?formId=103&flat[490]=1&intOption2="; //	Гараж - Гараж и стоянка (Капитальный гараж на сайте)
                case 4224: return @"http://garages.e1.ru/rent_available/?formId=107&flat[491]=1&intOption2="; //	Машиноместо в паркинге - Гараж и стоянка
                case 4226: return @"http://garages.e1.ru/rent_available/?formId=111&flat[1893]=1&intOption2="; //	Тент-пенал - Гараж и стоянка (Металлический гараж)
                case 4227: return @"http://garages.e1.ru/rent_available/?formId=115&flat[1894]=1&intOption2="; //	Другое - Гараж и стоянка (Овощехранилище)

                default: throw new ArgumentOutOfRangeException("Неподдерживаемые значения KeyRubric");
            }

            throw new ArgumentOutOfRangeException("Неподдерживаемые значения KeyRubric");
        }
    }
}
