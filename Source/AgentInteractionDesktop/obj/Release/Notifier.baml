   M S B A M L   `   `   `  ���� S  OPointel.TaskbarNotifier, Version=5.0.2.50, Culture=neutral, PublicKeyToken=nullbFclr-namespace:Pointel.TaskbarNotifier;assembly=Pointel.TaskbarNotifierPointel.TaskbarNotifier  U QAgent.Interaction.Desktop, Version=5.0.3.31, Culture=neutral, PublicKeyToken=null[2clr-namespace:Agent.Interaction.Desktop.Converters$Agent.Interaction.Desktop.Converters (   "Agent.Interaction.Desktop.Notifier   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     [NotifierFclr-namespace:Pointel.TaskbarNotifier;assembly=Pointel.TaskbarNotifier     6my-http://schemas.microsoft.com/wpf/2008/toolkit  @convert2clr-namespace:Agent.Interaction.Desktop.Converters  R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     -     � AllowsTransparency  .  � WindowStyle$
 None=�
������  � Title$ Notifier��$
��Auto��$	��400�� � SizeToContent$ Height=� � 	Resources  �� MergedDictionaries �� SourceI CAgent.Interaction.Desktop.Converters.UriUnescapeDataStringConverter   UriUnescapeDataString��  �� $R L/Agent.Interaction.Desktop;component/Resources/StylesResourceDictionary.xaml?�%   &	          .��� .+��� -   $��
MainBorder��$
��0.89q�$��8q�
�������._��  � RowDefinitions �� $
��Auto ��� �� $
��Auto �.+��� $��0��$��14(�$��Calibri)����#007edf
����������� *	TitleText+9� -   $��1��  SimpleExpanderTemp#��[  9� 
IsExpanded . $��RightToLeft=�$	��100������ $��	Call Data��$��Calibri)����#0878ef$��Right=�$	��355��.��� -   $��expGrid��$
��Auto�����#1d4380$��5,0,5,0q�.+�)  #Microsoft.Windows.Controls.DataGrid  -   $��
DGCallData��	   RowHeaderWidth$	 0Q�$��LeftToRight=�
�������$	��Top=�$��Stretch=�
   HorizontalGridLinesBrush
 �#D6D7D6   VerticalGridLinesBrush �#D6D7D6   AutoGenerateColumns .     CanUserAddRows .     
IsReadOnly .    CanUserDeleteRows .  $
��Auto=����� *NotifyCallDataView+   Columns 3  -Microsoft.Windows.Controls.DataGridTextColumn  /  )Microsoft.Windows.Controls.DataGridColumn   Header$	 Key��   Width8  2Microsoft.Windows.Controls.DataGridLengthConverter$ 85    
IsReadOnly . 4  .Microsoft.Windows.Controls.DataGridBoundColumn   Binding �� *Key+  $ Value��$ *  .  �� *Value+ �� 	Converter# [  �� -   $��btnRight��$��0,5,5,5q�$��2��$��Right=�$��70��$��30���� -   $��btnLeft��$��0,5,80,5q�$��2��$��Right=�$��70��$��30��  
CallButton#��[ 