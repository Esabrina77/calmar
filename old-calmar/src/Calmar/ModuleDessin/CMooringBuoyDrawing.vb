Imports BuoyLib
Imports BuoyLib.Buoy

Class CMooringBuoyDrawing
    Inherits Canvas

    Private Buoy As CBouee
    Private DataSite As CCalculMouillage.STR_RET_CALCUL_RESULTAT

    Private ListPoint As Dictionary(Of Double, Double)
    Private ListMaillon As New List(Of STR_MAILLON)

    Private NombreMaillon As Integer

    Private MaxX As Double
    Private MaxY As Double

    Private TailleMaillon As Double

    Private Structure STR_MAILLON

        Public Pt As Point
        Public Angle As Double

    End Structure

    WriteOnly Property Bouee As CBouee
        Set(value As CBouee)
            Buoy = value
        End Set
    End Property

    WriteOnly Property Resultat As CCalculMouillage.STR_RET_CALCUL_RESULTAT
        Set(value As CCalculMouillage.STR_RET_CALCUL_RESULTAT)
            DataSite = value
            ListPoint = GetCatenairePoints(DataSite.EffortHorizontalKg, DataSite.PoidsLineiqueImmergeChaine, DataSite.ProfondeurMax)

            MaxX = ListPoint.Last.Key
            MaxY = ListPoint.Last.Value

            TailleMaillon = CalculLongueurMaillon(DataSite.DiametreChaine, DataSite.Type)
            NombreMaillon = CInt(value.LongueurCat / TailleMaillon)
            Dim Angle As Double = 0

            ' Calcul des maillons
            For p0 = 0 To ListPoint.Count - 1
                If p0 > 0 Then Angle = CalculAngle(New Point(ListPoint.ElementAt(p0 - 1).Key, ListPoint.ElementAt(p0 - 1).Value), New Point(ListPoint.ElementAt(p0).Key, ListPoint.ElementAt(p0).Value))
                ListMaillon.Add(New STR_MAILLON() With {.Pt = New Point(ListPoint.ElementAt(p0).Key, ListPoint.ElementAt(p0).Value), .Angle = Angle})
            Next
        End Set
    End Property

    Private Function CalculLongueurMaillon(ByVal diametre As Double, ByVal type As String) As Double

        Dim tp As Double

        If type.ToLower = "stud" Then Return 4
        tp = Double.Parse(type.Replace("D", ""))

        Return diametre * tp

    End Function

    Private Function CalculAngle(ByVal Pt1 As Point, ByVal Pt2 As Point) As Double

        Dim distanceHypo As Double = Math.Sqrt((Pt2.X - Pt1.X) ^ 2 + (Pt2.Y - Pt1.Y) ^ 2)
        Dim distanceCoteAdj As Double = (Pt2.X - Pt1.X)

        CalculAngle = 360 - (Math.Acos(distanceCoteAdj / distanceHypo) * 180 / Math.PI)

    End Function

    Private Sub CBuoyDrawing_SizeChanged(sender As Object, e As System.Windows.SizeChangedEventArgs) Handles Me.SizeChanged

        Width = e.NewSize.Width
        Height = e.NewSize.Height

        Dim Width10p As Double = Width - (Width * 0.1)
        Dim Height34 As Double = Height * 3 / 4

        Dim RatioScreen As Double = Width / Height

        If Height = 0 Then
            If Width = 0 Then Return
            Height = Width
        End If
        If Width < (RatioScreen * Height) Then
            Height = Width / RatioScreen
        Else
            Width = RatioScreen * Height
        End If

        Me.Children.Clear()
        For Each maillon In ListMaillon
            Dim _elipse As Ellipse = CreateMaillon(TailleMaillon, maillon.Angle)
            Me.Children.Add(_elipse)
            _elipse.SetValue(Canvas.LeftProperty, maillon.Pt.X * Width10p / MaxX)
            _elipse.SetValue(Canvas.TopProperty, Height - (maillon.Pt.Y * Height34 / MaxY))
        Next

        Dim CBuoy As CBuoyDrawing = New CBuoyDrawing(Buoy)
        CBuoy.Width = CBuoy.LargeurMax * Width10p / MaxX
        CBuoy.Height = CBuoy.Width / (CBuoy.LargeurMax / CBuoy.HauteurMax)

        Me.Children.Add(CBuoy)
        CBuoy.SetValue(Canvas.LeftProperty, Width10p - ((CBuoy.Width / 2) - (TailleMaillon / 2))) '
        CBuoy.SetValue(Canvas.TopProperty, Height - (Height34 + CBuoy.Height - (TailleMaillon / 2)))

        Dim rtTransform As New RotateTransform()
        CBuoy.RenderTransform = rtTransform

        rtTransform.Angle = DataSite.AngleTangence
        rtTransform.CenterX = CBuoy.Width / 2
        rtTransform.CenterY = CBuoy.Height

    End Sub

    Private Function CreateMaillon(ByVal taille As Integer, ByVal angle As Double) As Ellipse

        Dim rtTransform As New RotateTransform()
        Dim myEllipse As New Ellipse()

        Dim mySolidColorBrush As New SolidColorBrush()

        myEllipse.Fill = New SolidColorBrush(Colors.Transparent)
        myEllipse.StrokeThickness = 1
        myEllipse.Stroke = Brushes.Black

        ' Set the width and height of the Ellipse.
        myEllipse.Width = taille
        myEllipse.Height = taille * 2 / 3

        myEllipse.RenderTransform = rtTransform
        rtTransform.Angle = angle

        rtTransform.CenterX = myEllipse.Width / 2
        rtTransform.CenterY = myEllipse.Height / 2

        Return myEllipse

    End Function

    Private Function GetCatenairePoints(ByVal ForceH As Double, ByVal PoidsLineique As Double, ByVal Profondeur As Double) As Dictionary(Of Double, Double)

        Dim valueList As New Dictionary(Of Double, Double)

        Dim EffortPoids As Double = (ForceH / PoidsLineique)
        Dim y As Double = 0
        Dim x As Double = 0.0

        Try
            Do
                y = EffortPoids * (Math.Cosh(x / EffortPoids) - 1)
                valueList.Add(x, y)

                ' Deplacement de 0.08m
                x += 0.25
            Loop While y < Profondeur
        Catch
        End Try

        Return valueList

    End Function

End Class
