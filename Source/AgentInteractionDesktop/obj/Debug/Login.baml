   M S B A M L   `   `   `  ����  Agent.Interaction.Desktop%    Agent.Interaction.Desktop.Login   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      I EWpfAnimatedGif, Version=1.4.4.0, Culture=neutral, PublicKeyToken=null,gif"http://wpfanimatedgif.codeplex.com  5      -   5        $��loginWindow��5        � Title$  Login��6   $	��400��6$   $
��Auto��60    � WindowStyle$
 None=�6>    � Icon$F @/Agent.Interaction.Desktop;component/Images/pointelicon16x16.ico��6Q    � AllowsTransparency . 
������  � WindowStartupLocation$ CenterScreen=�5   6    � SizeToContent$ Height=�6[    � 	Resources 5       �� MergedDictionaries �� Source   CantarellFontFamily	 �� 
TargetType
 �� Property �� Value�� 5       5	      �� 5
      $R L/Agent.Interaction.Desktop;component/Resources/StylesResourceDictionary.xaml?�5      %�   &	        '� O     *�5      ;7/Agent.Interaction.Desktop;component/Resources/#Calibri)�6f   �� 5      "	 �6   .�5      �� 
 � ��$ Calibri)�6#   5      5      .��5      � .+�5      �� -   5        $��
MainBorder��5   @   $
��0.89q�6   $��8q�6"   
�������._�5      � $��0q�6
    � RowDefinitions 5      �� 5      $
��Auto �6   6%   �� 5      $��30 �6   6#   .+�5      � $��5q�6   $��0��6    5      �� 5      $��50 �6   6$   �� 5      $
��Auto �6   6&   �� 5      $��40 �6   6$   .+�5!      � -   5        $��0��5!      
������� � ColumnDefinitions 5"      �� 5#   	   $
��Auto �6   �� 5$   	   $
��Auto �6   .+�5&      �� -   5        $��image1��5&      $��0��6   $��Right=�6,   $��55,3,0,5q�6H   $
��Auto��6Z   $��40��6g   $G��A/Agent.Interaction.Desktop;component/Images/Pointel Softphone.png��6�   �� 5'      -   5        $��10,0,0,0q�5'      $��1��6%   $
��Left=�65    �� Orientation$ Vertical=�6P   .+�5(   	   �� -   5        $��
loginTitle��5(      $��18(�6K   $��0,3,0,0q�6Y   ���#007edf$
��Bold"�6     keyTitle#���  6�   6�   �� 5)   	   -   5        $��loginTitleversion��5)      $��
140,-7,0,3q�6R     keyTitleVersion#���  6f   $��12(�6�   6�   5*   	   5+      �� 5,      -   5        $��borderContent��5,      $��1��6#   ���#656d90$��0.5,0.5,0.5,0q�6F   ._�5.      �  5/   	   �� 50   
   $
��Auto �6   6(   .+�52   	   � $��0��6    53   
   ��%�   '��       'G� H     �� 54      "	 ��6   .�55      �� 
 � ��$ 23��6%   56      �� 57      "	 G�6   .�58      �� 
 � ��$ 23��6%   59      5:       5;   
   �� 5<      -	   5         �Name$ grdrow_error��5<      ��6-   �� *ErrorRowHeight+6Q   �� 5=      $��28 �6   6'   �� 5>      $��28 �6   6'   �� 5?      $��0 �6   6&   �� 5@      ��6   �� *ConfDetails_SP_Height+6D   �� 5A      ��6   �� *ConfDetails_SP_Height+6D   �� 5B      ��6   �� *ConfDetails_SP_Height+6D   �� 5C      ��6   �� *!ConfDetails_SP_Height_keepplace+6N    5E   
   �� 5F      $
��Auto �6   $	��120��6)   6:   �� 5G      $��* �6   6(   .+�5I   
   �� -
   5        $��
stkp_Error��5I   [   $ 
Horizontal=�6   $��3q�6.   $��0��6:   $��2��6G   
��� ���$��Stretch=�6�   .+�5J      �� $��5,2,0,2q�6   $	��Top=�6"   $;��5/Agent.Interaction.Desktop;component/Images/Error.png��6:   $��20��6y   $��20��6�   �� 5K      -   5        $��lblInformation��5K      $��10,5,5,5q�6+   $	��320��6=   $
��Wrap=�6g   ��6I   �� *ErrorMessage+5L      5N      �� 5P   
   -   5        $��lblUsername��5P        keyUserName#���  6#   $��0��6K   $��1��6[   6j   �� 5R   
   -   5        $��txtUserName��5R   !    �� 	UndoLimit$ 10��6   $��1��64   $��1��6A     DefaultCopyContextmenu � ContextMenu# [ 6k    �� SelectedText$  ��5S      
 � Tag$ vinoth��6   $��Stretch=�6)   $��2q�6G     controltempTextbox#��[ 5T      -  'System.Windows.Input.KeyboardNavigation   TabIndex$ 0��5U         TabNavigation$ Local=�6H    �� 	MaxLength$	 255��6q   ��5R   Q   �� *
UserName+5U   q   �� 5W   
   -   5        $��lblPassword��5W        keyPassword#���  6#   $��0��6K   $��2��6[   6j   G� 5Z   
   -   5        $��txtPassword��5Z      $��2��6)   $��2��66   $��Stretch=�6F   $��2q�6d     controltempPasswordbox#��[ 6o   $ 1��6�   $ Local=�6  # [ 6-  6e  �� 5[   
   -   5        $��
cbLanguage��5[   \   $��3��6   $��1��6    $
��Left=�60   $��2,2,0,2q�6K   $	��150��6�   $��23��6�   ���#9CAAC1 �� DisplayMemberPath$
 Name��5\   
   ��5[   n   �� *LanguageList+ �� SelectedItem 5\   #   �� *SelectedLanguage+ �� Mode$ TwoWay=�6Z   �� 5^   
   -   5        $��lblApplication��5^      $��0��6&   $��4��66     keyApplicationName#���  6C   6t   �� 5`   
   -   5        $��txtApplication��5`   !   $ 10��6   $��Stretch=�67   $��1��6U   $��4��6e   $��2q�6r   #��[ 6�   $ 2��6�   $ Local=�61  # [ 6[  $	 255��6�  ��6}   �� *ApplicationName+6�  �� 5b   
   -   5        $��lblHostname��5b      $��0��6$   $��5��64    	 keyHostName#��� 	 6A   6k   �� 5c   
   -   5        $��cmbHostname��5c      $��5��6&   $��2q�63   $��1��6>   $��23��6N    �� 
IsEditable . $ 4��6d  $
 None=�6�   �� Text 6l   �� *HostNameText+��6�   �� *HostNameItemSource+ �� SelectedValue 6�   �� *HostNameSelectedValue+$ TwoWay=�5d      �� 5f   
   -   5        $��lblPort��5f      $��0��6   $��6��6/    
 keyPort#��� 
 6<   6b   �� 5g   
   -   5        $��cmbPort��5g      $��6��6"   $��2q�6/   $��1��6:   $��23��6J    . $ 4��6  $
 None=�6?   6h   �� *
PortText+��6�   �� *PortItemSource+ 6�   �� *PortSelectedValue+$ TwoWay=�5h      �� 5i   
   -   5        $��cbxKeepPlace��5i      $��7��6'   
���  ��$��1��6E   $��2q�6U   $��23��6`   $��Center=�6�   $ 6��6�   $ Local=�6�    d� 	IsChecked 6l   �� *KeepRecentPlace+$ TwoWay=�6  5j   
   5k   	   5l      �� 5m      -   5        $��brdSubmition��5m      $��2��6"   ���#656d90$	��0.5q�6E   ._�5o      � $��Center=�6    5p   	   �� 5q   
   $
��Auto �6   6(   .+�5s   	   � $��1��6    5t   
   �� 5u      $
��Auto �6   6+   �� 5v      $��* �6   6(   �� 5w      $��15 �6   6)   �� 5x      $
��Auto �6   6+   .+�5{   
   9� -   5        $��	hcontCtrl��5{        StretchyExpanderTemp#��[ 6$   $��0��6V   $��20��6f   $ 7��6�   $ Local=�6�   5}      �� 5~   
   -   5        $��btnLogin��5~      $��23��6!   $��80��6-   $��Right=�68   $��1��6T     NormalButton#��[ 6d   $ 8��6�   $ Local=�6�   6�   �� 5   
   -   5        $��	btnCancel��5      $��23��6"   $��80��6.   $��Right=�69   $��0,0,5,0q�6U   $��3��6f   #��[ 6v   $ 9��6�   $ Local=�6�   6�   5�   
   5�   	   5�      5�      �� 5�      $��31��6   ���#007edf$��Stretch=�6.   $	��Top=�6L   $��1��6d   $��0q�6q   ._�5�      � .+�5�      �� $��© 2013-2017 Pointel Inc��6   $��Calibri)�61   $
��Left=�6G   $��Center=�6b   $��12(�6}   
�������$��5,0,0,0q�6�   �� 5�      $��Right=�66   $��Center=�6R   ���#EA3737 �� CornerRadius$ 	10,0,0,10��6�   y�6   �� *TrialVisibility+._�5�   	   �� $��Calibri)�61   $��Center=�6F   $��Center=�6c   $��12(�6~   
�������$��	15,2,10,2q�6�   ��6   �� *TrailMessage+6�   5�   	   5�      5�      5�      5�      5�      5�      