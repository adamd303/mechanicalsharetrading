   M S B A M L   `   `   `  ����   Adam.Trading.Mechanical.View:    4Adam.Trading.Mechanical.View.SubViews.StockChartView   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      .��5      � -   5        $��grid1��5      $��10q�6     � ColumnDefinitions  5   
   �� 5      $
��Auto �6   �� 5	      $��* �6    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   .+�5   
   �� -   5         ��Name$ tbTitle��5      $��2q�6   $��1��60   $��0��6@    J� RenderTransformOrigin$ 0.5,0.5&�5      $��14(�61   $
��Bold"�6?   $��Stretch=�5      $��Stretch=�6/    �� TextAlignment$ Center=�6K   $��Title��5      �� 5   
   -   5        $ tbXLabel��5      $��2q�6   $��1��61   $��2��6A   $ 0.5,0.5&�5      $ Center=�60   $��X Axis��6G   �� 5   
   -   5        $��tbYLabel��5      $��2q�6   $��0��6/   $��1��6?   $ 0.5,0.5&�5      $ Center=�61   $��Y Axis��6H    � LayoutTransform 5      �� 5       �� Angle$	 -90Q�6&   5      � 5   
   -   5         �Name$ 	chartGrid��5       $��0,0,0,0q�6   $��1��64   $��1��6D   }�. 
������ 5!      �� 5"   
   -   5        $��
textCanvas��5"      $��2q�6   $��1��6.   $��1��6>   }�. .+�5#      �� -   5        $��chartCanvas��5#      }�. 
������ 6<   5$      5%      5&      