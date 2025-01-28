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

    Private Sub AddGrouping(ByVal sender As Object, ByVal args As RoutedEventArgs)
        'This groups by property "Category"
        Dim groupDescription As PropertyGroupDescription = New PropertyGroupDescription
        groupDescription.PropertyName = "Category"
        listingDataView.GroupDescriptions.Add(groupDescription)
    End Sub

    Private Sub RemoveGrouping(ByVal sender As Object, ByVal args As RoutedEventArgs)
        listingDataView.GroupDescriptions.Clear()
    End Sub

End Class
