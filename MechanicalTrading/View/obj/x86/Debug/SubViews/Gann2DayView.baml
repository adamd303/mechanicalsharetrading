   M S B A M L   `   `   `  ����W  SAdam.Trading.Mechanical.View, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nulla5clr-namespace:Adam.Trading.Mechanical.View.Properties'Adam.Trading.Mechanical.View.Properties  5      \ XAdam.Trading.Mechanical.ViewModel, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null�Zclr-namespace:Adam.Trading.Mechanical.ViewModel;assembly=Adam.Trading.Mechanical.ViewModel!Adam.Trading.Mechanical.ViewModel 5	      [ WAdam.Trading.Mechanical.ViewUtil, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null}Xclr-namespace:Adam.Trading.Mechanical.ViewUtil;assembly=Adam.Trading.Mechanical.ViewUtil Adam.Trading.Mechanical.ViewUtil 5
      8    2Adam.Trading.Mechanical.View.SubViews.Gann2DayView   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Fprop5clr-namespace:Adam.Trading.Mechanical.View.Properties     5      p	viewModelZclr-namespace:Adam.Trading.Mechanical.ViewModel;assembly=Adam.Trading.Mechanical.ViewModel     5	      mviewUtilXclr-namespace:Adam.Trading.Mechanical.ViewUtil;assembly=Adam.Trading.Mechanical.ViewUtil     5
      $	��230��5      $	��400��5      $	��400��5      $	��800��5      $��3q�5        � 	Resources  5         gann2DayViewModel S� 
ObjectType9  3Adam.Trading.Mechanical.ViewModel.Gann2DayViewModel S� ConstructorParameters7  1Adam.Trading.Mechanical.ViewUtil.FilePathProvider��%F   &	        S� 5   
   "  67    5        5      5      5      .��5      �� .+�5   
   � $
��Auto��6    � RowDefinitions 5      �� 5      $��7* �6    �� 5      $	��30* �6    �� 5      $	��30* �6    �� 5      $	��30* �6    �� 5      $	��30* �6    �� 5       $	��30* �6    �� 5!      $	��30* �6    �� 5"      $	��30* �6    �� 5#      $	��30* �6    �� 5$      $	��30* �6    �� 5%      $	��30* �6    �� 5&      $��7* �6     � ColumnDefinitions 5(      �� 5)      $��7* �6#   �� 5*      $
��200* �6#   �� 5+      $
��200* �6#   �� 5,      $
��200* �6#   �� 5-      $��7* �6#   .+�5/      �� $��0��6   $��1��6!   $
��Left=�50      $��Center=�51      7   1Adam.Trading.Mechanical.View.Properties.Resources   GannEntryStopRetracement#��Z 52      $
��Left=�53      $	��100��54      $	��400��55      �� 56      $��0��6   $��2��6#   $	��100��57      $	��400��58      $
��Left=�59      $��Center=�5:      ��5;      �� *GannEntryStopRetracement+ �� Mode$ TwoWay=� �� UpdateSourceTrigger$ PropertyChanged=� �� ValidatesOnDataErrors . �� 5<      $��0��6   $��3��6$   $
��Left=�5=      $��Center=�5>      	   
GannUseATR#��Z	 5?      
 d� 	IsChecked
 5@      �� *GannEntryStopRetracementATR+$ TwoWay=�$ PropertyChanged=��� 5A      $��1��6   $��1��6!   $
��Left=�5B      $��Center=�5C         GannEntryStopExceedHiLo#��Z 5D      $
��Left=�5E      $	��100��5F      $	��400��5G      �� 5H      $��1��6   $��2��6#   $	��100��5I      $	��400��5J      $
��Left=�5K      $��Center=�5L      ��5M      �� *GannEntryStopExceedHiLo+$ TwoWay=�$ PropertyChanged=� . �� 5N      $��1��6   $��3��6$   $
��Left=�5O      $��Center=�5P      #��Z	 5Q      
 5R      �� *GannEntryStopExceedHiLoATR+$ TwoWay=�$ PropertyChanged=��� 5S      $��2��6   $��1��6!   $
��Left=�5T      $��Center=�5U         GannExitStop#��Z 5V      $
��Left=�5W      $	��100��5X      $	��400��5Y      �� 5Z      $��2��6   $��3��6$   $
��Left=�5[      $��Center=�5\      #��Z	 5]      
 5^      �� *GannExitStopATR+$ TwoWay=�$ PropertyChanged=��� 5_      $��2��6   $��2��6#   $	��100��5`      $	��400��5a      $
��Left=�5b      $��Center=�5c      ��5d      �� *GannExitStop+$ TwoWay=�$ PropertyChanged=� . �� 5e      $��3��6   $��1��6!   $
��Left=�5f      $��Center=�5g         GannNoContractsCalc#��Z 5h      $
��Left=�5i      $	��100��5j      $	��400��5k      �� 5l      $��3��6   $��2��6$   $
��Left=�5m      $��Center=�5n         GannUseCurrentEquity#��Z 5o      
 5p      �� *UseOpenProfit+$ TwoWay=�$ PropertyChanged=��� 5q      $��4��6   $��1��6!   $
��Left=�5r      $��Center=�5s         GannInputFile#��Z 5t      $
��Left=�5u      $	��100��5v      $	��400��5w      �� 5x       �� Orientation$ 
Horizontal=�6   $��4��62   $��2��6?   .+�5y      �� $	��100��6   $	��400��5z      $
��Left=�5|      $��Center=�5}       �� 
IsReadOnly . ��5{      �� *	InputFile+$ TwoWay=�$ PropertyChanged=� . 5~      �� 5      -   5        $��24��5      $��48��5�       J� Opacity$	 0.8Q�5�      $
��Left=�5�      $��Center=�5�      $��Center=�5�      "   Gann2DayWindow_File_ToolTip � ToolTip# Z 5�      .��5�      �� $��../Icons/Browse.png��6   $��16��69   $��16��6E   5�      5�      �� 5�      $��5��6   $��1��6!   $
��Left=�5�      $��Center=�5�         GannDollarsPerPoint#��Z 5�      $
��Left=�5�      $	��100��5�      $	��400��5�      �� 5�      $��5��6   $��2��6#   $	��100��5�      $	��400��5�      $
��Left=�5�      $��Center=�5�      ��5�      �� *DollarsPerPoint+$ TwoWay=�$ PropertyChanged=� . �� 5�      $��6��6   $��1��6!   $
��Left=�5�      $��Center=�5�         GannSkidPoints#��Z 5�      $
��Left=�5�      $	��100��5�      $	��400��5�      �� 5�      $��6��6   $��2��6#   $	��100��5�      $	��400��5�      $
��Left=�5�      $��Center=�5�      ��5�      �� *
SkidPoints+$ TwoWay=�$ PropertyChanged=� . �� 5�      $��7��6   $��1��6!   $
��Left=�5�      $��Center=�5�         GannStartingEquity#��Z 5�      $
��Left=�5�      $	��100��5�      $	��400��5�      �� 5�      $��7��6   $��2��6#   $	��100��5�      $	��400��5�      $
��Left=�5�      $��Center=�5�      ��5�      �� *StartingEquity+$ TwoWay=�$ PropertyChanged=� . �� 5�      $��8��6   $��1��6!   $
��Left=�5�      $��Center=�5�         GannPercentRisk#��Z 5�      $
��Left=�5�      $	��100��5�      $	��400��5�      �� 5�      $��8��6   $��2��6#   $	��100��5�      $	��400��5�      $
��Left=�5�      $��Center=�5�      ��5�      �� *PercentRisk+$ TwoWay=�$ PropertyChanged=� . �� 5�      $��9��6   $��1��6!   $
��Left=�5�      $��Center=�5�         Gann2DayBenchmarkReturn#��Z 5�      $
��Left=�5�      $	��100��5�      $	��400��5�      �� 5�      $��9��6   $��2��6#   $	��100��5�      $	��400��5�      $
��Left=�5�      $��Center=�5�      ��5�      �� *BenchmarkReturn+$ TwoWay=�$ PropertyChanged=� . 5�      �� 5�   
   $ 
Horizontal=�6   .+�5�      �� $��2q�6   $��70��5�      $	��140��5�      $
��Left=�5�      $��Center=�5�         Gann2DayCalculate#��Z 5�      $��Center=�5�      ��5�      �� *CalculateCommand+�� 5�      $��2q�6   $
��Auto��6#   $��Right=�60   $��Center=�6L   
���  ����6g   �� *ErrorString+6�   5�      5�      5�      