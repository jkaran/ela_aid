   M S B A M L   `   `   `  ����  Agent.Interaction.Desktop/    )Agent.Interaction.Desktop.DispositionForm   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   -   5          � Title$  DispositionForm��5   	   $	��500��6!   $
��Auto��6-    � WindowStyle$
 None=�6;    � Icon$F @/Agent.Interaction.Desktop;component/Images/pointelicon16x16.ico��6N    � AllowsTransparency . 
������  � WindowStartupLocation$ CenterOwner=�5   6    � SizeToContent$ Height=�6Z    � ShowInTaskbar .   � 
ResizeMode$ NoResize=�5   �    � 	Resources 5      	 �� MergedDictionaries
 �� Source �� 
TargetType   	baseStyle �� Property �� Value �� BasedOn�� 5   
   	 5	      �� 5
      $R
 L/Agent.Interaction.Desktop;component/Resources/StylesResourceDictionary.xaml?�6%   %�   &	        '�� H     7   �� 5      " ��6   .�5      ��  � ��$ 12(�6-   5      �� 5      " ��6   8   5      5      .��5   
   �  � RowDefinitions 5      �� 5      $
��Auto �6    .+�5   
   �� -   5        $��
MainBorder��5   S   $��0��6   $
��0.89q�6   $��8q�65   
�������._�5      ��  �� Orientation$ Vertical=�6   .+�5      �� -   5        $��25��5      ���#007edf$��0q�6;   $��Stretch=�6F   $��Center=�6d   ._�5      �� -   5        $��lblTitleStatus��5        �� Padding$ 4,0,4,0q�66   $��Calibri)�6H   $��15(�6]   
�������$
��Bold"�6~   $��Center=�6�   $��Disposition Form��5       $
��Left=�69   6V   5      � 5      $��
10,15,10,5q�6    5       �� 5!      6+   �� 5"      6*   �� 5#      6*   �� 5$      6*   �� 5%      6*   �� 5&      6*   �� 5'      6*   �� 5(      6*   �� 5)      6*    � ColumnDefinitions 5+      �� 5,      6.   �� 5-      6-   .+�50      �� $	��MID��6    $��5,0,0,0q�6+   $��Bottom=�6<   $��12(�6W   6g   �� 51      -   5        $��txtMID��51      $��Stretch=�6,   $��25��6K   $��1��6W   $	��5,0q�6d    �� 	MaxLength$ 20��6q    �� 	UndoLimit$ 10��6�     DefaultCopyContextmenu � ContextMenu# [ 6�     controltempTextbox1#��[ 6�   6�   �� 53      $ 
Horizontal=�6!   $��2��6:   $��0,3,0,0q�6G   .+�54      �� $��Caller Name��6$   $��5,0,0,0q�68   $��Bottom=�6I   $��12(�6d   6t   �� 55      $��*��6$   $��5,0,5,0q�6-   $��Right=�6?   
���  ��$��12(�6l   6|   56      �� 57      -   5        $��txtCallerName��57      $��Stretch=�63   $��25��6R   $��3��6^   $��5q�6k   $	 100��6v   $ 10��6�   # [ 6�   #��[ 6�   6�   �� 59      $��Phone��6    $��4��6-   $��5,3,5,0q�6:   $��Bottom=�6K   $��12(�6f   6v   �� 5:      -   5        $��txtPhone��5:      $��Stretch=�6.   $��5��6L   $��5q�6Y   $ 20��6d   $��25��6t   $ 10��6�   # [ 6�   #��[ 6�   6�   �� 5<      $ 
Horizontal=�6!   $��1��6:   .+�5=      �� $	��LOB��6%   $��5,0,0,0q�61   $��Bottom=�6B   $��12(�6]   6m   �� 5>      $��*��6%   $��5,0,5,0q�6.   $��Right=�6@   
���  ��$��12(�6m   6}   5?      �� 5A      -   5        $��cbLOB��5A       ���#9CAAC1$��Stretch=�5B      $��1��6)   $��1��66   $��5q�6F   $��25��6Q   ��5A   -   �� *LOB+5B   Q   �� 5E      $ 
Horizontal=�6!   $��2��6;   $��1��6H   $��0,3,0,0q�6X   .+�5F      �� $��Termination Code��6%   $��5,0,0,0q�6=   $��Bottom=�6N   $��12(�6i   6y   �� 5G      $��*��6%   $��5,0,5,0q�6.   $��Right=�6@   
���  ��$��12(�6m   6}   5H      �� 5I      -	   5        $��cbTerminationCode��5I       ���#9CAAC1$��Stretch=�5J      $��3��6)   $��1��66   $��5q�6F   $��25��6R   ��5I   9   �� *LOB+5J   R   �� 5M      $
��AWD#��6    $��4��6,   $��1��69   $��5,3,5,0q�6I   $��Bottom=�6Z   $��12(�6u   6�   �� 5N      -
   5        $��txtAWD��5N      $��Stretch=�6,   $��5��6J   $��1��6W   $ 10��6g   $ 20��6v   # [ 6�   #��[ 6�   $��5q�6�   6�   �� 5P      $��Notes��6    $��6��6-   $��2��6:   $��5,3,5,0q�6N   $��Bottom=�6_   $��12(�6z   6�   �� 5R      -   5        $��txtNotes��5R   ?   $��2��6   $��7��62   $��50��6O   $	 255��6[   $��Stretch=�6k    �� TextWrapping$ WrapWithOverflow=�6�   $ 10��6�   # [ 6�   #��[ 6�   $��5q�6  6,  � 5T      $��2��6   $��8��6/    5U      �� 5V      $��* �6/   6;   �� 5W      $
��Auto �6/   6>   .+�5Y      �� -   5        $��txtError��5Y   $   $��Stretch=�64   
���  ��$��5q�6c   $��Center=�6n   $��WrapWithOverflow=�6�   6�   �� 5Z      -   5        $��btnSave��5Z   !   $��5q�60   $
��Save��6;   $��Right=�6J   $��23��6f   $��1��6r   $��80��5[   !     NormalButton#��[ 6,   6j   5\      5]      5^      5_      5`      5b      