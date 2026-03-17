Imports System.Net
Imports BuoyLib
Imports BuoyLib.Buoy
Imports System.Windows.Controls.Ribbon
Imports System.Data

Class MainWindow

#Region "Gestion des appels de fenetre externe"

    Private Delegate Sub DelegateDisplayAlert(ByVal WindowParent As Window, ByVal Message As String)

#End Region

#Region "Gestion de l'ouverture, deplacement et fermeture de l'application"

    Private VersionTxt As New Version()
    Private FilenameVersion As String = ""
    Private NewInstaller As String = ""

    Public Sub New()

        ' Cet appel est requis par le concepteur.
        InitializeComponent()

        ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().

    End Sub

    Private Sub MainWindow_Activated(sender As Object, e As System.EventArgs) Handles Me.Activated

        ShowMainWindow()

    End Sub

    Private Sub MainWindow_Deactivated(sender As Object, e As System.EventArgs) Handles Me.Deactivated

        HideMainWindow()

    End Sub

    Private Sub MainPage_Loaded(sender As Object, e As System.Windows.RoutedEventArgs) Handles Me.Loaded

        Title = "Calmar " + Version
        Icon = GetIconCalmar()

        FirstCheckDirectory()
        ReadAllLangFile()

#If DEBUG Then
        FonctionDebugEnable()
#Else
        FonctionReleaseEnable()
#End If

        SetMenuSousRepertoire()
        SetRadioButton()
        ChangeLangueInLayout()

        DisplayTreeView()
        SelectWorkingDirectory()

        BaseGeneral.LectureFichierGeneraux()
        LectureBase()

    End Sub

    Private Sub SelectWorkingDirectory()

        Dim it As TreeViewItem = Nothing
        Dim items As ItemCollection

        Dim i As Integer = 0
        Dim s As String() = RepertoireTravail.Split("\")
        Dim tag As String = ""

        items = TreeViewExplorer.Items

        ' Boucle de deffilement des dossiers
        Do
            i = RepertoireTravail.IndexOf("\"c, i) + IIf(i = 0, 1, 0)
            If i = -1 Then tag = RepertoireTravail Else tag = RepertoireTravail.Substring(0, i)
            it = IsItemHeaderExist(items, tag)

            If it Is Nothing Then
                Exit Do
            Else
                it.IsExpanded = True
                items = it.Items
                i += 1
            End If
        Loop While i > 0

        ' Si l'on trouve le repertoire on le selectionne
        If tag = RepertoireTravail And it IsNot Nothing Then
            it.IsSelected = True
        End If

    End Sub

    Private Function IsItemHeaderExist(ByVal Items As ItemCollection, ByVal name As String) As TreeViewItem

        If Items Is Nothing OrElse Items.Count = 0 Then Return Nothing

        For Each itemView As TreeViewItem In Items
            If itemView.Tag = name Then
                Return itemView
            End If
        Next

        Return Nothing

    End Function

    Private Sub DisplayTreeView()

        Dim item As TreeViewItem

        For Each s As String In IO.Directory.GetLogicalDrives
            item = New TreeViewItem
            item.Tag = s

            Dim Stack As New StackPanel
            Stack.Orientation = Orientation.Horizontal

            ' create Image
            Dim Image As New Image() With {.Width = 16, .Height = 16}
            Image.Source = New BitmapImage(New Uri("/Calmar;component/Images/diskdrive.png", UriKind.Relative))

            ' Add into stack
            Stack.Children.Add(Image)
            Stack.Children.Add(New TextBlock With {.Text = s, .TextWrapping = TextWrapping.Wrap})

            item.Header = Stack
            item.FontWeight = FontWeights.Normal
            item.Items.Add(Nothing)

            AddHandler item.Expanded, AddressOf folder_Expanded
            TreeViewExplorer.Items.Add(item)
        Next

    End Sub

    Private Sub folder_Expanded(ByVal sender As Object, ByVal e As RoutedEventArgs)

        Dim item As TreeViewItem = sender

        If Not item.IsExpanded Then Return

        Folder_Refresh_Expand(item)

    End Sub

    Private Sub Folder_Refresh_Expand(ByVal item As TreeViewItem)

        Dim Image As New Image
        Dim Stack As StackPanel
        Dim s As String

        folder_image(item)

        If (item.Items.Count = 1 AndAlso item.Items(0) Is Nothing) Then

            item.Items.Clear()
            Try

                For Each s In IO.Directory.GetDirectories(item.Tag.ToString())
                    Dim subitem As New TreeViewItem
                    subitem.Tag = s

                    Stack = New StackPanel
                    Stack.Orientation = Orientation.Horizontal

                    ' create Image
                    Image = New Image() With {.Width = 16, .Height = 16}
                    Image.Source = New BitmapImage(New Uri("/Calmar;component/Images/folder-closed.png", UriKind.Relative))

                    ' Add into stack
                    Stack.Children.Add(Image)
                    Stack.Children.Add(New TextBlock With {.Text = s.Substring(s.LastIndexOf("\") + 1), .TextWrapping = TextWrapping.Wrap})

                    subitem.Header = Stack
                    subitem.FontWeight = FontWeights.Normal
                    subitem.Items.Add(Nothing)
                    AddHandler subitem.Expanded, AddressOf folder_Expanded
                    AddHandler subitem.Unselected, AddressOf folder_Unselect
                    AddHandler subitem.Selected, AddressOf folder_Select
                    item.Items.Add(subitem)
                Next
            Catch ex As Exception

            End Try
        End If
    End Sub

    Private Sub folder_Unselect(ByVal sender As Object, ByVal e As RoutedEventArgs)

        Dim item As TreeViewItem = sender

        item.IsExpanded = False
        folder_image(item)

    End Sub

    Private Sub folder_Select(ByVal sender As Object, ByVal e As RoutedEventArgs)

        Dim item As TreeViewItem = sender

        item.IsExpanded = True
        folder_image(item)

    End Sub

    Private Sub folder_Collapsed(ByVal sender As Object, ByVal e As RoutedEventArgs)

        folder_image(sender)

    End Sub

    Private Sub folder_image(ByVal item As TreeViewItem)

        Dim Image As New Image
        Dim Stack As StackPanel
        Dim s As String

        s = item.Tag
        If Not s.EndsWith("\") Then
            Stack = item.Header
            Image = Stack.Children(0)

            If item.IsExpanded Then
                Image.Source = New BitmapImage(New Uri("/Calmar;component/Images/folder-open.png", UriKind.Relative))
            Else
                If item.IsSelected Then
                    Image.Source = New BitmapImage(New Uri("/Calmar;component/Images/folder-open.png", UriKind.Relative))
                Else
                    Image.Source = New BitmapImage(New Uri("/Calmar;component/Images/folder-closed.png", UriKind.Relative))
                End If
            End If
        End If

    End Sub

    Private Sub TreeViewExplorer_SelectedItemChanged(sender As Object, e As RoutedPropertyChangedEventArgs(Of Object)) Handles TreeViewExplorer.SelectedItemChanged

        Dim item As TreeViewItem = TreeViewExplorer.SelectedItem

        If IO.Directory.Exists(item.Tag) Then
            RepertoireTravail = item.Tag
            CheckDirectory()
            LectureBase()
        End If

    End Sub

    Private Sub SetMenuSousRepertoire()

        Dim MenuPrincipal As New MenuItem() With {.Header = "WorkingDirectory"}
        'MenuSelectionSousMenu.Items.Add(MenuPrincipal)

        CreateMenuForSousRepertoire(MenuPrincipal, GetDirectoryRoot() + "\WorkingDirectory")

    End Sub

    Private Sub CreateMenuForSousRepertoire(ByRef MenuIt As MenuItem, ByVal Rep As String)

        For Each SousDir In IO.Directory.GetDirectories(Rep)
            Dim SsMenu As MenuItem
            SsMenu = CreateMenuSousRepertoire(SousDir)
            MenuIt.Items.Add(SsMenu)
            CreateMenuForSousRepertoire(SsMenu, SousDir)
        Next

    End Sub

    Private Function CreateMenuSousRepertoire(ByVal Repertoire As String) As MenuItem

        Dim RepName As String
        Dim dirIO As IO.DirectoryInfo

        Try
            dirIO = New IO.DirectoryInfo(Repertoire)
            RepName = dirIO.Name
        Catch
            RepName = Repertoire
        End Try

        CreateMenuSousRepertoire = New MenuItem() With {.Header = RepName}

        ' Ajout du retour de Remplacement
        AddHandler CreateMenuSousRepertoire.Click, AddressOf SubMenuWorkDir_Click

    End Function

    Private Sub SubMenuWorkDir_Click(sender As Object, e As RoutedEventArgs)

        'Dim MenuClicked As MenuItem = sender
        'If MenuClicked Is Nothing Then Return

        'Stop

        'BaseBouees.RepertoireTravail = GetDirectoryRoot() + "\WorkingDirectory\" + MenuClicked.Header
        'LectureDataBase()

    End Sub

    Private Sub MenuButtonChangeDirectory_Click(sender As System.Object, e As System.Windows.RoutedEventArgs)

        Dim bt As Button = sender

        If bt.ContextMenu Is Nothing OrElse bt.ContextMenu.Items.Count = 0 Then Return

        bt.ContextMenu.IsEnabled = True
        bt.ContextMenu.PlacementTarget = bt
        bt.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom
        bt.ContextMenu.IsOpen = True

    End Sub

    Private Sub SetRadioButton()

        MainRadioButtonMasseEquipement.IsChecked = True
        MainRadioButtonProfondeur.IsChecked = True
        MainRadioButtonMarnage.IsChecked = True
        MainRadioButtonHouleMax.IsChecked = True
        MainRadioButtonHouleSignificative.IsChecked = True
        MainRadioButtonPeriodeHoule.IsChecked = True
        MainRadioButtonVitesseCourantKts.IsChecked = True
        MainRadioButtonVitesseVentKmh.IsChecked = True

    End Sub

    Private Sub MainButtonChangeWorkingDirectory_Click(sender As Object, e As RoutedEventArgs)

        Dim repDialog As New Forms.FolderBrowserDialog

        repDialog.ShowNewFolderButton = True
        repDialog.SelectedPath = RepertoireTravail

        If repDialog.ShowDialog Then
            RepertoireTravail = repDialog.SelectedPath
            CheckDirectory()
            LectureBase()
        End If

    End Sub

    Private Sub LectureBase()

        If BaseBouees.IsRepertoireTravail Then
            BaseBouees.LectureRepertoireTravail()
            If ApplicationArgs IsNot Nothing AndAlso ApplicationArgs.Count > 0 Then
                For Each arg In ApplicationArgs
                    BaseBouees.AjoutFichierBouee(arg)
                    Me.Title = "Calmar " + Version + " - " + RepertoireTravail
                Next
            End If
        Else
            Me.Dispatcher.BeginInvoke(New DelegateDisplayAlert(AddressOf DisplayAlert), New Object() {Me, GetTextLangue("GeneralErreurRepertoire", "Erreur repertoire de travail inaccessible")})
        End If

        Me.Dispatcher.BeginInvoke(New DelegateRefreshListBouee(AddressOf RefreshListBouee))

    End Sub

    Private Sub MainWindow_SizeChanged(sender As Object, e As System.Windows.SizeChangedEventArgs) Handles Me.SizeChanged

        RefreshDessinBouee(ListBuoyMenu.SelectElement)

    End Sub

    Private Sub MenuRibbonResultat_SelectionChanged(sender As System.Object, e As System.Windows.Controls.SelectionChangedEventArgs)

        ' Si on selectionne l'onglet calcul
        If MenuRibbonResultat.SelectedItem Is MainCalculTab Then
            If ListBuoyMenu IsNot Nothing AndAlso ListBuoyMenu.SelectElement IsNot Nothing Then
                If ListBuoyMenu IsNot Nothing Then ListBuoyMenu.Visibility = Windows.Visibility.Collapsed
                If TreeViewExplorer IsNot Nothing Then TreeViewExplorer.Visibility = Windows.Visibility.Collapsed
                If CadreDessinBouee IsNot Nothing Then CadreDessinBouee.Visibility = Windows.Visibility.Visible
                If TextBlockBuoyName IsNot Nothing Then TextBlockBuoyName.Visibility = Windows.Visibility.Visible

                If CadrePrincipale IsNot Nothing Then CadrePrincipale.SetValue(Grid.ColumnProperty, 0)
                If MenuRibbonResultat IsNot Nothing Then MenuRibbonResultat.SetValue(Grid.ColumnProperty, 0)
                If CadreSaisie IsNot Nothing Then CadreSaisie.Visibility = Windows.Visibility.Visible

                If ListBuoyMenu.SelectElement IsNot Nothing Then
                    RefreshDessinBouee(ListBuoyMenu.SelectElement)
                    RefreshListChaineLest()
                End If
            End If

            ' Si on selectionne l'onglet accueil
        ElseIf MenuRibbonResultat.SelectedItem Is MainAccueilTab Then

            If CadreSaisie IsNot Nothing Then CadreSaisie.Visibility = Windows.Visibility.Collapsed
            If MenuRibbonResultat IsNot Nothing Then MenuRibbonResultat.SetValue(Grid.ColumnProperty, 1)
            If CadrePrincipale IsNot Nothing Then CadrePrincipale.SetValue(Grid.ColumnProperty, 1)
            If ListBuoyMenu IsNot Nothing Then ListBuoyMenu.Visibility = Windows.Visibility.Visible
            If TreeViewExplorer IsNot Nothing Then TreeViewExplorer.Visibility = Windows.Visibility.Visible
            If CadreDessinBouee IsNot Nothing Then CadreDessinBouee.Visibility = Windows.Visibility.Collapsed
            If TextBlockBuoyName IsNot Nothing Then TextBlockBuoyName.Visibility = Windows.Visibility.Collapsed
        ElseIf MenuRibbonResultat.SelectedItem Is MainLangueTab Then

            LangueButtonSwitch()

        End If

    End Sub

    Private Sub TextBox_GotFocus(sender As System.Object, e As System.Windows.RoutedEventArgs)

        sender.SelectAll()

    End Sub

    ' Non affichage de la popup de minimize
    Private Sub MenuRibbon_ContextMenuOpening(sender As System.Object, e As System.Windows.Controls.ContextMenuEventArgs)
        e.Handled = True
    End Sub

    Private Sub MenuRibbonResultat_SizeChanged(sender As System.Object, e As System.Windows.SizeChangedEventArgs)

        Dim RibMenu As Ribbon = sender

        If RibMenu IsNot Nothing Then
            RibMenu.IsMinimized = False
        End If

        e.Handled = True

    End Sub

    Private EtapeMouse As Integer = 0

    Private Sub MenuRibbonResultat_MouseDoubleClick(sender As System.Object, e As System.Windows.Input.MouseButtonEventArgs)

        If MenuRibbonResultat.SelectedItem IsNot MainCalculTab Then
            EtapeMouse = 0
            Return
        End If

        Select Case EtapeMouse

            Case 0
                If e.RightButton = MouseButtonState.Pressed Then EtapeMouse = 1

            Case 1
                If e.LeftButton = MouseButtonState.Pressed Then MontreBoutonCalculSpecial() Else EtapeMouse = 0

        End Select

    End Sub

    Private Sub Window_KeyUp(sender As System.Object, e As System.Windows.Input.KeyEventArgs) Handles MyBase.KeyUp

        'If e.Key = Key.Escape Then
        '    If DisplayDemande(Me, "Voulez-vous quitter l'application") Then _
        '        Close()
        'End If

    End Sub

    Private Sub MainWindow_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown

        If (Keyboard.Modifiers And ModifierKeys.Control) Then
            If e.Key = Key.C Then
                TmpBuoy = ListBuoyMenu.SelectElement.Clone
                MainButtonCollerBouee.IsEnabled = True
            ElseIf e.Key = Key.V Then
                PasteFunction()
            End If
        End If

    End Sub

    Private Sub MainButtonManual_Click(sender As System.Object, e As System.Windows.RoutedEventArgs)

        Try
            Process.Start("explorer.exe " + RepertoireTravailRoot + "\Helps")
        Catch
        End Try

    End Sub

    Private Sub TextBox_KeyUp(sender As Object, e As KeyEventArgs) _
        Handles TextBoxProfondeur.KeyUp, TextBoxMarnage.KeyUp, TextBoxVitesseVent.KeyUp, TextBoxVitesseCourant.KeyUp,
        TextBoxPeriodeHoule.KeyUp, TextBoxHouleSignificative.KeyUp, TextBoxHouleMax.KeyUp

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

#Region "Gestion de la liste des bouées avec les commandes d'ajout et de suppression"

    Private TmpBuoy As CBouee
    Private Delegate Sub DelegateRefreshListBouee()

    Private Sub RefreshListBouee()

        ListBuoyMenu.SetElements(BaseBouees.GetBouees)
        If BaseBouees.GetBouees.Count = 0 AndAlso (IO.Directory.GetFiles(RepertoireTravail).Count = 0 And IO.Directory.GetDirectories(RepertoireTravail).Count = 0) Then
            MainButtonSuppressionDossier.IsEnabled = True
        Else
            MainButtonSuppressionDossier.IsEnabled = False
        End If

    End Sub

    Private Sub SelectionBouee(sender As Object, e As CBouee)

        MainGroupDetails.IsEnabled = False
        MainButtonCopierBouee.IsEnabled = False
        MainButtonSupprimerBouee.IsEnabled = False
        MainButtonModifierBouee.IsEnabled = False
        MainButtonSauvegardeFichier.IsEnabled = False

        If e Is Nothing Then
            ' on desactive l'onglet calcul
            MainCalculTab.IsEnabled = False
        Else
            ' on desactive l'onglet calcul
            MainCalculTab.IsEnabled = True

            ' on active les boutons de controle
            MainButtonSauvegardeFichier.IsEnabled = True
            MainButtonSupprimerBouee.IsEnabled = True
            MainGroupDetails.IsEnabled = True
            MainTextBlockPoidsTotalValue.Text = Math.Round(e.MASSE_BOUEE, 1).ToString() + " Kg"
            MainTextBlockVolumeTotalValue.Text = Math.Round(e.VOLUME, 2).ToString() + " m³"
            If Not e.Protege Then MainButtonCopierBouee.IsEnabled = True
            If Not e.Protege Then MainButtonModifierBouee.IsEnabled = True
        End If

    End Sub

    Private Sub RefreshDessinBouee(ByVal e As CBouee)

        If e Is Nothing Then
            MainButtonSauvegardeFichier.IsEnabled = False

            ' on efface le dessin
            CadreDessinBouee.Child = Nothing
            TextBlockBuoyName.Text = ""
        Else
            MainButtonSauvegardeFichier.IsEnabled = True

            ' on dessine la bouée
            CadreDessinBouee.Child = New CBuoyDrawing(e)
            TextBlockBuoyName.Text = e.Nom
        End If

    End Sub

    Private Sub AjoutBouee_Click(sender As Object, e As System.Windows.RoutedEventArgs)

        Dim NouvelleBouee As FenetreBouee
        Dim TmpBouee As New CBouee() With {.Nom = "Nom de la bouée"}

        While True
            NouvelleBouee = New FenetreBouee()

            NouvelleBouee.Buoy = TmpBouee
            NouvelleBouee.Owner = Me

            If NouvelleBouee.ShowDialog Then

                If BaseBouees.GetBouees.ContainsKey(NouvelleBouee.Buoy.Nom) Then
                    If DisplayDemande(Me, GetTextLangue("InfoBulleAddBuoy", "Voulez-vous remplacer la bouée existante")) Then
                        If Not BaseBouees.ModificationBouee(NouvelleBouee.Buoy.Nom, NouvelleBouee.Buoy) Then
                            DisplayAlert(Me, GetTextLangue("GeneralErreurModifBouee", "Erreur impossible de modifier la bouée"))

                            ' Sauvegarde des elements modifier
                            TmpBouee = NouvelleBouee.Buoy.Clone
                            Continue While
                        Else
                            RefreshListBouee()
                            Exit While
                        End If
                    Else
                        TmpBouee = NouvelleBouee.Buoy.Clone
                        Continue While
                    End If
                Else
                    If Not BaseBouees.AjouterBouee(NouvelleBouee.Buoy) Then
                        DisplayAlert(Me, GetTextLangue("GeneralErreurAjoutBouee", "Erreur impossible d'ajouter la bouée"))
                    Else
                        RefreshListBouee()
                        Exit While
                    End If
                End If

            Else
                Exit While
            End If

        End While

    End Sub

    Private Sub ModificationBouee_Click(sender As Object, e As System.Windows.RoutedEventArgs)

        Dim NomBouee As String
        Dim FenetreModification As FenetreBouee
        Dim TmpBouee As CBouee = ListBuoyMenu.SelectElement.Clone

        While True
            FenetreModification = New FenetreBouee
            FenetreModification.Owner = Me

            FenetreModification.Buoy = TmpBouee.Clone
            NomBouee = TmpBouee.Nom

            If FenetreModification.ShowDialog Then

                ' Enregistrement
                If Not BaseBouees.ModificationBouee(NomBouee, FenetreModification.Buoy) Then
                    DisplayAlert(Me, GetTextLangue("GeneralErreurModifBouee", "Erreur impossible de modifier la bouée"))

                    ' Sauvegarde des elements modifier
                    TmpBouee = FenetreModification.Buoy.Clone

                    Continue While
                Else
                    RefreshListBouee()
                    Exit While
                End If

            Else
                'Verification de modification sur les bouées
                If ListBuoyMenu.SelectElement.IsEqual(FenetreModification.Buoy) Then
                    Exit While
                Else
                    If DisplayDemande(Me, GetTextLangue("InfoBulleModifyBuoy", "Quitter sans enregistrer le modéle ?")) Then
                        Exit While
                    Else
                        ' Sauvegarde des elements modifier
                        TmpBouee = FenetreModification.Buoy.Clone
                    End If
                End If
            End If

        End While

    End Sub

    Private Sub ButtonCopierBouee_Click(sender As System.Object, e As System.Windows.RoutedEventArgs)

        TmpBuoy = ListBuoyMenu.SelectElement.Clone
        MainButtonCollerBouee.IsEnabled = True

    End Sub

    Private Sub MainButtonCollerBouee_Click(sender As Object, e As RoutedEventArgs)

        PasteFunction()

    End Sub

    Private Sub PasteFunction()

        If ListBuoyMenu.SelectElement Is Nothing AndAlso TmpBuoy IsNot Nothing Then

            Dim nameBuoy As String
            Dim Filename As String

            Do
                nameBuoy = DisplayInput(Me, GetTextLangue("InfoBulleNouveauNom", "Voulez-vous changer le nom") + " ?", TmpBuoy.Nom)
                If Not FilenameIsOK(nameBuoy, False) Then
                    nameBuoy = ""
                    DisplayAlert(Me, "Filename error")
                End If
            Loop While nameBuoy = ""


            If nameBuoy <> "" Then
                TmpBuoy.Nom = nameBuoy
            Else
                nameBuoy = TmpBuoy.Nom
            End If

            If TmpBuoy.Protege Then
                Filename = RepertoireTravail + "\" + nameBuoy + ".prtMB" ' Default file name
                BaseBouees.CryptEnregistreBouee(Filename, TmpBuoy)
            Else
                Filename = RepertoireTravail + "\" + nameBuoy + ".xmlMB" ' Default file name
                BaseBouees.EnregistreFichierBouee(Filename, TmpBuoy)
            End If

            LectureBase()
        Else
            Dim selectbuoy As CBouee = ListBuoyMenu.SelectElement

            If Not TmpBuoy.Protege And Not selectbuoy.Protege Then
                Dim FenetreCopy As New FenetreCopie
                FenetreCopy.Owner = Me

                FenetreCopy.SourceBuoy = TmpBuoy
                FenetreCopy.DestinationBuoy = ListBuoyMenu.SelectElement

                If FenetreCopy.ShowDialog Then
                    If BaseBouees.ModificationBouee(FenetreCopy.DestinationBuoy.Nom, FenetreCopy.DestinationBuoy) Then
                        RefreshListBouee()
                    End If
                End If
            End If
        End If

        TmpBuoy = Nothing
        MainButtonCollerBouee.IsEnabled = False

    End Sub

    Private Sub SuppressionBouee_Click(sender As Object, e As System.Windows.RoutedEventArgs)

        If ListBuoyMenu.SelectElement Is Nothing Then Return

        If DisplayDemande(Me, GetTextLangue("InfoBulleDeleteBuoy", "Voulez-vous supprimer la bouée ") + ListBuoyMenu.SelectElement.Nom + " ?") Then

            If Not BaseBouees.SuppressionBouee(ListBuoyMenu.SelectElement.Nom) Then DisplayAlert(Me, GetTextLangue("GeneralErreurSupprBouee", "Erreur impossible de supprimer la bouée"))
            RefreshListBouee()

        End If

    End Sub

    Private Sub ListBuoyMenu_MouseMove(sender As Object, e As MouseEventArgs) Handles ListBuoyMenu.MouseMove

        If ListBuoyMenu.SelectElement IsNot Nothing AndAlso e.LeftButton = MouseButtonState.Pressed Then
            DragDrop.DoDragDrop(ListBuoyMenu, ListBuoyMenu.SelectElement, DragDropEffects.Move)
        Else

        End If

    End Sub

    Private Sub ListBuoyMenu_MouseUp(sender As Object, e As MouseButtonEventArgs) Handles ListBuoyMenu.MouseUp

        If e.LeftButton = MouseButtonState.Released Then
            If Not ListBuoyMenu.IsMouseOnSelectItem(e) Then
                ListBuoyMenu.Unselect()
            End If
        End If

    End Sub

    Private Sub TreeViewExplorer_DragOver(sender As Object, e As DragEventArgs) Handles TreeViewExplorer.DragOver

        If e.Source.GetType Is GetType(TextBlock) Then
            Dim txt As TextBlock = e.Source
            txt.Background = New SolidColorBrush(Colors.AliceBlue)
        End If

    End Sub

    Private Sub TreeViewExplorer_DragLeave(sender As Object, e As DragEventArgs) Handles TreeViewExplorer.DragLeave

        If e.OriginalSource.GetType Is GetType(TextBlock) Then
            Dim txt As TextBlock = e.OriginalSource
            txt.Background = Nothing
        End If

    End Sub

    Private Sub TreeViewExplorer_Drop(sender As Object, e As DragEventArgs) Handles TreeViewExplorer.Drop
        Dim st As StackPanel
        Dim parent As TreeViewItem = Nothing

        If e.Source.GetType Is GetType(TextBlock) Then
            Dim txt As TextBlock = e.Source
            If txt.Parent IsNot Nothing Then
                txt.Background = Nothing
                st = txt.Parent
                If st.Parent IsNot Nothing Then parent = st.Parent
            End If
        End If

        If parent IsNot Nothing And e.Data.GetDataPresent("BuoyLib.Buoy.CBouee") Then
            Dim b As CBouee = e.Data.GetData("BuoyLib.Buoy.CBouee")
            Dim Filename As String

            If IO.Directory.Exists(parent.Tag) And parent.Tag <> RepertoireTravail Then
                If b.Protege Then
                    Filename = parent.Tag + "\" + b.Nom + ".prtMB" ' Default file name
                    If BaseBouees.CryptEnregistreBouee(Filename, b.Nom) Then BaseBouees.SuppressionBouee(b.Nom)
                Else
                    Filename = parent.Tag + "\" + b.Nom + ".xmlMB" ' Default file name
                    If BaseBouees.EnregistreFichierBouee(Filename, b.Nom) Then BaseBouees.SuppressionBouee(b.Nom)
                End If

                LectureBase()
            End If
        End If

    End Sub

#End Region

#Region "Gestion de l'import, export et de la sauvegarde"

    Private Sub OuvrirBase_Click(sender As Object, e As System.Windows.RoutedEventArgs)

        OuvrirBouees()

    End Sub

    Private Sub TextBlockImport_MouseLeftButtonUp(sender As System.Object, e As System.Windows.Input.MouseButtonEventArgs)

        OuvrirBouees()

    End Sub

    Private Sub OuvrirBouees()

        Dim FileOp As New Microsoft.Win32.OpenFileDialog()

        FileOp.DefaultExt = ".xmlMB|.prtMB" ' Default file extension
        FileOp.Filter = "Mooring buoys documents (.xmlMB)|*.xmlMB|Mooring buoys crypt (.prtMB)|*.prtMB" ' Filter files by extension
        FileOp.Multiselect = True

        ' Show open file dialog box
        Dim result? As Boolean = FileOp.ShowDialog()

        ' Process open file dialog box results
        If result = True Then
            ' Open document
            For Each fs In FileOp.FileNames
                BaseBouees.AjoutFichierBouee(fs)
            Next
            RefreshListBouee()
        End If

    End Sub

    Private Sub SaveBase_Click(sender As Object, e As System.Windows.RoutedEventArgs)

        SaveBouee()

    End Sub

    Private Sub TextBlockSauvegarder_MouseLeftButtonUp(sender As System.Object, e As System.Windows.Input.MouseButtonEventArgs)

        SaveBouee()

    End Sub

    Private Sub SaveBouee()

        Dim FileOp As New Microsoft.Win32.SaveFileDialog()

        If ListBuoyMenu.SelectElement Is Nothing Then Return

        If ListBuoyMenu.SelectElement.Protege Then
            FileOp.FileName = ListBuoyMenu.SelectElement.Nom + ".prtMB" ' Default file name
            FileOp.DefaultExt = ".prtMB" ' Default file extension
            FileOp.Filter = "Mooring buoys crypt (.prtMB)|*.prtMB" ' Filter files by extension
        Else
            FileOp.FileName = ListBuoyMenu.SelectElement.Nom + ".xmlMB" ' Default file name
            FileOp.DefaultExt = ".xmlMB" ' Default file extension
            FileOp.Filter = "Mooring buoys documents (.xmlMB)|*.xmlMB|Mooring buoys crypt (.prtMB)|*.prtMB" ' Filter files by extension
        End If

        ' Show open file dialog box
        Dim result? As Boolean = FileOp.ShowDialog()

        ' Process open file dialog box results
        If result = True Then
            'Si on n'a un fichier protege on le crypt
            If ListBuoyMenu.SelectElement.Protege Then
                ' Open document
                BaseBouees.CryptEnregistreBouee(FileOp.FileName, ListBuoyMenu.SelectElement.Nom)
            Else
                ' Sinon on peut le crypter ou pas
                If FileOp.FilterIndex = 1 Then
                    BaseBouees.EnregistreFichierBouee(FileOp.FileName, ListBuoyMenu.SelectElement.Nom)
                ElseIf FileOp.FilterIndex = 2 Then
                    BaseBouees.CryptEnregistreBouee(FileOp.FileName, ListBuoyMenu.SelectElement.Nom)
                End If
            End If
        End If

    End Sub

    Private Sub MainButtonSuppressionDossier_Click(sender As Object, e As RoutedEventArgs)

        Try
            IO.Directory.Delete(RepertoireTravail)
        Catch
            Return
        End Try

        RepertoireTravail = RepertoireTravail.Substring(0, RepertoireTravail.LastIndexOf("\"))
        SelectWorkingDirectory()

        Dim item As TreeViewItem = TreeViewExplorer.SelectedItem
        item.Items.Clear()
        item.Items.Add(Nothing)
        Folder_Refresh_Expand(item)

        LectureBase()

    End Sub

    Private Sub MainButtonNouveauDossier_Click(sender As Object, e As RoutedEventArgs)

        Dim NomDossier As String = DisplayInput(Me, GetTextLangue("Please specify the name of the new folder", "Please specify the name of the new folder"), GetTextLangue("New folder name", "New folder name"))

        If NomDossier.Count > 0 Then
            If FilenameIsOK(RepertoireTravail + "\" + NomDossier, True) Then
                IO.Directory.CreateDirectory(RepertoireTravail + "\" + NomDossier)
                Dim item As TreeViewItem = TreeViewExplorer.SelectedItem
                item.Items.Clear()
                item.Items.Add(Nothing)
                Folder_Refresh_Expand(item)
            End If
        End If

    End Sub

#End Region

#Region "Gestion du tri des bouee"

    Private Sub TriBoueeAsc_Click(sender As System.Object, e As System.Windows.RoutedEventArgs)

        If MainComboBoxListeTri.SelectedIndex < 0 Then Return

        If MainComboBoxListeTri.SelectedIndex = 0 Then
            ListBuoyMenu.TriCroissantDe(ListBuoy.EnumTri.Nom)
        ElseIf MainComboBoxListeTri.SelectedIndex = 1 Then
            ListBuoyMenu.TriCroissantDe(ListBuoy.EnumTri.Volume)
        End If

        RefreshListBouee()

    End Sub

    Private Sub TriBoueeDesc_Click(sender As System.Object, e As System.Windows.RoutedEventArgs)

        If MainComboBoxListeTri.SelectedIndex < 0 Then Return

        If MainComboBoxListeTri.SelectedIndex = 0 Then
            ListBuoyMenu.TriDecroissantDe(ListBuoy.EnumTri.Nom)
        ElseIf MainComboBoxListeTri.SelectedIndex = 1 Then
            ListBuoyMenu.TriDecroissantDe(ListBuoy.EnumTri.Volume)
        End If

        RefreshListBouee()

    End Sub

#End Region

#Region "Gestion de l'affichage de la fenetre"

    Private Sub HideMainWindow()

        'Me.Background = New SolidColorBrush(Colors.Gray)

    End Sub

    Private Sub ShowMainWindow()

        'Me.Background = New SolidColorBrush(Color.FromArgb(&HFF, &HDF, &HE9, &HF5))

    End Sub

#End Region

#Region "Gestion des evenements sur la saisie"

    Private ListEquipementSelect As New Dictionary(Of String, List(Of BuoyLib.Buoy.CEquipementSupplementaire))
    Private SelectChain As CChaine = Nothing

    Private Sub RefreshListChaineLest()

        If ListBuoyMenu.SelectElement Is Nothing Then Return

        Dim final = From key In BaseGeneral.GetChaines
                    Select key Where key.DN >= ListBuoyMenu.SelectElement.ChaineMin And key.DN <= ListBuoyMenu.SelectElement.ChaineMax
                    Order By key.DN Ascending
                    Group By key.DN Into Group

        ComboBoxChaineSelectionDN.Items.Clear()
        For Each ch In final
            ComboBoxChaineSelectionDN.Items.Add(ch.DN)
        Next

        ComboBoxSelectionLest.Items.Clear()
        For Each ls In ListBuoyMenu.SelectElement.LestBouee
            ComboBoxSelectionLest.Items.Add(ls.ToString)
        Next

        MainButtonCalcul.IsEnabled = False

    End Sub

#Region "Gestion du raffraichissement de la chaine"

    Private Sub ComboBoxChaineSelectionDN_SelectionChanged(sender As System.Object, e As System.Windows.Controls.SelectionChangedEventArgs)

        If ComboBoxChaineSelectionDN.SelectedIndex < 0 Then
            ComboBoxChaineSelectionType.SelectedIndex = -1
            ComboBoxChaineSelectionType.IsEnabled = False
            Return
        End If

        Dim final = From key In BaseGeneral.GetChaines
                    Select key Where key.DN = ComboBoxChaineSelectionDN.SelectedItem
                    Order By key.TYPE Ascending

        ComboBoxChaineSelectionType.SelectedIndex = -1
        ComboBoxChaineSelectionType.IsEnabled = True
        ComboBoxChaineSelectionType.Items.Clear()
        For Each ch In final
            ComboBoxChaineSelectionType.Items.Add(ch.TYPE)
        Next

    End Sub

    Private Sub ComboBoxChaineSelectionType_SelectionChanged(sender As System.Object, e As System.Windows.Controls.SelectionChangedEventArgs)

        If ComboBoxChaineSelectionType.SelectedIndex < 0 Then
            SelectChain = Nothing
            If CheckCalcul() Then ActiveBoutonCalculSpecial() Else DesactiveBoutonCalculSpecial()
            Return
        End If

        Dim final = From key In BaseGeneral.GetChaines
                    Select key Where key.DN = ComboBoxChaineSelectionDN.SelectedItem And key.TYPE = ComboBoxChaineSelectionType.SelectedItem

        If final.Count = 1 Then
            SelectChain = final(0)
            If CheckCalcul() Then ActiveBoutonCalculSpecial() Else DesactiveBoutonCalculSpecial()
        End If

    End Sub

    Private Function SortDictionaryDNAsc(ByVal dict As Dictionary(Of Double, List(Of String))) As Dictionary(Of Double, List(Of String))

        Dim final = From key In dict.Keys Order By key Ascending Select key

        SortDictionaryDNAsc = New Dictionary(Of Double, List(Of String))
        For Each item In final
            SortDictionaryDNAsc.Add(item, dict(item))
        Next

    End Function

#End Region

#Region "Gestion de la saisie des lests"

    Private SelectLest As CBouee.CLest = Nothing

    Private Sub ButtonSelectionLest_Click(sender As System.Object, e As System.Windows.RoutedEventArgs)
        ComboBoxSelectionLest.SelectedItem = Nothing
    End Sub

    Private Sub ComboBoxSelectionLest_SelectionChanged(sender As System.Object, e As System.Windows.Controls.SelectionChangedEventArgs)

        If ComboBoxSelectionLest.SelectedItem Is Nothing Then
            SelectLest = Nothing
            Return
        End If

        SelectLest = New CBouee.CLest(ComboBoxSelectionLest.SelectedValue)

    End Sub

#End Region

#Region "Gestion des equipements"

    Private Sub ButtonListEquipement_Click(sender As System.Object, e As System.Windows.RoutedEventArgs)

        Dim FenetreEquipementSite As New FenetreEquipement

        If ListEquipementSelect.Count > 0 Then CopyClassEquipementToFenetreEquipement(ListEquipementSelect, FenetreEquipementSite.ListEquipementSelectionner)

        FenetreEquipementSite.Owner = Me
        If FenetreEquipementSite.ShowDialog() Then
            CopyClassFenetreEquipementToEquipement(FenetreEquipementSite.ListEquipementSelectionner, ListEquipementSelect)
            CalculSommeMasse()
        End If

        FenetreEquipementSite = Nothing

    End Sub

    Private Sub CopyClassEquipementToFenetreEquipement(ByVal CEquipementSource As Dictionary(Of String, List(Of BuoyLib.Buoy.CEquipementSupplementaire)), ByRef CEquipementDestination As Dictionary(Of String, List(Of FenetreEquipement.CEquipementSupplementaireTreeView)))

        CEquipementDestination.Clear()

        For Each ValIt In CEquipementSource
            CEquipementDestination.Add(ValIt.Key, New List(Of FenetreEquipement.CEquipementSupplementaireTreeView))
            For Each It In ValIt.Value
                CEquipementDestination(ValIt.Key).Add(New FenetreEquipement.CEquipementSupplementaireTreeView With {.Name = It.Name,
                                                                                                                    .Categorie = It.Categorie,
                                                                                                                    .Nombre = It.Nombre,
                                                                                                                    .MasseUnitaire = It.MasseUnitaire})
            Next
        Next

    End Sub

    Private Sub CopyClassFenetreEquipementToEquipement(ByVal CEquipementSource As Dictionary(Of String, List(Of FenetreEquipement.CEquipementSupplementaireTreeView)), ByRef CEquipementDestination As Dictionary(Of String, List(Of BuoyLib.Buoy.CEquipementSupplementaire)))

        CEquipementDestination.Clear()

        For Each ValIt In CEquipementSource
            CEquipementDestination.Add(ValIt.Key, New List(Of BuoyLib.Buoy.CEquipementSupplementaire))
            For Each It In ValIt.Value
                CEquipementDestination(ValIt.Key).Add(It)
            Next
        Next

    End Sub

    Private Sub CalculSommeMasse()

        Dim MasseTotale As Double = 0

        For Each it In ListEquipementSelect
            For Each itEquipement In it.Value
                MasseTotale += itEquipement.Masse
            Next
        Next

        TextBoxMasseEquipement.Text = MasseTotale.ToString(EnglishFormat)

    End Sub

#End Region

#Region "Gestion des commandes pour les calculs"

    Private Sub RadioButtonVent_Checked(sender As System.Object, e As System.Windows.RoutedEventArgs)

        Dim d As Double

        If TextBoxVitesseVent.Text = "" Then Return
        Double.TryParse(TextBoxVitesseVent.Text, Globalization.NumberStyles.Any, EnglishFormat, d)

        If MainRadioButtonVitesseVentMS.IsChecked Then
            d = d / 3.6
        Else
            d = d * 3.6
        End If

        TextBoxVitesseVent.Text = Math.Round(d, 1).ToString(EnglishFormat)

    End Sub

    Private Sub RadioButtonCourant_Checked(sender As System.Object, e As System.Windows.RoutedEventArgs)

        Dim d As Double

        If TextBoxVitesseCourant.Text = "" Then Return
        Double.TryParse(TextBoxVitesseCourant.Text, Globalization.NumberStyles.Any, EnglishFormat, d)

        If MainRadioButtonVitesseCourantMS.IsChecked Then
            d = d * 1.852 / 3.6
        Else
            d = d * 3.6 / 1.852
        End If

        TextBoxVitesseCourant.Text = Math.Round(d, 1).ToString(EnglishFormat)

    End Sub

    Private UpdateTextBoxHoule As Boolean = False

    Private Sub TextBoxCheckValue_TextChanged(sender As System.Object, e As System.Windows.Controls.TextChangedEventArgs)

        Dim d As Double

        If UpdateTextBoxHoule = True Then Return
        If MainButtonCalcul Is Nothing Then Return
        UpdateTextBoxHoule = True

        sender.Text = sender.Text.Replace(",", ".")
        sender.Select(sender.Text.Length, 0)

        If sender Is TextBoxHouleSignificative Then
            Double.TryParse(TextBoxHouleSignificative.Text, Globalization.NumberStyles.Number, EnglishFormat, d)
            TextBoxHouleMax.Text = Math.Round(d * 1.85, 2).ToString(EnglishFormat)
        ElseIf sender Is TextBoxHouleMax Then
            Double.TryParse(TextBoxHouleMax.Text, Globalization.NumberStyles.Number, EnglishFormat, d)
            TextBoxHouleSignificative.Text = Math.Round(d / 1.85, 2).ToString(EnglishFormat)
        End If

        If CheckCalcul() Then ActiveBoutonCalculSpecial() Else DesactiveBoutonCalculSpecial()

        UpdateTextBoxHoule = False

    End Sub

    Private Sub TextBox_GotKeyboardFocus(sender As System.Object, e As System.Windows.Input.KeyboardFocusChangedEventArgs)

        sender.SelectAll()

    End Sub

    Private Sub TextBox_GotMouseCapture(sender As System.Object, e As System.Windows.Input.MouseEventArgs)

        sender.SelectAll()

    End Sub

    Private Function CheckCalcul() As Boolean

        Dim p As Double

        If ListBuoyMenu.SelectElement Is Nothing Then Return False
        If SelectChain Is Nothing Then Return False

        Try

            p = Double.Parse(TextBoxMasseEquipement.Text, EnglishFormat)
            p = Double.Parse(TextBoxProfondeur.Text, EnglishFormat)
            If p = 0 Then Return False
            p = Double.Parse(TextBoxMarnage.Text, EnglishFormat)
            p = Double.Parse(TextBoxHouleMax.Text, EnglishFormat)
            p = Double.Parse(TextBoxPeriodeHoule.Text, EnglishFormat)
            p = Double.Parse(TextBoxVitesseVent.Text, EnglishFormat)
            p = Double.Parse(TextBoxVitesseCourant.Text, EnglishFormat)
            p = Double.Parse(MainComboBoxDensiteCM.SelectedItem.Split(" ")(0), EnglishFormat)

        Catch
            Return False
        End Try

        Return True

    End Function

    Private Function IsDeferlement() As Boolean

        Dim per As Double, hau As Double, pro As Double, Lo As Double
        Dim HauteurDeferlement As Double

        IsDeferlement = False

        Try

            pro = Double.Parse(TextBoxProfondeur.Text, EnglishFormat)
            per = Double.Parse(TextBoxPeriodeHoule.Text, EnglishFormat)
            hau = Double.Parse(TextBoxHouleMax.Text, EnglishFormat)
            Lo = (1.56 * per ^ 2)

            If hau / Lo > 0.14 Then
                HauteurDeferlement = Lo * 0.14
            Else
                HauteurDeferlement = pro * 0.78
            End If
            If HauteurDeferlement < hau Then IsDeferlement = True

        Catch
            Return False
        End Try

        Return IsDeferlement

    End Function

    Private Function IsProfondeurCorrect() As Boolean

        Dim hca As Double, pro As Double

        IsProfondeurCorrect = False

        Try

            pro = Double.Parse(TextBoxProfondeur.Text, EnglishFormat)
            hca = ListBuoyMenu.SelectElement.StructureBouee.OffsetOrganeau

            If hca < pro Then
                IsProfondeurCorrect = True
            End If

        Catch
            IsProfondeurCorrect = False
        End Try

        Return IsProfondeurCorrect

    End Function

    Private Sub ButtonCalcul_Click(sender As System.Object, e As System.Windows.RoutedEventArgs)

        Dim longueurChaine As Double = 0.0

        If ListBuoyMenu.SelectElement Is Nothing Then Return
        If SelectChain Is Nothing Then Return

        If My.Computer.Keyboard.CtrlKeyDown Then
            Try
                'longueurChaine = Double.Parse(InputBox("Longueur chaine definie"), EnglishFormat)
            Catch
            End Try
        End If

        If IsDeferlement() Then
            DisplayAlert(Me, GetTextLangue("InfoBulleErreurDeferlement", "Erreur : Deferlement detecté"))
            Return
        End If
        If Not IsProfondeurCorrect() Then
            DisplayAlert(Me, GetTextLangue("InfoBulleErreurHauteurOrg", "Erreur : la hauteur de la structure est superieur à la profondeur"))
            Return
        End If

        Dim newFenetreResult As New FenetreResultatRibbon(ListBuoyMenu.SelectElement,
                                                    SelectChain,
                                                    ComboBoxChaineQualite.SelectedIndex + 1,
                                                    SelectLest,
                                                    ListEquipementSelect,
                                                    Double.Parse(TextBoxMasseEquipement.Text, EnglishFormat),
                                                    Double.Parse(TextBoxProfondeur.Text, EnglishFormat),
                                                    Double.Parse(TextBoxMarnage.Text, EnglishFormat),
                                                    Double.Parse(TextBoxHouleMax.Text, EnglishFormat),
                                                    Double.Parse(TextBoxPeriodeHoule.Text, EnglishFormat),
                                                    Double.Parse(TextBoxVitesseVent.Text, EnglishFormat),
                                                    MainRadioButtonVitesseVentKmh.IsChecked,
                                                    Double.Parse(TextBoxVitesseCourant.Text, EnglishFormat),
                                                    MainRadioButtonVitesseCourantKts.IsChecked,
                                                    MainComboBoxDensiteCM.SelectedItem)
        newFenetreResult.Owner = Me
        newFenetreResult.ShowDialog()


    End Sub

    Private Sub MainButtonCalculWeb_Click(sender As Object, e As RoutedEventArgs)

        Dim get_data As String = "model_buoy=" + System.Net.WebUtility.HtmlEncode(ListBuoyMenu.SelectElement.FileP) + "&model_buoy_name=" + System.Net.WebUtility.HtmlEncode(ListBuoyMenu.SelectElement.Nom.Replace(".xmlMB", "").Replace(".prtMB", "")) + ".prtMB&chain_dn_min=" + ListBuoyMenu.SelectElement.ChaineMin.ToString(EnglishFormat) + "&chain_dn_max=" + ListBuoyMenu.SelectElement.ChaineMax.ToString(EnglishFormat)

        My.Computer.Clipboard.SetText("http://109.6.140.213:8080/calcul.html?" + get_data)
        MessageBox.Show("http://109.6.140.213:8080/calcul.html?" + get_data)

        'Dim longueurChaine As Double = 0.0

        'If ListBuoyMenu.SelectElement Is Nothing Then Return
        'If SelectChain Is Nothing Then Return

        'If My.Computer.Keyboard.CtrlKeyDown Then
        '    Try
        '        'longueurChaine = Double.Parse(InputBox("Longueur chaine definie"), EnglishFormat)
        '    Catch
        '    End Try
        'End If

        'If IsDeferlement() Then
        '    DisplayAlert(Me, GetTextLangue("InfoBulleErreurDeferlement", "Erreur : Deferlement detecté"))
        '    Return
        'End If
        'If Not IsProfondeurCorrect() Then
        '    DisplayAlert(Me, GetTextLangue("InfoBulleErreurHauteurOrg", "Erreur : la hauteur de la structure est superieur à la profondeur"))
        '    Return
        'End If


        'Dim json As String = "{"
        'json += """bouee_data"": """ + ListBuoyMenu.SelectElement.File.Replace(vbCr, "").Replace(vbLf, "").Replace("""", "\""") + ""","
        'json += """chaine"": {"
        'json += """dn"": """ + SelectChain.DN.ToString(EnglishFormat) + ""","
        'json += """type"": """ + SelectChain.TYPE + ""","
        'json += """masse_lineique"": """ + SelectChain.MASSE_LINEIQUE.ToString(EnglishFormat) + ""","
        'json += """charge_epreuve_ql"": """ + SelectChain.CHARGE_EPREUVE(ComboBoxChaineQualite.SelectedIndex + 1).ToString(EnglishFormat) + """"
        'json += "},"
        'json += """masse_lest"": ""0"","
        'json += """masse_equipements"": ""0"","
        'json += """profondeur"": """ + TextBoxProfondeur.Text + ""","
        'json += """marnage"": """ + TextBoxMarnage.Text + ""","
        'json += """houle_significant"": """ + TextBoxHouleSignificative.Text + ""","
        'json += """periode_houle"": """ + TextBoxPeriodeHoule.Text + ""","
        'json += """vitesse_vent"": """ + TextBoxVitesseVent.Text + ""","
        'json += """vitesse_courant"": """ + TextBoxVitesseCourant.Text + ""","
        'json += """densite_cm"": """ + MainComboBoxDensiteCM.SelectedItem.Split(" ")(0) + """"
        'json += "}"

        'MessageBox.Show(PostData("http://localhost:51411/api/calmar/mooringcalculation", json))

    End Sub

    Private Function PostData(ByVal url As String, ByVal json As String)

        Dim responsebody As String = ""

        Dim httpWebRequest As HttpWebRequest = WebRequest.Create(url)

        httpWebRequest.ContentType = "application/json"
        httpWebRequest.Timeout = 30000 ' 30 sec
        httpWebRequest.ContentLength = json.Length
        httpWebRequest.Method = "POST"

        Dim stwriter As New IO.StreamWriter(httpWebRequest.GetRequestStream)

        stwriter.Write(json)
        stwriter.Close()
        Try
            Dim httpResponse As HttpWebResponse = httpWebRequest.GetResponse()
            Dim streamReader As New IO.StreamReader(httpResponse.GetResponseStream())

            responsebody = streamReader.ReadToEnd
            streamReader.Close()
        Catch ex As Exception
            responsebody = "Timeout " + ex.ToString
        End Try

        Return responsebody

    End Function

    Private Sub ButtonNoLoad_Click(sender As System.Object, e As System.Windows.RoutedEventArgs)

        If ListBuoyMenu.SelectElement Is Nothing Then Return
        If SelectChain Is Nothing Then Return

        Dim newFenetreResult As New FenetreResultatRibbon(ListBuoyMenu.SelectElement,
                                                    SelectChain,
                                                    ComboBoxChaineQualite.SelectedIndex + 1,
                                                    SelectLest,
                                                    ListEquipementSelect,
                                                    Double.Parse(TextBoxMasseEquipement.Text, EnglishFormat),
                                                    Double.Parse(TextBoxProfondeur.Text, EnglishFormat),
                                                    Double.Parse(TextBoxMarnage.Text, EnglishFormat),
                                                    0,
                                                    Double.Parse(TextBoxPeriodeHoule.Text, EnglishFormat),
                                                    0,
                                                    MainRadioButtonVitesseVentKmh.IsChecked,
                                                    0,
                                                    MainRadioButtonVitesseCourantKts.IsChecked,
                                                    MainComboBoxDensiteCM.SelectedItem)
        newFenetreResult.Owner = Me
        newFenetreResult.ShowDialog()

    End Sub

    Private Sub ButtonCurrent_Click(sender As System.Object, e As System.Windows.RoutedEventArgs)

        If ListBuoyMenu.SelectElement Is Nothing Then Return
        If SelectChain Is Nothing Then Return

        Dim newFenetreResult As New FenetreResultatRibbon(ListBuoyMenu.SelectElement,
                                                    SelectChain,
                                                    ComboBoxChaineQualite.SelectedIndex + 1,
                                                    SelectLest,
                                                    ListEquipementSelect,
                                                    Double.Parse(TextBoxMasseEquipement.Text, EnglishFormat),
                                                    Double.Parse(TextBoxProfondeur.Text, EnglishFormat),
                                                    Double.Parse(TextBoxMarnage.Text, EnglishFormat),
                                                    0,
                                                    Double.Parse(TextBoxPeriodeHoule.Text, EnglishFormat),
                                                    0,
                                                    MainRadioButtonVitesseVentKmh.IsChecked,
                                                    Double.Parse(TextBoxVitesseCourant.Text, EnglishFormat),
                                                    MainRadioButtonVitesseCourantKts.IsChecked,
                                                    MainComboBoxDensiteCM.SelectedItem)
        newFenetreResult.Owner = Me
        newFenetreResult.ShowDialog()

    End Sub

    Private Sub ButtonVent_Click(sender As System.Object, e As System.Windows.RoutedEventArgs)

        If ListBuoyMenu.SelectElement Is Nothing Then Return
        If SelectChain Is Nothing Then Return

        Dim newFenetreResult As New FenetreResultatRibbon(ListBuoyMenu.SelectElement,
                                                    SelectChain,
                                                    ComboBoxChaineQualite.SelectedIndex + 1,
                                                    SelectLest,
                                                    ListEquipementSelect,
                                                    Double.Parse(TextBoxMasseEquipement.Text, EnglishFormat),
                                                    Double.Parse(TextBoxProfondeur.Text, EnglishFormat),
                                                    Double.Parse(TextBoxMarnage.Text, EnglishFormat),
                                                    0,
                                                    Double.Parse(TextBoxPeriodeHoule.Text, EnglishFormat),
                                                    Double.Parse(TextBoxVitesseVent.Text, EnglishFormat),
                                                    MainRadioButtonVitesseVentKmh.IsChecked,
                                                    0,
                                                    MainRadioButtonVitesseCourantKts.IsChecked,
                                                    MainComboBoxDensiteCM.SelectedItem)
        newFenetreResult.Owner = Me
        newFenetreResult.ShowDialog()

    End Sub

    Private Sub ButtonHoule_Click(sender As System.Object, e As System.Windows.RoutedEventArgs)

        If ListBuoyMenu.SelectElement Is Nothing Then Return
        If SelectChain Is Nothing Then Return

        Dim newFenetreResult As New FenetreResultatRibbon(ListBuoyMenu.SelectElement,
                                                    SelectChain,
                                                    ComboBoxChaineQualite.SelectedIndex + 1,
                                                    SelectLest,
                                                    ListEquipementSelect,
                                                    Double.Parse(TextBoxMasseEquipement.Text, EnglishFormat),
                                                    Double.Parse(TextBoxProfondeur.Text, EnglishFormat),
                                                    Double.Parse(TextBoxMarnage.Text, EnglishFormat),
                                                    Double.Parse(TextBoxHouleMax.Text, EnglishFormat),
                                                    Double.Parse(TextBoxPeriodeHoule.Text, EnglishFormat),
                                                    0,
                                                    MainRadioButtonVitesseVentKmh.IsChecked,
                                                    0,
                                                    MainRadioButtonVitesseCourantKts.IsChecked,
                                                    MainComboBoxDensiteCM.SelectedItem)
        newFenetreResult.Owner = Me
        newFenetreResult.ShowDialog()

    End Sub

    Private Sub MainButtonCalculIALA1066_Click(sender As System.Object, e As System.Windows.RoutedEventArgs)

        If ListBuoyMenu.SelectElement Is Nothing Then Return
        If SelectChain Is Nothing Then Return

        If IsDeferlement() Then
            DisplayAlert(Me, GetTextLangue("InfoBulleErreurDeferlement", "Erreur : Deferlement detecté"))
            Return
        End If
        If Not IsProfondeurCorrect() Then
            DisplayAlert(Me, GetTextLangue("InfoBulleErreurHauteurOrg", "Erreur : la hauteur de la structure est superieur à la profondeur"))
            Return
        End If

        Dim newFenetreResult As New FenetreResultatRibbon(ListBuoyMenu.SelectElement,
                                                    SelectChain,
                                                    ComboBoxChaineQualite.SelectedIndex + 1,
                                                    SelectLest,
                                                    ListEquipementSelect,
                                                    Double.Parse(TextBoxMasseEquipement.Text, EnglishFormat),
                                                    Double.Parse(TextBoxProfondeur.Text, EnglishFormat),
                                                    Double.Parse(TextBoxMarnage.Text, EnglishFormat),
                                                    Double.Parse(TextBoxHouleMax.Text, EnglishFormat),
                                                    Double.Parse(TextBoxPeriodeHoule.Text, EnglishFormat),
                                                    Double.Parse(TextBoxVitesseVent.Text, EnglishFormat),
                                                    MainRadioButtonVitesseVentKmh.IsChecked,
                                                    Double.Parse(TextBoxVitesseCourant.Text, EnglishFormat),
                                                    MainRadioButtonVitesseCourantKts.IsChecked,
                                                    MainComboBoxDensiteCM.SelectedItem, True)
        newFenetreResult.Owner = Me
        newFenetreResult.ShowDialog()

    End Sub

#End Region

#Region "Gestion du site"

    Private Sub MainButtonSiteParametre_MouseDoubleClick(sender As System.Object, e As System.Windows.Input.MouseButtonEventArgs) Handles MainButtonSiteParametre.MouseDoubleClick

        Dim WinParametreSite As New FenetreParametreSite

        WinParametreSite.Trainee = CCalculMouillage.COEF_TRAINEE
        WinParametreSite.DensiteEau = CCalculMouillage.DENSITE_EAU

        WinParametreSite.Owner = Me
        If WinParametreSite.ShowDialog Then
            CCalculMouillage.COEF_TRAINEE = WinParametreSite.Trainee
            CCalculMouillage.DENSITE_EAU = WinParametreSite.DensiteEau
        End If

    End Sub

#End Region

#Region "Gestion de la sauvegarde des parametres de calcul"

    Private Sub MainButtonSaveInput_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)

        Dim svDial As New Microsoft.Win32.SaveFileDialog

        Dim xs As New System.Xml.Serialization.XmlSerializer(GetType(CInputParameter))

        Dim param As New CInputParameter
        Dim CategorieWithEquipement() As CInputParameter.CLSEquipements

        svDial.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)

        svDial.DefaultExt = ".xml" ' Default file extension
        svDial.Filter = "Fichier XML (.xml)|*.xml" ' Filter files by extension

        If svDial.ShowDialog Then

            Try
                param.ChaineDN = SelectChain.DN
                param.ChaineType = SelectChain.TYPE
                param.ChaineQualite = ComboBoxChaineQualite.SelectedIndex + 1
                If SelectLest IsNot Nothing Then
                    param.NombreLest = SelectLest._NombreLest
                    param.PoidsLest = SelectLest._PoidsLest
                End If

                ReDim CategorieWithEquipement(ListEquipementSelect.Count - 1)
                For p0 = 0 To ListEquipementSelect.Count - 1
                    CategorieWithEquipement(p0) = New CInputParameter.CLSEquipements
                    CategorieWithEquipement(p0).Categorie = ListEquipementSelect.Keys(p0)
                    Dim Equipements(ListEquipementSelect(CategorieWithEquipement(p0).Categorie).Count - 1) As CInputParameter.CLSEquipements.CEquipement
                    For p1 = 0 To ListEquipementSelect(CategorieWithEquipement(p0).Categorie).Count - 1
                        Equipements(p1) = New CInputParameter.CLSEquipements.CEquipement
                        Equipements(p1).Name = ListEquipementSelect(CategorieWithEquipement(p0).Categorie)(p1).Name
                        Equipements(p1).Categorie = ListEquipementSelect(CategorieWithEquipement(p0).Categorie)(p1).Categorie
                        Equipements(p1).Nombre = ListEquipementSelect(CategorieWithEquipement(p0).Categorie)(p1).Nombre
                        Equipements(p1).MasseUnitaire = ListEquipementSelect(CategorieWithEquipement(p0).Categorie)(p1).MasseUnitaire
                    Next
                    CategorieWithEquipement(p0).Equipement = Equipements
                Next
                param.Equipements = CategorieWithEquipement

                param.Profondeur = Double.Parse(TextBoxProfondeur.Text, EnglishFormat)
                param.Marnage = Double.Parse(TextBoxMarnage.Text, EnglishFormat)
                param.HouleMax = Double.Parse(TextBoxHouleMax.Text, EnglishFormat)
                param.PeriodeHoule = Double.Parse(TextBoxPeriodeHoule.Text, EnglishFormat)
                param.VitesseVentMS = MainRadioButtonVitesseVentMS.IsChecked
                param.VitesseVent = Double.Parse(TextBoxVitesseVent.Text, EnglishFormat)
                param.VitesseCourantMS = MainRadioButtonVitesseCourantMS.IsChecked
                param.VitesseCourant = Double.Parse(TextBoxVitesseCourant.Text, EnglishFormat)
                param.StringDensiteCM = MainComboBoxDensiteCM.SelectedItem.ToString

                Dim st As IO.Stream
                st = svDial.OpenFile
                xs.Serialize(st, param)
                st.Close()
            Catch
                MessageBox.Show("Error writing")
                Return
            End Try

        End If

    End Sub

    Private Sub MainButtonOpenInput_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)

        Dim opDial As New Microsoft.Win32.OpenFileDialog

        Dim xs As New System.Xml.Serialization.XmlSerializer(GetType(CInputParameter))
        Dim param As New CInputParameter

        opDial.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)

        opDial.DefaultExt = ".xml" ' Default file extension
        opDial.Filter = "Fichier XML (.xml)|*.xml" ' Filter files by extension
        opDial.Multiselect = False

        If opDial.ShowDialog Then

            Try
                Dim st As IO.Stream
                st = opDial.OpenFile
                param = xs.Deserialize(st)
                st.Close()

                ComboBoxChaineQualite.SelectedIndex = param.ChaineQualite - 1
                If param.NombreLest > 0 Then
                    For Each it In ComboBoxSelectionLest.Items
                        If it = New CBouee.CLest(param.NombreLest, param.PoidsLest).ToString Then
                            ComboBoxSelectionLest.SelectedItem = it
                            Exit For
                        End If
                    Next
                End If

                ListEquipementSelect.Clear()
                For Each equ In param.Equipements
                    ListEquipementSelect.Add(equ.Categorie, New List(Of BuoyLib.Buoy.CEquipementSupplementaire))
                    For Each eq In equ.Equipement
                        ListEquipementSelect(equ.Categorie).Add(New BuoyLib.Buoy.CEquipementSupplementaire With {.Name = eq.Name, _
                                                                                                                 .Categorie = eq.Categorie, _
                                                                                                                 .Nombre = eq.Nombre, _
                                                                                                                 .MasseUnitaire = eq.MasseUnitaire})
                    Next
                Next
                CalculSommeMasse()
                TextBoxProfondeur.Text = param.Profondeur.ToString(EnglishFormat)
                TextBoxMarnage.Text = param.Marnage.ToString(EnglishFormat)
                TextBoxHouleMax.Text = param.HouleMax.ToString(EnglishFormat)
                TextBoxPeriodeHoule.Text = param.PeriodeHoule.ToString(EnglishFormat)
                If param.VitesseVentMS Then
                    MainRadioButtonVitesseVentKmh.IsChecked = False
                    MainRadioButtonVitesseVentMS.IsChecked = True
                Else
                    MainRadioButtonVitesseVentKmh.IsChecked = True
                    MainRadioButtonVitesseVentMS.IsChecked = False
                End If
                TextBoxVitesseVent.Text = param.VitesseVent.ToString(EnglishFormat)
                If param.VitesseCourantMS Then
                    MainRadioButtonVitesseCourantKts.IsChecked = False
                    MainRadioButtonVitesseCourantMS.IsChecked = True
                Else
                    MainRadioButtonVitesseCourantKts.IsChecked = True
                    MainRadioButtonVitesseCourantMS.IsChecked = False
                End If
                TextBoxVitesseCourant.Text = param.VitesseCourant.ToString(EnglishFormat)
                For Each it In MainComboBoxDensiteCM.Items
                    If param.StringDensiteCM = it.ToString Then
                        MainComboBoxDensiteCM.SelectedItem = it
                        Exit For
                    End If
                Next

                For Each it In ComboBoxChaineSelectionDN.Items
                    If param.ChaineDN = it.ToString() Then
                        ComboBoxChaineSelectionDN.SelectedItem = it
                        Exit For
                    End If
                Next

                System.Threading.Thread.Sleep(500)
                For Each it In ComboBoxChaineSelectionType.Items
                    If param.ChaineType = it.ToString() Then
                        ComboBoxChaineSelectionType.SelectedItem = it
                        Exit For
                    End If
                Next

            Catch
                MessageBox.Show("Error opening")
                Return
            End Try

            If CheckCalcul() Then ActiveBoutonCalculSpecial() Else DesactiveBoutonCalculSpecial()

        End If

    End Sub

#End Region

#End Region

#Region "Gestion des calculs hydro"

    Private Structure HydroStat

        Public LCFDraft As Double
        Public Displ0 As Double
        Public Surf0 As Double

        Public Displ1 As Double
        Public Surf1 As Double

    End Structure

    Private Sub ButtonCalculHydro_Click(sender As System.Object, e As System.Windows.RoutedEventArgs)

        Dim TmpStruct As HydroStat
        Dim split() As String

        Dim sr As IO.StreamReader
        Dim sw As IO.StreamWriter

        Dim ListResult As New List(Of HydroStat)

        Dim TmpBuoy As CBouee = ListBuoyMenu.SelectElement.Clone
        Dim OpenFileD As New Microsoft.Win32.OpenFileDialog

        If OpenFileD.ShowDialog() Then

            sr = New IO.StreamReader(OpenFileD.FileName)

            While Not sr.EndOfStream
                split = sr.ReadLine.Split(";")
                ListResult.Add(New HydroStat() With {.LCFDraft = Double.Parse(split(0)), .Displ0 = Double.Parse(split(1)), .Surf0 = Double.Parse(split(2))})
            End While
            sr.Close()

            sw = New IO.StreamWriter(OpenFileD.FileName + "-OUT.csv")
            sw.WriteLine("LCF Draft;Displ %;Surf %;;LCF Draft;Displ DH;Displ JFB;Surf DH;Surf JFB")
            sw.WriteLine("")

            For p0 = 0 To ListResult.Count - 1

                TmpStruct = ListResult(p0)

                TmpBuoy.SET_HAUTEUR_IMMERGE(TmpStruct.LCFDraft)

                TmpStruct.Displ1 = TmpBuoy.VOLUME_IMMERGE
                TmpStruct.Surf1 = TmpBuoy.SURFACE_IMMERGEE

                ListResult(p0) = TmpStruct
                sw.WriteLine(TmpStruct.LCFDraft.ToString + ";" _
                             + Math.Round(CalculPourcent(TmpStruct.Displ0, TmpStruct.Displ1), 2).ToString + ";" _
                             + Math.Round(CalculPourcent(TmpStruct.Surf0, TmpStruct.Surf1), 2).ToString + ";;" _
                             + TmpStruct.LCFDraft.ToString + ";" _
                             + TmpStruct.Displ0.ToString + ";" + TmpStruct.Displ1.ToString + ";" _
                             + TmpStruct.Surf0.ToString + ";" + TmpStruct.Surf1.ToString)
            Next
            sw.Close()

        End If

    End Sub

    Private Function CalculPourcent(ByVal ValeurTheorique As Double, ByVal ValeurObtenue As Double) As Double

        If ValeurTheorique = 0 And ValeurObtenue = 0 Then Return 0
        If ValeurTheorique = 0 And ValeurObtenue <> 0 Then Return 1

        Return (ValeurTheorique - ValeurObtenue) / ValeurTheorique

    End Function

#End Region

#Region "Gestion des elements de debug"

    Private Sub CacheBoutonCalculSpecial()

        MainButtonNoLoad.Visibility = Windows.Visibility.Collapsed
        MainButtonCurrent.Visibility = Windows.Visibility.Collapsed
        MainButtonVent.Visibility = Windows.Visibility.Collapsed
        MainButtonHoule.Visibility = Windows.Visibility.Collapsed
        MainButtonCalculIALA1066.Visibility = Windows.Visibility.Collapsed
        MainButtonCalculWeb.Visibility = Visibility.Collapsed

    End Sub

    Private Sub MontreBoutonCalculSpecial()

        MainButtonNoLoad.Visibility = Windows.Visibility.Visible
        MainButtonCurrent.Visibility = Windows.Visibility.Visible
        MainButtonVent.Visibility = Windows.Visibility.Visible
        MainButtonHoule.Visibility = Windows.Visibility.Visible
        MainButtonCalculIALA1066.Visibility = Windows.Visibility.Visible
        MainButtonCalculWeb.Visibility = Visibility.Visible

    End Sub

    Private Sub ActiveBoutonCalculSpecial()

        MainButtonSaveInput.IsEnabled = True
        MainButtonCalcul.IsEnabled = True
        MainButtonCalculWeb.IsEnabled = True
        MainButtonNoLoad.IsEnabled = True
        MainButtonCurrent.IsEnabled = True
        MainButtonVent.IsEnabled = True
        MainButtonHoule.IsEnabled = True
        MainButtonCalculIALA1066.IsEnabled = True

    End Sub

    Private Sub DesactiveBoutonCalculSpecial()

        MainButtonSaveInput.IsEnabled = False
        MainButtonCalcul.IsEnabled = False
        MainButtonCalculWeb.IsEnabled = False
        MainButtonNoLoad.IsEnabled = False
        MainButtonCurrent.IsEnabled = False
        MainButtonVent.IsEnabled = False
        MainButtonHoule.IsEnabled = False
        MainButtonCalculIALA1066.IsEnabled = False

    End Sub

    Private Sub FonctionReleaseEnable()

        CacheBoutonCalculSpecial()
        MainButtonCalculHydro.Visibility = Windows.Visibility.Collapsed
        ButtonLangueExportXAML.Visibility = Windows.Visibility.Collapsed
        ButtonLangueExportCSV.Visibility = Windows.Visibility.Collapsed
        ButtonLangueChangeCSV.Visibility = Windows.Visibility.Collapsed

    End Sub

    Private Sub FonctionDebugEnable()

    End Sub

#End Region

#Region "Gestion de la langue"

    Private Sub LangueButtonSwitch()

        Select Case LangueAppli

            Case "fr"
                MainButtonLangueFr.IsChecked = True
                MainButtonLangueEn.IsChecked = False
                MainButtonLangueEs.IsChecked = False
                MainButtonLangueJp.IsChecked = False
                MainButtonLangueDe.IsChecked = False
                MainButtonLangueAr.IsChecked = False
                MainButtonLangueKr.IsChecked = False

            Case "en"
                MainButtonLangueFr.IsChecked = False
                MainButtonLangueEn.IsChecked = True
                MainButtonLangueEs.IsChecked = False
                MainButtonLangueJp.IsChecked = False
                MainButtonLangueDe.IsChecked = False
                MainButtonLangueAr.IsChecked = False
                MainButtonLangueKr.IsChecked = False

            Case "es"
                MainButtonLangueEs.IsChecked = True
                MainButtonLangueFr.IsChecked = False
                MainButtonLangueEn.IsChecked = False
                MainButtonLangueJp.IsChecked = False
                MainButtonLangueDe.IsChecked = False
                MainButtonLangueAr.IsChecked = False
                MainButtonLangueKr.IsChecked = False

            Case "jp"
                MainButtonLangueEs.IsChecked = False
                MainButtonLangueFr.IsChecked = False
                MainButtonLangueEn.IsChecked = False
                MainButtonLangueJp.IsChecked = True
                MainButtonLangueDe.IsChecked = False
                MainButtonLangueAr.IsChecked = False
                MainButtonLangueKr.IsChecked = False

            Case "de"
                MainButtonLangueEs.IsChecked = False
                MainButtonLangueFr.IsChecked = False
                MainButtonLangueEn.IsChecked = False
                MainButtonLangueJp.IsChecked = False
                MainButtonLangueDe.IsChecked = True
                MainButtonLangueAr.IsChecked = False
                MainButtonLangueKr.IsChecked = False

            Case "ar"
                MainButtonLangueEs.IsChecked = False
                MainButtonLangueFr.IsChecked = False
                MainButtonLangueEn.IsChecked = False
                MainButtonLangueJp.IsChecked = False
                MainButtonLangueDe.IsChecked = False
                MainButtonLangueAr.IsChecked = True
                MainButtonLangueKr.IsChecked = False

            Case "kr"
                MainButtonLangueEs.IsChecked = False
                MainButtonLangueFr.IsChecked = False
                MainButtonLangueEn.IsChecked = False
                MainButtonLangueJp.IsChecked = False
                MainButtonLangueDe.IsChecked = False
                MainButtonLangueAr.IsChecked = False
                MainButtonLangueKr.IsChecked = True

        End Select

    End Sub

    Private Sub ButtonLangueEn_Click(sender As System.Object, e As System.Windows.RoutedEventArgs)

        Dim UpdateProjectName As Boolean = False

        If ProjectName = GetTextLangue("TitreProjet", "Nom du projet") Then UpdateProjectName = True

        LangueAppli = "en"
        ChangeLangueInLayout()
        LangueButtonSwitch()

        If UpdateProjectName Then ProjectName = GetTextLangue("TitreProjet", "Nom du projet")

    End Sub

    Private Sub ButtonLangueFr_Click(sender As System.Object, e As System.Windows.RoutedEventArgs)

        Dim UpdateProjectName As Boolean = False

        If ProjectName = GetTextLangue("TitreProjet", "Nom du projet") Then UpdateProjectName = True

        LangueAppli = "fr"
        ChangeLangueInLayout()
        LangueButtonSwitch()

        If UpdateProjectName Then ProjectName = GetTextLangue("TitreProjet", "Nom du projet")

    End Sub

    Private Sub ButtonLangueEs_Click(sender As System.Object, e As System.Windows.RoutedEventArgs)

        Dim UpdateProjectName As Boolean = False

        If ProjectName = GetTextLangue("TitreProjet", "Nom du projet") Then UpdateProjectName = True

        LangueAppli = "es"
        ChangeLangueInLayout()
        LangueButtonSwitch()

        If UpdateProjectName Then ProjectName = GetTextLangue("TitreProjet", "Nom du projet")

    End Sub

    Private Sub MainButtonLangueDe_Click(sender As System.Object, e As System.Windows.RoutedEventArgs)

        Dim UpdateProjectName As Boolean = False

        If ProjectName = GetTextLangue("TitreProjet", "Nom du projet") Then UpdateProjectName = True

        LangueAppli = "de"
        ChangeLangueInLayout()
        LangueButtonSwitch()

        If UpdateProjectName Then ProjectName = GetTextLangue("TitreProjet", "Nom du projet")

    End Sub

    Private Sub MainButtonLangueAr_Click(sender As System.Object, e As System.Windows.RoutedEventArgs)

        Dim UpdateProjectName As Boolean = False

        If ProjectName = GetTextLangue("TitreProjet", "Nom du projet") Then UpdateProjectName = True

        LangueAppli = "ar"
        ChangeLangueInLayout()
        LangueButtonSwitch()

        If UpdateProjectName Then ProjectName = GetTextLangue("TitreProjet", "Nom du projet")

    End Sub

    Private Sub MainButtonLangueJp_Click(sender As System.Object, e As System.Windows.RoutedEventArgs)

        Dim UpdateProjectName As Boolean = False

        If ProjectName = GetTextLangue("TitreProjet", "Nom du projet") Then UpdateProjectName = True

        LangueAppli = "jp"
        ChangeLangueInLayout()
        LangueButtonSwitch()

        If UpdateProjectName Then ProjectName = GetTextLangue("TitreProjet", "Nom du projet")

    End Sub
    Private Sub MainButtonLangueKr_Click(sender As Object, e As RoutedEventArgs)

        Dim UpdateProjectName As Boolean = False

        If ProjectName = GetTextLangue("TitreProjet", "Nom du projet") Then UpdateProjectName = True

        LangueAppli = "kr"
        ChangeLangueInLayout()
        LangueButtonSwitch()

        If UpdateProjectName Then ProjectName = GetTextLangue("TitreProjet", "Nom du projet")

    End Sub

    Private Sub ButtonLangueExportXAML_Click(sender As System.Object, e As System.Windows.RoutedEventArgs)

        GetAllNameAndText()

    End Sub

    Private Sub ButtonLangueExportCSV_Click(sender As System.Object, e As System.Windows.RoutedEventArgs)

        GetAllNameAndTextCSV()

    End Sub

    Private Sub ButtonLangueChangeCSV_Click(sender As System.Object, e As System.Windows.RoutedEventArgs)

        ChangeCSVToXml()

    End Sub

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

    Private Function GetTextUpdate() As String

        Select Case LangueAppli

            Case "en"
                Return "Version " + VersionTxt.ToString + " available" + vbCrLf + "Do you want install it ?"

            Case "fr"
                Return "Version " + VersionTxt.ToString + " disponible" + vbCrLf + vbCrLf + "Voulez-vous l'installer ?"

        End Select

        Return "Version " + VersionTxt.ToString + " available" + vbCrLf + "Do you want install it ?"

    End Function

#End Region

End Class
