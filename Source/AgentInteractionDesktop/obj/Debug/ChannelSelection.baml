   M S B A M L   `   `   `  ����p  lSystem.Windows.Controls.Input.Toolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35pTclr-namespace:System.Windows.Controls;assembly=System.Windows.Controls.Input.ToolkitSystem.Windows.Controls  5   	   U QAgent.Interaction.Desktop, Version=5.0.3.27, Culture=neutral, PublicKeyToken=nullc6clr-namespace:Agent.Interaction.Desktop.CustomControls(Agent.Interaction.Desktop.CustomControls 5   	   0   *Agent.Interaction.Desktop.ChannelSelection   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   cmyTclr-namespace:System.Windows.Controls;assembly=System.Windows.Controls.Input.Toolkit     5   	   PCustomControl6clr-namespace:Agent.Interaction.Desktop.CustomControls     5   	   -   5        $��channelWindow��5        � Title$  Login��6	    � WindowStyle$
 None=�6,    � Icon$F @/Agent.Interaction.Desktop;component/Images/pointelicon16x16.ico��6?    � AllowsTransparency . 
������  � WindowStartupLocation$ CenterScreen=�5   <   $	��400��6a   $	��400��6m    � SizeToContent$ Height=�6|    � 	Resources 5       �� MergedDictionaries �� Source�� 5	   
    5
      �� 5      $R L/Agent.Interaction.Desktop;component/Resources/StylesResourceDictionary.xaml?�5      %    5      .��5      � .+�5   
   �� -   5        $��
MainBorder��5      $
��0.89q�6#   $��8q�6:   
�������._�5      � 	 � RowDefinitions	 5      �� 5      $
��Auto �6$   64   �� 5      $
��Auto �6%   65   �� 5      $
��Auto �6$   64   �� 5      $
��Auto �6$   64   .+�5      � -   5        $��0��5      
�������.+�5      �� -   5        $��5,0,0,0q�5   !   
�������$��Center=�6E   
 �� Orientation$
 
Horizontal=�6�   .+�5      �� -   5        $��image1��5       $
��Auto��6Y   $��40��6f   $G��A/Agent.Interaction.Desktop;component/Images/Pointel Softphone.png��6r   �� 5      $
 Vertical=�6%   .+�5      �� -   5        $��18(�5   $   $��0,3,0,0q�62   ���#007edf$
��Bold"�6�      keyTitle#���   6�   $��Calibri)�6�   6�   �� 5      -   5        $��channelTitleversion��5   $   $��
140,-7,0,3q�6?     keyTitleVersion#���  6~   $��Calibri)�6�   $��12(�6�   6�   5      5       5!      �� 5"      $��0.5,0.5,0.5,0q�6   ���#656d90$��5,0,5,0q�6O   $��1��6`   ._�5#      �  � ColumnDefinitions 5$      �� 5%      $
��161* �6/   6>   �� 5&      $
��237* �6/   6>   	 5(      �� 5)      $	��43* �6,   6;   �� 5*      -   5         �Name$ grdrow_cserror��5*   ,   ��6B   �� *CSErrorRowHeight+6h   �� 5+      $��1* �6,   6:   �� 5,      -	   5        $ grdRowVoice��5,   9   $	��23* �6,   6N   �� 5-      -
   5        $ grdRowAgentlogin��5-   ,   $	��23* �6D   6S   �� 5.      -   5        $ grdRowEmail��5.   ,   $
��Auto �6?   6O   �� 5/      -   5        $ 
grdRowChat��5/   ,   $
��Auto �6>   6N   �� 50      -   5        $ grdRowOutbound��50   ,   $
��Auto �6B   6R   .+�57      �� -   5        $��lblUserName��57   f     
KeyWelcome#���  6    $��2��6G   $��2q�6[   ���#E1F2FA6y   �� 58      -   5        $��label1��58   ]     KeyWelcomeinfo#���  6    $��2,20,0,0q�6K   $��2��6k   �� 59      -   5        $��lblPlace��59   E     KeyPlace#���  6    $��2��6U   $��	10,10,0,0q�6b   �� 5;      -   5        $��
stkp_Error��5;   `   $
 
Horizontal=�6%   $��1��6?   $��2��6L   $��3,0,3,3q�6r   
��� ���$��Stretch=�6�   .+�5<      �� $��5,2,0,2q�6$   $	��Top=�65   $;��5/Agent.Interaction.Desktop;component/Images/Error.png��6M   $��20��6�   $��20��6�   �� 5=      -   5        $��lblInformation��5=   (   $��10,5,5,5q�6>   $	��300��6P   $
��Wrap=�6|   ��6\   �� *CSErrorMessage+5>      5C      @  :Agent.Interaction.Desktop.CustomControls.AutoCompleteBoxEx  5F      -   5          Name$ txtPlace��5F   :   @  :Agent.Interaction.Desktop.CustomControls.AutoCompleteBoxEx   SelectionChanged$ txtPlace_SelectionChanged��6�     PreviewKeyUp$ txtPlace_PreviewKeyUp��6�     GotFocus$ txtPlace_GotFocus��6�     TextChanged$ txtPlace_TextChanged��6�  $��2��6L   $��1��6Y   $��2,2,12,0q�6i   $��23��6{   ���#ADAAAD
�������
���   �$��Calibri)�6  $��12(�6  $	��Top=�6N  $��Stretch=�6f  -   'System.Windows.Controls.AutoCompleteBox   SelectedItem 6�   �� *Place+ �� Mode$ TwoWay=�   Text 6�   �� *Place+   ItemsSource 6)  �� *AutoComplete+   ItemTemplate 5G      �� 5H   "   .R�5I   &   �� $��innertxt��60   
���   �$	��190��6d   $��2q�6p   $��Calibri)�6{   $��12(�6�   ��6@   �� 6�   5J   #    5L       �� 
TargetType �� Property �� Value �� Triggers P� Property P� Value  HorizontalScrollBar  VerticalScrollBar��%]  '�       '�� H     7  7  �� 5M   "   " �6(   .�5N   &   ��  � ��$ 85��6B   5O   #   �� 5P   "   " ��6(   .�5Q   &   ��  �@J�SnapsToDevicePixels . 6L   �� 5R   &    �@�OverridesDefaultStyle . 6N    5S   &   P� 5T   *    �@��Orientation$ 
Horizontal=�6I   . �5U   .   ��  � ��$
 Auto��6F   �� 5V   .    � ��$ 18��6G   �� 5W   .    � ��8   6I   5X   +   P� 5Y   *    �@��Orientation$ Vertical=�6I   . �5Z   .   ��  � ��$ 18��6F   �� 5[   .    � ��$
 Auto��6G   �� 5\   .    � ��8  6I   5]   +   5_   #   5`      5a      �� 5b      -   5        $��gbVoiceChannel��5b   O   $��5q�6#   $��3��6.   $��2��6;   ���#ADAAAD$��1q�6{   ��5c      �� 5d   "   -   5         ��Name$ 
chkbxVoice��5d   +     KeyVoice#���  6?   $��15��6d   {�.  d� 	IsChecked 6p   �� *IsVoiceChecked+5e   #   .��5h      � -   5        $��grd_VoiceLoginDetails��5h   #   $��1q�6@   $
��Auto��6K   	 5i   "   �� 5j   &   ��64   �� *LoginQueueRowHeight+6]   �� 5k   &   ��64   �� *AgentLoginIDRowHeight+6_   �� 5l   &   ��64   �� *AgentPasswordRowHeight+6`   �� 5m   &   ��64   �� *LDCodeRowHeight+6Y   �� 5n   &   ��64   �� *WrapTimeRowHeight+6[   �� 5o   &   -   5        $ grdPHS1��5o   4   $
��Auto �6C   6S   �� 5p   &   -   5        $ grdPHS2��5p   4   $
��Auto �6C   6S    5r   "   �� 5s   &   $
��158* �67   6F   �� 5t   &   $
��234* �67   6F   .+�5v   "   �� -   5        $��brdHide��5v   )   $y�	Collapsed=�68    K� ZIndex$	 100��6O   $��6��6b   $��2��6s   
�������  J� Opacity$
  0.35Q�6�   $��Stretch=�6�   $��Stretch=�6�   �� 5w   "   -   5        $��lblQueue��5w   (     KeyQueue#���  68   $��0��6]   $��0��6j   {�6z   �� ! �� ElementName$! 
chkbxVoice��" �� Path$" CheckBox.IsChecked��� 5x   "   -   5        $��cbQueue��5x   R   $��23��6+   $
��Left=�67   $	��Top=�6a   # �� SelectedIndex$# 0��6y   $��1��5y   a   $ �� 
IsEditable$ . $��2��6�   ��5x   �   �� *LoadCollection+% �� SelectedValue% 5y   '   �� *QueueSelectedValue+$ TwoWay=�{�6�   �� $! 
chkbxVoice��$" CheckBox.IsChecked�6�   �� 5{   "   -   5        $��lblAgentLogin��5{   (   $��Center=�6=    	 KeyAgentLogin#��� 	 6_   $��1��6�   $��0��6�   {�6�   �� $! 
chkbxVoice��$" CheckBox.IsChecked�6�   �� 5|   "   -   5        $��lblAgentLoginID��5|   (   $�� ��6?   $��1��6K   $��1��6X   $��2��6�   {�6h   �� $! 
chkbxVoice��$" CheckBox.IsChecked�6�   �� 5}   "   -   5        $��cbAgentLoginID��5}   +   $��23��6A   $��1��6M   $��1��6Z   $y�Hidden=�6j   $# 0��6�   ��6~   �� *AgentLoginIds+6�   �� 5   "   -   5        $��lblAgentPassword��5   (   $��Center=�6@    
 KeyAgentPassword#��� 
 6b   $��2��6�   $��0��6�   {�6�   �� $! 
chkbxVoice��$" CheckBox.IsChecked�6�   G� 5�   "   -    5        $��txtAgentPassword��5�   .   $��2��6F   $��1��6S   $��23��6c   $��Stretch=�6o   $��2��6�     controltempPasswordbox#��[ 6�   {�6�   �� $! 
chkbxVoice��$" CheckBox.IsChecked�69  �� 5�   "   -!   5        $��	lblLDCode��5�   (   $��Center=�69     	KeyLDCode#���  6[   $��3��6�   $��0��6�   {�6�   �� $! 
chkbxVoice��$" CheckBox.IsChecked�6�   G� 5�   "   -"   5        $��	txtLDCode��5�   .   $��3��6?   $��1��6L   $��23��6\   $��Stretch=�6h   $��2��6�   #��[ 6�   {�6�   �� $! 
chkbxVoice��$" CheckBox.IsChecked�6\  �� 5�   "   -#   5        $��lblWrapTime��5�   (   $��4��6<   $��0��6I   $��Center=�6Z     KeyWrapTime#���  6|   {�6�   �� $! 
chkbxVoice��$" CheckBox.IsChecked�6�   �� 5�   "   -$   5        $��txtWrapTime��5�   *   $��4��6=   $��1��6J   $��23��6Z   $��Stretch=�6f     controltempTextbox#��[ 6�   {�6�   �� $! 
chkbxVoice��$" CheckBox.IsChecked���6�   �� *
WrapTime+6@  �� 5�   "   -%   5        $��chkbxMediPac��5�   +   $��Login To MediPac��6@   
& � Tag$& MediPac��6[   $��8,0,0,0q�6i   $��14,5,0,5q�6{   $y�	Collapsed=�6�    6�   �� *IsMediPacChecked+6  �� 5�   "   -&   5        $��chkbxFacets��5�   +   $��Launch Facets��6?   $& Facets��6W   $��8,0,0,0q�6d   $��14,5,0,5q�6v   $y�	Collapsed=�6�    6�   �� *IsFacetsChecked+6  �� 5�   "   -'   5        $��	chkbxEpic��5�   +   $��Launch Epic��6=   $
& Epic��6S   $��8,0,0,0q�6^   $��14,5,0,5q�6p   $y�	Collapsed=�6�    6�   �� *IsEpicChecked+6�   �� 5�   "   -(   5        $��chkbxLawson��5�   +   $��Launch Lawson��6?   $& Lawson��6W   $��8,0,0,0q�6d   $��14,5,0,5q�6v   $y�	Collapsed=�6�    6�   �� *IsLawsonChecked+6  5�      5�      �� 5�      -)   5        $��
chkbxEmail��5�   #   $��5��65   $��email��6B   $��14,5,0,5q�6R   $y�	Collapsed=�6�    6d   �� *IsEmailChecked+6�   �� 5�      -*   5        $��	chkbxChat��5�   #   $��6��64   $
��chat��6A   $��14,5,0,5q�6P   $y�	Collapsed=�6�    6b   �� *IsChatChecked+6�   �� 5�      -+   5        $��chkbxOutbound��5�   #   $��7��68   $��outboundpreview��6E   $��8,0,0,0q�6_   $��14,5,0,5q�6q   $y�	Collapsed=�6�    6�   �� *IsOutboundChecked+6  5�      5�      �� 5�      ���#656d90$��0.5,0.5,0.5,0.5q�6/   $��5,0,5,0q�6Q   $��2��6b   ._�5�      � $��30��6   .+�5�      �� -,   5        $��	btnCancel��5�   -   $��23��6!   $��70��6>   $��Right=�6I   $��0,0,10,0q�6e   $��Center=�6w     NormalButton#��[ 5�   %   6K   �� 5�      --   5        $��btnOk��5�   [   $��23��6!   $��Right=�6-   $��0,0,90,0q�6I   $��Center=�6h   $��70��6�   #��[ 5�   %   6K   �� 5�      -.   5        $��btnChangeLogin��5�   Z   $��20��6!   $
��Left=�6-   $��10,6,0,0q�6H   $	��Top=�6p   $
��Auto��6�   ���#174292
������ .��5�      ��   KeyChangeLoginAccount#��[ 6(   $��	Underline{�6V   $��16��6r   ��5�   "   �� 5�   &   " ��6,    5�   *   P� 5�   .    �@J�IsMouseOver . . �5�   2   ��  � �� �#b8e0ff6P   5�   /   5�   '   5�      ��5�      �� 5�   "   ' �� 
TargetType"' ��62   .R�5�   &   �� #��z��67   5�   #   5�      5�      5�      �� 5�      $��30��6   ���#007edf$��Stretch=�6:   $	��Top=�6X   $��0,3,0,0q�6p   $��3��6�   ._�5�      � .+�5�      �� $��© 2013-2017 Pointel Inc��6$   $
��Left=�6C   $��Center=�6^   $��Calibri)�6y   $��12(�6�   
�������$��5,0,0,0q�6�   �� 5�      $��Right=�6H   $��Center=�6d   ���#EA3737( �� CornerRadius$( 	10,0,0,10��6�   y�6!   �� *TrialVisibility+._�5�      �� $��Calibri)�6F   $��Center=�6[   $��Center=�6x   $��12(�6�   
�������$��	15,2,10,2q�6�   ��6(   �� *TrailMessage+6�   5�      5�      5�      5�      5�      5�      5�      