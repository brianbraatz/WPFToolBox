Imports System
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Media
Imports System.Windows.Navigation

Namespace Border_change_vb

    '@ <summary>
    '@ Interaction logic for Pane1_xaml.xaml
    '@ </summary>
    Partial Class Pane1
                ' <Snippet2>
                Sub ChangeBG(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs)
	            root.Background = System.Windows.Media.Brushes.LightSteelBlue	
	            btn.Content = "Clicked!"
                Text1.Text = "The background is now LightSteelBlue"
                End Sub
                ' </Snippet2>

    End Class

End Namespace