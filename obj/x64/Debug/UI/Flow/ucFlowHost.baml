   M S B A M L   `   `   `  ����H  DPrinterCenter, Version=0.5.2.1, Culture=neutral, PublicKeyToken=null=#clr-namespace:PrinterCenter.UI.FlowPrinterCenter.UI.Flow  5      O,clr-namespace:PrinterCenter.UI.CommonSettingPrinterCenter.UI.CommonSetting  5      &     PrinterCenter.UI.Flow.ucFlowHost   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      5local#clr-namespace:PrinterCenter.UI.Flow     5      FCommonSetting,clr-namespace:PrinterCenter.UI.CommonSetting     5      $
��Auto��5	      $
��Auto��6     � DataContext  5
      �� *
FlowHostVM+   Locator �� Source# [   � 	Resources 5      >   8PrinterCenter.UI.CommonSetting.BoolToVisibilityConverter  BoolToVisibilityConverter��%   &	         5   
   62   5      .��5      �  � RowDefinitions 5   
   �� 5      $��1* �6   �� 5      $��5* �6   �� 5      $��5* �6    � ColumnDefinitions 5   
   �� 5      $��1* �6   �� 5      $��1* �6   �� 5      $��1* �6   .+�5!   
   �� $��0��6   $��0��6"    �� Orientation$ Vertical=�62   y�6I   �� *IsLane1WFExist+ �� 	Converter# [ .+�5#      �� $
��Wrap=�66   ��6   �� *Lane1WFDisk+66   �� 5$      $
��Wrap=�66   ��6   �� *Lane1WFPath+66   5%      �� 5&   
   $��0��6   $��2��6"   $ Vertical=�62   y�6I   �� *IsLane2WFExist+# [ .+�5(      �� $
��Wrap=�66   ��6   �� *Lane2WFDisk+66   �� 5)      $
��Wrap=�65   ��6   �� *Lane2WFPath+65   5*      �� 5+   
   -   5         ��Name$ 	lbLane1WF��5+      $��1��6%   $��0��62   $��10q�6�   $	��300��5,   6   y�5+   B   �� *IsLane1WFExist+# [ ��5,      �� *Lane1WFList+��5-      �� 5.      .R�5/      �� $ 
Horizontal=�6!   .+�50      �� $��check��6#   $��3q�6]   $��Center=�6h   {�.   d� 	IsChecked 60   �� *	IsChecked+	 �� Mode$	 TwoWay=�51   #   �� 52      $��1q�6E   ��6+   �� *Value+6E   53      54      56      �� 57   
   -   5        $ 	lbLane2WF��57      $��1��6%   $��2��62   $��10q�6�   $	��300��58   6   y�57   B   �� *IsLane2WFExist+# [ ��58      �� *Lane2WFList+��59      �� 5:      .R�5;      �� $ 
Horizontal=�6!   .+�5<      �� $��check��6#   $��3q�6]   $��Center=�6h   {�.   60   �� *	IsChecked+$	 TwoWay=�5=   #   �� 5>      $��1q�6E   ��6+   �� *Value+6E   5?      5@      5B      �� 5E   
   $��0��6   $��1��6!     @SPI_INSPECTED_DATA#���  61   6`   �� 5F   
   -   5        $ 	lbSPIData��5F      $��1��6%   $��1��62   $��10q�6C   $	��300��5G   ;   ��6   �� *spiInspectedData+��5H      �� 5I      .R�5J      �� $ 
Horizontal=�6!   .+�5K      �� $��check��6#   $��3q�6]   $��Center=�6h   {�.   60   �� *	IsChecked+$	 TwoWay=�5L   #   �� 5M      $��1q�6E   ��6+   �� *Value+6E   5N      5O      5Q      5S      5T      