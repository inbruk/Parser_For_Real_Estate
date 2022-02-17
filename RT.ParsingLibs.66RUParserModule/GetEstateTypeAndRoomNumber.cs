using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RT.ParsingLibs.WWW66RUParser
{
    internal static class GetEstateTypeAndRoomNumber
    {
        public static Boolean Convert(String captionString, out string realEstateType, out int roomNumber)
        {
            realEstateType = String.Empty;
            roomNumber = 0;

            if (captionString.Contains("1-к. квартиру"))
            {
                roomNumber = 1;
                realEstateType = "1-к. квартира";
                return true;
            }
            else
            {
                if (captionString.Contains("2-к. квартиру"))
                {
                    roomNumber = 2;
                    realEstateType = "2-к. квартира";
                    return true;
                }
                else
                {
                    if (captionString.Contains("3-к. квартиру"))
                    {
                        roomNumber = 3;
                        realEstateType = "3-к. квартира";
                        return true;
                    }
                    else
                    {
                        if (captionString.Contains("4-к. квартиру"))
                        {
                            roomNumber = 4;
                            realEstateType = "4-к. квартира";
                            return true;
                        }
                        else
                        {
                            if (captionString.Contains("5-к. квартиру"))
                            {
                                roomNumber = 5;
                                realEstateType = "5-к. квартира";
                                return true;
                            }
                            else
                            {
                                if (captionString.Contains("6-к. квартиру"))
                                {
                                    roomNumber = 6;
                                    realEstateType = "6-к. квартира";
                                    return true;
                                }
                                else
                                {
                                    if (captionString.Contains("комнату"))
                                    {
                                        realEstateType = "комната";
                                        return true;
                                    }
                                    else
                                    {
                                        if (captionString.Contains("гараж"))
                                        {
                                            realEstateType = "гараж";
                                            return true;
                                        }
                                        else
                                        {
                                            if (captionString.Contains("дом"))
                                            {
                                                realEstateType = "дом на участке";
                                                return true;
                                            }
                                            else
                                            {
                                                if (captionString.Contains("участок"))
                                                {
                                                    realEstateType = "участок";
                                                    return true;
                                                }
                                                else
                                                {
                                                    if (captionString.Contains("производство"))
                                                    {
                                                        realEstateType = "производственно складское помещение";
                                                        return true;
                                                    }
                                                    else
                                                    {
                                                        if (captionString.Contains("офис"))
                                                        {
                                                            realEstateType = "офис";
                                                            return true;
                                                        }
                                                        else
                                                        {
                                                            if (captionString.Contains("магазин"))
                                                            {
                                                                realEstateType = "торговое помещение";
                                                                return true;
                                                            }
                                                            else
                                                            {
                                                                if (captionString.Contains("склад"))
                                                                {
                                                                    realEstateType = "производственно складское помещение";
                                                                    return true;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return false;
        }
    }
}
