   M S B A M L   `   `   `  ���� H  DPrinterCenter, Version=0.5.2.1, Culture=neutral, PublicKeyToken=nullO,clr-namespace:PrinterCenter.UI.CommonSettingPrinterCenter.UI.CommonSetting  O Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem 4    .PrinterCenter.UI.CommonSetting.ucCommonSetting   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     >local,clr-namespace:PrinterCenter.UI.CommonSetting     c _Microsoft.Expression.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35=ed4http://schemas.microsoft.com/expression/2010/drawing  3System&clr-namespace:System;assembly=mscorlib  $
��Auto��$
��Auto��  � DataContext  4   .PrinterCenter.UI.CommonSetting.CommonSettingVM   � 	Resources    dataMatchingBasisEnum S� 
MethodName S� 
ObjectType  System.Enum S� MethodParameters M� TypeName>   8PrinterCenter.UI.CommonSetting.BoolToVisibilityConverter  BoolToVisibilityConverter  OnMouseEnter_Board �� TargetProperty �� 
TargetName9  3System.Windows.Media.Animation.EasingDoubleKeyFrame O� KeyTime	 O� Value  OnMouseEnter_Stencil��%M  &	        &	 >     &	 C     &	 7    S� $ 	GetValues��"   M� $ local:eMatchingBasis��  �� .��U� $U O(UIElement.RenderTransform).(TransformGroup.Children)[3].(TranslateTransform.X)�$ imgBoard��.\�  $ 0:0:0.25��$	 -74.013Q�  $ 0:0:0.5��$	 0.975Q�  $ 0:0:0.75��$	 75.833Q�  $ 0:0:1��$	 -0.05Q��� .��U� $U O(UIElement.RenderTransform).(TransformGroup.Children)[3].(TranslateTransform.X)�$ 
imgStencil��.\�  $ 0:0:0.25��$	 -74.704Q�  $ 0:0:0.5��$	 0Q�  $ 0:0:0.75��$	 75.371Q�  $ 0:0:1��$	 0Q�
 � Triggers
 :�  :� RoutedEvent$ ToggleButton.Click�� :� 
SourceName$ rbMoveStencil��.[��� -    ��Name$* $OnMouseEnter_Stencil_BeginStoryboard��#��[ :� $ ToggleButton.Click��$ rbMoveBoard��.[��� -   $( "OnMouseEnter_Board_BeginStoryboard��#��[ .���  � RowDefinitions �� $��1* ��� $��1* ��� $��1* ��� $��1* ��� $��1* �.+���  �� Orientation$ 
Horizontal=�$��10q�$��Center=�.+���   @MATCHING_BASIS#���  �� $��23��$	��200��$��10,0,0,0q� �� SelectedValue �� *MatchingBasis+ �� Mode$ TwoWay=�����  �� Source# [  9� $��1��  @PRINTER_ADJUST_METHOD#���  $
��Auto��
������ .���� $ Vertical=�
������ .+�	� -    	�Name$ rbMoveStencil��$
��Auto��$
��Auto�� d� 	IsChecked �� *IsMoveStencil+.���  �� $��2* ��� $��8* � � ColumnDefinitions �� $	��150 ��� $	��150 ��� $	��150 �.+���   @MOVE_STENCIL#���  $��0��$��3���� -    ��Name$ 
imgStencil��$	��150��$��images/Stencil.png�� J� RenderTransformOrigin$	 1,1&�$��50��$��1��$��1��z�V� .~��� �� �� T� 	� -   $ rbMoveBoard��$
��Auto��$
��Auto�� �� *	IsMovePCB+.���  �� $��2* ��� $��8* � �� $	��150 ��� $	��150 ��� $	��150 �.+���   	@MOVE_PCB#���  $��0��$��3���� -   $ imgBoard��$	��150��$��images/Board.png��$	 1,1&�$��50��$��1��$��1��z�V� .~��� �� �� T� 9� $��2��  @ROTATE_DIRECTION#���  $
��Auto��
������ .���� $ 
Horizontal=�
������ $��Center=�$��Center=�.+�	� $��20q� �� *
IsCWRotate+.���  �� $��2* ��� $��8* �.+��� $��0�� 	 
@CLOCKWISE#��� 	 �� $��80��$��80��$��1��$
��Left=�.+��� $��5��	 �� Y1$ 80��
���   ��� $��5��	 �� X1$ 0��$ 80��	 �� X2$ 80��	 �� Y2$ 80��
���   �-  'Microsoft.Expression.Controls.LineArrow  $��80��2  ,Microsoft.Expression.Controls.CompositeShape   Stroke
 �  ��   StrokeThickness$ 5Q�$��80���� $��θ��$��	10,50,0,0q�
���  ��	� $��20q� �� *IsCCWRotate+.���  �� $��2* ��� $��8* �.+��� $��0�� 
 @COUNTER_CLOCKWISE#��� 
 �� $��80��$��80��$��1��$
��Left=�.+��� $��5��$ 80��
���   ��� $��5��$ 0��$ 80��$ 80��$ 80��
���   �  $��80��
 �  ��$ 5Q�$��80��$ 0.5,0.5&�   StartCorner$ 
BottomLeft=�z�V� .~���  �� ScaleY$ 1Q�  �� ScaleX$  -1Q��� ! �� AngleY$! 0Q�" �� AngleX$" 0Q��� # �� Angle$# 0Q�T� �� $��θ��$��	10,50,0,0q�
���  ��9� $��3��  	@QUADRENT#���  $
��Auto��$
��Auto��
������ .��� $	��200��$	��200��$��10q� �� $��1* ��� $��1* � �� $��1* ��� $��1* �.+�	� -   $
 rbQ2��$��0��$��0��$��Center=�$��Center=�$��II�� �� *IsQuadrent2+	� -   $
 rbQ1��$��0��$��1��$��Center=�$��Center=�$��I�� �� *IsQuadrent1+	� -	   $
 rbQ3��$��1��$��0��$��Center=�$��Center=�$	��III�� �� *IsQuadrent3+	� -
   $
 rbQ4��$��1��$��1��$��Center=�$��Center=�$��IV�� �� *IsQuadrent4+�� $	��200��$	��200��$��0��$��2��$��0��$��2��.+�,  &Microsoft.Expression.Shapes.BlockArrow  
$ �� Top$$ 95��$	��200��$��10��% �� Left$% 0��&   Orientation$& Right=�
���   �$��5��  $$ 0��$��10��$	��200��$% 95��$& Up=�
���   �$��5���� $��images/PCBsmall.png��$��40��$��30��$$ 65��$% 55��y��� *	IsChecked+' �� 	Converter#' [ ( �� ElementName$
( rbQ2���� $��images/PCBsmall.png��$��40��$��30��$$ 65��$	% 105��y��� *	IsChecked+#' [ $
( rbQ1���� $��images/PCBsmall.png��$��40��$��30��$	$ 105��$% 55��y��� *	IsChecked+#' [ $
( rbQ3���� $��images/PCBsmall.png��$��40��$��30��$	$ 105��$	% 105��y��� *	IsChecked+#' [ $
( rbQ4��9� $��4��  @CANDIDATE_FILTER#���  $
��Auto��$
��Auto��
������ .��� 
������� �� $��1* ��� $��1* ��� $��1* � �� $��1* ��� $��3* �.+���   @HEIGHT#���  $��0��$��0��$��Center=� �� *IsHeightFilter+�� $��0��$��1��$ 
Horizontal=�$��4q�.+��� $��images/ArrowX.png��$��30��$ 0.5,0.5&�z�V� .~��� $ 1Q�$  -1Q��� $! 0Q�$" 0Q��� $# 0Q�T� �� $��30��$��60��$��Center=����� *HeightRange.LowerBound+�� $��images/CenterV.png��$��30���� $��30��$��60��$��Center=����� *HeightRange.UpperBound+�� $��images/ArrowX.png��$��30����  	 @AREA#���  $��1��$��0��$��Center=� �� *IsAreaFilter+�� $��1��$��1��$ 
Horizontal=�$��4q�.+��� $��images/ArrowX.png��$��30��$ 0.5,0.5&�z�V� .~��� $ 1Q�$  -1Q��� $! 0Q�$" 0Q��� $# 0Q�T� �� $��30��$��60��$��Center=����� *AreaRange.LowerBound+�� $��images/CenterV.png��$��30���� $��30��$��60��$��Center=����� *AreaRange.UpperBound+�� $��images/ArrowX.png��$��30����   @VOLUME#���  $��2��$��0��$��Center=� �� *IsVolumeFilter+�� $��2��$��1��$ 
Horizontal=�$��4q�.+��� $��images/ArrowX.png��$��30��$ 0.5,0.5&�z�V� .~��� $ 1Q�$  -1Q��� $! 0Q�$" 0Q��� $# 0Q�T� �� $��30��$��60��$��Center=����� *VolumeRange.LowerBound+�� $��images/CenterV.png��$��30���� $��30��$��60��$��Center=����� *VolumeRange.UpperBound+�� $��images/ArrowX.png��$��30��