   M S B A M L   `   `   `  ����@  <Snoop, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:SnoopSnoop  5          Snoop.PropertyGrid2   5      \ XPresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    6x,http://schemas.microsoft.com/winfx/2006/xaml   5      myclr-namespace:Snoop   5      -   5          �Name$  PropertyGrid��5      $��0��5      $��0��6    � 	Resources 5	       �� MergedDictionaries �� Source   UnselectedValueTemplate  SelectedValueTemplate   Snoop.EditTextBox  PropertyMenu   Snoop.PropertyGrid2   DelveCommand m� Command m� CommandParameter   DelveBindingCommand$   DelveBindingExpressionCommand	   ClearCommand
 �� 
TargetType �� Property   SortCommand �� Value  BreakpointTemplate �� 
TargetType  NoBreakpoint �� Triggers P� Property P� Value  
Breakpoint �� 
TargetName�� 5
       5      �� 5      $ 
Icons.xaml?�6   %o  &	        &	 @     &	 �     '�� �    &	 D    7  7  �� 5      .R�5      �� ��6   �� *Value+5      �� 5      .R�5        $��15��6   $
��Left=�6>   $��1q�6Y   $��0q�6e   
�������$��Center=�6�   ��6!   �� *StringValue+6�   5      �� 5      .@�5      m� $��Delve��6   # Z 6    6Q   �� m� 5      $��Delve Binding��6   # Z 6&    6`   �� m� 5      $��Delve BindingExpression��6   # Z 60    6t   �� m� 5      $��Clear/Reset��6   # Z	 6$    6W   �� 5      �� 5      "
 ��6   .�5      ��  � ��# Z 6    5       �� 5"      " ��60   .R�5#      � .+�5$      �� $
��Icon��6   8��  $��12��6@   $��12��6K   5%       5'      P� 5(       �@d�	IsChecked$
 TrueY�6$   . �5*      �� 8  $
 Icon��6E    � ��5+      5-      5/       � RowDefinitions 53      �� 54      $��22 �6   �� 55      $��20 �6   �� 56      $��* �6   .+�59      � $��1��6   .+�5:      �� -   5         ��Name$ PropertiesFilter��5:   �   $��0,0,42,2q�6n   $��0��6�   $��0��6�   $��0q�6�   ��6   ��  �� ElementName$ PropertyGrid�� �� Path$ StringFilter� �� UpdateSourceTrigger$ PropertyChanged=�6�   d� 5;      $��18��6S   $��18��6^   $��Right=�6j   $��0,0,22,2q�6�    d� 	IsChecked 6   �� $ PropertyGrid��$ ShowDefaults�6�   �� 5<         PopTargetCommand#��Z 6   $��0,0,0,2q�6B   $��18��6S   $��18��6^   $��0��6j   $��0��6w   $��Right=�6�   .��5=   5   ^5>      5?      �� 5@      $��2q�6   ��6   �� $ PropertyGrid��$
 Type��� 5B      -   5         ��Name$ ListView��5B   '   $��0��6   $��0��6   $��1��69   $��2��6I   $ �� IsSynchronizedWithCurrentItem$
 TrueY�6V   $��Stretch=�5D      $��2��5F      ��5C      �� $ PropertyGrid��$ 
Properties� �� SelectedItem 5E      �� $ PropertyGrid��$ 	Selection� �� Mode$ TwoWay=���5H      �� 5I      "
 ��6    �� Triggers 5J      �� 5K       �� Binding 6   �� *IsLocallySet+  �� Value	  False64   .a�5L      ��  � ��
 �����6%   5M      �� 5N       6   �� *IsExpression+  True64   .a�5O      ��  � ��
 ����6%   5P      �� 5Q       6   �� *
IsAnimated+  True62   .a�5R      ��  � ��
 �z���6%   5S      �� 5T       6   �� *IsInvalidBinding+  True68   .a�5U      ��  � ��
 �  ��6%   5V      �� 5W       6   �� *	IsVisible+	  False61   .a�5X      ��  � y�$ 	Collapsed=�6%   5Y      P� 5Z       �@��
IsSelected . . �5[      ��  � ��
 �����6%   5\      �� 5]       6   �� *HasChangedRecently+  True6:   .a�5^      ��  � ��
 � ���6%   5_      5a      ! �� View! 5c      �� 5d      .P�5e      �� " �� Width$" 25��6   # �� CellTemplate# 5f      �� 5g      .R�5h   	   �� $ � ToolTip$$ Set a debug breakpoint��66   #��[ 6W    6   �� *BreakOnChange+6W   5i   	   5k      �� 5l      $
��Name��6   $	" 200��6#   # 5m      �� 5n      .R�5o   	   �� -   5        $��15��5o   P   % � ContextMenu#% [ 6�   ��6   �� *DisplayName+$ 60   �� *DisplayName+6�   5p   	   5r      �� 5s      $��Value��6   $	" 143��6$   # 5t      �� 5u      .R�5v   	   �� $��ValueHolder��6   #��[  6K   $ 6+   �� *StringValue+��6�   �� & �� Triggers& 5w   	   ^� 5x   
   ' ^� 
Conditions' 5y      �� 5z      ( �� Binding( 6   �� *
IsSelected+) �� RelativeSource) �� * �� Mode$* FindAncestor=�+ �� AncestorType"+ ��, �� Value, True6�   �� 5{      ( 6   �� *	CanEdit+, True62   .0�5}      �� $ ValueHolder��6   # [ 6F    � ��5~      5�   	   5�      �� 5�      $��ValueSource��6   $	" 143��6*   # 5�      �� 5�      .R�5�   	   �� $��15��6`   ��6   �� *ValueSource.BaseValueSource+$ 6@   �� *ValueSource+6`   5�   	   5�      5�      5�      5�      