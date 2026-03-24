Imports BuoyLib.Buoy
Imports System.Windows.Controls.Ribbon
Imports System.Data

Public Class FenetreBouee

    Private _Buoy As CBouee = Nothing
    Const DimVisualisationH As Integer = 480
    Const DimVisualisationW As Integer = 280

    Private UpdateTextModif As Boolean = False

#Region "Gestion des evenements la fenetre"

    Public Sub New()

        ' Cet appel est requis par le concepteur.
        InitializeComponent()

        ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().

    End Sub

    Private Sub RetourOK(sender As Object, e As System.Windows.RoutedEventArgs)

        Me.DialogResult = True

    End Sub

    Private Sub RetourCancel(sender As Object, e As System.Windows.RoutedEventArgs)

        Me.DialogResult = False

    End Sub

    Private Sub FenetreBouee_Closing(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles Me.Closing

        If SelectMenu() <> SELECT_MENU.GeneralTab Then
            'If DisplayDemande(GetTextLangue("InfoBulleDemandeSauvegarde", "Voulez-vous sauvegarder ?")) Then
            '    Me.DialogResult = True
            'Else
            '    Me.DialogResult = False
            'End If
        End If

    End Sub

    Private Sub FenetreBouee_Loaded(sender As Object, e As System.Windows.RoutedEventArgs) Handles Me.Loaded

        Title = "Calmar " + Version
        Icon = GetIconCalmar()

        ChangeLangueInLayout()
        Me.SetValue(Window.TopProperty, Me.GetValue(Window.TopProperty) + 30)
        RefreshBuoyDisplay()

    End Sub

    Enum SELECT_MENU

        NoMenuSelected = -1
        GeneralTab = 0
        FlotteurTab = 1
        StructureTab = 2
        PyloneTab = 3
        VoyantTab = 4

    End Enum

    Private Function SelectMenu() As SELECT_MENU

        If MenuBouee.SelectedItem Is ManagerBuoyGeneralTab Then Return SELECT_MENU.GeneralTab
        If MenuBouee.SelectedItem Is ManagerBuoyFlotteurTab Then Return SELECT_MENU.FlotteurTab
        If MenuBouee.SelectedItem Is ManagerBuoyStructureTab Then Return SELECT_MENU.StructureTab
        If MenuBouee.SelectedItem Is ManagerBuoyPyloneTab Then Return SELECT_MENU.PyloneTab
        If MenuBouee.SelectedItem Is ManagerBuoyVoyantTab Then Return SELECT_MENU.VoyantTab

        Return SELECT_MENU.NoMenuSelected

    End Function

    Private Sub RefreshBuoyDisplay()

        If ListBoxElement Is Nothing Or VisuBuoy Is Nothing Then Return

        GridBouee.Visibility = Windows.Visibility.Collapsed
        GridFlotteur.Visibility = Windows.Visibility.Collapsed
        GridStructure.Visibility = Windows.Visibility.Collapsed
        GridPylone.Visibility = Windows.Visibility.Collapsed
        GridVoyant.Visibility = Windows.Visibility.Collapsed

        OrganeauView.Children.Clear()

        Select Case SelectMenu()

            Case SELECT_MENU.GeneralTab
                GridBouee.Visibility = Windows.Visibility.Visible
                BuoyDisplay()

            Case SELECT_MENU.FlotteurTab
                GridFlotteur.Visibility = Windows.Visibility.Visible
                FloatDisplay()

            Case SELECT_MENU.StructureTab
                GridStructure.Visibility = Windows.Visibility.Visible
                StructureDisplay()

            Case SELECT_MENU.PyloneTab
                GridPylone.Visibility = Windows.Visibility.Visible
                PyloneDisplay()

            Case SELECT_MENU.VoyantTab
                GridVoyant.Visibility = Windows.Visibility.Visible
                VoyantDisplay()

        End Select

        RefreshText()

    End Sub

    Private Sub BuoyDisplay()

        'GestionMouvementElement.Visibility = Windows.Visibility.Collapsed
        ListBoxElement.Visibility = Windows.Visibility.Collapsed
        VisuBuoy.Visibility = Windows.Visibility.Visible

        If Buoy IsNot Nothing Then

            TextBoxNomBouee.Text = Buoy.Nom
            VisuBuoy.Child = New CBuoyDrawing(Buoy)
            RefreshListLest()
            RefreshChainStackPanel()
        End If

    End Sub

    Private Sub FloatDisplay()

        Dim w As Double = 0.0
        Dim h As Double = 0.0
        Dim Ratio As Double

        Dim WRatio As Double = DimVisualisationW
        Dim HRatio As Double = DimVisualisationH

        Dim p0 As Integer

        VisuBuoy.Visibility = Windows.Visibility.Collapsed
        ListBoxElement.Visibility = Windows.Visibility.Visible
        'GestionMouvementElement.Visibility = Windows.Visibility.Visible

        If Buoy IsNot Nothing AndAlso Buoy.FlotteurBouee IsNot Nothing Then

            For p0 = 0 To Buoy.FlotteurBouee.Elements.Count - 1
                If w < Buoy.FlotteurBouee.Elements(p0).LongueurMaxElement Then w = Buoy.FlotteurBouee.Elements(p0).LongueurMaxElement
                h += Buoy.FlotteurBouee.Elements(p0).HauteurElement
            Next

            ' Recuperation du ratio
            Ratio = w / h
            CheckSize(WRatio, HRatio, Ratio, DimVisualisationW, DimVisualisationH)
            'If Ratio < 1 Then
            '    WRatio = HRatio * Ratio
            'Else
            '    HRatio = WRatio / Ratio
            'End If

            ' Affichage de la liste
            ListBoxElement.Items.Clear()
            For p0 = (Buoy.FlotteurBouee.Elements.Count - 1) To 0 Step -1
                ListBoxElement.Items.Add(Buoy.FlotteurBouee.Elements(p0).DrawCanvas(New Size(Buoy.FlotteurBouee.Elements(p0).LongueurMaxElement * WRatio / w, Buoy.FlotteurBouee.Elements(p0).HauteurElement * HRatio / h)))
            Next p0

        End If

        ' Raffraichissement des elements pour le remplacement
        MenuCopieFlotteur.Items.Clear()
        For Each itBuoy In BaseBouees.GetBouees
            'If itBuoy.Value.Nom = Buoy.Nom Then Continue For
            If itBuoy.Value.FlotteurBouee.Nom = "" Then Continue For
            MenuCopieFlotteur.Items.Add(CreateMenuFlotteur(itBuoy.Value))
        Next

        FlotteurNoSelection()

    End Sub

    Private Sub StructureDisplay()

        Dim w As Double = 0.0
        Dim h As Double = 0.0
        Dim Ratio As Double

        Dim WRatio As Double = DimVisualisationW
        Dim HRatio As Double = DimVisualisationH

        Dim CircleOrganeau As New Ellipse
        Dim MyHauteurMax As Double

        Dim p0 As Integer

        VisuBuoy.Visibility = Windows.Visibility.Collapsed
        ListBoxElement.Visibility = Windows.Visibility.Visible
        'GestionMouvementElement.Visibility = Windows.Visibility.Visible

        If Buoy IsNot Nothing AndAlso Buoy.StructureBouee IsNot Nothing Then

            For p0 = 0 To Buoy.StructureBouee.Elements.Count - 1
                If w < Buoy.StructureBouee.Elements(p0).LongueurMaxElement Then w = Buoy.StructureBouee.Elements(p0).LongueurMaxElement
                h += Buoy.StructureBouee.Elements(p0).HauteurElement
            Next

            ' Recuperation du ratio
            Ratio = w / h
            CheckSize(WRatio, HRatio, Ratio, DimVisualisationW, DimVisualisationH)

            ' Affichage de la liste
            ListBoxElement.Items.Clear()
            For p0 = (Buoy.StructureBouee.Elements.Count - 1) To 0 Step -1
                ListBoxElement.Items.Add(Buoy.StructureBouee.Elements(p0).DrawCanvas(New Size(Buoy.StructureBouee.Elements(p0).LongueurMaxElement * WRatio / w, Buoy.StructureBouee.Elements(p0).HauteurElement * HRatio / h)))
            Next p0

            If Buoy.StructureBouee.Elements.Count > 0 Then
                OrganeauView.Width = ListBoxElement.ActualWidth
                OrganeauView.Height = Buoy.StructureBouee.HauteurMax * HRatio / h + 6 * Buoy.StructureBouee.Elements.Count

                MyHauteurMax = Buoy.StructureBouee.OffsetFlotteur - Buoy.StructureBouee.OffsetOrganeau

                '' Dessin de l'organeau
                OrganeauView.Children.Clear()
                'OrganeauView.Children.Add(New Border() With {.BorderThickness = New Thickness(1), .BorderBrush = New SolidColorBrush(Colors.Red), .Width = OrganeauView.Width, .Height = OrganeauView.Height})

                CircleOrganeau.Stroke = New SolidColorBrush(Colors.Black)
                CircleOrganeau.Width = (_Buoy.ChaineMax / 1000) * OrganeauView.Width / Buoy.StructureBouee.LargeurMax
                CircleOrganeau.Height = (_Buoy.ChaineMax / 1000) * OrganeauView.Width / Buoy.StructureBouee.LargeurMax
                CircleOrganeau.SetValue(Canvas.TopProperty, OrganeauView.Height - (MyHauteurMax * OrganeauView.Height / Buoy.StructureBouee.HauteurMax) - (CircleOrganeau.Height) / 2)
                CircleOrganeau.SetValue(Canvas.LeftProperty, (OrganeauView.Width - CircleOrganeau.Width) / 2)
                OrganeauView.Children.Add(CircleOrganeau)
            End If
        End If

        ' Raffraichissement des elements pour le remplacement
        MenuCopieStructure.Items.Clear()
        For Each itBuoy In BaseBouees.GetBouees
            'If itBuoy.Value.Nom = Buoy.Nom Then Continue For
            If itBuoy.Value.StructureBouee.Nom = "" Then Continue For
            MenuCopieStructure.Items.Add(CreateMenuStructure(itBuoy.Value))
        Next

        StructureNoSelection()

    End Sub

    Private Sub PyloneDisplay()

        Dim w As Double = 0.0
        Dim h As Double = 0.0
        Dim Ratio As Double

        Dim WRatio As Double = DimVisualisationW
        Dim HRatio As Double = DimVisualisationH

        Dim p0 As Integer

        VisuBuoy.Visibility = Windows.Visibility.Collapsed
        ListBoxElement.Visibility = Windows.Visibility.Visible
        'GestionMouvementElement.Visibility = Windows.Visibility.Visible

        If Buoy IsNot Nothing AndAlso Buoy.PyloneBouee IsNot Nothing Then

            For p0 = 0 To Buoy.PyloneBouee.Count - 1
                If w < Buoy.PyloneBouee(p0).Element.LongueurMaxElement Then w = Buoy.PyloneBouee(p0).Element.LongueurMaxElement
                h += Buoy.PyloneBouee(p0).Element.HauteurElement
            Next

            ' Recuperation du ratio
            Ratio = w / h
            CheckSize(WRatio, HRatio, Ratio, DimVisualisationW, DimVisualisationH)
            'If Ratio < 1 Then
            '    WRatio = HRatio * Ratio
            'Else
            '    HRatio = WRatio / Ratio
            'End If

            ' Affichage de la liste
            ListBoxElement.Items.Clear()
            For p0 = (Buoy.PyloneBouee.Count - 1) To 0 Step -1
                ListBoxElement.Items.Add(Buoy.PyloneBouee(p0).Element.DrawCanvas(New Size(Buoy.PyloneBouee(p0).Element.LongueurMaxElement * WRatio / w, Buoy.PyloneBouee(p0).Element.HauteurElement * HRatio / h)))
            Next p0

        End If

        ' Raffraichissement des elements pour le remplacement
        MenuCopiePylone.Items.Clear()
        For Each itBuoy In BaseBouees.GetBouees
            'If itBuoy.Value.Nom = Buoy.Nom Then Continue For
            If itBuoy.Value.PyloneBouee.Length <= 0 Then Continue For
            MenuCopiePylone.Items.Add(CreateMenuPylone(itBuoy.Value))
        Next

        PyloneNoSelection()

    End Sub

    Private Sub VoyantDisplay()

        Dim w As Double = 0.0
        Dim h As Double = 0.0
        Dim Ratio As Double

        Dim WRatio As Double = DimVisualisationW
        Dim HRatio As Double = DimVisualisationH

        Dim p0 As Integer

        VisuBuoy.Visibility = Windows.Visibility.Collapsed
        ListBoxElement.Visibility = Windows.Visibility.Visible
        'GestionMouvementElement.Visibility = Windows.Visibility.Visible

        If Buoy IsNot Nothing AndAlso Buoy.EquipementBouee IsNot Nothing Then

            For p0 = 0 To Buoy.EquipementBouee.Count - 1
                If w < Buoy.EquipementBouee(p0).Element.LongueurMaxElement Then w = Buoy.EquipementBouee(p0).Element.LongueurMaxElement
                h += Buoy.EquipementBouee(p0).Element.HauteurElement
            Next

            ' Recuperation du ratio
            Ratio = w / h
            CheckSize(WRatio, HRatio, Ratio, DimVisualisationW, DimVisualisationH)
            'If Ratio < 1 Then
            '    WRatio = HRatio * Ratio
            'Else
            '    HRatio = WRatio / Ratio
            'End If

            ' Affichage de la liste
            ListBoxElement.Items.Clear()
            For p0 = (Buoy.EquipementBouee.Count - 1) To 0 Step -1
                ListBoxElement.Items.Add(Buoy.EquipementBouee(p0).Element.DrawCanvas(New Size(Buoy.EquipementBouee(p0).Element.LongueurMaxElement * WRatio / w, Buoy.EquipementBouee(p0).Element.HauteurElement * HRatio / h)))
            Next p0

        End If

        ' Raffraichissement des elements pour le remplacement
        MenuCopieVoyant.Items.Clear()
        For Each itBuoy In BaseBouees.GetBouees
            'If itBuoy.Value.Nom = Buoy.Nom Then Continue For
            If itBuoy.Value.EquipementBouee.Length <= 0 Then Continue For
            MenuCopieVoyant.Items.Add(CreateMenuVoyant(itBuoy.Value))
        Next

        VoyantNoSelection()

    End Sub

    Private Sub RefreshText()

        Select Case SelectMenu()

            Case SELECT_MENU.GeneralTab
                TextBoxNomBouee.Text = Buoy.Nom
                TextBoxBoueeMasse.Text = Math.Round(Buoy.MASSE_BOUEE, 1).ToString(EnglishFormat)
                TextBoxBoueeVolume.Text = Math.Round(Buoy.VOLUME, 1).ToString(EnglishFormat)
                TextBoxOffsetFlotteur.Text = Buoy.StructureBouee.OffsetFlotteur.ToString(EnglishFormat)

            Case SELECT_MENU.FlotteurTab
                If Buoy.FlotteurBouee IsNot Nothing Then
                    TextBoxNomFlotteur.Text = Buoy.FlotteurBouee.Nom
                    TextBoxFlotteurVolume.Text = Math.Round(Buoy.FlotteurBouee.Volume, 4).ToString(EnglishFormat)
                    TextBoxFlotteurMasse.Text = Buoy.FlotteurBouee.Masse.ToString(EnglishFormat)
                End If

            Case SELECT_MENU.StructureTab
                If Buoy.StructureBouee IsNot Nothing Then
                    TextBoxNomStructure.Text = Buoy.StructureBouee.Nom
                    TextBoxStructureMasse.Text = Buoy.StructureBouee.Masse.ToString(EnglishFormat)
                    TextBoxStructureVolume.Text = Buoy.StructureBouee.Volume.ToString(EnglishFormat)
                    TextBoxStructureOffset.Text = Buoy.StructureBouee.OffsetOrganeau.ToString(EnglishFormat)
                End If

            Case SELECT_MENU.PyloneTab
                TextBoxNomPylone.Text = ""

            Case SELECT_MENU.VoyantTab
                TextBoxNomVoyant.Text = ""

        End Select

    End Sub

    Private Sub CheckSize(ByRef WRatio As Double, ByRef HRatio As Double, ByVal ratio As Double, ByVal MaxW As Double, ByVal MaxH As Double)

        Dim delta As Integer = 0

        ' Recuperation du ratio
        Do
            If ratio < 1 Then
                WRatio = (MaxH + delta) * ratio
            Else
                HRatio = (MaxW + delta) / ratio
            End If
            delta -= 1
        Loop While (WRatio > MaxW Or HRatio > MaxH)

    End Sub

    Property Buoy As CBouee
        Get
            Return _Buoy
        End Get
        Set(value As CBouee)
            _Buoy = value
        End Set
    End Property

    Private Sub TextBoxNom_TextChanged(sender As System.Object, e As System.Windows.Controls.TextChangedEventArgs) _
        Handles TextBoxNomBouee.TextChanged, _
                TextBoxNomFlotteur.TextChanged, _
                TextBoxNomStructure.TextChanged, _
                TextBoxNomPylone.TextChanged, _
                TextBoxNomVoyant.TextChanged

        Dim TextBoxNom As TextBox = sender

        TextBoxNom.Text = TextBoxNom.Text.Replace("\", "")
        TextBoxNom.Text = TextBoxNom.Text.Replace("/", "")
        TextBoxNom.Text = TextBoxNom.Text.Replace(":", "")
        TextBoxNom.Text = TextBoxNom.Text.Replace("*", "")
        TextBoxNom.Text = TextBoxNom.Text.Replace("?", "")
        TextBoxNom.Text = TextBoxNom.Text.Replace("""", "")
        TextBoxNom.Text = TextBoxNom.Text.Replace("<", "")
        TextBoxNom.Text = TextBoxNom.Text.Replace(">", "")
        TextBoxNom.Text = TextBoxNom.Text.Replace("|", "")

        Select Case SelectMenu()

            Case SELECT_MENU.GeneralTab
                Buoy.Nom = TextBoxNom.Text

            Case SELECT_MENU.FlotteurTab
                Buoy.FlotteurBouee.Nom = TextBoxNom.Text

            Case SELECT_MENU.StructureTab
                Buoy.StructureBouee.Nom = TextBoxNom.Text

            Case SELECT_MENU.PyloneTab
                ModificationNomPylone()

            Case SELECT_MENU.VoyantTab
                ModificationNomVoyant()

        End Select

    End Sub

    Private Sub Ribbon_SelectionChanged(sender As System.Object, e As System.Windows.Controls.SelectionChangedEventArgs)

        RefreshBuoyDisplay()

    End Sub

    ' Non affichage de la popup de minimize
    Private Sub MenuRibbon_ContextMenuOpening(sender As System.Object, e As System.Windows.Controls.ContextMenuEventArgs)
        e.Handled = True
    End Sub

    Private Sub MenuRibbon_SizeChanged(sender As System.Object, e As System.Windows.SizeChangedEventArgs)

        Dim RibMenu As Ribbon = sender

        If RibMenu IsNot Nothing Then
            RibMenu.IsMinimized = False
        End If

        e.Handled = True

    End Sub

    Private Sub TextBox_GotKeyboardFocus(sender As System.Object, e As System.Windows.Input.KeyboardFocusChangedEventArgs)

        sender.SelectAll()

    End Sub

    Private Sub TextBox_GotMouseCapture(sender As System.Object, e As System.Windows.Input.MouseEventArgs)

        sender.SelectAll()

    End Sub

    Private Sub Button_Click(sender As System.Object, e As System.Windows.RoutedEventArgs)

        Dim bt As Button = sender

        If bt.ContextMenu Is Nothing OrElse bt.ContextMenu.Items.Count = 0 Then Return

        bt.ContextMenu.IsEnabled = True
        bt.ContextMenu.PlacementTarget = bt
        bt.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom
        bt.ContextMenu.IsOpen = True

    End Sub

    Private Sub Window_KeyUp(sender As System.Object, e As System.Windows.Input.KeyEventArgs) Handles MyBase.KeyUp

        If e.Key = Key.Escape Then
            Me.DialogResult = False
        End If

    End Sub

    Private Sub TextBox_KeyUp(sender As Object, e As KeyEventArgs) _
        Handles TextBoxOffsetFlotteur.KeyUp, TextBoxBoueeMasse.KeyUp, TextBoxBoueeVolume.KeyUp,
        TextBoxFlotteurMasse.KeyUp, TextBoxFlotteurVolume.KeyUp, TextBoxFloatHauteur.KeyUp,
        TextBoxFloatDiametreH.KeyUp, TextBoxFloatDiametreI.KeyUp, TextBoxFloatDiametreB.KeyUp,
        TextBoxFloatVolume.KeyUp, TextBoxStructureMasse.KeyUp, TextBoxStructureOffset.KeyUp,
        TextBoxStructureVolume.KeyUp, TextBoxStructHauteur.KeyUp, TextBoxStructDiametreH.KeyUp,
        TextBoxStructDiametreI.KeyUp, TextBoxStructDiametreB.KeyUp, TextBoxStructVolume.KeyUp,
        TextBoxPyloneMasse.KeyUp, TextBoxPyloneHauteur.KeyUp, TextBoxPyloneDiametreH.KeyUp,
        TextBoxPyloneDiametreB.KeyUp, TextBoxVoyantMasse.KeyUp, TextBoxVoyantHauteur.KeyUp,
        TextBoxVoyantDiametreH.KeyUp, TextBoxVoyantDiametreB.KeyUp

        If e.Key = Key.Return Then
            Try
                Dim d As Double = CalcEquation(sender.text)
                sender.text = d.ToString(EnglishFormat)
            Catch
                MessageBox.Show("Error mathematic calculation")
            End Try
        End If

    End Sub

    Private Function CalcEquation(ByVal equation As String) As Double

        Dim tempTable As New DataTable()
        Dim result As Object = tempTable.Compute(equation, "")

        Return result

    End Function


#End Region

#Region "Gestion des modification sur la bouée"

#Region "Gestion de l'ajout et de la suppression des lests"

    Private Sub RefreshListLest()

        If Buoy.NombreLestMin > 0 Then ComboBoxNombreLestMin.SelectedIndex = Buoy.NombreLestMin
        If Buoy.NombreLestMax > 0 Then ComboBoxNombreLestMax.SelectedIndex = Buoy.NombreLestMax

        Select Case Buoy.MasseLestUnitaire

            Case 2.5
                ComboBoxPoidsLest.SelectedIndex = 1
            Case 5
                ComboBoxPoidsLest.SelectedIndex = 2
            Case 10
                ComboBoxPoidsLest.SelectedIndex = 3
            Case 15
                ComboBoxPoidsLest.SelectedIndex = 4
            Case 20
                ComboBoxPoidsLest.SelectedIndex = 5
            Case 25
                ComboBoxPoidsLest.SelectedIndex = 6
            Case 35
                ComboBoxPoidsLest.SelectedIndex = 7
            Case 40
                ComboBoxPoidsLest.SelectedIndex = 8
            Case 50
                ComboBoxPoidsLest.SelectedIndex = 9
            Case 75
                ComboBoxPoidsLest.SelectedIndex = 10
            Case 100
                ComboBoxPoidsLest.SelectedIndex = 11

        End Select

    End Sub

    Private Sub ComboBoxNombreLestMin_SelectionChanged(sender As System.Object, e As System.Windows.Controls.SelectionChangedEventArgs)

        If Buoy IsNot Nothing Then
            Buoy.NombreLestMin = ComboBoxNombreLestMin.SelectedIndex
            If Buoy.NombreLestMax < Buoy.NombreLestMin Then
                ComboBoxNombreLestMax.SelectedIndex = Buoy.NombreLestMin
            End If
        End If

    End Sub

    Private Sub ComboBoxNombreLestMax_SelectionChanged(sender As System.Object, e As System.Windows.Controls.SelectionChangedEventArgs)

        If Buoy IsNot Nothing Then
            Buoy.NombreLestMax = ComboBoxNombreLestMax.SelectedIndex
            If Buoy.NombreLestMax < Buoy.NombreLestMin Then
                ComboBoxNombreLestMin.SelectedIndex = Buoy.NombreLestMax
            End If
        End If

    End Sub

    Private Sub ComboBoxPoidsLest_SelectionChanged(sender As System.Object, e As System.Windows.Controls.SelectionChangedEventArgs)

        If Buoy IsNot Nothing Then
            Select Case ComboBoxPoidsLest.SelectedIndex

                Case 1
                    Buoy.MasseLestUnitaire = 2.5
                Case 2
                    Buoy.MasseLestUnitaire = 5
                Case 3
                    Buoy.MasseLestUnitaire = 10
                Case 4
                    Buoy.MasseLestUnitaire = 15
                Case 5
                    Buoy.MasseLestUnitaire = 20
                Case 6
                    Buoy.MasseLestUnitaire = 25
                Case 7
                    Buoy.MasseLestUnitaire = 35
                Case 8
                    Buoy.MasseLestUnitaire = 40
                Case 9
                    Buoy.MasseLestUnitaire = 50
                Case 10
                    Buoy.MasseLestUnitaire = 75
                Case 11
                    Buoy.MasseLestUnitaire = 100

            End Select
        End If

    End Sub

#End Region

#Region "Gestion de la selection des chaines"

    Private Sub RefreshChainStackPanel()

        For p0 = 0 To ComboBoxChaineMin.Items.Count - 1
            If ComboBoxChaineMin.Items(p0).Content = Buoy.ChaineMin.ToString Then
                ComboBoxChaineMin.SelectedIndex = p0
                Exit For
            End If
        Next
        For p0 = 0 To ComboBoxChaineMax.Items.Count - 1
            If ComboBoxChaineMax.Items(p0).Content = Buoy.ChaineMax.ToString Then
                ComboBoxChaineMax.SelectedIndex = p0
                Exit For
            End If
        Next

    End Sub

    Private Sub ComboBoxChaine_SelectionChanged(sender As System.Object, e As System.Windows.Controls.SelectionChangedEventArgs)

        Dim d As Double

        If Double.TryParse(sender.SelectedItem.Content, d) Then
            If sender Is ComboBoxChaineMin Then
                Buoy.ChaineMin = d
            ElseIf sender Is ComboBoxChaineMax Then
                Buoy.ChaineMax = d
            End If
        End If

    End Sub

#End Region

    Private Sub ButtonDeplacerFlotteurAuDessus_Click(sender As System.Object, e As System.Windows.RoutedEventArgs)

        Dim d As Double

        Double.TryParse(TextBoxPasDeplacement.Text, Globalization.NumberStyles.Number, EnglishFormat, d)

        If d > 0 Then
            _Buoy.StructureBouee.OffsetFlotteur += d
            RefreshBuoyDisplay()
        End If

    End Sub

    Private Sub ButtonDeplacerFlotteurAuDessous_Click(sender As System.Object, e As System.Windows.RoutedEventArgs)

        Dim d As Double

        Double.TryParse(TextBoxPasDeplacement.Text, Globalization.NumberStyles.Number, EnglishFormat, d)

        If d > 0 Then
            _Buoy.StructureBouee.OffsetFlotteur -= d
            RefreshBuoyDisplay()
        End If

    End Sub

    Private Sub TextBoxOffsetFlotteur_TextChanged(sender As System.Object, e As System.Windows.Controls.TextChangedEventArgs)

        Dim d As Double

        Double.TryParse(TextBoxOffsetFlotteur.Text, Globalization.NumberStyles.Number, EnglishFormat, d)

        If d >= 0 Then
            If _Buoy.StructureBouee.OffsetFlotteur = d Then Exit Sub
            _Buoy.StructureBouee.OffsetFlotteur = d
            BuoyDisplay()
        End If

    End Sub

#End Region

#Region "Gestion de la selection d'un modele"

    Private Sub Border_MouseLeftButtonDown(sender As System.Object, e As System.Windows.Input.MouseButtonEventArgs)

        Select Case SelectMenu()

            Case SELECT_MENU.FlotteurTab
                If Not (ListBoxElement.SelectedIndex < 0) Then ListBoxElement.SelectedIndex = -1

            Case SELECT_MENU.StructureTab
                If Not (ListBoxElement.SelectedIndex < 0) Then ListBoxElement.SelectedIndex = -1

            Case SELECT_MENU.PyloneTab
                If Not (ListBoxElement.SelectedIndex < 0) Then ListBoxElement.SelectedIndex = -1

            Case SELECT_MENU.VoyantTab
                If Not (ListBoxElement.SelectedIndex < 0) Then ListBoxElement.SelectedIndex = -1

        End Select

    End Sub

    Private Sub ListBoxElement_SelectionChanged(sender As System.Object, e As System.Windows.Controls.SelectionChangedEventArgs)

        Select Case SelectMenu()

            Case SELECT_MENU.FlotteurTab
                SelectionElementFlotteur()

            Case SELECT_MENU.StructureTab
                SelectionElementStructure()

            Case SELECT_MENU.PyloneTab
                SelectionElementPylone()

            Case SELECT_MENU.VoyantTab
                SelectionElementVoyant()

        End Select

    End Sub

#End Region

#Region "Gestion de l'ajout ou de la modification du flotteur"

    Private Sub SelectionElementFlotteur()

        Dim IndexFloat As Integer

        ManagerBuoyButtonFlotteurDeplacerElementAuDessus.IsEnabled = False
        ManagerBuoyButtonFlotteurDeplacerElementAuDessous.IsEnabled = False

        If ListBoxElement.SelectedIndex < 0 Then
            FlotteurNoSelection()
            Return
        End If

        ManagerBuoyButtonNouveauFlotteur.IsChecked = False
        ManagerBuoyButtonModificationFlotteur.IsChecked = True

        ManagerBuoyButtonAjouterFlotteur.IsEnabled = False
        ManagerBuoyButtonSupprimerFlotteur.IsEnabled = True
        If ListBoxElement.SelectedIndex > 0 Then ManagerBuoyButtonFlotteurDeplacerElementAuDessus.IsEnabled = True
        If ListBoxElement.SelectedIndex < (Buoy.FlotteurBouee.Elements.Count - 1) Then ManagerBuoyButtonFlotteurDeplacerElementAuDessous.IsEnabled = True

        IndexFloat = Buoy.FlotteurBouee.Elements.Count - ListBoxElement.SelectedIndex - 1
        DessineElementFlotteurSelectionner.Child = Buoy.FlotteurBouee.ElementAt(IndexFloat).DrawCanvasByHeight(DessineElementFlotteurSelectionner.ActualWidth, DessineElementFlotteurSelectionner.ActualHeight)

        TextBoxFlotteurVolume.Text = Buoy.FlotteurBouee.Volume.ToString(EnglishFormat)
        TextBoxFloatVolumeCalcul.Text = Math.Round(Buoy.FlotteurBouee.ElementAt(IndexFloat).VolumeCalcule, 3).ToString(EnglishFormat)

        If UpdateTextModif Then Return
        UpdateTextModif = True
        TextBoxFloatHauteur.Text = Buoy.FlotteurBouee.ElementAt(IndexFloat).HauteurElement.ToString(EnglishFormat)
        TextBoxFloatDiametreH.Text = Buoy.FlotteurBouee.ElementAt(IndexFloat).DiameterHigh.ToString(EnglishFormat)
        TextBoxFloatDiametreI.Text = Buoy.FlotteurBouee.ElementAt(IndexFloat).DiameterInter.ToString(EnglishFormat)
        TextBoxFloatDiametreB.Text = Buoy.FlotteurBouee.ElementAt(IndexFloat).DiameterLow.ToString(EnglishFormat)
        TextBoxFloatVolume.Text = Buoy.FlotteurBouee.ElementAt(IndexFloat).VolumeReel.ToString(EnglishFormat)
        UpdateTextModif = False

    End Sub

    Private Sub FlotteurNoSelection()

        ManagerBuoyButtonNouveauFlotteur.IsChecked = True
        ManagerBuoyButtonModificationFlotteur.IsChecked = False

        ManagerBuoyButtonFlotteurDeplacerElementAuDessus.IsEnabled = False
        ManagerBuoyButtonFlotteurDeplacerElementAuDessous.IsEnabled = False

        DessineElementFlotteurSelectionner.Child = Nothing
        ManagerBuoyButtonSupprimerFlotteur.IsEnabled = False
        ManagerBuoyButtonAjouterFlotteur.IsEnabled = True

        If UpdateTextModif Then Return
        UpdateTextModif = True
        TextBoxFloatHauteur.Text = "0.0"
        TextBoxFloatDiametreH.Text = "0.0"
        TextBoxFloatDiametreI.Text = "0.0"
        TextBoxFloatDiametreB.Text = "0.0"
        TextBoxFloatVolume.Text = "0.0"
        TextBoxFloatVolumeCalcul.Text = "0.0"
        UpdateTextModif = False

    End Sub

    Private Sub NouveauFlotteur_Click(sender As System.Object, e As System.Windows.RoutedEventArgs)

        Dim TextBoxFloatHauteurText As String = TextBoxFloatHauteur.Text
        Dim TextBoxFloatDiametreHText As String = TextBoxFloatDiametreH.Text
        Dim TextBoxFloatDiametreIText As String = TextBoxFloatDiametreI.Text
        Dim TextBoxFloatDiametreBText As String = TextBoxFloatDiametreB.Text
        Dim TextBoxFloatVolumeText As String = TextBoxFloatVolume.Text

        RefreshBuoyDisplay()

        TextBoxFloatHauteur.Text = TextBoxFloatHauteurText
        TextBoxFloatDiametreH.Text = TextBoxFloatDiametreHText
        TextBoxFloatDiametreI.Text = TextBoxFloatDiametreIText
        TextBoxFloatDiametreB.Text = TextBoxFloatDiametreBText
        TextBoxFloatVolume.Text = TextBoxFloatVolumeText

    End Sub

    Private Sub AddFlotteur_Click(sender As System.Object, e As System.Windows.RoutedEventArgs)

        Dim NewElementFloat As New BuoyLib.CDimensionElementTroncCone

        Try
            NewElementFloat.HauteurElement = Double.Parse(TextBoxFloatHauteur.Text, EnglishFormat)
            NewElementFloat.DiameterHigh = Double.Parse(TextBoxFloatDiametreH.Text, EnglishFormat)
            NewElementFloat.DiameterInter = Double.Parse(TextBoxFloatDiametreI.Text, EnglishFormat)
            NewElementFloat.DiameterLow = Double.Parse(TextBoxFloatDiametreB.Text, EnglishFormat)
            NewElementFloat.VolumeReel = Double.Parse(TextBoxFloatVolume.Text, EnglishFormat)

            Buoy.FlotteurBouee.AddElement(NewElementFloat)
        Catch ex As Exception
            Return
        End Try

        RefreshBuoyDisplay()

        ListBoxElement.SelectedIndex = 0

    End Sub

    Private Sub SupprimerFlotteur_Click(sender As System.Object, e As System.Windows.RoutedEventArgs)

        If ListBoxElement.SelectedIndex < 0 Then Return

        Buoy.FlotteurBouee.RemoveElementAt(Buoy.FlotteurBouee.Elements.Count - ListBoxElement.SelectedIndex - 1)
        RefreshBuoyDisplay()

    End Sub

    Private Sub ButtonFlotteurDeplacerElementAuDessus_Click(sender As System.Object, e As System.Windows.RoutedEventArgs)

        Dim indexFloat As Integer

        If ListBoxElement.SelectedIndex < 0 Then Return
        indexFloat = ListBoxElement.SelectedIndex

        Buoy.FlotteurBouee.MoveElementUp(Buoy.FlotteurBouee.Elements.Count - ListBoxElement.SelectedIndex - 1)

        RefreshBuoyDisplay()

        ListBoxElement.SelectedIndex = indexFloat - 1

    End Sub

    Private Sub ButtonFlotteurDeplacerElementAuDessous_Click(sender As System.Object, e As System.Windows.RoutedEventArgs)

        Dim indexFloat As Integer

        If ListBoxElement.SelectedIndex < 0 Then Return
        indexFloat = ListBoxElement.SelectedIndex

        Buoy.FlotteurBouee.MoveElementDown(Buoy.FlotteurBouee.Elements.Count - ListBoxElement.SelectedIndex - 1)

        RefreshBuoyDisplay()

        ListBoxElement.SelectedIndex = indexFloat + 1

    End Sub

    Private Function CreateMenuFlotteur(ByVal buoy As CBouee) As MenuItem

        Dim SubMenuFloat As MenuItem
        CreateMenuFlotteur = New MenuItem() With {.Header = buoy.Nom}

        SubMenuFloat = New MenuItem() With {.Header = buoy.FlotteurBouee.Nom}
        CreateMenuFlotteur.Items.Add(SubMenuFloat)

        ' Ajout du retour de Remplacement
        AddHandler SubMenuFloat.Click, AddressOf SubMenuFloat_Click

    End Function

    Private Sub SubMenuFloat_Click(sender As Object, e As System.Windows.RoutedEventArgs)

        Dim MenuClicked As MenuItem = sender
        Dim ParentMenuClicked As MenuItem

        If MenuClicked.Parent Is Nothing Then Return

        ParentMenuClicked = MenuClicked.Parent
        If ParentMenuClicked Is Nothing Then Return

        If Not BaseBouees.GetBouees.ContainsKey(ParentMenuClicked.Header) Then Return

        Buoy.FlotteurBouee = BaseBouees.GetBouees(ParentMenuClicked.Header).FlotteurBouee.Clone

        RefreshBuoyDisplay()

    End Sub

    Private Sub TextBoxFlotteurMasse_TextChanged(sender As System.Object, e As System.Windows.Controls.TextChangedEventArgs)

        Try
            Buoy.FlotteurBouee.Masse = Double.Parse(TextBoxFlotteurMasse.Text, EnglishFormat)
        Catch
        End Try

    End Sub

    Private Sub TextBoxFloatHauteur_TextChanged(sender As System.Object, e As System.Windows.Controls.TextChangedEventArgs)

        Dim indexFloat As Integer
        Dim NewElementFloat As BuoyLib.CDimensionElementTroncCone

        If UpdateTextModif Then Return
        If ListBoxElement.SelectedIndex < 0 Then Return

        UpdateTextModif = True
        indexFloat = ListBoxElement.SelectedIndex

        Try
            NewElementFloat = Buoy.FlotteurBouee.ElementAt(Buoy.FlotteurBouee.Elements.Count - ListBoxElement.SelectedIndex - 1).Clone
            NewElementFloat.HauteurElement = Double.Parse(TextBoxFloatHauteur.Text, EnglishFormat)
            If NewElementFloat.HauteurElement >= 0 Then Buoy.FlotteurBouee.NewElementAt(Buoy.FlotteurBouee.Elements.Count - ListBoxElement.SelectedIndex - 1, NewElementFloat)
        Catch ex As Exception
            UpdateTextModif = False
            Return
        End Try

        FloatDisplay()
        ListBoxElement.SelectedIndex = indexFloat
        UpdateTextModif = False

    End Sub

    Private Sub TextBoxFloatDiametreH_TextChanged(sender As System.Object, e As System.Windows.Controls.TextChangedEventArgs)

        Dim indexFloat As Integer
        Dim NewElementFloat As BuoyLib.CDimensionElementTroncCone

        If UpdateTextModif Then Return
        If ListBoxElement.SelectedIndex < 0 Then Return

        UpdateTextModif = True
        indexFloat = ListBoxElement.SelectedIndex

        Try
            NewElementFloat = Buoy.FlotteurBouee.ElementAt(Buoy.FlotteurBouee.Elements.Count - ListBoxElement.SelectedIndex - 1).Clone
            NewElementFloat.DiameterHigh = Double.Parse(TextBoxFloatDiametreH.Text, EnglishFormat)
            If NewElementFloat.DiameterHigh >= 0 Then Buoy.FlotteurBouee.NewElementAt(Buoy.FlotteurBouee.Elements.Count - ListBoxElement.SelectedIndex - 1, NewElementFloat)
        Catch ex As Exception
            UpdateTextModif = False
            Return
        End Try

        FloatDisplay()
        ListBoxElement.SelectedIndex = indexFloat
        UpdateTextModif = False

    End Sub

    Private Sub TextBoxFloatDiametreI_TextChanged(sender As System.Object, e As System.Windows.Controls.TextChangedEventArgs)

        Dim indexFloat As Integer
        Dim NewElementFloat As BuoyLib.CDimensionElementTroncCone

        If UpdateTextModif Then Return
        If ListBoxElement.SelectedIndex < 0 Then Return

        UpdateTextModif = True
        indexFloat = ListBoxElement.SelectedIndex

        Try
            NewElementFloat = Buoy.FlotteurBouee.ElementAt(Buoy.FlotteurBouee.Elements.Count - ListBoxElement.SelectedIndex - 1).Clone
            NewElementFloat.DiameterInter = Double.Parse(TextBoxFloatDiametreI.Text, EnglishFormat)
            If NewElementFloat.DiameterInter >= 0 Then Buoy.FlotteurBouee.NewElementAt(Buoy.FlotteurBouee.Elements.Count - ListBoxElement.SelectedIndex - 1, NewElementFloat)
        Catch ex As Exception
            UpdateTextModif = False
            Return
        End Try

        FloatDisplay()
        ListBoxElement.SelectedIndex = indexFloat
        UpdateTextModif = False

    End Sub

    Private Sub TextBoxFloatDiametreB_TextChanged(sender As System.Object, e As System.Windows.Controls.TextChangedEventArgs)

        Dim indexFloat As Integer
        Dim NewElementFloat As BuoyLib.CDimensionElementTroncCone

        If UpdateTextModif Then Return
        If ListBoxElement.SelectedIndex < 0 Then Return

        UpdateTextModif = True
        indexFloat = ListBoxElement.SelectedIndex

        Try
            NewElementFloat = Buoy.FlotteurBouee.ElementAt(Buoy.FlotteurBouee.Elements.Count - ListBoxElement.SelectedIndex - 1).Clone
            NewElementFloat.DiameterLow = Double.Parse(TextBoxFloatDiametreB.Text, EnglishFormat)
            If NewElementFloat.DiameterLow >= 0 Then Buoy.FlotteurBouee.NewElementAt(Buoy.FlotteurBouee.Elements.Count - ListBoxElement.SelectedIndex - 1, NewElementFloat)
        Catch ex As Exception
            UpdateTextModif = False
            Return
        End Try

        FloatDisplay()
        ListBoxElement.SelectedIndex = indexFloat
        UpdateTextModif = False

    End Sub

    Private Sub TextBoxFloatVolume_TextChanged(sender As System.Object, e As System.Windows.Controls.TextChangedEventArgs)

        Dim indexFloat As Integer
        Dim NewElementFloat As BuoyLib.CDimensionElementTroncCone

        If UpdateTextModif Then Return
        If ListBoxElement.SelectedIndex < 0 Then Return

        UpdateTextModif = True
        indexFloat = ListBoxElement.SelectedIndex

        Try
            NewElementFloat = Buoy.FlotteurBouee.ElementAt(Buoy.FlotteurBouee.Elements.Count - ListBoxElement.SelectedIndex - 1).Clone
            NewElementFloat.VolumeReel = Double.Parse(TextBoxFloatVolume.Text, EnglishFormat)
            If NewElementFloat.VolumeReel >= 0 Then Buoy.FlotteurBouee.NewElementAt(Buoy.FlotteurBouee.Elements.Count - ListBoxElement.SelectedIndex - 1, NewElementFloat)
        Catch ex As Exception
            UpdateTextModif = False
            Return
        End Try

        FloatDisplay()
        ListBoxElement.SelectedIndex = indexFloat
        UpdateTextModif = False

    End Sub

#End Region

#Region "Gestion de l'ajout ou de la modification de la structure"

    Private Sub SelectionElementStructure()

        Dim IndexStruct As Integer

        ManagerBuoyButtonStructureDeplacerElementAuDessus.IsEnabled = False
        ManagerBuoyButtonStructureDeplacerElementAuDessous.IsEnabled = False

        If ListBoxElement.SelectedIndex < 0 Then
            StructureNoSelection()
            Return
        End If

        ManagerBuoyButtonNouveauStructure.IsChecked = False
        ManagerBuoyButtonModificationStructure.IsChecked = True

        ManagerBuoyButtonAjouterStructure.IsEnabled = False
        ManagerBuoyButtonSupprimerStructure.IsEnabled = True
        If ListBoxElement.SelectedIndex > 0 Then ManagerBuoyButtonStructureDeplacerElementAuDessus.IsEnabled = True
        If ListBoxElement.SelectedIndex < (Buoy.StructureBouee.Elements.Count - 1) Then ManagerBuoyButtonStructureDeplacerElementAuDessous.IsEnabled = True

        IndexStruct = Buoy.StructureBouee.Elements.Count - ListBoxElement.SelectedIndex - 1
        DessineElementStructureSelectionner.Child = Buoy.StructureBouee.ElementAt(IndexStruct).DrawCanvasByHeight(DessineElementStructureSelectionner.ActualWidth, DessineElementStructureSelectionner.ActualHeight)

        TextBoxStructureVolume.Text = Buoy.StructureBouee.Volume.ToString(EnglishFormat)
        TextBoxStructVolumeCalcul.Text = Math.Round(Buoy.StructureBouee.ElementAt(IndexStruct).VolumeCalcule, 3).ToString(EnglishFormat)

        If UpdateTextModif Then Return
        UpdateTextModif = True
        TextBoxStructHauteur.Text = Buoy.StructureBouee.ElementAt(IndexStruct).HauteurElement.ToString(EnglishFormat)
        TextBoxStructDiametreH.Text = Buoy.StructureBouee.ElementAt(IndexStruct).DiameterHigh.ToString(EnglishFormat)
        TextBoxStructDiametreI.Text = Buoy.StructureBouee.ElementAt(IndexStruct).DiameterInter.ToString(EnglishFormat)
        TextBoxStructDiametreB.Text = Buoy.StructureBouee.ElementAt(IndexStruct).DiameterLow.ToString(EnglishFormat)
        TextBoxStructVolume.Text = Buoy.StructureBouee.ElementAt(IndexStruct).VolumeReel.ToString(EnglishFormat)
        UpdateTextModif = False

    End Sub

    Private Sub StructureNoSelection()

        ManagerBuoyButtonNouveauStructure.IsChecked = True
        ManagerBuoyButtonModificationStructure.IsChecked = False

        ManagerBuoyButtonStructureDeplacerElementAuDessus.IsEnabled = False
        ManagerBuoyButtonStructureDeplacerElementAuDessous.IsEnabled = False

        DessineElementStructureSelectionner.Child = Nothing
        ManagerBuoyButtonSupprimerStructure.IsEnabled = False
        ManagerBuoyButtonAjouterStructure.IsEnabled = True

        If UpdateTextModif Then Return
        UpdateTextModif = True
        TextBoxStructHauteur.Text = "0.0"
        TextBoxStructDiametreH.Text = "0.0"
        TextBoxStructDiametreI.Text = "0.0"
        TextBoxStructDiametreB.Text = "0.0"
        TextBoxStructVolume.Text = "0.0"
        TextBoxStructVolumeCalcul.Text = "0.0"
        UpdateTextModif = False

    End Sub

    Private Sub NouveauStructure_Click(sender As System.Object, e As System.Windows.RoutedEventArgs)

        Dim HauteurText As String = TextBoxStructHauteur.Text
        Dim DiametreHText As String = TextBoxStructDiametreH.Text
        Dim DiametreIText As String = TextBoxStructDiametreI.Text
        Dim DiametreBText As String = TextBoxStructDiametreB.Text
        Dim VolumeText As String = TextBoxStructVolume.Text

        RefreshBuoyDisplay()

        TextBoxStructHauteur.Text = HauteurText
        TextBoxStructDiametreH.Text = DiametreHText
        TextBoxStructDiametreI.Text = DiametreIText
        TextBoxStructDiametreB.Text = DiametreBText
        TextBoxStructVolume.Text = VolumeText

    End Sub

    Private Sub AddStructure_Click(sender As System.Object, e As System.Windows.RoutedEventArgs)

        Dim NewElementStruct As New BuoyLib.CDimensionElementTroncCone

        Try
            NewElementStruct.HauteurElement = Double.Parse(TextBoxStructHauteur.Text, EnglishFormat)
            NewElementStruct.DiameterHigh = Double.Parse(TextBoxStructDiametreH.Text, EnglishFormat)
            NewElementStruct.DiameterInter = Double.Parse(TextBoxStructDiametreI.Text, EnglishFormat)
            NewElementStruct.DiameterLow = Double.Parse(TextBoxStructDiametreB.Text, EnglishFormat)
            NewElementStruct.VolumeReel = Double.Parse(TextBoxStructVolume.Text, EnglishFormat)

            Buoy.StructureBouee.AddElement(NewElementStruct)
        Catch ex As Exception
            Return
        End Try

        RefreshBuoyDisplay()

        ListBoxElement.SelectedIndex = 0

    End Sub

    Private Sub SupprimerStructure_Click(sender As System.Object, e As System.Windows.RoutedEventArgs)

        If ListBoxElement.SelectedIndex < 0 Then Return

        Buoy.StructureBouee.RemoveElementAt(Buoy.StructureBouee.Elements.Count - ListBoxElement.SelectedIndex - 1)
        RefreshBuoyDisplay()

    End Sub

    Private Sub ButtonStructureDeplacerElementAuDessus_Click(sender As System.Object, e As System.Windows.RoutedEventArgs)

        Dim indexStruct As Integer

        If ListBoxElement.SelectedIndex < 0 Then Return
        indexStruct = ListBoxElement.SelectedIndex

        Buoy.StructureBouee.MoveElementUp(Buoy.StructureBouee.Elements.Count - ListBoxElement.SelectedIndex - 1)

        RefreshBuoyDisplay()

        ListBoxElement.SelectedIndex = indexStruct - 1

    End Sub

    Private Sub ButtonStructureDeplacerElementAuDessous_Click(sender As System.Object, e As System.Windows.RoutedEventArgs)

        Dim indexStruct As Integer

        If ListBoxElement.SelectedIndex < 0 Then Return
        indexStruct = ListBoxElement.SelectedIndex

        Buoy.StructureBouee.MoveElementDown(Buoy.StructureBouee.Elements.Count - ListBoxElement.SelectedIndex - 1)

        RefreshBuoyDisplay()

        ListBoxElement.SelectedIndex = indexStruct + 1

    End Sub

    Private Function CreateMenuStructure(ByVal buoy As CBouee) As MenuItem

        Dim SubMenuStruct As MenuItem
        CreateMenuStructure = New MenuItem() With {.Header = buoy.Nom}

        SubMenuStruct = New MenuItem() With {.Header = buoy.StructureBouee.Nom}
        CreateMenuStructure.Items.Add(SubMenuStruct)

        ' Ajout du retour de Remplacement
        AddHandler SubMenuStruct.Click, AddressOf ComboBoxRemplaceStructure_Click

    End Function

    Private Sub ComboBoxRemplaceStructure_Click(sender As Object, e As System.Windows.RoutedEventArgs)

        Dim MenuClicked As MenuItem = sender
        Dim ParentMenuClicked As MenuItem

        If MenuClicked.Parent Is Nothing Then Return

        ParentMenuClicked = MenuClicked.Parent
        If ParentMenuClicked Is Nothing Then Return

        If Not BaseBouees.GetBouees.ContainsKey(ParentMenuClicked.Header) Then Return

        Buoy.StructureBouee = BaseBouees.GetBouees(ParentMenuClicked.Header).StructureBouee.Clone

        RefreshBuoyDisplay()

    End Sub

    Private Sub TextBoxStructureMasse_TextChanged(sender As System.Object, e As System.Windows.Controls.TextChangedEventArgs)

        Try
            Buoy.StructureBouee.Masse = Double.Parse(TextBoxStructureMasse.Text, EnglishFormat)
        Catch
        End Try

    End Sub

    Private Sub TextBoxStructureOffset_TextChanged(sender As System.Object, e As System.Windows.Controls.TextChangedEventArgs)

        Dim indexStruct As Integer

        If UpdateTextModif Then Return
        UpdateTextModif = True
        indexStruct = ListBoxElement.SelectedIndex

        Try
            Buoy.StructureBouee.OffsetOrganeau = Double.Parse(TextBoxStructureOffset.Text, EnglishFormat)
        Catch
        End Try

        StructureDisplay()

        ListBoxElement.SelectedIndex = indexStruct
        UpdateTextModif = False

    End Sub

    Private Sub TextBoxStructHauteur_TextChanged(sender As System.Object, e As System.Windows.Controls.TextChangedEventArgs)

        Dim indexStruct As Integer
        Dim NewElementStruct As BuoyLib.CDimensionElementTroncCone

        If ListBoxElement.SelectedIndex < 0 Then Return

        If UpdateTextModif Then Return
        UpdateTextModif = True
        indexStruct = ListBoxElement.SelectedIndex

        Try
            NewElementStruct = Buoy.StructureBouee.ElementAt(Buoy.StructureBouee.Elements.Count - ListBoxElement.SelectedIndex - 1).Clone
            NewElementStruct.HauteurElement = Double.Parse(TextBoxStructHauteur.Text, EnglishFormat)
            If NewElementStruct.HauteurElement >= 0 Then Buoy.StructureBouee.NewElementAt(Buoy.StructureBouee.Elements.Count - ListBoxElement.SelectedIndex - 1, NewElementStruct)
        Catch ex As Exception
            UpdateTextModif = False
            Return
        End Try

        StructureDisplay()
        ListBoxElement.SelectedIndex = indexStruct
        UpdateTextModif = False

    End Sub

    Private Sub TextBoxStructDiametreH_TextChanged(sender As System.Object, e As System.Windows.Controls.TextChangedEventArgs)

        Dim indexStruct As Integer
        Dim NewElementStruct As BuoyLib.CDimensionElementTroncCone

        If ListBoxElement.SelectedIndex < 0 Then Return

        If UpdateTextModif Then Return
        UpdateTextModif = True
        indexStruct = ListBoxElement.SelectedIndex

        Try
            NewElementStruct = Buoy.StructureBouee.ElementAt(Buoy.StructureBouee.Elements.Count - ListBoxElement.SelectedIndex - 1).Clone
            NewElementStruct.DiameterHigh = Double.Parse(TextBoxStructDiametreH.Text, EnglishFormat)
            If NewElementStruct.DiameterHigh >= 0 Then Buoy.StructureBouee.NewElementAt(Buoy.StructureBouee.Elements.Count - ListBoxElement.SelectedIndex - 1, NewElementStruct)
        Catch ex As Exception
            UpdateTextModif = False
            Return
        End Try

        StructureDisplay()
        ListBoxElement.SelectedIndex = indexStruct
        UpdateTextModif = False

    End Sub

    Private Sub TextBoxStructDiametreI_TextChanged(sender As System.Object, e As System.Windows.Controls.TextChangedEventArgs)

        Dim indexStruct As Integer
        Dim NewElementStruct As BuoyLib.CDimensionElementTroncCone

        If ListBoxElement.SelectedIndex < 0 Then Return

        If UpdateTextModif Then Return
        UpdateTextModif = True
        indexStruct = ListBoxElement.SelectedIndex

        Try
            NewElementStruct = Buoy.StructureBouee.ElementAt(Buoy.StructureBouee.Elements.Count - ListBoxElement.SelectedIndex - 1).Clone
            NewElementStruct.DiameterInter = Double.Parse(TextBoxStructDiametreI.Text, EnglishFormat)
            If NewElementStruct.DiameterInter >= 0 Then Buoy.StructureBouee.NewElementAt(Buoy.StructureBouee.Elements.Count - ListBoxElement.SelectedIndex - 1, NewElementStruct)
        Catch ex As Exception
            UpdateTextModif = False
            Return
        End Try

        StructureDisplay()
        ListBoxElement.SelectedIndex = indexStruct
        UpdateTextModif = False

    End Sub

    Private Sub TextBoxStructDiametreB_TextChanged(sender As System.Object, e As System.Windows.Controls.TextChangedEventArgs)

        Dim indexStruct As Integer
        Dim NewElementStruct As BuoyLib.CDimensionElementTroncCone

        If ListBoxElement.SelectedIndex < 0 Then Return

        If UpdateTextModif Then Return
        UpdateTextModif = True
        indexStruct = ListBoxElement.SelectedIndex

        Try
            NewElementStruct = Buoy.StructureBouee.ElementAt(Buoy.StructureBouee.Elements.Count - ListBoxElement.SelectedIndex - 1).Clone
            NewElementStruct.DiameterLow = Double.Parse(TextBoxStructDiametreB.Text, EnglishFormat)
            If NewElementStruct.DiameterLow >= 0 Then Buoy.StructureBouee.NewElementAt(Buoy.StructureBouee.Elements.Count - ListBoxElement.SelectedIndex - 1, NewElementStruct)
        Catch ex As Exception
            UpdateTextModif = False
            Return
        End Try

        StructureDisplay()
        ListBoxElement.SelectedIndex = indexStruct
        UpdateTextModif = False

    End Sub

    Private Sub TextBoxStructVolume_TextChanged(sender As System.Object, e As System.Windows.Controls.TextChangedEventArgs)

        Dim indexStruct As Integer
        Dim NewElementStruct As BuoyLib.CDimensionElementTroncCone

        If ListBoxElement.SelectedIndex < 0 Then Return

        If UpdateTextModif Then Return
        UpdateTextModif = True
        indexStruct = ListBoxElement.SelectedIndex

        Try
            NewElementStruct = Buoy.StructureBouee.ElementAt(Buoy.StructureBouee.Elements.Count - ListBoxElement.SelectedIndex - 1).Clone
            NewElementStruct.VolumeReel = Double.Parse(TextBoxStructVolume.Text, EnglishFormat)
            If NewElementStruct.VolumeReel >= 0 Then Buoy.StructureBouee.NewElementAt(Buoy.StructureBouee.Elements.Count - ListBoxElement.SelectedIndex - 1, NewElementStruct)
        Catch ex As Exception
            UpdateTextModif = False
            Return
        End Try

        StructureDisplay()
        ListBoxElement.SelectedIndex = indexStruct
        UpdateTextModif = False

    End Sub

#End Region

#Region "Gestion de l'ajout ou de la modification des pylones"

    Private Sub SelectionElementPylone()

        Dim IndexStruct As Integer

        ManagerBuoyButtonPyloneDeplacerElementAuDessus.IsEnabled = False
        ManagerBuoyButtonPyloneDeplacerElementAuDessous.IsEnabled = False

        If ListBoxElement.SelectedIndex < 0 Then
            PyloneNoSelection()
            Return
        End If

        ManagerBuoyButtonNouveauPylone.IsChecked = False
        ManagerBuoyButtonModificationPylone.IsChecked = True
        ManagerBuoyButtonCopiePylone.IsEnabled = True

        ManagerBuoyButtonAjouterPylone.IsEnabled = False
        ManagerBuoyButtonSupprimerPylone.IsEnabled = True
        If ListBoxElement.SelectedIndex > 0 Then ManagerBuoyButtonPyloneDeplacerElementAuDessus.IsEnabled = True
        If ListBoxElement.SelectedIndex < (Buoy.PyloneBouee.Count - 1) Then ManagerBuoyButtonPyloneDeplacerElementAuDessous.IsEnabled = True

        IndexStruct = Buoy.PyloneBouee.Count - ListBoxElement.SelectedIndex - 1
        DessineElementPyloneSelectionner.Child = Buoy.PyloneBouee.ElementAt(IndexStruct).DrawCanvasByHeight(DessineElementPyloneSelectionner.ActualWidth, DessineElementPyloneSelectionner.ActualHeight)

        If UpdateTextModif Then Return
        UpdateTextModif = True
        TextBoxNomPylone.Text = Buoy.PyloneBouee(IndexStruct).Nom
        TextBoxPyloneMasse.Text = Buoy.PyloneBouee(IndexStruct).Masse.ToString(EnglishFormat)

        TextBoxPyloneHauteur.Text = Buoy.PyloneBouee(IndexStruct).Element.HauteurElement.ToString(EnglishFormat)
        TextBoxPyloneDiametreH.Text = Buoy.PyloneBouee(IndexStruct).Element.LongueurHautElement.ToString(EnglishFormat)
        TextBoxPyloneDiametreB.Text = Buoy.PyloneBouee(IndexStruct).Element.LongueurBasElement.ToString(EnglishFormat)
        UpdateTextModif = False

    End Sub

    Private Sub PyloneNoSelection()

        ManagerBuoyButtonNouveauPylone.IsChecked = True
        ManagerBuoyButtonModificationPylone.IsChecked = False
        ManagerBuoyButtonCopiePylone.IsEnabled = False

        ManagerBuoyButtonPyloneDeplacerElementAuDessus.IsEnabled = False
        ManagerBuoyButtonPyloneDeplacerElementAuDessous.IsEnabled = False

        DessineElementPyloneSelectionner.Child = Nothing
        ManagerBuoyButtonSupprimerPylone.IsEnabled = False
        ManagerBuoyButtonAjouterPylone.IsEnabled = True

        If UpdateTextModif Then Return
        UpdateTextModif = True
        TextBoxNomPylone.Text = ""
        TextBoxPyloneMasse.Text = ""

        TextBoxPyloneHauteur.Text = "0.0"
        TextBoxPyloneDiametreH.Text = "0.0"
        TextBoxPyloneDiametreB.Text = "0.0"
        UpdateTextModif = False

    End Sub

    Private Sub NouveauPylone_Click(sender As System.Object, e As System.Windows.RoutedEventArgs)

        Dim Hauteur As String = TextBoxPyloneHauteur.Text
        Dim DiametreH As String = TextBoxPyloneDiametreH.Text
        Dim DiametreB As String = TextBoxPyloneDiametreB.Text

        RefreshBuoyDisplay()

        TextBoxPyloneHauteur.Text = Hauteur
        TextBoxPyloneDiametreH.Text = DiametreH
        TextBoxPyloneDiametreB.Text = DiametreB

    End Sub

    Private Sub AddPylone_Click(sender As System.Object, e As System.Windows.RoutedEventArgs)

        Dim NewElement As New CPylone

        Try
            NewElement.Nom = TextBoxNomPylone.Text
            NewElement.Masse = Double.Parse(TextBoxPyloneMasse.Text, EnglishFormat)
            NewElement.Element.HauteurElement = Double.Parse(TextBoxPyloneHauteur.Text, EnglishFormat)
            NewElement.Element.LongueurHautElement = Double.Parse(TextBoxPyloneDiametreH.Text, EnglishFormat)
            NewElement.Element.LongueurBasElement = Double.Parse(TextBoxPyloneDiametreB.Text, EnglishFormat)

            Buoy.NouveauPyloneBouee = NewElement
        Catch ex As Exception
            Return
        End Try

        RefreshBuoyDisplay()

        ListBoxElement.SelectedIndex = 0

    End Sub

    Private Sub SupprimerPylone_Click(sender As System.Object, e As System.Windows.RoutedEventArgs)

        If ListBoxElement.SelectedIndex < 0 Then Return

        Buoy.RemovePylone(Buoy.PyloneBouee.Count - ListBoxElement.SelectedIndex - 1)
        RefreshBuoyDisplay()

    End Sub

    Private Sub ButtonPyloneDeplacerElementAuDessus_Click(sender As System.Object, e As System.Windows.RoutedEventArgs)

        Dim indexStruct As Integer

        If ListBoxElement.SelectedIndex < 0 Then Return
        indexStruct = ListBoxElement.SelectedIndex

        Buoy.MoveUpPylone(Buoy.PyloneBouee.Count - ListBoxElement.SelectedIndex - 1)

        RefreshBuoyDisplay()

        ListBoxElement.SelectedIndex = indexStruct - 1

    End Sub

    Private Sub ButtonPyloneDeplacerElementAuDessous_Click(sender As System.Object, e As System.Windows.RoutedEventArgs)

        Dim indexStruct As Integer

        If ListBoxElement.SelectedIndex < 0 Then Return
        indexStruct = ListBoxElement.SelectedIndex

        Buoy.MoveDownPylone(Buoy.PyloneBouee.Count - ListBoxElement.SelectedIndex - 1)

        RefreshBuoyDisplay()

        ListBoxElement.SelectedIndex = indexStruct + 1

    End Sub

    Private Function CreateMenuPylone(ByVal buoy As CBouee) As MenuItem

        Dim SubMenuStruct As MenuItem
        CreateMenuPylone = New MenuItem() With {.Header = buoy.Nom}

        For Each Pyl In buoy.PyloneBouee
            SubMenuStruct = New MenuItem() With {.Header = Pyl.Nom}
            CreateMenuPylone.Items.Add(SubMenuStruct)

            ' Ajout du retour de Remplacement
            AddHandler SubMenuStruct.Click, AddressOf ComboBoxRemplacePylone_Click
        Next

    End Function

    Private Sub ComboBoxRemplacePylone_Click(sender As Object, e As System.Windows.RoutedEventArgs)

        Dim MenuClicked As MenuItem = sender
        Dim ParentMenuClicked As MenuItem

        If MenuClicked.Parent Is Nothing Then Return

        ParentMenuClicked = MenuClicked.Parent
        If ParentMenuClicked Is Nothing Then Return

        If Not BaseBouees.GetBouees.ContainsKey(ParentMenuClicked.Header) Then Return

        For Each pyl In BaseBouees.GetBouees(ParentMenuClicked.Header).PyloneBouee
            If pyl.Nom = MenuClicked.Header Then
                Dim i As Integer = ListBoxElement.SelectedIndex

                Buoy.ModifyPylone(Buoy.PyloneBouee.Count - ListBoxElement.SelectedIndex - 1, pyl.Clone)
                RefreshBuoyDisplay()

                ListBoxElement.SelectedIndex = i
                Return
            End If
        Next

    End Sub

    Private Sub ModificationNomPylone()

        Dim indexStruct As Integer
        Dim NewElement As New CPylone

        If ListBoxElement.SelectedIndex < 0 Then Return

        If UpdateTextModif Then Return
        UpdateTextModif = True
        indexStruct = ListBoxElement.SelectedIndex

        Try
            NewElement = Buoy.PyloneBouee(Buoy.PyloneBouee.Count - ListBoxElement.SelectedIndex - 1)
            NewElement.Nom = TextBoxNomPylone.Text
            Buoy.ModifyPylone(Buoy.PyloneBouee.Count - ListBoxElement.SelectedIndex - 1, NewElement)
        Catch ex As Exception
            UpdateTextModif = False
            Return
        End Try

        PyloneDisplay()
        ListBoxElement.SelectedIndex = indexStruct
        UpdateTextModif = False

    End Sub

    Private Sub TextBoxPyloneMasse_TextChanged(sender As System.Object, e As System.Windows.Controls.TextChangedEventArgs)

        Dim indexStruct As Integer
        Dim NewElement As New CPylone

        If ListBoxElement.SelectedIndex < 0 Then Return

        If UpdateTextModif Then Return
        UpdateTextModif = True
        indexStruct = ListBoxElement.SelectedIndex

        Try
            NewElement = Buoy.PyloneBouee(Buoy.PyloneBouee.Count - ListBoxElement.SelectedIndex - 1)
            NewElement.Masse = Double.Parse(TextBoxPyloneMasse.Text, EnglishFormat)
            Buoy.ModifyPylone(Buoy.PyloneBouee.Count - ListBoxElement.SelectedIndex - 1, NewElement)
        Catch ex As Exception
            UpdateTextModif = False
            Return
        End Try

        PyloneDisplay()
        ListBoxElement.SelectedIndex = indexStruct
        UpdateTextModif = False

    End Sub

    Private Sub TextBoxPyloneHauteur_TextChanged(sender As System.Object, e As System.Windows.Controls.TextChangedEventArgs)

        Dim indexStruct As Integer
        Dim NewElement As New CPylone

        If ListBoxElement.SelectedIndex < 0 Then Return

        If UpdateTextModif Then Return
        UpdateTextModif = True
        indexStruct = ListBoxElement.SelectedIndex

        Try
            NewElement = Buoy.PyloneBouee(Buoy.PyloneBouee.Count - ListBoxElement.SelectedIndex - 1).Clone
            NewElement.Element.HauteurElement = Double.Parse(TextBoxPyloneHauteur.Text, EnglishFormat)
            If NewElement.Element.HauteurElement >= 0 Then Buoy.ModifyPylone(Buoy.PyloneBouee.Count - ListBoxElement.SelectedIndex - 1, NewElement)
        Catch ex As Exception
            UpdateTextModif = False
            Return
        End Try

        PyloneDisplay()
        ListBoxElement.SelectedIndex = indexStruct
        UpdateTextModif = False

    End Sub

    Private Sub TextBoxPyloneDiametreH_TextChanged(sender As System.Object, e As System.Windows.Controls.TextChangedEventArgs)

        Dim indexStruct As Integer
        Dim NewElement As New CPylone

        If ListBoxElement.SelectedIndex < 0 Then Return

        If UpdateTextModif Then Return
        UpdateTextModif = True
        indexStruct = ListBoxElement.SelectedIndex

        Try
            NewElement = Buoy.PyloneBouee(Buoy.PyloneBouee.Count - ListBoxElement.SelectedIndex - 1).Clone
            NewElement.Element.LongueurHautElement = Double.Parse(TextBoxPyloneDiametreH.Text, EnglishFormat)
            If NewElement.Element.LongueurHautElement >= 0 Then Buoy.ModifyPylone(Buoy.PyloneBouee.Count - ListBoxElement.SelectedIndex - 1, NewElement)
        Catch ex As Exception
            UpdateTextModif = False
            Return
        End Try

        PyloneDisplay()
        ListBoxElement.SelectedIndex = indexStruct
        UpdateTextModif = False

    End Sub

    Private Sub TextBoxPyloneDiametreB_TextChanged(sender As System.Object, e As System.Windows.Controls.TextChangedEventArgs)

        Dim indexStruct As Integer
        Dim NewElement As New CPylone

        If ListBoxElement.SelectedIndex < 0 Then Return

        If UpdateTextModif Then Return
        UpdateTextModif = True
        indexStruct = ListBoxElement.SelectedIndex

        Try
            NewElement = Buoy.PyloneBouee(Buoy.PyloneBouee.Count - ListBoxElement.SelectedIndex - 1).Clone
            NewElement.Element.LongueurBasElement = Double.Parse(TextBoxPyloneDiametreB.Text, EnglishFormat)
            If NewElement.Element.LongueurBasElement >= 0 Then Buoy.ModifyPylone(Buoy.PyloneBouee.Count - ListBoxElement.SelectedIndex - 1, NewElement)
        Catch ex As Exception
            UpdateTextModif = False
            Return
        End Try

        PyloneDisplay()
        ListBoxElement.SelectedIndex = indexStruct
        UpdateTextModif = False

    End Sub

#End Region

#Region "Gestion de l'ajout ou de la modification des equipements"

    Private Sub SelectionElementVoyant()

        Dim IndexStruct As Integer

        ManagerBuoyButtonVoyantDeplacerElementAuDessus.IsEnabled = False
        ManagerBuoyButtonVoyantDeplacerElementAuDessous.IsEnabled = False

        If ListBoxElement.SelectedIndex < 0 Then
            VoyantNoSelection()
            Return
        End If

        ManagerBuoyButtonNouveauVoyant.IsChecked = False
        ManagerBuoyButtonModificationVoyant.IsChecked = True
        ManagerBuoyButtonCopieVoyant.IsEnabled = True

        ManagerBuoyButtonAjouterVoyant.IsEnabled = False
        ManagerBuoyButtonSupprimerVoyant.IsEnabled = True
        If ListBoxElement.SelectedIndex > 0 Then ManagerBuoyButtonVoyantDeplacerElementAuDessus.IsEnabled = True
        If ListBoxElement.SelectedIndex < (Buoy.EquipementBouee.Count - 1) Then ManagerBuoyButtonVoyantDeplacerElementAuDessous.IsEnabled = True

        IndexStruct = Buoy.EquipementBouee.Count - ListBoxElement.SelectedIndex - 1
        DessineElementVoyantSelectionner.Child = Buoy.EquipementBouee.ElementAt(IndexStruct).DrawCanvasByHeight(DessineElementVoyantSelectionner.ActualWidth, DessineElementVoyantSelectionner.ActualHeight)

        If UpdateTextModif Then Return
        UpdateTextModif = True
        TextBoxNomVoyant.Text = Buoy.EquipementBouee(IndexStruct).Nom
        TextBoxVoyantMasse.Text = Buoy.EquipementBouee(IndexStruct).Masse.ToString(EnglishFormat)

        TextBoxVoyantHauteur.Text = Buoy.EquipementBouee(IndexStruct).Element.HauteurElement.ToString(EnglishFormat)
        TextBoxVoyantDiametreH.Text = Buoy.EquipementBouee(IndexStruct).Element.LongueurHautElement.ToString(EnglishFormat)
        TextBoxVoyantDiametreB.Text = Buoy.EquipementBouee(IndexStruct).Element.LongueurBasElement.ToString(EnglishFormat)
        UpdateTextModif = False

    End Sub

    Private Sub VoyantNoSelection()

        ManagerBuoyButtonNouveauVoyant.IsChecked = True
        ManagerBuoyButtonModificationVoyant.IsChecked = False
        ManagerBuoyButtonCopieVoyant.IsEnabled = False

        ManagerBuoyButtonVoyantDeplacerElementAuDessus.IsEnabled = False
        ManagerBuoyButtonVoyantDeplacerElementAuDessous.IsEnabled = False

        DessineElementVoyantSelectionner.Child = Nothing
        ManagerBuoyButtonSupprimerVoyant.IsEnabled = False
        ManagerBuoyButtonAjouterVoyant.IsEnabled = True

        If UpdateTextModif Then Return
        UpdateTextModif = True
        TextBoxNomVoyant.Text = ""
        TextBoxVoyantMasse.Text = ""

        TextBoxVoyantHauteur.Text = "0.0"
        TextBoxVoyantDiametreH.Text = "0.0"
        TextBoxVoyantDiametreB.Text = "0.0"
        UpdateTextModif = False

    End Sub

    Private Sub NouveauVoyant_Click(sender As System.Object, e As System.Windows.RoutedEventArgs)

        Dim Hauteur As String = TextBoxVoyantHauteur.Text
        Dim DiametreH As String = TextBoxVoyantDiametreH.Text
        Dim DiametreB As String = TextBoxVoyantDiametreB.Text

        RefreshBuoyDisplay()

        TextBoxVoyantHauteur.Text = Hauteur
        TextBoxVoyantDiametreH.Text = DiametreH
        TextBoxVoyantDiametreB.Text = DiametreB


    End Sub

    Private Sub AddVoyant_Click(sender As System.Object, e As System.Windows.RoutedEventArgs)

        Dim NewElement As New CEquipement

        Try
            NewElement.Nom = TextBoxNomVoyant.Text
            NewElement.Masse = Double.Parse(TextBoxVoyantMasse.Text, EnglishFormat)
            NewElement.Element.HauteurElement = Double.Parse(TextBoxVoyantHauteur.Text, EnglishFormat)
            NewElement.Element.LongueurHautElement = Double.Parse(TextBoxVoyantDiametreH.Text, EnglishFormat)
            NewElement.Element.LongueurBasElement = Double.Parse(TextBoxVoyantDiametreB.Text, EnglishFormat)

            Buoy.NouvelleEquipementBouee = NewElement
        Catch ex As Exception
            Return
        End Try

        RefreshBuoyDisplay()

        ListBoxElement.SelectedIndex = 0

    End Sub

    Private Sub SupprimerVoyant_Click(sender As System.Object, e As System.Windows.RoutedEventArgs)

        If ListBoxElement.SelectedIndex < 0 Then Return

        Buoy.RemoveEquipement(Buoy.EquipementBouee.Count - ListBoxElement.SelectedIndex - 1)
        RefreshBuoyDisplay()

    End Sub

    Private Sub ButtonVoyantDeplacerElementAuDessus_Click(sender As System.Object, e As System.Windows.RoutedEventArgs)

        Dim indexStruct As Integer

        If ListBoxElement.SelectedIndex < 0 Then Return
        indexStruct = ListBoxElement.SelectedIndex

        Buoy.MoveUpEquipement(Buoy.EquipementBouee.Count - ListBoxElement.SelectedIndex - 1)

        RefreshBuoyDisplay()

        ListBoxElement.SelectedIndex = indexStruct - 1

    End Sub

    Private Sub ButtonVoyantDeplacerElementAuDessous_Click(sender As System.Object, e As System.Windows.RoutedEventArgs)

        Dim indexStruct As Integer

        If ListBoxElement.SelectedIndex < 0 Then Return
        indexStruct = ListBoxElement.SelectedIndex

        Buoy.MoveDownEquipement(Buoy.EquipementBouee.Count - ListBoxElement.SelectedIndex - 1)

        RefreshBuoyDisplay()

        ListBoxElement.SelectedIndex = indexStruct + 1

    End Sub

    Private Function CreateMenuVoyant(ByVal buoy As CBouee) As MenuItem

        Dim SubMenuStruct As MenuItem
        CreateMenuVoyant = New MenuItem() With {.Header = buoy.Nom}

        For Each Pyl In buoy.EquipementBouee
            SubMenuStruct = New MenuItem() With {.Header = Pyl.Nom}
            CreateMenuVoyant.Items.Add(SubMenuStruct)

            ' Ajout du retour de Remplacement
            AddHandler SubMenuStruct.Click, AddressOf ComboBoxRemplaceVoyant_Click
        Next

    End Function

    Private Sub ComboBoxRemplaceVoyant_Click(sender As Object, e As System.Windows.RoutedEventArgs)

        Dim MenuClicked As MenuItem = sender
        Dim ParentMenuClicked As MenuItem

        If MenuClicked.Parent Is Nothing Then Return

        ParentMenuClicked = MenuClicked.Parent
        If ParentMenuClicked Is Nothing Then Return

        If Not BaseBouees.GetBouees.ContainsKey(ParentMenuClicked.Header) Then Return

        For Each equ In BaseBouees.GetBouees(ParentMenuClicked.Header).EquipementBouee
            If equ.Nom = MenuClicked.Header Then
                Dim i As Integer = ListBoxElement.SelectedIndex

                Buoy.ModifyEquipement(Buoy.EquipementBouee.Count - ListBoxElement.SelectedIndex - 1, equ.Clone)
                RefreshBuoyDisplay()

                ListBoxElement.SelectedIndex = i
                Return
            End If
        Next

    End Sub

    Private Sub ModificationNomVoyant()

        Dim indexStruct As Integer
        Dim NewElement As New CEquipement

        If ListBoxElement.SelectedIndex < 0 Then Return

        If UpdateTextModif Then Return
        UpdateTextModif = True
        indexStruct = ListBoxElement.SelectedIndex

        Try
            NewElement = Buoy.EquipementBouee(Buoy.EquipementBouee.Count - ListBoxElement.SelectedIndex - 1)
            NewElement.Nom = TextBoxNomVoyant.Text
            Buoy.ModifyEquipement(Buoy.EquipementBouee.Count - ListBoxElement.SelectedIndex - 1, NewElement)
        Catch ex As Exception
            UpdateTextModif = False
            Return
        End Try

        VoyantDisplay()
        ListBoxElement.SelectedIndex = indexStruct
        UpdateTextModif = False

    End Sub

    Private Sub TextBoxVoyantMasse_TextChanged(sender As System.Object, e As System.Windows.Controls.TextChangedEventArgs)

        Dim indexStruct As Integer
        Dim NewElement As New CEquipement

        If ListBoxElement.SelectedIndex < 0 Then Return

        If UpdateTextModif Then Return
        UpdateTextModif = True
        indexStruct = ListBoxElement.SelectedIndex

        Try
            NewElement = Buoy.EquipementBouee(Buoy.EquipementBouee.Count - ListBoxElement.SelectedIndex - 1)
            NewElement.Masse = Double.Parse(TextBoxVoyantMasse.Text, EnglishFormat)
            Buoy.ModifyEquipement(Buoy.EquipementBouee.Count - ListBoxElement.SelectedIndex - 1, NewElement)
        Catch ex As Exception
            UpdateTextModif = False
            Return
        End Try

        VoyantDisplay()
        ListBoxElement.SelectedIndex = indexStruct
        UpdateTextModif = False

    End Sub

    Private Sub TextBoxVoyantHauteur_TextChanged(sender As System.Object, e As System.Windows.Controls.TextChangedEventArgs)

        Dim indexStruct As Integer
        Dim NewElement As New CEquipement

        If ListBoxElement.SelectedIndex < 0 Then Return

        If UpdateTextModif Then Return
        UpdateTextModif = True
        indexStruct = ListBoxElement.SelectedIndex

        Try
            NewElement = Buoy.EquipementBouee(Buoy.EquipementBouee.Count - ListBoxElement.SelectedIndex - 1).Clone
            NewElement.Element.HauteurElement = Double.Parse(TextBoxVoyantHauteur.Text, EnglishFormat)
            If NewElement.Element.HauteurElement >= 0 Then Buoy.ModifyEquipement(Buoy.EquipementBouee.Count - ListBoxElement.SelectedIndex - 1, NewElement)
        Catch ex As Exception
            UpdateTextModif = False
            Return
        End Try

        VoyantDisplay()
        ListBoxElement.SelectedIndex = indexStruct
        UpdateTextModif = False

    End Sub

    Private Sub TextBoxVoyantDiametreH_TextChanged(sender As System.Object, e As System.Windows.Controls.TextChangedEventArgs)

        Dim indexStruct As Integer
        Dim NewElement As New CEquipement

        If ListBoxElement.SelectedIndex < 0 Then Return

        If UpdateTextModif Then Return
        UpdateTextModif = True
        indexStruct = ListBoxElement.SelectedIndex

        Try
            NewElement = Buoy.EquipementBouee(Buoy.EquipementBouee.Count - ListBoxElement.SelectedIndex - 1).Clone
            NewElement.Element.LongueurHautElement = Double.Parse(TextBoxVoyantDiametreH.Text, EnglishFormat)
            If NewElement.Element.LongueurHautElement >= 0 Then Buoy.ModifyEquipement(Buoy.EquipementBouee.Count - ListBoxElement.SelectedIndex - 1, NewElement)
        Catch ex As Exception
            UpdateTextModif = False
            Return
        End Try

        VoyantDisplay()
        ListBoxElement.SelectedIndex = indexStruct
        UpdateTextModif = False

    End Sub

    Private Sub TextBoxVoyantDiametreB_TextChanged(sender As System.Object, e As System.Windows.Controls.TextChangedEventArgs)

        Dim indexStruct As Integer
        Dim NewElement As New CEquipement

        If ListBoxElement.SelectedIndex < 0 Then Return

        If UpdateTextModif Then Return
        UpdateTextModif = True
        indexStruct = ListBoxElement.SelectedIndex

        Try
            NewElement = Buoy.EquipementBouee(Buoy.EquipementBouee.Count - ListBoxElement.SelectedIndex - 1).Clone
            NewElement.Element.LongueurBasElement = Double.Parse(TextBoxVoyantDiametreB.Text, EnglishFormat)
            If NewElement.Element.LongueurBasElement >= 0 Then Buoy.ModifyEquipement(Buoy.EquipementBouee.Count - ListBoxElement.SelectedIndex - 1, NewElement)
        Catch ex As Exception
            UpdateTextModif = False
            Return
        End Try

        VoyantDisplay()
        ListBoxElement.SelectedIndex = indexStruct
        UpdateTextModif = False

    End Sub

#End Region

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

                Case GetType(RibbonButton)
                    obj.Label = Cle.Text

                Case GetType(RibbonToggleButton)
                    obj.Label = Cle.Text

                Case GetType(Button)
                    obj.Content = Cle.Text

                Case GetType(RibbonTab)
                    obj.Header = Cle.Text

                Case GetType(RibbonGroup)
                    obj.Header = Cle.Text

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
