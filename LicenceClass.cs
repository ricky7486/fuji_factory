using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace PrinterCenter
{
    /// <summary>
    /// 從TR7007i移植過來
    /// </summary>
    public class Context
    {
        static readonly object synclock = new Object();

        static Context uniqueInstance;

        public static void CreateInstance()
        {
            if (uniqueInstance == null)
                uniqueInstance = new Context();
        }

        public static Context Instance
        {
            get
            {
                if (uniqueInstance == null)
                {
                    lock (synclock)
                    {
                        CreateInstance();
                    }
                }
                return uniqueInstance;
            }
        }

        bool _isOfflineMode;
        /// <summary> OfflineMode定義: 不使用硬體(含Camera, Lighting, Motion, PLC)。在SPI機台電腦、辦公室電腦或筆記型電腦上皆可運行OfflineMode</summary>
		public bool IsOfflineMode
        {
            get { return _isOfflineMode; }
            set
            {
                _isOfflineMode = value;
                //Log.Info("IsOfflineMode is set to " + _isOfflineMode);
            }
        }

        private bool _isOfflineTuning;
        /// <summary> 是否為離線調整模式 (當值為true時，IsOfflineMode亦為true) </summary>
		public bool IsOfflineTuning
        {
            get { return _isOfflineTuning; }
            set
            {
                _isOfflineTuning = value;
                IsOfflineMode = value;
            }
        }

    }
    public enum eCategory55B : int
	{
		Standard = 1,
		AdminTool = 2,
		PrintCL = 4,
		OffLine = 8,
		APC = 16,
		Calibration = 32,
		PrintCL_MPM = 64,
		PrintCL_DEK = 128,
	}
	public enum eCategory61B : int
	{
		PrintCL_EKRA = 1,
		ControlCenter = 2,
		PrintCL_DESEN = 4,
		CL_SiemensPlacer = 8,
		CL_YamahaPlacer = 16,
		CL_SamsungPlacer = 32,
		CL_PanasonicPlacer = 64,
		CL_ASYSBuffer = 128
	}
	public enum eCategory62B : int
	{
		PrintCL_MINAMI = 1,
		PrintCL_GKG = 2,
        PrintCL_ESE = 4,
        CL_FujiPlacer = 8
	}
	public enum eDglContent : int
	{
		NONE = 0,
		SPI = 1,
		Admini = 99,
		Operator = -41
	}

	public static class TRILicense
	{
		[DllImport("TRI_LicenseKey_X64.dll", EntryPoint = "TRI_UsbGetDevNum", SetLastError = false, CharSet = CharSet.Auto, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
		private static extern int TRI_UsbGetDevNum();
		[DllImport("TRI_LicenseKey_X64.dll", EntryPoint = "TRI_UsbRead", SetLastError = false, CharSet = CharSet.Auto, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
		private static extern int TRI_UsbRead(UInt16 Address, int UsbKeyNum);
		[DllImport("TRI_LicenseKey_X64.dll", EntryPoint = "TRI_UsbReadAll", SetLastError = false, CharSet = CharSet.Auto, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
		private static extern int TRI_UsbReadAll(IntPtr Buffer, int UsbKeyNum);

		enum eDongleKey : ushort
		{
			CategoryContent = 0, //Byte 0 => Bit 1: SPI, Bit 99: Administrator
			CategoryType = 1, //Byte 1 => Bit 1=7006, Bit 2=7007, Bit 3 = 7007i
			Category55B = 55, //Byte 55 => Bit 1: Standard, Bit 2: Administrator Tool, Bit 3: Printer Closed Loop, Bit 4: Offline editor
			Category61B = 61, // Byte 61 => Bit 1: Printer Closed Loop (EKRA), Bit 2: Control Center Tool, Bit 3: Printer Closed Loop (DESEN), Bit 4: Placer Closed Loop (Siemens)
			//Bit 5: Placer Closed Loop (Yamaha),  Bit 6: Placer Closed Loop (Samsung),  Bit 7: Placer Closed Loop (Panasonic),  Bit 8: Placer Closed Loop (ASYS), 
			Category62B = 62, // Byte 62 => Bit 1: Printer Closed Loop (MINAMI), Bit 1: Printer Closed Loop (MINAMI)
		}

		enum eDglType : int
		{
			TR7006 = 1,
			TR7007 = 2,
			TR7007i = 4
		}

		public const string OperatorName = "TRI";
		public const string OperatorPassword = "0228328918";

		public static bool bAdm = true;
		/// <summary>
		///  Check all dongle key
		/// </summary>
		/// <param name="dongleKey">Key Content</param>
		/// <param name="nVal">Value</param>
		private static bool CheckAllKey(eDongleKey dongleKey, int nVal)
		{
			for (int i = 1; i <= TRI_UsbGetDevNum(); i++)
			{
				if (TRI_UsbRead((ushort)dongleKey, i) == nVal)
					return true;
			}
			return false;
		}

		private static bool CheckDefaultKeyExist(eDongleKey dongleKey)
		{
			for (int i = 1; i <= TRI_UsbGetDevNum(); i++)
			{
				if (TRI_UsbRead((ushort)dongleKey, i) == (int)eDglContent.Operator)
					return true;
			}
			return false;
		}

		#region back door
		private static bool bActiveSoftwareAdministratorLicence = false;
		public static void ActivatePsudoAdministrator() //PenNote: For fast UI debug only!!! Please don't use in other place.
		{
            if (bActiveSoftwareAdministratorLicence == false) //a trick to remove compile warning (not used variable)
			    bActiveSoftwareAdministratorLicence = true;
		}

		private static System.Threading.Timer countDownTimer = null;
		private static readonly int iDefaultMinutes = 30; //30 min
		private static readonly int iMaximuntMinutes = 300; //300 min
		private static readonly int iMinimuntMinutes = 1; //1 min
		public static void ActiveSoftwareAdministratorLicence(string sPassword)
		{
			string[] words = sPassword.Split(' ');
			string strKeyWord = "";
			string strTime = iDefaultMinutes.ToString();
			for (int i = 0; i < words.Length; i++)
			{
				string word = words[i];
				switch (i)
				{
					case 0:
						{
							strKeyWord = word;
							break;
						}
					case 1:
						{
							strTime = word;
							break;
						}
				}


			}

			if (strKeyWord.Equals("TRAdm"))
			{
				int iMinutes;
				if (int.TryParse(strTime, out iMinutes))
				{
					if (iMinutes < iMinimuntMinutes)
					{
						iMinutes = iMinimuntMinutes;
					}

					if (iMinutes > iMaximuntMinutes)
					{
						iMinutes = iMaximuntMinutes;
					}

				}
				else
				{
					iMinutes = iDefaultMinutes;
				}


				TRILicense.bActiveSoftwareAdministratorLicence = true;
				countDownTimer = new System.Threading.Timer(new TimerCallback(action), null, iMinutes * 60 * 1000, 0);
			}
			else
			{
				//Try error
				TRILicense.bActiveSoftwareAdministratorLicence = false;
			}

			//Update licence
			IsAdministrator();
		}
		private static void action(object obj)
		{
			TRILicense.bActiveSoftwareAdministratorLicence = false;
			IsAdministrator();

			countDownTimer.Dispose();
			countDownTimer = null;
		}
		#endregion

		static eDglContent currentRight = eDglContent.NONE;
		public static eDglContent CurrentRight
		{
			get { return TRILicense.currentRight; }
			set
			{
				if (TRILicense.currentRight == value)
					return;
				TRILicense.currentRight = value;
				TRILicense.OnChangedCurrentRight(EventArgs.Empty);
			}
		}

		public static event EventHandler EventChangedCurrentRight;
		private static void OnChangedCurrentRight(EventArgs e)
		{
			if (EventChangedCurrentRight != null)
				EventChangedCurrentRight(null, e);
		}

		public static bool IsAdministrator()
		{
#if DEBUG //加此判斷，避免Release版被attach debugger，因而自動取得administrator權限
            if (Debugger.IsAttached) //避免Debug版流出被利用
            {
                CurrentRight = eDglContent.Admini;
                return true;
            }
#endif
            if (Context.Instance.IsOfflineMode)
            {
                CurrentRight = eDglContent.SPI;
                return true;
            }
            if (((CheckAllKey(eDongleKey.CategoryContent, (int)eDglContent.Admini) || bActiveSoftwareAdministratorLicence)) && bAdm)
			{
				CurrentRight = eDglContent.Admini;
				return true;
			}

			return false;
        }

		public static bool IsSpiAuthority()
		{
			if (CheckAllKey(eDongleKey.CategoryContent, (int)eDglContent.SPI))
			{
				CurrentRight = eDglContent.SPI;
				return true;
			}
			return false;
		}

		public static bool IsOperatorAuthority()
		{
			if (CheckDefaultKeyExist(eDongleKey.CategoryContent))
			{
				CurrentRight = eDglContent.Operator;
				return true;
			}
			return false;
		}

		public static bool LicenceIsValid()
		{
			return (IsAdministrator() || IsSpiAuthority() || IsOperatorAuthority());
		}

		const int nDglContent = (int)eDglContent.SPI;
		const int nDglType = (int)eDglType.TR7007 | (int)eDglType.TR7007i;

		public static bool IsVaild<T>(T license) where T : struct, IConvertible
		{
			if (!typeof(T).IsEnum)
				throw new ArgumentException("T must be an enumerated type");

			if (IsAdministrator())
				return true;

			ushort category;
			switch (typeof(T).Name)
			{
				case "eCategory55B": category = (ushort)eDongleKey.Category55B; break;
				case "eCategory61B": category = (ushort)eDongleKey.Category61B; break;
				case "eCategory62B": category = (ushort)eDongleKey.Category62B; break;
				default: return false;
			}

			Enum tmp = Enum.Parse(typeof(T), license.ToString()) as Enum;
			int nLicense = Convert.ToInt32(tmp);

			for (int i = 1; i <= TRI_UsbGetDevNum(); i++)
			{
				if ((TRI_UsbRead((ushort)eDongleKey.CategoryContent, i) & nDglContent) > 0 &&
					(TRI_UsbRead((ushort)eDongleKey.CategoryType, i) & nDglType) > 0 &&
					(TRI_UsbRead(category, i) & nLicense) > 0)
					return true;
			}
			return false;
		}
	}
}
