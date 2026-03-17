Imports System.ComponentModel

Public Class FenetreWait

    Public WithEvents Worker As BackgroundWorker

    Public Sub New(Optional Text As String = "Waiting")

        ' Cet appel est requis par le concepteur.
        InitializeComponent()

        ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
        Worker = New BackgroundWorker
        TextBlockWait.Text = Text

    End Sub

    Private Sub FenetreWait_Loaded(sender As Object, e As System.Windows.RoutedEventArgs) Handles Me.Loaded

        Worker.RunWorkerAsync()

    End Sub

    Private Sub Worker_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles Worker.DoWork

    End Sub

    Private Sub Worker_ProgressChanged(sender As Object, e As System.ComponentModel.ProgressChangedEventArgs) Handles Worker.ProgressChanged

    End Sub

    Private Sub Worker_RunWorkerCompleted(sender As Object, e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles Worker.RunWorkerCompleted

        Close()

    End Sub

End Class
