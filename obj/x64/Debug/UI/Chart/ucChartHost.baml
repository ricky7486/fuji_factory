   M S B A M L   `   `   `  ����H  DPrinterCenter, Version=0.5.2.1, Culture=neutral, PublicKeyToken=null?$clr-namespace:PrinterCenter.UI.ChartPrinterCenter.UI.Chart  5      l hSystem.Windows.Controls.DataVisualization.Toolkit, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null�{clr-namespace:System.Windows.Controls.DataVisualization.Charting;assembly=System.Windows.Controls.DataVisualization.Toolkit2System.Windows.Controls.DataVisualization.Charting 5      �rclr-namespace:System.Windows.Controls.DataVisualization;assembly=System.Windows.Controls.DataVisualization.Toolkit)System.Windows.Controls.DataVisualization 5	      I)clr-namespace:PrinterCenter.CustomControlPrinterCenter.CustomControl  5
      (    "PrinterCenter.UI.Chart.ucChartHost   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      6local$clr-namespace:PrinterCenter.UI.Chart     5      �chartingToolkit{clr-namespace:System.Windows.Controls.DataVisualization.Charting;assembly=System.Windows.Controls.DataVisualization.Toolkit     5      �datavisrclr-namespace:System.Windows.Controls.DataVisualization;assembly=System.Windows.Controls.DataVisualization.Toolkit     5	      CcustomControl)clr-namespace:PrinterCenter.CustomControl     5
      $
��Auto��5      $
��Auto��6     � DataContext  5      �� *ChartHostVM+   Locator �� Source# [  .��5      �  � RowDefinitions 5   
   �� 5      $
��0.5* �6   �� 5      $��5* �6   �� 5      $��5* �6   �� 5      $��5* �6   .+�5   
   �� $��0��6    �� Orientation$ 
Horizontal=�6"   $��Center=�6;   .+�5      ��   @DISPLAY_HISTORY_STATISTIC#��[ 6   6M   �� 5      -   5         ��Name$ cbDisplaySelector��5      $	��200��62   $��10,0,0,0q�6>   6�   5      >  8System.Windows.Controls.DataVisualization.Charting.Chart  5   
   -   5          Name$ chartDx��5          Title$ Dx��61   
������ $��0q�6W   $��Stretch=�6k   $��1��5          
TitleStyle 5      �� 5        �� 
TargetType5  /System.Windows.Controls.DataVisualization.Title"  6   .�5!      �� 	 �� Property	 � ��
 �� Value$
 Center=�6<   �� 5"      	 � ��

 �   �63   5#         LegendStyle 5%      �� 5&      6  0System.Windows.Controls.DataVisualization.Legend"  6   .�5'      �� 	 � ��$
 0��6.   5(         Series 53      C  =System.Windows.Controls.DataVisualization.Charting.LineSeries  H  BSystem.Windows.Controls.DataVisualization.Charting.DataPointSeries   DependentValuePath$ Value��54         IndependentValuePath$	 Key��55         IsSelectionEnabled .    ItemsSource 56      �� *Dx+�  �System.Windows.Controls.DataVisualization.Charting.LineAreaBaseSeries`1[[System.Windows.Controls.DataVisualization.Charting.LineDataPoint, System.Windows.Controls.DataVisualization.Toolkit, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null]]   IndependentAxis 58      C  =System.Windows.Controls.DataVisualization.Charting.LinearAxis  59      =  7System.Windows.Controls.DataVisualization.Charting.Axis   Orientation$ X=�65   D	  >System.Windows.Controls.DataVisualization.Charting.DisplayAxis 	  Title$ X��5:   #    	  ShowGridLines . D
  >System.Windows.Controls.DataVisualization.Charting.NumericAxis 
  Maximum�  �System.Windows.Controls.DataVisualization.Charting.NullableConverter`1[[System.Double, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]$	 100 5<   #    
  Minimum$ 0 5=   #      DependentRangeAxis 5?        5@      $ Y=�65   $ Y��5A   #    . $	 100 5C   #   $
 -100 5D   #   5F      5J        5L   
   -   5        $ chartDy��5L       $ Dy��61   
������ $��0q�6W   $��Stretch=�6k   $��2��5M        5O      �� 5P      "  6   .�5Q      �� 	 � ��$
 Center=�6<   �� 5R      	 � ��

 �   �63   5S       5U      �� 5V      "  6   .�5W      �� 	 � ��$
 0��6.   5X       5c        $ Value��5d      $	 Key��5e       .  5f      �� *Dy+ 5h        5i      $ X=�65   $ X��5j   #    . $	 100 5l   #   $ 0 5m   #    5o        5p      $ Y=�65   $ Y��5q   #    . $	 100 5s   #   $
 -100 5t   #   5v      5x        5z   
   -   5        $ 
chartTheta��5z       $ Theta��64   
������ $��0q�6]   $��Stretch=�6q   $��3��5{        5}      �� 5~      "  6   .�5      �� 	 � ��$
 Center=�6<   �� 5�      	 � ��

 �   �63   5�       5�      �� 5�      "  6   .�5�      �� 	 � ��$
 0��6.   5�       5�        $ Value��5�      $	 Key��5�       .  5�      �� *Theta+ 5�        5�      $ X=�65   $ X��5�   #    . $	 100 5�   #   $ 0 5�   #    5�        5�      $ Y=�65   $ Y��5�   #    . $	 100 5�   #   $
 -100 5�   #   5�      5�      5�      5�      