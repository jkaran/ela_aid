   M S B A M L   `   `   `  ����S  OPointel.TaskbarNotifier, Version=5.0.1.50, Culture=neutral, PublicKeyToken=nullbFclr-namespace:Pointel.TaskbarNotifier;assembly=Pointel.TaskbarNotifierPointel.TaskbarNotifier  5   	    Agent.Interaction.Desktop(   "Agent.Interaction.Desktop.Notifier   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   [NotifierFclr-namespace:Pointel.TaskbarNotifier;assembly=Pointel.TaskbarNotifier     5   	   6my-http://schemas.microsoft.com/wpf/2008/toolkit  5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   -   5          � AllowsTransparency  .  � WindowStyle$
 None=�5   Z   
������  � Title$ Notifier��5   	   $
��Auto��6   $	��400��6(    � SizeToContent$ Height=�64    � 	Resources 5       �� MergedDictionaries �� Source�� 5	   
    5
      �� 5      $R L/Agent.Interaction.Desktop;component/Resources/StylesResourceDictionary.xaml?�5      %    5      .��5      � .+�5   
   �� -   5        $��
MainBorder��5   F   $
��0.89q�6   $��8q�6(   
�������._�5      �  � RowDefinitions 5      �� 5      $
��Auto �6$   64   �� 5      6&   �� 5      $
��Auto �6$   64   .+�5      �� $��0��6   $��14(�6%   $��Calibri)�63   ���#007edf
���������6p   �� *	TitleText+6�   9� 5      -   5        $��1��5         SimpleExpanderTemp#��[  6(    9� 
IsExpanded . $��RightToLeft=�6i   $	��100��6�   ��5      �� 5      $��	Call Data��6    $��Calibri)�64   ���#0878ef$��Right=�6^   $	��355��6�   .��5      � -   5        $��expGrid��5      $
��Auto��6*   ���#1d4380$��5,0,5,0q�6M   .+�5      )  #Microsoft.Windows.Controls.DataGrid  -   5        $��
DGCallData��5   &   	   RowHeaderWidth$	 0Q�68   $��LeftToRight=�6v   
�������$	��Top=�6�   $��Stretch=�6�   
   HorizontalGridLinesBrush
 �#D6D7D6   VerticalGridLinesBrush �#D6D7D6   AutoGenerateColumns .     CanUserAddRows .     
IsReadOnly .    CanUserDeleteRows .  $
��Auto=�5   �   ��5   K   �� *NotifyCallDataView+   Columns 5       3  -Microsoft.Windows.Controls.DataGridTextColumn  5!   "   /  )Microsoft.Windows.Controls.DataGridColumn   Header$	 Key��68      Width8  2Microsoft.Windows.Controls.DataGridLengthConverter$ 85 6]      
IsReadOnly . 4  .Microsoft.Windows.Controls.DataGridBoundColumn   Binding 6E   �� *Key+6h     5"   "   $ Value��68   $ * 6a    .  6G   �� *Value+6k   5$      5%      5&      �� 5'      -   5        $��btnRight��5'      $��0,5,5,5q�6)   $��2��6:   $��Right=�6G   $��70��6c   $��30��6n   5(      �� 5)      -   5        $��btnLeft��5)      $��0,5,80,5q�6(   $��2��6:   $��Right=�6G   $��70��6c   $��30��6n     
CallButton#��[ 6z   6�   5*      5+      5,      5-      