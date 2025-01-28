Imports System
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Controls.Primitives
Imports System.Windows.Documents
Imports System.Windows.Navigation
Imports System.Windows.Shapes
Imports System.Windows.Data
Imports System.Windows.Media


Namespace Thumb_wcp_vb

    '@ <summary>
    '@ Interaction logic for Pane1_xaml.xaml
    '@ </summary>
    Partial Class Pane1
        '<Snippet3>
        Sub ShowPosition(ByVal sender As Object, ByVal e As DragDeltaEventArgs)
            Dim yadjust As Double
            Dim xadjust As Double
            'Move the Thumb to the mouse position during the drag operation
            If ((xadjust >= 0) And (yadjust >= 0)) Then
                yadjust = myCanvasStretch.Height + e.VerticalChange
                xadjust = myCanvasStretch.Width + e.HorizontalChange
                myCanvasStretch.Width = xadjust
                myCanvasStretch.Height = yadjust
                Canvas.SetLeft(myThumb, Canvas.GetLeft(myThumb) + _
                                            e.HorizontalChange)
                Canvas.SetTop(myThumb, (Canvas.GetTop(myThumb) + _
                                            e.VerticalChange))
                changes.Text = "Size: " & _
                                    myCanvasStretch.Width.ToString() & _
                                     ", " & _
                                    myCanvasStretch.Height.ToString()
            End If

        End Sub
        '</Snippet3>
        '<SnippetDragStartedHandler>
        Sub onDragStarted(ByVal sender As Object, ByVal e As DragStartedEventArgs)
            myThumb.Background = Brushes.Orange
        End Sub
        '</SnippetDragStartedHandler>

        '<SnippetDragCompletedHandler>
        Sub onDragCompleted(ByVal sender As Object, _
                          ByVal e As DragCompletedEventArgs)
            myThumb.Background = Brushes.Blue
        End Sub
        '</SnippetDragCompletedHandler>

    End Class

End Namespace