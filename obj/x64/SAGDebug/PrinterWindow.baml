   M S B A M L   `   `   `  ���� H  DPrinterCenter, Version=0.5.2.1, Culture=neutral, PublicKeyToken=null-clr-namespace:PrinterCenterPrinterCenter  I)clr-namespace:PrinterCenter.CustomControlPrinterCenter.CustomControl  3clr-namespace:PrinterCenter.UIPrinterCenter.UI  S.clr-namespace:PrinterCenter.UI.OneLaneSelector PrinterCenter.UI.OneLaneSelector  O,clr-namespace:PrinterCenter.UI.CommonSettingPrinterCenter.UI.CommonSetting  [2clr-namespace:PrinterCenter.UI.SharedFolderSetting$PrinterCenter.UI.SharedFolderSetting  A%clr-namespace:PrinterCenter.UI.CustomPrinterCenter.UI.Custom  ?$clr-namespace:PrinterCenter.UI.ChartPrinterCenter.UI.Chart  =#clr-namespace:PrinterCenter.UI.FlowPrinterCenter.UI.Flow  =#clr-namespace:PrinterCenter.UI.WipePrinterCenter.UI.Wipe  A%clr-namespace:PrinterCenter.UI.DoctorPrinterCenter.UI.Doctor  M+clr-namespace:PrinterCenter.UI.FujiEasyLinkPrinterCenter.UI.FujiEasyLink  !    PrinterCenter.PrinterWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     -localclr-namespace:PrinterCenter     CcustomControl)clr-namespace:PrinterCenter.CustomControl     7netDriveToolclr-namespace:PrinterCenter.UI     JoneLaneSelector.clr-namespace:PrinterCenter.UI.OneLaneSelector     FcommonSetting,clr-namespace:PrinterCenter.UI.CommonSetting     H	sfSetting2clr-namespace:PrinterCenter.UI.SharedFolderSetting     :ucCustom%clr-namespace:PrinterCenter.UI.Custom     8ucChart$clr-namespace:PrinterCenter.UI.Chart     6ucFlow#clr-namespace:PrinterCenter.UI.Flow     6ucWipe#clr-namespace:PrinterCenter.UI.Wipe     :ucDoctor%clr-namespace:PrinterCenter.UI.Doctor     >ucFuji+clr-namespace:PrinterCenter.UI.FujiEasyLink     c _System.Windows.Interactivity, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35Bi:http://schemas.microsoft.com/expression/2010/interactivity  -      DialogWindowStyle#��[    @PRINTER_CENTER  � Title#  �  $	��700��$	��600�� � SizeToContent$ WidthAndHeight=� � DataContext �� *PrinterWindowVM+  Locator �� Source# [  � 	Resources >   8PrinterCenter.UI.CommonSetting.BoolToVisibilityConverter  BoolToVisibilityConverter��%   &	         .��� $	��800��$
��Auto�� � ColumnDefinitions �� $��1* ��� $��8* �.+��� $��0�����	#FF629ABB$��5q� �� CornerRadius$ 10��$
��Auto��._���  �� Orientation$ Vertical=�$��10q�.+�	�  	� 	GroupName$ ViewSelector��$��50��$��50��$��10q�$��0q�#��["�y��� *SettingVisible+	 �� 	Converter#	 [ 
 d� 	IsChecked
 �� *SettingIsCheck+.���� $��Images/Setting.png����   @SETTING#���  $��Center=�$
��Wrap=�y��� *SettingVisible+#	 [ 	� $ ViewSelector��$��50��$��50��$��10q�$��0q�#��["�y��� *ChartVisible+#	 [ 
 �� *ChartIsCheck+.���� $��Images/chart.png����  
 @CHART#���  $��Center=�$
��Wrap=�y��� *ChartVisible+#	 [ 	� $ ViewSelector��$��50��$��50��$��10q�$��0q�#��["�y��� *FlowVisible+#	 [ 
 �� *FlowIsCheck+.���� $��Images/Flow.png����  	 @FLOW#���  $��Center=�$
��Wrap=�y��� *FlowVisible+#	 [ 	� $ ViewSelector��$��50��$��50��$��10q�$��0q�#��["�y��� *WipeVisible+#	 [ 
 �� *WipeIsCheck+.���� $��Images/Wipe.png����  	 @WIPE#���  $��Center=�$
��Wrap=�y��� *WipeVisible+#	 [ 	� $ ViewSelector��$��50��$��50��$��10q�$��0q�#��["�y��� *DoctorVisible+#	 [ 
 �� *DoctorIsCheck+.���� $��Images/Doctor.png��$��35��$��35����   @DOCTOR#���  $��Center=�$
��Wrap=�y��� *DoctorVisible+#	 [ � $��1��$��20q�y��� *SettingIsCheck+#	 [ .+�0   *PrinterCenter.CustomControl.StepTabControl     TabTextColor
 �   �   SelectedTabTextColor
 �  ��   SelectedTabBackground �	#FFE0C581   	TabMargin$ 10q�   	TabHeight$ 60��   TabWidth$	 100��
������ $��0q� �� SelectedIndex �� *stcSelectedIndex+.@���  		 @TOOL#��� 	 .��%   PrinterCenter.UI.ucNetworkDrive  -     Name$ ucNetworkDrive����  
 @GENERAL_SETTING#��� 
 $��0,0,0,0q�.��9   3PrinterCenter.UI.OneLaneSelector.ucLaneSelectorHost  �� -    ��Name$ tabLane1�� 
 @LANE1#���  $��0,0,0,0q�$y�	Collapsed=�.���� .+�4   .PrinterCenter.UI.CommonSetting.ucCommonSetting  -     Name$ ucLane1_Common��$
��Auto��@   :PrinterCenter.UI.SharedFolderSetting.ucSharedFolderSetting  -     Name$ 
ucLane1_SF���� -   $ tabLane2�� 
 @LANE2#���  $��0,0,0,0q�$y�	Collapsed=�.���� .+�  -   $ ucLane2_Common��$
��Auto��  -   $ 
ucLane2_SF����   @CUSTOMIZATION#���  y��� *isCustomizationVisible+#	 [ .��&    PrinterCenter.UI.Custom.ucCustom  -	     Name$ ucCustom���� $��FujiEasyLink��y��� *isFujiEasyLinkVisible+#	 [ .��2   ,PrinterCenter.UI.FujiEasyLink.ucFujiEasyLink  -
     Name$ ucFujiEasyLink���� $��Final��.���  � RowDefinitions �� $��0.35* ��� $��0.35* ��� $
��0.2* ��� $
��0.1* � �� $
��0.2* ��� $
��0.8* �.+��� -    ��Name$ lbStatusReporter��$	��500��$	��160��$��0��$��2���� -   $ 
lbDebugMsg��$	��500��$	��160��$��1��$��2���� $��2��$��1��$	��150��$��40��$��Right=�$��5q�  @OPEN_SERVICE#���  ���� *OpenServiceCommand+y��� *BtnOpenServiceVisible+#	 [ �� $��3��$��2��
������ .@��� .���� $ 
Horizontal=�$��Center=�.+���   @SERIAL_NUMBER#���  �� -    ��Name$ tbSerialNumber��$��10,0,0,0q�$	��100������ *SN+{��� *
SNEnable+�� $��0q�#��[" �$��30��$��10,0,0,0q����� *SNEditCommand+.���� $��/Images/EditModel.png��� $��1��$��20q�y��� *ChartIsCheck+#	 [ .+�(	   "PrinterCenter.UI.Chart.ucChartHost	  -    	 Name$ ucChartHost��$	��800��� $��1��$��20q�y��� *FlowIsCheck+#	 [ .+�&
    PrinterCenter.UI.Flow.ucFlowHost
  -    
 Name$ 
ucFlowHost��� $��1��$��20q�y��� *WipeIsCheck+#	 [ .+�"   PrinterCenter.UI.Wipe.ucWipe  -     Name$ ucWipe��� $��1��$��20q�y��� *DoctorIsCheck+#	 [ .+�&    PrinterCenter.UI.Doctor.ucDoctor  -     Name$ ucDoctor��