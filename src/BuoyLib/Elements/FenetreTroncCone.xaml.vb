Partial Public Class FenetreTroncCone
    Inherits ChildWindow

    Private _Element As CDimensionElementTroncCone
    Private TmpElement As CDimensionElementTroncCone

    Public Sub New(ByRef RefElement As CDimensionElementTroncCone)
        InitializeComponent()

        _Element = RefElement
        Element = New CDimensionElementTroncCone(Me) With {
            .CouleurInterieur = _Element.CouleurInterieur,
            .CouleurTrait = _Element.CouleurTrait,
            .DiameterHigh = _Element.DiameterHigh,
            .DiameterInter = _Element.DiameterInter,
            .DiameterLow = _Element.DiameterLow,
            .HauteurElement = _Element.HauteurElement,
            .Volume = _Element.Volume
        }

    End Sub

    Private Sub OKButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs) Handles OKButton.Click

        _Element = New CDimensionElementTroncCone(Me) With {
            .CouleurInterieur = Element.CouleurInterieur,
            .CouleurTrait = Element.CouleurTrait,
            .DiameterHigh = Element.DiameterHigh,
            .DiameterInter = Element.DiameterInter,
            .DiameterLow = Element.DiameterLow,
            .HauteurElement = Element.HauteurElement,
            .Volume = Element.Volume
        }

        Me.DialogResult = True
    End Sub

    Private Sub CancelButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs) Handles CancelButton.Click
        Me.DialogResult = False
    End Sub

    Property Element() As CDimensionElementTroncCone
        Get
            Return TmpElement
        End Get
        Set(value As CDimensionElementTroncCone)
            TmpElement = value
            MakeElement()
        End Set
    End Property

    Private Sub MakeElement()

        DiametreBas.Text = TmpElement.DiameterLow.ToString
        DiametreHaut.Text = TmpElement.DiameterHigh.ToString
        DiametreInt.Text = TmpElement.DiameterInter.ToString
        Hauteur.Text = TmpElement.HauteurElement.ToString
        Volume.Text = TmpElement.Volume.ToString

        DrawElement()

    End Sub

    Private Sub DrawElement()

        DrawCanvas.Child = TmpElement.DrawCanvasByHeight(150, 60)

    End Sub

    Private Sub DiametreHaut_TextChanged(sender As System.Object, e As System.Windows.Controls.TextChangedEventArgs) Handles DiametreHaut.TextChanged

        Dim dd As Double

        If Double.TryParse(DiametreHaut.Text, dd) Then
            TmpElement.DiameterHigh = dd
            DrawElement()
        End If

    End Sub

    Private Sub Hauteur_TextChanged(sender As System.Object, e As System.Windows.Controls.TextChangedEventArgs) Handles Hauteur.TextChanged

        Dim dd As Double

        If Double.TryParse(Hauteur.Text, dd) Then
            TmpElement.HauteurElement = dd
            DrawElement()
        End If

    End Sub

    Private Sub LongueurBas_TextChanged(sender As System.Object, e As System.Windows.Controls.TextChangedEventArgs) Handles DiametreBas.TextChanged

        Dim dd As Double

        If Double.TryParse(DiametreBas.Text, dd) Then
            TmpElement.DiameterLow = dd
            DrawElement()
        End If

    End Sub

    Private Sub Volume_TextChanged(sender As System.Object, e As System.Windows.Controls.TextChangedEventArgs) Handles Volume.TextChanged

        Dim dd As Double

        If Double.TryParse(Volume.Text, dd) Then
            TmpElement.Volume = dd
        End If

    End Sub

    Private Sub DiametreInt_TextChanged(sender As System.Object, e As System.Windows.Controls.TextChangedEventArgs) Handles DiametreInt.TextChanged

        Dim dd As Double

        If Double.TryParse(DiametreInt.Text, dd) Then
            TmpElement.DiameterInter = dd
        End If

    End Sub

End Class
