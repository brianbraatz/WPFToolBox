' Interaction logic for Window1.xaml
Partial Public Class Window1
    Inherits System.Windows.Window

    Public Sub New()
        InitializeComponent()

    End Sub

    Private Function CreateToolTipForTabItem(ByVal brush As VisualBrush, ByVal strTitle As String) As ToolTip

        brush.Viewport = New Rect(New Size(300, 225))
        brush.Viewbox = New Rect(New Size(300, 225))

        Dim myRectangle As New Rectangle()
        myRectangle.Width = 300
        myRectangle.Height = 225
        myRectangle.RadiusY = 5
        myRectangle.RadiusX = 5
        myRectangle.StrokeThickness = 0.7
        myRectangle.Stroke = Brushes.Black
        myRectangle.Fill = brush

        Dim objGrid As New Grid
        objGrid.RowDefinitions.Add(New RowDefinition())
        objGrid.RowDefinitions.Add(New RowDefinition())

        Dim objBorder As New Border
        objBorder.CornerRadius = New CornerRadius(10)
        objBorder.BorderThickness = New Thickness(5)
        objBorder.Background = New SolidColorBrush(Colors.White)
        '
        'TODO you can changes this to a brush resource in your applications
        objBorder.BorderBrush = New SolidColorBrush(Colors.LightBlue)
        '
        objBorder.Child = myRectangle
        objBorder.SetValue(Grid.RowProperty, 1)
        objGrid.Children.Add(objBorder)

        Dim objTitleBorder As New Border
        objTitleBorder.CornerRadius = New CornerRadius(5)
        objTitleBorder.BorderThickness = New Thickness(1)
        '
        'TODO you can changes this to a brush resource in your applications
        objTitleBorder.Background = New SolidColorBrush(Colors.WhiteSmoke)
        '
        objTitleBorder.BorderBrush = System.Windows.Media.Brushes.Black
        objTitleBorder.Margin = New Thickness(0, 0, 0, 12)
        objTitleBorder.HorizontalAlignment = Windows.HorizontalAlignment.Center

        Dim objTB As New TextBlock
        objTB.HorizontalAlignment = Windows.HorizontalAlignment.Center
        objTB.Text = strTitle
        objTB.Margin = New Thickness(4, 2, 4, 2)

        objTitleBorder.Child = objTB
        objGrid.Children.Add(objTitleBorder)

        Dim objEffects As New Effects.OuterGlowBitmapEffect
        objEffects.GlowColor = Colors.Black
        objEffects.GlowSize = 5

        Dim objTitleEffects As New Effects.DropShadowBitmapEffect
        objTitleEffects.ShadowDepth = 3

        Dim tt As New ToolTip
        tt.Content = objGrid
        tt.SnapsToDevicePixels = True
        tt.BorderBrush = Brushes.Transparent
        tt.Background = Brushes.Transparent
        ToolTipService.SetHasDropShadow(tt, False)

        'TODO if you need to position your tooltip, do it here
        '
        ' uses these properties to adjust the placement of your tooltip
        'tt.Placement = Primitives.PlacementMode.Top
        'tt.HorizontalOffset = -120
        'tt.VerticalOffset = -5

        objBorder.BitmapEffect = objEffects
        objTitleBorder.BitmapEffect = objTitleEffects
        tt.Margin = New Thickness(15, 17, 15, 17)

        Return tt

    End Function

    Private Sub Window1_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        Me.ToolTip = CreateToolTipForTabItem(New VisualBrush(CType(Me.layoutRoot, UIElement)), "Vista Taskbar Style ToolTip For Window")
    End Sub

End Class
