   M S B A M L   `   `   `  ����@  <Snoop, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:SnoopSnoop  5          Snoop.EventsView   5      \ XPresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    6x,http://schemas.microsoft.com/winfx/2006/xaml   5      myclr-namespace:Snoop   5      Jd@http://schemas.microsoft.com/expression/interactivedesigner/2006   5      Fmc;http://schemas.openxmlformats.org/markup-compatibility/2006   6h   -   5          �Name$  Events��5      $��0��5      $��0��6    � 	Resources 5	       �� MergedDictionaries �� Source#   Snoop.ObjectToStringConverter   	NiceNamer  CategoryTemplate �� Path �� DataType$  System.Windows.RoutedEventArgs2  ,System.Windows.Input.ExecutedRoutedEventArgs �� Orientation �� 	Converter   Snoop.EventTracker d� 	IsChecked   Snoop.TrackedEvent	 �� ItemsSource
 � ToolTip � ColumnDefinitions � RowDefinitions �� Triggers �� Value �� Binding �� 
TargetName �� Property �� Value   Snoop.EventEntry  NoSelectionComboBox �� 
TargetType ��Name � IsSharedSizeScope z� SharedSizeGroup ��Name d�Name �� Mode �� RelativeSource �� Mode �� AncestorType �� 
TargetType �� CornerRadius �� 
StartPoint  �� EndPoint! �� RelativeTransform" T� X# T� Y$ �� ScaleX% �� ScaleY& �� AngleX' �� AngleY( �� Angle) � Offset* � Color+ ��Name, F�Name- �� StrokeEndLineCap. �� StrokeStartLineCap/ �� StrokeLineJoin0 �� Left
1 �� Top2 J� RenderTransformOrigin3 �� Stretch4 �� Triggers5 ]� 
Conditions6 �� Property7 �� Value8 P� Property9 P� Value: �Name; � AllowsTransparency< � ActualWidth= �� MaxDropDownHeight> ��Name-  'System.Windows.Input.KeyboardNavigation?   DirectionalNavigation@ �� 
GroupStyle(  "System.Windows.Controls.GroupStyleA   HeaderTemplateB P� 
SourceNameC F� ShadowDepthD F� Softness�� 5
       5      �� 5      $" ProperTreeViewResources.xaml?�6   %  &	        &	      (�� |     5      *, +)(�� �     5      *, +)7   (�� R    5      *, +)(�� �    5$   <   *, +)7   7   (�� 7    5E      *, +)7   &	 �    7    5      6   �� 5      .R�5      �� $
��Bold"�6*   $��2,5,0,0q�6<   ��6   �� $
 Name�6<   5      �� 5      "  6   .R�5      �� ��6   �� *RoutedEvent.Name+5      �� 5      "  6   .R�5      �� $ 
Horizontal=�6   .+�5      �� ��6   �� *	Command+8   5      5      �� 5      "  6   .R�5       ��  6   �� *	IsEnabled+��6.   �� *RoutedEvent.Name+5!      �� 5$      "  6<   	 6   �� *Stack+.R�5%      � $
��Grid��6   -   5        
������ $:
 4Double-click to select the element that handled this��5%   0    5)      �� 5*      $��* �6   �� 5+      $��* �6    5-      �� 5.      $��* �6   �� 5/      $��* �6   .+�51      �� $ 
Horizontal=�6   .+�52      �� ��6   �� *	EventArgs+�� 54      $
�� on ��6   �� 55      $��1��6\   ��6   �� *Originator.Handler+8   6\   56      �� 57      $��Handler��6   $y�	Collapsed=�6%   $��1��6<   $��2��6I   .��58      �� $ 
Horizontal=�6   .+�59   	   �� Handled By:6!   �� 5:   	   ��6   �� *	HandledBy+8  5;   	   5<      5=       5>      �� 5?      $
 True��6/    6   �� *	Handled+.a�5@      �� $ Handler��6    � y�$ Visible=�6:   5A      5C      �� 5E      "  6   .R�5F      � $
��Grid��6   -   5        
������ $:
 4Double-click to select the element that handled this��5F   0   .+�5G      �� $ 
Horizontal=�6   .+�5H      �� ��6   �� *	Handler+8   �� 5I      $��
 handled: ��6   �� 5J      $��1��6+   ��6   �� *	Handled+6+   5K      5L      5M      �� 5O      " ��61   .R�5P      � $
  Grid��6   .+�5Q      �� $ Border��6$   #��z��64   #��z��6`   ��6   Z� ._�5R      � $  Grid1��6+    .  5S   	   �� 5T   
   $��1 �6   �� 5U   
   $��* �6   �� 5V   
   $
��Auto �6   $ ComboBoxButton��6(   .+�5X   	   �� $ 	TextBlock��6�   $>��83,4.36166666666667,3.41060513164848E-13,2.36166666666664q�6   $��13.2766666666667��6U   $��1��6o   $��2��6   $��Common events��6�   d� 5Y   	   $ ToggleButton��6*   $��3��6    6@   �� *IsDropDownOpen+$ TwoWay=� �� $ TemplatedParent=� Z� ��5Z   
   �� 5[      " d�6   .�5\      ��  � ��$ 0��6'   �� 5]       � ��$ 0��6(   �� 5^       � ��$
 Auto��6$   �� 5_       � ��$
 Auto��6%   �� 5`       � �� �	#00FFFFFF6)   �� 5a       � |� .  6(   �� 5b       �@��	ClickMode$ Press=�6(   �� 5c       � �� 5d      �� 5e      " d�6   .R�5f      � $
  Grid��6>   #��z��6    5g      �� 5h      $��* �6"   �� 5i      $
��Auto �6"   $ ComboBoxButton��6/   .+�5k      �� $ Border��6l   $
��Left=�6   $��2,2,2,2q�62   $��14.6791723969389��6C   $��1��6\   $��1,1,1,1q�6|   $ 3,3,3,3��6�   ��5l      �� 5m      $ 0.5,0&�6&   $  0.5,1&�69   ! 5n      V� 5o      .~�5p      T� $
" -0.5Q�6(   $
# -0.5Q�61   �� 5q      $$ 1.2938731180987744Q�6$   $% 1.2938731180987744Q�6@   �� 5r      $& 0Q�6#   $' 0Q�6.   �� 5s      $( 137.35598639732007Q�6%   T� 5t      $	" 0.5Q�6(   $	# 0.5Q�60   T� 5u      $" -0.052250302263790425Q�6(   $# 0.061724038623558443Q�6B   5v      .��5x      � $) 0.41025641025641024Q�6    $4* .sc#1, 0.0884037539, 0.0884037539, 0.0884037539��6=   � 5y      $) 1Q�6    $+* %sc#1, 0.3278404, 0.3278404, 0.3278404��6+   � 5z      $) 0Q�6    $1* +sc#1, 0.398262918, 0.398262918, 0.398262918��6+   5{      ._�5}      �� $+ Canvas��6/   $��16��6   $��16��6#   .+�5~      F� $
, Path��6?  2���+sc#1, 0.102488264, 0.102488264, 0.102488264$��2��6p   $- Round=�6�   $. Round=�6�   $/ Round=�6�   $��10.0079642791082��6�   $��7.69193004565297��6�   $0 1.41917012579142��6  $1 3.53853164614431��6!  $2 0.5,0.5&�6M  $
3 Fill=�6m  6���4��&��?�Uvn@����k�@��� Q��� �q��cI@	��6`   Z� z�5      V� 5�      .~�5�      T� $" 0Q�6(   $# 0Q�6.   �� 5�      $$ 1Q�6$   $% 1Q�6/   �� 5�      $& 0Q�6#   $' 0Q�6.   �� 5�      $( 0Q�6%   T� 5�      $" 0Q�6(   $# 0Q�6.   T� 5�      $" 0Q�6(   $# 0Q�6.   5�      5�      5�      5�      5�      4 5�      ]� 5�      5 5�      �� 5�      6 �@J�IsMouseOver7 . 63   �� 5�      6 �@d�	IsChecked$7 FalseY�61   5�      P� 5�      8 � ��9 . 5�      P� 5�      8 � {�9 .  5�      5�      5�      5�      5�   
   5�   	   5�      � 5�      $: 
PART_Popup��6�   |�.  $��Bottom=�6�   $��Slide=�6�   ; . ��6   �� *IsDropDownOpen+ �� $ TemplatedParent=� Z� .��5�      � $  DropDown��6f   #��z< 6   #��z= 66   .+�5�   	   �� $ DropDownBorder��6:   #��z��6   #��� ��6R   $��1,1,1,1q�6�   �� 5�   	   $> ScrollViewer��6'   $��1,1,1,1q�6   .��5�   
   �� $? 	Contained=�6l   ��6   �� *ItemsSource+ �� *TemplatedParent+@ 5�        5�      8A   6   5�      5�   
   5�   	   5�      5�      4 5�      ]� 5�      5 5�      �� 5�   	   6 �@J�IsKeyboardFocusWithin7 . 64   �� 5�   	   6 �@��IsDropDownOpen7 .  6-   ./�5�      �� # � ��6%    � ��5�      P� 5�      $B 
PART_Popup��6.   8 �@�HasDropShadow9 . . �5�      �� $ DropDown��61    � ��$ 0,0,3,3q�6!   61   �� 5�      $ DropDown��6;    �@J�SnapsToDevicePixels . �� 5�      $ DropDownBorder��6'    �@J�BitmapEffect 5�   	   F� 5�   
   $	C 1.5Q�6!   $
D 0.15Q�63   5�   	   5�      P� 5�      8 �@��HasItems9 .  . �5�      �� $ DropDownBorder��6/    � ��$ 95��6$   6/   5�      P� 5�      8 � {�9 .  . �5�      �� # � ��6%    � ���� 5�      # � ��6%    � ��5�      P� 5�      8 �@��
IsGrouping9 . . �5�      ��  � �� .  68   5�      P� 5�      8 � ��$
9 Auto��6    . �5�      ��  � ��$	 150��6#   5�      P� 5�      8 � ��$
9 Auto��6!   . �5�      ��  � ��$ 20��6$   5�      5�      5�       5�      �� 5�      $��22 �6   �� 5�      $��* �6   .+�5     �� -   5        E ��Name$E EventChooser��5  F   $��Stretch=�6   $��Stretch=�6*   #��[ 6\   $F �� IsSynchronizedWithCurrentItem$
F TrueY�6�   $��0,0,20,0q�6�   ��6�   �� G �� ElementName$G Events��$ AvailableEvents�6�   �� 5     $��X��6
   $��18��6   $��18��6!   $��0,-2,0,0q�6-   	   Snoop.EventsViewH 	  ClearCommand#��ZH 6@   $��Right=�6p   $
 Clear��6�   
   Snoop.ProperTreeView
  5     -   5        I 
 Name$I 	EventTree��5     $��1��6(   $��Stretch=�6x   ��65   �� $G Events��$ InterestingEvents���5     �� 5       ProperTreeViewItemStyleJ �� BasedOn#J [ 6      Snoop.ProperTreeViewItem"  6>   .�5     ��  � ��$ Stretch=�63   �� 5	      � ��$ Stretch=�6,   K �� TriggersK 5
     �� 5      6   �� *	Handled+ True6/   .a�5     ��  � ��
 ����6%   5     P� 5     8 �@Q�
IsSelected9 . . �5     ��  � ��
 �����6%   5     5     5     5     