   M S B A M L   `   `   `  ���� H  DPrinterCenter, Version=0.5.2.1, Culture=neutral, PublicKeyToken=null?$clr-namespace:PrinterCenter.UI.ChartPrinterCenter.UI.Chart  l hSystem.Windows.Controls.DataVisualization.Toolkit, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null�{clr-namespace:System.Windows.Controls.DataVisualization.Charting;assembly=System.Windows.Controls.DataVisualization.Toolkit2System.Windows.Controls.DataVisualization.Charting �rclr-namespace:System.Windows.Controls.DataVisualization;assembly=System.Windows.Controls.DataVisualization.Toolkit)System.Windows.Controls.DataVisualization I)clr-namespace:PrinterCenter.CustomControlPrinterCenter.CustomControl  (    "PrinterCenter.UI.Chart.ucChartHost   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     6local$clr-namespace:PrinterCenter.UI.Chart     �chartingToolkit{clr-namespace:System.Windows.Controls.DataVisualization.Charting;assembly=System.Windows.Controls.DataVisualization.Toolkit     �datavisrclr-namespace:System.Windows.Controls.DataVisualization;assembly=System.Windows.Controls.DataVisualization.Toolkit     CcustomControl)clr-namespace:PrinterCenter.CustomControl     $
��Auto��$
��Auto��  � DataContext  �� *ChartHostVM+   Locator �� Source# [  .���  � RowDefinitions �� $
��0.5* ��� $��5* ��� $��5* ��� $��5* �.+��� $��0�� �� Orientation$ 
Horizontal=�$��Center=�.+���   @DISPLAY_HISTORY_STATISTIC#��[ �� -    ��Name$ cbDisplaySelector��$	��200��$��10,0,0,0q�>  8System.Windows.Controls.DataVisualization.Charting.Chart  -     Name$ chartDx��   Title$ Dx��
������ $��0q�$��Stretch=�$��1��   
TitleStyle ��  �� 
TargetType5  /System.Windows.Controls.DataVisualization.Title"  .��� 	 �� Property	 � ��
 �� Value$
 Center=��� 	 � ��

 �   �   LegendStyle �� 6  0System.Windows.Controls.DataVisualization.Legend"  .��� 	 � ��$
 0��   Series C  =System.Windows.Controls.DataVisualization.Charting.LineSeries  H  BSystem.Windows.Controls.DataVisualization.Charting.DataPointSeries   DependentValuePath$ Value��   IndependentValuePath$	 Key��   IsSelectionEnabled .    ItemsSource �� *Dx+�  �System.Windows.Controls.DataVisualization.Charting.LineAreaBaseSeries`1[[System.Windows.Controls.DataVisualization.Charting.LineDataPoint, System.Windows.Controls.DataVisualization.Toolkit, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null]]   IndependentAxis C  =System.Windows.Controls.DataVisualization.Charting.LinearAxis  =  7System.Windows.Controls.DataVisualization.Charting.Axis   Orientation$ X=�D	  >System.Windows.Controls.DataVisualization.Charting.DisplayAxis 	  Title$ X�� 	  ShowGridLines . D
  >System.Windows.Controls.DataVisualization.Charting.NumericAxis 
  Maximum�  �System.Windows.Controls.DataVisualization.Charting.NullableConverter`1[[System.Double, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]$	 100  
  Minimum$ 0    DependentRangeAxis   $ Y=�$ Y�� . $	 100 $
 -100   -   $ chartDy��$ Dy��
������ $��0q�$��Stretch=�$��2�� �� "  .��� 	 � ��$
 Center=��� 	 � ��

 �   � �� "  .��� 	 � ��$
 0��   $ Value��$	 Key�� .  �� *Dy+   $ X=�$ X�� . $	 100 $ 0    $ Y=�$ Y�� . $	 100 $
 -100   -   $ 
chartTheta��$ Theta��
������ $��0q�$��Stretch=�$��3�� �� "  .��� 	 � ��$
 Center=��� 	 � ��

 �   � �� "  .��� 	 � ��$
 0��   $ Value��$	 Key�� .  �� *Theta+   $ X=�$ X�� . $	 100 $ 0    $ Y=�$ Y�� . $	 100 $
 -100 