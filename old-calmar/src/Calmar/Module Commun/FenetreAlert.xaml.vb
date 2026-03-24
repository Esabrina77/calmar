Public Class FenetreAlert

    Property Message As String
        Get
            Return TextBlockMessage.Text.Replace(vbCrLf, "")
        End Get
        Set(value As String)
            TextBlockMessage.Text = vbCrLf + value + vbCrLf
        End Set
    End Property

    Private Sub RetourOK(sender As Object, e As System.Windows.RoutedEventArgs)

        Me.DialogResult = True

    End Sub

    Private Sub FenetreAlert_Loaded(sender As Object, e As System.Windows.RoutedEventArgs) Handles Me.Loaded

        IconeSystem.Source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(System.Drawing.SystemIcons.Warning.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions())

    End Sub

End Class
