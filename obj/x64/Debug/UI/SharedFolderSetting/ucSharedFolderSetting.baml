   M S B A M L   `   `   `  ����H  DPrinterCenter, Version=0.5.2.1, Culture=neutral, PublicKeyToken=null[2clr-namespace:PrinterCenter.UI.SharedFolderSetting$PrinterCenter.UI.SharedFolderSetting  5      O,clr-namespace:PrinterCenter.UI.CommonSettingPrinterCenter.UI.CommonSetting  5      @    :PrinterCenter.UI.SharedFolderSetting.ucSharedFolderSetting   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Dlocal2clr-namespace:PrinterCenter.UI.SharedFolderSetting     5      FCommonSetting,clr-namespace:PrinterCenter.UI.CommonSetting     5      c _System.Windows.Interactivity, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35Bi:http://schemas.microsoft.com/expression/2010/interactivity  5      $
��Auto��5
      $
��Auto��6     � 	Resources  5      >   8PrinterCenter.UI.CommonSetting.BoolToVisibilityConverter   BoolToVisibilityConverter��%   &	          5   
   62   5       � DataContext 5      @   :PrinterCenter.UI.SharedFolderSetting.SharedFolderSettingVM  5   
   .��5      � $
��Auto��6   $��10q�6    � RowDefinitions 5   
   �� 5      $��1* �6   �� 5      $��3* �6   �� 5      $��3* �6   �� 5      $��1* �6   .+�5   
   �� $��0��6   ���	#FFBDC9D1$��2,2,2,0q�66   $��19��6P    �� CornerRadius$ 	17,17,0,0��6\   ._�5      �� -   5         ��Name$ tbVendor��5   5   $��Center=�6   
���  ��$��Normal"�6o   $��14(�6�   ��6H   Z� ��5      �� *Vendor+ �� Mode$ OneWay=�5      ]� 5!   
   -   5         ]�Name$ dpInSharedFolder��5!      $��1��6/   $
��Auto��6<   $��10q�6I   y�6U   �� *
IsInEnable+ �� 	Converter# [  $ TwoWay=�.+�5"      ��   @RECIEVE#���  6   $	��100��6:   $
��Left=�6F   $��Center=�6\   �� 5#      $��Center=�6   $
��Left=�64   $	��200��6O   $
��Wrap=�6[     @NETWORKDRIVE_MAPPING_DISK#���  5$      �� 5%      $��40��6   $	��250��6$   $��Right=�60   $��Right=�6L    �� SelectedValue 5&      �� *InDriveInfo+$ TwoWay=���68   �� *InSharedFolder+.  (System.Windows.Interactivity.Interaction	   Triggers	 5'      /  )System.Windows.Interactivity.EventTrigger  5(      
   	EventName$
 SelectionChanged��6%   .  (System.Windows.Interactivity.TriggerBase   Actions. 5)      6  0System.Windows.Interactivity.InvokeCommandAction     Command 60   �� *InSFSelectionChangedCommand+5*      5,      5-      ]� 5.   
   -   5        $ dpOutSharedFolder��5.      $��2��60   $
��Auto��6=   $��10q�6J   y�6V   �� *IsOutEnable+# [  $ TwoWay=�.+�50      ��  	 @SEND#���  6   $	��100��68   $
��Left=�6D   $��Center=�6Z   �� 51      $��Center=�6   $
��Left=�64   $	��200��6O   $
��Wrap=�6[   #���  52      �� 53      $��40��6   $	��250��6$   $��Right=�60   $��Right=�6M    54      �� *OutDriveInfo+$ TwoWay=���69   �� *OutSharedFolder+	 55        56      $
 SelectionChanged��6%   . 57         60   �� *OutSFSelectionChangedCommand+58      5:      5;      �� 5<   
   $��3��6   ���	#FFBDC9D1$��2,0,2,2q�67   $��19��6Q   $ 	0,0,17,17��6]   $
��Auto��6v   $��0,0,0,10q�6�   5@      5A      