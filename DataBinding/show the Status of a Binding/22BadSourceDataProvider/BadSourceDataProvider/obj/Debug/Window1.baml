   M S B A M L   `   `   `  ����W  SBadSourceDataProvider, Version=1.0.2862.18677, Culture=neutral, PublicKeyToken=null=#clr-namespace:BadSourceDataProviderBadSourceDataProvider  5      #    BadSourceDataProvider.Window1   5   	   \ XPresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      6x,http://schemas.microsoft.com/winfx/2006/xaml   5      1local#clr-namespace:BadSourceDataProvider   5      -   5        $	��win��5        � Title$  BadSourceDataProvider��5      $	��400��5      $	��312��5       � 	Resources 5
      7  1System.Windows.Data.XmlNamespaceMappingCollection   	namespace 
� Prefix
 
� Uri � Source � XPath  404Provider � XmlNamespaceManager��%�   &	        &	 l     7     5      
� 5      $ yweather��6   $- 'http://xml.weather.yahoo.com/ns/rss/1.0?�6+   5      � 5      $" http://www.beacosta.com/ffff?�6   $	 rss��6:   8   6Z   5      .��5      ]�  � DataContext 6   ��  �� Source# [ 	 �� XPath$	 channel��.+�5      �� $��Bottom=�6   .@�5      �� -   5        $��	statusBar��5      ��5      �� 5      
 �� ElementName$	
 win��6    �� Path$ StatusMessage�6!   5      5      �� 5      $��30q�6   
����  �$��2q�6*    �� Padding$ 10q�6>   ._�5      �  � ColumnDefinitions 5      �� 5      $	��100 �6   �� 5      $��* �6    � RowDefinitions 5!      �� 5"      $��30 �6   �� 5#      �� 5$      �� 5%      �� 5&      �� 5'      �� 5(      $
��Auto �6   .+�5*      �� -   5        $��title��5*   �   $��2��6H   $
��Bold"�6\   $��16(�6n   $��0��6|   $��0��6�   $��Center=�6�   
���""����6   �� $	 title�� �� NotifyOnTargetUpdated . 6�   �� 5+      $��1��6   $��0��6   $
��Bold"�6)   $��Right=�6;   .��6W   Date:6^   �� 5,      $��1��6   $��1��6   $
��Left=�6P   ��6)   �� $	 item/pubDate��6P   �� 5-      $��2��6   $��0��6   $
��Bold"�6)   $��Right=�6;   .��6W   
Condition:6c   �� 5.      $��2��6   $��1��6   $
��Left=�6a   ��6)   �� $#	 item/yweather:condition/@text��6a   �� 5/      $��3��6   $��0��6   $
��Bold"�6)   $��Right=�6;   .��6W   Temperature:6e   �� 50       �� Orientation$ 
Horizontal=�6   $��3��6*   $��1��67   .+�51      �� $
��Left=�6E   ��6   �� $#	 item/yweather:condition/@temp��6E   �� 52      $
��Left=�6C   ��6   �� $!	 yweather:units/@temperature��6C   53      �� 54      $��4��6   $��0��6   $
��Bold"�6)   $��Right=�6;   .��6W   	Pressure:6b   �� 55      $ 
Horizontal=�6   $��4��6*   $��1��67   .+�56      �� $
��Left=�6E   ��6   �� $#	 yweather:atmosphere/@pressure��6E   �� 57      $
��Left=�6@   ��6   �� $	 yweather:units/@pressure��6@   58      �� 59      $��5��6   $��0��6   $
��Bold"�6)   $��Right=�6;   .��6W   Wind Speed:6d   �� 5:      $ 
Horizontal=�6   $��5��6*   $��1��67   .+�5;      �� $
��Left=�6<   ��6   �� $	 yweather:wind/@speed��6<   �� 5<      $
��Left=�6=   ��6   �� $	 yweather:units/@speed��6=   5=      �� 5>      -   5        $��	fixButton��5>   �   $��0��6   $��2��6/   $��6��6C   $��Center=�6P   $y�Hidden=�6m   $��0,5,0,0q�6�   .��6�   Fix XML Source6�   �� 5?      -   5        $��refreshButton��5?      $��0��6   $��2��6-   $��6��6A   $��Center=�6N   $y�Hidden=�6k   $��0,5,0,0q�6�   .��6�   Refresh Data6�   5@      5A      5B      5C      