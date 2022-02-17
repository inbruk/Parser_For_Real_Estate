using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Linq;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using System.Globalization;
using RT.Crawler;
using RT.ParsingLibs.Models;
using RT.ParsingLibs.Requests;
using RT.ParsingLibs.Responses;

using HtmlAgilityPack;
using HtmlAPDocument = HtmlAgilityPack.HtmlDocument;

namespace RT.ParsingLibs.E1RUParser
{
    internal class CommonRealtyAdvertismentPageParser
    {
        public String ClearStringFromSlashTSlashNQuotNBsp(String src)
        {
            String res = src.Replace("\n", "").Replace("\t", "").Replace("&quot;", "").Replace("&nbsp;", "");
            return res;
        }

        public HtmlNode GetNextDDNodeInContacts(HtmlNode dtNode)
        {
            if (dtNode == null) return null;
            if (dtNode.ParentNode == null) return null;

            HtmlNode resultNode = null;

            HtmlNodeCollection dtNodeParentChildrenColl = dtNode.ParentNode.ChildNodes;
            Int32 position = dtNodeParentChildrenColl.IndexOf(dtNode) + 1;
            if (dtNodeParentChildrenColl.Count > position && dtNodeParentChildrenColl[position].Name=="dd" )
            {
                resultNode = dtNodeParentChildrenColl[position];
            }
            else
            {
                position++;
                if (dtNodeParentChildrenColl.Count > position && dtNodeParentChildrenColl[position].Name=="dd" )
                {
                    resultNode = dtNodeParentChildrenColl[position];
                }
            }

            return resultNode;
        }

        public WebPublication Parse(String currUrl, String mainPartOfUrl, String currPublicationId, Bind currBind)
        {
            CultureInfo culture = CultureInfo.InvariantCulture;

            WebPublication webPub = new WebPublication();
            webPub.PublicationId = currPublicationId;
            webPub.Url = new Uri(currUrl);
            webPub.Site = new Uri(mainPartOfUrl);
            webPub.ActionId = currBind.ActionId;
            webPub.RegionId = currBind.RegionId;
            webPub.RubricId = currBind.RubricId;

            // загрузим HtmlDocument из HtmlAgilityPack для последующего парсинга --------------------------------------------------------------------------------------------------------------------------
            HtmlAPDocument htmlDoc = CrawlerHelper.GetHtmlDocumentByUrl(currUrl);

            webPub.AdditionalInfo = new AdditionalInfo()
            {
                RealtyAdditionalInfo = new RealtyAdditionalInfo()
            };
            
            HtmlNode docNode = htmlDoc.DocumentNode;
            HtmlNodeCollection h3Nodes = docNode.SelectNodes(@"//h3");

            // теперь используя HtmlAgilityPack находим нужные параметры на странице и заполняем WebPublication.AdditionalInfo.RealtyAdditionalInfo --------------------------------------------------------

            // Адрес
            HtmlNode addressNode = docNode.SelectSingleNode(@"//p[@class='card__address']");
            String fullAddress = addressNode.InnerText;
            String districtFromAddress = null;
            String pureAddress = null;
            if( fullAddress.Contains("район") )
            {
                
                String[] fullAddressSplitted = fullAddress.Split(',');
                Int32 countOfParts = fullAddressSplitted.Length;
                
                if (countOfParts >= 2)
                    districtFromAddress = fullAddressSplitted[1];

                StringBuilder pureAddressSB = new StringBuilder(255);
                pureAddressSB.Append(fullAddressSplitted[0]);
                pureAddressSB.Append(",");

                if (countOfParts >= 3)
                    pureAddressSB.Append(fullAddressSplitted[2]);

                pureAddressSB.Append(",");

                if (countOfParts >= 4)
                    pureAddressSB.Append(fullAddressSplitted[3]);

                if (countOfParts == 4)
                    pureAddress = pureAddressSB.ToString();
                else
                    pureAddress = fullAddress;
            }
            else
            {
                pureAddress = fullAddress;
            }

            if (addressNode != null)
            {
                webPub.AdditionalInfo.RealtyAdditionalInfo.Address = pureAddress;
            }

            // Полная стоимость
            HtmlNode costAllNode = docNode.SelectSingleNode(@"//p[@class='card__cost']");
            if (costAllNode != null)
            {
                String costAllStr = costAllNode.InnerText;
                int rubIndex = costAllStr.IndexOf("руб");
                if (rubIndex > 0)
                    costAllStr = costAllStr.Remove(rubIndex);
                costAllStr = ClearStringFromSlashTSlashNQuotNBsp(costAllStr);
                Decimal costAllDecimal;
                if (Decimal.TryParse(costAllStr, out costAllDecimal))
                {
                    webPub.AdditionalInfo.RealtyAdditionalInfo.CostAll = costAllDecimal;
                }
            }

            // стоимость за метр квадратный 
            HtmlNode сostPerMeterNode = docNode.SelectSingleNode(@"//p[@class='card__price']");
            if (сostPerMeterNode != null)
            {
                String сostPerMeterStr = сostPerMeterNode.InnerText;
                int rubIndex = сostPerMeterStr.IndexOf("руб");
                if(rubIndex>0)
                {
                    сostPerMeterStr = сostPerMeterStr.Remove(rubIndex);
                }                
                сostPerMeterStr = ClearStringFromSlashTSlashNQuotNBsp(сostPerMeterStr);
                Decimal сostPerMeterDecimal;
                if (Decimal.TryParse(сostPerMeterStr, out сostPerMeterDecimal))
                {
                    webPub.AdditionalInfo.RealtyAdditionalInfo.CostPerMeter = сostPerMeterDecimal;
                }
            }

            IEnumerable<HtmlNode> dts = htmlDoc.DocumentNode.SelectNodes(@"//dt[@class='key-value__key']").Nodes(); // все dt из таблицы в середине
            HtmlNodeCollection dtsSpaceNodes = htmlDoc.DocumentNode.SelectNodes(@"//dt[@class='sms-card-list__key']");
            if (dtsSpaceNodes != null)
            {
                IEnumerable<HtmlNode> dtsSpace = dtsSpaceNodes.ToList(); // все dt в строке где площадь

                // Этаж
                HtmlNode floorNode = dts.SingleOrDefault(x => x.InnerHtml != null && x.InnerHtml == "Этаж");
                if (floorNode != null)
                {
                    floorNode = floorNode.ParentNode.ParentNode.ChildNodes.SingleOrDefault(x => x.Name == "dd");

                    String floorNodeStr = floorNode.InnerText;
                    Int32 floorNodeInt;
                    if (Int32.TryParse(floorNodeStr, out floorNodeInt))
                    {
                        webPub.AdditionalInfo.RealtyAdditionalInfo.Floor = floorNodeInt;
                    }
                }

                // Этажность, количество этажей
                HtmlNode floorNumberNode = dts.SingleOrDefault(x => x.InnerHtml != null && x.InnerHtml.Contains("Этажность"));
                if (floorNumberNode != null)
                {
                    floorNumberNode = floorNumberNode.ParentNode.ParentNode.ChildNodes.SingleOrDefault(x => x.Name == "dd");

                    String floorNumberNodeStr = floorNumberNode.InnerText;
                    Int32 floorNumberNodeInt;
                    if (Int32.TryParse(floorNumberNodeStr, out floorNumberNodeInt))
                    {
                        webPub.AdditionalInfo.RealtyAdditionalInfo.FloorNumber = floorNumberNodeInt;
                    }
                }

                // Общая площадь
                HtmlNode totalSpaceNode = dtsSpace.Where(x => x.InnerText != null).SingleOrDefault(x => x.InnerText.Contains("бщая"));
                if (totalSpaceNode == null)
                {
                    totalSpaceNode = dtsSpace.SingleOrDefault(x => x.InnerHtml != null && x.InnerHtml.Contains("площадь дома")); // для дач и коттеджей
                }
                if (totalSpaceNode != null)
                {
                    totalSpaceNode = totalSpaceNode.ParentNode.ChildNodes.SingleOrDefault(x => x.Name == "dd");

                    String totalSpaceStr = totalSpaceNode.InnerText;
                    int mIndex = totalSpaceStr.IndexOf(" м");
                    if (mIndex > 0)
                        totalSpaceStr = totalSpaceStr.Remove(mIndex);
                    totalSpaceStr = ClearStringFromSlashTSlashNQuotNBsp(totalSpaceStr);
                    Double totalSpaceDbl;
                    if (Double.TryParse(totalSpaceStr, NumberStyles.AllowDecimalPoint, culture, out totalSpaceDbl))
                    {
                        Int32 totalSpaceInt = (Int32)totalSpaceDbl;
                        webPub.AdditionalInfo.RealtyAdditionalInfo.TotalSpace = totalSpaceInt;
                    }
                }

                // Жилая площадь
                HtmlNode livingSpaceNode = dtsSpace.Where(x => x.InnerText != null).SingleOrDefault(x => x.InnerHtml.Contains("Жилая"));
                if (livingSpaceNode != null)
                {
                    livingSpaceNode = livingSpaceNode.ParentNode.ChildNodes.SingleOrDefault(x => x.Name == "dd");

                    String livingSpaceStr = livingSpaceNode.InnerText;
                    int mIndex = livingSpaceStr.IndexOf(" м");
                    if (mIndex > 0)
                        livingSpaceStr = livingSpaceStr.Remove(mIndex);
                    livingSpaceStr = ClearStringFromSlashTSlashNQuotNBsp(livingSpaceStr);
                    Double livingSpaceDbl;
                    if (Double.TryParse(livingSpaceStr, NumberStyles.AllowDecimalPoint, culture, out livingSpaceDbl))
                    {
                        Int32 livingSpaceInt = (Int32)livingSpaceDbl;
                        webPub.AdditionalInfo.RealtyAdditionalInfo.LivingSpace = livingSpaceInt;
                    }
                }

                // площадь кухни
                HtmlNode kitchenSpaceNode = dtsSpace.Where(x => x.InnerText != null).SingleOrDefault(x => x.InnerHtml.Contains("Кухня"));
                if (kitchenSpaceNode != null)
                {
                    kitchenSpaceNode = kitchenSpaceNode.ParentNode.ChildNodes.SingleOrDefault(x => x.Name == "dd");

                    String kitchenSpaceStr = kitchenSpaceNode.InnerText;
                    int mIndex = kitchenSpaceStr.IndexOf(" м");
                    if (mIndex > 0)
                        kitchenSpaceStr = kitchenSpaceStr.Remove(mIndex);
                    kitchenSpaceStr = ClearStringFromSlashTSlashNQuotNBsp(kitchenSpaceStr);
                    Double kitchenSpaceDbl;
                    if (Double.TryParse(kitchenSpaceStr, NumberStyles.AllowDecimalPoint, culture, out kitchenSpaceDbl))
                    {
                        Int32 kitchenSpaceInt = (Int32)kitchenSpaceDbl;
                        webPub.AdditionalInfo.RealtyAdditionalInfo.KitchenSpace = kitchenSpaceInt;
                    }
                }

                // площадь земли/участка
                HtmlNode landSpaceNode = dtsSpace.Where(x => x.InnerText != null).SingleOrDefault(x => x.InnerHtml.Contains("участка"));
                if (landSpaceNode != null)
                {
                    landSpaceNode = landSpaceNode.ParentNode.ChildNodes.SingleOrDefault(x => x.Name == "dd");

                    String landSpaceStr = landSpaceNode.InnerText;
                    int mIndex = landSpaceStr.IndexOf(" м");
                    if (mIndex > 0)
                        landSpaceStr = landSpaceStr.Remove(mIndex);
                    Double landSpaceDbl;
                    if (Double.TryParse(landSpaceStr, NumberStyles.AllowDecimalPoint, culture, out landSpaceDbl))
                    {
                        Int32 landSpaceInt = (Int32)landSpaceDbl;
                        webPub.AdditionalInfo.RealtyAdditionalInfo.KitchenSpace = landSpaceInt;
                    }
                }
            }
                        
            // район
            if( districtFromAddress!=null )
            {
                webPub.AdditionalInfo.RealtyAdditionalInfo.District = districtFromAddress;
            }

            // форма собственности
            HtmlNode viewFromPropertyNode = dts.SingleOrDefault(x => x.InnerHtml != null && x.InnerHtml.Contains("Форма собственности"));
            if (viewFromPropertyNode != null)
            {
                viewFromPropertyNode = viewFromPropertyNode.ParentNode.ParentNode.ChildNodes.SingleOrDefault(x => x.Name == "dd");

                String viewFromPropertyStr = viewFromPropertyNode.InnerText;
                webPub.AdditionalInfo.RealtyAdditionalInfo.ViewFromProperty = viewFromPropertyStr;
            }

            // материал стен
            HtmlNode wallМaterialNode = dts.SingleOrDefault(x => x.InnerHtml != null && x.InnerHtml.Contains("Материал дома"));
            if (wallМaterialNode != null)
            {
                wallМaterialNode = wallМaterialNode.ParentNode.ParentNode.ChildNodes.SingleOrDefault(x => x.Name == "dd");
                String wallМaterialStr = ClearStringFromSlashTSlashNQuotNBsp(wallМaterialNode.InnerText);
                webPub.AdditionalInfo.RealtyAdditionalInfo.WallМaterial = wallМaterialStr;
            }

            // Санузел
            HtmlNode wcNode = dts.SingleOrDefault(x => x.InnerHtml != null && x.InnerHtml.Contains("Санузел"));
            if (wcNode != null)
            {
                wcNode = wcNode.ParentNode.ParentNode.ChildNodes.SingleOrDefault(x => x.Name == "dd");

                String wcStr = wcNode.InnerText;
                webPub.AdditionalInfo.RealtyAdditionalInfo.Wc = wcStr;
            }

            // наличие балкона/лоджии
            HtmlNode balconyNode = dts.SingleOrDefault(x => x.InnerHtml != null && x.InnerHtml.Contains("Балкон"));
            HtmlNode loggiaNode = dts.SingleOrDefault(x => x.InnerHtml != null && x.InnerHtml.Contains("Лоджия"));
            if ( balconyNode!=null || loggiaNode != null )
            {
                webPub.AdditionalInfo.RealtyAdditionalInfo.IsLoggia = true;
            }
            else
            {
                webPub.AdditionalInfo.RealtyAdditionalInfo.IsLoggia = false;
            }

            // наличие гаража
            HtmlNode parkingNode = dts.SingleOrDefault(x => x.InnerHtml != null && x.InnerHtml.Contains("Подземный гараж"));
            if (parkingNode != null)
            {
                webPub.AdditionalInfo.RealtyAdditionalInfo.IsParking = true;
            }
            else
            {
                webPub.AdditionalInfo.RealtyAdditionalInfo.IsParking = false;
            }

            // арендуемая площадь и иногда площадь общая
            if (h3Nodes.Count >= 1)
            {
                HtmlNode leasableSpaceHeaderNode = h3Nodes.SingleOrDefault(x => x.InnerHtml == "Площадь");
                if (leasableSpaceHeaderNode != null)
                {
                    HtmlNode leasableSpaceNode = leasableSpaceHeaderNode.ParentNode.SelectSingleNode("//dd");
                    if (leasableSpaceNode != null)
                    {
                        String leasableSpaceStr = leasableSpaceNode.InnerText;
                        int meterIndex = leasableSpaceStr.IndexOf(" м");
                        if( meterIndex>0 )
                            leasableSpaceStr = leasableSpaceStr.Remove(meterIndex);
                        leasableSpaceStr = ClearStringFromSlashTSlashNQuotNBsp(leasableSpaceStr);
                        Double leasableSpaceDbl;
                        if (Double.TryParse(leasableSpaceStr, NumberStyles.AllowDecimalPoint, culture, out leasableSpaceDbl))
                        {
                            Int32 leasableSpaceInt = (Int32)leasableSpaceDbl;

                            if (currBind.ActionId == 3)
                            {
                                webPub.AdditionalInfo.RealtyAdditionalInfo.LeasableSpace = leasableSpaceInt;
                            }
                            else
                            {
                                webPub.AdditionalInfo.RealtyAdditionalInfo.TotalSpace = leasableSpaceInt;
                            }
                        }
                    }
                }
            }

            // тип недвижимости
            HtmlNode realEstateTypeNode = docNode.SelectSingleNode(@"//h2[@class='card__header']");
            String realEstateTypeStr = realEstateTypeNode.InnerHtml;
            webPub.AdditionalInfo.RealtyAdditionalInfo.RealEstateType = realEstateTypeStr;

            // количество комнат
            if (realEstateTypeStr.Contains("1"))
            {
                webPub.AdditionalInfo.RealtyAdditionalInfo.RoomNumber = 1;
            }
            else
            {
                if (realEstateTypeStr.Contains("2")) 
                {
                    webPub.AdditionalInfo.RealtyAdditionalInfo.RoomNumber = 2;
                }
                else
                {
                    if (realEstateTypeStr.Contains("3"))
                    {
                        webPub.AdditionalInfo.RealtyAdditionalInfo.RoomNumber = 3;
                    }
                    else
                    {
                        if (realEstateTypeStr.Contains("4"))
                        {
                            webPub.AdditionalInfo.RealtyAdditionalInfo.RoomNumber = 4;
                        }
                        else
                        {
                            if (realEstateTypeStr.Contains("5"))
                            {
                                webPub.AdditionalInfo.RealtyAdditionalInfo.RoomNumber = 5;
                            }
                            else
                            {
                                if (realEstateTypeStr.Contains("6")) webPub.AdditionalInfo.RealtyAdditionalInfo.RoomNumber = 6;
                            }
                        }
                    }
                }
            }

            //public string Furnish { get; set; } отделка мне не попалась в результатах            
            //public int Tenancy { get; set; } срок аренды не попался 
            //public string AppointmentOfRoom { get; set; } не понятно что писать сюда

            // теперь используя HtmlAgilityPack сохраняем контакты разместившего ---------------------------------------------------------------------------------------------------------------------------
            webPub.Contact = new WebPublicationContact();
            HtmlNode contactsNode = h3Nodes.SingleOrDefault(x => x.InnerHtml == "Контакты");
            HtmlNodeCollection contactsNodeDts = contactsNode.SelectNodes("//dt");
            
            if( contactsNode!=null )
            {
                // контактное лицо два варианта
                HtmlNode contactNameNode = contactsNode.ParentNode.SelectNodes("//span").SingleOrDefault(x => x.Attributes["class"]!=null && x.Attributes["class"].Value.Contains("card__author-name") );
                if (contactNameNode == null)
                {
                    contactNameNode = contactsNode.ParentNode.SelectNodes("//a").Single(x => x.Attributes["class"] != null && x.Attributes["class"].Value.Contains("card__author-name"));
                }
                if( contactNameNode!=null )
                {
                    String contactNameStr = contactNameNode.InnerText;
                    contactNameStr = ClearStringFromSlashTSlashNQuotNBsp(contactNameStr);
                    webPub.Contact.ContactName = contactNameStr;
                }

                // юр. лицо разместившего два варианта
                HtmlNodeCollection contactAuthorNodesColl = contactsNode.ParentNode.SelectNodes("//strong");
                HtmlNode contactAuthorNode = contactAuthorNodesColl.SingleOrDefault(x => x.Attributes["class"] != null && x.Attributes["class"].Value == "organization-informer__title");
                if (contactAuthorNode != null)
                {
                    HtmlNode contactAuthorANode = contactAuthorNode.ChildNodes.SingleOrDefault( x => x.Name=="a");
                    if( contactAuthorANode!=null )
                    {
                        contactAuthorNode = contactAuthorANode;
                    }
                    String contactAuthorStr = contactAuthorNode.InnerText;
                    contactAuthorStr = ClearStringFromSlashTSlashNQuotNBsp(contactAuthorStr);
                    webPub.Contact.Author = contactAuthorStr;
                }

                if (contactsNodeDts.Count >= 1)
                {
                    // URL юр. лицо разместившего
                    HtmlNode authorUrlNode = contactsNodeDts.SingleOrDefault(x => x.InnerHtml != null && x.InnerHtml.Contains("Сайт:"));
                    authorUrlNode = GetNextDDNodeInContacts(authorUrlNode);
                    if (authorUrlNode != null)
                    {
                        authorUrlNode = authorUrlNode.ChildNodes.SingleOrDefault(x => x.Name == "a");

                        String authorUrlStr = authorUrlNode.Attributes["href"].Value;

                        try // перестраховка
                        {
                            Uri authorUri = new Uri(authorUrlStr);
                            webPub.Contact.AuthorUrl = authorUri;
                        }
                        catch
                        {
                            ; // если uri не создается по битой ссылке, то не обрабатываем его
                        }
                    }
//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! ТУТ ЗАСАДА email спрятан в javascript-e
//                <script type="text/javascript" language="javascript">
//<!--
//{document.write(String.fromCharCode(60,97,32,104,114,101,102,61,34,109,97,105,108,116,111,58,110,111,118,111,115,101,108,45,97,107,97,100,101,109,64,109,97,105,108,46,114,117,34,32,116,105,116,108,101,61,34,34,99,108,97,115,115,61,34,114,101,45,108,105,110,107,34,62,110,111,118,111,115,101,108,45,97,107,97,100,101,109,64,109,97,105,108,46,114,117,60,47,97,62))}
////-->
//</script>
//                <noscript>Включите javascript, чтобы увидеть e-mail.</noscript>


                    // Email
                    webPub.Contact.Email = new List<String>();
                    HtmlNode emailDtNode = contactsNodeDts.SingleOrDefault(x => x.InnerHtml != null && x.InnerHtml.Contains("e-mail:"));
                    HtmlNode emailDdNode = GetNextDDNodeInContacts(emailDtNode);
                    if (emailDdNode != null)
                    {
                        List<HtmlNode> emailANodeColl = emailDdNode.ChildNodes.Where(x => x.Name == "a").ToList();
                        foreach (HtmlNode currEmailANode in emailANodeColl)
                        {
                            String emailStr = currEmailANode.InnerText;
                            webPub.Contact.Email.Add(emailStr);
                        }
                    }

                    // Icq не попадались
                    // Skype не попадались

                    // Phone
                    HtmlNodeCollection phoneNodes = contactsNode.SelectNodes("//li[@class='card__phones-list-item']");
                    webPub.Contact.Phone = new List<String>();
                    foreach (HtmlNode currPhoneNode in phoneNodes)
                    {
                        String phoneStr = currPhoneNode.InnerText;
                        phoneStr = ClearStringFromSlashTSlashNQuotNBsp(phoneStr);
                        webPub.Contact.Phone.Add(phoneStr);
                    }
                }
            }

            // теперь используя HtmlAgilityPack находим нужные параметры на странице и заполняем WebPublication ---------------------------------------------------------------------------------------

            // описание
            HtmlNode descriptionDivNode = docNode.SelectSingleNode("//div[@class='card__comments-section']");
            if (descriptionDivNode != null)
            {
                HtmlNode descriptionNode = descriptionDivNode.ChildNodes.FirstOrDefault(x => x.Name == "p");
                if (descriptionNode != null)
                {
                    String descriptionStr = descriptionNode.InnerText;
                    descriptionStr = ClearStringFromSlashTSlashNQuotNBsp(descriptionStr);
                    webPub.Description = descriptionStr;
                }
            }

            // дата последней модификации
            HtmlNodeCollection modifDateNodes = docNode.SelectNodes(@"//span[@class='card__publication-date']");
            if (modifDateNodes.Count >= 1)
            {
                String modifDateStr = modifDateNodes[0].InnerText;

                DateTime modifDate;
                if (DateTime.TryParse(modifDateStr, out modifDate))
                {
                    webPub.ModifyDate = modifDate;
                }
            }
            
            // теперь используя HtmlAgilityPack сохраняем ссылки на изображения -----------------------------------------------------------------------------------------------------------------------
            HtmlNodeCollection imageANodes = docNode.SelectNodes(@"//a[@class='card__photo-link']");
            if (imageANodes != null)
            {
                if (imageANodes.Count >= 1)
                {
                    webPub.Photos = new List<Uri>();
                    foreach (HtmlNode currImageNode in imageANodes)
                    {
                        String imageANodesHrefStr = currImageNode.Attributes["href"].Value;

                        try // перестраховка
                        {
                            Uri currImageHref = new Uri(imageANodesHrefStr);
                            webPub.Photos.Add(currImageHref);
                        }
                        catch
                        {
                            ; // если uri не создается по битой ссылке, то не обрабатываем его
                        }
                    }
                }
            }

            return webPub;
        }
    }
}


