   M S B A M L   `   `   `  ���� p  lSystem.Windows.Controls.Input.Toolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35pTclr-namespace:System.Windows.Controls;assembly=System.Windows.Controls.Input.ToolkitSystem.Windows.Controls  U QAgent.Interaction.Desktop, Version=5.0.3.31, Culture=neutral, PublicKeyToken=nullc6clr-namespace:Agent.Interaction.Desktop.CustomControls(Agent.Interaction.Desktop.CustomControls .   (Agent.Interaction.Desktop.ChangePassword   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     cmyTclr-namespace:System.Windows.Controls;assembly=System.Windows.Controls.Input.Toolkit     PCustomControl6clr-namespace:Agent.Interaction.Desktop.CustomControls     -   $��channelWindow��  � Title$  Login�� � WindowStyle$
 None=� � Icon$F @/Agent.Interaction.Desktop;component/Images/pointelicon16x16.ico�� � ShowInTaskbar .   � AllowsTransparency . 
������  � WindowStartupLocation$ CenterScreen=�$	��400��$	��400�� � SizeToContent$ Height=� � 	Resources  �� MergedDictionaries	 �� Source��  �� $R	 L/Agent.Interaction.Desktop;component/Resources/StylesResourceDictionary.xaml?�%    .��� .+��� -   $��
MainBorder��$
��0.89q�$��8q�
�������._�� 
 � RowDefinitions
 �� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto �.+�� -   $��0��
�������.+��� -   $��5,0,0,0q�
�������$��Center=� �� Orientation$ 
Horizontal=�.+��� -   $��image1��$
��Auto��$��40��$G��A/Agent.Interaction.Desktop;component/Images/Pointel Softphone.png���� $ Vertical=�.+��� -   $��18(�$��0,3,0,0q����#007edf$
��Bold"�   keyTitle#���   $��Calibri)��� -   $��channelTitleversion��$��
140,-7,0,3q�  keyTitleVersion#���  $��Calibri)�$��12(��� $��0.5,0.5,0.5,0q����#656d90$��5,0,5,0q�$��1��._��  � ColumnDefinitions �� $
��161* ��� $
��237* �
 �� $
��Auto ��� ���� *CSErrorRowHeight+�� $��28 ��� $��28 ��� $��28 ��� $��28 �.+��� -   $��lblUserName��$!��Please enter a new password��$��14(�$��2��$��2q����#E1F2FA�� -	   $��
stkp_Error��$ 
Horizontal=�$��1��$��2��$��3,0,3,3q�
��� ���$��Stretch=�.+��� $��5,2,0,2q�$	��Top=�$;��5/Agent.Interaction.Desktop;component/Images/Error.png��$��20��$��20���� -
   $��lblInformation��$��10,5,5,5q�$	��300��$
��Wrap=����� *CSErrorMessage+�� $��
User Name:��$��2��$��Center=��� $��Old Password:��$��3��$��Center=��� $��New Password:��$��4��$��Center=��� $��Confirm Password:��$��5��$��Center=��� $��2��$��1��  DefaultCopyContextmenu � ContextMenu# [ 
 � Tag$ vinoth��$��Stretch=�$��2q����#d3d3d3���� *
UserName+G� -   $��
txtOldPass��$��3��$��1��$��Stretch=�$��2q�  controltempPasswordbox#��[ -  'System.Windows.Input.KeyboardNavigation   TabIndex$ 0��   TabNavigation$ Local=� �� $y�	Collapsed=�G� -   $��
txtNewPass��$��4��$��1��$��Stretch=�$��2q�#��[ $ 1��$ Local=� �� $y�	Collapsed=�G� -   $��txtConfPass��$��5��$��1��$��Stretch=�$��2q�#��[ $ 2��$ Local=� �� $y�	Collapsed=��� ���#656d90$��0.5,0.5,0.5,0.5q�$��5,0,5,0q�$��2��._�� $��30��.+��� -   $��	btnCancel��$��23��$��70��$��Right=�$��0,0,10,0q�$��Center=�  NormalButton#��[ $��Cancel��$ 4��$ Local=��� -   $��btnOk��$��23��$��Right=�$��0,0,90,0q�$��Center=�$��70��#��[ $��Ok��$ 3��$ Local=��� $��30�����#007edf$��Stretch=�$	��Top=�$��0,3,0,0q�$��3��._�� .+��� $��© 2013-2016 Pointel Inc��$
��Left=�$��Center=�$��Calibri)�$��12(�
�������$��5,0,0,0q��� $��Right=�$��Center=����#EA3737 �� CornerRadius$ 	10,0,0,10��y��� *TrialVisibility+._��� $��Calibri)�$��Center=�$��Center=�$��12(�
�������$��	15,2,10,2q����� *TrailMessage+