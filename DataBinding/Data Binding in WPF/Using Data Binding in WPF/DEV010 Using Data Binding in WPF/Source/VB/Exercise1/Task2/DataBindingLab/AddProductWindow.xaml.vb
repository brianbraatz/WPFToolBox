Partial Public Class AddProductWindow
    Inherits Window

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub SubmitProduct(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Me.Close()
    End Sub
End Class
