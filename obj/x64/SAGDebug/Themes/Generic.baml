   M S B A M L   `   `   `  ���� H  DPrinterCenter, Version=0.5.2.1, Culture=neutral, PublicKeyToken=nullI)clr-namespace:PrinterCenter.CustomControlPrinterCenter.CustomControl  l hSystem.Windows.Controls.DataVisualization.Toolkit, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null��clr-namespace:System.Windows.Controls.DataVisualization.Charting.Primitives;assembly=System.Windows.Controls.DataVisualization.Toolkit=System.Windows.Controls.DataVisualization.Charting.Primitives �rclr-namespace:System.Windows.Controls.DataVisualization;assembly=System.Windows.Controls.DataVisualization.Toolkit)System.Windows.Controls.DataVisualization R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0890    *PrinterCenter.CustomControl.StepTabControl  �� 
TargetType �� 	Resources   Key g� Duration �� TargetProperty t� From	 t� To g� DecelerationRatio W� From	 W� To	 g� AccelerationRatio
 �� Property �� Value �� 
TargetType �Name J� SnapsToDevicePixels-  'System.Windows.Input.KeyboardNavigation   TabNavigation � ColumnDefinitions � RowDefinitions ��Name K� IsItemsHost    	TabMargin   TabIndex K� ZIndex ��Name   DirectionalNavigation ��Name �� SelectedContentTemplate �� SelectedContent" �� SelectedContentStringFormat �� ContentStringFormat �� Triggers P� Property  P� Value! �� 
TargetName" �� StrokeDashArray# �� EndPoint$ �� 
StartPoint% � Color& � Offset' �� RelativeSource( �� AncestorType) �� Stretch8   2PrinterCenter.CustomControl.ScrollableCategoryAxisD  >System.Windows.Controls.DataVisualization.Charting.DisplayAxis5  /System.Windows.Controls.DataVisualization.Title*  Name+   
TitleStyle�� F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     :x,http://schemas.microsoft.com/winfx/2006/xaml     3local)clr-namespace:PrinterCenter.CustomControl  �chartingprimitives�clr-namespace:System.Windows.Controls.DataVisualization.Charting.Primitives;assembly=System.Windows.Controls.DataVisualization.Toolkit  ~datavisrclr-namespace:System.Windows.Controls.DataVisualization;assembly=System.Windows.Controls.DataVisualization.Toolkit  %�  '         '  a    �� "     ���� SlideAndFadeRightIn  .��t� $ 0:0:.75D�$ Margin�$ -500,0,500,0X�$ 0X�$ .9Q�W� $ 0:0:.25D�$ Opacity�$ 0X�$ 1X��� SlideAndFadeLeftIn  .��t� $ 0:0:.75D�$ Margin�$ 500,0,-500,0X�$ 0X�$ .9Q�W� $ 0:0:.25D�$ Opacity�$ 0X�$ 1X��� Fade  .��W� $ 0:0:.25D�$ Opacity�$ 0X�$ 1X��� 	SlideLeft  .��t� $ 0:0:.75D�$ Margin�$ 500,0,-500,0X�$ 0X�$ .9Q��� 
SlideRight  .��t� $ 0:0:.75D�$ Margin�$ -500,0,500,0X�$ 0X�$	 .9Q�.��� 
 � ��$ 2q��� 
 � ��$ Center=��� 
 � ��$ Center=��� 
 � ��
 ������� 
 � �� �	#FFACACAC�� 
 � ��$ 1q��� 
 � ��# � ���� 
 � �� �� "   .R�� $ PART_GridRoot��}�.  . $ Local=� �� �� $��0 � �� $
��Auto ��� $��* �.+��� $ PART_Header��
������ $��0�� . #��z $��0��$ 1��$ 1���� $ PART_AnimationHost��#��z��#��z��#��z��$��0��$ 	Contained=�$��1��$ 2��$ Local=�._��� $ PART_SelectedContentHost��#��z #��z # z $��SelectedContent��#��z��# z  P�  � {�  .  . ��� $! PART_GridRoot��# � ��
 � ���� 
 � �� �� "  ��.��� 
 �@�FocusVisualStyle �� .��� 
 � �� �� .R��� $��2q� . #��� ��$��1��$	" 1 2R��� 
 � ��
 �   ��� 
 � �� �� $	# 0,1&�$	$ 0,0&�.��� $% 	#FFF0F0F0��$& 0Q�� $% 	#FFE5E5E5��$& 1Q��� 
 � �� �	#FFACACAC�� 
 � ��$ 0q��� 
 � ��$	 6,2q��� 
 � ��$ Stretch=��� 
 � ��$ Stretch=��� 
 � �� �� " ��.R�� $��TabHost������ *	TabHeight+' �� $( local:StepTabControlL����� *
TabWidth+' �� $( local:StepTabControlL� �� $��0.15* ��� $
��0.7* ��� $��0.15* �.+�F� $��TabArrowHead��4����        р��     р�� ����    ���р�� ��� 	$
) Fill=�$��0������ *TabBackground+' �� $( local:StepTabControlL�� $��1��.+��� $��TabArrowBody��$
) Fill=����� *TabBackground+' �� $( local:StepTabControlL��� $��HeaderArrow��#��z��$��Center=����� *TabTextColor+' �� $( local:StepTabControlL�F� $��TabArrowTail��"����        р�� ��� �    ���	$
) Fill=�$��2������ *TabBackground+' �� $( local:StepTabControlL� P�  �@��
IsSelected  . . ��� $! HeaderArrow��
 � ��$
 Bold"��� $! HeaderArrow�� �� *SelectedTabTextColor+' �� $( local:StepTabControlL�
 � ���� $! TabArrowHead�� �� *SelectedTabBackground+' �� $( local:StepTabControlL�
 � ���� $! TabArrowBody�� �� *SelectedTabBackground+' �� $( local:StepTabControlL�
 � ���� $! TabArrowTail�� �� *SelectedTabBackground+' �� $( local:StepTabControlL�
 � ��P�  � {�  .  . ��� $! HeaderArrow��
 � ��
 ������� "   .��� 
 � �� .  �� 
 �@ 
TitleStyle �� "   .��� 
 � ��$ Italic$��� 
 �@ MajorTickMarkStyle �� "  ��.��� 
 � ��
 �   ��� 

 �@��X2$ 4���� 

 �@��Y2$ 4���� 
 �@ GridLineStyle �� "  ��.��� 
 � ��
 ������� 
 � �� �� "  .R��� .+�� $ AxisGrid��#��z��.+�  $* 	AxisTitle��#��z+ 