   M S B A M L   `   `   `  ���� W  SAdam.Trading.Mechanical.View, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nulla5clr-namespace:Adam.Trading.Mechanical.View.Properties'Adam.Trading.Mechanical.View.Properties  \ XAdam.Trading.Mechanical.ViewModel, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null�Zclr-namespace:Adam.Trading.Mechanical.ViewModel;assembly=Adam.Trading.Mechanical.ViewModel!Adam.Trading.Mechanical.ViewModel [ WAdam.Trading.Mechanical.ViewUtil, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null}Xclr-namespace:Adam.Trading.Mechanical.ViewUtil;assembly=Adam.Trading.Mechanical.ViewUtil Adam.Trading.Mechanical.ViewUtil 8    2Adam.Trading.Mechanical.View.SubViews.Gann2DayView   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     Fprop5clr-namespace:Adam.Trading.Mechanical.View.Properties     p	viewModelZclr-namespace:Adam.Trading.Mechanical.ViewModel;assembly=Adam.Trading.Mechanical.ViewModel     mviewUtilXclr-namespace:Adam.Trading.Mechanical.ViewUtil;assembly=Adam.Trading.Mechanical.ViewUtil     $	��230��$	��400��$	��400��$	��800��$��3q�  � 	Resources     gann2DayViewModel S� 
ObjectType9  3Adam.Trading.Mechanical.ViewModel.Gann2DayViewModel S� ConstructorParameters7  1Adam.Trading.Mechanical.ViewUtil.FilePathProvider��%   &	        S� "     .���� .+�� $
��Auto�� � RowDefinitions �� $��7* ��� $	��30* ��� $	��30* ��� $	��30* ��� $	��30* ��� $	��30* ��� $	��30* ��� $	��30* ��� $	��30* ��� $	��30* ��� $��7* � � ColumnDefinitions �� $��7* ��� $
��200* ��� $
��200* ��� $
��200* ��� $��7* �.+��� $��0��$��1��$
��Left=�$��Center=�7   1Adam.Trading.Mechanical.View.Properties.Resources   GannEntryStopRetracement#��Z $
��Left=�$	��100��$	��400���� $��0��$��2��$	��100��$	��400��$
��Left=�$��Center=����� *GannEntryStopRetracement+ �� Mode$ TwoWay=� �� UpdateSourceTrigger$ PropertyChanged=� �� ValidatesOnDataErrors . �� $��0��$��3��$
��Left=�$��Center=�$��%��	 d� 	IsChecked	 �� *!GannEntryStopRetracementPercent+$ TwoWay=�$ PropertyChanged=��� $��1��$��1��$
��Left=�$��Center=�
   GannEntryStopExceedHiLo#��Z
 $
��Left=�$	��100��$	��400���� $��1��$��2��$	��100��$	��400��$
��Left=�$��Center=����� *GannEntryStopExceedHiLo+$ TwoWay=�$ PropertyChanged=� . �� $��1��$��3��$
��Left=�$��Center=�$��%��	 �� * GannEntryStopExceedHiLoPercent+$ TwoWay=�$ PropertyChanged=��� $��2��$��1��$
��Left=�$��Center=�   GannExitStop#��Z $
��Left=�$	��100��$	��400���� $��2��$��3��$
��Left=�$��Center=�$��%��	 �� *GannExitStopPercent+$ TwoWay=�$ PropertyChanged=��� $��2��$��2��$	��100��$	��400��$
��Left=�$��Center=����� *GannExitStop+$ TwoWay=�$ PropertyChanged=� . �� $��3��$��1��$
��Left=�$��Center=�   GannInputFile#��Z $
��Left=�$	��100��$	��400����  �� Orientation$ 
Horizontal=�$��3��$��2��.+��� $	��100��$	��400��$
��Left=�$��Center=� �� 
IsReadOnly . ���� *	InputFile+$ TwoWay=�$ PropertyChanged=� . �� -   $��24��$��48�� J� Opacity$	 0.8Q�$
��Left=�$��Center=�$��Center=�"   Gann2DayWindow_File_ToolTip � ToolTip# Z .���� $��../Icons/Browse.png��$��16��$��16���� $��4��$��1��$
��Left=�$��Center=�   GannDollarsPerPoint#��Z $
��Left=�$	��100��$	��400���� $��4��$��2��$	��100��$	��400��$
��Left=�$��Center=����� *DollarsPerPoint+$ TwoWay=�$ PropertyChanged=� . �� $��5��$��1��$
��Left=�$��Center=�   GannSkidPoints#��Z $
��Left=�$	��100��$	��400���� $��5��$��2��$	��100��$	��400��$
��Left=�$��Center=����� *
SkidPoints+$ TwoWay=�$ PropertyChanged=� . �� $��6��$��1��$
��Left=�$��Center=�   GannStartingEquity#��Z $
��Left=�$	��100��$	��400���� $��6��$��2��$	��100��$	��400��$
��Left=�$��Center=����� *StartingEquity+$ TwoWay=�$ PropertyChanged=� . �� $��7��$��1��$
��Left=�$��Center=�   GannPercentRisk#��Z $
��Left=�$	��100��$	��400���� $��7��$��2��$	��100��$	��400��$
��Left=�$��Center=����� *PercentRisk+$ TwoWay=�$ PropertyChanged=� . �� $��8��$��1��$
��Left=�$��Center=�   Gann2DayBenchmarkReturn#��Z $
��Left=�$	��100��$	��400���� $��8��$��2��$	��100��$	��400��$
��Left=�$��Center=����� *BenchmarkReturn+$ TwoWay=�$ PropertyChanged=� . �� $ 
Horizontal=�.+��� $��2q�$��70��$	��140��$
��Left=�$��Center=�   Gann2DayCalculate#��Z $��Center=����� *CalculateCommand+�� $��2q�$
��Auto��$��Right=�$��Center=�
���  ������ *ErrorString+