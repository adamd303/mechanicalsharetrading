   M S B A M L   `   `   `  ����J  FWeeklyConverter, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null1clr-namespace:WeeklyConverterWeeklyConverter  5          WeeklyConverter.Window1   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      .mainclr-namespace:WeeklyConverter     5        � Title$  Convert Daily Data��5      $	��110��5      $	��650��5      .��5	      � $
��Auto��6    � RowDefinitions 5
   
   �� 5      $��7 �6   �� 5      $��30 �6   �� 5      $��30 �6   �� 5      $��7 �6    � ColumnDefinitions 5   
   �� 5      $��11 �6   �� 5      $	��600 �6   �� 5      $	��11* �6   .+�5   
   "   WeeklyConverter.FileSelector  -   5          Name$ InputFileSelector��5      $��1��67   $��1��6D   $	��410��6T   $
��Left=�6`      Mode$
 Open=�6{      MaxDisplayLength$	 100��6�   � 5   
   -   5        $��	chartGrid��5      $��1��6    $��1��6-   $
��Auto��6=   $
��Auto��6K    5      �� 5      $
��Auto �6     5      �� 5      $
��Auto �6#   5      �� 5   
   -   5        $��ProcessFile��5      $��2��6$   $��1��61   $	��100��6A   $
��Left=�6M   $��Center=�6h   $��Process Input File��5      $��Center=�6*   6L   5       5!      