   M S B A M L   `   `   `  ���� U  QAgent.Interaction.Desktop, Version=5.0.3.31, Culture=neutral, PublicKeyToken=nullc6clr-namespace:Agent.Interaction.Desktop.CustomControls(Agent.Interaction.Desktop.CustomControls  E'clr-namespace:Agent.Interaction.DesktopAgent.Interaction.Desktop  [2clr-namespace:Agent.Interaction.Desktop.Converters$Agent.Interaction.Desktop.Converters  ,    &Agent.Interaction.Desktop.SoftPhoneBar   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     PCustomControl6clr-namespace:Agent.Interaction.Desktop.CustomControls     I EWpfAnimatedGif, Version=1.4.4.0, Culture=neutral, PublicKeyToken=null,gif"http://wpfanimatedgif.codeplex.com  3local'clr-namespace:Agent.Interaction.Desktop  @convert2clr-namespace:Agent.Interaction.Desktop.Converters  6my-http://schemas.microsoft.com/wpf/2008/toolkit  -   $��SoftphoneBar��  � Title$	  AID�� � 
ResizeMode$ 	CanResize=� � WindowStyle$
 None=� � Icon$@ :/Agent.Interaction.Desktop;component/Images/Agent32x32.ico��
������  � AllowsTransparency .  � WindowStartupLocation$ CenterScreen=� � 	Resources  �� MergedDictionaries �� Source9   3Agent.Interaction.Desktop.Converters.ValueConverter   ValueConverterD   >Agent.Interaction.Desktop.Converters.DGCellForeGroundConverter  CellForegroundG   AAgent.Interaction.Desktop.Converters.DGCellCellMouseOverConverter  CellMouseOverC   =Agent.Interaction.Desktop.Converters.DGRowBackgroundConverter  RowBackgroundJ  DAgent.Interaction.Desktop.Converters.DGGlobalStatBackGroundConverter  GlobalStatBackgroundI  CAgent.Interaction.Desktop.Converters.UriUnescapeDataStringConverter  UriUnescapeDataString 	 Login 
 Logout 	 Ready 	 NotReady 

 Answer  Release  Hold  Retrieve��  �� $R L/Agent.Interaction.Desktop;component/Resources/StylesResourceDictionary.xaml?�%�   &	        &	      &	 
     &	      &	      &	      &	      &	 #     &	 (     &		 -     &	
 2     &	 7     &	 <     &	 A                 �� �� �� �� �� �� �� �� 	 J� CommandBindings	 )  #System.Windows.Input.CommandBinding  -   
   Command#
 [   -   #
 [   -   #
 [   -   #
 [	   -   #
 [
   -   #
 [   -   #
 [   -	   #
 [ .��� .+��� -
   $��
MainBorder��$��8q�$��1q�
������� %� IsVirtualizing .  %� VirtualizationMode$ 	Recycling=����� *MainBorderBrush+._��  . $ 	Recycling=� � RowDefinitions �� $��* ��� $��5 �.+��� -   $��RightSideRect��$��2��
������ $��3�� K� ZIndex$
 1000��$��Right=�$��Stretch=��� -   $��RightbottomSideRect��$��1��
������ $��8��$��5��$
 2200��$��Right=�$��Bottom=��� -   $��BottomSideRect��$��1��
������ $��3��$
 1000��$��Stretch=�$��Bottom=�� 
������� . $ 	Recycling=� �� -    �Name$ grdTitle��$
��Auto ��� -   $ grdTool��$
��Auto ��� -   $ 	grdBottom������ *BottomRowHeight+.+�� -   $��	TitleGrid��$��0�� � ColumnDefinitions �� $
��Auto ��� $��* ��� $
��Auto �.+��� -   $��agentStateBorder��$��0��$��5,0,30,0q�
�������$��26��._��� -    �� Orientation$ 
Horizontal=�.+��� -   $��imgSoftIcon��$��15��$��16��$
��Left=�$	��Top=�$G��A/Agent.Interaction.Desktop;component/Images/Pointel Softphone.png���� -   $��imgStatusOption��$;��5/Agent.Interaction.Desktop;component/Images/Eject.png��$��10��$��10��$��Bottom=�$y�	Collapsed=��� -   $��1��$��18�����#007EDF$	��Top=�$��0,-1,0,0q�._��  �� $��* ��� $
��Auto �.+�� $��0��$
��Left=����� *TitleBgColor+ �� TargetNullValue$ Transparent�� �� $
��Auto ��� $
��Auto ��� $
��Auto �.+��� -   $��lblTitleStatus��$��0��$��Calibri)�$��14(�
�������$
��Bold"�$��Stretch=�
������ $
��Left=����� *TitleStatusText+�� -   $��lbldummy��$��1��$��3��$��Calibri)�$��14(�
�������$
��Bold"�$��Stretch=�
������ $
��Left=�9   3Agent.Interaction.Desktop.CustomControls.TimerLabel  -     Name$ 
tlblStatus��$��2��$��0,0,4,0q�   Text$ 
[00:00:00]��$��Calibri)�$��14(�
�������$
��Bold"�$
��Left=�$��Stretch=�
������ $��61��y��� *TimerEnabled+ �� FallbackValue$ Visible��$ Visible���� -   $��txtCallStatus��$��1��$��
-0.3,0,5,0q�$��Calibri)�$��14(�
�������$
��Bold"�$��Stretch=�$��Right=����� *CallTypeStatus+�� $��2��$��60��$
��Auto��$��Right=�$	��Top=�$ 
Horizontal=�.+��� -   $��btnPin��$��16��$��20��  	PinButton#��[ |�.  �� -   $��btnMinimize��$��16��$��20��  MinimizeButton#��[ |�.  �� -   $��btnExit��$��16��$��20��  CloseButton#��[ |�.  � -   $��ToolGrid��$��1��$��0,0,0,3q�$
��Auto�� . $ 	Recycling=�  �� 
TargetType �� Property �� Value��%-   '��       �� " ��.���  � ��$ 11(� �� $
��Auto ��� $��* �.+�� -   $��grdTools_Buttons��$��0��$	��550��$	��750��$��Right=� J� SnapsToDevicePixels .  �� $
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
��Auto ��� $
��Auto ��� $
��Auto �.+��� -    $��btnLogin��$��0��$��45��$��55��$��55��$��5,0,0,0q�|�.    NormalImageButton#��[ $	��Top=�$��Stretch=�.���� $ Vertical=�$��Center=�$��45��$��55��.+��� $��40��$��40������ *LogImageSource+�� $��Center=� �� TextAlignment$ Center=�$��Calibri)�$��11(�$
��Wrap=�$��2,0,2,0q����� *	LogText+ � ToolTip �� 
����������#ADAAAD$	��0.5q�$��-5q�._��  �� $
��Auto ��� $��* �.+��� -!   $��
LogHeading��$��0�� �� Padding$ 	10,5,10,0q�$
��Bold"�$��Calibri)����� *	LogText+�� -"   $��
LogContent��$��1��$ 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200������ " �� �� Triggers ^�  ^� 
Conditions ��  �� Binding �� *	LogText+  �� Value	  Login.0���  � ��$( "Agent login through Voice channel.��^�  ��  �� *	LogText+
  Logout.0���  � ��$&  Agent Logout from Voice Channel.���� -#   $��btnReady��$��1��$��45��$��55��$��55��|�.  #��[ $	��Top=�$��Stretch=�{��� *IsReadyEnabled+.���� $ Vertical=�$��Center=�$��45��$��55��.+��� $��40��$��40������ *ReadyImageSource+�� $��Center=�$ Center=�$��Calibri)�$
��Wrap=�$��2,0,2,0q����� *	ReadyText+ �� 
����������#ADAAAD$	��0.5q�$��-5q�._��  �� $
��Auto ��� $��* �.+��� -$   $��ReadyHeading��$��0��$ 	10,5,10,0q�$
��Bold"�$��Calibri)����� *	ReadyText+�� -%   $��ReadyContent��$��1��$ 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200������ " �� ^�  ��  �� *	ReadyText+	  Ready.0���  � ��$, &Agent can change the state to 'Ready'.��^�  ��  �� *	ReadyText+  	Not Ready.0���  � ��$@ :Agent can change the state to 'NotReady' with reasoncodes.���� -&   $��	btnAnswer��$��2��$��45��$��55��$��55��|�.  #��[ $	��Top=�$��Stretch=�{��� *IsTalkEnabled+���� *TalkBtnCommand+.���� $ Vertical=�$��Center=�$��45��$��55��.+��� $��40��$��40��"	  WpfAnimatedGif.ImageBehavior! 	  AnimatedSource! �� *TalkImageSource+�� $��Center=�$ Center=�$��Calibri)�$
��Wrap=�$��2,0,2,0q����� *
TalkText+ �� 
����������#ADAAAD$	��0.5q�$��-5q�._��  �� $
��Auto ��� $��* �.+��� -'   $��TalkHeading��$��0��$ 	10,5,10,0q�$
��Bold"�$��Calibri)����� *
TalkText+�� -(   $��TalkContent��$��1��$ 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200������ " �� ^�  ��  �� *
TalkText+  Talk.0���  � ��$) #Agent can answer the incoming call.��^�  ��  �� *
TalkText+  Release.0���  � ��$) #Agent can release the current call.���� -)   $��btnHold��$��3��$��45��$��55��$��55��|�.  #��[ $	��Top=�$��Stretch=�{��� *IsHoldEnabled+���� *HoldBtnCommand+.���� $ Vertical=�$��Center=�$��45��$��55��.+��� $��40��$��40������ *HoldImageSource+�� $��Center=�$ Center=�$��Calibri)�$
��Wrap=�$��2,0,2,0q����� *
HoldText+ �� 
����������#ADAAAD$	��0.5q�$��-5q�._��  �� $
��Auto ��� $��* �.+��� -*   $��HoldHeading��$��0��$ 	10,5,10,0q�$
��Bold"�$��Calibri)����� *
HoldText+�� -+   $��HoldContent��$��1��$ 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200������ " �� ^�  ��  �� *
HoldText+  Hold.0���  � ��$, &Agent can put the current call onhold.��^�  ��  �� *
HoldText+  Retrieve.0���  � ��$' !Agent can retrieve the held call.���� -,   $��btnTransfer��$��4��$��45��$��65��$��55��|�.  #��[ $	��Top=�$��Stretch=�{��� *IsTransEnabled+.���� $ Vertical=�$��Center=�$��45��$��65��.+��� $��40��$��40������ *TransImageSource+�� $��Center=�$ Center=�$��Calibri)�$
��Wrap=�$��2,0,2,0q����� *	TransText+ �� 
����������#ADAAAD$	��0.5q�$��-5q�._��  �� $
��Auto ��� $��* �.+��� --   $��TransHeading��$��0��$ 	10,5,10,0q�$
��Bold"�$��Calibri)����� *	TransText+�� -.   $��TransContent��$��1��$ 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200������ " �� ^�  ��  �� *	TransText+	  Trans.0���  � ��$< 6Agent can initiate the transfer call to another agent.��^�  ��  �� *	TransText+  
Comp Trans.0���  � ��$+ %Agent can complete the transfer call.���� -/   $��btnConference��$��5��$��45��$��65��$��55��|�.  #��[ $	��Top=�$��Stretch=�{��� *IsConfEnabled+.���� $ Vertical=�$��Center=�$��45��$��65��.+��� $��40��$��40������ *ConfImageSource+�� $��Center=�$ Center=�$��Calibri)�$
��Wrap=�$��2,0,2,0q����� *
ConfText+ �� 
����������#ADAAAD$	��0.5q�$��-5q�._��  �� $
��Auto ��� $��* �.+��� -0   $��ConfHeading��$��0��$ 	10,5,10,0q�$
��Bold"�$��Calibri)����� *
ConfText+�� -1   $��ConfContent��$��1��$ 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200������ " �� ^�  ��  �� *
ConfText+  Conf.0���  � ��$< 6Agent can initiate conference call with another agent.��^�  ��  �� *
ConfText+  	Comp Conf.0���  � ��$- 'Agent can complete the conference call.��^�  ��  �� *
ConfText+  Delete Conf.0���  � ��$@ :Agent can delete the conference call for the specified DN.���� -2   $��btnReconnect��$��6��$��45��$��55��$��55��|�.  #��[ $	��Top=�$��Stretch=�{��� *IsReConEnabled+.���� $ Vertical=�$��Center=�$��45��$��55��.+��� $��40��$��40������ *ReConImageSource+�� $��Center=�$ Center=�$��Calibri)�$
��Wrap=�$��2,0,2,0q����� *	ReConText+ �� 
����������#ADAAAD$	��0.5q�$��-5q�._��  �� $
��Auto ��� $��* �.+��� -3   $��ReConHeading��$��0��$ 	10,5,10,0q�$
��Bold"�$��Calibri)����� *	ReConText+�� -4   $��ReConContent��$��1��$ 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200��$x��rAgent can cancel the current Transfer/conference call and re-establish new Transfer/conference call with the agent���� -5   $��btnAlternateCall��$��7��$��45��$��55��$��55��|�.  #��[ $	��Top=�$��Stretch=�{��� *IsAlternateCallEnabled+.���� $ Vertical=�$��Center=�$��45��$��55��.+��� $��40��$��40������ *AlternateCallImageSource+�� $��Center=�$ Center=�$��11(�$��Calibri)�$
��Wrap=�$��2,0,2,0q����� *AlternateCallText+ �� 
����������#ADAAAD$	��0.5q�$��-5q�._��  �� $
��Auto ��� $
��Auto �.+��� -6   $��AlternateCallHeading��$��0��$ 	10,5,10,0q�$
��Bold"�$��Calibri)����� *AlternateCallText+�� -7   $��AlternateCallContent��$��1��$ 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200��$$��Agent can make alternate call.���� -8   $��btnMergeCall��$��8��$��45��$��65��$��55��|�.  #��[ $	��Top=�$��Stretch=�{��� *IsMergeCallEnabled+.���� $ Vertical=�$��Center=�$��45��$��65��.+��� $��40��$��40������ *MergeCallImageSource+�� $��Center=�$ Center=�$��11(�$��Calibri)�$
��Wrap=�$��2,0,2,0q����� *MergeCallText+ �� 
����������#ADAAAD$	��0.5q�$��-5q�._��  �� $
��Auto ��� $
��Auto �.+��� -9   $��MergeCallHeading��$��0��$ 	10,5,10,0q�$
��Bold"�$��Calibri)����� *MergeCallText+�� -:   $��MergeCallContent��$��1��$ 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200��$ ��Agent can merge all calls.���� -;   $��txtDN��$��9��$	��Top=�$��5,5,0,5q�$��23��$	��115��" �� 
IsEditable" . # �� MaxDropDownHeight$	# 100��$��Calibri)����� �� -<   $��btnDial��$��10��$��45��$��55��$��55��|�.  #��[ $	��Top=�$��Stretch=�{��� *IsDialEnabled+.���� $ Vertical=�$��Center=�$��45��$��55��.+��� $��40��$��40������ *DialImageSource+�� $��Center=�$ Center=�$��Calibri)�$
��Wrap=�$��2,0,2,0q����� *
DialText+ �� 
����������#ADAAAD$	��0.5q�$��-5q�._��  �� $
��Auto ��� $��* �.+��� -=   $��DailHeading��$��0��$ 	10,5,10,0q�$
��Bold"�$��Calibri)����� *
DialText+�� ->   $��DailContent��$��1��$"��Make Call to entered number.��$ 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200������ " ��.���  � y�$ 	Collapsed=� ^�  ��  �� *
DialText+  Dial.0���  � y�$ Visible=��� -?   $��DailContent1��$"��Send dtmf for entered digit.��$��1��$ 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200������ " ��.���  � y�$ 	Collapsed=� ^�  ��  �� *
DialText+  DTMF.0���  � y�$ Visible=��� -@   $��btnCallData��$��11��$��45��$��55��$��55��|�.  #��[ $	��Top=�$��Stretch=�{��� *IsAttachDataEnabled+���� *AttachDataBtnCommand+.���� $ Vertical=�$��Center=�$��45��$��55��.+��� $��40��$��40������ *AttachDataImageSource+�� $��Center=�$ Center=�$��11(�$��Calibri)�$
��Wrap=�$��2,0,2,0q����� *AttachDataText+ �� 
����������#ADAAAD$	��0.5q�$��-5q�._��  �� $
��Auto ��� $
��Auto �.+��� -A   $��AttachDataHeading��$��0��$ 	10,5,10,0q�$
��Bold"�$��Calibri)����� *AttachDataText+�� -B   $��AttachDataContent��$��1��$ 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200��$��Show current call data.���� -C   $��12��$��45��$��75��$��55��|�.  #��[ $	��Top=�$��Stretch=�y��� *IsSalesForceEnabled+.���� $ Vertical=�$��Center=�$��45��$��75��.+��� $@��:/Agent.Interaction.Desktop;component/Images/Salseforce.png��$��50��$��40���� $��Center=�$ Center=�$��11(�$��Calibri)�$
��Wrap=�$��2,0,2,0q����� *SalesForceText+$ DisConnected�� �� 
����������#ADAAAD$	��0.5q�$��-5q�._��  �� $
��Auto ��� $
��Auto �.+��� $��0��$ 	10,5,10,0q�$
��Bold"�$��Calibri)����� *SalesForceText+$ DisConnected���� $��1��$ 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200������ *SalesForceTextToolTip+$@ :SalesForce is dis-connected with Agent Interaction Desktop���� -D   $��btn_Menu��#��[ $��40��$��50��$��	10,0,10,0q�|�.  $��1��$��Center=�$��Right=�$��Center=�$��Center=� �� 
����������#ADAAAD$	��0.5q�$��-5q�._��  �� $
��Auto ��� $��* �.+��� $��	Utilities��$��0��$ 	10,5,10,0q�$
��Bold"�$��Calibri)��� $��1��$/��)Access and use workspace supporting views��$ 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200��.���� $��30��$��30��$:��4/Agent.Interaction.Desktop;component/Images/Menu.png��� -E   $��	grdNotify��$��20��$��20��$��1��$��Right=�$��Bottom=�$��0,0,16,9q�$ 10000�� �� 
����������#ADAAAD$	��0.5q�$��-5q�._��  �� $
��Auto ��� $��* �.+��� $��Unread message��$��0��$ 	10,5,10,0q�$
��Bold"�$��Calibri)��� $��1��$5��/You have some unread message in My Message tab.��$ 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200��.+�A� 
���  ��
���  ��$��3��$��16��$��16��� $ � Orientation$$ 
Horizontal=�$��Center=�$��Center=�.+��� -F   $��txtUnreadmessageCount��$��UltraCondensed&�$
��Bold"�
�������$
��Left=�$ Center=�$��Center=����� *UnreadMessageCount+
% � Tag% �� *MessageCountRange+�� $��+��$��7��$
��-0.7q�$��UltraCondensed&�$��	ExtraBold"�
�������$��Right=�$
 Left=�$��Center=����� " ��.���  � y�$ 	Collapsed=� �� & �� Binding& �� *MessageCountRange+' �� Value' 0.a���  � y�$ 	Collapsed=��� & �� *MessageCountRange+' 1.a���  � y�$ Visible=����� " �.���  � y�$ Visible=� �� & �� *MessageCountRange+' -1.a���  � y�$ 	Collapsed=�� -G   $��BottomWindowGrid��$��2�� . $ 	Recycling=�.+��� -H   $��DataTabControl��( �� TabStripPlacement$( Bottom=�$
��Auto��$��5,5,5,0q� . $ 	Recycling=�y��� *WorksapceTabVisibility+) �� SelectedIndex) �� *TabSelectedIndex+.@��� $��My _Channels�� . $ 	Recycling=�.��)
  #Microsoft.Windows.Controls.DataGrid
  -I   $��DGMyChannels��* 
  RowHeaderWidth$* 0Q�$��5q�
�������$��Stretch=�$��Stretch=�$
��Left=�+ 
  HorizontalGridLinesBrush+ �#D6D7D6, 
  VerticalGridLinesBrush, �#D6D7D6- 
  AutoGenerateColumns- .  . 
  CanUserAddRows. .  / 
  CanUserDeleteRows/ .  0 
  CanUserSortColumns0 .   . $ 	Recycling=����� *MediaStatus+1 
  Columns1 7  1Microsoft.Windows.Controls.DataGridTemplateColumn  /  )Microsoft.Windows.Controls.DataGridColumn2   Header$2 Media��3   Width8  2Microsoft.Windows.Controls.DataGridLengthConverter$	3 130 4   MinWidth$	4 115Q�5   MaxWidth$	5 125Q�6   CanUserSort6 .  7   SortMemberPath$7 ChannelName��8   CellTemplate8 �� .R��� $ 
Horizontal=�.+��� $��imgChannelIcon��$��15��$��15��$��5,0,5,0q����� *ChannelIconImageSource+�� $��txtChannelName��$
��Left=�$��2,0,0,0q�$��Center=�9 �� 	IsEnabled9 . ���� *ChannelName+:   	CellStyle: �� -  'Microsoft.Windows.Controls.DataGridCell"  .�-J   ��  � ��$ 0q� P� ; P� Property; �@ 
IsSelected< P� Value< . . ���  � ��  HighlightedBrush# [ ��  � ��
 �   �  $2 Status��$
3 Auto $	4 120Q�6 .  $7 ChannelState��=   
IsReadOnly= . 8 �� .R��  �� $
��Auto ��� $
��Auto ��� $��* �.+��� $��imgChannelStateIcon��$��0��$
��Left=�$��5,2,5,2q�$��10��$��10������ *ChannelStateImageSource+�� $��txtChannelState��$��1��$
��Left=�$��2,0,0,0q�$��Center=�9 . ���� *ChannelState+� $��2��y��� > �� Path$> DataContext.TimerEnabled�? �� ElementName$? DGMyChannels��$ Visible��$ Visible�� �� $
��Auto ��� $
��Auto �.+�  $ CustomTimer��$��0,0,5,0q�$��Right=�$��Center=� �� *ChannelInitialTime+y��� *
StateTimer+�� $��lblOnCallState��$��0,0,5,0q�$��1��$��Right=�$��	[Pending]��$��Center=����� " �� �� & �� $> 
StateTimer�' 	Collapsed.a���  � y�$ Visible=��� & �� $> 
StateTimer�' Visible.a���  � y�$ 	Collapsed=�: �� "  .�-K   ��  � ��$ 0q� P� ; �@ 
IsSelected< . . ���  � ��# [ ��  � ��
 �   �  $2 Forward��$3 * $	4 120Q�6 .  $7 Forward��8 �� .R��� $��
txtForward��$��5,0,5,0q�$
��Left=�$��Center=����� *	Forward+: �� "  .�-L   ��  � ��$ 0q� P� ; �@ 
IsSelected< . . ���  � ��# [ ��  � ��
 �   �@ 
  RowStyle@ �� ,  &Microsoft.Windows.Controls.DataGridRow"   P� ; �@ 
IsSelected< . . ���  � ��# [ ��  � ��
 �   �A 
  	CellStyleA �� "  .���  � ��$ 2q��� -M   $��MyStatistics��$��My _Statistics�� . $ 	Recycling=�y��� *StatisticsTabVisibility+.��
  -N   $��DGMyStatistics��$* 0Q�$��5q�
�������$��Stretch=�$��Stretch=�$
��Left=�+ �#D6D7D6, �#D6D7D6- .  . .  / .  0 .  . $ 	Recycling=����� *MyStatistics+1   $2 Key Performance Indicator��$3 * $	4 200Q�6 . $7 StatisticName��8 �� .R��� $��
txtKPIName��$
��Left=�$��2,0,0,0q�B �� ShowOnDisabledB . ���� *StatisticName+  $2 Value��$3 * $	4 200Q�6 .  8 �� .R��� $��
txtKPIName��$
��Left=�$��2,0,0,0q�B . ���� *StatisticValue+���� *ThresoldColor+@ �� "  .���  � �� �� *StatisticType+C �� 	Converter#C [  P� ; �@ 
IsSelected< . . ���  � ��# [ �� -O   $��ContactStatistics��$ ��Con_tact Center Statistics�� . $ 	Recycling=�y��� *StatisticsTabVisibility+.��
  -P   $��DGContactCenterStatistics��$* 0Q�$��5q�
�������$��Stretch=�$��Stretch=�$
��Left=�+ �#D6D7D6, �#D6D7D6- .  0 . . .  / .   . $ 	Recycling=����� *ContactCenterStatistics+1   $3 * $	4 200Q�$2 Contact Center Resource��6 . $7 ContactStatisticName��8 �� .R��� $��	txCCRName��$
��Left=�$��2,0,0,0q�B . ���� *ContactStatisticName+  $3 * $	4 200Q�$2 Description��6 . 8 �� .R��� $��	txCCRDesc��$
��Left=�$��2,0,0,0q�B . ���� *ContactStatisticDesc+  $3 * $	4 200Q�$2 Value��6 .  8 �� .R��� $��
txtKPIName��$
��Left=�$��2,0,0,0q�B . ���� *ContactStatisticValue+���� *ContactThresoldColor+@ �� "  .���  � �� `� D `� 	Converter#D [ .1��� $> ContactStatisticType��� $> ContactStatisticName� P� ; �@ 
IsSelected< . . ���  � ��# [ �� -Q   $��TabMyCampaigns��$��My _Campaigns��.��� -R   $��CampaignTabGrid���� -S   $��
TabHistory��$��My _History��.��� -T   $��HistoryTabGrid���� -U   $��TabMySkills��$��
My S_kills��y��� *SkillsTabVisibility+.��� .+�
  -V   $��DGSkills��$* 0Q�$��5q�
�������$��Stretch=�$��Stretch=�$
��Left=�+ �#D6D7D6E 
  HeadersVisibility$E Column=�, �#D6D7D6- .  0 . F 
  SelectionMode$F Single=�G 
  SelectionUnit$G FullRow=�. .  / .   . $ 	Recycling=����� *
MySkills+ H �� 
TargetTypeI �� PaddingJ � ContextMenuK 
  SelectAllCommandL �� RelativeSourceM �� AncestorTypeN �� TypeInTargetAssemblyO �� 
ResourceId!P 
  HeadersVisibilityConverter:  4Microsoft.Windows.Controls.DataGridHeadersVisibility
Q   AllR �� ConverterParameterJ  DMicrosoft.Windows.Controls.Primitives.DataGridColumnHeadersPresenterS   ColumnT ��NameU �� CanContentScrollV �� OrientationW � OverridesDefaultStyleX �� ScrollableHeightY � MaximumZ �� ViewportHeight[ �� ViewportSize*\ �� #ComputedVerticalScrollBarVisibility] � Value^ �� Mode  VerticalScrollBar_ �� ScrollableWidth` �� ViewportWidth,a �� %ComputedHorizontalScrollBarVisibility  HorizontalScrollBar��%6  '
        7  7  �� " 
 .���  � ��
 �������  � �� �� "H 
 .R��� #��z��#��z��#��z�� . #I z��._��� $��DG_ScrollViewer��-W   |�.  J �� *SkillsContextMenu+���� "H ��.R��  �� $
��Auto ��� $��* ��� $
��Auto � �� $
��Auto ��� $��* ��� $
��Auto �.+��� #��ZK |�.  ���� L �� "M 
 $ > CellsPanelHorizontalOffset���C� *�� "N 
 $"O DataGridSelectAllButtonStyle��+y��� L �� "M 
 $> HeadersVisibility�#C ZP #R ZQ   $!��PART_ColumnHeadersPresenter��$��1��y��� L �� "M 
 $> HeadersVisibility�#C ZP #R ZS �� $!T PART_ScrollContentPresenter��$��1��$��2��#U z���� $��PART_VerticalScrollBar��$��1��$��1��$V Vertical=�$��Right=�$��0,0,0,0q� . W . #Y zX #[ zZ #y�z\ ] �� $> VerticalOffset�L �� *TemplatedParent+$^ OneWay=����� " ��.���  �@J�SnapsToDevicePixels . ��  �@�OverridesDefaultStyle .  P� ; �@��Orientation$< Vertical=�. ���  � ��$ 18����  � ��$
 Auto����  � ��8   � $��2��$��1�� �� ���� L �� "M 
 $.> (NonFrozenColumnsViewportHorizontalOffset��� $��* �.+��� $��PART_HorizontalScrollBar��$��1��$V 
Horizontal=�#Y z_ #[ z` #y�za ] �� $> HorizontalOffset�L �� *TemplatedParent+$^ OneWay=����� " ��.���  �@J�SnapsToDevicePixels . ��  �@�OverridesDefaultStyle .  P� ; �@��Orientation$< 
Horizontal=�. ���  � ��$
 Auto����  � ��$ 18����  � ��8  .���� # z 1   $3 * $	4 200Q�$
2 Name��9 . 6 . $7 	SkillName��: �� "  .���  � �� �� "H  .R�� #��z��.+��� $��Center=�-X   ��  � ��$ 0q� P� ; �@ 
IsSelected< . . ���  � ��# [ ��  � ��
 �   �8 �� .R��� $��txtSkillName��$
��Left=�$��2,0,0,0q����� *	SkillName+  $3 * $	4 200Q�$2 Level��6 . 8 �� .R��� $��txtSkillLevel��$
��Left=�$��2,0,0,0q�B . ���� *
SkillLevel+: �� "  .���  � �� �� "H  .R�� #��z��.+��� $��Center=�-Y   ��  � ��$ 0q� P� ; �@ 
IsSelected< . . ���  � ��# [ ��  � ��
 �   �@ �� "  .���  � ��$ 0q���  � ��$ 20�� P� ; �@ 
IsSelected< . . ���  � ��# [ �� -Z   $��
TabWorkbin��$��My Work_bin�� . $ 	Recycling=�.��]� -[   $��dockWorkbin�� . $ 	Recycling=��� -\   $��calldata��$��_Interaction Data�� . $ 	Recycling=�y��� *A_DataTabVisibility+.���� -]   $��tb_CallData��$��5q� . $ 	Recycling=�.@��� -^   $��Interactiondata��$��Interaction Data��.��� -_   $��grdInteractionData�� �� $
��Auto ��� .+�� $
 1000��$��Right=�$	��Top=�$��0�� �� �� �� .+��� -`   $��btnHimss��$��0��|�.  #��[ $��0,4,5,0q�y��� *IsShowMIDPushButton+$ 	Collapsed��$ 	Collapsed��.���� $ Vertical=�$��Center=�.+��� $;��5/Agent.Interaction.Desktop;component/Images/popup.png��$��18��$��18���� $��Center=�$ Center=�$��	MID Popup��$��Calibri)�$��11(�$
��Wrap=�$��2,0,2,0q��� -a   $��btnAddCallData��$��1��|�.  $��30��$��30��$��0,4,5,0q�#��[ $	��Top=�$��Right=�$��Stretch=�y��� *IsVoiceEnabledAddCallData+.���� $ Vertical=�$��Center=�.+��� $9��3/Agent.Interaction.Desktop;component/Images/Add.png��$��18��$��18���� $��Center=�$ Center=�$	��Add��$��Calibri)�$��11(�$
��Wrap=�$��2,0,2,0q� �� 
����������#ADAAAD$	��0.5q�$��-5q�._��  �� $
��Auto ��� $��* �.+��� $	��Add��$��0��$ 	10,5,10,0q�$
��Bold"�$��Calibri)��� $��1��$ 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200��$��Insert addition call data���� -b   $��btnOpenCallDataWin��$��2��|�.  $��0,4,5,0q�#��[ $	��Top=�$��Right=�$��Stretch=�y��� *IsVoicePopCaseData+.���� $ Vertical=�$��Center=�.+��� $K��E/Agent.Interaction.Desktop;component/Images/Voice/Call.AttachData.png��$��18��$��18���� $��Center=�$ Center=�$��Float Case Data��$��Calibri)�$��11(�$
��Wrap=�$��2,0,2,0q� �� 
����������#ADAAAD$	��0.5q�$��-5q�._��  �� $
��Auto ��� $��* �.+��� $��CaseData Popup��$��0��$ 	10,5,10,0q�$
��Bold"�$��Calibri)��� $��1��$ 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200��$"��Open call data pop-up Window���� -c   $��txtAttachDataInfo��$��1��$��10q�$��No attach data found.��$��Calibri)�$��12(�$��Stretch=�$��Center=�$
��Auto��
  -d   $��DGAttachData��$��1��$* 0Q�
�������$
G Cell=�$F Single=�$��Stretch=�$	��Top=�$��Stretch=�+ �#D6D7D6, �#D6D7D6$
��Auto=�- .  . .  / .  0 . b 
  ClipboardCopyMode$
b None=����� *NotifyCallDataView+1 3  -Microsoft.Windows.Controls.DataGridTextColumn  $	2 Key��$	3 180 $	4 150Q�$	5 200Q�B . = . ���� *
Fontweight+c   
FontFamilyc �� *
Fontfamily+4  .Microsoft.Windows.Controls.DataGridBoundColumnd   Bindingd �� *Key+e   ClipboardContentBindinge Z� f   ElementStylef �� " ��.���  � ��$
 Wrap=���  � ��$ 2q���  �@��Padding$ 2q�  $2 Value��$	3 10* $	4 190Q�-e   cdValue��g   ClipboardContentBindingg �� *Value+#C [ 8 �� .R��� $��txtValue��$
��Left=�$
��Wrap=�$��2q�$ 2q�$	��200��$��Normal"�$��Calibri)�B . h � Cursorh �� *Value+#C [ ���� *Value+#C [ ���� *Value+#C [ J �� .@�m� $
��Copy��-f   $
��Copy��i m� InputGestureText$i  ��j   CellEditingTemplatej �� .R�� $��	editStack��$��Stretch=�$��0,0,20,0q����� $? cdValue��$> Width� �� $��* ��� $
��Auto ��� $
��Auto �.+��� $��txtValue��$��0��k �� 	UndoLimit$k 10��$��Stretch=�$��2q�$��2q�l �� TextWrapping$
l Wrap=�$	��100��  DefaultCopyContextmenu#J [ ���� *Value+$^ TwoWay=��� $��	btnUpdate��-g   $��1��$
h Hand��$��2,0,2,0q�$
��Left=�$��Center=�$
��Auto��$
��Auto�����#007EDF
������ .���� $ Vertical=�$��Center=�.+��� $:��4/Agent.Interaction.Desktop;component/Images/Edit.png��$��10��$��10�� �� 
����������#ADAAAD$	��0.5q�$��-5q�._��  �� $
��Auto ��� $��* �.+��� $��UpdateHeading��$��Update��$��0��$ 	10,5,10,0q�$
��Bold"�$��Calibri)��� $��UpdateContent��$��1��$ 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200��$��Agent can update data.���� $��	btnCancel��-h   $��2��$
h Hand��$��2,0,2,0q�$
��Left=�$��Center=�$
��Auto��$
��Auto�����#007EDF
������ .���� $ Vertical=�$��Center=�.+��� $<��6/Agent.Interaction.Desktop;component/Images/cancel.png��$��10��$��10�� �� 
����������#ADAAAD$	��0.5q�$��-5q�._��  �� $
��Auto ��� $��* �.+��� $��CancelHeading��$��Cancel��$��0��$ 	10,5,10,0q�$
��Bold"�$��Calibri)��� $��CancelContent��$��1��$ 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200��$��Agent can cancel data.��: �� "  .���  � ��$ 0q� P� ; �@ 
IsSelected< . . ���  � ��# [ ��  � ��
 �   ���  �@+�FocusedElement �� $? txtValue���� -i   $��commonDispositionTab��$��Disposition Code��y��� *DispositionVisibility+.��� -j   $��dispositionGrid��$��5q�$��Stretch=��� -k   $��voiceContact��$��Contact��.��� -l   $��gridVoiceContact���� -m   $��DispositionTab��$��_Disposition Code�� . $ 	Recycling=�y��� *DispositionOnlyVisibility+.���� -n   $��stk_Dispositioncodeonly��$	��150��$��Stretch=�$ Vertical=�$��5q� . $ 	Recycling=��� -o   $��popUpUrl��$��
_Popup Url�� . $ 	Recycling=�.��� -p   $��gridPopupURL��.+��� -q   $��tbSubTab��$��Stretch=�$��Stretch=� . $ 	Recycling=�.@��� -r   $��firstTab��$��	First Tab��.��� -s   $��	gridFirst��
���   �.+�(  "System.Windows.Controls.WebBrowser  -t   $��firstBrowser��$��Stretch=�$��Stretch=��� -u   $��
imgLoading��$y�	Collapsed=�$ 0��$��Center=�$��Center=�$=! 7/Agent.Interaction.Desktop;component/Images/Loading.GIF��$��80��$��80��  -v   $��
webBrowser��$��Stretch=�$��Stretch=��� -w   $��MessageTabControl��$
��Auto��$��5,5,5,0q�$( Bottom=� . $ 	Recycling=�y��� *MessageTabVisibility+.@��� $��My Messa_ges��.��� .+�
  -x   $��DGMyMessages��$��Stretch=�$* 0Q�$��5q�
�������$��Stretch=�$
��Left=�+ �#D6D7D6, �#D6D7D6- .  . .  / .  0 .  . $ 	Recycling=����� *
MyMessages+1   m   
Visibility$m Hidden=�d �� *Index+  $3 20 $4 20Q�$5 20Q�8 �� .R��� $��imgMessageIcon��$��15��$��15������ *MessageIconImageSource+  $
2 Type��6 . $7 MessageType��$3 90 $4 80Q�8 �� .R��� $��txtMessageType��$
��Left=�$��Center=�$��2,0,0,0q����� *MessageType+  $2 Sender��6 . $7 MessageSender��$3 90 $4 80Q�8 �� .R��� $��txtMessageSender��$
��Left=�$��Center=�$��2,0,0,0q����� *MessageSender+  $2 Subject��6 . $7 MessageSubject��$	3 150 $	4 120Q�8 �� .R��� $��
txtSubject��$��2,0,0,0q�$
��Left=�$��Center=����� *MessageSubject+  $2 Priority��6 . $7 MessagePriority��$3 90 $4 80Q�8 �� .R��� $��txtPriority��$
��Left=�$��Center=����� *MessagePriority+���� *MessagePriority+#C [    $
2 Date��6 . $7 MessageDate��$	3 140 $	4 110Q�8 �� .R��� $��txtDate��$
��Left=�$��Center=����� *MessageDate+  $2 Audience��6 . $7 MessageAudience��$3 * $4 70Q�8 �� .R��� $��txtAudience��$
��Left=�$��Center=����� *MessageAudience+@ �� "   �� & �� *ISread+	' False.a���  � ��$ DemiBold"��� & �� *ISread+' True.a���  � ��$ Normal"�P� ; �@ 
IsSelected< . . ���  � ��# [ P� ; �@J�IsMouseOver< . . ���  �@�ToolTip _� n _� HasDropShadown .  o _� 	Placement$o 
MousePoint=�p _� 	StaysOpenp . $��Bottom=���Z� ��Z� .���� ���#ADAAAD$��1q�$��2q�$
��Auto��$
��Auto��
�������._��  �� $
��Auto ��� $
��Auto � �� $
��Auto �.+��� $��
lblSubject��$��0��$��0��$
��Left=�$��2q�$��Subject:���� $��
txtSubject��$��1��$��0��$
��Left=�$��2q�$	��350��$
��Wrap=����� *MessageSubject+.�-y   �� -z   $��txtNoBroadcastMessage��$��Center=�$	��Top=�$
��0,10q��� -{   $��ContactTabControl��$
��Auto��$��5,5,5,0q�$( Bottom=� . $ 	Recycling=�y��� *ContactTabVisibility+.@��� $��Cont_act Directory��.��� -|   $��ContactTabGrid��$��Stretch=�$��Stretch=��� -}   $��btn_Hide��$
 1000��$	��Top=�$��Right=�#��[ $��18��$��18��$��6q�.���� $K��E/Agent.Interaction.Desktop;component/Images/Custom Close selected.png��$��10��$��10�� �� 
����������#ADAAAD$	��0.5q�$��-5q�._��  �� $
��Auto ��� $��* �.+��� $
��Hide��$��0��$ 	10,5,10,0q�$
��Bold"�$��Calibri)��� $��1��$)��#Hide the workspace supporting views��$ 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200��� -~   $��
popupState��$��Bottom=�q � 	StaysOpenq .  r � AllowsTransparencyr . $��Bottom=�.���� ���#ADAAAD$��1q�$��10q�$
��Auto��$
��Auto��
�������._��  �� $
��Auto ��� $
��Auto � �� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto �.+��� $ 
Horizontal=�$��2��$��0��$
��Left=�$��10,5,0,5q�.+��� -   $��img_agentStatus��$��10��$��10������ *AgentStateImageSource+�� -�   $��lblagentName��$
��Bold"�$��12(�$��5,0,0,0q����� *AgentFullName+�� -�   $��lblLoggedIN_time��$��2��$��1��$
��Left=�$��40,3,0,0q��� -�   $��
lblLoginID��$��2��$��2��$
��Left=�$��40,3,0,0q��� -�   $��lblagentPlace��$��2��$��3��$
��Left=�$��40,3,0,0q����� *	UserPlace+
  -�   $��DGMyChannel��$* 0Q�
������ $��	10,10,5,5q�
�������$
��Left=�$��Center=�$
��Left=�
+ ���� 
, ���� - .  . .  / .  $��2��$��4��$
E None=����� *MediaStatus+1   $2 Media��$
3 Auto $4 70Q�8 �� .R��� $ 
Horizontal=�.+��� $��imgChannelIcon��$��15��$��15������ *ChannelIconImageSource+�� $��txtChannelName��$
��Left=�$��2,0,0,0q����� *ChannelName+  $2 Status��$
3 Auto $	4 130Q�8 �� .R��  �� $
��Auto ��� $
��Auto ��� $
��Auto �.+��� $��imgChannelStateIcon��$��0��$
��Left=�$��5,0,5,0q�$��10��$��10������ *ChannelStateImageSource+�� $��txtChannelState��$��1��$
��Left=�$��2,0,0,0q����� *ChannelState+  $��5,0,5,0q�$��2��$��Right=� �� *ChannelInitialTime+y��� *
StateTimer+�� $��5,0,5,0q�$��2��$��Right=�$��	[Pending]��$��Center=����� " �� �� & �� $> 
StateTimer�' 	Collapsed.a���  � y�$ Visible=��� & �� $> 
StateTimer�' Visible.a���  � y�$ 	Collapsed=�: �� "  .�-�   ��  � ��$ 0q� P� ; �@ 
IsSelected< . . ���  � ��# [ ��  � ��
 �   �@ �� "   P� ; �@ 
IsSelected< . . ���  � ��# [ 