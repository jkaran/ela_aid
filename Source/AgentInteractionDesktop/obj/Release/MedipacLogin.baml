   M S B A M L   `   `   `  ����   Agent.Interaction.Desktop,    &Agent.Interaction.Desktop.MedipacLogin   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     -     � Title$  MedipacLogin��$	��400��$
��Auto�� � WindowStyle$
 None=� � Icon$F @/Agent.Interaction.Desktop;component/Images/pointelicon16x16.ico�� � AllowsTransparency . 
������  � WindowStartupLocation$ CenterScreen=� � SizeToContent$ Height=� � 	Resources  �� MergedDictionaries �� Source   CantarellFontFamily	 �� 
TargetType
 �� Property �� Value��  �� $R L/Agent.Interaction.Desktop;component/Resources/StylesResourceDictionary.xaml?�%}   &	        '� A     *�;7/Agent.Interaction.Desktop;component/Resources/#Calibri)��� "	 �.��� 
 � ��$ Calibri)�.��� .+��� -   $��
MainBorder��$��0��$
��0.89q�$��8q�
�������._��  � RowDefinitions �� -    �Name$ mainRowHeader��$��50 ��� -   $ mainRowError���� -   $ mainRowContent��$
��Auto ��� -   $ mainRowFooter��.+�� -   $��0��
������� � ColumnDefinitions �� $
��Auto ��� $
��Auto �.+��� -   $��image1��$��0��$��Right=�$��55,3,0,5q�$
��Auto��$��40��$G��A/Agent.Interaction.Desktop;component/Images/Pointel Softphone.png���� -	   $��10,0,0,0q�$��1��$
��Left=� �� Orientation$ Vertical=�.+��� -
   $��
loginTitle��$��18(�$��0,3,0,0q����#007edf$
��Bold"�  keyTitle#���  �� -   $��loginTitleversion��$��
140,-7,0,3q�  keyTitleVersion#���  $��12(��� -   $��
stkp_Error��$ 
Horizontal=�$��3q�$��1��
��� ���$��Stretch=�.+��� $��5,2,0,2q�$	��Top=�$;��5/Agent.Interaction.Desktop;component/Images/Error.png��$��20��$��20���� -   $��lblError��$��10,5,5,5q�$	��320��$
��Wrap=��� $��2��$ Vertical=�$��Stretch=�$��5q�.+��� $��Info Net Login�����#ADAAAD$��1q�.��� $��5q� �� $��28 ��� $��28 � �� $
��Auto �$	��120���� $��* �.+��� -   $��lblInfoNetUsername��$��	User Name���� -   $��txtInfoNetUserName��$��1�� �� 	UndoLimit$ 10��  DefaultCopyContextmenu � ContextMenu# [  �� SelectedText$  ��
 � Tag$ vinoth��$��Stretch=�$��2q�  controltempTextbox#��[ -  'System.Windows.Input.KeyboardNavigation   TabIndex$ 0��   TabNavigation$ Local=� �� 	MaxLength$	 255���� -   $��lblInfoNetPassword��$��Password��$��0��$��1��G� -   $��txtInfoNetPassword��$��1��$��2��$��Stretch=�$��2q�  controltempPasswordbox#��[ $ 1��$ Local=�# [ �� -   $��	gbMedipac��$��MediPac�����#ADAAAD$��1q�$��0,5,0,0q�.��� $��5q� �� $��28 ��� $��28 ��� $��28 � �� $
��Auto �$	��120���� $��* �.+��� -   $��lblMediPacUsername��$��	User Name���� -   $��txtMediPacUserName��$��1��$ 10��# [ $  ��$ vinoth��$��Stretch=�$��2q�#��[ $ 0��$ Local=�$	 255���� -   $��lblMediPacPassword��$��Password��$��0��$��1��G� -   $��txtMediPacPassword��$��1��$��2��$��Stretch=�$��2q�#��[ $ 1��$ Local=�# [ �� -   $��cbxKeepPlace��$��2��$��2��
���  ��$��5q�$��23��$��Center=�$ 6��$ Local=�$�� Same as Info Net���� $��Right=�$ 
Horizontal=�.+��� -   $��btnOk��$��23��$��5,5,5,0q�$��Center=�$��70��  NormalButton#��[ $��Ok���� -   $��	btnCancel��$��23��$��70��$��5,5,0,0q�$��Center=�#��[ $��Cancel���� $��31�����#007edf$��Stretch=�$	��Top=�$��3��$��0q�._�� .+��� $��© 2013-2016 Pointel Inc��$��Calibri)�$
��Left=�$��Center=�$��12(�
�������$��5,0,0,0q�