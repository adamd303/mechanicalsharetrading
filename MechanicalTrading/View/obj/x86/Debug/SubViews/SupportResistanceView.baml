   M S B A M L   `   `   `  ����W  SAdam.Trading.Mechanical.View, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nulla5clr-namespace:Adam.Trading.Mechanical.View.Properties'Adam.Trading.Mechanical.View.Properties  5      \ XAdam.Trading.Mechanical.ViewModel, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null�Zclr-namespace:Adam.Trading.Mechanical.ViewModel;assembly=Adam.Trading.Mechanical.ViewModel!Adam.Trading.Mechanical.ViewModel 5	      [ WAdam.Trading.Mechanical.ViewUtil, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null}Xclr-namespace:Adam.Trading.Mechanical.ViewUtil;assembly=Adam.Trading.Mechanical.ViewUtil Adam.Trading.Mechanical.ViewUtil 5
      A    ;Adam.Trading.Mechanical.View.SubViews.SupportResistanceView   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Fprop5clr-namespace:Adam.Trading.Mechanical.View.Properties     5      p	viewModelZclr-namespace:Adam.Trading.Mechanical.ViewModel;assembly=Adam.Trading.Mechanical.ViewModel     5	      mviewUtilXclr-namespace:Adam.Trading.Mechanical.ViewUtil;assembly=Adam.Trading.Mechanical.ViewUtil     5
      $	��200��5      $	��400��5      $	��400��5      $	��800��5      $��3q�5        � 	Resources  5         supportResistanceViewModel S� 
ObjectTypeB  <Adam.Trading.Mechanical.ViewModel.SupportResistanceViewModel S� ConstructorParameters7  1Adam.Trading.Mechanical.ViewUtil.FilePathProvider��%F   &	        S� 5   
   "  6@    5        5      5      5      .��5      �� .+�5   
   � $
��Auto��6    � RowDefinitions 5      �� 5      $��7* �6    �� 5      $	��30* �6    �� 5      $	��30* �6    �� 5      $	��30* �6    �� 5      $	��30* �6    �� 5       $	��30* �6    �� 5!      $	��30* �6    �� 5"      $	��30* �6    �� 5#      $��7* �6     � ColumnDefinitions 5%      �� 5&      $��7* �6#   �� 5'      $
��200* �6#   �� 5(      $
��200* �6#   �� 5)      $��7* �6#   .+�5+      �� $��1��6   $��1��6!   $
��Left=�5,      $��Center=�5-      7   1Adam.Trading.Mechanical.View.Properties.Resources    SupportResistanceLongTerm#��Z 5.      $
��Left=�5/      $	��100��50      $	��400��51      �� 52      $��1��6   $��2��6#   $	��100��53      $	��400��54      $
��Left=�55      $��Center=�56      ��57      �� *LongTermSupportResistanceDays+ �� Mode$ TwoWay=� �� UpdateSourceTrigger$ PropertyChanged=� �� ValidatesOnDataErrors . �� 58      $��2��6   $��1��6!   $
��Left=�59      $��Center=�5:      !	   SupportResistanceShortTerm#��Z	 5;      $
��Left=�5<      $	��100��5=      $	��400��5>      �� 5?      $��2��6   $��2��6#   $	��100��5@      $	��400��5A      $
��Left=�5B      $��Center=�5C      ��5D      �� * ShortTermSupportResistanceDays+$ TwoWay=�$ PropertyChanged=� . �� 5E      $��3��6   $��1��6!   $
��Left=�5F      $��Center=�5G      !
   SupportResistanceInputFile#��Z
 5H      $
��Left=�5I      $	��100��5J      $	��400��5K      �� 5L       �� Orientation$ 
Horizontal=�6   $��3��62   $��2��6?   .+�5M      �� $	��100��6   $	��400��5N      $
��Left=�5P      $��Center=�5Q       �� 
IsReadOnly . ��5O      �� *	InputFile+$ TwoWay=�$ PropertyChanged=� . 5R      �� 5S      -   5        $��24��5S      $��48��5T       J� Opacity$	 0.8Q�5U      $
��Left=�5V      $��Center=�5W      $��Center=�5X      +   $SupportResistanceWindow_File_ToolTip � ToolTip# Z 5Y      .��5[      �� $��../Icons/Browse.png��6   $��16��69   $��16��6E   5\      5]      �� 5^      $��4��6   $��1��6!   $
��Left=�5_      $��Center=�5`      &   SupportResistanceStartingEquity#��Z 5a      $
��Left=�5b      $	��100��5c      $	��400��5d      �� 5e      $��4��6   $��2��6#   $	��100��5f      $	��400��5g      $
��Left=�5h      $��Center=�5i      ��5j      �� *StartingEquity+$ TwoWay=�$ PropertyChanged=� . �� 5k      $��5��6   $��1��6!   $
��Left=�5l      $��Center=�5m      #   SupportResistancePercentRisk#��Z 5n      $
��Left=�5o      $	��100��5p      $	��400��5q      �� 5r      $��5��6   $��2��6#   $	��100��5s      $	��400��5t      $
��Left=�5u      $��Center=�5v      ��5w      �� *PercentRisk+$ TwoWay=�$ PropertyChanged=� . �� 5x      $��6��6   $��1��6!   $
��Left=�5y      $��Center=�5z      $   SupportResistanceSkidFraction#��Z 5{      $
��Left=�5|      $	��100��5}      $	��400��5~      �� 5      $��6��6   $��2��6#   $	��100��5�      $	��400��5�      $
��Left=�5�      $��Center=�5�      ��5�      �� *SkidFraction+$ TwoWay=�$ PropertyChanged=� . �� 5�      $��7��6   $��1��6!   $
��Left=�5�      $��Center=�5�      '    SupportResistanceBenchmarkReturn#��Z 5�      $
��Left=�5�      $	��100��5�      $	��400��5�      �� 5�      $��7��6   $��2��6#   $	��100��5�      $	��400��5�      $
��Left=�5�      $��Center=�5�      ��5�      �� *BenchmarkReturn+$ TwoWay=�$ PropertyChanged=� . 5�      �� 5�   
   $ 
Horizontal=�6   .+�5�      �� $��2q�6   $��70��5�      $	��140��5�      $
��Left=�5�      $��Center=�5�      !   SupportResistanceCalculate#��Z 5�      $��Center=�5�      ��5�      �� *CalculateCommand+�� 5�      $��2q�6   $
��Auto��6#   $��Right=�60   $��Center=�6L   
���  ����6g   �� *ErrorString+6�   5�      5�      5�      