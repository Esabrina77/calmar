Imports BuoyLib
Imports BuoyLib.Buoy

Class CBuoyDrawing
    Inherits Canvas

    Private _Buoy As CBouee

    Private _LargeurMax As Double
    Private _HauteurMax As Double
    Private _RatioScreen As Double

    Private IsRatioScreenEnable As Boolean = True

    Public Sub New(ByVal Buoy As CBouee)
        MyBase.New()
        _Buoy = Buoy
        MakeSizingMax()
    End Sub

    ReadOnly Property Nom As String
        Get
            Return _Buoy.Nom
        End Get
    End Property

    Public ReadOnly Property LargeurMax As Double
        Get
            Return _LargeurMax
        End Get
    End Property

    Public ReadOnly Property HauteurMax As Double
        Get
            Return _HauteurMax
        End Get
    End Property

    ReadOnly Property MyCanvas As Canvas
        Get
            Return Me
        End Get
    End Property

    Property RatioEnable As Boolean
        Get
            Return IsRatioScreenEnable
        End Get
        Set(value As Boolean)
            IsRatioScreenEnable = value
        End Set
    End Property

    Private Sub MakeSizingMax()

        _HauteurMax = 0
        _LargeurMax = 0

        ' Hauteur de la flotteur
        ' Recuperation de toutes les tailles
        If _Buoy.FlotteurBouee IsNot Nothing Then
            With _Buoy.FlotteurBouee
                For Each el In .Elements
                    _HauteurMax += el.HauteurElement
                    If el.LongueurMaxElement > LargeurMax Then _LargeurMax = el.LongueurMaxElement
                Next
            End With
        End If

        ' Deduction de l'offset
        If _Buoy.FlotteurBouee IsNot Nothing And _Buoy.StructureBouee IsNot Nothing Then
            If _Buoy.FlotteurBouee.HauteurMax < (_Buoy.StructureBouee.HauteurMax - _Buoy.StructureBouee.OffsetFlotteur) Then
                _HauteurMax = 0
            End If
        End If

        ' Hauteur de la structure
        ' Recuperation de toutes les tailles
        If _Buoy.StructureBouee IsNot Nothing Then
            With _Buoy.StructureBouee
                For Each el In .Elements
                    _HauteurMax += el.HauteurElement
                    If el.LongueurMaxElement > LargeurMax Then _LargeurMax = el.LongueurMaxElement
                Next
            End With
        End If

        '' Hauteur de la pylone
        '' Recuperation de toutes les tailles
        If _Buoy.PyloneBouee IsNot Nothing Then
            For Each el In _Buoy.PyloneBouee
                _HauteurMax += el.Element.HauteurElement
                If el.Element.LongueurMaxElement > LargeurMax Then _LargeurMax = el.Element.LongueurMaxElement
            Next
        End If

        '' Recuperation de toutes les tailles
        If _Buoy.EquipementBouee IsNot Nothing Then
            For Each el In _Buoy.EquipementBouee
                _HauteurMax += el.Element.HauteurElement
                If el.Element.LongueurMaxElement > LargeurMax Then _LargeurMax = el.Element.LongueurMaxElement
            Next
        End If

        _RatioScreen = LargeurMax / HauteurMax

    End Sub

    Private Sub CBuoyDrawing_SizeChanged(sender As Object, e As System.Windows.SizeChangedEventArgs) Handles Me.SizeChanged

        Dim CircleOrganeau As New Ellipse()

        Dim Cnvs As Canvas
        Dim MyHauteurMax As Double = 0.0

        Width = e.NewSize.Width
        Height = e.NewSize.Height

        If IsRatioScreenEnable Then
            If Height = 0 Then
                If Width = 0 Then Return
                Height = Width
            End If
            If Width < (_RatioScreen * Height) Then
                Height = Width / _RatioScreen
            Else
                Width = _RatioScreen * Height
            End If
        End If

        sender.Children.clear()

        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        '''' MISE EN PLACE DE LA STRUCTURE
        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        If _Buoy.StructureBouee IsNot Nothing Then
            With _Buoy.StructureBouee
                If _Buoy.StructureBouee.Elements.Count > 0 Then
                    Cnvs = .DrawCanvas(New Size(Width * (.LargeurMax / LargeurMax), Height * (.HauteurMax / HauteurMax)))
                    ' Mise en place de la position de la structure
                    MyHauteurMax += Cnvs.Height
                    Cnvs.SetValue(Canvas.TopProperty, Height - MyHauteurMax)
                    ' Verification quel est la longueur a prendre
                    Cnvs.SetValue(Canvas.LeftProperty, (Width - Cnvs.Width) / 2)
                    sender.Children.Add(Cnvs)
                End If

                ' Definition de la hauteur par l'offset
                MyHauteurMax = Height * (.OffsetFlotteur / HauteurMax)

            End With
        End If
        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        '''' FIN DE LA MISE EN PLACE DE LA STRUCTURE
        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        '''' MISE EN PLACE DU FLOTTEUR
        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        If _Buoy.FlotteurBouee IsNot Nothing Then
            With _Buoy.FlotteurBouee
                Cnvs = .DrawCanvas(New Size(Width * (.LargeurMax / LargeurMax), Height * (.HauteurMax / HauteurMax)))

                ' Mise en place de la position de la structure
                MyHauteurMax += Cnvs.Height
                Cnvs.SetValue(Canvas.TopProperty, Height - MyHauteurMax)
                ' Verification quel est la longueur a prendre
                Cnvs.SetValue(Canvas.LeftProperty, (Width - Cnvs.Width) / 2)
                sender.Children.Add(Cnvs)
            End With
        End If
        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        '''' FIN DE LA MISE EN PLACE DU FLOTTEUR
        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

        ' Mise a niveau apres l'offset
        If _Buoy.StructureBouee IsNot Nothing AndAlso _Buoy.StructureBouee.Elements.Length > 0 Then
            MyHauteurMax = Height * (_Buoy.StructureBouee.HauteurMax / HauteurMax)
        End If

        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        '''' MISE EN PLACE DU PYLONE
        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        For Each pyl In _Buoy.PyloneBouee
            If pyl Is Nothing Then Continue For
            Cnvs = pyl.DrawCanvas(New Size(Width * (pyl.LargeurMax / LargeurMax), Height * (pyl.HauteurMax / HauteurMax)))
            'Cnvs.Width = _Size.Width * (pyl.LargeurMax / LargeurMax)
            'Cnvs.Height = _Size.Height * (pyl.HauteurMax / HauteurMax)

            ' Mise en place de la position de la structure
            MyHauteurMax += Cnvs.Height
            Cnvs.SetValue(Canvas.TopProperty, Height - MyHauteurMax)
            ' Verification quel est la longueur a prendre
            Cnvs.SetValue(Canvas.LeftProperty, (Width - Cnvs.Width) / 2)
            sender.Children.Add(Cnvs)
        Next
        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        '''' FIN DE LA MISE EN PLACE DU PYLONE
        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        '''' MISE EN PLACE DES EQUIPEMENTS
        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        For Each equ In _Buoy.EquipementBouee
            If equ Is Nothing Then Continue For
            Cnvs = equ.DrawCanvas(New Size(Width * (equ.LargeurMax / LargeurMax), Height * (equ.HauteurMax / HauteurMax)))
            'Cnvs.Width = _Size.Width * (equ.LargeurMax / LargeurMax)
            'Cnvs.Height = _Size.Height * (equ.HauteurMax / HauteurMax)

            ' Mise en place de la position de la structure
            MyHauteurMax += Cnvs.Height
            Cnvs.SetValue(Canvas.TopProperty, Height - MyHauteurMax)
            ' Verification quel est la longueur a prendre
            Cnvs.SetValue(Canvas.LeftProperty, (Width - Cnvs.Width) / 2)
            sender.Children.Add(Cnvs)
        Next
        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        '''' FIN DE LA MISE EN PLACE DES EQUIPEMENTS
        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

        'Dessin de l'organeau

        If _Buoy.StructureBouee IsNot Nothing Then
            With _Buoy.StructureBouee

                ' Definition de la hauteur par l'offset
                MyHauteurMax = Height * (.OffsetFlotteur / HauteurMax)

                ' Dessin de l'organeau
                CircleOrganeau.Stroke = New SolidColorBrush(Colors.Black)
                CircleOrganeau.Width = Width * ((_Buoy.ChaineMax * 2 / 1000) / LargeurMax)
                CircleOrganeau.Height = Width * ((_Buoy.ChaineMax * 2 / 1000) / LargeurMax)
                CircleOrganeau.SetValue(Canvas.TopProperty, Height - MyHauteurMax + (Height * (.OffsetOrganeau / HauteurMax)) - CircleOrganeau.Height / 2)
                CircleOrganeau.SetValue(Canvas.LeftProperty, (Width - CircleOrganeau.Width) / 2)
                sender.Children.Add(CircleOrganeau)
            End With
        End If

    End Sub

End Class
