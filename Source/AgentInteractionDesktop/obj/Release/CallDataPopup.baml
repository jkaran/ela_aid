   M S B A M L   `   `   `  ���� U  QAgent.Interaction.Desktop, Version=5.0.3.31, Culture=neutral, PublicKeyToken=null[2clr-namespace:Agent.Interaction.Desktop.Converters$Agent.Interaction.Desktop.Converters  -    'Agent.Interaction.Desktop.CallDataPopup   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Fconvert2clr-namespace:Agent.Interaction.Desktop.Converters     6my-http://schemas.microsoft.com/wpf/2008/toolkit  -   $��CallDataWin��  � 
ResizeMode$  	CanResize=� � Title$ CallDataPopup�� � WindowStyle$
 None=� � Icon$E ?/Agent.Interaction.Desktop;component/Images/Call Attachment.png�� � AllowsTransparency . 
������  � WindowStartupLocation$ CenterScreen=� � ShowInTaskbar . $	��420��$	��350��$	��350��$	��250�� � 	Resources  �� MergedDictionaries	 �� SourceD   >Agent.Interaction.Desktop.Converters.DGCellForeGroundConverter   CellForegroundG   AAgent.Interaction.Desktop.Converters.DGCellCellMouseOverConverter  CellMouseOverI  CAgent.Interaction.Desktop.Converters.UriUnescapeDataStringConverter  UriUnescapeDataString
 �� 
TargetType  	baseStyle �� Property �� Value �� BasedOn��  �� $R	 L/Agent.Interaction.Desktop;component/Resources/StylesResourceDictionary.xaml?�%w   &	        &	      &	 
     &	      '�� 2     7        �� "
 ��.���  � ��$ 12(��� "
 ��8   .��� .+��� -   $��
MainBorder��$
��0.89q�$��8q�
������� %� IsVirtualizing .  %� VirtualizationMode$ 	Recycling=�._��  . $ 	Recycling=� � RowDefinitions �� $��* ��� $��5 �.+��� -   $��RightSideRect��$��2��
������ $��3�� K� ZIndex$
 1000��$��Right=�$��Stretch=��� -   $��RightbottomSideRect��$��1��
������ $��8��$��5��$
 2200��$��Right=�$��Bottom=��� -   $��BottomSideRect��$��1��
������ $��3��$
 1000��$��Stretch=�$��Bottom=�� 
������� . $ 	Recycling=� �� $
��Auto ��� .+�� -   $��0��
������� � ColumnDefinitions �� $
��Auto ��� $��* ��� $
��Auto �.+��� ._��� $��5,2,0,0q�$��0��
�������$��60��.+��� -   $��imgSoftIcon��$��15��$��16��$
��Left=�$��Center=�$E��?/Agent.Interaction.Desktop;component/Images/Call Attachment.png���� $��1��$��18�����#007edf$��0q�$��Stretch=�$	��Top=�._��� -   $��lblTitleStatus�� �� Padding$ 4,0,4,0q�$��Calibri)�$��13(�
�������$
��Bold"�$��Stretch=�
������ $��Interaction Data��$��Center=��� $��2��$��60��$
��Auto��$��Right=�$	��Top=� �� Orientation$ 
Horizontal=�.+��� -	   $��btnPin��$��16��$��20��  	PinButton#��[ |�.  �� -
   $��btnMinimize��$��16��$��20��  MinimizeButton#��[ |�.  �� -   $��btnExit��$��16��$��20��  CloseButton#��[ |�.  �� -   $��
grupBorder��$��1�����#656d90$��5q�$	��0.5q����� $��Case Information�����#007edf$��Calibri)�$
��Bold"�$��13(�.��� $��Stretch=� �� $
��Auto ��� .+�� $��Right=� �� �� .+��� -   |�.  $��0��  NormalImageButton#��[ $��0,2,5,0q�$��Center=�$	��Top=�$��Stretch=�y��� *IsShowMIDPushButton+ �� TargetNullValue$ 	Collapsed�� �� FallbackValue$ 	Collapsed��.���� $ Vertical=�$��Center=�.+��� $;��5/Agent.Interaction.Desktop;component/Images/popup.png��$��15��$��15���� $��Center=� �� TextAlignment$ Center=�$��	MID Popup��$��Calibri)�$��11(�$
��Wrap=�$��2,0,2,0q��� -   $��btnAddCallData��$��1��|�.  $��30��$��30��$��0,2,5,0q�#��[ $	��Top=�$��Center=�$��Stretch=�y��� *IsVoiceEnabledAddCallData+.���� $ Vertical=�$��Center=�.+��� $9��3/Agent.Interaction.Desktop;component/Images/Add.png��$��15��$��15���� $��Center=�$ Center=�$	��Add��$��Calibri)�$��11(�$
��Wrap=�$��2,0,2,0q� � ToolTip �� 
����������#ADAAAD$	��0.5q�$��-5q�._��  �� $
��Auto ��� $��* �.+��� $	��Add��$��0��$ 	10,5,10,0q�$
��Bold"�$��Calibri)��� $��1��$ 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200��$��Insert addition call data���� -   $��txtAttachDataInfo��$��1��$��10q�$��No attach data found.��$��Calibri)�$��12(�$��Stretch=�$��Center=�$
��Auto��)  #Microsoft.Windows.Controls.DataGrid  -     Name$ DGAttachData��$��1��   RowHeaderWidth$ 0Q�
�������   SelectionUnit$
 Cell=�   SelectionMode$ Single=�$��Stretch=�$	��Top=�$��Stretch=�   HorizontalGridLinesBrush �#D6D7D6   VerticalGridLinesBrush �#D6D7D6$
��Auto=�   AutoGenerateColumns .      CanUserAddRows  .  !   CanUserDeleteRows! .  "   CanUserSortColumns" . #   ClipboardCopyMode$
# None=����� *NotifyCallDataView+$   Columns$ 3  -Microsoft.Windows.Controls.DataGridTextColumn  /  )Microsoft.Windows.Controls.DataGridColumn%   Header$	% Key��&   Width8  2Microsoft.Windows.Controls.DataGridLengthConverter$	& 120 '   MinWidth$	' 100Q�(   MaxWidth$	( 150Q�) �� ShowOnDisabled) . *   
IsReadOnly* . ���� *
Fontweight++   
FontFamily+ �� *
Fontfamily+4  .Microsoft.Windows.Controls.DataGridBoundColumn,   Binding, �� *Key+-   ClipboardContentBinding- Z� .   ElementStyle. �� "
 ��.���  � ��$
 Wrap=���  � ��$ 2q���  �@��Padding$ 2q�7	  1Microsoft.Windows.Controls.DataGridTemplateColumn	  $% Value��$& * $	' 150Q�-   cdValue��/   ClipboardContentBinding/ �� *Value+0 �� 	Converter#0 [ 1 	  CellTemplate1 �� .R��� 2 ��Name$2 txtValue��$
��Left=�$
��Wrap=�$��2q�$ 2q�$	��200��$��Normal"�$��Calibri)�) . 3 � Cursor3 �� *Value+#0 [ ���� *Value+#0 [ ���� *Value+#0 [  4 � ContextMenu4 �� .@�m� 5 m�Name$
5 Copy��-   $
��Copy��6 m� InputGestureText$6  ��7 	  CellEditingTemplate7 �� .R�� 8 �Name$8 	editStack��$��Stretch=�$��0,0,20,0q����� *Width+9 �� ElementName$9 cdValue�� �� $��* ��� $
��Auto ��� $
��Auto �.+��� : ��Name$: txtValue��$��0��; �� 	UndoLimit$; 10��$��Stretch=�$��2q�$��2q�< �� TextWrapping$
< Wrap=�$	��100��  DefaultCopyContextmenu#4 [ ���� *Value+= �� Mode$= TwoWay=��� > ��Name$> 	btnUpdate��-   $��1��$
3 Hand��$��2,0,2,0q�$
��Left=�$��Center=�$
��Auto��$
��Auto�����#007EDF
������  �� 
����������#ADAAAD$	��0.5q�$��-5q�._��  �� $
��Auto ��� $��* �.+��� $2 UpdateHeading��$��Update��$��0��$ 	10,5,10,0q�$
��Bold"�$��Calibri)��� $2 UpdateContent��$��1��$ 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200��$��Agent can update data.��.���� $ Vertical=�$��Center=�.+��� $:��4/Agent.Interaction.Desktop;component/Images/Edit.png��$��10��$��10���� $> 	btnCancel��-   $��2��$
3 Hand��$��2,0,2,0q�$
��Left=�$��Center=�$
��Auto��$
��Auto�����#007EDF
������  �� 
����������#ADAAAD$	��0.5q�$��-5q�._��  �� $
��Auto ��� $��* �.+��� $2 CancelHeading��$��Cancel��$��0��$ 	10,5,10,0q�$
��Bold"�$��Calibri)��� $2 CancelContent��$��1��$ 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200��$��Agent can cancel data.��.���� $ Vertical=�$��Center=�.+��� $<��6/Agent.Interaction.Desktop;component/Images/cancel.png��$��10��$��10��?   	CellStyle? �� -
  'Microsoft.Windows.Controls.DataGridCell"
 
 .���  � ��$ 0q�@ �� Triggers@ P� A P� PropertyA �@
 
IsSelectedB P� ValueB . . ���  � �� 	 HighlightedBrush# [	 ��  � ��
 �   ���  �@+�FocusedElement �� $9 txtValue��