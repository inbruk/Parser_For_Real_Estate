using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RT.ParsingLibs.E1RUParser
{
    internal class Region2GetRequestParamConverter : IParserParam2GetRequestParamConverter
    {
        /// <summary>
        /// Осуществляет преобразование, в случае, если параметр неверный выбрасывает ArgumentOutOfRangeException
        /// </summary>
        /// <param name="parserParam">значение параметра Region парсера</param>
        /// <returns>кусок GET Запроса</returns>
        public String Convert(Int32 parserParam)
        {
            switch (parserParam)
            {
                case 66:   return @""; // Свердловская область (любой город)

                case 2944: return @"118548"; //     г. Берёзовский                    ﻿
                case 2953: return @"119708"; //	    г. Артёмовский

                case 2931: return @"115488"; // 	г. Екатеринбург
                case 2932: return @"132108"; // 	г. Нижний Тагил
                case 2933: return @"131968"; // 	г. Каменск-Уральский
                case 2934: return @"130708"; // 	г. Первоуральск
                case 2935: return @"138468"; // 	г. Серов
                case 2936: return @"130588"; // 	г. Новоуральск
                case 2937: return @"116088"; // 	г. Асбест
                case 2938: return @"131308"; // 	г. Полевской
                case 2939: return @"129448"; // 	г. Краснотурьинск
                case 2940: return @"131588"; // 	г. Ревда
                case 2941: return @"116728"; // 	г. Верхняя Пышма
                case 2942: return @"130048"; // 	г. Лесной
                case 2943: return @"120988"; // 	г. Верхняя Салда
                case 2945: return @"118288"; // 	г. Качканар
                case 2946: return @"137988"; // 	г. Алапаевск
                case 2947: return @"138328"; // 	г. Ирбит
                case 2948: return @"138368"; // 	г. Красноуфимск
                case 2949: return @"125048"; // 	г. Реж
                case 2950: return @"127768"; // 	г. Тавда
                case 2951: return @"126268"; // 	г. Сухой Лог
                case 2952: return @"129788"; // 	г. Кушва
                case 2954: return @"120688"; // 	г. Богданович
                case 2955: return @"131748"; // 	г. Североуральск
                case 2956: return @"118108"; // 	г. Карпинск
                case 2957: return @"138348"; // 	г. Камышлов
                case 2958: return @"129568"; // 	г. Красноуральск
                case 2959: return @"117228"; // 	г. Заречный
                case 2960: return @"123108"; // 	г. Невьянск
                case 2961: return @"130148"; // 	г. Нижняя Тура
                case 2962: return @"118348"; // 	г. Кировград
                case 2963: return @"127128"; // 	г. Сысерть
                case 2964: return @"138628"; // 	г. Среднеуральск
                case 2965: return @"128188"; // 	г. Талица
                case 2966: return @"128708"; // 	г. Туринск
                case 2967: return @"138208"; // 	г. Нижняя Салда
                case 2968: return @"116188"; // 	пгт Рефтинский
                case 2969: return @"117328"; // 	г. Ивдель
                case 2970: return @"138808"; // 	г. Дегтярск
                case 2971: return @"126328"; // 	г. Арамиль
                case 2972: return @"119728"; // 	пгт Арти
                case 2973: return @"124028"; // 	г. Новая Ляля
                case 2974: return @"119328"; // 	п. Буланаш
                case 2975: return @"119968"; // 	пгт Белоярский
                case 2976: return @"138568"; // 	г. Верхний Тагил
                case 2977: return @"123648"; // 	г. Нижние Серги
                case 2979: return @"128108"; // 	п. Троицкий
                case 2981: return @"123228"; // 	пгт Бисерть
                case 2982: return @"138768"; // 	г. Верхняя Тура
                case 2983: return @"124808"; // 	пгт Пышма
                case 2984: return @"138708"; // 	г. Волчанск
                case 2985: return @"116168"; // 	пгт Малышева
                case 2987: return @"123668"; // 	г. Михайловск
                case 2989: return @"122268"; // 	п. Еланский
                case 2991: return @"121148"; // 	г. Верхотурье
                case 2992: return @"131928"; // 	п. Черемухово
                case 2993: return @"126388"; // 	п. Большой Исток
                case 2995: return @"128728"; // 	пгт Шаля
                case 2996: return @"123248"; // 	пгт Верхние Серги
                case 2997: return @"128208"; // 	пгт Тугулым
                case 2998: return @"197088"; // 	с. Байкалово
                case 3000: return @"253908"; // 	с. Туринская Слобода
                case 3001: return @"130728"; // 	п. Билимбай
                case 3002: return @"123068"; // 	п. Цементный
                case 3004: return @"122748"; // 	пгт Верх-Нейвинский
                case 3005: return @"131028"; // 	п. Новоуткинск
                case 3006: return @"116508"; // 	п. Монетный
                case 3008: return @"126368"; // 	п. Бобровский
                case 3009: return @"126488"; // 	п. Двуреченск
                case 3010: return @"119788"; // 	пгт Ачит
                case 3011: return @"116428"; // 	п. Красногвардейский
                case 3013: return @"120068"; // 	пгт Верхнее Дуброво
                case 3018: return @"124208"; // 	пгт Горноуральский
                case 3022: return @"116008"; // 	п. Шабровский
                case 3023: return @"123188"; // 	пгт Атиг
                case 3028: return @"117748"; // 	пгт Пелым
                case 3030: return @"115928"; // 	п. Совхозный
                case 3031: return @"129088"; // 	п. Шамары
                case 3032: return @"118408"; // 	п. Левиха
                case 3033: return @"115888"; // 	п. Северка
                case 3035: return @"116888"; // 	п. Исеть
                case 3037: return @"130908"; // 	п. Кузино
                case 3038: return @"129008"; // 	пгт Староуткинск
                case 3039: return @"119928"; // 	п. Уфимский
                case 3043: return @"115848"; // 	п. Садовый

                default: throw new ArgumentOutOfRangeException("Неподдерживаемые значения KeyRubric");
            }

            throw new ArgumentOutOfRangeException("Неподдерживаемые значения KeyRubric");
        }
    }
}
