   M S B A M L   `   `   `  ���� H  DPrinterCenter, Version=0.5.2.1, Culture=neutral, PublicKeyToken=null3clr-namespace:PrinterCenter.UIPrinterCenter.UI  %    PrinterCenter.UI.ucNetworkDrive   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     0localclr-namespace:PrinterCenter.UI       � DataContext  ,   &PrinterCenter.UI.NetworkDriveViewModel  .���  � RowDefinitions �� $��0.15* ��� $
��0.1* ��� $
��0.1* ��� $
��0.1* ��� $
��0.1* ��� $
��0.1* ��� $
��0.1* ��� $
��0.1* ��� $��0.15* � � ColumnDefinitions �� $
��0.5* ��� $
��0.5* �.+��� $��1��$��2��$��23��    @CREATE_NETWORKDRIVE_MAPPING#���   $��15,0,0,0q�$
��Bold"��� $��2��$��0�� �� Orientation$ 
Horizontal=�$��20,5,0,5q�.+��� $��23�� 
 @DRIVE#���  �� $��: ��$��Center=��� -    ��Name$ cbAvaiableDrives��$��2��$��1��$��31��$
��Left=�$	��Top=�$	��200��$	��0,5q� �� SelectedValue �� *NetDriveM.DriveInfo+ �� Mode$ TwoWay=��� $��3��$��0��$ 
Horizontal=�$��20,5,0,5q�.+��� $��23��  @SHARED_FOLDER#���  �� $��: ��$��Center=��� -    ��Name$ tbShareFolder��$��3��$��1��$��28��$
��Left=�$	��Top=�$	��200��$	��0,5q����� *NetDriveM.ShareFolder+$ TwoWay=��� $��4��$��1��$�� ex: \\IP\Folder ��$��Center=��� $��5��$��0��$ 
Horizontal=�$��20,5,0,5q�.+��� $��23��  
@USER_NAME#���  �� $��: ��$��Center=��� -   $ 
tbUserName��$��5��$��1��$��28��$
��Left=�$	��Top=�$	��200��$	��0,5q����� *NetDriveM.UserName+$ TwoWay=��� $��6��$��0��$ 
Horizontal=�$
��Left=�$��Center=�$��20,5,0,5q�.+��� $��23��  	@PASSWORD#���  �� $��: ��$��Center=��� -   $ 
tbPassword��$��6��$��1��$��28��$
��Left=�$	��Top=�$	��200��$	��0,5q����� *NetDriveM.Password+$ TwoWay=��� $��7��$��0��$��40��$	��100��$
��Left=�$��5q�  
@DIAGNOSIS#���  ���� *	Diagnosis+�� $��7��$��1��$��40��$	��100��$��Right=�$��5q�  @CREATE#���  ���� *Create+