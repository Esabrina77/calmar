Namespace Buoy

    Public Class CFlotteur

        Private _Nom As String
        Private _Masse As Double

        Protected CouleurInterieur As Color = Colors.Yellow

        Private _LargeurMax As Double
        Private _HauteurMax As Double
        Private RatioScreen As Double

        Private _Size As Size

        Private _Elements As New List(Of CDimensionElementTroncCone)

        Public Sub New()
            _Nom = ""
            _Masse = 0
            CouleurInterieur = Colors.Yellow
            _Elements.Clear()
        End Sub

        Public Function Clone() As CFlotteur

            Clone = New CFlotteur

            Clone.Nom = Nom
            Clone.Masse = Masse

            For Each el In Elements
                Clone.AddElement(el.Clone)
            Next

        End Function

        Public Function IsEqual(ByVal CompFlotteur As CFlotteur) As Boolean

            Dim p0 As Integer

            If CompFlotteur.Nom <> Nom Then Return False
            If CompFlotteur.Volume <> Volume Then Return False
            If CompFlotteur.Masse <> Masse Then Return False

            If CompFlotteur.Elements.Count <> Elements.Count Then Return False
            For p0 = 0 To Elements.Count - 1
                If Not CompFlotteur.ElementAt(p0).IsEqual(Elements(p0)) Then Return False
            Next

            Return True

        End Function

#Region "Propriete de la classe"

        Property Nom As String
            Get
                Return _Nom
            End Get
            Set(ByVal value As String)
                _Nom = value
            End Set
        End Property

        ReadOnly Property Volume As Double
            Get
                Volume = 0
                If Elements.Length = 0 Then Return Volume
                For Each el In Elements
                    Volume += el.Volume
                Next
                Return Volume
            End Get
        End Property

        Property Masse As Double
            Get
                Return _Masse
            End Get
            Set(ByVal value As Double)
                _Masse = value
            End Set
        End Property

        Property Couleur As Color
            Set(value As Color)
                CouleurInterieur = value
            End Set
            Get
                Return CouleurInterieur
            End Get
        End Property

        ReadOnly Property LargeurMax() As Double
            Get
                Return _LargeurMax
            End Get
        End Property

        ReadOnly Property HauteurMax() As Double
            Get
                Return _HauteurMax
            End Get
        End Property

        ReadOnly Property RatioEcran() As Double
            Get
                Return (LargeurMax / HauteurMax)
            End Get
        End Property

#End Region

#Region "Gestion de l'ajout, suppression des elements de flotaison"

        ReadOnly Property Elements() As CDimensionElementTroncCone()
            Get
                Return _Elements.ToArray
            End Get
        End Property

        Public Sub AddElement(ByVal Element As CDimensionElementTroncCone)
            _Elements.Add(Element)
            MakeSizingMax()
            Element.CouleurInterieur = Couleur
        End Sub

        Public Sub AddElementAt(ByVal index As Integer, ByVal Element As CDimensionElementTroncCone)
            _Elements.Insert(index, Element)
            MakeSizingMax()
            Element.CouleurInterieur = Couleur
        End Sub

        Public Sub RemoveElementAt(ByVal index As Integer)
            _Elements.RemoveAt(index)
            MakeSizingMax()
        End Sub

        Public Sub NewElementAt(ByVal index As Integer, ByVal Element As CDimensionElementTroncCone)
            If index > -1 AndAlso index < _Elements.Count Then
                _Elements(index) = New CDimensionElementTroncCone() With {
                    .CouleurInterieur = Element.CouleurInterieur,
                    .CouleurTrait = Element.CouleurTrait,
                    .DiameterHigh = Element.DiameterHigh,
                    .DiameterInter = Element.DiameterInter,
                    .DiameterLow = Element.DiameterLow,
                    .HauteurElement = Element.HauteurElement,
                    .VolumeReel = Element.VolumeReel
                    }
                MakeSizingMax()
            End If
        End Sub

        Public Function ElementAt(ByVal index As Integer) As CDimensionElementTroncCone
            Return _Elements(index)
        End Function

        Public Sub MoveElementUp(ByVal index As Integer)

            Dim Element As CDimensionElementTroncCone = ElementAt(index)

            RemoveElementAt(index)
            AddElementAt(index + 1, Element)

        End Sub

        Public Sub MoveElementDown(ByVal index As Integer)

            Dim Element As CDimensionElementTroncCone = ElementAt(index)

            RemoveElementAt(index)
            AddElementAt(index - 1, Element)

        End Sub

        Private Sub MakeSizingMax()

            _HauteurMax = 0
            _LargeurMax = 0

            ' Recuperation de toutes les tailles
            For Each el In _Elements
                _HauteurMax += el.HauteurElement
                If el.LongueurMaxElement > _LargeurMax Then _LargeurMax = el.LongueurMaxElement
            Next

            RatioScreen = _LargeurMax / _HauteurMax

        End Sub

#End Region

#Region "Gestion du dessin de la piece"

        Private Sub DrawingBySize(ByVal sender As Object)

            Dim p0 As Integer = 0
            Dim Sz As Size
            Dim TmpCnvs As Canvas
            Dim MyHauteurMax As Double = 0 '_Size.Height

            If sender IsNot Nothing Then

                sender.Width = _Size.Width
                sender.Height = _Size.Height

                'Placement des elements dans l'ordre
                sender.Children.Clear()
                For Each el In _Elements

                    ' Mise en place des hauteurs et largeur
                    If LargeurMax = 0 Then Sz.Width = 0 Else Sz.Width = _Size.Width * (el.LongueurMaxElement / LargeurMax)
                    If HauteurMax = 0 Then Sz.Height = 0 Else Sz.Height = _Size.Height * (el.HauteurElement / HauteurMax)

                    TmpCnvs = el.DrawCanvas(Sz)
                    TmpCnvs.Tag = p0
                    sender.Children.Add(TmpCnvs)

                    ' Placement en hauteur
                    MyHauteurMax += TmpCnvs.Height
                    TmpCnvs.SetValue(Canvas.TopProperty, _Size.Height - MyHauteurMax)

                    ' Verification quel est la longueur a prendre
                    TmpCnvs.SetValue(Canvas.LeftProperty, (_Size.Width - TmpCnvs.Width) / 2)

                    ' Incrementation de l'element
                    p0 += 1
                Next

            End If

        End Sub

        ReadOnly Property DrawCanvas(ByVal NewSize As Size) As Canvas
            Get
                Dim Cnvs As New Canvas
                _Size = NewSize
                DrawingBySize(Cnvs)
                Return Cnvs
            End Get
        End Property

        ReadOnly Property DrawCanvasByHeight(ByVal Width As Double, ByVal Height As Double) As Canvas
            Get
                Dim Cnvs As New Canvas

                Do
                    If Width < _Size.Width Then Height -= 1
                    _Size = New Size((Height * RatioEcran), Height)
                Loop While Width < _Size.Width

                DrawingBySize(Cnvs)
                Return Cnvs
            End Get
        End Property

        ReadOnly Property DrawCanvasMiniature() As Canvas
            Get
                Dim Cnvs As New Canvas
                _Size = New Size(80 * IIf(RatioEcran > 0, RatioEcran, 1), 80)
                DrawingBySize(Cnvs)
                Return Cnvs
            End Get
        End Property

#End Region

#Region "Propriete de la classe pour les calculs de ligne de mouillage"

        Private _HAUTEUR_IMMERGEE As Double
        Private _VOLUME_IMMERGE As Double
        Private _SURFACE_IMMERGEE As Double
        Private _SURFACE_EMERGEE As Double

        Property HAUTEUR_IMMERGEE() As Double
            Get
                Return _HAUTEUR_IMMERGEE
            End Get
            Set(ByVal value As Double)
                _HAUTEUR_IMMERGEE = value
                CalculDonneesSurfaceVolume()
            End Set
        End Property

        ReadOnly Property VOLUME_IMMERGE() As Double
            Get
                Return _VOLUME_IMMERGE
            End Get
        End Property

        ReadOnly Property SURFACE_IMMERGEE() As Double
            Get
                Return _SURFACE_IMMERGEE
            End Get
        End Property

        ReadOnly Property SURFACE_EMERGEE() As Double
            Get
                Return _SURFACE_EMERGEE
            End Get
        End Property

#Region "Gestion des fonctions de calcul"

        Private Sub CalculDonneesSurfaceVolume()
            
            Dim p0 As Integer = 0
            Dim HauteurBas As Double = 0.0

            ' Initialisation de 
            _SURFACE_IMMERGEE = 0
            _SURFACE_EMERGEE = 0
            _VOLUME_IMMERGE = 0

            ' Mise en place des valeurs initiale
            For p0 = 0 To Elements.Count - 1
                If HauteurBas + Elements(p0).HauteurElement <= HAUTEUR_IMMERGEE Then
                    ' Calcul des valeurs immergée
                    _VOLUME_IMMERGE += Elements(p0).Volume
                    _SURFACE_IMMERGEE += Elements(p0).Surface
                End If
                If HAUTEUR_IMMERGEE > HauteurBas And HauteurBas + Elements(p0).HauteurElement > HAUTEUR_IMMERGEE Then
                    ' Calcul des valeurs immergée
                    _VOLUME_IMMERGE += Elements(p0).VolumeByHauteur(HAUTEUR_IMMERGEE - HauteurBas)
                    _SURFACE_IMMERGEE += Elements(p0).SurfaceByHauteur(HAUTEUR_IMMERGEE - HauteurBas)(0)
                    ' Calcul des valeurs emerge
                    _SURFACE_EMERGEE += Elements(p0).SurfaceByHauteur(HAUTEUR_IMMERGEE - HauteurBas)(1)
                End If
                If HauteurBas >= HAUTEUR_IMMERGEE Then
                    ' Calcul des valeurs emerge
                    _SURFACE_EMERGEE += Elements(p0).Surface
                End If

                ' Incrementation de la hateur de l'element
                HauteurBas += Elements(p0).HauteurElement
            Next p0

            ' Multiplication par 1000 pour avoir des dcm3
            _VOLUME_IMMERGE = _VOLUME_IMMERGE * 1000

        End Sub

        'Private Sub CalculDimensionFlotteurHauteurImmergee()

        '    Dim p0 As Integer = 0
        '    Dim VolumeDeplace As Double = 0

        '    Dim VolumeBas As Double = 0
        '    Dim VolumeInter As Double = 0

        '    Dim HauteurBas As Double = 0
        '    Dim HauteurInter As Double = 0

        '    Dim AirBas As Double = 0
        '    Dim AirInter As Double = 0
        '    Dim AirHaut As Double = 0

        '    Dim LongueurInter As Double = 0

        '    Dim ratio_erreur As Double = 0

        '    ' Verification que l'on est bien immergée
        '    If HAUTEUR_IMMERGEE < 0 Then
        '        _SURFACE_EMERGEE = 0
        '        _SURFACE_IMMERGEE = 0
        '        _VOLUME_IMMERGE = 0
        '        Exit Sub
        '    End If

        '    ' Mise en place des valeurs initiale
        '    For p0 = 0 To Elements.Count - 1
        '        ' Verification que la hauteur est superieur ou pas
        '        If HauteurBas + Elements(p0).HauteurElement > HAUTEUR_IMMERGEE Then Exit For
        '        ' Calcul des valeurs par definition
        '        VolumeBas += Elements(p0).Volume
        '        HauteurBas += Elements(p0).HauteurElement
        '        AirBas += CalculTrapeze(Elements(p0).DiameterLow, Elements(p0).DiameterHigh, Elements(p0).HauteurElement)
        '    Next p0

        '    ' Verification que l'on sort pas de la liste
        '    If p0 = Elements.Count Then
        '        _SURFACE_EMERGEE = 0
        '        _SURFACE_IMMERGEE = AirBas
        '        _VOLUME_IMMERGE = VolumeBas * 1000
        '        Exit Sub
        '    End If

        '    ' Mise en place des variables
        '    VolumeInter = Elements(p0).Volume
        '    HauteurInter = HAUTEUR_IMMERGEE - HauteurBas
        '    For p1 = p0 + 1 To Elements.Count - 1
        '        AirHaut += CalculTrapeze(Elements(p1).DiameterLow, Elements(p1).DiameterHigh, Elements(p1).HauteurElement)
        '    Next p1

        '    LongueurInter = LineariteSurTrapeze(Elements(p0).DiameterLow, Elements(p0).DiameterHigh, Elements(p0).HauteurElement, HauteurInter)
        '    AirInter = CalculTrapeze(Elements(p0).DiameterLow, LongueurInter, HauteurInter)

        '    ratio_erreur = VolumeInter / (CalculVolumeTroncDeCone(Elements(p0).HauteurElement, CalculAirDisque(Elements(p0).DiameterLow), CalculAirDisque(Elements(p0).DiameterHigh)) - CalculVolumeTroncDeCone(Elements(p0).HauteurElement, CalculAirDisque(Elements(p0).DiameterInter), CalculAirDisque(Elements(p0).DiameterInter)))

        '    ' Calcul du volume deplace
        '    VolumeDeplace = VolumeBas + (CalculVolumeTroncDeCone(HauteurInter, CalculAirDisque(Elements(p0).DiameterLow), CalculAirDisque(LongueurInter)) - CalculVolumeTroncDeCone(HauteurInter, CalculAirDisque(Elements(p0).DiameterInter), CalculAirDisque(Elements(p0).DiameterInter))) * ratio_erreur
        '    _VOLUME_IMMERGE = VolumeDeplace * 1000

        '    ' Calcul des airs
        '    _SURFACE_IMMERGEE = (AirBas + AirInter)
        '    _SURFACE_EMERGEE = ((CalculTrapeze(Elements(p0).DiameterLow, Elements(p0).DiameterHigh, Elements(p0).HauteurElement) - AirInter) + AirHaut)

        'End Sub

        Private Function CalculTrapeze(ByVal L1 As Double, ByVal L2 As Double, ByVal H As Double) As Double

            Return H * (L1 + L2) / 2

        End Function

        Private Function CalculAirDisque(ByVal D As Double) As Double

            Return Math.PI * (D / 2) ^ 2

        End Function

        Private Function CalculVolumeTroncDeCone(ByVal H As Double, ByVal B1 As Double, ByVal b2 As Double) As Double

            Return H / 3 * (B1 + Math.Sqrt(B1 * b2) + b2)

        End Function

        Private Function LineariteSurTrapeze(ByVal L1 As Double, ByVal L2 As Double, ByVal H As Double, ByVal NewHauteur As Double) As Double

            Return L1 + (NewHauteur * (L2 - L1) / H)
            'Return L1 + ((NewHauteur * ((L2 - L1) / 2)) / H) * 2

        End Function

#End Region

#End Region

    End Class

End Namespace