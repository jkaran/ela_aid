   M S B A M L   `   `   `  ����  Agent.Interaction.Desktop0    *Agent.Interaction.Desktop.WebBrowserWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   -   5        $��WebBrowserWin��5   9     � Title$  WebBrowserWindow��6	    � 
ResizeMode$ 	CanResize=�6"    � WindowStyle$
 None=�6O    � Icon$@ :/Agent.Interaction.Desktop;component/Images/Agent32x32.ico��6b   
������  � AllowsTransparency . $	��300��5   {   $	��300��6�   $	��450��6�   $	��830��6�    � WindowStartupLocation$ CenterScreen=�5        � 	Resources 5       �� MergedDictionaries �� Source�� 5   
    5	      �� 5
      $R L/Agent.Interaction.Desktop;component/Resources/StylesResourceDictionary.xaml?�6%   %    5      .��5      � .+�5   
   �� -   5        $��
MainBorder��5      $��8q�6K   $��1q�6V   
�������	 %� IsVirtualizing	 . 
 %� VirtualizationMode$
 	Recycling=�5      ��5   #   �� *MainBorderBrush+._�5      � 	 . $
 	Recycling=�5       � RowDefinitions 5      �� 5      $��* �6$   �� 5      $��5 �6$   .+�5      �� -   5        $��RightSideRect��5      $��2��61   
������ $��10��6U    K� ZIndex$
 1000��6`   $��Right=�6t   $��Stretch=�6�   �� 5      -   5        $��RightbottomSideRect��5      $��1��67   
������ $��10��6W   $��5��6b   $
 2200��6n   $��Right=�6�   $��Bottom=�6�   �� 5      -   5        $��BottomSideRect��5      $��1��62   
������ $��10��6R   $
 1000��6^   $��Stretch=�6r   $��Bottom=�6�   � 5      
�������	 . $
 	Recycling=�5       5      �� 5      -   5         �Name$ grdTitle��5   (   $
��Auto �68   6H   �� 5       -   5        $ 	grdBottom��5    (   ��69   �� *BottomRowHeight+6^   .+�5"      � -   5        $��	TitleGrid��5"   (   $��0��6    � ColumnDefinitions 5#      �� 5$      $
��Auto �6/   6>   �� 5%      $��* �6/   6;   �� 5&      $
��Auto �60   6?   .+�5*      �� -	   5        $��imgSoftIcon��5*   J   $��15��6!   $��16��6,   $��5,0,30,0q�68   $
��Left=�6]   $	��Top=�6x   $G��A/Agent.Interaction.Desktop;component/Images/Pointel Softphone.png��6�   �� 5.      -
   5        $��1��5.   !   $��18��61   ���#007EDF$	��Top=�6R   $��0,-1,0,0q�6j   ._�50      �  51   "   �� 52   &   $��* �67   �� 54   &   $
��Auto �67   .+�56   "   � $��0��6'   $
��Left=�68   ��6S   �� *TitleBgColor+ 57   &   �� 58   *   $
��Auto �6;   �� 59   *   $
��Auto �6;   �� 5:   *   $
��Auto �6;   .+�5<   &   �� -   5        $��lblTitleStatus��5<   R   $��0��60   $��10,0,0,0q�6@   $��Calibri)�6h   $��14(�6}   
�������$
��Bold"�6�   $��Stretch=�6�   
������ $
��Left=�5=   C   ��6!   �� *TitleStatusText+6�   �� 5>   &   -   5        $��lbldummy��5>   @   $��1��60   $��3��6P   $��Calibri)�6Z   $��14(�6o   
�������$
��Bold"�6�   $��Stretch=�6�   
������ $
��Left=�5?   %   6C   5@   #   �� 5D   "   -   5        $��txtCallStatus��5D   <   $��1��6,   $��
-0.3,0,5,0q�6Q   $��Calibri)�6e   $��14(�6z   
�������$
��Bold"�6�   $��Stretch=�6�   $��Right=�5E   F   ��6&   �� *CallTypeStatus+6�   5F      5G      �� 5H      $��2��6&   $��60��66   $��0,0,6,0q�6A   $
��Auto��6R   $��Right=�6`   $	��Top=�6|    �� Orientation$ 
Horizontal=�6�   .+�5I      �� -   5        $��btnMinimize��5I   %   $��16��68   $��20��6D      MinimizeButton#��[  6O   6w   �� 5J      -   5        $��btnMaximize��5J   %   $��16��69   $��20��6E     MaximizeButton#��[ 6P   6x   �� 5K      -   5        $��btnExit��5K   %   $��16��64   $��20��6@     CloseButton#��[ 6K   6p   5L      5M      � 5N      -   5        $��BottomWindowGrid��5N      $��1��63   	 . $
 	Recycling=�5O      $��
10,0,10,10q�6E   .+�5P      (  "System.Windows.Controls.WebBrowser  -   5        $��
webBrowser��5P   %   $��Stretch=�67   $��Stretch=�6U   {�.  6�   5Q      5R      5S      5T      5U      5V      