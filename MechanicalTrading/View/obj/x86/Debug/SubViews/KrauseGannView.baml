   M S B A M L   `   `   `  ����W  SAdam.Trading.Mechanical.View, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nulla5clr-namespace:Adam.Trading.Mechanical.View.Properties'Adam.Trading.Mechanical.View.Properties  5      \ XAdam.Trading.Mechanical.ViewModel, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null�Zclr-namespace:Adam.Trading.Mechanical.ViewModel;assembly=Adam.Trading.Mechanical.ViewModel!Adam.Trading.Mechanical.ViewModel 5	      [ WAdam.Trading.Mechanical.ViewUtil, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null}Xclr-namespace:Adam.Trading.Mechanical.ViewUtil;assembly=Adam.Trading.Mechanical.ViewUtil Adam.Trading.Mechanical.ViewUtil 5
      :    4Adam.Trading.Mechanical.View.SubViews.KrauseGannView   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Fprop5clr-namespace:Adam.Trading.Mechanical.View.Properties     5      p	viewModelZclr-namespace:Adam.Trading.Mechanical.ViewModel;assembly=Adam.Trading.Mechanical.ViewModel     5	      mviewUtilXclr-namespace:Adam.Trading.Mechanical.ViewUtil;assembly=Adam.Trading.Mechanical.ViewUtil     5
      $	��200��5      $	��400��5      $	��400��5      $	��800��5      $��3q�5        � 	Resources  5         krauseGannViewModel S� 
ObjectType;  5Adam.Trading.Mechanical.ViewModel.KrauseGannViewModel S� ConstructorParameters7  1Adam.Trading.Mechanical.ViewUtil.FilePathProvider��%F   &	        S� 5   
   "  69    5        5      5      5      .��5      �� .+�5   
   � $
��Auto��6    � RowDefinitions 5      �� 5      $��7* �6    �� 5      $	��30* �6    �� 5      $	��30* �6    �� 5      $	��30* �6    �� 5      $	��30* �6    �� 5       $	��30* �6    �� 5!      $	��30* �6    �� 5"      $	��30* �6    �� 5#      $	��30* �6    �� 5$      $	��30* �6    �� 5%      $	��30* �6    �� 5&      $��7* �6     � ColumnDefinitions 5(      �� 5)      $��7* �6#   �� 5*      $
��200* �6#   �� 5+      $
��200* �6#   �� 5,      $
��200* �6#   �� 5-      $��7* �6#   .+�5/      �� $��0��6   $��1��6!   $
��Left=�50      $��Center=�51      7   1Adam.Trading.Mechanical.View.Properties.Resources!   KrauseEntryStopRetracement#��Z 52      $
��Left=�53      $	��100��54      $	��200��55      �� 56      $��0��6   $��2��6#   $	��100��57      $	��200��58      $
��Left=�59      $��Center=�5:      ��5;      �� *KrauseEntryStopRetracement+ �� Mode$ TwoWay=� �� UpdateSourceTrigger$ PropertyChanged=� �� ValidatesOnDataErrors . �� 5<      $��1��6   $��1��6!   $
��Left=�5=      $��Center=�5>       	   KrauseEntryStopExceedHiLo#��Z	 5?      $
��Left=�5@      $	��100��5A      $	��200��5B      �� 5C      $��1��6   $��2��6#   $	��100��5D      $	��200��5E      $
��Left=�5F      $��Center=�5G      ��5H      �� *KrauseEntryStopExceedHiLo+$ TwoWay=�$ PropertyChanged=� . �� 5I      $��2��6   $��1��6!   $
��Left=�5J      $��Center=�5K      
   KrauseRetracementStop#��Z
 5L      $
��Left=�5M      $	��100��5N      $	��200��5O      �� 5P      $��2��6   $��2��6#   $	��100��5Q      $	��200��5R      $
��Left=�5S      $��Center=�5T      ��5U      �� *KrauseRetracementFraction+$ TwoWay=�$ PropertyChanged=� . �� 5V      $��2��6   $��3��6$   $
��Left=�5W      $��Center=�5X         KrauseUseRetracementStop#��Z 5Y       d� 	IsChecked 5Z      �� *KrauseUseRetracementStop+$ TwoWay=�$ PropertyChanged=��� 5[      $��3��6   $��1��6!   $
��Left=�5\      $��Center=�5]         KrauseInputFile#��Z 5^      $
��Left=�5_      $	��100��5`      $	��200��5a      �� 5b       �� Orientation$ 
Horizontal=�6   $��3��62   $��2��6?   .+�5c      �� $	��100��6   $	��200��5d      $
��Left=�5f      $��Center=�5g       �� 
IsReadOnly . ��5e      �� *	InputFile+$ TwoWay=�$ PropertyChanged=� . 5h      �� 5i      -   5        $��24��5i      $��48��5j       J� Opacity$	 0.8Q�5k      $
��Left=�5l      $��Center=�5m      $��Center=�5n          KrauseWindow_File_ToolTip � ToolTip# Z 5o      .��5q      �� $��../Icons/Browse.png��6   $��16��69   $��16��6E   5r      5s      �� 5t      $��4��6   $��1��6!   $
��Left=�5u      $��Center=�5v         KrauseDollarsPerPoint#��Z 5w      $
��Left=�5x      $	��100��5y      $	��200��5z      �� 5{      $��4��6   $��2��6#   $	��100��5|      $	��200��5}      $
��Left=�5~      $��Center=�5      ��5�      �� *DollarsPerPoint+$ TwoWay=�$ PropertyChanged=� . �� 5�      $��5��6   $��1��6!   $
��Left=�5�      $��Center=�5�         KrauseSkidPoints#��Z 5�      $
��Left=�5�      $	��100��5�      $	��200��5�      �� 5�      $��5��6   $��2��6#   $	��100��5�      $	��200��5�      $
��Left=�5�      $��Center=�5�      ��5�      �� *
SkidPoints+$ TwoWay=�$ PropertyChanged=� . �� 5�      $��6��6   $��1��6!   $
��Left=�5�      $��Center=�5�         KrauseStartingEquity#��Z 5�      $
��Left=�5�      $	��100��5�      $	��200��5�      �� 5�      $��6��6   $��2��6#   $	��100��5�      $	��200��5�      $
��Left=�5�      $��Center=�5�      ��5�      �� *StartingEquity+$ TwoWay=�$ PropertyChanged=� . �� 5�      $��7��6   $��1��6!   $
��Left=�5�      $��Center=�5�         KrauseNoContracts#��Z 5�      $
��Left=�5�      $	��100��5�      $	��200��5�      �� 5�      $��7��6   $��2��6#   $	��100��5�      $	��200��5�      $
��Left=�5�      $��Center=�5�      ��5�      �� *KrauseNoContracts+$ TwoWay=�$ PropertyChanged=� . �� 5�      $��7��6   $��3��6$   $
��Left=�5�      $��Center=�5�         KrauseFixContracts#��Z 5�       5�      �� *KrauseFixContracts+$ TwoWay=�$ PropertyChanged=��� 5�      $��8��6   $��1��6!   $
��Left=�5�      $��Center=�5�         TickSize#��Z 5�      $
��Left=�5�      $	��100��5�      $	��200��5�      �� 5�      $��8��6   $��2��6#   $	��100��5�      $	��200��5�      $
��Left=�5�      $��Center=�5�      ��5�      �� *
TickSize+$ TwoWay=�$ PropertyChanged=� . �� 5�      $��9��6   $��1��6!   $
��Left=�5�      $��Center=�5�         KrauseBenchmarkReturn#��Z 5�      $
��Left=�5�      $	��100��5�      $	��200��5�      �� 5�      $��9��6   $��2��6#   $	��100��5�      $	��200��5�      $
��Left=�5�      $��Center=�5�      ��5�      �� *BenchmarkReturn+$ TwoWay=�$ PropertyChanged=� . 5�      �� 5�   
   $ 
Horizontal=�6   .+�5�      �� $��2q�6   $��70��5�      $	��140��5�      $
��Left=�5�      $��Center=�5�         KrauseCalculate#��Z 5�      $��Center=�5�      ��5�      �� *CalculateCommand+�� 5�      $��2q�6   $
��Auto��6#   $��Right=�60   $��Center=�6L   
���  ����6g   �� *ErrorString+6�   5�      5�      5�      