   M S B A M L   `   `   `  ���� U  QAgent.Interaction.Desktop, Version=5.0.3.31, Culture=neutral, PublicKeyToken=nullc6clr-namespace:Agent.Interaction.Desktop.CustomControls(Agent.Interaction.Desktop.CustomControls  &     Agent.Interaction.Desktop.Skills   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     PCustomControl6clr-namespace:Agent.Interaction.Desktop.CustomControls     -   $��
UserSkills��  � AllowsTransparency  .  � WindowStyle$
 None=� � WindowStartupLocation$ CenterScreen=� � ShowInTaskbar .   � Topmost . $
��Auto��$	��320�� � SizeToContent$ Height=� � 
ResizeMode$ NoResize=� � Icon$I C/Agent.Interaction.Desktop;component/Images/Pointel%20Softphone.png�� � 	Resources 	 �� MergedDictionaries
 �� Source   BlinkBorder �� 
TargetName �� TargetProperty W� From	 W� To g� RepeatBehavior g� AutoReverse g� Duration�� 	 �� $R
 L/Agent.Interaction.Desktop;component/Resources/StylesResourceDictionary.xaml?�%i   &	        �� -   .��W� $ 
MainBorder��$ Opacity�$ 1X�$ 0X�$ 4x�� . $ 0:0:0.1D�.��� .+��� -   $��
MainBorder��$��1q����#656d90._��  � RowDefinitions �� $
��Auto ��� $
��Auto ��� $��50 �.+��� -   $��lblTitle��$��0��$��14(�$��Stretch=�$��Calibri)����#007edf
��������� $��1��$��2�����#656d90$	��0.5q�$��5q�� -   $��mainGrid��$��1��$��50��$��10q� � ColumnDefinitions �� $��75 ���  �� $
��Auto ��� $
��Auto ��� -    �Name$ errorRowHeight��$��0 �.+��� -   $��txtSkillName��$��0��$��0��$��
Skill Name��$��Stretch=�$��Center=�$��14(�$��Calibri)�$��5q�@   :Agent.Interaction.Desktop.CustomControls.AutoCompleteBoxEx  -     Name$ cmbSkillName��@   :Agent.Interaction.Desktop.CustomControls.AutoCompleteBoxEx   GotFocus$ AutoComplete_GotFocus��   SelectionChanged$# cmbSkillName_SelectionChanged��   PreviewKeyUp$ PreviewKeyUp��$��0��$��1��$��5q�$��23�����#ADAAAD
�������
���   �$��Calibri)�$��12(�$��Center=�$��Stretch=�-  'System.Windows.Input.KeyboardNavigation   TabIndex$ 0��   TabNavigation$ Local=�p lSystem.Windows.Controls.Input.Toolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35-  'System.Windows.Controls.AutoCompleteBox   SelectedItem �� *Place+ �� Mode$ TwoWay=�   ItemsSource �� *AutoComplete+   ItemTemplate �� .R��� $��innertxt��
���   �$
��Auto��$��2q� �� Padding$ 2,0,5,0q�$��Calibri)�$��12(�����    �� 
TargetType! �� Property" �� Value# �� Triggers$ P� Property% P� Value  HorizontalScrollBar  VerticalScrollBar��%z  '�       '�� #     7  7  �� "  �.��� ! � ��$" 85���� "  ��.��� ! �@J�SnapsToDevicePixels" . �� ! �@�OverridesDefaultStyle" . # P� $ �@��Orientation$% 
Horizontal=�. ��� ! � ��$
" Auto���� ! � ��$" 18���� ! � ��8"   P� $ �@��Orientation$% Vertical=�. ��� ! � ��$" 18���� ! � ��$
" Auto���� ! � ��8"  �� -	   $��txtlevelname��$��0��$��1��$��Skill Level��$��Stretch=�$��Center=�$��14(�$��Calibri)�$��5q�  -
   $ cmbSkillLevel��$ AutoComplete_GotFocus��$  cmbSkillLevel_PreviewKeyUp��&   PreviewKeyDown$& cmbSkillValue_KeyDown��$��1��$��1��$��5q�$��23�����#ADAAAD
�������
���   �$��Calibri)�$��12(�$ 1��$ Local=�$��Center=�$��Stretch=� �� *Place+$ TwoWay=� �� *SkillLevelSource+ �� .R��� $��innertxt��
���   �$
��Auto��$��2q�$ 2,0,5,0q�$��Calibri)�$��12(�����  ��%z  '�       '�� #     7  7  �� "  �.��� ! � ��$" 85���� "  ��.��� ! �@J�SnapsToDevicePixels" . �� ! �@�OverridesDefaultStyle" . # P� $ �@��Orientation$% 
Horizontal=�. ��� ! � ��$
" Auto���� ! � ��$" 18���� ! � ��8"   P� $ �@��Orientation$% Vertical=�. ��� ! � ��$" 18���� ! � ��$
" Auto���� ! � ��8"  �� -   $��
stkp_Error��' �� Orientation$' 
Horizontal=�$��3q�$��3��$��2��
��� ���$��Stretch=�.+��� $��5,2,0,2q�$	��Top=�$;��5/Agent.Interaction.Desktop;component/Images/Error.png��$��20��$��20���� -   $��txtError��$��Bottom=�$��4q�$	��240��$
��Wrap=�� $��2�� �� $��75 ��� .+��� $��2��$' 
Horizontal=�$��Center=�.+��� -   $��btnOk��$��0,8,10,0q�$
��Left=�$	��Top=�$ 2��$ Local=�$��Ok��$��23��$��80��  NormalButton#��[ �� -   $��	btnCancel��$��10,8,0,0q�$��Right=�$	��Top=�$ 3��$ Local=�$��Cancel��$��23��$��80��#��[ 