   M S B A M L   `   `   `  ����H  DPrinterCenter, Version=0.5.2.1, Culture=neutral, PublicKeyToken=nullI)clr-namespace:PrinterCenter.CustomControlPrinterCenter.CustomControl  5      l hSystem.Windows.Controls.DataVisualization.Toolkit, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null��clr-namespace:System.Windows.Controls.DataVisualization.Charting.Primitives;assembly=System.Windows.Controls.DataVisualization.Toolkit=System.Windows.Controls.DataVisualization.Charting.Primitives 5      �rclr-namespace:System.Windows.Controls.DataVisualization;assembly=System.Windows.Controls.DataVisualization.Toolkit)System.Windows.Controls.DataVisualization 5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0890    *PrinterCenter.CustomControl.StepTabControl  �� 
TargetType �� 	Resources   Key g� Duration �� TargetProperty t� From	 t� To g� DecelerationRatio W� From	 W� To	 g� AccelerationRatio
 �� Property �� Value �� 
TargetType �Name J� SnapsToDevicePixels-  'System.Windows.Input.KeyboardNavigation   TabNavigation � ColumnDefinitions � RowDefinitions ��Name K� IsItemsHost    	TabMargin   TabIndex K� ZIndex ��Name   DirectionalNavigation ��Name �� SelectedContentTemplate �� SelectedContent" �� SelectedContentStringFormat �� ContentStringFormat �� Triggers P� Property  P� Value! �� 
TargetName" �� StrokeDashArray# �� EndPoint$ �� 
StartPoint% � Color& � Offset' �� RelativeSource( �� AncestorType) �� Stretch8   2PrinterCenter.CustomControl.ScrollableCategoryAxisD  >System.Windows.Controls.DataVisualization.Charting.DisplayAxis5  /System.Windows.Controls.DataVisualization.Title*  Name+   
TitleStyle�� 5      F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     :x,http://schemas.microsoft.com/winfx/2006/xaml     5      3local)clr-namespace:PrinterCenter.CustomControl  5      �chartingprimitives�clr-namespace:System.Windows.Controls.DataVisualization.Charting.Primitives;assembly=System.Windows.Controls.DataVisualization.Toolkit  5      ~datavisrclr-namespace:System.Windows.Controls.DataVisualization;assembly=System.Windows.Controls.DataVisualization.Toolkit  5      %�  '         '  �    �� 5
      "    6    5   
   ���� 5      SlideAndFadeRightIn  .��5      t� $ 0:0:.75D�6%   $ Margin�68   $ -500,0,500,0X�6[   $ 0X�6o   $ .9Q�6v   W� 5      $ 0:0:.25D�6"   $ Opacity�65   $ 0X�6Y   $ 1X�6b   5      �� 5      SlideAndFadeLeftIn  .��5      t� $ 0:0:.75D�6%   $ Margin�68   $ 500,0,-500,0X�6[   $ 0X�6o   $ .9Q�6v   W� 5      $ 0:0:.25D�6"   $ Opacity�65   $ 0X�6Y   $ 1X�6b   5      �� 5      Fade  .��5      W� $ 0:0:.25D�6"   $ Opacity�65   $ 0X�6Y   $ 1X�6b   5      �� 5       	SlideLeft  .��5!      t� $ 0:0:.75D�6%   $ Margin�68   $ 500,0,-500,0X�6[   $ 0X�6o   $ .9Q�6v   5"      �� 5$      
SlideRight  .��5%      t� $ 0:0:.75D�6%   $ Margin�68   $ -500,0,500,0X�6[   $ 0X�6o   $	 .9Q�6v   5&      5'      .�5+   
   �� 
 � ��$ 2q�6$   �� 5,   
   
 � ��$ Center=�67   �� 5-   
   
 � ��$ Center=�65   �� 5.   
   
 � ��
 �����6'   �� 50   
   
 � �� �	#FFACACAC6(   �� 51   
   
 � ��$ 1q�6,   �� 52   
   
 � ��# � ��6'   �� 54   
   
 � �� 55      �� 56      "   6"   .R�58      � $ PART_GridRoot��6   }�.  . $ Local=�6b    5:      �� 5;      �� 5<      $��0 �60    5>      �� 5?      $
��Auto �6-   �� 5@      $��* �6-   .+�5B      �� $ PART_Header��6$   
������ $��0��5C   $    . #��z 5D   $   $��0��5F   $   $ 1��61   $ 1��6Q   �� 5H      $ PART_AnimationHost��6!   #��z��6=   #��z��6i   #��z��6�   $��0��5I   !   $ 	Contained=�61   $��1��6f   $ 2��6s   $ Local=�6�   ._�5K      �� $ PART_SelectedContentHost��6/   #��z 6Q   #��z 6�   # z 6�   $��SelectedContent��6�   #��z��6  # z 6@  5L      5M       5O      P� 5P       � {�  .  . �5Q      �� $! PART_GridRoot��6G   # � ��6b   
 � ��5R      5U      5X      �� 5Z   
   
 � �� 5[      �� 5\      "  ��6   .�5^      �� 
 �@�FocusVisualStyle 5_      �� 5`      .�5a   "   �� 
 � �� 5b   &   �� 5c   *   .R�5d   .   �� $��2q�68    . #��� ��6^   $��1��6�   $	" 1 2R�6�   5e   +   5g   #   5h      5j      �� 5k      
 � ��
 �   �63   �� 5l      
 � �� 5m      �� 5n      $	# 0,1&�62   $	$ 0,0&�6A   .��5o   "   � $% 	#FFF0F0F0��6/   $& 0Q�6A   � 5p   "   $% 	#FFE5E5E5��6/   $& 1Q�6A   5q      5s      �� 5t      
 � �� �	#FFACACAC64   �� 5u      
 � ��$ 0q�6/   �� 5v      
 � ��$	 6,2q�60   �� 5w      
 � ��$ Stretch=�6C   �� 5x      
 � ��$ Stretch=�6A   �� 5y      
 � �� 5z      �� 5{      " ��6.   .R�5~   &   � $��TabHost��6+   ��5   +   �� *	TabHeight+' �� $( local:StepTabControlL���5�   +   �� *
TabWidth+' �� $( local:StepTabControlL� 5�   &   �� 5�   *   $��0.15* �6;   6K   �� 5�   *   $
��0.7* �6;   6J   �� 5�   *   $��0.15* �6;   6K   .+�5�   &   F� $��TabArrowHead��6+   4����        р��     р�� ����    ���р�� ��� 	$
) Fill=�5�   +   $��0��5�   +   ��5�   a   �� *TabBackground+' �� $( local:StepTabControlL�5�   =   � 5�   &   $��1��6+   .+�5�   *   �� $��TabArrowBody��64   $
) Fill=�5�   4   ��5�   H   �� *TabBackground+' �� $( local:StepTabControlL�5�   F   �� 5�   *   $��HeaderArrow��64   #��z��6G   $��Center=�6g   ��6�   �� *TabTextColor+' �� $( local:StepTabControlL�5�   '   F� 5�   &   $��TabArrowTail��6,   "����        р�� ��� �    ���	$
) Fill=�5�   ,   $��2��5�   ,   ��5�   W   �� *TabBackground+' �� $( local:StepTabControlL�5�   >   5�   #    5�   "   P� 5�   &    �@��
IsSelected  . . �5�   *   �� $! HeaderArrow��61   
 � ��$
 Bold"�6`   �� 5�   *   $! HeaderArrow��61    6`   �� *SelectedTabTextColor+' �� $( local:StepTabControlL�
 � ���� 5�   *   $! TabArrowHead��61    6[   �� *SelectedTabBackground+' �� $( local:StepTabControlL�
 � ���� 5�   *   $! TabArrowBody��61    6[   �� *SelectedTabBackground+' �� $( local:StepTabControlL�
 � ���� 5�   *   $! TabArrowTail��61    6[   �� *SelectedTabBackground+' �� $( local:StepTabControlL�
 � ��5�   '   P� 5�   &    � {�  .  . �5�   *   �� $! HeaderArrow��61   
 � ��
 �����6`   5�   '   5�      5�      5�      5�      5�      �� 5�      "   6   .�5�   
   �� 
 � �� .  6&   �� 5�   
   
 �@ 
TitleStyle 5�      �� 5�      "   6   .�5�      �� 
 � ��$ Italic$�62   5�      5�      �� 5�   
   
 �@ MajorTickMarkStyle 5�      �� 5�      "  ��6   .�5�      �� 
 � ��
 �   �6/   �� 5�      

 �@��X2$ 4��6+   �� 5�      

 �@��Y2$ 4��6+   5�      5�      �� 5�   
   
 �@ GridLineStyle 5�      �� 5�      "  ��6   .�5�      �� 
 � ��
 �����6/   5�      5�      �� 5�   
   
 � �� 5�      �� 5�      "  6"   .R�5�      �� .+�5�      � $ AxisGrid��6   #��z��61   .+�5�        $* 	AxisTitle��6,   #��z+ 6?   5�      5�      5�      5�      5�      5�      