   M S B A M L   `   `   `  ���� U  QAgent.Interaction.Desktop, Version=5.0.3.31, Culture=neutral, PublicKeyToken=nullU/clr-namespace:Agent.Interaction.Desktop.Helpers!Agent.Interaction.Desktop.Helpers  [2clr-namespace:Agent.Interaction.Desktop.Converters$Agent.Interaction.Desktop.Converters  '    !Agent.Interaction.Desktop.DialPad   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     6my-http://schemas.microsoft.com/wpf/2008/toolkit  ;local/clr-namespace:Agent.Interaction.Desktop.Helpers  @convert2clr-namespace:Agent.Interaction.Desktop.Converters  :d2http://schemas.microsoft.com/expression/blend/2008  -     � 	Resources   �� MergedDictionaries �� Source   GroupHeaderStyle �� 
TargetType �� Property �� Value �� 
TargetType 9�Name 9� 
IsExpandedC   =Agent.Interaction.Desktop.Converters.DGRowForegroundConverter  RowForegroundA   ;Agent.Interaction.Desktop.Converters.DGRowIsEnableConverter  	RowEnableI  CAgent.Interaction.Desktop.Converters.UriUnescapeDataStringConverter  UriUnescapeDataString��  �� $R L/Agent.Interaction.Desktop;component/Resources/StylesResourceDictionary.xaml?�%�   &	        &	 �     &	 �     &	 �     �� " ��.���  � �� �� " ��.R�9� $	 exp�� . ���#007edf
����������� $��Calibri)�$
��Bold"�$��Normal&�
����������� *Name+.����       .��� $
��Auto��.+��� -   $��SelectionTab��	 K� ZIndex$	 0��$��	0,-3,0,-3q�$	��209��$	��275��.@��� -   $��
tabRequeue��$��Calibri)����� 
 �� Orientation$
 
Horizontal=�.+��� $��12��$C��=/Agent.Interaction.Desktop;component/Images/move.to.queue.png���� -   $�� Requeue��$��12(�$��Center=�$��Center=�.��� -   $��gdRequeueMain��
�������.+�� $��6q� � ColumnDefinitions �� $��60 ���  � RowDefinitions �� �� $
��Auto ��� .+��� -   $��txtRequeueSkillSet��$��Skill Set : ��$��0��$��0��$��Center=�$��Right=��� -   $��	cmbSkills��  �� IsTextSearchCaseSensitive .   �� IsTextSearchEnabled .  w� TextPath$ Skill��$��23��$	��135��$��0��$��1��$
��Left=�$��Center=����� *RequeueData+ �� 
GroupStyle (  "System.Windows.Controls.GroupStyle     HeaderTemplate �� .R��� $
��Bold"����� *Name+���� .R��� ���� *Skill+�� -   $��txtReqeueComments��$��Comments : ��$��1��$��0��$��Center=�$��Right=��� -	   $��txtComments�� �� 	UndoLimit$ 10��$
��Left=�$��Center=�$��1��$��1�� �� TextWrapping$
 Wrap=� �� 	MaxLength$	 256��$��80��$	��135�� �� MaxLines$ 10�� �� AcceptsReturn . $
��Auto=����� *RequeueComments+�� -
   $��btnClick��$��Requeue��$��25��$��60��$��0,0,0,20q�  NormalButton#��[ $��Center=�$��Center=�$��2��$��2���� -   $��	txt_Error��$��2��$��2��$��Bottom=�$��5q�
���  ��$
��Wrap=�$5��/No skill set configured or unable to read data.��$y�	Collapsed=��� -   $��tabDial��$��Calibri)����� $
 
Horizontal=�.+��� $��12��$A��;/Agent.Interaction.Desktop;component/Images/Voice.Short.png���� -   $�� Dial��$��12(�$��Center=�$��Center=�.��� 
������� �� �� ��  �� $��50 ��� �� �� �� �� $��40 �.+��� -   $��
txtNumbers��$ 10��$��0��$��3��$��5q� �� 
IsReadOnly . $��0q�
������ 
������ ���#007EDF$��Calibri)�$
��Bold"�$��35(�$��Center=����� *DialedNumbers+ �� *MaxDialDigits+�� -   $
��btn1��$��1��$��0��$��1��$��1q�#��[ �� -   $
��btn2��$��1��$��1��$��2��$��1q�#��[ �� -   $
��btn3��$��1��$��2��$��3��$��1q�#��[ �� -   $
��btn4��$��2��$��0��$��4��$��1q�#��[ �� -   $
��btn5��$��2��$��1��$��5��$��1q�#��[ �� -   $
��btn6��$��2��$��2��$��6��$��1q�#��[ �� -   $
��btn7��$��3��$��0��$��7��$��1q�#��[ �� -   $
��btn8��$��3��$��1��$��8��$��1q�#��[ �� -   $
��btn9��$��3��$��2��$��9��$��1q�#��[ �� -   $
��btns��$��4��$��0��$��*��$��1q�#��[ �� -   $
��btn0��$��4��$��1��$��0��$��1q�#��[ �� -   $
��btnh��$��4��$��2��$��#��$��1q�#��[ � $��5��$��3�� �� �� .+��� -   $
��Call��$��1q�$��0�����#4DBF4D  
CallButton#��[ $
��Dial���� -   $��btnClear��$��1q�$��1��  ClearButton#��[ $��	Backspace���� -   $��tabPhoneBook��$��Calibri)�y��� *IsPhoneBookEnabled+���� $
 
Horizontal=�.+��� $��12��$?��9/Agent.Interaction.Desktop;component/Images/PhoneBook.png���� -   $�� Phone Book��$��12(�$��Center=�$��Center=�.��� 
������� �� $��* �.+��� $��1��$��Contacts From��$��5q����#E3E3E3$��Calibri)�.���  �� $
��Auto ��� $
��Auto ��� $
��Auto ��� �� $
��Auto � �� �� .+��� -   $��ChkAgentLevel��$��0��$��5q�$��0��$��Local Contact��$��11(�
���  ���� -    $��ChkApplicationLevel��$��0��$��5q�$��1��$��Global Contact��$��11(�
����  ��� -!   $��ChkGroupLevel��$��1��$��5q�$��0��$��Group Contact��$��11(�
��� � ��� $��0,2,0,2q�$��2��$��2��$��20�����#adaaad$	��0.5q�._��  �� �� $
��Auto �.+��� -"   $��txtContactSearch��$ 10��
������ 
�������$��0,1,0,-1q��� $��1��$��2q�$<��6/Agent.Interaction.Desktop;component/Images/Search.png��$��15��)  #Microsoft.Windows.Controls.DataGrid  -#   $��
dgvContact�����#adaaad$��3��$��Stretch=�$��Stretch=�$��2��   AutoGenerateColumns .     HeadersVisibility$ Column=�   CanUserAddRows .     CanUserDeleteRows .     RowHeaderWidth$ 0Q�   HorizontalGridLinesBrush �#D6D7D6   VerticalGridLinesBrush �#D6D7D6   
IsReadOnly .     CanUserSortColumns  . !   Columns! 3  -Microsoft.Windows.Controls.DataGridTextColumn  /  )Microsoft.Windows.Controls.DataGridColumn"   Header$
" Name��#   Width8  2Microsoft.Windows.Controls.DataGridLengthConverter$# 75 $   CanUserSort$ . %   SortMemberPath$
% Name��4	  .Microsoft.Windows.Controls.DataGridBoundColumn& 	  Binding& �� *Name+'   	CellStyle' �� -
  'Microsoft.Windows.Controls.DataGridCell" 
 .�-$   ��  � ��$ 0q�( �� Triggers( P� ) P� Property) �@
 
IsSelected* P� Value* . . ���  � ��  HighlightedBrush# [ ��  � ��
 �   �  $" Number��$# * $ .  & �� *Number+' �� " 
 .�-%   ��  � ��$ 0q�( P� ) �@
 
IsSelected* . . ���  � ��# [ ��  � ��
 �   �  $ .  +   
Visibility$+ Hidden=�& �� *Type+,   RowStyle, �� ,  &Microsoft.Windows.Controls.DataGridRow"  .���  � �� `� - `� 	Converter#- [ .1��� . �� Path$
. Type��� $. Number���  � {� �� $. Number�/ �� 	Converter#/ [ ( �� 0 �� Binding0 �� $. TextBlock.Foreground�1 �� Value1 	LightGray.a���  � {� .  P� ) �@ 
IsSelected* . . ���  � ��# [ ��  � ��
 �   ��� -&   $��	lblStatus��$��4��$��2��$��11(�$��Stretch=�$��Center=�$��No contacts available��
����i��