   M S B A M L   `   `   `  ����   Agent.Interaction.Desktop/    )Agent.Interaction.Desktop.DispositionForm   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     -     � Title$  DispositionForm��$	��500��$
��Auto�� � WindowStyle$
 None=� � Icon$F @/Agent.Interaction.Desktop;component/Images/pointelicon16x16.ico�� � AllowsTransparency . 
������  � WindowStartupLocation$ CenterOwner=� � SizeToContent$ Height=� � ShowInTaskbar .   � 
ResizeMode$ NoResize=� � 	Resources 	 �� MergedDictionaries
 �� Source �� 
TargetType   	baseStyle �� Property �� Value �� BasedOn�� 	 �� $R
 L/Agent.Interaction.Desktop;component/Resources/StylesResourceDictionary.xaml?�%J   &	        '�� #     7   �� " ��.���  � ��$ 12(��� " ��8   .���  � RowDefinitions �� $
��Auto �.+��� -   $��
MainBorder��$��0��$
��0.89q�$��8q�
�������._���  �� Orientation$ Vertical=�.+��� -   $��25�����#007edf$��0q�$��Stretch=�$��Center=�._��� -   $��lblTitleStatus�� �� Padding$ 4,0,4,0q�$��Calibri)�$��15(�
�������$
��Bold"�$��Center=�$��Disposition Form��$
��Left=�� $��
10,15,10,5q� �� �� �� �� �� �� �� �� ��  � ColumnDefinitions �� �� .+��� $	��MID��$��5,0,0,0q�$��Bottom=�$��12(��� -   $��txtMID��$��Stretch=�$��25��$��1��$	��5,0q� �� 	MaxLength$ 20�� �� 	UndoLimit$ 10��  DefaultCopyContextmenu � ContextMenu# [   controltempTextbox1#��[ �� $ 
Horizontal=�$��2��$��0,3,0,0q�.+��� $��Caller Name��$��5,0,0,0q�$��Bottom=�$��12(��� $��*��$��5,0,5,0q�$��Right=�
���  ��$��12(��� -   $��txtCallerName��$��Stretch=�$��25��$��3��$��5q�$	 100��$ 10��# [ #��[ �� $��Phone��$��4��$��5,3,5,0q�$��Bottom=�$��12(��� -   $��txtPhone��$��Stretch=�$��5��$��5q�$ 20��$��25��$ 10��# [ #��[ �� $ 
Horizontal=�$��1��.+��� $	��LOB��$��5,0,0,0q�$��Bottom=�$��12(��� $��*��$��5,0,5,0q�$��Right=�
���  ��$��12(��� -   $��cbLOB�����#9CAAC1$��Stretch=�$��1��$��1��$��5q�$��25������ *LOB+�� $ 
Horizontal=�$��2��$��1��$��0,3,0,0q�.+��� $��Termination Code��$��5,0,0,0q�$��Bottom=�$��12(��� $��*��$��5,0,5,0q�$��Right=�
���  ��$��12(��� -	   $��cbTerminationCode�����#9CAAC1$��Stretch=�$��3��$��1��$��5q�$��25������ *LOB+�� $
��AWD#��$��4��$��1��$��5,3,5,0q�$��Bottom=�$��12(��� -
   $��txtAWD��$��Stretch=�$��5��$��1��$ 10��$ 20��# [ #��[ $��5q��� $��Notes��$��6��$��2��$��5,3,5,0q�$��Bottom=�$��12(��� -   $��txtNotes��$��2��$��7��$��50��$	 255��$��Stretch=� �� TextWrapping$ WrapWithOverflow=�$ 10��# [ #��[ $��5q�� $��2��$��8�� �� $��* ��� $
��Auto �.+��� -   $��txtError��$��Stretch=�
���  ��$��5q�$��Center=�$��WrapWithOverflow=��� -   $��btnSave��$��5q�$
��Save��$��Right=�$��23��$��1��$��80��  NormalButton#��[ 