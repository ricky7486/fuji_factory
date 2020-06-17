using aejw.Network;
using PrinterCenter.UI.CommonSetting;
using PrinterCenter.UI.SharedFolderSetting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace PrinterCenter.File
{
    public static class XElementExtensions
    {
        static public string getValue(this XElement root, string name)
        {
            try
            {
                string value =
                    (string)(from el in root.Descendants(name)
                             select el).First().Value;

                return value;
            }
            catch
            {
                return null;
            }


        }
  
        static public string getAttributeValue(this XElement root, string element, string attribute)
        {
            try
            {
                string value =
                    (string)(from el in root.Descendants(element)
                             select el).First().Attribute(attribute).Value;

                return value;
            }
            catch
            {
                return null;
            }
        }
        static public string getAttributeValue(this XElement target, string attribute)
        {
            try
            {
                string value =
                    target.Attribute(attribute).Value;

                return value;
            }
            catch
            {
                return null;
            }
        }
        static public List<XElement> getElements(this XElement root, string element)
        {
            List<XElement> ret = new List<XElement>();
            try
            {
                ret =
                        (from el in root.Descendants(element)
                           select el).ToList();
                return ret;
            }
            catch
            {
                return null;
            }
        }

        static public XElement getElement(this XElement root, string element)
        {
            try
            {
                return (from el in root.Descendants(element)
                        select el).FirstOrDefault();

            }
            catch
            {
                return null;
            }
        }

        static public CommonSettingVM ToCommonSettingVM(this XElement root)
        {
            CommonSettingVM ret = null;
            try
            {
                ret = new CommonSettingVM();

                eMatchingBasis basis;
                Enum.TryParse(root.getValue("MatchingBasis"), true, out basis);// #改Enum
                ret.MatchingBasis = basis;

                ret.IsMoveStencil = bool.Parse(root.getAttributeValue("Adjustment", "IsMoveStencil"));
                ret.IsMovePCB = bool.Parse(root.getAttributeValue("Adjustment", "IsMovePCB"));

                ret.IsCWRotate = bool.Parse(root.getAttributeValue("Rotate", "IsCWRotate"));
                ret.IsCCWRotate = bool.Parse(root.getAttributeValue("Rotate", "IsCCWRotate"));


                ret.IsQuadrent1 = bool.Parse(root.getAttributeValue("Quadrent", "IsQuadrent1"));
                ret.IsQuadrent2 = bool.Parse(root.getAttributeValue("Quadrent", "IsQuadrent2"));
                ret.IsQuadrent3 = bool.Parse(root.getAttributeValue("Quadrent", "IsQuadrent3"));
                ret.IsQuadrent4 = bool.Parse(root.getAttributeValue("Quadrent", "IsQuadrent4"));

                var FilterElement = root.getElement("Filter");
                ret.IsHeightFilter = bool.Parse(FilterElement.getValue("Height"));
                ret.HeightRange.LowerBound = double.Parse(FilterElement.getAttributeValue("Height", "LowerBound"));
                ret.HeightRange.UpperBound = double.Parse(FilterElement.getAttributeValue("Height", "UpperBound"));
                ret.IsAreaFilter = bool.Parse(FilterElement.getValue("Area"));
                ret.AreaRange.LowerBound = double.Parse(FilterElement.getAttributeValue("Area", "LowerBound"));
                ret.AreaRange.UpperBound = double.Parse(FilterElement.getAttributeValue("Area", "UpperBound"));
                ret.IsVolumeFilter = bool.Parse(FilterElement.getValue("Volume"));
                ret.VolumeRange.LowerBound = double.Parse(FilterElement.getAttributeValue("Volume", "LowerBound"));
                ret.VolumeRange.UpperBound = double.Parse(FilterElement.getAttributeValue("Volume", "UpperBound"));
            }
            catch
            {
                return null;
            }
            return ret;
        }

        static public SharedFolderSettingVM ToSharedFolderSettingVM(this XElement root)
        {
            SharedFolderSettingVM ret = null;
            try
            {
                ret = new SharedFolderSettingVM();
                ret.IsInEnable = bool.Parse(root.getValue("In"));
                string _driveletter = root.getAttributeValue("In","DriveLetter");
                string _driveprovider = root.getAttributeValue("In", "DriveProvider");

                ret.InDriveInfo = _driveletter + "(" + _driveprovider + ")";
                //檢查有否有此driveinfo，若沒有，則create 一個
                var _lstCurrentDrives = WmiDiskHelper.GetDiskNames();
                bool bExist = _lstCurrentDrives.Exists(X => X.DiskID == _driveletter && X.DiskProviderName == _driveprovider);
                if (!bExist)
                {
                    NetworkDriveWrapper.MappingNetDrive(_driveletter, _driveprovider, "", "");

                }
                ret.IsOutEnable = bool.Parse(root.getValue("Out"));
                _driveletter = root.getAttributeValue("Out", "DriveLetter");
                _driveprovider = root.getAttributeValue("Out", "DriveProvider");

                ret.OutDriveInfo = _driveletter + "(" + _driveprovider + ")";
                //檢查有否有此driveinfo，若沒有，則create 一個
                _lstCurrentDrives = WmiDiskHelper.GetDiskNames();
                bExist = _lstCurrentDrives.Exists(X => X.DiskID == _driveletter && X.DiskProviderName == _driveprovider);
                if (!bExist)
                {
                    NetworkDriveWrapper.MappingNetDrive(_driveletter, _driveprovider, "", "");

                }

            }
            catch
            {
                return null;
            }
            return ret;
        }
    }
}
