   M S B A M L   `   `   `  ����H  DPrinterCenter, Version=0.5.2.1, Culture=neutral, PublicKeyToken=null-clr-namespace:PrinterCenterPrinterCenter  5   	   I)clr-namespace:PrinterCenter.CustomControlPrinterCenter.CustomControl  5   	   3clr-namespace:PrinterCenter.UIPrinterCenter.UI  5   	   S.clr-namespace:PrinterCenter.UI.OneLaneSelector PrinterCenter.UI.OneLaneSelector  5	   	   O,clr-namespace:PrinterCenter.UI.CommonSettingPrinterCenter.UI.CommonSetting  5
   	   [2clr-namespace:PrinterCenter.UI.SharedFolderSetting$PrinterCenter.UI.SharedFolderSetting  5   	   A%clr-namespace:PrinterCenter.UI.CustomPrinterCenter.UI.Custom  5   	   ?$clr-namespace:PrinterCenter.UI.ChartPrinterCenter.UI.Chart  5   	   =#clr-namespace:PrinterCenter.UI.FlowPrinterCenter.UI.Flow  5   	   =#clr-namespace:PrinterCenter.UI.WipePrinterCenter.UI.Wipe  5   	   A%clr-namespace:PrinterCenter.UI.DoctorPrinterCenter.UI.Doctor  5   	   M+clr-namespace:PrinterCenter.UI.FujiEasyLinkPrinterCenter.UI.FujiEasyLink  5   	   !    PrinterCenter.PrinterWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   -localclr-namespace:PrinterCenter     5   	   CcustomControl)clr-namespace:PrinterCenter.CustomControl     5   	   7netDriveToolclr-namespace:PrinterCenter.UI     5   	   JoneLaneSelector.clr-namespace:PrinterCenter.UI.OneLaneSelector     5	   	   FcommonSetting,clr-namespace:PrinterCenter.UI.CommonSetting     5
   	   H	sfSetting2clr-namespace:PrinterCenter.UI.SharedFolderSetting     5   	   :ucCustom%clr-namespace:PrinterCenter.UI.Custom     5   	   8ucChart$clr-namespace:PrinterCenter.UI.Chart     5   	   6ucFlow#clr-namespace:PrinterCenter.UI.Flow     5   	   6ucWipe#clr-namespace:PrinterCenter.UI.Wipe     5   	   :ucDoctor%clr-namespace:PrinterCenter.UI.Doctor     5   	   >ucFuji+clr-namespace:PrinterCenter.UI.FujiEasyLink     5   	   c _System.Windows.Interactivity, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35Bi:http://schemas.microsoft.com/expression/2010/interactivity  5      -   5           DialogWindowStyle#��[  5   	     @PRINTER_CENTER  � Title#  �  5   	   $	��700��5   	   $	��600��6    � SizeToContent$ WidthAndHeight=�5   	    � DataContext 5   
   �� *PrinterWindowVM+  Locator �� Source# [  � 	Resources 5      >   8PrinterCenter.UI.CommonSetting.BoolToVisibilityConverter  BoolToVisibilityConverter��%   &	         5   
   62   5      .��5      � $	��800��6   $
��Auto��6    � ColumnDefinitions 5   
   �� 5      $��1* �6   �� 5       $��8* �6   .+�5"   
   �� $��0��6   ���	#FF629ABB$��5q�69    �� CornerRadius$ 10��6M   $
��Auto��6_   ._�5#      ��  �� Orientation$ Vertical=�6   $��10q�60   .+�5%      	�  	� 	GroupName$ ViewSelector��6   $��50��67   $��50��6B   $��10q�6N   $��0q�6[   #��["�6o   y�5&      �� *SettingVisible+	 �� 	Converter#	 [ 
 d� 	IsChecked
 5'      �� *SettingIsCheck+.��5(      �� $��Images/Setting.png��6   6;   5)      �� 5*        @SETTING#���  6   $��Center=�6>   $
��Wrap=�6[   y�6o   �� *SettingVisible+#	 [ 6�   	� 5,      $ ViewSelector��6   $��50��67   $��50��6B   $��10q�6N   $��0q�6Z   #��["�6n   y�5-      �� *ChartVisible+#	 [ 
 5.      �� *ChartIsCheck+.��5/      �� $��Images/chart.png��6   69   50      �� 51       
 @CHART#���  6   $��Center=�6<   $
��Wrap=�6Y   y�6m   �� *ChartVisible+#	 [ 6�   	� 53      $ ViewSelector��6   $��50��67   $��50��6B   $��10q�6N   $��0q�6Z   #��["�6n   y�54      �� *FlowVisible+#	 [ 
 55      �� *FlowIsCheck+.��56      �� $��Images/Flow.png��6   68   57      �� 58       	 @FLOW#���  6   $��Center=�6;   $
��Wrap=�6X   y�6l   �� *FlowVisible+#	 [ 6�   	� 5:      $ ViewSelector��6   $��50��67   $��50��6B   $��10q�6N   $��0q�6Z   #��["�6n   y�5;      �� *WipeVisible+#	 [ 
 5<      �� *WipeIsCheck+.��5=      �� $��Images/Wipe.png��6   68   5>      �� 5?       	 @WIPE#���  6   $��Center=�6;   $
��Wrap=�6X   y�6l   �� *WipeVisible+#	 [ 6�   	� 5A      $ ViewSelector��6   $��50��67   $��50��6B   $��10q�6N   $��0q�6Z   #��["�6n   y�5B      �� *DoctorVisible+#	 [ 
 5C      �� *DoctorIsCheck+.��5D      �� $��Images/Doctor.png��6   $��35��67   $��35��6B   6P   5E      �� 5F        @DOCTOR#���  6   $��Center=�6=   $
��Wrap=�6Z   y�6n   �� *DoctorVisible+#	 [ 6�   5G      5H      � 5J   
   $��1��6   $��20q�6!   y�5K      �� *SettingIsCheck+#	 [ .+�5L      0   *PrinterCenter.CustomControl.StepTabControl     TabTextColor
 �   �   SelectedTabTextColor
 �  ��   SelectedTabBackground �	#FFE0C581   	TabMargin$ 10q�5M   '      	TabHeight$ 60��67      TabWidth$	 100��6F   
������ $��0q�5N   +    �� SelectedIndex 5O   +   �� *stcSelectedIndex+.@�5Q      ��  		 @TOOL#��� 	 6   .��5R      %   PrinterCenter.UI.ucNetworkDrive  -   5          Name$ ucNetworkDrive��5R   2   6L   5S      �� 5U       
 @GENERAL_SETTING#��� 
 6   $��0,0,0,0q�6F   .��5V      9   3PrinterCenter.UI.OneLaneSelector.ucLaneSelectorHost  6<   5Y      �� 5[      -   5         ��Name$ tabLane1��5[       
 @LANE1#���  6,   $��0,0,0,0q�6N   $y�	Collapsed=�6_   .��5]      �� .+�5^      4   .PrinterCenter.UI.CommonSetting.ucCommonSetting  -   5          Name$ ucLane1_Common��5^   9   $
��Auto��6Q   6a   @   :PrinterCenter.UI.SharedFolderSetting.ucSharedFolderSetting  5_      -   5          Name$ 
ucLane1_SF��5_   :   6P   5`      5b      �� 5d      -   5        $ tabLane2��5d       
 @LANE2#���  6,   $��0,0,0,0q�6N   $y�	Collapsed=�6_   .��5f      �� .+�5g        -   5        $ ucLane2_Common��5g   9   $
��Auto��6Q   6a     5h      -   5        $ 
ucLane2_SF��5h   :   6P   5i      5j      �� 5m        @CUSTOMIZATION#���  6   y�6D   �� *isCustomizationVisible+#	 [ .��5n      &    PrinterCenter.UI.Custom.ucCustom  -	   5          Name$ ucCustom��5n   (   6<   5o      �� 5r      $��FujiEasyLink��6   y�60   �� *isFujiEasyLinkVisible+#	 [ .��5s      2   ,PrinterCenter.UI.FujiEasyLink.ucFujiEasyLink  -
   5          Name$ ucFujiEasyLink��5s   ,   6F   5t      �� 5v      $��Final��6   .��5z      �  � RowDefinitions 5{      �� 5|      $��0.35* �6,   �� 5}      $��0.35* �6,   �� 5~      $
��0.2* �6,   �� 5      $
��0.1* �6,    5�      �� 5�      $
��0.2* �6/   �� 5�      $
��0.8* �6/   .+�5�      �� -   5         ��Name$ lbStatusReporter��5�   "   $	��500��6<   $	��160��6H   $��0��6U   $��2��6b   �� 5�      -   5        $ 
lbDebugMsg��5�   "   $	��500��66   $	��160��6B   $��1��6O   $��2��6\   �� 5�      $��2��6!   $��1��6.   $	��150��5�   !   $��40��6-   $��Right=�6:   $��5q�6V     @OPEN_SERVICE#���  5�   !   ��6K   �� *OpenServiceCommand+y�6r   �� *BtnOpenServiceVisible+#	 [ 6�   �� 5�      $��3��6$   $��2��61   
������ .@�5�      �� .��5�   "   �� $ 
Horizontal=�6-   $��Center=�6G   .+�5�   &   ��   @SERIAL_NUMBER#���  60   �� 5�   &   -   5         ��Name$ tbSerialNumber��5�   .   $��10,0,0,0q�6Z   $	��100��6l   ��6F   �� *SN+{�6x   �� *
SNEnable+6�   �� 5�   &   $��0q�6/   #��[" �6C   $��30��6~   $��10,0,0,0q�5�      ��61   �� *SNEditCommand+.��5�   *   �� $��/Images/EditModel.png��60   5�   '   5�   #   5�      5�      5�      5�      5�      5�      � 5�   
   $��1��6   $��20q�5�      y�5�      �� *ChartIsCheck+#	 [ .+�5�      (	   "PrinterCenter.UI.Chart.ucChartHost	  -   5         	 Name$ ucChartHost��5�   "   $	��800��67   6F   5�      � 5�   
   $��1��6   $��20q�5�      y�5�      �� *FlowIsCheck+#	 [ .+�5�      &
    PrinterCenter.UI.Flow.ucFlowHost
  -   5         
 Name$ 
ucFlowHost��5�       66   5�      � 5�   
   $��1��6   $��20q�5�      y�5�      �� *WipeIsCheck+#	 [ .+�5�      "   PrinterCenter.UI.Wipe.ucWipe  -   5          Name$ ucWipe��5�      6.   5�      � 5�   
   $��1��6   $��20q�5�      y�5�      �� *DoctorIsCheck+#	 [ .+�5�      &    PrinterCenter.UI.Doctor.ucDoctor  -   5          Name$ ucDoctor��5�       64   5�      5�      5�      