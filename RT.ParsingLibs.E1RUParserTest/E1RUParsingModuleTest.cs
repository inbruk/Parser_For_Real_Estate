using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using RT.ParsingLibs.Models;
using RT.ParsingLibs.Requests;
using RT.ParsingLibs.Responses;
using RT.ParsingLibs.E1RUParser;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RT.ParsingLibs.ParsersTests
{
    [TestClass]
    public class E1RUParserModuleTest
    {
        [TestMethod]
        public async Task E1RULightTest()
        {
            E1RUParsingModule currE1RUParser = new E1RUParsingModule();

            AboutResponse aboutResult = currE1RUParser.About();

            IList<int> keysActionsResult = currE1RUParser.KeysActions();
            IList<int> keysRubrics = currE1RUParser.KeysRubrics();
            IList<int> keysRegions = currE1RUParser.KeysRegions();

            Bind bind = new Bind
            {
                ActionId = 1,
                RegionId = 66,
                RubricId = 1384
            };
            IList<string> sourcesResult = currE1RUParser.Sources(bind);

            ParseResponse parseResp;

            parseResp = await  currE1RUParser.Result(new ParseRequest() { ActionId = 1, RegionId = 66, RubricId = 1384, LastPublicationId = "63646068" });

            parseResp = await  currE1RUParser.Result(new ParseRequest() { ActionId = 1, RegionId = 2931, RubricId = 1385, LastPublicationId = null });
            parseResp = await  currE1RUParser.Result(new ParseRequest() { ActionId = 1, RegionId = 2932, RubricId = 1386, LastPublicationId = null });
            parseResp = await  currE1RUParser.Result(new ParseRequest() { ActionId = 1, RegionId = 2933, RubricId = 1387, LastPublicationId = null });
            parseResp = await  currE1RUParser.Result(new ParseRequest() { ActionId = 1, RegionId = 2934, RubricId = 1388, LastPublicationId = null });
            parseResp = await  currE1RUParser.Result(new ParseRequest() { ActionId = 1, RegionId = 2935, RubricId = 1389, LastPublicationId = null });
            parseResp = await  currE1RUParser.Result(new ParseRequest() { ActionId = 1, RegionId = 2936, RubricId = 1390, LastPublicationId = null });

            parseResp = await  currE1RUParser.Result(new ParseRequest() { ActionId = 1, RegionId = 2937, RubricId = 1392, LastPublicationId = null });
            parseResp = await  currE1RUParser.Result(new ParseRequest() { ActionId = 1, RegionId = 2938, RubricId = 1393, LastPublicationId = null });
            parseResp = await  currE1RUParser.Result(new ParseRequest() { ActionId = 1, RegionId = 2939, RubricId = 1394, LastPublicationId = null });
            parseResp = await  currE1RUParser.Result(new ParseRequest() { ActionId = 1, RegionId = 2940, RubricId = 1395, LastPublicationId = null });
            parseResp = await  currE1RUParser.Result(new ParseRequest() { ActionId = 1, RegionId = 2941, RubricId = 1396, LastPublicationId = null });
            parseResp = await  currE1RUParser.Result(new ParseRequest() { ActionId = 1, RegionId = 2942, RubricId = 1397, LastPublicationId = null });

            parseResp = await  currE1RUParser.Result(new ParseRequest() { ActionId = 1, RegionId = 2931, RubricId = 1399, LastPublicationId = null });
            parseResp = await  currE1RUParser.Result(new ParseRequest() { ActionId = 1, RegionId = 2931, RubricId = 1400, LastPublicationId = null });

            parseResp = await  currE1RUParser.Result(new ParseRequest() { ActionId = 1, RegionId = 66, RubricId = 4220, LastPublicationId = null });

            parseResp = await  currE1RUParser.Result(new ParseRequest() { ActionId = 1, RegionId = 66, RubricId = 1403, LastPublicationId = null });
            parseResp = await  currE1RUParser.Result(new ParseRequest() { ActionId = 1, RegionId = 66, RubricId = 1404, LastPublicationId = null });
            parseResp = await  currE1RUParser.Result(new ParseRequest() { ActionId = 1, RegionId = 66, RubricId = 1405, LastPublicationId = null });
            parseResp = await  currE1RUParser.Result(new ParseRequest() { ActionId = 1, RegionId = 66, RubricId = 1406, LastPublicationId = null });
            parseResp = await  currE1RUParser.Result(new ParseRequest() { ActionId = 1, RegionId = 66, RubricId = 1407, LastPublicationId = null });

            parseResp = await  currE1RUParser.Result(new ParseRequest() { ActionId = 1, RegionId = 66, RubricId = 4221, LastPublicationId = null });
            parseResp = await  currE1RUParser.Result(new ParseRequest() { ActionId = 1, RegionId = 66, RubricId = 4223, LastPublicationId = null });
            parseResp = await  currE1RUParser.Result(new ParseRequest() { ActionId = 1, RegionId = 66, RubricId = 4224, LastPublicationId = null });
            parseResp = await  currE1RUParser.Result(new ParseRequest() { ActionId = 1, RegionId = 66, RubricId = 4226, LastPublicationId = null });
            parseResp = await  currE1RUParser.Result(new ParseRequest() { ActionId = 1, RegionId = 66, RubricId = 4227, LastPublicationId = null });

            parseResp = await  currE1RUParser.Result(new ParseRequest() { ActionId = 3, RegionId = 2931, RubricId = 1407, LastPublicationId = null });


            parseResp = await currE1RUParser.Result(new ParseRequest() { ActionId = 1, RegionId = 2931, RubricId = 1396, LastPublicationId = "8350148" });
        }
    }
}
