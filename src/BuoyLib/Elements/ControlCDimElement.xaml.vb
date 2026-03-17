Imports System.Windows

Public Class ControlCDimElement

    Private _Element As CDimensionElement


    Property Element() As CDimensionElement
        Get
            Return _Element
        End Get
        Set(value As CDimensionElement)
            _Element = value
            MakeElement()
        End Set
    End Property

    Private Sub MakeElement()

        LongueurBas.Text = _Element.LongueurBasElement.ToString
        LongueurHaut.Text = _Element.LongueurHautElement.ToString
        Hauteur.Text = _Element.HauteurElement.ToString

        DrawCanvas.Child = _Element.DrawCanvas(New Size((80 * (_Element.LongueurMaxElement / _Element.HauteurElement)), 80))

    End Sub

    Private Sub LongueurHaut_TextChanged(sender As System.Object, e As System.Windows.Controls.TextChangedEventArgs) Handles LongueurHaut.TextChanged

        Dim dd As Double

        If Double.TryParse(LongueurHaut.Text, dd) Then
            _Element.LongueurHautElement = dd
        End If

    End Sub

    Private Sub Hauteur_TextChanged(sender As System.Object, e As System.Windows.Controls.TextChangedEventArgs) Handles Hauteur.TextChanged

        Dim dd As Double

        If Double.TryParse(Hauteur.Text, dd) Then
            _Element.HauteurElement = dd
        End If

    End Sub

    Private Sub LongueurBas_TextChanged(sender As System.Object, e As System.Windows.Controls.TextChangedEventArgs) Handles LongueurBas.TextChanged

        Dim dd As Double

        If Double.TryParse(LongueurBas.Text, dd) Then
            _Element.LongueurBasElement = dd
        End If

    End Sub

End Class
