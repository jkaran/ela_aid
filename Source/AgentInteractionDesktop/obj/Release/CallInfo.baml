   M S B A M L   `   `   `  ����   Agent.Interaction.Desktop(    "Agent.Interaction.Desktop.CallInfo   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     6my-http://schemas.microsoft.com/wpf/2008/toolkit  -   $��UserCallInfo��  � Title$  CallInfo��$
��Auto��$	��800��$
��Auto�� � WindowStyle$
 None=� � Icon$> 8/Agent.Interaction.Desktop;component/Images/Callinfo.png�� � AllowsTransparency . 
������  � WindowStartupLocation$ CenterScreen=� � ShowInTaskbar .  � SizeToContent$ WidthAndHeight=� � 	Resources  �� MergedDictionaries	 �� Source
 �� 
TargetType   	baseStyle �� Property �� Value �� BasedOn��  �� $R	 L/Agent.Interaction.Desktop;component/Resources/StylesResourceDictionary.xaml?�%J   &	        '�� #     7   �� "
 ��.���  � ��$ 12(��� "
 ��8   .��� .+��  � RowDefinitions �� $
��Auto ��� -   $��
MainBorder��$��0��$
��0.89q�$��8q�
�������._�� $��0q� �� $
��Auto ��� $
��Auto �.+�� -   $��0��
������� � ColumnDefinitions �� $
��Auto ��� $��* ��� $
��Auto �.+��� ._��� $��5,2,0,0q�$��0��
�������$��60��.+��� -   $��imgSoftIcon��$��15��$��16��$
��Left=�$��Center=�$A��;/Agent.Interaction.Desktop;component/Images/Voice.Short.png���� $��1��$��18�����#007edf$��0q�$��Stretch=�$	��Top=�._��� -   $��lblTitleStatus�� �� Padding$ 4,0,4,0q�$��Calibri)�$��13(�
�������$
��Bold"�$��Stretch=����#4DBF4D$��Interaction Data��$
��Left=��� $��2��$��60��$
��Auto��$��Right=�$	��Top=� �� Orientation$ 
Horizontal=�.+��� -   $��btnPin��$��16��$��20��  	PinButton#��[ �� -   $��btnMinimize��$��16��$��20��  MinimizeButton#��[ �� -   $��btnExit��$��16��$��20��  CloseButton#��[ �� $��1�����#656d90$��5q�$	��0.5q�._�� -	   $��maniGrid�� �� $
��Auto ��� $
��Auto ��� $
��Auto �.+��� $��0�����#656d90$	��0.5q�$��5,5,5,3q� �� CornerRadius$ 2��._�� -
   $��
DGCalldata��$��5q� �� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto � �� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto �.+��� $��0��$��0��$ 
Horizontal=�$	��120��.+��� -   $��lblKey1��$
��Left=�$��Normal"�$��Calibri)�$��Center=�$��Subscriber Id���� -   $��
txtSBSB_ID�� �� 	UndoLimit$ 10��$
��Left=�$��Normal"�$��2q�$	��150��$��Calibri)�$��0��$��1��$�� ��  controltempTextbox#��[ $��0��{�.  .�� J� InputBindings ��  �� Command$% ApplicationCommands.NotACommand��
 �� Key$ V�� �� 	Modifiers$ Controlf�.���� -   $��lblKey2��$
��Left=�$��Normal"�$��2q�$	��120��$��0��$��2��$��Calibri)�$��Mem Code - Suffix���� -   $��txtMEME_SFX��$ 10��$
��Left=�$��Normal"�$��2q�$	��150��$��0��$��3��$��Calibri)�$�� ��#��[ $��1��{�.  .�� �� $% ApplicationCommands.NotACommand��$ V��$ Controlf�.���� $��1��$��0��$ 
Horizontal=�$��2q�$	��120��.+��� -   $��lblKey3��$
��Left=�$��Normal"�$��Calibri)�$��Center=�$	��ANI���� -   $��lblMandatoryKey2��$
��Left=�$��Normal"�$��Calibri)�$��Center=�
���  ��$��*���� -   $��txtCSCI_PHONE��$ 10��$��Stretch=�$��2q�$	��150��$��Calibri)�$��1��$��1��  controltempTextbox1#��[ $��2�� �� 	MaxLength$ 10��.�� �� $% ApplicationCommands.NotACommand��$ V��$ Controlf�.���� $��1��$��2��$ 
Horizontal=�$��2q�$	��120��.+��� -   $��lblKey6��$
��Left=�$��Normal"�$��Center=�$��2q�$	��120��$��Calibri)�$
��DNIS���� -   $��txtDNIS��$ 10��$��Stretch=�$��2q�$	��150��$��1��$��Center=�$��3��$��Calibri)�#��[ $��3��.�� �� $% ApplicationCommands.NotACommand��$ V��$ Controlf�.���� $��2��$��0��$ 
Horizontal=�$	��120��.+��� -   $��lblKey5��$
��Left=�$��Normal"�$��Calibri)�$��Center=�$��	Member Id���� -   $��lblMandatoryKey3��$
��Left=�$��Normal"�$��Calibri)�$��Center=�
���  ��$��*���� $��2��$��1�����#ADAAAD$��2q�$��1q�._�� $��Center=� �� �� .+��� $��0�����#ADAAAD$��0,0,0,1q�._��� $ 
Horizontal=�$	��150��.+��� -   $��btnBackward��$��15��
������  � ToolTip �� 
����������#ADAAAD$	��0.5q�$��-5q�._��  �� $
��Auto �.+��� $��Previous��$��0��$ 	10,5,10,5q�$��Calibri)����� "
 ��.���  � �� �� .R���  ��Name$ Close��$:��4/Agent.Interaction.Desktop;component/Images/back.png��$��10��$��10�� �� Triggers ��  �� Value$
 True�� �� Binding ��  �� RelativeSource ��  �� Mode$ FindAncestor=�  �� AncestorType"  ��! �� Path$! IsMouseOver�.a��� " �� 
TargetName$" Close�� � ��$: 4/Agent.Interaction.Desktop;component/Images/back.png���� $
 True�� ��  �� $ FindAncestor=�"  ��$! 	IsPressed�.a��� $" Close�� � ��$: 4/Agent.Interaction.Desktop;component/Images/back.png����  � �� �� # �� 
TargetType"# ��$ � 	Resources$ ��%    .R��� $��Border��
������ ._��� $��content��#��z��#��z����. % J� SnapsToDevicePixels#% z% #��z���� -   $��txtMEMBER_ID��$ 10��$��Stretch=�$��Calibri)�$	��105��$��Center=����#ADAAAD$��1,0,1,0q�$��4��.�� �� $% ApplicationCommands.NotACommand��$ V��$ Controlf�.���� -   $��
btnForward��$��15�� �� 
����������#ADAAAD$	��0.5q�$��-5q�._��  �� $
��Auto �.+��� $
��Next��$��0��$ 	10,5,10,5q�$��Calibri)����� "
 ��.���  � �� �� .R��� $ Close��$=��7/Agent.Interaction.Desktop;component/Images/forward.png��$��10��$��10�� �� $
 True�� ��  �� $ FindAncestor=�"  ��$! IsMouseOver�.a��� $" Close�� � ��$= 7/Agent.Interaction.Desktop;component/Images/forward.png���� $
 True�� ��  �� $ FindAncestor=�"  ��$! 	IsPressed�.a��� $" Close�� � ��$= 7/Agent.Interaction.Desktop;component/Images/forward.png����  � �� �� "# ��$ ��%    .R��� $��Border��
������ ._��� $��content��#��z��#��z����. #% z% #��z���� -   $��	btnUpdate��$��15�� �� 
����������#ADAAAD$	��0.5q�$��-5q�._��  �� $
��Auto �.+��� $��Update��$��0��$ 	10,5,10,5q�$��Calibri)����� "
 ��.���  � �� �� .R��� $ Close��$9��3/Agent.Interaction.Desktop;component/Images/Add.png��$��10��$��10�� �� $
 True�� ��  �� $ FindAncestor=�"  ��$! IsMouseOver�.a��� $" Close�� � ��$9 3/Agent.Interaction.Desktop;component/Images/Add.png���� $
 True�� ��  �� $ FindAncestor=�"  ��$! 	IsPressed�.a��� $" Close�� � ��$9 3/Agent.Interaction.Desktop;component/Images/Add.png����  � �� �� "# ��$ ��%    .R��� $��Border��
������ ._��� $��content��#��z��#��z����. #% z% #��z���� $ 
Horizontal=�$��Center=�$��1��.+��� -   $��
txtContent��$�� ���� -   $��lblKey20��$
��Left=�$��Normal"�$��2q�$	��120��$��2��$��2��$��Calibri)�$��
Target VDN��$��Center=��� -   $��txtTarget_VDN��$ 10��$��Stretch=�$��2q�$	��150��$��2��$��3��$��Calibri)�$��Center=�#��[ $��5��.�� �� $% ApplicationCommands.NotACommand��$ V��$ Controlf�.���� -   $��lblKey7��$
��Left=�$��Normal"�$��2q�$	��120��$��3��$��0��$��Calibri)�$��Center=�$��Provider Id���� -   $��
txtPRPR_ID��$ 10��$��Stretch=�$��2q�$	��150��$��3��$��1��$��Calibri)�$�� ��#��[ $��6��.�� �� $% ApplicationCommands.NotACommand��$ V��$ Controlf�.���� -   $��lblKey8��$
��Left=�$��Normal"�$��2q�$	��120��$��3��$��2��$��Calibri)�$��Center=�$��Member Validated���� -    $��txtMEMBER_INFO��$ 10��$��Stretch=�$��2q�$	��150��$��3��$��3��$��Calibri)�$�� ��#��[ $��7��.�� �� $% ApplicationCommands.NotACommand��$ V��$ Controlf�.���� -!   $��lblKey9��$
��Left=�$��Normal"�$��2q�$	��120��$��4��$��0��$��Calibri)�$��Group Id���� -"   $��
txtGRGR_ID��$ 10��$��Stretch=�$��2q�$	��150��$��4��$��1��$��Calibri)�$�� ��#��[ $��8��.�� �� $% ApplicationCommands.NotACommand��$ V��$ Controlf�.���� -#   $��lblKey10��$
��Left=�$��Normal"�$��2q�$	��120��$��4��$��2��$��Calibri)�$��Disclaimer Played���� -$   $��txtCSTK_SUMMARY��$ 10��$��Stretch=�$��2q�$	��150��$��4��$��3��$��Calibri)�$�� ��#��[ $��9��.�� �� $% ApplicationCommands.NotACommand��$ V��$ Controlf�.���� -%   $��lblKey19��$
��Left=�$��Normal"�$��2q�$	��120��$��5��$��0��$��Calibri)�$��Zip Code��$��Center=��� -&   $��	txtMemZIP��$ 10��$��Stretch=�$��2q�$	��150��$��5��$��1��$��Calibri)�$��Bottom=�#��[ $��10��.�� �� $% ApplicationCommands.NotACommand��$ V��$ Controlf�.���� -'   $��lblKey12��$
��Left=�$��Normal"�$��2q�$	��120��$��5��$��2��$��Calibri)�$��Claims Date of Service��$��Center=��� -(   $��txtCSTK_MEM_SUMMARY��$ 10��$��Stretch=�$��2q�$	��150��$��5��$��3��$��Calibri)�$��Bottom=�#��[ $��11��.�� �� $% ApplicationCommands.NotACommand��$ V��$ Controlf�.���� -)   $��lblKey13��$
��Left=�$��Normal"�$��2q�$	��120��$��6��$��0��$��Calibri)�$��
First Name��$��Center=��� -*   $��txtCSCI_FIRST_NAME��$ 10��$��Stretch=�$��2q�$	��150��$��6��$��1��$��Calibri)�$��Bottom=�#��[ $��12��.�� �� $% ApplicationCommands.NotACommand��$ V��$ Controlf�.���� -+   $��lblKey14��$
��Left=�$��Normal"�$��2q�$	��120��$��6��$��2��$��Calibri)�$��	Last Name��$��Center=��� -,   $��txtCSCI_LAST_NAME��$ 10��$��Stretch=�$��2q�$	��150��$��6��$��3��$��Calibri)�$��Bottom=�#��[ $��13��.�� �� $% ApplicationCommands.NotACommand��$ V��$ Controlf�.���� --   $��lblKey15��$
��Left=�$��Normal"�$��2q�$	��120��$��7��$��0��$��Calibri)�$��Middle Name��$��Center=��� -.   $��txtCSCI_MID_INIT��$ 10��$��Stretch=�$��2q�$	��150��$��7��$��1��$��Calibri)�$��Bottom=�#��[ $��14��.�� �� $% ApplicationCommands.NotACommand��$ V��$ Controlf�.���� -/   $��lblKey16��$
��Left=�$��Normal"�$��2q�$	��120��$��7��$��2��$��Calibri)�$��Date of Birth��$��Center=��� -0   $��txtMEMBER_DOB��$ 10��$��Stretch=�$��2q�$	��150��$��7��$��3��$��Calibri)�$��Bottom=�#��[ $��15��.�� �� $% ApplicationCommands.NotACommand��$ V��$ Controlf�.���� -1   $��lblKey22��$
��Left=�$��Normal"�$��2q�$	��120��$��8��$��0��$��Calibri)�$��Call Reason��$��Center=��� -2   $��txtCSTK_MCTR_CATG��$ 10��$��Stretch=�$��2q�$	��150��$��8��$��1��$��Calibri)�$��Bottom=�#��[ $��16��.�� �� $% ApplicationCommands.NotACommand��$ V��$ Controlf�.���� -3   $��lblKey18��$
��Left=�$��Normal"�$��2q�$	��120��$��8��$��2��$��Calibri)�$��Caller Type��$��Center=��� -4   $��txtCSTK_CUST_IND��$ 10��$��Stretch=�$��2q�$	��150��$��8��$��3��$��Calibri)�$��Bottom=�#��[ $��17��.�� �� $% ApplicationCommands.NotACommand��$ V��$ Controlf�.���� $��1�����#656d90$��5,3,5,3q�$	��0.5q�$ 2��._�� $��5q� �� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto � �� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto �.+��� $��0��$��0��$ 
Horizontal=�$��2q�$	��120��.+��� -5   $��	lblKey171��$
��Left=�$��Normal"�$��Calibri)�$��Center=�$��Destination���� -6   $��lblMandatoryKey4��$
��Left=�$��Normal"�$��Calibri)�$��Center=�
���  ��$��*���� -7   $��txtDestination��$ 10��$��Stretch=�$��2q�$	��150��$��0��$��1��$��Calibri)�$��Center=�#��[ $	��FID��{�.  $��18���� $��0��$��2��$ 
Horizontal=�$��2q�$	��120��.+��� -8   $��	lblKey181��$
��Left=�$��Normal"�$��Calibri)�$��Center=�$��Origin���� -9   $��lblMandatoryKey5��$
��Left=�$��Normal"�$��Calibri)�$��Center=�
���  ��$��*���� -:   $��	txtOrigin��$ 10��$��Stretch=�$��2q�$	��150��$��0��$��3��$��Calibri)�#��[ $��Company Server��{�.  $��19���� $��1��$��0��$ 
Horizontal=�$��2q�$	��120��.+��� -;   $��	lblKey191��$
��Left=�$��Normal"�$��Calibri)�$��Center=�$��PZAP APP ID���� -<   $��lblMandatoryKey6��$
��Left=�$��Normal"�$��Calibri)�$��Center=�
���  ��$��*���� -=   $��txtPZAP_APP_ID��$ 10��$��Stretch=�$��2q�$	��150��$��1��$��1��$��Calibri)�#��[ $
��CST0��{�.  $��20���� ->   $��	lblKey201��$
��Left=�$��Normal"�$��2q�$	��120��$��1��$��2��$��Calibri)�$��Center=�$��
Active Row���� -?   $��txtActiveRow��$ 10��$��Stretch=�$��2q�$	��150��$��1��$��3��$��Calibri)�{�.  #��[ $��21���� -@   $��	lblKey211��$
��Left=�$��Normal"�$��2q�$	��120��$��2��$��0��$��Calibri)�$��Center=�$��Section Name���� -A   $��txtSectionName��$ 10��$��Stretch=�$��2q�$	��150��$��2��{�.  $��1��$��Calibri)�#��[ $��22���� -B   $��	lblKey221��$
��Left=�$��Normal"�$��2q�$	��120��$��2��$��2��$��Calibri)�$��Center=�$��SubSection Name���� -C   $��txtSubSectionName��$ 10��$��Stretch=�$��2q�$	��150��$��2��{�.  $��3��$��Calibri)�#��[ $��23���� $��3��$��0��$ 
Horizontal=�$	��120��.+��� -D   $��lblKey23��$
��Left=�$��Normal"�$��Calibri)�$��Center=�$��Action���� -E   $��lblMandatoryKey7��$
��Left=�$��Calibri)�$��Normal"�$��Center=�
���  ��$��*���� -F   $��	txtAction��$ 10��$��Stretch=�$��2q�$	��150��$��3��$	��New��$��1��$��Calibri)�#��[ {�.  $��24���� -G   $��lblKey24��$
��Left=�$��Normal"�$��2q�$	��120��$��3��$��2��$��Calibri)�$��Center=�$��
App Action���� -H   $��txtAppAction��$ 10��$��Stretch=�$��2q�$	��150��$��3��{�.  $��3��$��Calibri)�#��[ $��25���� -I   $��lblKey25��$
��Left=�$��Normal"�$��2q�$	��120��$��4��$��0��$��Calibri)�$��Center=�$��Calling Method���� -J   $��txtCALLIN_METHOD��$ 10��$��Stretch=�$��2q�$	��150��$��4��$��1��$��Calibri)�$��1��#��[ {�.  $��26���� -K   $��lblKey26��$
��Left=�$��Normal"�$��2q�$	��120��$��4��$��2��$��Calibri)�$��Center=�$��Call Received On���� -L   $��txtCALL_RECEIVED_ON��$ 10��$��Stretch=�$��2q�$	��150��$��4��$��3��$��Calibri)�#��[ $��Customer Care��{�.  $��27���� -M   $��lblKey27��$
��Left=�$��Normal"�$��2q�$	��120��$��5��$��0��$��Calibri)�$��Center=�$��MCPD Source���� -N   $��txtMCPD_SOURCE��$ 10��$��Stretch=�$��2q�$	��150��$��5��{�.  $��1��$��Calibri)�#��[ $
��CS01��$��28���� -O   $��lblKey28��$
��Left=�$��Normal"�$��2q�$	��120��$��5��$��2��$��Calibri)�$��Center=�$��Calling For���� -P   $��txtCALLING_FOR��$ 10��$��Stretch=�$��2q�$	��150��$��5��$�� ��$��3��$��Calibri)�#��[ $��29��{�.  �� -Q   $��lblKey29��$
��Left=�$��Normal"�$��2q�$	��120��$��6��$��0��$��Calibri)�$��Center=�$��	Page Type���� -R   $��txtPAGE_TYPE��$ 10��$��Stretch=�$��2q�$	��150��$��6��$��1��$��Calibri)�#��[ $�� ��$��30��{�.  �� -S   $��lblKey32��$
��Left=�$��Normal"�$��2q�$	��120��$��6��$��2��$��Calibri)�$��Center=�$��ConnID���� -T   $��	txtConnID��$ 10��$��Stretch=�$��2q�$	��150��$��6��$��3��$��Calibri)�#��[ $�� ��$��31��{�.  �� $��2�����#656d90$��0q�$��5,0,5,0q�._�� .+��� -U   $��
stkp_Error��$ 
Horizontal=�$
��Auto��$y�Hidden=�$��5q�$��2��$��2��$
��Left=�.+��� $��5,2,0,2q�$	��Top=�$;��5/Agent.Interaction.Desktop;component/Images/Error.png��$��20��$��20���� -V   $��lblInformation��$
��Auto��$	��Top=�$�� * Fields are mandatory�����#e32222�� -W   $��btnOk��$��0,2,95,2q�$
��Save��$��Right=�$��23��$��80��  NormalButton#��[ $��32���� -X   $��	btnCancel��$��0,2,5,2q�$��Cancel��$��Right=�$��23��$��80��#��[ $��33��