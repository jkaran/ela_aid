   M S B A M L   `   `   `  ����   Agent.Interaction.Desktop0    *Agent.Interaction.Desktop.MyMessageSummary   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     -   $��MessageSummary��  � AllowsTransparency  .  � WindowStyle$
 None=� � 
ResizeMode$ NoResize=� � WindowStartupLocation$ CenterScreen=� � ShowInTaskbar .   � Topmost . 
������ $
��Auto��$	��400�� � SizeToContent$ Height=� � 	Resources  �� MergedDictionaries	 �� Source   BlinkBorder
 �� 
TargetName �� TargetProperty W� From	 W� To g� RepeatBehavior g� AutoReverse g� Duration��  �� $R	 L/Agent.Interaction.Desktop;component/Resources/StylesResourceDictionary.xaml?�%i   &	        �� -   .��W� $
 
MainBorder��$ Opacity�$ 1X�$ 0X�$ 2x�� . $ 0:0:0.1D�.���� -   $��
MainBorder��$
��0.89q�$��8q�
�������._��  � RowDefinitions �� $
��Auto ��� �� $
��Auto �.+��� -   $��MessageType��$��0��$��14(�$��Stretch=�$��Calibri)����#007edf
��������� -   $��BroadCastBackgroundBrush��$��1��$��2q�._�� -   $��grd_Details��$��1��
�������$��1,10,1,1q� J� Opacity$	 0.9Q� � ColumnDefinitions �� $��70 ��� $	��275��.+��� -   $��MessageIconImageSource��$��Center=�$��Center=�$��50��$��50��� -   $��Details��$��1��$��5,0,0,0q�$��Center=�$��1�� �� $��80 ���  �� �� �� �� �� .+��� -	   $��	Message11��$��12(�$��Calibri)�$
��Bold"�$��0��$��0���� -
   $��
Message111��$��12(�$��Calibri)�$
��Wrap=�$��0��$��1���� -   $��	Message22��$��12(�$��Calibri)�$
��Bold"�$��1��$��0���� -   $��
Message222��$��12(�$��Calibri)�$��1��$��1���� -   $��	Message33��$��12(�$��Calibri)�$
��Bold"�$��2��$��0���� -   $��
Message333��$��12(�$��Calibri)�$��2��$��1���� -   $��	Message44��$��12(�$��Calibri)�$
��Bold"�$��3��$��0���� -   $��
Message444��$��12(�$��Calibri)�$��3��$��1���� -   $��	Message55��$��12(�$��Calibri)�$
��Bold"�$��4��$��0���� -   $��
Message555��$��12(�$��Calibri)�$��4��$��1���� ���#656d90$	��0.5q�$��2��$��	2,0.5,2,2q�._��  �� ���� *MsgRowHeight+�� $
��Auto �.+��� $��0��$	��200��$��5q�$
��Auto=�  �� 
TargetType �� Property �� Value �� Triggers P� Property P� Value  HorizontalScrollBar  VerticalScrollBar��%M  '��       7  7  �� " ��.���  �@J�SnapsToDevicePixels . ��  �@�OverridesDefaultStyle .  P�  �@��Orientation$ 
Horizontal=�. ���  � ��$
 Auto����  � ��$ 18����  � ��8   P�  �@��Orientation$ Vertical=�. ���  � ��$ 18����  � ��$
 Auto����  � ��8  .���� -   $��MessageBodyMsg��$��5q�$��12(�$��Calibri)�$
��Wrap=�$
��Auto��$
��Auto���� -   $��	btn_right��$��1��$��5,5,5,10q�$��Right=�$��23��$��80��  NormalButton#��[ $��Ok��