   M S B A M L   `   `   `  ����p  lSystem.Windows.Controls.Input.Toolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35pTclr-namespace:System.Windows.Controls;assembly=System.Windows.Controls.Input.ToolkitSystem.Windows.Controls  5   	   U QAgent.Interaction.Desktop, Version=5.0.3.27, Culture=neutral, PublicKeyToken=nullc6clr-namespace:Agent.Interaction.Desktop.CustomControls(Agent.Interaction.Desktop.CustomControls 5   	   +   %Agent.Interaction.Desktop.RefinePlace   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   cmyTclr-namespace:System.Windows.Controls;assembly=System.Windows.Controls.Input.Toolkit     5   	   PCustomControl6clr-namespace:Agent.Interaction.Desktop.CustomControls     5   	   -   5        $��RefineWindow��5        � Title$  Login��6	    � WindowStyle$
 None=�6+    � Icon$F @/Agent.Interaction.Desktop;component/Images/pointelicon16x16.ico��6>    � AllowsTransparency . 
������  � WindowStartupLocation$ CenterScreen=�5   <   $	��400��6a   $	��400��6m    � ShowInTaskbar .   � SizeToContent$ Height=�5   	    � 	Resources 5
       �� MergedDictionaries	 �� Source�� 5   
    5      �� 5      $R	 L/Agent.Interaction.Desktop;component/Resources/StylesResourceDictionary.xaml?�5      %    5      .��5      � .+�5   
   �� -   5        $��
MainBorder��5      $
��0.89q�6#   $��8q�6:   
�������._�5      � 
 � RowDefinitions
 5      �� 5      $
��Auto �6$   64   �� 5      $
��Auto �6%   65   �� 5      $
��Auto �6$   64   �� 5      $
��Auto �6$   64   .+�5      � $��0��6   
�������.+�5      �� $��5,0,0,0q�6!   
�������$��Center=�6E    �� Orientation$ 
Horizontal=�6b   .+�5      �� -   5        $��image1��5       $
��Auto��6.   $��40��6;   $G��A/Agent.Interaction.Desktop;component/Images/Pointel Softphone.png��6G   �� 5      $ Vertical=�6%   .+�5      �� $��18(�6$   $��0,3,0,0q�62   ���#007edf$
��Bold"�6X      keyTitle#���   6j   $��Calibri)�6�   6�   �� 5       -   5        $��channelTitleversion��5    $   $��
140,-7,0,3q�6?     keyTitleVersion#���  6S   $��Calibri)�6   $��12(�6�   6�   5!      5"      5#      �� 5$      $��0.5,0.5,0.5,0q�6   ���#656d90$��5,0,5,0q�6O   $��1��6`   ._�5%      �  � ColumnDefinitions 5&      �� 5'      $
��161* �6/   6>   �� 5(      $
��237* �6/   6>   
 5*      �� 5+      $	��43* �6,   6;   �� 5,      -   5         �Name$ grdrow_cserror��5,   ,   ��6B   �� *CSErrorRowHeight+6h   �� 5-      $��1* �6,   6:   �� 5.      $	��23* �6,   6;   �� 5/      -   5         ��Name$ grdRowAgentlogin��5/   ,   $	��23* �6F   6U   �� 50      -   5        $ grdRowEmail��50   ,   $
��Auto �6?   6O   �� 51      -   5        $ 
grdRowChat��51   ,   $
��Auto �6>   6N   �� 52      -	   5        $ grdRowOutbound��52   ,   $
��Auto �6B   6R   .+�59      �� -
   5        $��lblUserName��59   f     
KeyWelcome#���  6    $��2��6G   $��2q�6[   ���#E1F2FA6y   �� 5:      -   5        $��label1��5:   ]     KeyWelcomeinfo#���  6    $��2,20,0,0q�6K   $��2��6k   �� 5;      -   5        $��lblPlace��5;   E     KeyPlace#���  6    $��2��6U   $��	10,10,0,0q�6b   �� 5=      -   5        $��
stkp_Error��5=   `   $ 
Horizontal=�6%   $��1��6?   $��2��6L   $��3,0,3,3q�6r   
��� ���$��Stretch=�6�   .+�5>      �� $��5,2,0,2q�6$   $	��Top=�65   $;��5/Agent.Interaction.Desktop;component/Images/Error.png��6M   $��20��6�   $��20��6�   �� 5?      -   5        $��lblInformation��5?   (   $��10,5,5,5q�6>   $	��300��6P   $
��Wrap=�6|   ��6\   �� *CSErrorMessage+5@      5E      @  :Agent.Interaction.Desktop.CustomControls.AutoCompleteBoxEx  5H      -   5          Name$ txtPlace��5H   :   @  :Agent.Interaction.Desktop.CustomControls.AutoCompleteBoxEx   SelectionChanged$ txtPlace_SelectionChanged��6�     PreviewKeyUp$ txtPlace_PreviewKeyUp��6�     GotFocus$ txtPlace_GotFocus��6�  $��2��6L   $��2��6Y   $��2,2,12,2q�6i   $��23��6{   ���#ADAAAD
�������
���   �$��Calibri)�6  $��12(�6  $��Center=�6N  $��Stretch=�6i  -   'System.Windows.Controls.AutoCompleteBox   SelectedItem 6�   �� *Place+ �� Mode$ TwoWay=�   Text 6�   �� *Place+   ItemsSource 6)  �� *AutoComplete+   ItemTemplate 5I      �� 5J   "   .R�5K   &   �� $��innertxt��60   
���   �$	��190��6d   $��2q�6p   $��Calibri)�6{   $��12(�6�   ��6@   �� 6�   5L   #    5N       �� 
TargetType �� Property �� Value �� Triggers P� Property P� Value  HorizontalScrollBar  VerticalScrollBar��%]  '�       '�� H     7  7  �� 5O   "   " �6(   .�5P   &   ��  � ��$ 85��6B   5Q   #   �� 5R   "   " ��6(   .�5S   &   ��  �@J�SnapsToDevicePixels . 6L   �� 5T   &    �@�OverridesDefaultStyle . 6N    5U   &   P� 5V   *    �@��Orientation$ 
Horizontal=�6I   . �5W   .   ��  � ��$
 Auto��6F   �� 5X   .    � ��$ 18��6G   �� 5Y   .    � ��8   6I   5Z   +   P� 5[   *    �@��Orientation$ Vertical=�6I   . �5\   .   ��  � ��$ 18��6F   �� 5]   .    � ��$
 Auto��6G   �� 5^   .    � ��8  6I   5_   +   5a   #   5b      5c      �� 5d      -   5        $��gbVoiceChannel��5d   O   $��5q�6#   $��3��6.   $��2��6;   ���#ADAAAD$��1q�6{   ��5e      �� 5f   "   -   5         ��Name$ 
chkbxVoice��5f   +     KeyVoice#���  6?   $��15��6d   {�.  d� 	IsChecked 6p   �� *IsVoiceChecked+5g   #   .��5j      � -   5        $��grd_VoiceLoginDetails��5j   #   $��1q�6@   $
��Auto��6K   
 5k   "   �� 5l   &   ��64   �� *LoginQueueRowHeight+6]   �� 5m   &   ��64   �� *AgentLoginIDRowHeight+6_   �� 5n   &   ��64   �� *AgentPasswordRowHeight+6`   �� 5o   &   ��64   �� *WrapTimeRowHeight+6[    5q   "   �� 5r   &   $
��140* �67   6F   �� 5s   &   $
��166* �67   6F   �� 5t   &   $	��41* �67   .+�5v   "   �� -   5        $��lblQueue��5v   (     KeyQueue#���  68   $��0��6]   $��0��6j   {�6z   ��   �� ElementName$  
chkbxVoice��! �� Path$! CheckBox.IsChecked��� 5w   "   -   5        $��cbQueue��5w   R   $��23��6+   $
��Left=�67   $	��Top=�6a   " �� SelectedIndex$" 0��6y   $��1��5x   a   # �� 
IsEditable# . $��2��6�   ��5w   �   �� *LoadCollection+$ �� SelectedValue$ 5x   '   �� *QueueSelectedValue+$ TwoWay=�{�6�   �� $  
chkbxVoice��$! CheckBox.IsChecked�6�   �� 5y   "   -   5        $��lblAgentLogin��5y   (   $��Center=�6=    	 KeyAgentLogin#��� 	 6_   $��1��6�   $��0��6�   {�6�   �� $  
chkbxVoice��$! CheckBox.IsChecked�6�   �� 5z   "   -   5        $��lblAgentLoginID��5z   (   $�� ��6?   $��1��6K   $��1��6X   $��2��6�   {�6h   �� $  
chkbxVoice��$! CheckBox.IsChecked�6�   �� 5{   "   -   5        $��cbAgentLoginID��5{   +   $��23��6A   $��1��6M   $��1��6Z   $y�Hidden=�6j   $" 0��6�   $��2��6�   $	��0,1q�6�   ��6~   �� *AgentLoginIds+6�   �� 5|   "   -   5        $��lblAgentPassword��5|   (   $��Center=�6@    
 KeyAgentPassword#��� 
 6b   $��2��6�   $��0��6�   {�6�   �� $  
chkbxVoice��$! CheckBox.IsChecked�6�   G� 5}   "   -   5        $��txtAgentPassword��5}   .   $��2��6F   $��1��6S   $��23��6c   $��Stretch=�6o   $��2��6�     controltempPasswordbox#��[ 6�   {�6�   �� $  
chkbxVoice��$! CheckBox.IsChecked�6  �� 5~   "   -   5        $��lblWrapupTime��5~   (   $��Center=�6=     KeyWrapTime#���  6_   $��3��6�   $��0��6�   {�6�   �� $  
chkbxVoice��$! CheckBox.IsChecked�6�   �� 5   "   -   5        $��txtWrapupTime��5   *   $��3��6?   $��1��6L   $��23��6\   $��Stretch=�6h   $��2��6�     controltempTextbox#��[ 6�   {�6�   �� $  
chkbxVoice��$! CheckBox.IsChecked���6  �� *
WrapTime+6Z  5�      5�      �� 5�      -   5        $��
chkbxEmail��5�   #   $��5��65   $��email��6B   $��14,5,0,5q�6R   $y�Hidden=�6�    6d   �� *IsEmailChecked+6�   �� 5�      -   5        $��	chkbxChat��5�   #   $��6��64   $
��chat��6A   $��14,5,0,5q�6P   $y�Hidden=�6�    6b   �� *IsChatChecked+6�   �� 5�      -   5        $��chkbxOutbound��5�   #   $��7��68   $��outboundpreview��6E   $��8,0,0,0q�6_   $��14,5,0,5q�6q   $y�Hidden=�6�    6�   �� *IsOutboundChecked+6  5�      5�      �� 5�      ���#656d90$��0.5,0.5,0.5,0.5q�6/   $��5,0,5,0q�6Q   $��2��6b   ._�5�      � $��30��6   .+�5�      �� -   5        $��	btnCancel��5�   -   $��23��6!   $��70��6>   $��Right=�6I   $��0,0,10,0q�6e   $��Center=�6w     NormalButton#��[ 5�   %   6K   �� 5�      -    5        $��btnOk��5�   [   $��23��6!   $��Right=�6-   $��0,0,90,0q�6I   $��Center=�6h   $��70��6�   #��[ 5�   %   6K   5�      5�      �� 5�      $��30��6   ���#007edf$��Stretch=�6:   $	��Top=�6X   $��0,3,0,0q�6p   $��4��6�   ._�5�      � .+�5�      �� $��© 2013-2017 Pointel Inc��6$   $
��Left=�6C   $��Center=�6^   $��Calibri)�6y   $��12(�6�   
�������$��5,0,0,0q�6�   �� 5�      $��Right=�6H   $��Center=�6d   ���#EA3737% �� CornerRadius$% 	10,0,0,10��6�   y�6!   �� *TrialVisibility+._�5�      �� $��Calibri)�6F   $��Center=�6[   $��Center=�6x   $��12(�6�   
�������$��	15,2,10,2q�6�   ��6(   �� *TrailMessage+6�   5�      5�      5�      5�      5�      5�      5�      