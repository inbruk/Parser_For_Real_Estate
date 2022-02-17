using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using RT.ParsingLibs.Models;
using RT.ParsingLibs.Requests;
using RT.ParsingLibs.Responses;
using RT.ParsingLibs.WWW66RUParser;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RT.ParsingLibs.ParsersTests
{
    [TestClass]
    public class WWW66RUParserModuleTest
    {
        [TestMethod]
        public async Task WWW66RULightTest()
        {
            WWW66RUParserModule currWWW66RUParser = new WWW66RUParserModule();

            AboutResponse aboutResult = currWWW66RUParser.About();

            IList<int> keysActionsResult = currWWW66RUParser.KeysActions();
            IList<int> keysRubrics = currWWW66RUParser.KeysRubrics();
            IList<int> keysRegions = currWWW66RUParser.KeysRegions();

            IList<string> sourcesResult1  = currWWW66RUParser.Sources(new Bind { ActionId = 1, RegionId = 66,   RubricId = 1383 });
            IList<string> sourcesResult2  = currWWW66RUParser.Sources(new Bind { ActionId = 3, RegionId = 2931, RubricId = 1384 }); 
            IList<string> sourcesResult3  = currWWW66RUParser.Sources(new Bind { ActionId = 5, RegionId = 66,   RubricId = 1385 });
            IList<string> sourcesResult4  = currWWW66RUParser.Sources(new Bind { ActionId = 1, RegionId = 2931, RubricId = 1386 });
            IList<string> sourcesResult5  = currWWW66RUParser.Sources(new Bind { ActionId = 3, RegionId = 66,   RubricId = 1387 });
            IList<string> sourcesResult6  = currWWW66RUParser.Sources(new Bind { ActionId = 5, RegionId = 2931, RubricId = 1388 });
            IList<string> sourcesResult7  = currWWW66RUParser.Sources(new Bind { ActionId = 1, RegionId = 66,   RubricId = 1389 });
            IList<string> sourcesResult8  = currWWW66RUParser.Sources(new Bind { ActionId = 3, RegionId = 2931, RubricId = 1390 });

            IList<string> sourcesResult15 = currWWW66RUParser.Sources(new Bind { ActionId = 3, RegionId = 2931, RubricId = 4220 });

            IList<string> sourcesResult16 = currWWW66RUParser.Sources(new Bind { ActionId = 1, RegionId = 66,   RubricId = 1403 });
            IList<string> sourcesResult17 = currWWW66RUParser.Sources(new Bind { ActionId = 3, RegionId = 2931, RubricId = 1404 });
            IList<string> sourcesResult18 = currWWW66RUParser.Sources(new Bind { ActionId = 1, RegionId = 66,   RubricId = 1405 });
            IList<string> sourcesResult19 = currWWW66RUParser.Sources(new Bind { ActionId = 3, RegionId = 2931, RubricId = 1406 });

            IList<string> sourcesResult20 = currWWW66RUParser.Sources(new Bind { ActionId = 1, RegionId = 66, RubricId = 4221 });
            IList<string> sourcesResult21 = currWWW66RUParser.Sources(new Bind { ActionId = 3, RegionId = 66, RubricId = 4223 });

            // проверки обработки ошибочных параметров
            //try
            //{
            //    IList<string> sourcesResult22 = currWWW66RUParser.Sources(new Bind { ActionId = 5, RegionId = 66,   RubricId = 8385 });
            //    IList<string> sourcesResult23 = currWWW66RUParser.Sources(new Bind { ActionId = 3, RegionId = 2931, RubricId = 1394 });
            //    IList<string> sourcesResult24 = currWWW66RUParser.Sources(new Bind { ActionId = 5, RegionId = 66, RubricId = 1405 });
            //}
            //catch(Exception ex)
            //{
            //    ex = null;
            //}


            ParseResponse parseResp1 = await currWWW66RUParser.Result(new ParseRequest() { ActionId = 1, RegionId = 2931, RubricId = 1387, LastPublicationId = "4627713-prodam-2-k-kvartiru-ul-proninoj-38-uktus" });
            ParseResponse parseResp2 = await currWWW66RUParser.Result(new ParseRequest() { ActionId = 1, RegionId = 2931, RubricId = 1405, LastPublicationId = "|" });

            ParseResponse parseResp;

            parseResp = await currWWW66RUParser.Result(new ParseRequest() { ActionId = 1, RegionId = 66, RubricId = 1383, LastPublicationId = null });
            parseResp = await currWWW66RUParser.Result(new ParseRequest() { ActionId = 3, RegionId = 2931, RubricId = 1384, LastPublicationId = null });
            parseResp = await currWWW66RUParser.Result(new ParseRequest() { ActionId = 5, RegionId = 66, RubricId = 1385, LastPublicationId = null });
            parseResp = await currWWW66RUParser.Result(new ParseRequest() { ActionId = 1, RegionId = 2931, RubricId = 1386, LastPublicationId = null });
            parseResp = await currWWW66RUParser.Result(new ParseRequest() { ActionId = 3, RegionId = 66, RubricId = 1387, LastPublicationId = null });
            parseResp = await currWWW66RUParser.Result(new ParseRequest() { ActionId = 5, RegionId = 2931, RubricId = 1388, LastPublicationId = null });
            parseResp = await currWWW66RUParser.Result(new ParseRequest() { ActionId = 1, RegionId = 66, RubricId = 1389, LastPublicationId = null });

            parseResp = await currWWW66RUParser.Result(new ParseRequest() { ActionId = 1, RegionId = 2931, RubricId = 1390, LastPublicationId = null });

            parseResp = await currWWW66RUParser.Result(new ParseRequest() { ActionId = 3, RegionId = 2931, RubricId = 4220, LastPublicationId = null });

            parseResp = await currWWW66RUParser.Result(new ParseRequest() { ActionId = 1, RegionId = 66, RubricId = 1403, LastPublicationId = null });
            parseResp = await currWWW66RUParser.Result(new ParseRequest() { ActionId = 3, RegionId = 2931, RubricId = 1404, LastPublicationId = null });
            parseResp = await currWWW66RUParser.Result(new ParseRequest() { ActionId = 1, RegionId = 66, RubricId = 1405, LastPublicationId = null });
            parseResp = await currWWW66RUParser.Result(new ParseRequest() { ActionId = 3, RegionId = 2931, RubricId = 1406, LastPublicationId = null });

            parseResp = await currWWW66RUParser.Result(new ParseRequest() { ActionId = 1, RegionId = 66, RubricId = 4221, LastPublicationId = null });
            parseResp = await currWWW66RUParser.Result(new ParseRequest() { ActionId = 3, RegionId = 66, RubricId = 4223, LastPublicationId = null });



        }
    }
}
