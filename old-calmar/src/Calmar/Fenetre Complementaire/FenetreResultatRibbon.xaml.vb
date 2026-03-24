Imports BuoyLib.Buoy
Imports System.Windows.Controls.Ribbon
Imports System.Windows.Controls.DataVisualization.Charting

Public Class FenetreResultatRibbon

    Private FenetreLoaded As Boolean = False

    Private RefreshTime As Date

    Private Calcul1 As New CCalculMouillage
    Private Calcul2 As New CCalculMouillage

    Private Resultat1 As CCalculMouillage.STR_RET_CALCUL_RESULTAT
    Private Resultat2 As CCalculMouillage.STR_RET_CALCUL_RESULTAT

    Private EquipementSelect As New Dictionary(Of String, List(Of BuoyLib.Buoy.CEquipementSupplementaire))

    Private RatioPixel As Double = 0.0

    Private WaitEndFunction As Boolean = False
    Private WaitTemplate As FenetreWait

    Public Sub New(ByVal BoueeComparateur1 As CBouee,
                   ByVal ChaineComparateur1 As CChaine,
                   ByVal QualiteComparateur1 As Integer,
                   ByVal Lest As CBouee.CLest,
                   ByVal ListEquipement As Dictionary(Of String, List(Of BuoyLib.Buoy.CEquipementSupplementaire)),
                   ByVal MasseEquipement As Double,
                   ByVal Profondeur As Double,
                   ByVal Marnage As Double,
                   ByVal HouleMax As Double,
                   ByVal PeriodeHoule As Double,
                   ByVal VitesseVent As Double,
                   ByVal VitesseVentKM As Boolean,
                   ByVal VitesseCourant As Double,
                   ByVal VitesseCourantKts As Boolean,
                   ByVal StringDensiteCM As String,
                   Optional IALA1066 As Boolean = False)

        ' Cet appel est requis par le concepteur.
        InitializeComponent()

        ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().

        ' Definition de la langue
        ChangeLangueInLayout(LayoutRoot)

        ' Selection de la densité du corps mort
        For Each it In ResultComboBoxDensiteCM1.Items
            If it = StringDensiteCM Then
                ResultComboBoxDensiteCM1.SelectedItem = it
                Exit For
            End If
        Next
        For Each it In ResultComboBoxDensiteCM2.Items
            If it = StringDensiteCM Then
                ResultComboBoxDensiteCM2.SelectedItem = it
                Exit For
            End If
        Next

        Calcul1.IALA1066 = IALA1066
        Calcul2.IALA1066 = IALA1066

        ' Calcul sur la bouée numero 1
        Calcul1.BUOY = BoueeComparateur1
        Calcul1.CHAIN = ChaineComparateur1

        ' Selection de la qualité
        Calcul1.QualiteChaine = QualiteComparateur1

        Calcul1.Lest = Lest
        Calcul1.MasseEquipement = MasseEquipement
        Calcul1.Profondeur = Profondeur
        Calcul1.Marnage = Marnage
        Calcul1.HouleMax = HouleMax
        Calcul1.PeriodeHoule = PeriodeHoule
        If VitesseVentKM Then
            Calcul1.VitesseVentKMH = VitesseVent
        Else
            Calcul1.VitesseVent = VitesseVent
        End If
        If VitesseCourantKts Then
            Calcul1.VitesseCourantNDS = VitesseCourant
        Else
            Calcul1.VitesseCourant = VitesseCourant
        End If

        AddHandler Calcul1.RetourCalcul, AddressOf RetourCalcul1

        ' Calcul sur la bouée numero 2
        'Calcul2.BUOY = BoueeComparateur1
        Calcul2.CHAIN = ChaineComparateur1

        ' Selection de la qualité
        Calcul2.QualiteChaine = QualiteComparateur1

        Calcul2.Lest = Lest
        Calcul2.MasseEquipement = MasseEquipement
        Calcul2.Profondeur = Profondeur
        Calcul2.Marnage = Marnage
        Calcul2.HouleMax = HouleMax '* 0.8
        Calcul2.PeriodeHoule = PeriodeHoule
        If VitesseVentKM Then
            Calcul2.VitesseVentKMH = VitesseVent '* 0.8
        Else
            Calcul2.VitesseVent = VitesseVent '* 0.8
        End If
        If VitesseCourantKts Then
            Calcul2.VitesseCourantNDS = VitesseCourant '* 0.8
        Else
            Calcul2.VitesseCourant = VitesseCourant '* 0.8
        End If

        AddHandler Calcul2.RetourCalcul, AddressOf RetourCalcul2

        EquipementSelect = ListEquipement
        RefreshListBouee()

    End Sub

    Private Sub FenetreResultat_Loaded(sender As Object, e As System.Windows.RoutedEventArgs) Handles Me.Loaded

        Title = "Calmar " + Version
        Icon = GetIconCalmar()

#If DEBUG Then
        ResultRibbonGroupPlus.Visibility = Windows.Visibility.Visible
#Else
        ResultRibbonGroupPlus.Visibility = Windows.Visibility.Hidden
#End If

        ' Avancement de la fenetre
        Me.SetValue(Window.TopProperty, Me.GetValue(Window.TopProperty) + 30)

        TextBoxProjectName.Text = ProjectName

        FenetreLoaded = True
        Calcul1.DoCalcul()
        Calcul2.DoCalcul()

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

    Private EtapeMouse As Integer = 0

    Private Sub MenuRibbon_MouseDoubleClick(sender As System.Object, e As System.Windows.Input.MouseButtonEventArgs)

        Select Case EtapeMouse

            Case 0
                If e.RightButton = MouseButtonState.Pressed Then EtapeMouse = 1

            Case 1
                If e.LeftButton = MouseButtonState.Pressed Then ResultRibbonGroupPlus.Visibility = Windows.Visibility.Visible Else EtapeMouse = 0

        End Select

    End Sub

    Private Function SortDictionaryDNAsc(ByVal dict As Dictionary(Of Double, List(Of String))) As Dictionary(Of Double, List(Of String))

        Dim final = From key In dict.Keys Order By key Ascending Select key

        SortDictionaryDNAsc = New Dictionary(Of Double, List(Of String))
        For Each item In final
            SortDictionaryDNAsc.Add(item, dict(item))
        Next

    End Function

    Private Sub RefreshListBouee()

        BoueeComparateur1.Items.Clear()
        BoueeComparateur2.Items.Clear()
        For Each Bouee In BaseBouees.GetBouees
            BoueeComparateur1.Items.Add(Bouee.Value.Nom)
            BoueeComparateur2.Items.Add(Bouee.Value.Nom)

            If Bouee.Value.Nom = Calcul1.BUOY.Nom Then BoueeComparateur1.SelectedIndex = BoueeComparateur1.Items.Count - 1
            If Calcul2.BUOY IsNot Nothing AndAlso Bouee.Value.Nom = Calcul2.BUOY.Nom Then BoueeComparateur2.SelectedIndex = BoueeComparateur2.Items.Count - 1
        Next

        ChaineQualiteComparateur1.SelectedIndex = Calcul1.QualiteChaine - 1
        ChaineQualiteComparateur2.SelectedIndex = Calcul2.QualiteChaine - 1

    End Sub

#Region "Gestion de la selection des elements de la premiere bouée"

    Private Sub BoueeComparateur1_SelectionChanged(sender As System.Object, e As System.Windows.Controls.SelectionChangedEventArgs)

        ' Mise en place de la selection de la bouée
        Calcul1.BUOY = BaseBouees.GetBouees(BoueeComparateur1.SelectedItem)
        RefreshListLest(Calcul1, ComboBoxLestComparateur1)

        Dim final = From key In BaseGeneral.GetChaines
                    Select key Where key.DN >= Calcul1.BUOY.ChaineMin And key.DN <= Calcul1.BUOY.ChaineMax
                    Order By key.DN Ascending
                    Group By key.DN Into Group

        ' Mise en place de la selection de la chaine
        ChaineDNComparateur1.Items.Clear()
        For Each Chaine In final
            ChaineDNComparateur1.Items.Add(Chaine.DN)
            If Chaine.DN = Calcul1.CHAIN.DN Then ChaineDNComparateur1.SelectedIndex = ChaineDNComparateur1.Items.Count - 1
        Next

    End Sub

    Private Sub ChaineDNComparateur1_SelectionChanged(sender As System.Object, e As System.Windows.Controls.SelectionChangedEventArgs)

        If ChaineDNComparateur1.SelectedIndex < 0 Then Return

        Dim final = From key In BaseGeneral.GetChaines Order By key Ascending Select key Where key.DN = ChaineDNComparateur1.SelectedItem

        ChaineTypeComparateur1.Items.Clear()
        For Each Chaine In final
            ChaineTypeComparateur1.Items.Add(Chaine.TYPE)
            If Chaine.TYPE = Calcul1.CHAIN.TYPE Then ChaineTypeComparateur1.SelectedIndex = ChaineTypeComparateur1.Items.Count - 1
        Next

    End Sub

    Private Sub ChaineTypeComparateur1_SelectionChanged(sender As System.Object, e As System.Windows.Controls.SelectionChangedEventArgs)

        If ChaineTypeComparateur1.SelectedIndex < 0 Then Return

        Dim final = From key In BaseGeneral.GetChaines Order By key Ascending Select key Where key.DN = ChaineDNComparateur1.SelectedItem And key.TYPE = ChaineTypeComparateur1.SelectedItem

        If final.Count = 1 Then
            Calcul1.CHAIN = final(0)
            If FenetreLoaded Then Calcul1.DoCalcul()
        End If

    End Sub

    Private Sub ChaineQualiteComparateur1_SelectionChanged(sender As System.Object, e As System.Windows.Controls.SelectionChangedEventArgs)

        Calcul1.QualiteChaine = ChaineQualiteComparateur1.SelectedIndex + 1
        If FenetreLoaded Then Calcul1.DoCalcul()

    End Sub

    Private Sub ComboBoxDensiteCM1_SelectionChanged(sender As System.Object, e As System.Windows.Controls.SelectionChangedEventArgs)

        If ResultComboBoxDensiteCM1.SelectedItem Is Nothing Then Return

        Calcul1.DensiteCM = Double.Parse(ResultComboBoxDensiteCM1.SelectedItem.Split(" ")(0), EnglishFormat)
        If FenetreLoaded Then Calcul1.DoCalcul()

    End Sub

    Private Sub ComboBoxLestComparateur1_SelectionChanged(sender As System.Object, e As System.Windows.Controls.SelectionChangedEventArgs)

        If ComboBoxLestComparateur1.SelectedItem Is Nothing Then
            Calcul1.Lest = New CBouee.CLest("")
        Else
            Calcul1.Lest = New CBouee.CLest(ComboBoxLestComparateur1.SelectedItem)
        End If

        If FenetreLoaded Then Calcul1.DoCalcul()

    End Sub

    Private Sub ButtonLestComparateur1_Click(sender As System.Object, e As System.Windows.RoutedEventArgs)
        ComboBoxLestComparateur1.SelectedItem = Nothing
    End Sub

#End Region

#Region "Gestion de la selection des elements de la deuxieme bouée"

    Private Sub BoueeComparateur2_SelectionChanged(sender As System.Object, e As System.Windows.Controls.SelectionChangedEventArgs)

        Calcul2.BUOY = BaseBouees.GetBouees(BoueeComparateur2.SelectedItem)
        RefreshListLest(Calcul2, ComboBoxLestComparateur2)

        Dim final = From key In BaseGeneral.GetChaines
                    Select key Where key.DN >= Calcul2.BUOY.ChaineMin And key.DN <= Calcul2.BUOY.ChaineMax
                    Order By key.DN Ascending
                    Group By key.DN Into Group

        ' Mise en place de la selection de la chaine
        ChaineDNComparateur2.Items.Clear()
        For Each Chaine In final
            ChaineDNComparateur2.Items.Add(Chaine.DN)
            If Chaine.DN = Calcul1.CHAIN.DN Then ChaineDNComparateur2.SelectedIndex = ChaineDNComparateur2.Items.Count - 1
        Next

    End Sub

    Private Sub ChaineDNComparateur2_SelectionChanged(sender As System.Object, e As System.Windows.Controls.SelectionChangedEventArgs)

        If ChaineDNComparateur2.SelectedIndex < 0 Then Return

        Dim final = From key In BaseGeneral.GetChaines Order By key Ascending Select key Where key.DN = ChaineDNComparateur2.SelectedItem

        ChaineTypeComparateur2.Items.Clear()
        For Each Chaine In final
            ChaineTypeComparateur2.Items.Add(Chaine.TYPE)
            If Chaine.TYPE = Calcul2.CHAIN.TYPE Then ChaineTypeComparateur2.SelectedIndex = ChaineTypeComparateur2.Items.Count - 1
        Next

    End Sub

    Private Sub ChaineTypeComparateur2_SelectionChanged(sender As System.Object, e As System.Windows.Controls.SelectionChangedEventArgs)

        If ChaineTypeComparateur2.SelectedIndex < 0 Then Return

        Dim final = From key In BaseGeneral.GetChaines Order By key Ascending Select key Where key.DN = ChaineDNComparateur2.SelectedItem And key.TYPE = ChaineTypeComparateur2.SelectedItem

        If final.Count = 1 Then
            Calcul2.CHAIN = final(0)
            If FenetreLoaded Then Calcul2.DoCalcul()
        End If

    End Sub

    Private Sub ChaineQualiteComparateur2_SelectionChanged(sender As System.Object, e As System.Windows.Controls.SelectionChangedEventArgs)

        Calcul2.QualiteChaine = ChaineQualiteComparateur2.SelectedIndex + 1
        If FenetreLoaded Then Calcul2.DoCalcul()

    End Sub

    Private Sub ComboBoxDensiteCM2_SelectionChanged(sender As System.Object, e As System.Windows.Controls.SelectionChangedEventArgs)

        If ResultComboBoxDensiteCM2.SelectedItem Is Nothing Then Return

        Calcul2.DensiteCM = Double.Parse(ResultComboBoxDensiteCM2.SelectedItem.Split(" ")(0), EnglishFormat)
        If FenetreLoaded Then Calcul2.DoCalcul()

    End Sub

    Private Sub ComboBoxLestComparateur2_SelectionChanged(sender As System.Object, e As System.Windows.Controls.SelectionChangedEventArgs)

        If ComboBoxLestComparateur2.SelectedItem Is Nothing Then
            Calcul2.Lest = New CBouee.CLest("")
        Else
            Calcul2.Lest = New CBouee.CLest(ComboBoxLestComparateur2.SelectedItem)
        End If

        If FenetreLoaded Then Calcul2.DoCalcul()

    End Sub

    Private Sub ButtonLestComparateur2_Click(sender As System.Object, e As System.Windows.RoutedEventArgs)
        ComboBoxLestComparateur2.SelectedItem = Nothing
    End Sub

#End Region

    Private Sub RefreshListLest(ByVal CalculEnCours As CCalculMouillage, ByRef ComboBoxLest As ComboBox)

        ComboBoxLest.Items.Clear()
        For Each lest In CalculEnCours.BUOY.LestBouee
            ComboBoxLest.Items.Add(lest.ToString)
            If CalculEnCours.Lest IsNot Nothing AndAlso lest.ToString = CalculEnCours.Lest.ToString Then ComboBoxLest.SelectedItem = lest.ToString
        Next

        If ComboBoxLest.Items.Count = 0 AndAlso CalculEnCours.Lest IsNot Nothing Then CalculEnCours.Lest = New CBouee.CLest("")

    End Sub

#Region "Affichage retour de calcul"

    Private Sub RetourCalcul1(ByVal RetourCalcul As CCalculMouillage.STR_RET_CALCUL_RESULTAT)

        ' Resultat Comparateur 1
        TextBoxProfMax.Text = Math.Round(RetourCalcul.ProfondeurMax, 1).ToString(EnglishFormat) + " m"
        TextBoxLongueurCat1.Text = Math.Round(RetourCalcul.LongueurCat, 1).ToString(EnglishFormat) + " m"
        TextBoxTensionMaxMouillage1.Text = Math.Round(RetourCalcul.TensionMaxMouillage, 2).ToString(EnglishFormat) + " t"
        TextBoxMasseMinCM1.Text = Math.Round(RetourCalcul.MasseMinimaleCM, 1).ToString(EnglishFormat) + " t"
        TextBoxReserveFlota1.Text = Math.Round(RetourCalcul.ReserveFlotabilite, 0).ToString(EnglishFormat) + " %"
        TextBoxFrancBord1.Text = Math.Round(RetourCalcul.FrancBordBouee, 2).ToString(EnglishFormat) + " m"
        TextBoxSurfaceLatEmergee1.Text = Math.Round(RetourCalcul.SurfaceLateraleEmergee, 1).ToString(EnglishFormat) + " m²"
        TextBoxProfMin.Text = Math.Round(RetourCalcul.ProfondeurMin, 1).ToString(EnglishFormat) + " m"
        TextBoxRayonEvitage1.Text = Math.Round(RetourCalcul.RayonEvitage, 1).ToString(EnglishFormat) + " m"
        TextBoxCoefSecuriteChaine1.Text = Math.Round(RetourCalcul.CoefSecChaine, 1).ToString(EnglishFormat)
        TextBoxTraineeHorizontale1.Text = Math.Round(RetourCalcul.EffortHorizontalKg / 1000, 1).ToString(EnglishFormat) + " t"
        TextBoxDeplacementTotalB1.Text = Math.Round(RetourCalcul.DeplacementTotal, 1).ToString(EnglishFormat) + " t"
        TextBoxTirantEauBouee1.Text = Math.Round(RetourCalcul.TirantEau, 2).ToString(EnglishFormat) + " m"
        TextBoxSurfaceLateraleImmergee1.Text = Math.Round(RetourCalcul.SurfaceLateraleImmergee, 1).ToString(EnglishFormat) + " m²"

        TextBoxDiamChaine1.Text = Math.Round(RetourCalcul.DiametreChaine, 0).ToString(EnglishFormat) + " mm"
        TextBoxTypeChaine1.Text = RetourCalcul.Type
        TextBoxDensiteCM1.Text = Math.Round(RetourCalcul.DensiteCM, 1).ToString(EnglishFormat) + " t/m³"
        If Calcul1.Lest IsNot Nothing Then TextBoxLest1.Text = Calcul1.Lest.ToString Else TextBoxLest1.Text = ""

        AffichageParametre(RetourCalcul)

        LineChart1.Title = Calcul1.BUOY.Nom
        System.Threading.ThreadPool.QueueUserWorkItem(New System.Threading.WaitCallback(AddressOf ThreadDefinitionGraphique), New Object() _
                                                      {LineChart1, _
                                                       RetourCalcul.EffortHorizontalKg, _
                                                       RetourCalcul.PoidsLineiqueImmergeChaine, _
                                                       RetourCalcul.HauteurCatenaire, _
                                                       RetourCalcul.ProfondeurMax})
        'DefinitionGraphique(LineChart1, GetCatenairePoints(RetourCalcul.EffortHorizontalKg, RetourCalcul.PoidsLineiqueImmergeChaine, RetourCalcul.ProfondeurMax))

        Resultat1 = RetourCalcul

    End Sub

    Private Sub RetourCalcul2(ByVal RetourCalcul As CCalculMouillage.STR_RET_CALCUL_RESULTAT)

        ' Resultat Comparateur 2
        TextBoxLongueurCat2.Text = Math.Round(RetourCalcul.LongueurCat, 1).ToString(EnglishFormat) + " m"
        TextBoxTensionMaxMouillage2.Text = Math.Round(RetourCalcul.TensionMaxMouillage, 2).ToString(EnglishFormat) + " t"
        TextBoxMasseMinCM2.Text = Math.Round(RetourCalcul.MasseMinimaleCM, 1).ToString(EnglishFormat) + " t"
        TextBoxReserveFlota2.Text = Math.Round(RetourCalcul.ReserveFlotabilite, 0).ToString(EnglishFormat) + " %"
        TextBoxFrancBord2.Text = Math.Round(RetourCalcul.FrancBordBouee, 2).ToString(EnglishFormat) + " m"
        TextBoxSurfaceLatEmergee2.Text = Math.Round(RetourCalcul.SurfaceLateraleEmergee, 1).ToString(EnglishFormat) + " m²"
        TextBoxRayonEvitage2.Text = Math.Round(RetourCalcul.RayonEvitage, 1).ToString(EnglishFormat) + " m"
        TextBoxCoefSecuriteChaine2.Text = Math.Round(RetourCalcul.CoefSecChaine, 1).ToString(EnglishFormat)
        TextBoxTraineeHorizontale2.Text = Math.Round(RetourCalcul.EffortHorizontalKg / 1000, 1).ToString(EnglishFormat) + " t"
        TextBoxDeplacementTotalB2.Text = Math.Round(RetourCalcul.DeplacementTotal, 1).ToString(EnglishFormat) + " t"
        TextBoxTirantEauBouee2.Text = Math.Round(RetourCalcul.TirantEau, 2).ToString(EnglishFormat) + " m"
        TextBoxSurfaceLateraleImmergee2.Text = Math.Round(RetourCalcul.SurfaceLateraleImmergee, 1).ToString(EnglishFormat) + " m²"

        TextBoxDiamChaine2.Text = Math.Round(RetourCalcul.DiametreChaine, 0).ToString(EnglishFormat) + " mm"
        TextBoxTypeChaine2.Text = RetourCalcul.Type
        TextBoxDensiteCM2.Text = Math.Round(RetourCalcul.DensiteCM, 1).ToString(EnglishFormat) + " t/m³"
        If Calcul2.Lest IsNot Nothing Then TextBoxLest2.Text = Calcul2.Lest.ToString Else TextBoxLest2.Text = ""

        AffichageParametre(RetourCalcul)

        LineChart2.Title = Calcul2.BUOY.Nom
        System.Threading.ThreadPool.QueueUserWorkItem(New System.Threading.WaitCallback(AddressOf ThreadDefinitionGraphique), New Object() _
                                                      {LineChart2, _
                                                       RetourCalcul.EffortHorizontalKg, _
                                                       RetourCalcul.PoidsLineiqueImmergeChaine, _
                                                       RetourCalcul.HauteurCatenaire, _
                                                       RetourCalcul.ProfondeurMax})
        'DefinitionGraphique(LineChart2, GetCatenairePoints(RetourCalcul.EffortHorizontalKg, RetourCalcul.PoidsLineiqueImmergeChaine, RetourCalcul.ProfondeurMax))

        Resultat2 = RetourCalcul

    End Sub

    Private Sub AffichageParametre(ByVal RetourCalcul As CCalculMouillage.STR_RET_CALCUL_RESULTAT)

        ' Parametre
        TextBoxProfondeur.Text = Math.Round(RetourCalcul.Profondeur, 1).ToString(EnglishFormat) + " m"
        TextBoxMassEquipement.Text = Math.Round(RetourCalcul.MasseEquipement, 0).ToString(EnglishFormat) + " kg"
        TextBoxMarnage.Text = Math.Round(RetourCalcul.Marnage, 1).ToString(EnglishFormat) + " m"
        TextBoxPeriodeVague.Text = Math.Round(RetourCalcul.PeriodeVague, 0).ToString(EnglishFormat) + " s"
        TextBoxVitMaxCms.Text = Math.Round(RetourCalcul.VitMaxCourantMS, 1).ToString(EnglishFormat) + " m/s"
        TextBoxVitMaxCnds.Text = Math.Round(RetourCalcul.VitMaxCourantNDS, 1).ToString(EnglishFormat) + " Nds"
        TextBoxHtMaxVague.Text = Math.Round(RetourCalcul.HauteurMaxVague, 2).ToString(EnglishFormat) + " m"
        TextBoxHtSignifVague.Text = Math.Round(RetourCalcul.HauteurSignifVague, 2).ToString(EnglishFormat) + " m"
        TextBoxVitMaxVentms.Text = Math.Round(RetourCalcul.VitMaxVentMS, 1).ToString(EnglishFormat) + " m/s"
        TextBoxVitMaxVentkmh.Text = Math.Round(RetourCalcul.VitMaxVentKMH, 0).ToString(EnglishFormat) + " km/h"

        ' Avances
        TextBoxDensiteEau.Text = Math.Round(RetourCalcul.DensiteEau, 3).ToString(EnglishFormat)
        TextBoxCoefTrainee.Text = Math.Round(RetourCalcul.CoefTraine, 1).ToString(EnglishFormat)

    End Sub

#End Region

#Region "Gestion du graphique"

    Private Sub lineChart_SizeChanged(sender As System.Object, e As System.Windows.SizeChangedEventArgs)

        RatioPixel = e.NewSize.Height / e.NewSize.Width

    End Sub

    Private Sub MakeAxes(ByVal Profondeur As Double)

        Dim Pt As Dictionary(Of Double, Double)
        Dim ListPointMer As New Dictionary(Of Double, Double)

        Dim MaxX As Double = 0.0
        Dim MaxY As Double = 0.0

        Dim x As Double = 0.0

        Dim xAxis As LinearAxis = lineChart.ActualAxes(0)
        Dim yAxis As LinearAxis = lineChart.ActualAxes(1)

        If LineChart1.DataContext Is Nothing Then Return

        Pt = LineChart1.DataContext

        MaxX = Pt.Last.Key
        MaxY = Profondeur

        If LineChart2.DataContext IsNot Nothing AndAlso LineChart2.DataContext.Count > 0 Then
            Pt = LineChart2.DataContext
            If MaxX < Pt.Last.Key Then MaxX = Pt.Last.Key
        End If

        ' Mise en place d'un axe si la hauteur est plus importante que la rayon d'evitage max de la chaine
        yAxis.Maximum = Profondeur + Profondeur * 0.05
        xAxis.Maximum = yAxis.Maximum / RatioPixel

        ' Si l'axe X maximun est inferieur a la valeur max alors on fait un ratio par rapport a l'axe des abscisse
        If xAxis.Maximum < MaxX Then
            xAxis.Maximum = MaxX + (MaxX * 0.05)
            yAxis.Maximum = MaxX * RatioPixel
        End If

        ' Mise en place du fond de la mer
        ListPointMer.Add(0, Profondeur)
        ListPointMer.Add(xAxis.Maximum, Profondeur)

        AreaProfondeur.IndependentValuePath = "Key"
        AreaProfondeur.DependentValuePath = "Value"

        AreaProfondeur.SetBinding(LineSeries.ItemsSourceProperty, New Binding())
        AreaProfondeur.DataContext = ListPointMer

    End Sub

    Private Sub DefinitionGraphique(ByRef catanaryChain As LineSeries, ByVal Values As IDictionary(Of Double, Double), ByVal Profondeur As Double)

        catanaryChain.IndependentValuePath = "Key"
        catanaryChain.DependentValuePath = "Value"

        catanaryChain.SetBinding(LineSeries.ItemsSourceProperty, New Binding())

        catanaryChain.DataContext = Nothing
        catanaryChain.DataContext = Values

        If RatioPixel > 0 Then MakeAxes(Profondeur)

    End Sub

    Private Sub ThreadDefinitionGraphique(ByVal state As Object)

        AppelRefreshGraphique(state(0), GetCatenairePoints(state(1), state(2), state(3)), state(4))

    End Sub

    Private Sub AppelRefreshGraphique(ByVal catanaryChain As LineSeries, ByVal Values As IDictionary(Of Double, Double), ByVal Profondeur As Double)

        Me.Dispatcher().BeginInvoke(New DelegateRefreshGraphique(AddressOf RefreshGraphique), {catanaryChain, Values, Profondeur})

    End Sub

    Private Delegate Sub DelegateRefreshGraphique(ByVal catanaryChain As LineSeries, ByVal Values As IDictionary(Of Double, Double), ByVal Profondeur As Double)

    Private Sub RefreshGraphique(ByVal catanaryChain As LineSeries, ByVal Values As IDictionary(Of Double, Double), ByVal Profondeur As Double)

        catanaryChain.IndependentValuePath = "Key"
        catanaryChain.DependentValuePath = "Value"

        catanaryChain.SetBinding(LineSeries.ItemsSourceProperty, New Binding())

        catanaryChain.DataContext = Nothing
        catanaryChain.DataContext = Values

        If RatioPixel > 0 Then MakeAxes(Profondeur)

    End Sub

    Private Function GetCatenairePoints(ByVal ForceH As Double, ByVal PoidsLineique As Double, ByVal Profondeur As Double) As Dictionary(Of Double, Double)

        Dim PasEvolutif As Double = 0.5
        Dim valueList As New Dictionary(Of Double, Double)

        Dim EffortPoids As Double = (ForceH / PoidsLineique)
        Dim y As Double = 0
        Dim x As Double = 0.0
        Dim xMax As Double = 0.0

        If EffortPoids < 1 Then EffortPoids = 1

        xMax = EffortPoids * ACosH((Profondeur / EffortPoids) + 1)
        PasEvolutif = xMax / 200
        y = EffortPoids * (Math.Cosh(x / EffortPoids) - 1)

        Try
            Do
                valueList.Add(x, y)

                ' Deplacement de 0.5m
                x += PasEvolutif
                y = EffortPoids * (Math.Cosh(x / EffortPoids) - 1)
            Loop While y < Profondeur
            valueList.Add(xMax, Profondeur)
        Catch
        End Try

        Return valueList

    End Function

    Private Function ACosH(ByVal value As Double) As Double
        Return Math.Log(value + Math.Sqrt(value * value - 1))
    End Function

#End Region

    Private Sub HideFenetre()

        Do
            System.Threading.Thread.Sleep(100)
        Loop While Not WaitEndFunction

    End Sub

    Private Sub CopyResultToDocument(ByRef FlowDoc As DocumentResultatMooring)

        With FlowDoc

            .TextBoxProfMax.Text = TextBoxProfMax.Text
            .TextBoxProfMin.Text = TextBoxProfMin.Text

            ' Resultat Comparateur 1
            .TextBoxLongueurCat1.Text = TextBoxLongueurCat1.Text
            .TextBoxTensionMaxMouillage1.Text = TextBoxTensionMaxMouillage1.Text
            .TextBoxMasseMinCM1.Text = TextBoxMasseMinCM1.Text
            .TextBoxReserveFlota1.Text = TextBoxReserveFlota1.Text
            .TextBoxFrancBord1.Text = TextBoxFrancBord1.Text
            .TextBoxSurfaceLatEmergee1.Text = TextBoxSurfaceLatEmergee1.Text
            .TextBoxRayonEvitage1.Text = TextBoxRayonEvitage1.Text
            .TextBoxCoefSecuriteChaine1.Text = TextBoxCoefSecuriteChaine1.Text
            .TextBoxTraineeHorizontale1.Text = TextBoxTraineeHorizontale1.Text
            .TextBoxDeplacementTotalB1.Text = TextBoxDeplacementTotalB1.Text
            .TextBoxTirantEauBouee1.Text = TextBoxTirantEauBouee1.Text
            .TextBoxSurfaceLateraleImmergee1.Text = TextBoxSurfaceLateraleImmergee1.Text

            .TextBoxDiamChaine1.Text = TextBoxDiamChaine1.Text
            .TextBoxTypeChaine1.Text = TextBoxTypeChaine1.Text
            .TextBoxDensiteCM1.Text = TextBoxDensiteCM1.Text
            .TextBoxLest1.Text = TextBoxLest1.Text

            ' Resultat Comparateur 2
            .TextBoxLongueurCat2.Text = TextBoxLongueurCat2.Text
            .TextBoxTensionMaxMouillage2.Text = TextBoxTensionMaxMouillage2.Text
            .TextBoxMasseMinCM2.Text = TextBoxMasseMinCM2.Text
            .TextBoxReserveFlota2.Text = TextBoxReserveFlota2.Text
            .TextBoxFrancBord2.Text = TextBoxFrancBord2.Text
            .TextBoxSurfaceLatEmergee2.Text = TextBoxSurfaceLatEmergee2.Text
            .TextBoxRayonEvitage2.Text = TextBoxRayonEvitage2.Text
            .TextBoxCoefSecuriteChaine2.Text = TextBoxCoefSecuriteChaine2.Text
            .TextBoxTraineeHorizontale2.Text = TextBoxTraineeHorizontale2.Text
            .TextBoxDeplacementTotalB2.Text = TextBoxDeplacementTotalB2.Text
            .TextBoxTirantEauBouee2.Text = TextBoxTirantEauBouee2.Text
            .TextBoxSurfaceLateraleImmergee2.Text = TextBoxSurfaceLateraleImmergee2.Text

            .TextBoxDiamChaine2.Text = TextBoxDiamChaine2.Text
            .TextBoxTypeChaine2.Text = TextBoxTypeChaine2.Text
            .TextBoxDensiteCM2.Text = TextBoxDensiteCM2.Text
            .TextBoxLest2.Text = TextBoxLest2.Text

            ' Parametre
            .TextBoxProfondeur.Text = TextBoxProfondeur.Text
            .TextBoxMassEquipement.Text = TextBoxMassEquipement.Text
            .TextBoxMarnage.Text = TextBoxMarnage.Text
            .TextBoxPeriodeVague.Text = TextBoxPeriodeVague.Text
            .TextBoxVitMaxCms.Text = TextBoxVitMaxCms.Text
            .TextBoxVitMaxCnds.Text = TextBoxVitMaxCnds.Text
            .TextBoxHtMaxVague.Text = TextBoxHtMaxVague.Text
            .TextBoxHtSignifVague.Text = TextBoxHtSignifVague.Text
            .TextBoxVitMaxVentms.Text = TextBoxVitMaxVentms.Text
            .TextBoxVitMaxVentkmh.Text = TextBoxVitMaxVentkmh.Text

            'Avancés
            .TextBoxDensiteEau.Text = TextBoxDensiteEau.Text
            .TextBoxCoefTrainee.Text = TextBoxCoefTrainee.Text

            ' Mise en place des noms des bouée
            If Calcul1.BUOY IsNot Nothing Then .NomBouee1.Text = Calcul1.BUOY.Nom
            If Calcul2.BUOY IsNot Nothing Then .NomBouee2.Text = Calcul2.BUOY.Nom

            'Affichage de l'image
            .PrintChartImage.Source = GetChartImage()

            .NomLogiciel.Text = ProjectName
            .DateJour.Text = Now.ToString()
            .VersionLogiciel.Text = My.Application.Info.AssemblyName + " V" + getVersion().ToString
        End With

    End Sub

    Private Function getVersion() As Version

        Dim version As String = System.Reflection.Assembly.GetExecutingAssembly().FullName
        Dim start As Integer = version.IndexOf("Version=") + 8
        Dim length As Integer = version.IndexOf(",", start) - start

        Return New Version(version.Substring(start, length))

    End Function

    Private Function GetChartImage() As RenderTargetBitmap

        GetChartImage = New RenderTargetBitmap(lineChart.ActualWidth, lineChart.ActualHeight, 96D, 96D, PixelFormats.Pbgra32)
        GetChartImage.Render(lineChart)

    End Function

    Private Sub PrintButton_Click(sender As System.Object, e As RoutedEventArgs)

        Dim PaddingSize As Integer = 50
        Dim DocMooring As New DocumentResultatMooring
        Dim NewPrintDialogue As New PrintDialog

        NewPrintDialogue.PrintQueue = LocalPrintServer.GetDefaultPrintQueue()
        NewPrintDialogue.PrintTicket = NewPrintDialogue.PrintQueue.DefaultPrintTicket()
        NewPrintDialogue.PrintTicket.PageOrientation = PageOrientation.Landscape

        If NewPrintDialogue.ShowDialog Then

            ' Affichage de la fenetre d'Attente
            WaitEndFunction = False
            WaitTemplate = New FenetreWait
            AddHandler WaitTemplate.Worker.DoWork, AddressOf HideFenetre
            WaitTemplate.Show()

            ' Mise en place de la taille papier
            NewPrintDialogue.PrintTicket.PageMediaSize = New PageMediaSize(PageMediaSizeName.ISOA4)

            ' Changement de la Langue
            ChangeLangueInLayout(DocMooring.LayoutRoot)

            ' Mise en place de la taille du docuement
            DocMooring.PageHeight = NewPrintDialogue.PrintableAreaHeight
            DocMooring.PageWidth = NewPrintDialogue.PrintableAreaWidth

            ' Mise en place de la taille canvas
            DocMooring.LayoutRoot.Width = DocMooring.PageWidth - PaddingSize * 2
            DocMooring.LayoutRoot.Height = DocMooring.PageHeight - PaddingSize * 2

            DocMooring.GridPrintCanvas.Width = DocMooring.LayoutRoot.Width * 0.8
            DocMooring.GridPrintCanvas.Height = DocMooring.LayoutRoot.Height * 0.8

            ' Mise en place de la taille grid et canvas
            DocMooring.PagePadding = New Thickness(PaddingSize)
            DocMooring.ColumnGap = 0
            DocMooring.ColumnWidth = NewPrintDialogue.PrintableAreaWidth

            CopyResultToDocument(DocMooring)

            Dim dps As IDocumentPaginatorSource = DocMooring
            NewPrintDialogue.PrintDocument(dps.DocumentPaginator, ProjectName)

        End If

        WaitEndFunction = True

    End Sub

    Private Sub ExportButton_Click(sender As System.Object, e As RoutedEventArgs)

        ExportResultatCSV()

    End Sub

    Private Sub ResultButtonVisualisation_Click(sender As System.Object, e As System.Windows.RoutedEventArgs)

        Dim FenetreVisu As New FenetreVisualisation(Calcul1, Calcul2)

        FenetreVisu.Owner = Me
        FenetreVisu.ShowDialog()
        FenetreVisu = Nothing

    End Sub

    Private Sub ExportResultatCSV()

        Dim NomBouee1 As String = ""
        Dim NomBouee2 As String = ""

        Dim Sep As Char = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator
        Dim SaveFs As IO.StreamWriter
        Dim FileOp As New Microsoft.Win32.SaveFileDialog()

        FileOp.FileName = ProjectName + ".txt" ' Default file name
        FileOp.DefaultExt = ".txt" ' Default file extension
        FileOp.Filter = GetTextLangue("TextBlockCSVPointVirg", "Fichier Txt point virgule") + " (.Txt)|*.txt|" + GetTextLangue("TextBlockCSVVirg", "Fichier Txt virgule") + " (.Txt)|*.txt|" + GetTextLangue("TextBlockCSVTab", "Fichier Txt Tabulation") + " (.txt)|*.txt" ' Filter files by extension

        ' Process open file dialog box results
        If FileOp.ShowDialog() Then

            ' Selection du separateur pour le fichier
            Select Case FileOp.FilterIndex

                Case 1
                    Sep = ";"

                Case 2
                    Sep = ","

                Case 3
                    Sep = vbTab

            End Select

            ' Affichage de la fenetre d'Attente
            WaitEndFunction = False
            WaitTemplate = New FenetreWait
            AddHandler WaitTemplate.Worker.DoWork, AddressOf HideFenetre
            WaitTemplate.Show()

            ' Ouverture du fichier pour l'ecriture
            Try
                SaveFs = New IO.StreamWriter(FileOp.FileName, False, System.Text.Encoding.UTF8)
            Catch
                Return
            End Try

            Try
                SaveFs.WriteLine(GetTextLangue("GeneralTextBlockProject", "Project") + Sep + ProjectName)
                SaveFs.WriteLine("Version" + Sep + My.Application.Info.AssemblyName + " V" + getVersion().ToString)
                SaveFs.WriteLine("Date" + Sep + Now.ToString)
                SaveFs.WriteLine("")

                SaveFs.WriteLine(GetTextLangue("GeneralTextBlockParametre", "Parametre"))
                SaveFs.WriteLine("")

                SaveFs.WriteLine(GetTextLangue("ResultTextBlockDensiteEau", "Densité de l'eau") + Sep + Math.Round(Resultat1.DensiteEau, 0).ToString())
                SaveFs.WriteLine(GetTextLangue("ResultTextBlockCoefTrainee", "Coef. trainée") + Sep + Math.Round(Resultat1.CoefTraine, 1).ToString())

                SaveFs.WriteLine(GetTextLangue("MainButtonSiteParametre", "Paramètre avancés"))
                SaveFs.WriteLine("")
                SaveFs.WriteLine(GetTextLangue("ResultTextBlockProfondeur", "Profondeur") + Sep + Math.Round(Resultat1.Profondeur, 0).ToString() + Sep + "m")
                SaveFs.WriteLine(GetTextLangue("ResultTextBlockMarnage", "Marnage") + Sep + Math.Round(Resultat1.Marnage, 1).ToString() + Sep + "m")
                SaveFs.WriteLine("")

                SaveFs.WriteLine(GetTextLangue("ResultTextBlockHtMaxVagues", "Hauteur max. vagues") + Sep + Math.Round(Resultat1.HauteurMaxVague, 1).ToString() + Sep + "m")
                SaveFs.WriteLine(GetTextLangue("ResultTextBlockHtSignifVague", "Hauteur significative vagues") + Sep + Math.Round(Resultat1.HauteurSignifVague, 1).ToString() + Sep + "m")
                SaveFs.WriteLine(GetTextLangue("ResultTextBlockPeriodeVague", "Periode vagues") + Sep + Math.Round(Resultat1.PeriodeVague, 0).ToString() + Sep + "s")

                SaveFs.WriteLine(GetTextLangue("ResultTextBlockVitCourantMS", "Vitesse courant") + Sep + Math.Round(Resultat1.VitMaxCourantMS, 1).ToString() + Sep + "m/s")
                SaveFs.WriteLine(GetTextLangue("ResultTextBlockVitCourantNds", "Vitesse courant") + Sep + Math.Round(Resultat1.VitMaxCourantNDS, 1).ToString() + Sep + "Nds")

                SaveFs.WriteLine(GetTextLangue("ResultTextBlockVitVentMS", "Vitesse vent") + Sep + Math.Round(Resultat1.VitMaxVentMS, 0).ToString() + Sep + "m/s")
                SaveFs.WriteLine(GetTextLangue("ResultTextBlockVitVentKMH", "Vitesse vent") + Sep + Math.Round(Resultat1.VitMaxVentKMH, 0).ToString() + Sep + "km/h")

                SaveFs.WriteLine("")
                SaveFs.WriteLine(GetTextLangue("EquipTextBlockListBoxEquipement", "Liste des équipements sélectionnés"))
                For Each Categorie In EquipementSelect
                    SaveFs.WriteLine("*" + Categorie.Key)
                    For Each Equip In Categorie.Value
                        SaveFs.WriteLine(Equip.Nombre.ToString + "x " + Equip.Name + Sep + Equip.MasseUnitaire.ToString(EnglishFormat) + Sep + "kg")
                    Next
                Next
                SaveFs.WriteLine(GetTextLangue("ResultTextBlockMasseEquipement", "Masse equipement") + Sep + Math.Round(Resultat1.MasseEquipement, 0).ToString() + Sep + "kg")

                SaveFs.WriteLine("")
                SaveFs.WriteLine(GetTextLangue("ResultTextBlockTitleResultat", "Resultat"))

                SaveFs.WriteLine("")
                SaveFs.WriteLine(GetTextLangue("GeneralTextBlockProfondeurMax", "Profondeur maximale") + Sep + Math.Round(Resultat1.ProfondeurMax, 1).ToString() + Sep + "m")
                SaveFs.WriteLine(GetTextLangue("GeneralTextBlockProfondeurMin", "Profondeur minimale") + Sep + Math.Round(Resultat1.ProfondeurMin, 1).ToString() + Sep + "m")
                SaveFs.WriteLine("")

                NomBouee1 = Calcul1.BUOY.Nom

                SaveFs.WriteLine("")
                SaveFs.WriteLine("" + Sep + NomBouee1)
                SaveFs.WriteLine("")
                SaveFs.WriteLine(GetTextLangue("ResultTextBlockLongueurCat", "Longueur Catenaire") + Sep + Math.Round(Resultat1.LongueurCat, 1).ToString() + Sep + "m")
                SaveFs.WriteLine(GetTextLangue("ResultTextBlockTensionMaxMouillage", "Tension maximale mouillage") + Sep + Math.Round(Resultat1.TensionMaxMouillage, 1).ToString() + Sep + "t")
                SaveFs.WriteLine(GetTextLangue("ResultRibbonGroupDensiteCM", "Densité corps mort") + Sep + Math.Round(Resultat1.DensiteCM, 1).ToString() + Sep + "t/m³")
                SaveFs.WriteLine(GetTextLangue("ResultTextBlockMasseMinCM", "Masse minimale corps mort") + Sep + Math.Round(Resultat1.MasseMinimaleCM, 1).ToString() + Sep + "t")
                SaveFs.WriteLine(GetTextLangue("ResultTextBlockReserveFlota", "Reserve flotabilité bouée") + Sep + Math.Round(Resultat1.ReserveFlotabilite, 0).ToString() + Sep + "%")
                SaveFs.WriteLine(GetTextLangue("ResultTextBlockFrancBord", "Franc bord bouée") + Sep + Math.Round(Resultat1.FrancBordBouee, 2).ToString() + Sep + "m")
                SaveFs.WriteLine(GetTextLangue("ResultTextBlockSurfLatImme", "Surface laterale immergee") + Sep + Math.Round(Resultat1.SurfaceLateraleImmergee, 1).ToString() + Sep + "m²")
                SaveFs.WriteLine(GetTextLangue("ResultTextBlockRayonEvitage", "Rayon d'évitage maximale") + Sep + Math.Round(Resultat1.RayonEvitage, 1).ToString() + Sep + "m")
                SaveFs.WriteLine(GetTextLangue("GeneralTextBlockCoefSecuChaineIALA", "Coef. securité chaine (IALA: ≥5\\Ce)") + Sep + Math.Round(Resultat1.CoefSecChaine, 1).ToString())
                SaveFs.WriteLine(GetTextLangue("ResultTextBlockTraineH", "Trainée horizontal") + Sep + Math.Round(Resultat1.EffortHorizontalKg, 1).ToString() + Sep + "kg")
                SaveFs.WriteLine(GetTextLangue("ResultTextBlockDeplTotBuoy", "Deplacement total bouée") + Sep + Math.Round(Resultat1.DeplacementTotal, 1).ToString() + Sep + "t")
                SaveFs.WriteLine(GetTextLangue("ResultTextBlockTirantEau", "Tirant d'eau bouée") + Sep + Math.Round(Resultat1.TirantEau, 2).ToString() + Sep + "m")
                SaveFs.WriteLine(GetTextLangue("ResultTextBlockSurfLatEme", "Surface laterale emergée") + Sep + Math.Round(Resultat1.SurfaceLateraleEmergee, 1).ToString() + Sep + "m²")
                SaveFs.WriteLine("")
                SaveFs.WriteLine(GetTextLangue("ResultTextBlockLest", "Lest") + Sep + Calcul1.Lest.ToString())
                SaveFs.WriteLine(GetTextLangue("GeneralTextBlockDiamChaine", "Diametre chaine") + Sep + Math.Round(Resultat1.DiametreChaine, 0).ToString() + Sep + "mm")
                SaveFs.WriteLine(GetTextLangue("GeneralTextBlockTypeChaine", "Type chaine") + Sep + Resultat1.Type)

                If Calcul2.BUOY IsNot Nothing Then
                    NomBouee2 = Calcul2.BUOY.Nom
                    SaveFs.WriteLine("")
                    SaveFs.WriteLine("")
                    SaveFs.WriteLine("" + Sep + NomBouee2)
                    SaveFs.WriteLine("")
                    SaveFs.WriteLine(GetTextLangue("ResultTextBlockLongueurCat", "Longueur Catenaire") + Sep + Math.Round(Resultat2.LongueurCat, 1).ToString() + Sep + "m")
                    SaveFs.WriteLine(GetTextLangue("ResultTextBlockTensionMaxMouillage", "Tension maximale mouillage") + Sep + Math.Round(Resultat2.TensionMaxMouillage, 1).ToString() + Sep + "t")
                    SaveFs.WriteLine(GetTextLangue("ResultRibbonGroupDensiteCM", "Densité corps mort") + Sep + Math.Round(Resultat2.DensiteCM, 1).ToString() + Sep + "t/m³")
                    SaveFs.WriteLine(GetTextLangue("ResultTextBlockMasseMinCM", "Masse minimale corps mort") + Sep + Math.Round(Resultat2.MasseMinimaleCM, 1).ToString() + Sep + "t")
                    SaveFs.WriteLine(GetTextLangue("ResultTextBlockReserveFlota", "Reserve flotabilité bouée") + Sep + Math.Round(Resultat2.ReserveFlotabilite, 0).ToString() + Sep + "%")
                    SaveFs.WriteLine(GetTextLangue("ResultTextBlockFrancBord", "Franc bord bouée") + Sep + Math.Round(Resultat2.FrancBordBouee, 2).ToString() + Sep + "m")
                    SaveFs.WriteLine(GetTextLangue("ResultTextBlockSurfLatImme", "Surface laterale immergee") + Sep + Math.Round(Resultat2.SurfaceLateraleImmergee, 1).ToString() + Sep + "m²")
                    SaveFs.WriteLine(GetTextLangue("ResultTextBlockRayonEvitage", "Rayon d'évitage maximale") + Sep + Math.Round(Resultat2.RayonEvitage, 1).ToString() + Sep + "m")
                    SaveFs.WriteLine(GetTextLangue("GeneralTextBlockCoefSecuChaineIALA", "Coef. securité chaine (IALA: ≥5\\Ce)") + Sep + Math.Round(Resultat2.CoefSecChaine, 1).ToString())
                    SaveFs.WriteLine(GetTextLangue("ResultTextBlockTraineH", "Trainée horizontal") + Sep + Math.Round(Resultat2.EffortHorizontalKg, 1).ToString() + Sep + "kg")
                    SaveFs.WriteLine(GetTextLangue("ResultTextBlockDeplTotBuoy", "Deplacement total bouée") + Sep + Math.Round(Resultat2.DeplacementTotal, 1).ToString() + Sep + "t")
                    SaveFs.WriteLine(GetTextLangue("ResultTextBlockTirantEau", "Tirant d'eau bouée") + Sep + Math.Round(Resultat2.TirantEau, 2).ToString() + Sep + "m")
                    SaveFs.WriteLine(GetTextLangue("ResultTextBlockSurfLatEme", "Surface laterale emergée") + Sep + Math.Round(Resultat2.SurfaceLateraleEmergee, 1).ToString() + Sep + "m²")
                    SaveFs.WriteLine("")
                    SaveFs.WriteLine(GetTextLangue("ResultTextBlockLest", "Lest") + Sep + Calcul2.Lest.ToString())
                    SaveFs.WriteLine(GetTextLangue("GeneralTextBlockDiamChaine", "Diametre chaine") + Sep + Math.Round(Resultat2.DiametreChaine, 0).ToString() + Sep + "mm")
                    SaveFs.WriteLine(GetTextLangue("GeneralTextBlockTypeChaine", "Type chaine") + Sep + Resultat2.Type)
                End If
            Catch
            End Try

            SaveFs.Close()

            WaitEndFunction = True
        End If


    End Sub

    Private Sub BoutonTest_Click(sender As System.Object, e As System.Windows.RoutedEventArgs)

        'Dim _FenetreDessin As New FenetreDessin(Calcul1.BUOY.Nom, Resultat1)
        '_FenetreDessin.ShowDialog()

    End Sub

    Private Sub SpecialResult_Click(sender As System.Object, e As System.Windows.RoutedEventArgs)

        Dim message As String

        With Resultat1

            message = "Resultat Bouée 1:" + vbCrLf + vbCrLf

            message += "Masse de la bouée (Kg) = " + Math.Round(.MasseBouee, 2).ToString(EnglishFormat) + vbCrLf

            message += "Deplacement totale (Kg) = " + Math.Round(.DeplacementTotal * 1000, 3).ToString(EnglishFormat) + vbCrLf

            message += "Volume deplace (dcm³) = " + Math.Round(.VolumeDeplacer, 3).ToString(EnglishFormat) + vbCrLf

            message += "Reserve de flotabilite (%) = " + Math.Round(.ReserveFlotabilite, 1).ToString(EnglishFormat) + vbCrLf

            message += "Tirant d'eau origine (m) = " + Math.Round(.Enfoncement, 2).ToString(EnglishFormat) + vbCrLf

            message += "Hauteur catenaire (m) = " + Math.Round(.HauteurCatenaire, 2).ToString(EnglishFormat) + vbCrLf

            message += "Effort Horizontal (Kg) = " + Math.Round(.EffortHorizontalKg, 3).ToString(EnglishFormat) + vbCrLf

            message += "Poids lineique immergé chaine (kg) = " + Math.Round(.PoidsLineiqueImmergeChaine, 3).ToString(EnglishFormat) + vbCrLf

            message += "Masse lineique chaine (kg) = " + Math.Round(Calcul1.CHAIN.MASSE_LINEIQUE, 3).ToString(EnglishFormat) + vbCrLf

            message += "Charge d'épreuve (t) = " + Math.Round(Calcul1.CHAIN.CHARGE_EPREUVE(Calcul1.QualiteChaine), 2).ToString(EnglishFormat) + vbCrLf

            message += "Angle tangence (°) = " + Math.Round(.AngleTangence, 3).ToString(EnglishFormat) + vbCrLf

            message += "Poids lest immergé (kg) = " + Math.Round(.PoidsLestImmerge, 3).ToString(EnglishFormat) + vbCrLf

            message += "Trainée vent (kg) = " + Math.Round(.TraineeVent * 1000, 3).ToString(EnglishFormat) + vbCrLf

            message += "Trainée courant surface sur bouee (kg) = " + Math.Round(.TraineeVague * 1000, 3).ToString(EnglishFormat) + vbCrLf

            message += "Trainée courant sur chaine (kg) = " + Math.Round(.TraineeCourant * 1000, 3).ToString(EnglishFormat) + vbCrLf

            message += "Vitesse courant de surface (m/s) = " + Math.Round(.VitesseCourantSurface, 3).ToString(EnglishFormat) + vbCrLf

            message += vbCrLf
            message += "Temps du calcul (ms) = " + .TimeCalcul.TotalMilliseconds.ToString + vbCrLf

        End With

        DisplayInformation(Me, message)

    End Sub

    Private Sub TextBoxProjectName_TextChanged(sender As System.Object, e As System.Windows.Controls.TextChangedEventArgs)

        ProjectName = TextBoxProjectName.Text

    End Sub

#Region "Gestion du changement de langue"

    Private Sub ChangeLangueInLayout(ByVal LayoutRt As Grid)

        Dim obj As Object

        ' Verification que la cle du language exist
        If Not LangDictionnary.ContainsKey(LangueAppli) Then Exit Sub

        For Each Cle In LangDictionnary(LangueAppli)
            obj = LayoutRt.FindName(Cle.Name)
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

                Case GetType(LinearAxis)
                    obj.Title = Cle.Text

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

    Private Sub TextBoxProfondeur_KeyUp(sender As System.Object, e As System.Windows.Input.KeyEventArgs)

        Dim d As Double

        If Not e.Key = Key.Enter Then Return

        Double.TryParse(TextBoxProfondeur.Text, Globalization.NumberStyles.AllowDecimalPoint, EnglishFormat, d)
        If d > 0 And FenetreLoaded Then
            Calcul1.Profondeur = d
            Calcul2.Profondeur = d

            Calcul1.DoCalcul()
            If Calcul2.BUOY IsNot Nothing Then Calcul2.DoCalcul()
        End If

    End Sub

    Private Sub Window_KeyUp(sender As System.Object, e As System.Windows.Input.KeyEventArgs) Handles MyBase.KeyUp

        If e.Key = Key.Escape Then
            Close()
        End If

    End Sub

End Class
