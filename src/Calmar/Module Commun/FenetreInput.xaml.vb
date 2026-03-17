Public Class FenetreInput

    Property Message As String
        Get
            Return TextBlockMessage.Text.Replace(vbCrLf, "")
        End Get
        Set(value As String)
            TextBlockMessage.Text = vbCrLf + value + vbCrLf
        End Set
    End Property

    Property TextBoxMessage As String
        Get
            Return TextBlockRetourMessage.Text
        End Get
        Set(value As String)
            TextBlockRetourMessage.Text = value
        End Set
    End Property

    Private Sub RetourOK(sender As Object, e As System.Windows.RoutedEventArgs)

        Me.DialogResult = True

    End Sub

    Private Sub RetourCancel(sender As Object, e As System.Windows.RoutedEventArgs)

        Me.DialogResult = False

    End Sub

    Private Sub FenetreDemande_Loaded(sender As Object, e As System.Windows.RoutedEventArgs) Handles Me.Loaded

        IconeSystem.Source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(System.Drawing.SystemIcons.Question.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions())
        TextBlockRetourMessage.Focus()
        TextBlockRetourMessage.SelectAll()

    End Sub

    Private Sub TextBlockRetourMessage_KeyDown(sender As System.Object, e As System.Windows.Input.KeyEventArgs) Handles TextBlockRetourMessage.KeyDown

        If e.Key = Key.Enter Then
            Me.DialogResult = True
        End If

    End Sub

End Class
