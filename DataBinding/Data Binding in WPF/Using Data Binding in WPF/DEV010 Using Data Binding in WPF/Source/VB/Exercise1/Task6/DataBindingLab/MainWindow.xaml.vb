Imports System.ComponentModel

Partial Public Class MainWindow
    Inherits Window

    Private listingDataView As CollectionViewSource

    Public Sub New()
        InitializeComponent()
        listingDataView = CType((Me.Resources("listingDataView")), CollectionViewSource)
    End Sub

    Private Sub ShowOnlyBargainsFilter(ByVal sender As Object, ByVal e As FilterEventArgs)
        Dim product As AuctionItem = CType(e.Item, AuctionItem)
        If Not (product Is Nothing) Then
            'Filter out products with price 25 or above
            If product.CurrentPrice < 25 Then
                e.Accepted = True
            Else
                e.Accepted = False
            End If
        End If
    End Sub

End Class
