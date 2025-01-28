Imports System     
Imports System.Windows     
Imports System.Windows.Controls     
Imports System.Windows.Documents     
Imports System.Windows.Navigation     
Imports System.Windows.Data     

namespace ExpenseIt

    Partial Public Class Pane1
        Inherits Page

        Dim myWindow As NavigationWindow
        Dim app As Application

        Private Sub OnLoaded(ByVal sender As Object, ByVal args As RoutedEventArgs)

            app = CType(System.Windows.Application.Current, Application)
            myWindow = CType(app.MainWindow, NavigationWindow)
        End Sub

        Private Sub CreateReport(ByVal sender As Object, ByVal args As RoutedEventArgs)

            Dim pane2 As Pane2 = New Pane2()
            pane2.DataContext = ListBox1.SelectedItem
            pane2.InitializeComponent()
            myWindow.Navigate(pane2)

        End Sub
    End Class
    End Namespace
