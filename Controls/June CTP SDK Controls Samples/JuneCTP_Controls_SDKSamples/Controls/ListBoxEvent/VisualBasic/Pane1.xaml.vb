Imports System
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Controls.Primitives
Imports System.Windows.Documents
Imports System.Windows.Navigation
Imports System.Windows.Shapes
Imports System.Windows.Data
Imports System.Windows.Media

Namespace ListBox_vb

     
   Partial Class Pane1
        '<Snippet5>
        Sub PrintText(ByVal sender As Object, ByVal e As SelectionChangedEventArgs)
            Dim lbsender As ListBox
            Dim li As ListBoxItem
            lbsender = CType(sender, ListBox)
            li = CType(lbsender.SelectedItem, ListBoxItem)
            tb.Text = "   You selected " & li.Content.ToString & "."
        End Sub
        '</Snippet5>
    End Class

End Namespace