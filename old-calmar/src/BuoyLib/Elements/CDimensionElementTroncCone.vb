Public Class CDimensionElementTroncCone
    Inherits CDimensionElement

    Private _Diameter_L As Double
    Private _Diameter_H As Double
    Private _Diameter_I As Double
    Private _Volume As Double

    Public Overloads Function Clone() As CDimensionElementTroncCone

        Clone = New CDimensionElementTroncCone()

        Clone.LongueurBasElement = LongueurBasElement
        Clone.LongueurHautElement = LongueurHautElement
        Clone.HauteurElement = HauteurElement

        Clone.DiameterLow = DiameterLow
        Clone.DiameterHigh = DiameterHigh
        Clone.DiameterInter = DiameterInter
        Clone.VolumeReel = VolumeReel

        Clone.CouleurInterieur = CouleurInterieur
        Clone.CouleurTrait = CouleurTrait

    End Function

    Public Overloads Function IsEqual(ByVal CompDimEl As CDimensionElementTroncCone) As Boolean

        If CompDimEl.HauteurElement <> HauteurElement Then Return False
        If CompDimEl.DiameterLow <> DiameterLow Then Return False
        If CompDimEl.DiameterHigh <> DiameterHigh Then Return False
        If CompDimEl.DiameterInter <> DiameterInter Then Return False
        If CompDimEl.Volume <> Volume Then Return False

        Return True

    End Function

    Property DiameterLow As Double
        Get
            Return _Diameter_L
        End Get
        Set(ByVal value As Double)
            _Diameter_L = value
            MyBase.LongueurBasElement = _Diameter_L
        End Set
    End Property

    Property DiameterHigh As Double
        Get
            Return _Diameter_H
        End Get
        Set(ByVal value As Double)
            _Diameter_H = value
            MyBase.LongueurHautElement = _Diameter_H
        End Set
    End Property

    Property DiameterInter As Double
        Get
            Return _Diameter_I
        End Get
        Set(ByVal value As Double)
            _Diameter_I = value
        End Set
    End Property

    ReadOnly Property Volume As Double
        Get
            If _Volume = 0 Then Return VolumeCalcule
            Return _Volume
        End Get
    End Property

    ReadOnly Property SurfaceDiameterLow As Double
        Get
            Return Math.PI * (DiameterLow / 2) ^ 2
        End Get
    End Property

    ReadOnly Property SurfaceDiameterHigh As Double
        Get
            Return Math.PI * (DiameterHigh / 2) ^ 2
        End Get
    End Property

    ReadOnly Property SurfaceDiameterInter As Double
        Get
            Return Math.PI * (DiameterInter / 2) ^ 2
        End Get
    End Property

    Property VolumeReel As Double
        Get
            Return _Volume
        End Get
        Set(ByVal value As Double)
            _Volume = value
        End Set
    End Property

    ReadOnly Property VolumeCalcule As Double
        Get
            Return CalculVolumeTroncDeCone(MyBase.HauteurElement, SurfaceDiameterLow, SurfaceDiameterHigh) - CalculVolumeTroncDeCone(MyBase.HauteurElement, SurfaceDiameterInter, SurfaceDiameterInter)
        End Get
    End Property

    ReadOnly Property RatioVolume As Double
        Get
            If VolumeCalcule = 0 Then Return 1
            Return Volume / VolumeCalcule
        End Get
    End Property

    Public Function VolumeByHauteur(ByVal H As Double) As Double

        Dim L_Inter As Double = DiameterLow + (H * (DiameterHigh - DiameterLow) / MyBase.HauteurElement)

        Return (CalculVolumeTroncDeCone(H, SurfaceDiameterLow, (Math.PI * (L_Inter / 2) ^ 2)) - CalculVolumeTroncDeCone(H, SurfaceDiameterInter, SurfaceDiameterInter)) * RatioVolume

    End Function

    Private Function CalculVolumeTroncDeCone(ByVal H As Double, ByVal B1 As Double, ByVal b2 As Double) As Double

        Return H / 3 * (B1 + Math.Sqrt(B1 * b2) + b2)

    End Function

End Class
