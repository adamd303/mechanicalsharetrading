   M S B A M L   `   `   `  ���� W  SAdam.Trading.Mechanical.View, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nulla5clr-namespace:Adam.Trading.Mechanical.View.Properties'Adam.Trading.Mechanical.View.Properties  \ XAdam.Trading.Mechanical.ViewModel, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null�Zclr-namespace:Adam.Trading.Mechanical.ViewModel;assembly=Adam.Trading.Mechanical.ViewModel!Adam.Trading.Mechanical.ViewModel [ WAdam.Trading.Mechanical.ViewUtil, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null}Xclr-namespace:Adam.Trading.Mechanical.ViewUtil;assembly=Adam.Trading.Mechanical.ViewUtil Adam.Trading.Mechanical.ViewUtil A    ;Adam.Trading.Mechanical.View.SubViews.SupportResistanceView   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     Fprop5clr-namespace:Adam.Trading.Mechanical.View.Properties     p	viewModelZclr-namespace:Adam.Trading.Mechanical.ViewModel;assembly=Adam.Trading.Mechanical.ViewModel     mviewUtilXclr-namespace:Adam.Trading.Mechanical.ViewUtil;assembly=Adam.Trading.Mechanical.ViewUtil     $	��200��$	��400��$	��400��$	��800��$��3q�  � 	Resources     supportResistanceViewModel S� 
ObjectTypeB  <Adam.Trading.Mechanical.ViewModel.SupportResistanceViewModel S� ConstructorParameters7  1Adam.Trading.Mechanical.ViewUtil.FilePathProvider��%   &	        S� "     .���� .+�� $
��Auto�� � RowDefinitions �� $��7* ��� $	��30* ��� $	��30* ��� $	��30* ��� $	��30* ��� $	��30* ��� $	��30* ��� $	��30* ��� $��7* � � ColumnDefinitions �� $��7* ��� $
��200* ��� $
��200* ��� $��7* �.+��� $��1��$��1��$
��Left=�$��Center=�7   1Adam.Trading.Mechanical.View.Properties.Resources    SupportResistanceLongTerm#��Z $
��Left=�$	��100��$	��400���� $��1��$��2��$	��100��$	��400��$
��Left=�$��Center=����� *LongTermSupportResistanceDays+ �� Mode$ TwoWay=� �� UpdateSourceTrigger$ PropertyChanged=� �� ValidatesOnDataErrors . �� $��2��$��1��$
��Left=�$��Center=�!	   SupportResistanceShortTerm#��Z	 $
��Left=�$	��100��$	��400���� $��2��$��2��$	��100��$	��400��$
��Left=�$��Center=����� * ShortTermSupportResistanceDays+$ TwoWay=�$ PropertyChanged=� . �� $��3��$��1��$
��Left=�$��Center=�!
   SupportResistanceInputFile#��Z
 $
��Left=�$	��100��$	��400����  �� Orientation$ 
Horizontal=�$��3��$��2��.+��� $	��100��$	��400��$
��Left=�$��Center=� �� 
IsReadOnly . ���� *	InputFile+$ TwoWay=�$ PropertyChanged=� . �� -   $��24��$��48�� J� Opacity$	 0.8Q�$
��Left=�$��Center=�$��Center=�+   $SupportResistanceWindow_File_ToolTip � ToolTip# Z .���� $��../Icons/Browse.png��$��16��$��16���� $��4��$��1��$
��Left=�$��Center=�&   SupportResistanceStartingEquity#��Z $
��Left=�$	��100��$	��400���� $��4��$��2��$	��100��$	��400��$
��Left=�$��Center=����� *StartingEquity+$ TwoWay=�$ PropertyChanged=� . �� $��5��$��1��$
��Left=�$��Center=�#   SupportResistancePercentRisk#��Z $
��Left=�$	��100��$	��400���� $��5��$��2��$	��100��$	��400��$
��Left=�$��Center=����� *PercentRisk+$ TwoWay=�$ PropertyChanged=� . �� $��6��$��1��$
��Left=�$��Center=�$   SupportResistanceSkidFraction#��Z $
��Left=�$	��100��$	��400���� $��6��$��2��$	��100��$	��400��$
��Left=�$��Center=����� *SkidFraction+$ TwoWay=�$ PropertyChanged=� . �� $��7��$��1��$
��Left=�$��Center=�'    SupportResistanceBenchmarkReturn#��Z $
��Left=�$	��100��$	��400���� $��7��$��2��$	��100��$	��400��$
��Left=�$��Center=����� *BenchmarkReturn+$ TwoWay=�$ PropertyChanged=� . �� $ 
Horizontal=�.+��� $��2q�$��70��$	��140��$
��Left=�$��Center=�!   SupportResistanceCalculate#��Z $��Center=����� *CalculateCommand+�� $��2q�$
��Auto��$��Right=�$��Center=�
���  ������ *ErrorString+