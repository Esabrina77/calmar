Namespace Buoy

    Public Class CEquipement

        Private _Nom As String
        Private _Masse As Double

        Private _Element As CDimensionElement

        Protected CouleurInterieur As Color

        Private _Size As Size

        Public Sub New()

            _Nom = ""
            _Masse = 0
            _Element = New CDimensionElement() With {.HauteurElement = 0, .LongueurBasElement = 0, .LongueurHautElement = 0}
            Couleur = Colors.Yellow

        End Sub

        Public Function Clone() As CEquipement

            Clone = New CEquipement

            Clone.Nom = Nom
            Clone.Masse = Masse

            Clone.Element = Element.Clone

        End Function

        Public Function IsEqual(ByVal CompEquipement As CEquipement) As Boolean

            If CompEquipement.Nom <> Nom Then Return False
            If CompEquipement.Masse <> Masse Then Return False

            If Not CompEquipement.Element.IsEqual(Element) Then Return False

            Return True

        End Function

        Property Nom As String
            Get
                Return _Nom
            End Get
            Set(ByVal value As String)
                _Nom = value
            End Set
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
                If Element IsNot Nothing Then Element.CouleurInterieur = value
                CouleurInterieur = value
            End Set
            Get
                Return CouleurInterieur
            End Get
        End Property

        ReadOnly Property LargeurMax() As Double
            Get
                Return Element.LongueurMaxElement
            End Get
        End Property

        ReadOnly Property HauteurMax() As Double
            Get
                Return Element.HauteurElement
            End Get
        End Property

        ReadOnly Property Surface() As Double
            Get
                Return Element.HauteurElement * (Element.LongueurBasElement + Element.LongueurHautElement) / 2
            End Get
        End Property

        ReadOnly Property RatioEcran() As Double
            Get
                Return (LargeurMax / HauteurMax)
            End Get
        End Property

        Property Element As CDimensionElement
            Set(value As CDimensionElement)
                _Element = value
                If value IsNot Nothing Then value.CouleurInterieur = Couleur
            End Set
            Get
                Return _Element
            End Get
        End Property

        Private Sub DrawingBySize(ByVal sender As Object)

            Dim Sz As Size
            Dim TmpCnvs As Canvas
            Dim MyHauteurMax As Double = 0 '_Size.Height

            If sender IsNot Nothing Then

                sender.Width = _Size.Width
                sender.Height = _Size.Height

                'Placement des elements dans l'ordre
                sender.Children.Clear()

                ' Mise en place des hauteurs et largeur
                If LargeurMax = 0 Then Sz.Width = 0 Else Sz.Width = _Size.Width * (Element.LongueurMaxElement / LargeurMax)
                If HauteurMax = 0 Then Sz.Height = 0 Else Sz.Height = _Size.Height * (Element.HauteurElement / HauteurMax)

                TmpCnvs = Element.DrawCanvas(Sz)
                sender.Children.Add(TmpCnvs)

                ' Placement en hauteur
                MyHauteurMax += TmpCnvs.Height
                TmpCnvs.SetValue(Canvas.TopProperty, _Size.Height - MyHauteurMax)

                ' Verification quel est la longueur a prendre
                TmpCnvs.SetValue(Canvas.LeftProperty, (_Size.Width - TmpCnvs.Width) / 2)

            End If

        End Sub

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

    End Class

    Public Class CEquipementSupplementaire

        Public _Name As String
        Public _Categorie As String
        Public _Nombre As Integer
        Public _MasseUnitaire As Double

        Public Sub New()
            _Name = ""
            _Categorie = ""
            _Nombre = 1
            _MasseUnitaire = 0
        End Sub

        Property Name As String
            Get
                Return _Name
            End Get
            Set(value As String)
                _Name = value
            End Set
        End Property

        Property Categorie As String
            Get
                Return _Categorie
            End Get
            Set(value As String)
                _Categorie = value
            End Set
        End Property

        Property Nombre As Integer
            Get
                Return _Nombre
            End Get
            Set(value As Integer)
                _Nombre = value
            End Set
        End Property

        Property MasseUnitaire As Double
            Get
                Return _MasseUnitaire
            End Get
            Set(value As Double)
                _MasseUnitaire = value
            End Set
        End Property

        ReadOnly Property Masse As Double
            Get
                Return Nombre * MasseUnitaire
            End Get
        End Property

    End Class

End Namespace