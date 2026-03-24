Public Class FenetreVisualisation

    Private _DrawingBuoy1 As CBuoyDrawing
    Private _DrawingBuoy2 As CBuoyDrawing

    Private _CalculBuoy1 As CCalculMouillage
    Private _CalculBuoy2 As CCalculMouillage

    Private _RetourCalcul1 As CCalculMouillage.STR_RET_CALCUL_RESULTAT = Nothing
    Private _RetourCalcul2 As CCalculMouillage.STR_RET_CALCUL_RESULTAT = Nothing

    Private VitesseCourant As Double = 0
    Private VitesseVent As Double = 0
    Private HouleMax As Double = 0

    Public Sub New(ByVal CalculBuoy1 As CCalculMouillage, ByVal CalculBuoy2 As CCalculMouillage)

        ' Cet appel est requis par le concepteur.
        InitializeComponent()

        ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
        _CalculBuoy1 = CalculBuoy1
        _CalculBuoy2 = CalculBuoy2

        VitesseCourant = _CalculBuoy1.VitesseCourant
        VitesseVent = _CalculBuoy1.VitesseVent
        HouleMax = _CalculBuoy1.HouleMax

        AddHandler _CalculBuoy1.RetourCalcul, AddressOf _CalculBuoy1_RetourCalcul
        AddHandler _CalculBuoy2.RetourCalcul, AddressOf _CalculBuoy2_RetourCalcul

    End Sub

    Private Sub FenetreVisualisation_Closing(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles Me.Closing

        RemoveHandler _CalculBuoy1.RetourCalcul, AddressOf _CalculBuoy1_RetourCalcul
        RemoveHandler _CalculBuoy2.RetourCalcul, AddressOf _CalculBuoy2_RetourCalcul

        CalculFull(_CalculBuoy1)
        _CalculBuoy1.DoCalcul()
        CalculFull(_CalculBuoy2)
        _CalculBuoy2.DoCalcul()

    End Sub

    Private Sub Window_Loaded(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles MyBase.Loaded

        Title = "Calmar " + Version
        Icon = GetIconCalmar()

        _DrawingBuoy1 = New CBuoyDrawing(_CalculBuoy1.BUOY)
        BuoyDrawing1.Child = _DrawingBuoy1

        If _CalculBuoy2.CheckCalcul Then
            _DrawingBuoy2 = New CBuoyDrawing(_CalculBuoy2.BUOY)

            BuoyDrawing2.Child = _DrawingBuoy2
        End If

        ViewRadioButtonPIC.IsChecked = True

        ChangeLangueInLayout()

    End Sub

    Private Sub Window_KeyUp(sender As System.Object, e As System.Windows.Input.KeyEventArgs) Handles MyBase.KeyUp

        If e.Key = Key.Escape Then
            Close()
        End If

    End Sub

    Private Sub CalculPIC(ByRef cc As CCalculMouillage)

        cc.VitesseCourant = 0
        cc.VitesseVent = 0
        cc.HouleMax = 0

    End Sub

    Private Sub CalculFull(ByRef cc As CCalculMouillage)

        cc.VitesseCourant = VitesseCourant
        cc.VitesseVent = VitesseVent
        cc.HouleMax = HouleMax

    End Sub

    Private Sub _CalculBuoy1_RetourCalcul(RetourCalcul As CCalculMouillage.STR_RET_CALCUL_RESULTAT)

        _RetourCalcul1 = RetourCalcul
        RefreshGraphics()

        'If RetourCalcul.TirantEau / _DrawingBuoy1.HauteurMax > 1 Then
        '    MakeWaterLevel(WaterLevel1, BuoyDrawing1.ActualHeight)
        '    BackgroundWaterLevel1.Margin = New Thickness(0, 0, 0, BuoyDrawing1.ActualHeight)
        'Else
        '    MakeWaterLevel(WaterLevel1, BuoyDrawing1.ActualHeight * (1 - (RetourCalcul.TirantEau / _DrawingBuoy1.HauteurMax)))
        '    BackgroundWaterLevel1.Margin = New Thickness(0, 0, 0, BuoyDrawing1.ActualHeight * (RetourCalcul.TirantEau / _DrawingBuoy1.HauteurMax))
        'End If

        'If Not _CalculBuoy2.CheckCalcul Then
        '    If RetourCalcul.TirantEau / _DrawingBuoy1.HauteurMax > 1 Then
        '        MakeWaterLevel(WaterLevel2, BuoyDrawing1.ActualHeight)
        '        BackgroundWaterLevel2.Margin = New Thickness(0, 0, 0, BuoyDrawing1.ActualHeight)
        '    Else
        '        MakeWaterLevel(WaterLevel2, BuoyDrawing1.ActualHeight * (1 - (RetourCalcul.TirantEau / _DrawingBuoy1.HauteurMax)))
        '        BackgroundWaterLevel2.Margin = New Thickness(0, 0, 0, BuoyDrawing1.ActualHeight * (RetourCalcul.TirantEau / _DrawingBuoy1.HauteurMax))
        '    End If
        'End If

    End Sub

    Private Sub _CalculBuoy2_RetourCalcul(RetourCalcul As CCalculMouillage.STR_RET_CALCUL_RESULTAT)

        _RetourCalcul2 = RetourCalcul
        RefreshGraphics()

        'If RetourCalcul.TirantEau / _DrawingBuoy2.HauteurMax > 1 Then
        '    MakeWaterLevel(WaterLevel2, BuoyDrawing2.ActualHeight)
        '    BackgroundWaterLevel2.Margin = New Thickness(0, 0, 0, BuoyDrawing2.ActualHeight)
        'Else
        '    MakeWaterLevel(WaterLevel2, BuoyDrawing2.ActualHeight * (1 - (RetourCalcul.TirantEau / _DrawingBuoy2.HauteurMax)))
        '    BackgroundWaterLevel2.Margin = New Thickness(0, 0, 0, BuoyDrawing2.ActualHeight * (RetourCalcul.TirantEau / _DrawingBuoy2.HauteurMax))
        'End If

    End Sub

    Private Sub MakeWaterLevel(ByVal Bd As Border, ByVal y As Double)

        Bd.Margin = New Thickness(0, y, 0, 0)

    End Sub

    Private Sub ViewRadioButtonPIC_Checked(sender As System.Object, e As System.Windows.RoutedEventArgs)

        If _CalculBuoy1 Is Nothing Then Return
        If _CalculBuoy2 Is Nothing Then Return

        If ViewRadioButtonPIC.IsChecked Then
            CalculPIC(_CalculBuoy1)
            _CalculBuoy1.DoCalcul()
            CalculPIC(_CalculBuoy2)
            If _CalculBuoy2.CheckCalcul Then _CalculBuoy2.DoCalcul()
        ElseIf ViewRadioButtonFullLoad.IsChecked Then
            CalculFull(_CalculBuoy1)
            _CalculBuoy1.DoCalcul()
            CalculFull(_CalculBuoy2)
            If _CalculBuoy2.CheckCalcul Then _CalculBuoy2.DoCalcul()
        End If

    End Sub

    Private Sub RefreshGraphics()

        If _DrawingBuoy1 IsNot Nothing AndAlso _DrawingBuoy2 Is Nothing Then

            BuoyDrawing1.Width = GridDessin.ActualWidth / 2
            BuoyDrawing1.Height = GridDessin.ActualHeight

            If _RetourCalcul1.TirantEau / _DrawingBuoy1.HauteurMax > 1 Then
                MakeWaterLevel(WaterLevel, 0)
                BackgroundWaterLevel.Margin = New Thickness(0, 0, 0, GridDessin.ActualHeight)
            Else
                MakeWaterLevel(WaterLevel, GridDessin.ActualHeight * (1 - (_RetourCalcul1.TirantEau / _DrawingBuoy1.HauteurMax)))
                BackgroundWaterLevel.Margin = New Thickness(0, 0, 0, GridDessin.ActualHeight * (_RetourCalcul1.TirantEau / _DrawingBuoy1.HauteurMax))
            End If

        ElseIf _DrawingBuoy2 IsNot Nothing Then

            DessinBoueeEchelle()

            If IsBouee1SousLeau() And IsBouee2SousLeau() Then

                FullWaterLevel()

                BuoyDrawing1.Margin = New Thickness(0, 0, 0, 0)
                BuoyDrawing2.Margin = New Thickness(0, 0, 0, 0)

            ElseIf IsBouee1SousLeau() Then

                WaterLevelBouee2()
                SetBouee1SousLeau()

            ElseIf IsBouee2SousLeau() Then

                WaterLevelBouee1()
                SetBouee2SousLeau()

            Else

                If _DrawingBuoy1.HauteurMax >= _DrawingBuoy2.HauteurMax Then

                    WaterLevelBouee1()
                    SetBouee2ParRapportBouee1()

                Else

                    WaterLevelBouee2()
                    SetBouee1ParRapportBouee2()

                End If

            End If

        End If

    End Sub

    Private Sub DessinBoueeEchelle()

        If _DrawingBuoy1.HauteurMax >= _DrawingBuoy2.HauteurMax Then
            BuoyDrawing1.Width = GridDessin.ActualWidth / 2
            BuoyDrawing1.Height = GridDessin.ActualHeight

            BuoyDrawing2.Width = BuoyDrawing1.Width * (_DrawingBuoy2.LargeurMax / _DrawingBuoy1.LargeurMax)
            BuoyDrawing2.Height = BuoyDrawing1.Height * (_DrawingBuoy2.HauteurMax / _DrawingBuoy1.HauteurMax)
        Else
            BuoyDrawing2.Width = GridDessin.ActualWidth / 2
            BuoyDrawing2.Height = GridDessin.ActualHeight

            BuoyDrawing1.Width = BuoyDrawing2.Width * (_DrawingBuoy1.LargeurMax / _DrawingBuoy2.LargeurMax)
            BuoyDrawing1.Height = BuoyDrawing2.Height * (_DrawingBuoy1.HauteurMax / _DrawingBuoy2.HauteurMax)
        End If

    End Sub

    Private Function IsBouee1SousLeau() As Boolean

        If _RetourCalcul1.TirantEau / _DrawingBuoy1.HauteurMax > 1 Then Return True

        Return False

    End Function

    Private Function IsBouee2SousLeau() As Boolean

        If _RetourCalcul2.TirantEau / _DrawingBuoy2.HauteurMax > 1 Then Return True

        Return False

    End Function

    Private Sub FullWaterLevel()

        MakeWaterLevel(WaterLevel, GridDessin.ActualHeight)
        BackgroundWaterLevel.Margin = New Thickness(0, 0, 0, GridDessin.ActualHeight)

    End Sub

    Private Sub WaterLevelBouee1()

        MakeWaterLevel(WaterLevel, GridDessin.ActualHeight * (1 - (_RetourCalcul1.TirantEau / _DrawingBuoy1.HauteurMax)))
        BackgroundWaterLevel.Margin = New Thickness(0, 0, 0, GridDessin.ActualHeight * (_RetourCalcul1.TirantEau / _DrawingBuoy1.HauteurMax))

    End Sub

    Private Sub WaterLevelBouee2()

        MakeWaterLevel(WaterLevel, GridDessin.ActualHeight * (1 - (_RetourCalcul2.TirantEau / _DrawingBuoy2.HauteurMax)))
        BackgroundWaterLevel.Margin = New Thickness(0, 0, 0, GridDessin.ActualHeight * (_RetourCalcul2.TirantEau / _DrawingBuoy2.HauteurMax))

    End Sub

    Private Sub SetBouee1SousLeau()

        BuoyDrawing1.Margin = New Thickness(0, WaterLevel.Margin.Top, 0, 0)

    End Sub

    Private Sub SetBouee2SousLeau()

        BuoyDrawing2.Margin = New Thickness(0, WaterLevel.Margin.Top, 0, 0)

    End Sub

    Private Sub SetBouee1ParRapportBouee2()

        Dim DeltaY As Double

        DeltaY = (_DrawingBuoy2.HauteurMax - _DrawingBuoy1.HauteurMax) '/ 2
        DeltaY -= _RetourCalcul2.TirantEau
        DeltaY += _RetourCalcul1.TirantEau
        BuoyDrawing1.Margin = New Thickness(0, BuoyDrawing2.Height * DeltaY / _DrawingBuoy2.HauteurMax, 0, 0)

    End Sub

    Private Sub SetBouee2ParRapportBouee1()

        Dim DeltaY As Double

        DeltaY = (_DrawingBuoy1.HauteurMax - _DrawingBuoy2.HauteurMax)
        DeltaY -= _RetourCalcul1.TirantEau
        DeltaY += _RetourCalcul2.TirantEau
        BuoyDrawing2.Margin = New Thickness(0, BuoyDrawing1.Height * DeltaY / _DrawingBuoy1.HauteurMax, 0, 0)

    End Sub

#Region "Gestion du changement de langue"

    Private Sub ChangeLangueInLayout()

        Dim obj As Object

        ' Verification que la cle du language exist
        If Not LangDictionnary.ContainsKey(LangueAppli) Then Exit Sub

        For Each Cle In LangDictionnary(LangueAppli)
            obj = LayoutRoot.FindName(Cle.Name)
            If obj Is Nothing Then Continue For

            Select Case obj.GetType

                Case GetType(TextBox)
                    obj.Text = Cle.Text

                Case GetType(TextBlock)
                    obj.Text = Cle.Text

                Case GetType(Button)
                    obj.Content = Cle.Text

                Case GetType(ComboBox)
                    LanguePopulateList(obj, Cle.Text)

                Case GetType(RadioButton)
                    obj.Content = Cle.Text

            End Select
        Next

    End Sub

    Private Sub LanguePopulateList(ByVal Cb As ComboBox, ByVal Txt As String)

        Dim index As Integer
        Dim List As String
        Dim rd() As String

        Try

            Cb.Items.Clear()
            Integer.TryParse(Txt.Split("|")(1), index)
            List = Txt.Split("|")(0)
            rd = List.Substring(1, List.Length - 2).Split(",")

            For Each ligne In rd
                Cb.Items.Add(ligne)
            Next

            Cb.SelectedIndex = index - 1
        Catch
        End Try

    End Sub

    Private Function GetTextLangue(ByVal Name As String, ByVal DefaultText As String) As String

        ' Verification que la cle du language exist
        If Not LangDictionnary.ContainsKey(LangueAppli) Then Return DefaultText

        For Each Cle In LangDictionnary(LangueAppli)
            If Cle.Name = Name Then
                Return Cle.Text
            End If
        Next

        Return DefaultText

    End Function

#End Region

End Class
