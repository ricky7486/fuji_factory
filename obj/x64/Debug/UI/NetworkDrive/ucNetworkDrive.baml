   M S B A M L   `   `   `  ����H  DPrinterCenter, Version=0.5.2.1, Culture=neutral, PublicKeyToken=null3clr-namespace:PrinterCenter.UIPrinterCenter.UI  5      %    PrinterCenter.UI.ucNetworkDrive   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      0localclr-namespace:PrinterCenter.UI     5        � DataContext  5      ,   &PrinterCenter.UI.NetworkDriveViewModel  5   
   .��5      �  � RowDefinitions 5   
   �� 5      $��0.15* �6   �� 5      $
��0.1* �6   �� 5      $
��0.1* �6   �� 5      $
��0.1* �6   �� 5      $
��0.1* �6   �� 5      $
��0.1* �6   �� 5      $
��0.1* �6   �� 5      $
��0.1* �6   �� 5      $��0.15* �6    � ColumnDefinitions 5   
   �� 5      $
��0.5* �6   �� 5      $
��0.5* �6   .+�5!   
   �� $��1��6   $��2��6"   $��23��66       @CREATE_NETWORKDRIVE_MAPPING#���   6B   $��15,0,0,0q�6x   $
��Bold"�6�   �� 5$   
   $��2��6   $��0��6"    �� Orientation$ 
Horizontal=�62   $��20,5,0,5q�6K   .+�5%      �� $��23��6    
 @DRIVE#���  6'   �� 5&      $��: ��6   $��Center=�6"   5'      �� 5(   
   -   5         ��Name$ cbAvaiableDrives��5(   0   $��2��6   $��1��6    $��31��6J   $
��Left=�6V   $	��Top=�6s   $	��200��6�   $	��0,5q�6�    �� SelectedValue 5)      �� *NetDriveM.DriveInfo+ �� Mode$ TwoWay=��� 5+   
   $��3��6   $��0��6"   $ 
Horizontal=�62   $��20,5,0,5q�6K   .+�5,      �� $��23��6     @SHARED_FOLDER#���  6%   �� 5-      $��: ��6   $��Center=�6"   5/      �� 50   
   -   5         ��Name$ tbShareFolder��50   /   $��3��6   $��1��6   $��28��6F   $
��Left=�6R   $	��Top=�6n   $	��200��6�   $	��0,5q�51   F   ��6   �� *NetDriveM.ShareFolder+$ TwoWay=�6F   �� 54   
   $��4��6   $��1��6!   $�� ex: \\IP\Folder ��61   $��Center=�6J   �� 55   
   $��5��6   $��0��6"   $ 
Horizontal=�62   $��20,5,0,5q�6K   .+�56      �� $��23��6     
@USER_NAME#���  6%   �� 57      $��: ��6   $��Center=�6"   59      �� 5:   
   -   5        $ 
tbUserName��5:   0   $��5��6   $��1��6   $��28��6D   $
��Left=�6P   $	��Top=�6l   $	��200��6�   $	��0,5q�6�   ��5;      �� *NetDriveM.UserName+$ TwoWay=��� 5<   
   $��6��6   $��0��6"   $ 
Horizontal=�62   $
��Left=�6L   $��Center=�6g   $��20,5,0,5q�6�   .+�5=      �� $��23��6     	@PASSWORD#���  6%   �� 5>      $��: ��6   $��Center=�6"   5?      �� 5@   
   -   5        $ 
tbPassword��5@   /   $��6��6   $��1��6   $��28��6C   $
��Left=�6O   $	��Top=�6k   $	��200��6�   $	��0,5q�6�   ��5A      �� *NetDriveM.Password+$ TwoWay=��� 5B   
   $��7��6   $��0��6   $��40��6.   $	��100��6:   $
��Left=�6F   $��5q�6a     
@DIAGNOSIS#���  6l   ��5C      �� *	Diagnosis+�� 5E   
   $��7��6   $��1��6   $��40��6.   $	��100��6:   $��Right=�6F   $��5q�6b     @CREATE#���  6m   ��5F      �� *Create+5K      5L      