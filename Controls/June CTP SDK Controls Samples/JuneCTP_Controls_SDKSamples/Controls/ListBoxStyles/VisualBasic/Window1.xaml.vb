Imports System
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Controls.Primitives
Imports System.Windows.Documents
Imports System.Windows.Navigation
Imports System.Windows.Shapes
Imports System.Windows.Data
Imports System.Collections.ObjectModel
Imports System.Collections.Generic


Namespace SDKSample

    '@ <summary>
    '@ Interaction logic for Window1_xaml.xaml
    '@ </summary>
    Partial Class Window1

        Private Sub PrintText(ByVal Sender As Object, ByVal e As SelectionChangedEventArgs)

            Dim li As ListBoxItem

            li = (CType(Sender, ListBox).SelectedItem)

            tb.Text = "   You selected " + li.Content.ToString() + "."

        End Sub
    End Class

End Namespace

