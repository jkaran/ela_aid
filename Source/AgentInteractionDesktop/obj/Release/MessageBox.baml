   M S B A M L   `   `   `  ���� p  lSystem.Windows.Controls.Input.Toolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35pTclr-namespace:System.Windows.Controls;assembly=System.Windows.Controls.Input.ToolkitSystem.Windows.Controls   Agent.Interaction.Desktop*   $Agent.Interaction.Desktop.MessageBox   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     cmyTclr-namespace:System.Windows.Controls;assembly=System.Windows.Controls.Input.Toolkit     -     � AllowsTransparency  .  � WindowStyle$
 None=� � 
ResizeMode$ NoResize=� � WindowStartupLocation$ CenterScreen=� � ShowInTaskbar .   � Topmost . 
������ $
��Auto��$	��370�� � SizeToContent$ Height=� � Icon$I C/Agent.Interaction.Desktop;component/Images/Pointel%20Softphone.png�� � 	Resources 	 �� MergedDictionaries
 �� Source�� 	 �� $R
 L/Agent.Interaction.Desktop;component/Resources/StylesResourceDictionary.xaml?�%    .���� -   $��
MainBorder��$
��0.89q�$��8q�
�������._��  � RowDefinitions �� $
��Auto ��� $
��Auto ��� $
��Auto �.+��� -   $��lblTitle��$��0��$��14(�$��Stretch=�$��Calibri)����#007edf
��������� $��1��$��2�����#656d90$	��0.5q�$��5q�� $��1��$��50��$��10q� �� $��* ��� -    �Name$ growFwd��$��0 �.+��� -   $��txtblockContent��$��0��$
��Wrap=�$��Calibri)�$��12(�
���   �$��8q��� $��1�� �� Orientation$ 
Horizontal=�.+��� -   $��
lblForward��$��50,0,0,0q�$��Forward��-   'System.Windows.Controls.AutoCompleteBox  -   $��
txtForward��$��5,0,0,0q����#ADAAAD
�������
���   �$��Calibri)�$��12(�$��Stretch=�$	��200��   Text �� *	ForwardDN+   SelectedItem �� *	ForwardDN+ �� Mode$ TwoWay=�   ItemsSource �� *
ForwardDns+   ItemTemplate �� .R��� $��innertxt��
���   �$	��172��$��2q�$��Calibri)�$��12(�����   �� 
TargetType �� Property �� Value �� Triggers P� Property P� Value   HorizontalScrollBar  VerticalScrollBar��%z  '�       '�� #     7   7  �� " �.���  � ��$ 70���� " ��.���  �@J�SnapsToDevicePixels . ��  �@�OverridesDefaultStyle .  P�  �@��Orientation$ 
Horizontal=�. ���  � ��$
 Auto����  � ��$ 18����  � ��8   P�  �@��Orientation$ Vertical=�. ���  � ��$ 18����  � ��$
 Auto����  � ��8  �� -   $��ForwardError��$��2��
���  ��$��Calibri)�$	��Top=�$��10,5,0,0q�$
��Left=��� -	   $��	btn_right��$��	0,0,10,10q�$��2��$��Right=�$��23��$��80��  NormalButton#��[ �� -
   $��btn_left��$��
0,0,100,10q�$��2��$��Right=�$��23��$��80��#��[ 