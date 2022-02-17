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

namespace RT.ParsingLibs.WWW66RUParser
{
    internal class OldRealtyAdvertismentPageParser
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
            if (dtNodeParentChildrenColl.Count > position && dtNodeParentChildrenColl[position].Name == "dd")
            {
                resultNode = dtNodeParentChildrenColl[position];
            }
            else
            {
                position++;
                if (dtNodeParentChildrenColl.Count > position && dtNodeParentChildrenColl[position].Name == "dd")
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

            HtmlDocument htmlDoc = CrawlerHelper.GetHtmlDocumentByUrl(currUrl);
            HtmlNode docNode = htmlDoc.DocumentNode;

            List<HtmlNode> dtListFromHorizontalTable = new List<HtmlNode>(); // пустой список если нет таких
            HtmlNodeCollection dtListFromHorizontalTableColl = docNode.SelectNodes(@"//dt[@class='b-properties-table__label']");
            if (dtListFromHorizontalTableColl!=null && dtListFromHorizontalTableColl.Count>=1 )
            {
                dtListFromHorizontalTable = dtListFromHorizontalTableColl.ToList();
            }

            // теперь используя HtmlAgilityPack находим нужные параметры на странице и заполняем собственно WebPublication ---------------------------------------------------------------------------------
            String description = String.Empty;
            HtmlNodeCollection divDescs = docNode.SelectNodes(@"//div[@class='b-doska-ncard__text']");
            if (divDescs != null)
            {
                foreach(HtmlNode currDivDesc in divDescs)
                {
                    if (currDivDesc != null && currDivDesc.ChildNodes.Count >= 1)
                    {
                        List<HtmlNode> pDescs = currDivDesc.ChildNodes.Where(x => x.Name == "p" || x.Name == "#text").ToList();
                        foreach (HtmlNode currPDesc in pDescs)
                        {
                            if (currPDesc != null)
                            {
                                description += currPDesc.InnerText;
                            }
                            else
                            {
                                description += currDivDesc.InnerText;
                            }
                        }
                    }
                }
                if ( String.IsNullOrEmpty(description)==false )
                {
                    description = ClearStringFromSlashTSlashNQuotNBsp(description);
                    webPub.Description = description;
                }
            }

            HtmlNode ddModifyDate = docNode.SelectSingleNode(@"//dd[@class='b-card-info__desc']");
            if( ddModifyDate !=null )
            {
                String dateTimeStr = ddModifyDate.InnerText;
                DateTime modifyDate;
                if( DateTime.TryParse(dateTimeStr, out modifyDate) )
                {
                    webPub.ModifyDate = modifyDate;
                }
            }

            // теперь используя HtmlAgilityPack находим нужные параметры на странице и заполняем WebPublication.AdditionalInfo.RealtyAdditionalInfo --------------------------------------------------------

            webPub.AdditionalInfo = new AdditionalInfo()
            {
                RealtyAdditionalInfo = new RealtyAdditionalInfo()
            };            

            HtmlNode h2Address = docNode.SelectSingleNode(@"//h2[@class='b-doska-ncard__heading']");
            if( h2Address!=null )
            {
                String h2AddressStr = h2Address.InnerText;
                String[] h2AddressStrArray = h2AddressStr.Split
                (
                    new String[] 
                    { 
                        "производство,", "квартиру,", "комнату,", "квартире,", "офис,", "дом,", "доме,", "гараж,", "участок,", "магазин,", "склад,"
                    }, StringSplitOptions.RemoveEmptyEntries
                );

                if (h2AddressStrArray.Count() > 0)
                {
                    h2AddressStr = h2AddressStrArray[h2AddressStrArray.Count() - 1];
                    Int32 idx = h2AddressStr.IndexOf('(');
                    if (idx >= 0)
                    {
                        h2AddressStr = h2AddressStr.Remove(idx);
                        webPub.AdditionalInfo.RealtyAdditionalInfo.Address = h2AddressStr;
                    }
                }
            }

            HtmlNode divCostAll = docNode.SelectSingleNode(@"//div[@class='b-price-tag b-price-tag_layout_realty']");
            if( divCostAll!=null )
            {
                String costAllStr = divCostAll.InnerText;
                Int32 idx = costAllStr.IndexOf('р');
                costAllStr = costAllStr.Remove(idx);
                costAllStr = costAllStr.Replace(" ", "");
                costAllStr = ClearStringFromSlashTSlashNQuotNBsp(costAllStr);
                Decimal costAllDec;
                if( Decimal.TryParse(costAllStr, out costAllDec) )
                {
                    webPub.AdditionalInfo.RealtyAdditionalInfo.CostAll = costAllDec;
                }
            }

            HtmlNode pCostPerMeter = docNode.SelectSingleNode(@"//p[@class='b-price-tag__details']");
            if( pCostPerMeter!=null )
            {
                String costPerMeter = pCostPerMeter.InnerText;
                Int32 idx = costPerMeter.IndexOf('р');
                if (idx >= 0)
                {
                    costPerMeter = costPerMeter.Remove(idx);
                }
                costPerMeter = costPerMeter.Replace(" ", "");
                costPerMeter = ClearStringFromSlashTSlashNQuotNBsp(costPerMeter);
                Decimal costPerMeterDec;
                if( Decimal.TryParse(costPerMeter, out costPerMeterDec) )
                {
                    webPub.AdditionalInfo.RealtyAdditionalInfo.CostPerMeter = costPerMeterDec;
                }
            }

            if( h2Address!=null )
            {
                String districtStr = h2Address.InnerText;
                if (districtStr.Contains('('))
                {
                    Int32 idx = districtStr.IndexOf('(') + 1;
                    districtStr = districtStr.Remove(0, idx);
                }

                if (districtStr.Contains(')'))
                {
                    Int32 idx = districtStr.IndexOf(')');
                    districtStr = districtStr.Remove(idx);
                }

                webPub.AdditionalInfo.RealtyAdditionalInfo.District = districtStr;
            }

            HtmlNode floorDtNode = dtListFromHorizontalTable.SingleOrDefault(x => x.InnerText == "Этаж");
            if( floorDtNode!=null )
            {
                HtmlNode floorDdNode = GetNextDDNodeInContacts(floorDtNode);
                if (floorDdNode != null)
                {
                    String floorDdStr = floorDdNode.InnerText;
                    if (String.IsNullOrWhiteSpace(floorDdStr))
                    {
                        String[] florrStrArr = floorDdStr.Split(new String[] { " " }, StringSplitOptions.None);
                        
                        if( florrStrArr.Count()>=1)
                        {
                            String currFloorStr = florrStrArr[0];
                            int currFloor;
                            if (int.TryParse(currFloorStr, out currFloor))
                            {
                                webPub.AdditionalInfo.RealtyAdditionalInfo.Floor = currFloor;
                            }
                        }

                        if (florrStrArr.Count() >= 3)
                        {
                            String currFloorNumberStr = florrStrArr[2];
                            int currFloorNumber;
                            if (int.TryParse(currFloorNumberStr, out currFloorNumber))
                            {
                                webPub.AdditionalInfo.RealtyAdditionalInfo.FloorNumber = currFloorNumber;
                            }
                        }
                    }
                }
            }

            HtmlNode totalSpaceDtNode = dtListFromHorizontalTable.SingleOrDefault(x => x.InnerText == "Общая площадь" || x.InnerText == "Площадь");
            if (totalSpaceDtNode != null)
            {
                HtmlNode totalSpaceDdNode = GetNextDDNodeInContacts(totalSpaceDtNode);
                String totalSpaceDdStr = totalSpaceDdNode.InnerText;
                String currTotalSpaceStr = totalSpaceDdStr.Split(new String[] { " " }, StringSplitOptions.None)[0];

                int totalSpace;
                if (int.TryParse(currTotalSpaceStr, out totalSpace))
                {
                    webPub.AdditionalInfo.RealtyAdditionalInfo.TotalSpace = totalSpace;
                }
            }

            HtmlNode livingSpaceDtNode = dtListFromHorizontalTable.SingleOrDefault(x => x.InnerText == "Жилая" || x.InnerText == "Комнаты" );
            if (livingSpaceDtNode != null)
            {
                HtmlNode livingSpaceDdNode = GetNextDDNodeInContacts(livingSpaceDtNode);
                String livingSpaceDdStr = livingSpaceDdNode.InnerText;
                String currLivingSpaceStr = livingSpaceDdStr.Split(new String[] { " " }, StringSplitOptions.None)[0];

                int livingSpace;
                if (int.TryParse(currLivingSpaceStr, out livingSpace))
                {
                    webPub.AdditionalInfo.RealtyAdditionalInfo.LivingSpace = livingSpace;
                }
            }

            HtmlNode kitchenSpaceDtNode = dtListFromHorizontalTable.SingleOrDefault(x => x.InnerText == "Кухня");
            if (kitchenSpaceDtNode != null)
            {
                HtmlNode kitchenSpaceDdNode = GetNextDDNodeInContacts(kitchenSpaceDtNode);
                String kitchenSpaceDdStr = kitchenSpaceDdNode.InnerText;
                String currKitchenSpaceStr = kitchenSpaceDdStr.Split(new String[] { " " }, StringSplitOptions.None)[0];

                int kitchenSpace;
                if (int.TryParse(currKitchenSpaceStr, out kitchenSpace))
                {
                    webPub.AdditionalInfo.RealtyAdditionalInfo.KitchenSpace = kitchenSpace;
                }
            }

            // там сотки и точно не распарсить, раскоментарить если надо
            //HtmlNode landSpaceDtNode = dtListFromHorizontalTable.SingleOrDefault(x => x.InnerText == "Площадь участка");
            //if (landSpaceDtNode != null)
            //{
            //    HtmlNode landSpaceDdNode = GetNextDDNodeInContacts(landSpaceDtNode);
            //    String landSpaceDdStr = landSpaceDdNode.InnerText;
            //    String currLandSpaceStr = landSpaceDdStr.Split(new String[] { " " }, StringSplitOptions.None)[0];

            //    int landSpace;
            //    if (int.TryParse(currLandSpaceStr, out landSpace))
            //    {
            //        webPub.AdditionalInfo.RealtyAdditionalInfo.LandSpace = landSpace;
            //    }
            //}

            if (h2Address != null)
            {
                String realEstateType;
                int roomNumber;

                String captionString = h2Address.InnerText;
                if (GetEstateTypeAndRoomNumber.Convert(captionString, out realEstateType, out roomNumber))
                {
                    webPub.AdditionalInfo.RealtyAdditionalInfo.RealEstateType = realEstateType;
                    webPub.AdditionalInfo.RealtyAdditionalInfo.RoomNumber = roomNumber;
                }
            }
            
            // остальное не было найдено на этом сайте

            //webPub.AdditionalInfo.RealtyAdditionalInfo.IsLoggia
            //webPub.AdditionalInfo.RealtyAdditionalInfo.IsParking
            //webPub.AdditionalInfo.RealtyAdditionalInfo.LeasableSpace
            //webPub.AdditionalInfo.RealtyAdditionalInfo.ViewFromProperty
            //webPub.AdditionalInfo.RealtyAdditionalInfo.WallМaterial
            //webPub.AdditionalInfo.RealtyAdditionalInfo.Wc
          
            //public string Furnish { get; set; } отделка мне не попалась в результатах            
            //public int Tenancy { get; set; } срок аренды не попался 
            //public string AppointmentOfRoom { get; set; } не понятно что писать сюда

            // теперь используя HtmlAgilityPack сохраняем контакты разместившего ---------------------------------------------------------------------------------------------------------------------------
            
            webPub.Contact = new WebPublicationContact();

            HtmlNode spanName = docNode.SelectSingleNode(@"//span[@class='b-card-info__desc']");
            if( spanName!=null )
            {
                String name = spanName.InnerText;
                name = ClearStringFromSlashTSlashNQuotNBsp(name);
                webPub.Contact.ContactName = name;
            }
            
            HtmlNode pPhone = docNode.SelectSingleNode(@"//p[@class='b-card-phone__phone']");
            if( pPhone!=null )
            {
                HtmlAttribute attrPhoneData = pPhone.Attributes["data-phone"];
                if( attrPhoneData!=null )
                {
                    String phone = attrPhoneData.Value;
                    phone = ClearStringFromSlashTSlashNQuotNBsp(phone);
                    webPub.Contact.Phone = new List<String>() { phone };
                }
            }

            // остальное не было найдено на этом сайте

            //webPub.Contact.Author = 
            //webPub.Contact.AuthorUrl            
            //webPub.Contact.Email
            //webPub.Contact.Icq            
            //webPub.Contact.Skype


            // теперь используя HtmlAgilityPack сохраняем ссылки на изображения -----------------------------------------------------------------------------------------------------------------------
            webPub.Photos = new List<Uri>();
            HtmlNodeCollection imgNodeColl = docNode.SelectNodes(@"//img[@class='b-showcase__img lazyload_img' or @class='b-showcase__img']");
            if( imgNodeColl!=null )
            {
                List<HtmlNode> imgNodes = imgNodeColl.ToList();
                if( imgNodes.Count>=1 )
                {
                    foreach(HtmlNode currImgNode in imgNodes)
                    {
                        HtmlAttribute srcAttr = currImgNode.Attributes["data-original"]; // во всех случаях кроме первого
                        if( srcAttr==null )
                        {
                            srcAttr = currImgNode.Attributes["src"];
                        }
                        
                        if( srcAttr!=null )
                        {
                            String currImgSrcStr = srcAttr.Value;
                            try
                            {
                                Uri currImgUri = new Uri(currImgSrcStr);
                                webPub.Photos.Add(currImgUri);
                            }
                            catch
                            {
                                ; // просто обходим (игнорируем) некорректные ссылки
                            }
                        }
                    }
                }
            }


            return webPub;
        }
    }
}
