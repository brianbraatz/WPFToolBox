   M S B A M L   `   `   `  ����@  <Snoop, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:SnoopSnoop  5          Snoop.SnoopUI   5      \ XPresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    6x,http://schemas.microsoft.com/winfx/2006/xaml   5      Fmc;http://schemas.openxmlformats.org/markup-compatibility/2006   5      myclr-namespace:Snoop   5      Jd@http://schemas.microsoft.com/expression/interactivedesigner/2006   5
        � Title$  Snoop��5       � ShowInTaskbar .  $	��700��5
      $	��580��6   ���	#FFE0DFE3 � InheritanceBehavior$ SkipToThemeNext=�5      |�.   � DataContext 5	      ��  �� RelativeSource �� *Self+ � 	Resources 5       �� MergedDictionaries �� Source �� 
TargetType   RefreshButton	 �� Property
 �� Value ��Name J� SnapsToDevicePixels �� StrokeDashArray �� EndPoint �� 
StartPoint � Color � Offset �� 
TargetType ��Name �� CornerRadius �� Triggers P� Property P� Value �� 
TargetName#   Snoop.ObjectToStringConverter  	NiceNamer�� 5       5      �� 5      $" ProperTreeViewResources.xaml?�6   �� 5      $ 
Icons.xaml?�6   %�  &	        &	 �    �� 5      " ��6   .�5      �� 	 �@�FocusVisualStyle
 5      �� 5      .�5   	   �� 	 � ��
 5   
   �� 5      .R�5      �� $ 	Rectangle��6N   ���	#FF000000$��1��6)   $��3,3,3,3q�6=    . $	 1 2R�6|   5      5   
   5   	   5!      �� 5"      	 � ��
 5#      �� 5$      $ 0.5,1&�6   $ 0.5,0&�6-   .��5%   	   � $ 	#FF69E861��6   $ 0Q�6(   � 5&   	   $ 	#FFE3E5F0��6   $	 0.5Q�6(   � 5'   	   $ 	#FF1CC211��6   $ 1Q�6(   5(   	   5*      �� 5+      	 � ��
 �	#FF003C746$   �� 5,      	 � ��#
 � ��6#   �� 5-      	 � ��$
 Center=�63   �� 5.      	 � ��$
 Center=�61   �� 5/      	 � ��
 50      �� 51      " ��6   .R�52   	   � $
��Auto��6   $
��Auto��6   .+�53   
   �� $ border��6   ���	#FF696969$��1,1,1,1q�69   $ 4,4,4,4��6S   �� 54   
   $��2,2,2,2q�6   55   
    56   	   P� 57   
    �@J�IsKeyboardFocused . 6/   P� 58   
    �@d�	IsChecked$
 TrueY�64   P� 59   
    � {� .  . �5:      �� #
 � ��6(   	 � ��5;      P� 5<   
    �@J�IsMouseOver . . �5=      �� $ border��6(   	 � ��
 5>      �� 5?      $ 1,0.5&�6!   $ 0,0.5&�62   .��5@      � $ 	#FFCEFFCF��6   $ 0Q�6-   � 5A      $ 	#FFE1FFE3��6   $ 0.43888888888888888Q�6-   � 5B      $ 	#FFC4FFC8��6   $ 1Q�6-   5C      5E      5F      5H   	   5J      5K        5M      6    5O      .��5Q      �  � ColumnDefinitions 5S      �� 5T      $��1* �6   �� 5U      $��2* �6    � RowDefinitions 5W      �� 5X      $��24 �6   �� 5Y      $��5* �6   �� 5Z      $��1* �6   �� 5[      $��16 �6   .+�5^      �� -   5        $��
TreeFilter��5^       �� 
IsEditable .  � ToolTip$&  Filter tree view by name or type��6s   $��2,2,20,2q�6�    �� Text 61   ��  �� Path$ Filter� �� UpdateSourceTrigger$ PropertyChanged=�.@�5_      �� .��6   Clear Filter6    �� 5`      .��6   Visuals with binding Errors6/   �� 5a      6   5b      �� 5d         Snoop.SnoopUI    RefreshCommand#��Z  6   #��[  6:   $��16��6a   $��Right=�6l   $��0,4,2,4q�6�   .��5e      ��  
 Reload#��[ 6   5f         Snoop.ProperTreeView  5h      -   5        !  Name$
! Tree��5h      $��1��6$   $��2��61   $��2,0,2,0q�6B   $��Stretch=�6t   $��0��6�   $��0��6�   ��6S   �� *
Filtered+ 5i      " �� DataType   Snoop.VisualTreeItem# �� ItemsSource��%�   (��       5j      *, +)�� 6   ""  6   # 6E   �� *
Children+.R�5k      �� $��Stretch=�6"   
������ ��6   �� 6@   5l      5m      ��5o      �� 5p        ProperTreeViewItemStyle$ �� BasedOn#$ [ 6      Snoop.ProperTreeViewItem"  6?   .�5q      �� 	 �@Q�
IsSelected
 6$   �� *
IsSelected+% �� Mode$% TwoWay=��� 5r      	 �@Q�
IsExpanded
 6$   �� *
IsExpanded+$% TwoWay=�5s      5u      �� 5w      -   5        & ��Name$& GridSplitter��5w   L   $
��Left=�6   $��-2,0,0,0q�6,   $��0��6>   $��Stretch=�6b   $��4��6~   $
��Auto��6�   $��1��6�   $��1��6�   $��0��6�   $��3��6�   �� 5y      $��1��6   $��2��6   $��2,2,2,0q�60   .@�5z      �� $��
Properties��6   .��5{         Snoop.PropertyGrid2  -   5        '  Name$' PropertyGrid��5{      (   Target( 6-   �� $ CurrentSelection.Target�5|      �� 5}      $��Events��6   .��5~         Snoop.EventsView  )   Scope) 6   �� *Root.Target+5      5�      �� 5�      $��3��6   $��4��6   .@�5�      �� $��Right=�6   $��0,-3,0,0q�6+   $Y��STip: Hold down Ctrl-Shift and mouse over elements in the target app to select them.��6=   �� 5�      $��Right=�6   $��3,-3,0,0q�6+      Snoop.NoFocusHyperlink  5�      *   SelectFocusCommand+ �� Command#+ Z* 6   ��.  , �� CommandParameter, 6M   �� *CurrentFocus+.�6�   .�5�      �� ��6   �� *CurrentFocus+- �� 	Converter#- [ 5�      5�      5�      �� 5�      $��Stretch=�6   $	��Top=�6/   $��2,0,0,0q�6G   $��4��6X   $��1��6c   $��2��6s   	   Snoop.Previewer	  5�      -   5        . 	 Name$. PreviewArea��5�   j   $��1��6   $��2��6!   $��2,4,0,0q�6.   / 	  Target/ 6?   �� *CurrentSelection.Target+6j   5�      5�      