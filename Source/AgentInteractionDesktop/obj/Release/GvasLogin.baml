   M S B A M L   `   `   `  ����   Agent.Interaction.Desktop)    #Agent.Interaction.Desktop.GvasLogin   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     -   $��gvasloginWindow��  � Title$  Login��$	��400��$
��Auto�� � WindowStyle$
 None=� � Icon$F @/Agent.Interaction.Desktop;component/Images/pointelicon16x16.ico�� � AllowsTransparency . 
������  � WindowStartupLocation$ CenterScreen=� � SizeToContent$ Height=� � 	Resources  �� MergedDictionaries �� Source   CantarellFontFamily	 �� 
TargetType
 �� Property �� Value��  �� $R L/Agent.Interaction.Desktop;component/Resources/StylesResourceDictionary.xaml?�%}   &	        '� A     *�;7/Agent.Interaction.Desktop;component/Resources/#Calibri)��� "	 �.��� 
 � ��$ Calibri)�.��� .+��� -   $��
MainBorder��$
��0.89q�$��8q�
�������._�� $��0q� � RowDefinitions �� $
��Auto ��� $��30 �.+�� $��5q�$��0�� �� $��50 ��� $
��Auto ��� $��40 �.+�� -   $��0��
������� � ColumnDefinitions �� $
��Auto ��� $
��Auto �.+��� -   $��image1��$��0��$��Right=�$��55,3,0,5q�$
��Auto��$��40��$G��A/Agent.Interaction.Desktop;component/Images/Pointel Softphone.png���� -   $��10,0,0,0q�$��1��$
��Left=� �� Orientation$ Vertical=�.+��� -   $��
loginTitle��$��18(�$��0,3,0,0q����#007edf$
��Bold"�  keyTitle#���  �� -   $��loginTitleversion��$��
140,-7,0,3q�  keyTitleVersion#���  $��12(��� -   $��borderContent��$��1�����#656d90$��0.5,0.5,0.5,0q�._��  �� $
��Auto �.+��  ��%Z   '��       'G� #     �� "	 ��.��� 
 � ��$ 23���� "	 G�.��� 
 � ��$ 23�� �� -	    �Name$ grdrow_error������ *ErrorRowHeight+�� �� $��30 ��� $��30 � �� $
��Auto �$	��120���� $��* �.+��� -
   $��
stkp_Error��$ 
Horizontal=�$��3q�$��0��$��2��
��� ���$��Stretch=�.+��� $��5,2,0,2q�$	��Top=�$;��5/Agent.Interaction.Desktop;component/Images/Error.png��$��20��$��20���� -   $��lblInformation��$��10,5,5,5q�$	��320��$
��Wrap=����� *ErrorMessage+�� $��2��$��1��$��Application Type��$��2,5,2,5q����#ADAAAD$��1q�.���� $ 
Horizontal=�$��5q�.+��� -   $��	chkb_gvas��$
��GVAS��$��80,0,0,0q��� -   $��	chkb_evas��$
��EVAS��$��20,0,0,0q��� -   $��	chkb_nvas��$
��NVAS��$��20,0,0,0q��� -   $��brd_Hide��$��2��$��2��$��2��$��2,0,2,0q� K� ZIndex$
 1000��
������� J� Opacity$
 0.35Q�$y�Visible=��� -   $��lblUsername��  keyUserName#���  $��0��$��2���� -   $��txtUserName�� �� 	UndoLimit$ 10��$��2��$��1��  DefaultCopyContextmenu � ContextMenu# [  �� SelectedText$  ��
 � Tag$ vinoth��$��Stretch=�$��2q�  controltempTextbox#��[ -  'System.Windows.Input.KeyboardNavigation   TabIndex$ 0��   TabNavigation$ Local=� �� 	MaxLength$	 255���� -   $��lblPassword��  keyPassword#���  $��0��$��3��G� -   $��txtPassword��$��3��$��2��$��Stretch=�$��2q�  controltempPasswordbox#��[ $ 1��$ Local=�# [ �� -   $��brdSubmition��$��2�����#656d90$	��0.5q�._�� $��Center=� �� $
��Auto �.+�� $��1�� �� $
��Auto ��� $��* ��� $��15 ��� $
��Auto �.+��� -   $��btnLogin��$��23��$��80��$��Right=�$��1��  NormalButton#��[ {�.  $ 8��$ Local=��� -   $��	btnCancel��$��23��$��80��$��Right=�$��0,0,5,0q�$��3��#��[ $ 9��$ Local=��� $��31�����#007edf$��Stretch=�$	��Top=�$��1��$��0q�._�� .+��� $��© 2013-2017 Pointel Inc��$��Calibri)�$
��Left=�$��Center=�$��12(�
�������$��5,0,0,0q��� $��Right=�$��Center=����#EA3737 �� CornerRadius$ 	10,0,0,10��y��� *TrialVisibility+._��� $��Calibri)�$��Center=�$��Center=�$��12(�
�������$��	15,2,10,2q����� *TrailMessage+