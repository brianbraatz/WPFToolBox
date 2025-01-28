Imports System
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Media
Imports System.Windows.Navigation

Namespace SDKSample

    Public Class StackPanel_VB
        Inherits Page

        Public Sub New()
            '<Snippet2>
            WindowTitle = "StackPanel Sample"
            'Create a StackPanel as the root Panel
            Dim myStackPanel As New StackPanel()
            Dim myBorder1 As New Border()
            myBorder1.Background = Brushes.SkyBlue
            myBorder1.BorderBrush = Brushes.Black
            myBorder1.BorderThickness = New Thickness(1)
            Dim txt1 As New TextBlock()
            txt1.Foreground = Brushes.Black
            txt1.FontSize = 12
            txt1.Text = "Stacked Item #1"
            myBorder1.Child = txt1

            Dim myBorder2 As New Border()
            myBorder2.Background = Brushes.CadetBlue
            myBorder2.Width = 400
            myBorder2.BorderBrush = Brushes.Black
            myBorder2.BorderThickness = New Thickness(1)
            Dim txt2 As New TextBlock()
            txt2.Foreground = Brushes.Black
            txt2.FontSize = 14
            txt2.Text = "Stacked Item #2"
            myBorder2.Child = txt2

            Dim myBorder3 As New Border()
            myBorder3.Background = Brushes.LightGoldenrodYellow
            myBorder3.BorderBrush = Brushes.Black
            myBorder3.BorderThickness = New Thickness(1)
            Dim txt3 As New TextBlock()
            txt3.Foreground = Brushes.Black
            txt3.FontSize = 16
            txt3.Text = "Stacked Item #3"
            myBorder3.Child = txt3

            Dim myBorder4 As New Border()
            myBorder4.Background = Brushes.PaleGreen
            myBorder4.Width = 200
            myBorder4.BorderBrush = Brushes.Black
            myBorder4.BorderThickness = New Thickness(1)
            Dim txt4 As New TextBlock()
            txt4.Foreground = Brushes.Black
            txt4.FontSize = 18
            txt4.Text = "Stacked Item #4"
            myBorder4.Child = txt4

            Dim myBorder5 As New Border()
            myBorder5.Background = Brushes.White
            myBorder5.BorderBrush = Brushes.Black
            myBorder5.BorderThickness = New Thickness(1)
            Dim txt5 As New TextBlock()
            txt5.Foreground = Brushes.Black
            txt5.FontSize = 20
            txt5.Text = "Stacked Item #5"
            myBorder5.Child = txt5

            ' Add the Borders to the StackPanel Children Collection
            myStackPanel.Children.Add(myBorder1)
            myStackPanel.Children.Add(myBorder2)
            myStackPanel.Children.Add(myBorder3)
            myStackPanel.Children.Add(myBorder4)
            myStackPanel.Children.Add(myBorder5)
            Me.Content = myStackPanel
            '</Snippet2>
        End Sub
    End Class
    'Displays the Sample
    Public Class app
        Inherits Application

        Protected Overrides Sub OnStartup(ByVal e As StartupEventArgs)
            MyBase.OnStartup(e)
            CreateAndShowMainWindow()
        End Sub

        Private Sub CreateAndShowMainWindow()
            ' Create the application's main window.
            Dim myWindow As New NavigationWindow()
            ' Display the sample
            Dim myContent As New StackPanel_VB()
            myWindow.Navigate(myContent)
            MainWindow = myWindow
            myWindow.Show()
        End Sub
    End Class
    ' Starts the application.
    Public NotInheritable Class EntryClass

        Public Shared Sub Main()
            Dim app As New app()
            app.Run()
        End Sub
    End Class
End Namespace