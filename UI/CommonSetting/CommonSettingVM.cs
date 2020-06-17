using GalaSoft.MvvmLight;
using PrinterCenter.Log;
using System;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Linq;


namespace PrinterCenter.UI.CommonSetting
{

    public enum eMatchingBasis
    {
        Sequence,
        Barcode
    }
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>

    public class Range
    {
        public Range(double l, double u)
        {
            LowerBound = l;
            UpperBound = u;
        }
        public double LowerBound { get; set; }
        public double UpperBound { get; set; }
    }

 
    public class CommonSettingVM : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the CommonSettingVM class.
        /// </summary>
        public CommonSettingVM()
        {
            _IsMoveStencil = true;
            _IsMovePCB = false;
            _IsQuadrent1 = true;
            _IsQuadrent2 = false;
            _IsQuadrent3 = false;
            _IsQuadrent4 = false;
            _IsCWRotate = false;
            _IsCCWRotate = true;
            _HeightRange = new Range(0.0, 0.0);
            _AreaRange = new Range(0.0, 0.0);
            _VolumeRange = new Range(0.0, 0.0);
            _MatchingBasis = eMatchingBasis.Sequence;
            _IsAreaFilter = _IsHeightFilter = _IsVolumeFilter = false;
        }
        private bool _IsMoveStencil;
        public bool IsMoveStencil
        {
            get { return _IsMoveStencil; }
            set { Set(() => IsMoveStencil, ref _IsMoveStencil, value); }
        }
        private bool _IsMovePCB;
        public bool IsMovePCB
        {
            get { return _IsMovePCB; }
            set { Set(() => IsMovePCB, ref _IsMovePCB, value); }
        }

        private bool _IsCWRotate;
        public bool IsCWRotate
        {
            get { return _IsCWRotate; }
            set { Set(() => IsCWRotate, ref _IsCWRotate, value); }
        }

        private bool _IsCCWRotate;
        public bool IsCCWRotate
        {
            get { return _IsCCWRotate; }
            set { Set(() => IsCCWRotate, ref _IsCCWRotate, value); }
        }

        private bool _IsQuadrent1;
        public bool IsQuadrent1
        {
            get { return _IsQuadrent1; }
            set { Set(() => IsQuadrent1, ref _IsQuadrent1, value); }
        }

        private bool _IsQuadrent2;
        public bool IsQuadrent2
        {
            get { return _IsQuadrent2; }
            set { Set(() => IsQuadrent2, ref _IsQuadrent2, value); }
        }

        private bool _IsQuadrent3;
        public bool IsQuadrent3
        {
            get { return _IsQuadrent3; }
            set { Set(() => IsQuadrent3, ref _IsQuadrent3, value); }
        }

        private bool _IsQuadrent4;
        public bool IsQuadrent4
        {
            get { return _IsQuadrent4; }
            set { Set(() => IsQuadrent4, ref _IsQuadrent4, value); }
        }

        private eMatchingBasis _MatchingBasis;
        public eMatchingBasis MatchingBasis
        {
            get { return _MatchingBasis; }
            set {
                Set(() => MatchingBasis, ref _MatchingBasis, value);
                Log4.PrinterLogger.InfoFormat("[A]MatchingBasis={0}", MatchingBasis.ToString());
            }
        }


        private Range _HeightRange;
        public Range HeightRange
        {
            get { return _HeightRange; }
            set { Set(() => HeightRange, ref _HeightRange, value); }
        }
        private Range _AreaRange;
        public Range AreaRange
        {
            get { return _AreaRange; }
            set { Set(() => AreaRange, ref _AreaRange, value); }
        }
        private Range _VolumeRange;
        public Range VolumeRange
        {
            get { return _VolumeRange; }
            set { Set(() => VolumeRange, ref _VolumeRange, value); }
        }

        private bool _IsHeightFilter;
        public bool IsHeightFilter
        {
            get { return _IsHeightFilter; }
            set { Set(() => IsHeightFilter, ref _IsHeightFilter, value); }
        }
        private bool _IsAreaFilter;
        public bool IsAreaFilter    
        {
            get { return _IsAreaFilter; }
            set { Set(() => IsAreaFilter, ref _IsAreaFilter, value); }
        }
        private bool _IsVolumeFilter;
        public bool IsVolumeFilter
        {
            get { return _IsVolumeFilter; }
            set { Set(() => IsVolumeFilter, ref _IsVolumeFilter, value); }
        }




        public CommonSettingVM Clone()
        {
            return (CommonSettingVM)this.MemberwiseClone();
        }
        public XElement ToXml()
        {
            XElement root = new XElement("CommonSetting"
                                            ,new XElement("MatchingBasis", MatchingBasis.ToString())
                                            ,new XElement("Adjustment"
                                                                    , new XAttribute("IsMoveStencil", IsMoveStencil.ToString())
                                                                    , new XAttribute("IsMovePCB", IsMovePCB.ToString())
                                                        )
                                            , new XElement("Rotate"
                                                                    , new XAttribute("IsCWRotate", IsCWRotate.ToString())
                                                                    , new XAttribute("IsCCWRotate", IsCCWRotate.ToString())
                                                        )
                                            , new XElement("Quadrent"
                                                                    , new XAttribute("IsQuadrent1", IsQuadrent1.ToString())
                                                                    , new XAttribute("IsQuadrent2", IsQuadrent2.ToString())
                                                                    , new XAttribute("IsQuadrent3", IsQuadrent3.ToString())
                                                                    , new XAttribute("IsQuadrent4", IsQuadrent4.ToString())
                                                        )
                                            , new XElement("Filter"
                                                                    , new XElement("Height" , IsHeightFilter.ToString()
                                                                                            
                                                                                            , new XAttribute("LowerBound", HeightRange.LowerBound)
                                                                                            , new XAttribute("UpperBound", HeightRange.UpperBound)
                                                                                  )
                                                                    , new XElement("Area", IsAreaFilter.ToString()
                                                                                            
                                                                                            , new XAttribute("LowerBound", AreaRange.LowerBound)
                                                                                            , new XAttribute("UpperBound", AreaRange.UpperBound)
                                                                                  )
                                                                    , new XElement("Volume", IsVolumeFilter.ToString()
                                                                                           
                                                                                            , new XAttribute("LowerBound", VolumeRange.LowerBound)
                                                                                            , new XAttribute("UpperBound", VolumeRange.UpperBound)
                                                                                  )
                                                        )
                                            
                                            
                                            
                                            
                                            );
            

            return root;
        }
    }

   
}