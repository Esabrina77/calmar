Namespace Buoy

    Public Class CStructure

        Private _Nom As String
        Private _OffsetFlotteur As Double
        Private _OffsetOrganeau As Double
        Private _Masse As Double

        Protected CouleurInterieur As Color

        Private _LargeurMax As Double
        Private _HauteurMax As Double

        Private _Size As Size

        Private _Elements As New List(Of CDimensionElementTroncCone)

        Private Cnvs As Canvas

        Public Sub New()
            _Nom = ""
            _Masse = 0
            CouleurInterieur = Colors.Gray
            _Elements.Clear()
        End Sub

        Public Function Clone() As CStructure

            Clone = New CStructure

            Clone.Nom = Nom
            Clone.OffsetFlotteur = OffsetFlotteur
            Clone.OffsetOrganeau = OffsetOrganeau
            Clone.Masse = Masse

            For Each el In Elements
                Clone.AddElement(el.Clone)
            Next

        End Function

        Public Function IsEqual(ByVal CompStruct As CStructure) As Boolean

            Dim p0 As Integer

            If CompStruct.Nom <> Nom Then Return False
            If CompStruct.OffsetFlotteur <> OffsetFlotteur Then Return False
            If CompStruct.OffsetOrganeau <> OffsetOrganeau Then Return False
            If CompStruct.Volume <> Volume Then Return False
            If CompStruct.Masse <> Masse Then Return False

            If CompStruct.Elements.Count <> Elements.Count Then Return False
            For p0 = 0 To Elements.Count - 1
                If Not CompStruct.ElementAt(p0).IsEqual(Elements(p0)) Then Return False
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

        Property OffsetFlotteur As Double
            Get
                Return _OffsetFlotteur
            End Get
            Set(ByVal value As Double)
                _OffsetFlotteur = value
            End Set
        End Property

        Property OffsetOrganeau As Double
            Get
                Return _OffsetOrganeau
            End Get
            Set(ByVal value As Double)
                _OffsetOrganeau = value
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

        Public Sub RemoveElementAt(ByVal index As Integer)
            _Elements.RemoveAt(index)
            MakeSizingMax()
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

        End Sub

#End Region

#Region "Gestion du dessin de la piece"

        Private Sub DrawingBySize(ByVal sender As Object)

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
                    sender.Children.Add(TmpCnvs)

                    ' Placement en hauteur
                    MyHauteurMax += TmpCnvs.Height
                    TmpCnvs.SetValue(Canvas.TopProperty, _Size.Height - MyHauteurMax)

                    ' Verification quel est la longueur a prendre
                    TmpCnvs.SetValue(Canvas.LeftProperty, (_Size.Width - TmpCnvs.Width) / 2)
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

        Private _HAUTEUR_FLOTTEUR As Double
        Private _HAUTEUR_IMMERGEE As Double
        Private _HAUTEUR_IMMERGEE_0 As Double
        Private _VOLUME_IMMERGE As Double
        Private _SURFACE_IMMERGEE As Double
        Private _SURFACE_EMERGEE As Double

        Property HAUTEUR_FLOTTEUR() As Double
            Get
                Return _HAUTEUR_FLOTTEUR
            End Get
            Set(ByVal value As Double)
                _HAUTEUR_FLOTTEUR = value
            End Set
        End Property

        Property HAUTEUR_IMMERGEE() As Double
            Get
                Return _HAUTEUR_IMMERGEE
            End Get
            Set(ByVal value As Double)
                _HAUTEUR_IMMERGEE = value
                _HAUTEUR_IMMERGEE_0 = OffsetFlotteur + value
                CalculDonneesSurfaceVolume()
            End Set
        End Property

        ReadOnly Property HAUTEUR_FIN_FLOTTEUR() As Double
            Get
                Return OffsetFlotteur + HAUTEUR_FLOTTEUR
            End Get
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

        Private _INTER_HAUTEUR As Double
        Private _INTER_LONGUEUR As Double

        ReadOnly Property INTER_HAUTEUR() As Double
            Get
                Return _INTER_HAUTEUR
            End Get
        End Property
        ReadOnly Property INTER_LONGUEUR() As Double
            Get
                Return _INTER_LONGUEUR
            End Get
        End Property

#Region "Gestion des fonctions de calcul"

        Private Sub CalculDonneesSurfaceVolume()

            Dim p0 As Integer = 0
            Dim HauteurBas As Double = 0.0

            _INTER_HAUTEUR = 0
            _INTER_LONGUEUR = 0

            ' Initialisation de 
            _SURFACE_IMMERGEE = 0
            _SURFACE_EMERGEE = 0
            _VOLUME_IMMERGE = 0

            ' Mise en place des valeurs initiale
            For p0 = 0 To Elements.Count - 1
                If HauteurBas + Elements(p0).HauteurElement <= _HAUTEUR_IMMERGEE_0 Then
                    ' Calcul des valeurs immergée
                    _VOLUME_IMMERGE += Elements(p0).Volume
                    If Not (HauteurBas >= OffsetFlotteur And HauteurBas < HAUTEUR_FIN_FLOTTEUR) Then _SURFACE_IMMERGEE += Elements(p0).Surface
                End If
                If _HAUTEUR_IMMERGEE_0 > HauteurBas And HauteurBas + Elements(p0).HauteurElement > _HAUTEUR_IMMERGEE_0 Then
                    ' Calcul des valeurs immergée
                    _VOLUME_IMMERGE += Elements(p0).VolumeByHauteur(_HAUTEUR_IMMERGEE_0 - HauteurBas)

                    ' Calcul de la surface
                    If HauteurBas < OffsetFlotteur And _HAUTEUR_IMMERGEE_0 > HauteurBas Then
                        ' Calcul de la surface immergée
                        _SURFACE_IMMERGEE += Elements(p0).SurfaceByHauteur(_HAUTEUR_IMMERGEE_0 - HauteurBas)(0)
                    End If
                    If _HAUTEUR_IMMERGEE_0 > HAUTEUR_FIN_FLOTTEUR Then
                        ' Calcul de la surface immergée
                        _SURFACE_IMMERGEE += Elements(p0).SurfaceByHauteur(_HAUTEUR_IMMERGEE_0 - HAUTEUR_FIN_FLOTTEUR)(0)
                    End If
                    If (HauteurBas + Elements(p0).HauteurElement) > HAUTEUR_FIN_FLOTTEUR And (HauteurBas + Elements(p0).HauteurElement) > _HAUTEUR_IMMERGEE_0 Then
                        ' Calcul de la surface emerge
                        _SURFACE_EMERGEE += Elements(p0).SurfaceByHauteur((HauteurBas + Elements(p0).HauteurElement) - _HAUTEUR_IMMERGEE_0)(1)
                    End If
                End If
                If HauteurBas >= _HAUTEUR_IMMERGEE_0 Then
                    ' Calcul de la surface emerge
                    If Not (HauteurBas >= OffsetFlotteur And HauteurBas < HAUTEUR_FIN_FLOTTEUR) Then _SURFACE_EMERGEE += Elements(p0).Surface
                End If

                ' Incrementation de la hateur de l'element
                HauteurBas += Elements(p0).HauteurElement
            Next p0

            ' Multiplication par 1000 pour avoir des dcm3
            _VOLUME_IMMERGE = _VOLUME_IMMERGE * 1000

        End Sub

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

        End Function

#End Region

#End Region

    End Class

End Namespace