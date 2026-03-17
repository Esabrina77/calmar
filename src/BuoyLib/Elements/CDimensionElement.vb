Public Class CDimensionElement

    ' Unite en mm
    Private _LongueurBasElement As Double
    Private _LongueurHautElement As Double
    Private _HauteurElement As Double

    Private _CouleurInterieur As Color = Colors.White
    Private _CouleurTrait As Color = Colors.Black

    Private _Size As Size

    Public Function Clone() As CDimensionElement

        Clone = New CDimensionElement()

        Clone.LongueurBasElement = LongueurBasElement
        Clone.LongueurHautElement = LongueurHautElement
        Clone.HauteurElement = HauteurElement

        Clone.CouleurInterieur = CouleurInterieur
        Clone.CouleurTrait = CouleurTrait

    End Function

    Public Function IsEqual(ByVal CompDimEl As CDimensionElement) As Boolean

        If CompDimEl.LongueurBasElement <> LongueurBasElement Then Return False
        If CompDimEl.LongueurHautElement <> LongueurHautElement Then Return False
        If CompDimEl.HauteurElement <> HauteurElement Then Return False

        Return True

    End Function

    Property LongueurBasElement As Double
        Get
            Return _LongueurBasElement
        End Get
        Set(ByVal value As Double)
            _LongueurBasElement = value
        End Set
    End Property

    Property LongueurHautElement As Double
        Get
            Return _LongueurHautElement
        End Get
        Set(ByVal value As Double)
            _LongueurHautElement = value
        End Set
    End Property

    ReadOnly Property LongueurMaxElement As Double
        Get
            If _LongueurHautElement > _LongueurBasElement Then Return _LongueurHautElement
            If _LongueurBasElement > _LongueurHautElement Then Return _LongueurBasElement
            Return _LongueurHautElement
        End Get
    End Property

    Property HauteurElement As Double
        Get
            Return _HauteurElement
        End Get
        Set(ByVal value As Double)
            _HauteurElement = value
        End Set
    End Property

    Property CouleurInterieur As Color
        Get
            Return _CouleurInterieur
        End Get
        Set(ByVal value As Color)
            _CouleurInterieur = value
        End Set
    End Property

    Property CouleurTrait As Color
        Get
            Return _CouleurTrait
        End Get
        Set(ByVal value As Color)
            _CouleurTrait = value
        End Set
    End Property

    ReadOnly Property Surface() As Double
        Get
            Return _HauteurElement * (LongueurBasElement + LongueurHautElement) / 2
        End Get
    End Property

    Public Function SurfaceByHauteur(ByVal H As Double) As Double()

        Dim L_Inter As Double = LongueurBasElement + (H * (LongueurHautElement - LongueurBasElement) / HauteurElement)

        Return New Double() {CalculTrapeze(LongueurBasElement, L_Inter, H), CalculTrapeze(L_Inter, LongueurHautElement, HauteurElement - H)}

    End Function

    Private Function CalculTrapeze(ByVal L1 As Double, ByVal L2 As Double, ByVal H As Double) As Double

        Return H * (L1 + L2) / 2

    End Function

    Private Function MakeCanvas() As Canvas

        Dim Poly As New Polygon

        Dim LeftHaut As Double = 0
        Dim LeftBas As Double = 0
        Dim LargeurHaut As Double = 0
        Dim LargeurBas As Double = 0

        Dim HauteurMax As Double = 0

        Dim MyHauteurMax As Double = 0 '_Size.Height

        MakeCanvas = New Canvas

        HauteurMax = _Size.Height
        LargeurHaut = _Size.Width
        LargeurBas = _Size.Width

        If LongueurHautElement > LongueurBasElement Then
            LargeurBas = _Size.Width * (LongueurBasElement / LongueurHautElement)
            LeftBas = _Size.Width * (((LongueurHautElement - LongueurBasElement) / 2) / LongueurHautElement)
        ElseIf LongueurHautElement < LongueurBasElement Then
            LargeurHaut = _Size.Width * (LongueurHautElement / LongueurBasElement)
            LeftHaut = _Size.Width * (((LongueurBasElement - LongueurHautElement) / 2) / LongueurBasElement)
        End If

        Poly.Fill = New SolidColorBrush(_CouleurInterieur)
        Poly.Stroke = New SolidColorBrush(_CouleurTrait)
        Poly.StrokeThickness = 1

        Poly.Points.Clear()
        Poly.Points.Add(New Point(LeftHaut, 0))
        Poly.Points.Add(New Point(LeftHaut + LargeurHaut, 0))
        Poly.Points.Add(New Point(LeftBas + LargeurBas, _Size.Height))
        Poly.Points.Add(New Point(LeftBas, _Size.Height))
        Poly.Points.Add(New Point(LeftHaut, 0))

        MakeCanvas.Width = _Size.Width
        MakeCanvas.Height = _Size.Height

        MakeCanvas.Children.Clear()
        MakeCanvas.Children.Add(Poly)

        'AddHandler MakeCanvas.MouseLeftButtonDown, AddressOf _Parent.ClickElement

    End Function

    ReadOnly Property DrawCanvas(ByVal NewSize As Size) As Canvas
        Get
            _Size = NewSize
            Return MakeCanvas()
        End Get
    End Property

    ReadOnly Property DrawCanvasByHeight(ByVal Width As Double, ByVal Height As Double) As Canvas
        Get
            Do
                If Width < _Size.Width Then Height -= 1
                _Size = New Size((Height * (LongueurMaxElement / HauteurElement)), Height)
            Loop While Width < _Size.Width

            Return MakeCanvas()
        End Get
    End Property

End Class
