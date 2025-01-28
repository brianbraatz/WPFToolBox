'This is a list of commonly used namespaces for a pane.
Imports System
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Documents
Imports System.Windows.Navigation
Imports System.Windows.Shapes
Imports System.Windows.Data
Imports System.Windows.Media

	'@ <summary>
	'@ Interaction logic for Pane1.xaml
	'@ </summary>

	
        public partial class Pane1 
    Inherits StackPanel

	        
                Dim str as  string 
                Dim ffamily as  FontFamily 
                Dim fsize as  Double 

		' To use PaneLoaded put Loaded="PaneLoaded" in root element of .xaml file.
		' Sub PaneLoaded(object sender, EventArgs e) {}
		' Sample event handler:  
    Sub ChangeBackground(ByVal Sender As Object, ByVal e As RoutedEventArgs)

        '<Snippet1>
        If (Equals(btn.Background, Brushes.Red)) Then

            btn.Background = New LinearGradientBrush(Colors.LightBlue, Colors.SlateBlue, 90)
            btn.Content = "Background"

        Else

            btn.Background = Brushes.Red
            btn.Content = "Control background changes from red to a blue gradient."
        End If
        '</Snippet1>
    End Sub
    Sub ChangeForeground(ByVal Sender As Object, ByVal e As RoutedEventArgs)

        '<Snippet2>
        If (Equals(btn1.Foreground, Brushes.Green)) Then

            btn1.Foreground = Brushes.Black
            btn1.Content = "Foreground"

        Else

            btn1.Foreground = Brushes.Green
            btn1.Content = "Control foreground(text) changes from black to green."
        End If
        '</Snippet2>
    End Sub
    Sub ChangeFontFamily(ByVal Sender As Object, ByVal e As RoutedEventArgs)

        '<Snippet3>
        ffamily = btn2.FontFamily
        str = ffamily.ToString()
        If (str = ("Arial Black")) Then

            btn2.FontFamily = New FontFamily("Arial")
            btn2.Content = "FontFamily"

        Else

            btn2.FontFamily = New FontFamily("Arial Black")
            btn2.Content = "Control font family changes from Arial to Arial Black."

        End If
        '</Snippet3>
    End Sub
    Sub ChangeFontSize(ByVal Sender As Object, ByVal e As RoutedEventArgs)

        '<Snippet4>
        fsize = btn3.FontSize
        If (fsize = 16.0) Then

            btn3.FontSize = 10.0
            btn3.Content = "FontSize"

        Else

            btn3.FontSize = 16.0
            btn3.Content = "Control font size changes from 10 to 16."
        End If
        '</Snippet4>
    End Sub
    Sub ChangeFontStyle(ByVal Sender As Object, ByVal e As RoutedEventArgs)

        '<Snippet5>
        If (btn4.FontStyle = FontStyles.Italic) Then

            btn4.FontStyle = FontStyles.Normal
            btn4.Content = "FontStyle"

        Else

            btn4.FontStyle = FontStyles.Italic
            btn4.Content = "Control font style changes from Normal to Italic."
        End If
        '</Snippet5>
    End Sub
    Sub ChangeFontWeight(ByVal Sender As Object, ByVal e As RoutedEventArgs)

        '<Snippet6>
        If (btn5.FontWeight = FontWeights.Bold) Then

            btn5.FontWeight = FontWeights.Normal
            btn5.Content = "FontWeight"

        Else

            btn5.FontWeight = FontWeights.Bold
            btn5.Content = "Control font weight changes from Normal to Bold."
        End If
        '</Snippet6>
    End Sub
    Sub ChangeBorderBrush(ByVal Sender As Object, ByVal e As RoutedEventArgs)

        '<Snippet7>
        str = ((btn6.BorderBrush).ToString())
        If (Equals(btn6.BorderBrush, Brushes.Red)) Then

            btn6.BorderBrush = Brushes.Black
            btn6.Content = "BorderBrush"


        Else

            btn6.BorderBrush = Brushes.Red
            btn6.Content = "Control border brush changes from red to black."
        End If
        '</Snippet7>
    End Sub
    Sub ChangeHorizontalContentAlignment(ByVal Sender As Object, ByVal e As RoutedEventArgs)

        '<Snippet8>
        If (btn7.HorizontalContentAlignment = HorizontalAlignment.Left) Then

            btn7.HorizontalContentAlignment = HorizontalAlignment.Right
            btn7.Content = "HorizontalContentAlignment"


        Else

            btn7.HorizontalContentAlignment = HorizontalAlignment.Left
            btn7.Content = "Control horizontal alignment changes from left to right."
        End If
        '</Snippet8>
    End Sub
    Sub ChangeVerticalContentAlignment(ByVal Sender As Object, ByVal e As RoutedEventArgs)

        '<Snippet9>
        If (btn8.VerticalContentAlignment = VerticalAlignment.Top) Then

            btn8.VerticalContentAlignment = VerticalAlignment.Bottom
            btn8.Content = "VerticalContentAlignment"


        Else

            btn8.VerticalContentAlignment = VerticalAlignment.Top
            btn8.Content = "Control vertical alignment changes from top to bottom."
        End If
        '</Snippet9>
    End Sub

End Class
