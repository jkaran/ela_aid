   M S B A M L   `   `   `  ����U  QAgent.Interaction.Desktop, Version=5.0.3.27, Culture=neutral, PublicKeyToken=nullc6clr-namespace:Agent.Interaction.Desktop.CustomControls(Agent.Interaction.Desktop.CustomControls  5   	   &     Agent.Interaction.Desktop.Skills   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   PCustomControl6clr-namespace:Agent.Interaction.Desktop.CustomControls     5   	   -   5        $��
UserSkills��5   T     � AllowsTransparency  .  � WindowStyle$
 None=�5   #    � WindowStartupLocation$ CenterScreen=�67    � ShowInTaskbar .   � Topmost . $
��Auto��5   
   $	��320��6    � SizeToContent$ Height=�6$    � 
ResizeMode$ NoResize=�5   V    � Icon$I C/Agent.Interaction.Desktop;component/Images/Pointel%20Softphone.png��5   	    � 	Resources 5      	 �� MergedDictionaries
 �� Source   BlinkBorder �� 
TargetName �� TargetProperty W� From	 W� To g� RepeatBehavior g� AutoReverse g� Duration�� 5	   
   	 5
      �� 5      $R
 L/Agent.Interaction.Desktop;component/Resources/StylesResourceDictionary.xaml?�5      %�   &	        �� 5      -   5        .��5      W� $ 
MainBorder��6"   $ Opacity�5      $ 1X�5      $ 0X�5      $ 4x��5       . $ 0:0:0.1D�5      5      5      .��5      � .+�5   
   �� -   5        $��
MainBorder��5   ;   $��1q�6   ���#656d90._�5      �  � RowDefinitions 5      �� 5      $
��Auto �6$   64   �� 5      $
��Auto �6$   64   �� 5      $��50 �6$   62   .+�5!      �� -   5        $��lblTitle��5!   %   $��0��6   $��14(�65   $��Stretch=�6C   $��Calibri)�6a   ���#007edf
�������5"   X   �� 5#      $��1��6   $��2��6&   ���#656d90$	��0.5q�6M   $��5q�6c   � 5$      -   5        $��mainGrid��5$      $��1��6'   $��50��64   $��10q�6D    � ColumnDefinitions 5%      �� 5&      $��75 �6+   68   �� 5'      6-    5)      �� 5*      $
��Auto �6(   68   �� 5+      $
��Auto �6(   68   �� 5-      -   5         �Name$ errorRowHeight��5-   (   $��0 �6>   6K   .+�5/      �� -   5        $��txtSkillName��5/       $��0��64   $��0��6D   $��
Skill Name��6R   $��Stretch=�6d   $��Center=�6�   $��14(�50      $��Calibri)�6   $��5q�64   @   :Agent.Interaction.Desktop.CustomControls.AutoCompleteBoxEx  52      -   5          Name$ cmbSkillName��52   6   @   :Agent.Interaction.Desktop.CustomControls.AutoCompleteBoxEx   GotFocus$ AutoComplete_GotFocus��53   r      SelectionChanged$# cmbSkillName_SelectionChanged��54   p      PreviewKeyUp$ PreviewKeyUp��55   .   $��0��52   L   $��1��6Y   $��5q�6i   $��23��6t   ���#ADAAAD
�������
���   �$��Calibri)�53   O   $��12(�6d   $��Center=�54   7   $��Stretch=�6R   -  'System.Windows.Input.KeyboardNavigation   TabIndex$ 0��55   J      TabNavigation$ Local=�6j   p lSystem.Windows.Controls.Input.Toolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35-  'System.Windows.Controls.AutoCompleteBox   SelectedItem 52   �   �� *Place+ �� Mode$ TwoWay=�   ItemsSource 54      �� *AutoComplete+   ItemTemplate 56      �� 57      .R�58   "   �� $��innertxt��6,   
���   �$
��Auto��6`   $��2q�6m    �� Padding$ 2,0,5,0q�6x   $��Calibri)�6�   $��12(�6�   ��6<   �� 6�   59       5;        �� 
TargetType! �� Property" �� Value# �� Triggers$ P� Property% P� Value  HorizontalScrollBar  VerticalScrollBar��%]  '�       '�� H     7  7  �� 5<      "  �6$   .�5=   "   �� ! � ��$" 85��6>   5>      �� 5?      "  ��6$   .�5@   "   �� ! �@J�SnapsToDevicePixels" . 6H   �� 5A   "   ! �@�OverridesDefaultStyle" . 6J   # 5B   "   P� 5C   &   $ �@��Orientation$% 
Horizontal=�6E   . �5D   *   �� ! � ��$
" Auto��6B   �� 5E   *   ! � ��$" 18��6C   �� 5F   *   ! � ��8"   6E   5G   '   P� 5H   &   $ �@��Orientation$% Vertical=�6E   . �5I   *   �� ! � ��$" 18��6B   �� 5J   *   ! � ��$
" Auto��6C   �� 5K   *   ! � ��8"  6E   5L   '   5N      5O      5P      �� 5T      -	   5        $��txtlevelname��5T       $��0��64   $��1��6D   $��Skill Level��6R   $��Stretch=�6e   $��Center=�6�   $��14(�5U      $��Calibri)�6   $��5q�64     5W      -
   5        $ cmbSkillLevel��5W   6   $ AutoComplete_GotFocus��6�   $  cmbSkillLevel_PreviewKeyUp��5Y   g   &   PreviewKeyDown$& cmbSkillValue_KeyDown��6�   $��1��5W   M   $��1��6Z   $��5q�6j   $��23��6u   ���#ADAAAD
�������
���   �$��Calibri)�5X   N   $��12(�6c   $ 1��6�   $ Local=�6�   $��Center=�5Y   .   $��Stretch=�6I    5W   �   �� *Place+$ TwoWay=� 5X   q   �� *SkillLevelSource+ 5Z      �� 5[      .R�5\   "   �� $��innertxt��6,   
���   �$
��Auto��6`   $��2q�6m   $ 2,0,5,0q�6x   $��Calibri)�6�   $��12(�6�   ��6<   �� 6�   5]       5_      ��%]  '�       '�� H     7  7  �� 5`      "  �6$   .�5a   "   �� ! � ��$" 85��6>   5b      �� 5c      "  ��6$   .�5d   "   �� ! �@J�SnapsToDevicePixels" . 6H   �� 5e   "   ! �@�OverridesDefaultStyle" . 6J   # 5f   "   P� 5g   &   $ �@��Orientation$% 
Horizontal=�6E   . �5h   *   �� ! � ��$
" Auto��6B   �� 5i   *   ! � ��$" 18��6C   �� 5j   *   ! � ��8"   6E   5k   '   P� 5l   &   $ �@��Orientation$% Vertical=�6E   . �5m   *   �� ! � ��$" 18��6B   �� 5n   *   ! � ��$
" Auto��6C   �� 5o   *   ! � ��8"  6E   5p   '   5r      5s      5t      �� 5z      -   5        $��
stkp_Error��5z   g   ' �� Orientation$' 
Horizontal=�6!   $��3q�6:   $��3��6F   $��2��6S   
��� ���$��Stretch=�6�   .+�5{      �� $��5,2,0,2q�6    $	��Top=�61   $;��5/Agent.Interaction.Desktop;component/Images/Error.png��6I   $��20��6�   $��20��6�   �� 5|      -   5        $��txtError��5|   $   $��Bottom=�64   $��4q�6O   $	��240��6Z   $
��Wrap=�6f   5}      5      5�      � 5�      $��2��6    5�      �� 5�      $��75 �6+   68   �� 5�      6-   .+�5�      �� $��2��6!   $' 
Horizontal=�61   $��Center=�6J   .+�5�      �� -   5        $��btnOk��5�   !   $��0,8,10,0q�6.   $
��Left=�6@   $	��Top=�6[   $ 2��6s   $ Local=�6�   $��Ok��6�   $��23��6�   $��80��6�     NormalButton#��[ 6�   6  �� 5�      -   5        $��	btnCancel��5�   !   $��10,8,0,0q�62   $��Right=�6E   $	��Top=�6a   $ 3��6y   $ Local=�6�   $��Cancel��6�   $��23��6�   $��80��6�   #��[ 6�   6*  5�      5�      5�      5�      5�      5�      