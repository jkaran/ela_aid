   M S B A M L   `   `   `  ����p  lSystem.Windows.Controls.Input.Toolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35pTclr-namespace:System.Windows.Controls;assembly=System.Windows.Controls.Input.ToolkitSystem.Windows.Controls  5   	   U QAgent.Interaction.Desktop, Version=5.0.3.27, Culture=neutral, PublicKeyToken=nullc6clr-namespace:Agent.Interaction.Desktop.CustomControls(Agent.Interaction.Desktop.CustomControls 5   	   .   (Agent.Interaction.Desktop.ChangePassword   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   
   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   cmyTclr-namespace:System.Windows.Controls;assembly=System.Windows.Controls.Input.Toolkit     5   	   PCustomControl6clr-namespace:Agent.Interaction.Desktop.CustomControls     5   	   -   5        $��channelWindow��5        � Title$  Login��6	    � WindowStyle$
 None=�6,    � Icon$F @/Agent.Interaction.Desktop;component/Images/pointelicon16x16.ico��6?    � ShowInTaskbar .   � AllowsTransparency . 
������  � WindowStartupLocation$ CenterScreen=�5   <   $	��400��6a   $	��400��6m    � SizeToContent$ Height=�6|    � 	Resources 5
       �� MergedDictionaries	 �� Source�� 5   
    5      �� 5      $R	 L/Agent.Interaction.Desktop;component/Resources/StylesResourceDictionary.xaml?�5      %    5      .��5      � .+�5   
   �� -   5        $��
MainBorder��5      $
��0.89q�6#   $��8q�6:   
�������._�5      � 
 � RowDefinitions
 5      �� 5      $
��Auto �6$   64   �� 5      $
��Auto �6%   65   �� 5      $
��Auto �6$   64   �� 5      $
��Auto �6$   64   .+�5      � -   5        $��0��5      
�������.+�5      �� -   5        $��5,0,0,0q�5   !   
�������$��Center=�6E    �� Orientation$ 
Horizontal=�6�   .+�5      �� -   5        $��image1��5       $
��Auto��6Y   $��40��6f   $G��A/Agent.Interaction.Desktop;component/Images/Pointel Softphone.png��6r   �� 5      $ Vertical=�6%   .+�5      �� -   5        $��18(�5   $   $��0,3,0,0q�62   ���#007edf$
��Bold"�6�      keyTitle#���   6�   $��Calibri)�6�   6�   �� 5       -   5        $��channelTitleversion��5    $   $��
140,-7,0,3q�6?     keyTitleVersion#���  6~   $��Calibri)�6�   $��12(�6�   6�   5!      5"      5#      �� 5$      $��0.5,0.5,0.5,0q�6   ���#656d90$��5,0,5,0q�6O   $��1��6`   ._�5%      �  � ColumnDefinitions 5&      �� 5'      $
��161* �6/   6>   �� 5(      $
��237* �6/   6>   
 5*      �� 5+      $
��Auto �6,   �� 5,      ��6,   �� *CSErrorRowHeight+�� 5-      $��28 �6,   �� 5.      $��28 �6,   �� 5/      $��28 �6,   �� 50      $��28 �6,   .+�53      �� -   5        $��lblUserName��53   s   $!��Please enter a new password��6    $��14(�6F   $��2��6T   $��2q�6h   ���#E1F2FA6�   �� 54      -	   5        $��
stkp_Error��54   `   $ 
Horizontal=�6%   $��1��6?   $��2��6L   $��3,0,3,3q�6r   
��� ���$��Stretch=�6�   .+�55      �� $��5,2,0,2q�6$   $	��Top=�65   $;��5/Agent.Interaction.Desktop;component/Images/Error.png��6M   $��20��6�   $��20��6�   �� 56      -
   5        $��lblInformation��56   (   $��10,5,5,5q�6>   $	��300��6P   $
��Wrap=�6|   ��6\   �� *CSErrorMessage+57      58      �� 59      $��
User Name:��6    $��2��65   $��Center=�6B   �� 5:      $��Old Password:��6    $��3��68   $��Center=�6E   �� 5;      $��New Password:��6    $��4��68   $��Center=�6E   �� 5<      $��Confirm Password:��6    $��5��6<   $��Center=�6I   �� 5=      $��2��6    $��1��6-     DefaultCopyContextmenu � ContextMenu# [ 6Z   
 � Tag$ vinoth��5>       $��Stretch=�6-   $��2q�6K   ���#d3d3d3��5=   =   �� *
UserName+5>   V   G� 5?      -   5        $��
txtOldPass��5?   &   $��3��68   $��1��6E   $��Stretch=�6U   $��2q�6s     controltempPasswordbox#��[ 5@   &   -  'System.Windows.Input.KeyboardNavigation   TabIndex$ 0��6~      TabNavigation$ Local=�5A   '    5B      �� 5C   "   $y�	Collapsed=�6.   5E      G� 5F      -   5        $��
txtNewPass��5F   &   $��4��68   $��1��6E   $��Stretch=�6U   $��2q�6s   #��[ 5G   &   $ 1��6~   $ Local=�5H   '    5I      �� 5J   "   $y�	Collapsed=�6.   5L      G� 5M      -   5        $��txtConfPass��5M   &   $��5��69   $��1��6F   $��Stretch=�6V   $��2q�6t   #��[ 5N   &   $ 2��6~   $ Local=�5O   '    5P      �� 5Q   "   $y�	Collapsed=�6.   5S      5T      5U      �� 5V      ���#656d90$��0.5,0.5,0.5,0.5q�6/   $��5,0,5,0q�6Q   $��2��6b   ._�5W      � $��30��6   .+�5X      �� -   5        $��	btnCancel��5X   -   $��23��6!   $��70��6>   $��Right=�6I   $��0,0,10,0q�6e   $��Center=�6w     NormalButton#��[ 5Y   %   $��Cancel��6c   $ 4��6u   $ Local=�5Z   '   �� 5[      -   5        $��btnOk��5[   [   $��23��6!   $��Right=�6-   $��0,0,90,0q�6I   $��Center=�6h   $��70��6�   #��[ 5\   %   $��Ok��6_   $ 3��6m   $ Local=�5]   '   5^      5_      �� 5`      $��30��6   ���#007edf$��Stretch=�6:   $	��Top=�6X   $��0,3,0,0q�6p   $��3��6�   ._�5c      � .+�5d      �� $��© 2013-2016 Pointel Inc��6$   $
��Left=�6C   $��Center=�6^   $��Calibri)�6y   $��12(�6�   
�������$��5,0,0,0q�6�   �� 5e      $��Right=�6H   $��Center=�6d   ���#EA3737 �� CornerRadius$ 	10,0,0,10��6�   y�6!   �� *TrialVisibility+._�5f      �� $��Calibri)�6F   $��Center=�6[   $��Center=�6x   $��12(�6�   
�������$��	15,2,10,2q�6�   ��6(   �� *TrailMessage+6�   5g      5h      5i      5j      5k      5l      5m      