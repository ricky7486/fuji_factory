   M S B A M L   `   `   `  ����H  DPrinterCenter, Version=0.5.2.1, Culture=neutral, PublicKeyToken=nullM+clr-namespace:PrinterCenter.UI.FujiEasyLinkPrinterCenter.UI.FujiEasyLink  5      O,clr-namespace:PrinterCenter.UI.CommonSettingPrinterCenter.UI.CommonSetting  5      2    ,PrinterCenter.UI.FujiEasyLink.ucFujiEasyLink   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      =local+clr-namespace:PrinterCenter.UI.FujiEasyLink     5      FCommonSetting,clr-namespace:PrinterCenter.UI.CommonSetting     5      $	��500��5
      $	��300��6     � 	Resources  5      >   8PrinterCenter.UI.CommonSetting.BoolToVisibilityConverter   BoolToVisibilityConverter��%   &	          5   
   62   5       � DataContext 5      �� 5   
    �� Path$ FujiEasyLinkVM�6     Locator �� Source# [ 6(   .��5      �  � RowDefinitions 5   
   �� 5      $
��1.5* �6   �� 5      $��2* �6   �� 5      $��2* �6   �� 5      $��2* �6   �� 5      $��2* �6   �� 5      $��2* �6   �� 5      $��2* �6   �� 5      $
��1.5* �6   �� 5      $��1* �6   �� 5       $
��1.5* �6   �� 5!      $��2* �6   �� 5"      $��2* �6   �� 5#      $��2* �6   �� 5$      $��2* �6   �� 5%      $��2* �6   �� 5&      $��2* �6   �� 5'      $
��1.5* �6   �� 5)      $��1* �6   �� 5*      $��5* �6    � ColumnDefinitions 5,   
   �� 5-      $��1* �6   �� 5.      $��1* �6   �� 5/      $��1* �6   �� 50      $��1* �6   �� 51      $��1* �6   .+�54   
   �� $��0��6   $��5��6   ���	#FFBDC9D1$��2,2,2,0q�6J   $��19��6d    �� CornerRadius$ 	17,17,0,0��6p   ._�55      �� $��Center=�6    
 @LANE1#���  66   
���  ��$��Normal"�6}   $��14(�6�   ��6V   Z� 6�   56      �� 57   
   -   5         ��Name$ cbOutImageLane1��57      $��Center=�6-   $��1��6O   $��0��6\     @OUTPUT_IMAGE#���  6m   $��10,0,0,0q�6�   $��Center=�6�    d� 	IsChecked 6�   �� *isOutputImageLane1+6�   �� 58   
   -   5        $ cbCopyLane1��58      $��Center=�6)   $��2��6K   $��0��6X   $��2��6h    	 @COPY#���  6|   $��10,0,0,0q�6�   $��Center=�6�    6�   �� *isCopyLane1+6�   �� 59   
   $��1��6   $��1��6!    	 @PATH#���  61   $��Center=�6P   �� 5:   
   $��2��6   $��1��6!   #���  61   $��Center=�6P   �� 5;   
   $��1��6   $��2��6   $��2��6/   ��6C   �� *ImagePathLane1+{�6c   �� *	IsChecked+	 �� ElementName$	 cbOutImageLane1���� 5<   
   $��2��6   $��2��6   $��2��6/   ��6C   �� *CopyPathLane1+{�6b   �� *	IsChecked+$	 cbCopyLane1���� 5=   
   -   5        
 ��Name$
 Lane1OutputImage��5=      $��1��6+   $��4��68   $��30��6H   $��30��6S   ��6_   �� *FindFolderCommand+��6�   �� $	 Lane1OutputImage��.��5>      �� $<��6/PrinterCenter;component/Images/folder-search-icon.png��6   6V   5?      �� 5@   
   -   5        $
 	Lane1Copy��5@      $��2��6$   $��4��61   $��30��6A   $��30��6L   ��6X   �� *FindFolderCommand+��6~   �� $	 	Lane1Copy��.��5A      �� $<��6/PrinterCenter;component/Images/folder-search-icon.png��6   6V   5B      �� 5D   
   $��3��6   $��4��6"    �� Orientation$ 
Horizontal=�66   .+�5E      �� $��
ChangeOver��6   $��Center=�6-   $��10,0,0,0q�6O    6a   �� *isChangeOverEnableLane1+6�   �� 5F      $��Mode:��6   $��Center=�6%   $��20,0,0,0q�6@   	� 5G       	� 	GroupName$ ChangeOverLane1��6   $��Kanban��67   $��Center=�6H    6j   �� *isKanbanLane1+6�   	� 5H      $ ChangeOverLane1��6   $��PanelID��67   $��Center=�6I    6k   �� *isPanelIDLane1+6�   5I      �� 5J   
   $��4��6   $��0��6!   $��CURRENT��61   $��10,0,0,0q�6@   y�6R   �� *isChangeOverEnableLane1+ �� 	Converter# [  �� 5K   
   $��4��6   $��1��6   $��3��6/     @FUJI_CURRENT_DES � ToolTip# �  6�   y�6C   �� *isChangeOverEnableLane1+# [  ��5L      �� *Lane1Current+�� 5M   
   -   5        $
 Lane1Current��5M      $��4��6'   $��4��64   $��30��6D   $��30��6O   ��6[   �� *FindFolderCommand+��6�   �� $	 Lane1Current��y�5N      �� *isChangeOverEnableLane1+# [  .��5O      �� $<��6/PrinterCenter;component/Images/folder-search-icon.png��6   6V   5P      �� 5Q   
   $��5��6   $��0��6!   $
��NEXT��61   $��10,0,0,0q�6=   y�6O   �� *isChangeOverEnableLane1+# [  �� 5R   
   $��5��6   $��1��6   $��3��6/     @FUJI_NEXT_DES# �  6�   y�6C   �� *isChangeOverEnableLane1+# [  ��5S      �� *	Lane1Next+�� 5T   
   -   5        $
 	Lane1Next��5T      $��5��6$   $��4��61   $��30��6A   $��30��6L   ��6X   �� *FindFolderCommand+��6~   �� $	 	Lane1Next��y�5U      �� *isChangeOverEnableLane1+# [  .��5V      �� $<��6/PrinterCenter;component/Images/folder-search-icon.png��6   6V   5W      �� 5X   
   $��6��6   $��0��6!   $��XML Location:��61   $��10,0,0,0q�6F   y�6X   �� *isChangeOverEnableLane1+# [  �� 5Y   
   $��6��6   $��1��6   $��3��6/   # �  6�   y�6C   �� *isChangeOverEnableLane1+# [  ��5Z      �� *Lane1XMLLocation+�� 5[   
   -   5        $
 Lane1XMLLocation��5[      $��6��6+   $��4��68   $��30��6H   $��30��6S   ��6_   �� *FindFolderCommand+��6�   �� $	 Lane1XMLLocation��y�5\      �� *isChangeOverEnableLane1+# [  .��5]      �� $<��6/PrinterCenter;component/Images/folder-search-icon.png��6   6V   5^      �� 5_   
   $��7��6   $��5��6   ���	#FFBDC9D1$��2,0,2,2q�6K   $��19��6e   $ 	0,0,17,17��6q   �� 5a   
   $��9��6   $��5��6   ���	#FFBDC9D1$��2,2,2,0q�6J   $��19��6d   $ 	17,17,0,0��6p   ._�5b      �� $��Center=�6    
 @LANE2#���  66   
���  ��$��Normal"�6}   $��14(�6�   ��6V   Z� 6�   5c      �� 5d   
   -   5        $ cbOutImageLane2��5d      $��10��6,   $��0��6:   $��2��6J   #���  6^   $��10,0,0,0q�6�   $��Center=�6�    6�   �� *isOutputImageLane2+6�   �� 5e   
   -	   5        $ cbCopyLane2��5e      $��11��6(   $��0��66   $��2��6F   #���  6Z   $��10,0,0,0q�6�   $��Center=�6�    6|   �� *isCopyLane2+6�   �� 5f   
   $��10��6   $��1��6"   #���  62   $��Center=�6Q   �� 5g   
   $��11��6   $��1��6"   #���  62   $��Center=�6Q   �� 5h   
   $��10��6   $��2��6    $��2��60   ��6D   �� *ImagePathLane2+{�6d   �� *	IsChecked+$	 cbOutImageLane2���� 5i   
   $��11��6   $��2��6    $��2��60   ��6D   �� *CopyPathLane2+{�6c   �� *	IsChecked+$	 cbCopyLane2���� 5j   
   -
   5        $
 Lane2OutputImage��5j      $��10��6+   $��4��69   $��30��6I   $��30��6T   ��6`   �� *FindFolderCommand+��6�   �� $	 Lane2OutputImage��.��5k      �� $<��6/PrinterCenter;component/Images/folder-search-icon.png��6   6V   5l      �� 5m   
   -   5        $
 	Lane2Copy��5m      $��11��6$   $��4��62   $��30��6B   $��30��6M   ��6Y   �� *FindFolderCommand+��6   �� $	 	Lane2Copy��.��5n      �� $<��6/PrinterCenter;component/Images/folder-search-icon.png��6   6V   5o      �� 5q   
   $��12��6   $��4��6#   $ 
Horizontal=�67   .+�5r      �� $��
ChangeOver��6   $��Center=�6-   $��10,0,0,0q�6O    6a   �� *isChangeOverEnableLane2+6�   �� 5s      $��Mode:��6   $��Center=�6%   $��20,0,0,0q�6@   	� 5t      $ ChangeOverLane2��6   $��Kanban��67   $��Center=�6H    6j   �� *isKanbanLane2+6�   	� 5u      $ ChangeOverLane2��6   $��PanelID��67   $��Center=�6I    6k   �� *isPanelIDLane2+6�   5v      �� 5w   
   $��13��6   $��0��6"   $��CURRENT��62   $��10,0,0,0q�6A   y�6S   �� *isChangeOverEnableLane2+# [  �� 5x   
   $��13��6   $��1��6    $��3��60   # �  6�   y�6D   �� *isChangeOverEnableLane2+# [  ��5y      �� *Lane2Current+�� 5z   
   -   5        $
 Lane2Current��5z      $��13��6'   $��4��65   $��30��6E   $��30��6P   ��6\   �� *FindFolderCommand+��6�   �� $	 Lane2Current��y�5{      �� *isChangeOverEnableLane2+# [  .��5|      �� $<��6/PrinterCenter;component/Images/folder-search-icon.png��6   6V   5}      �� 5~   
   $��14��6   $��0��6"   $
��NEXT��62   $��10,0,0,0q�6>   y�6P   �� *isChangeOverEnableLane2+# [  �� 5   
   $��14��6   $��1��6    $��3��60   # �  6�   y�6D   �� *isChangeOverEnableLane2+# [  ��5�      �� *	Lane2Next+�� 5�   
   -   5        $
 	Lane2Next��5�      $��14��6$   $��4��62   $��30��6B   $��30��6M   ��6Y   �� *FindFolderCommand+��6   �� $	 	Lane2Next��y�5�      �� *isChangeOverEnableLane2+# [  .��5�      �� $<��6/PrinterCenter;component/Images/folder-search-icon.png��6   6V   5�      �� 5�   
   $��15��6   $��0��6"   $��XML Location��62   $��10,0,0,0q�6F   y�6X   �� *isChangeOverEnableLane2+# [  �� 5�   
   $��15��6   $��1��6    $��3��60   # �  6�   y�6D   �� *isChangeOverEnableLane2+# [  ��5�      �� *Lane2XMLLocation+�� 5�   
   -   5        $
 Lane2XMLLocation��5�      $��15��6+   $��4��69   $��30��6I   $��30��6T   ��6`   �� *FindFolderCommand+��6�   �� $	 Lane2XMLLocation��y�5�      �� *isChangeOverEnableLane2+# [  .��5�      �� $<��6/PrinterCenter;component/Images/folder-search-icon.png��6   6V   5�      �� 5�   
   $��16��6   $��5��6   ���	#FFBDC9D1$��2,0,2,2q�6L   $��19��6f   $ 	0,0,17,17��6r   �� 5�   
   $��17��6   $��0��6"   $��ChangeOVer Mapping��62   $��10,0,0,0q�6L   &   System.Windows.Controls.DataGrid  5�   
   -   5          Name$ dataGrid��5�      $��18��6%   $��0��63   $��5��6C      CanUserAddRows .    AutoGenerateColumns .    CanUserDeleteRows . ��5�      �� *MappingList+ �� Mode$ TwoWay=� �� UpdateSourceTrigger$ PropertyChanged=�   Columns 5�      5�      5�      5�      