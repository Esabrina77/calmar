Public Class CEchelle

    Private Structure STR_ECHELLE

        Public NbPixel As Integer
        Public InMetre As Integer

    End Structure

    Private Shared TableauEchelle() As STR_ECHELLE = {New STR_ECHELLE() With {.NbPixel = 100, .InMetre = 1}, _
                                               New STR_ECHELLE() With {.NbPixel = 150, .InMetre = 1}, _
                                               New STR_ECHELLE() With {.NbPixel = 200, .InMetre = 1}, _
                                               New STR_ECHELLE() With {.NbPixel = 250, .InMetre = 1}, _
                                               New STR_ECHELLE() With {.NbPixel = 300, .InMetre = 1}}

    Private Shared Echelle As Integer = 0

    Public Shared Sub ZoomUp()

        Echelle += 1
        If Echelle >= TableauEchelle.Length Then Echelle = TableauEchelle.Length - 1

    End Sub

    Public Shared Sub ZoomDown()

        Echelle -= 1
        If Echelle < 0 Then Echelle = 0

    End Sub

    Public Shared Function LongueurToPixel(ByVal Longueur As Double) As Double

        Return (Longueur * TableauEchelle(Echelle).NbPixel) / TableauEchelle(Echelle).InMetre

    End Function

End Class