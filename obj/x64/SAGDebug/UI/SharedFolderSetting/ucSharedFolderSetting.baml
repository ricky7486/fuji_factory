   M S B A M L   `   `   `  ���� H  DPrinterCenter, Version=0.5.2.1, Culture=neutral, PublicKeyToken=null[2clr-namespace:PrinterCenter.UI.SharedFolderSetting$PrinterCenter.UI.SharedFolderSetting  O,clr-namespace:PrinterCenter.UI.CommonSettingPrinterCenter.UI.CommonSetting  @    :PrinterCenter.UI.SharedFolderSetting.ucSharedFolderSetting   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     Dlocal2clr-namespace:PrinterCenter.UI.SharedFolderSetting     FCommonSetting,clr-namespace:PrinterCenter.UI.CommonSetting     c _System.Windows.Interactivity, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35Bi:http://schemas.microsoft.com/expression/2010/interactivity  $
��Auto��$
��Auto��  � 	Resources  >   8PrinterCenter.UI.CommonSetting.BoolToVisibilityConverter   BoolToVisibilityConverter��%   &	           � DataContext @   :PrinterCenter.UI.SharedFolderSetting.SharedFolderSettingVM  .��� $
��Auto��$��10q� � RowDefinitions �� $��1* ��� $��3* ��� $��3* ��� $��1* �.+��� $��0�����	#FFBDC9D1$��2,2,2,0q�$��19�� �� CornerRadius$ 	17,17,0,0��._��� -    ��Name$ tbVendor��$��Center=�
���  ��$��Normal"�$��14(���Z� ���� *Vendor+ �� Mode$ OneWay=�]� -    ]�Name$ dpInSharedFolder��$��1��$
��Auto��$��10q�y��� *
IsInEnable+ �� 	Converter# [  $ TwoWay=�.+���   @RECIEVE#���  $	��100��$
��Left=�$��Center=��� $��Center=�$
��Left=�$	��200��$
��Wrap=�  @NETWORKDRIVE_MAPPING_DISK#���  �� $��40��$	��250��$��Right=�$��Right=� �� SelectedValue �� *InDriveInfo+$ TwoWay=����� *InSharedFolder+.  (System.Windows.Interactivity.Interaction	   Triggers	 /  )System.Windows.Interactivity.EventTrigger  
   	EventName$
 SelectionChanged��.  (System.Windows.Interactivity.TriggerBase   Actions. 6  0System.Windows.Interactivity.InvokeCommandAction     Command �� *InSFSelectionChangedCommand+]� -   $ dpOutSharedFolder��$��2��$
��Auto��$��10q�y��� *IsOutEnable+# [  $ TwoWay=�.+���  	 @SEND#���  $	��100��$
��Left=�$��Center=��� $��Center=�$
��Left=�$	��200��$
��Wrap=�#���  �� $��40��$	��250��$��Right=�$��Right=� �� *OutDriveInfo+$ TwoWay=����� *OutSharedFolder+	   $
 SelectionChanged��.    �� *OutSFSelectionChangedCommand+�� $��3�����	#FFBDC9D1$��2,0,2,2q�$��19��$ 	0,0,17,17��$
��Auto��$��0,0,0,10q�