Public Class FutureDateRule
    Inherits ValidationRule

    Public Overrides Function Validate(ByVal value As Object, ByVal cultureInfo As System.Globalization.CultureInfo) As System.Windows.Controls.ValidationResult
        Dim DateVal As DateTime
        Try
            DateVal = DateTime.Parse(value.ToString)
        Catch ex As FormatException
            Return New ValidationResult(False, "Value is not a valid date.")
        End Try

        If DateTime.Now.Date > DateVal Then
            Return New ValidationResult(False, "Please enter a date in the future.")
        Else
            Return New ValidationResult(True, Nothing)
        End If
    End Function
End Class
