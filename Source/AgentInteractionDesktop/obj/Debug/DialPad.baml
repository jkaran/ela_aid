   M S B A M L   `   `   `  ����U  QAgent.Interaction.Desktop, Version=5.0.3.27, Culture=neutral, PublicKeyToken=nullU/clr-namespace:Agent.Interaction.Desktop.Helpers!Agent.Interaction.Desktop.Helpers  5      [2clr-namespace:Agent.Interaction.Desktop.Converters$Agent.Interaction.Desktop.Converters  5      '    !Agent.Interaction.Desktop.DialPad   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      6my-http://schemas.microsoft.com/wpf/2008/toolkit  5      ;local/clr-namespace:Agent.Interaction.Desktop.Helpers  5      @convert2clr-namespace:Agent.Interaction.Desktop.Converters  5      :d2http://schemas.microsoft.com/expression/blend/2008  5      -   5          � 	Resources  5       �� MergedDictionaries �� Source   GroupHeaderStyle �� 
TargetType �� Property �� Value �� 
TargetType 9�Name 9� 
IsExpandedC   =Agent.Interaction.Desktop.Converters.DGRowForegroundConverter  RowForegroundA   ;Agent.Interaction.Desktop.Converters.DGRowIsEnableConverter  	RowEnableI  CAgent.Interaction.Desktop.Converters.UriUnescapeDataStringConverter  UriUnescapeDataString�� 5       5      �� 5      $R L/Agent.Interaction.Desktop;component/Resources/StylesResourceDictionary.xaml?�5      %�  &	        &	 G    &	 Z    &	 m    �� 5      " ��6$   .�5      ��  � �� 5      �� 5      " ��6   .R�5   	   9� $	 exp��6    . ���#007edf
���������5   
   �� 5      $��Calibri)�6+   $
��Bold"�6@   $��Normal&�6S   
���������6   �� *Name+5      .��5   
   �� 5   
   5   	   5!      5"        5#      6&     5$      6$     5%      6,   5&      .��5(      � $
��Auto��6   .+�5)      �� -   5        $��SelectionTab��5)      	 K� ZIndex$	 0��6#   $��	0,-3,0,-3q�64   $	��209��6G   $	��275��6S   .@�5,      �� -   5        $��
tabRequeue��5,   %   $��Calibri)�6   ��5-      �� 5.      
 �� Orientation$
 
Horizontal=�6   .+�5/      �� $��12��6   $C��=/Agent.Interaction.Desktop;component/Images/move.to.queue.png��6   �� 50      -   5        $�� Requeue��50      $��12(�6"   $��Center=�60   $��Center=�6K   6h   51      .��53   	   � -   5        $��gdRequeueMain��53      
�������.+�54   
   � $��6q�6    � ColumnDefinitions 55      �� 56      $��60 �6   6*   �� 57      6    � RowDefinitions 59      �� 5:      6   �� 5;      $
��Auto �6   6*   �� 5<      6   .+�5>      �� -   5        $��txtRequeueSkillSet��5>      $��Skill Set : ��6/   $��0��6C   $��0��6P   $��Center=�6`   $��Right=�6{   �� 5?      -   5        $��	cmbSkills��5?        �� IsTextSearchCaseSensitive .   �� IsTextSearchEnabled .  w� TextPath$ Skill��6�   $��23��5@      $	��135��6   $��0��6#   $��1��60   $
��Left=�6@   $��Center=�6[   ��5?   %   �� *RequeueData+ �� 
GroupStyle 5A      (  "System.Windows.Controls.GroupStyle  5B         HeaderTemplate 5C      �� 5D      .R�5E      �� $
��Bold"�6   ��6+   �� *Name+5F      5H      ��5J      �� 5K      .R�5L      �� ��6   �� *Skill+5M      5O      �� 5Q      -   5        $��txtReqeueComments��5Q      $��Comments : ��6.   $��1��6A   $��0��6N   $��Center=�6^   $��Right=�6y   �� 5R      -	   5        $��txtComments��5R   "    �� 	UndoLimit$ 10��6   $
��Left=�65   $��Center=�6q   $��1��6�   $��1��6�    �� TextWrapping$
 Wrap=�6�    �� 	MaxLength$	 256��6�   $��80��5S   
   $	��135��6    �� MaxLines$ 10��6"    �� AcceptsReturn . $
��Auto=�6E   ��5R   P   �� *RequeueComments+5S   E   �� 5U      -
   5        $��btnClick��5U      $��Requeue��6"   $��25��64   $��60��6@     NormalButton#��[ 6K   $
��Left=�6q   $��2��6�   $��1��6�   6�   5V      5W   
   5X      �� 5Y      -   5        $��tabDial��5Y   J   $��Calibri)�6   ��5Z      �� 5[      $
 
Horizontal=�6   .+�5\      �� $��12��6   $A��;/Agent.Interaction.Desktop;component/Images/Voice.Short.png��6   �� 5]      -   5        $�� Dial��5]      $��12(�6   $��Center=�6-   $��Center=�6H   6e   5^      .��5`      � 
������� 5a      �� 5b      6   �� 5c      6   �� 5d      6    5f      �� 5g      $��50 �6   6$   �� 5h      6   �� 5i      6   �� 5j      6   �� 5k      6   �� 5l      $��40 �6   6$   .+�5n      �� -   5        $��
txtNumbers��5n      $ 10��6   $��0��60   $��3��6=   $��5q�6Q    �� 
IsReadOnly . $��0q�6n   
������ 
������ ���#007EDF$��Calibri)�5o      $
��Bold"�6   $��35(�6S   $��Center=�6b   ��5n   �   �� *DialedNumbers+ 5o   /   �� *MaxDialDigits+6�   �� 5p      -   5        $
��btn1��5p      $��1��6   $��0��6'   $��1��67   $��1q�6C   #��[ 6N   6�   �� 5q      -   5        $
��btn2��5q      $��1��6   $��1��6'   $��2��67   $��1q�6C   #��[ 6N   6�   �� 5r      -   5        $
��btn3��5r      $��1��6   $��2��6'   $��3��67   $��1q�6C   #��[ 6N   6�   �� 5s      -   5        $
��btn4��5s      $��2��6   $��0��6'   $��4��67   $��1q�6C   #��[ 6N   6�   �� 5t      -   5        $
��btn5��5t      $��2��6   $��1��6'   $��5��67   $��1q�6C   #��[ 6N   6�   �� 5u      -   5        $
��btn6��5u      $��2��6   $��2��6'   $��6��67   $��1q�6C   #��[ 6N   6�   �� 5v      -   5        $
��btn7��5v      $��3��6   $��0��6'   $��7��67   $��1q�6C   #��[ 6N   6�   �� 5w      -   5        $
��btn8��5w      $��3��6   $��1��6'   $��8��67   $��1q�6C   #��[ 6N   6�   �� 5x      -   5        $
��btn9��5x      $��3��6   $��2��6'   $��9��67   $��1q�6C   #��[ 6N   6�   �� 5y      -   5        $
��btns��5y      $��4��6   $��0��6'   $��*��67   $��1q�6C   #��[ 6N   6�   �� 5z      -   5        $
��btn0��5z      $��4��6   $��1��6'   $��0��67   $��1q�6C   #��[ 6N   6�   �� 5{      -   5        $
��btnh��5{      $��4��6   $��2��6'   $��#��67   $��1q�6C   #��[ 6N   6�   � 5|      $��5��6   $��3��6    5}      �� 5~   	   6   �� 5   	   6   .+�5�      �� -   5        $
��Call��5�      $��1q�6   $��0��6&   ���#4DBF4D  
CallButton#��[ 6K   $
��Dial��6o   6�   �� 5�      -   5        $��btnClear��5�      $��1q�6%   $��1��61     ClearButton#��[ 6\   $��	Backspace��6�   6�   5�      5�      5�      �� 5�      -   5        $��tabPhoneBook��5�   #   $��Calibri)�6   y�67   �� *IsPhoneBookEnabled+��5�      �� 5�      $
 
Horizontal=�6   .+�5�      �� $��12��6   $?��9/Agent.Interaction.Desktop;component/Images/PhoneBook.png��6   �� 5�      -   5        $�� Phone Book��5�      $��12(�6%   $��Center=�63   $��Center=�6N   6k   5�      .��5�      � 
������� 5�      �� 5�      $��* �6   6    .+�5�      �� $��1��6   $��Contacts From��6   $��5q�62   ���#E3E3E3$��Calibri)�6S   .��5�      �  5�      �� 5�      $
��Auto �6   6&   �� 5�      $
��Auto �6   6&   �� 5�      $
��Auto �6   6&   �� 5�      6   �� 5�      $
��Auto �6   6&    5�      �� 5�      6   �� 5�      6   .+�5�      �� -   5        $��ChkAgentLevel��5�   o   $��0��6   $��5q�6   $��0��6(   $��Local Contact��68   $��11(�6P   
���  ��6�   �� 5�      -   5        $��ChkApplicationLevel��5�   q   $��0��6   $��5q�6   $��1��6(   $��Global Contact��68   $��11(�6Q   
����  �6�   �� 5�      -    5        $��ChkGroupLevel��5�   q   $��1��6   $��5q�6   $��0��6(   $��Group Contact��68   $��11(�6P   
��� � �6�   �� 5�      $��0,2,0,2q�6   $��2��6   $��2��6,   $��20��6@   ���#adaaad$	��0.5q�6b   ._�5�      �  5�   	   �� 5�   
   6   �� 5�   
   $
��Auto �6   6*   .+�5�   	   �� -!   5        $��txtContactSearch��5�       $ 10��6   
������ 
�������$��0,1,0,-1q�6e   6�   �� 5�   	   $��1��6   $��2q�6   $<��6/Agent.Interaction.Desktop;component/Images/Search.png��6*   $��15��6j   5�   	   5�      )  #Microsoft.Windows.Controls.DataGrid  5�      -"   5        $��
dgvContact��5�   )   ���#adaaad$��3��6;   $��Stretch=�6H   $��Stretch=�6f   $��2��6�      AutoGenerateColumns .     HeadersVisibility$ Column=�6�      CanUserAddRows .     CanUserDeleteRows .     RowHeaderWidth$ 0Q�6�      HorizontalGridLinesBrush �#D6D7D6   VerticalGridLinesBrush �#D6D7D6   
IsReadOnly .     CanUserSortColumns  . !   Columns! 5�      3  -Microsoft.Windows.Controls.DataGridTextColumn  5�   	   /  )Microsoft.Windows.Controls.DataGridColumn"   Header$
" Name��6   #   Width8  2Microsoft.Windows.Controls.DataGridLengthConverter$# 75 6F   $   CanUserSort$ . %   SortMemberPath$
% Name��6d   4	  .Microsoft.Windows.Controls.DataGridBoundColumn& 	  Binding& 6-   �� *Name+'   	CellStyle' 5�   
   �� 5�      -
  'Microsoft.Windows.Controls.DataGridCell" 
 6   .�5�      -#   5        �� 5�       � ��$ 0q�6.   ( �� Triggers( 5�      P� 5�      ) P� Property) �@
 
IsSelected* P� Value* . . �5�      ��  � ��  HighlightedBrush# [ 6+   �� 5�       � ��
 �   �6+   5�      5�      5�   
     5�   	   $" Number��6   $# * 6J   $ .  & 6/   �� *Number+' 5�   
   �� 5�      " 
 6   .�5�      -$   5        �� 5�       � ��$ 0q�6.   ( 5�      P� 5�      ) �@
 
IsSelected* . . �5�      ��  � ��# [ 6+   �� 5�       � ��
 �   �6+   5�      5�      5�   
     5�   	   $ .  +   
Visibility$+ Hidden=�6L   & 6   �� *Type+6L   ,   RowStyle, 5�      �� 5�   	   ,  &Microsoft.Windows.Controls.DataGridRow"  6   .�5�   
   ��  � �� 5�      `� 5�      - `� 	Converter#- [ 6   .1�5�      �� . �� Path$
. Type�6   �� 5�      $. Number�6   5�      5�      �� 5�   
    � {� 6&   �� $. Number�/ �� 	Converter#/ [ ( 5�   
   �� 5�      0 �� Binding0 6   �� $. TextBlock.Foreground�1 �� Value1 	LightGray6E   .a�5�      ��  � {� .  6(   5�      P� 5�      ) �@ 
IsSelected* . . �5�      ��  � ��# [ 6)   �� 5�       � ��
 �   �6)   5�      5�   
   5�      �� 5�      -%   5        $��	lblStatus��5�      $��4��6   $��2��6+   $��11(�6?   $��Stretch=�6M   $��Center=�6k   $��No contacts available��6�   
����i��6�   5�      5�      5�      5�      5�      5�      5�      