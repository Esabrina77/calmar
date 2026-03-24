Imports System.Windows.Controls.Ribbon

Public Class FenetreEquipement

    Private ListEquipement As New Dictionary(Of String, List(Of CEquipementSupplementaireTreeView))
    Public ListEquipementSelectionner As New Dictionary(Of String, List(Of CEquipementSupplementaireTreeView))

    Public Class CEquipementSupplementaireTreeView
        Inherits BuoyLib.Buoy.CEquipementSupplementaire

        Public Sub New()
            MyBase.New()
        End Sub

        ReadOnly Property TreeViewName As String
            Get
                Return Name + " (" + MasseUnitaire.ToString(EnglishFormat) + " kg)"
            End Get
        End Property

        ReadOnly Property TreeViewNameCount As String
            Get
                Return Nombre.ToString + "x " + Name + " (" + MasseUnitaire.ToString(EnglishFormat) + " kg)"
            End Get
        End Property

    End Class

    Private Sub Window_Loaded(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles MyBase.Loaded

        Title = "Calmar " + Version
        Icon = GetIconCalmar()

        ChangeLangueInLayout()

        ChargementEquipementDepuisBase()

        If ListEquipementSelectionner.Count > 0 Then
            TreeViewEquipementSelect.ItemsSource = ListEquipementSelectionner
            CalculSommeMasse()
        End If

    End Sub

    Private Sub ChargementEquipementDepuisBase()

        TreeViewEquipement.ItemsSource = Nothing
        ListEquipement.Clear()

        For Each it In BaseGeneral.GetEquipement
            AjoutEquipementInTreeView(TreeViewEquipement, ListEquipement, EquipementSupInTreeView(it))
        Next

    End Sub

    Private Function EquipementSupInTreeView(ByVal EquipementSup As BuoyLib.Buoy.CEquipementSupplementaire) As CEquipementSupplementaireTreeView

        EquipementSupInTreeView = New CEquipementSupplementaireTreeView

        EquipementSupInTreeView.Categorie = EquipementSup.Categorie
        EquipementSupInTreeView.Name = EquipementSup.Name
        EquipementSupInTreeView.MasseUnitaire = EquipementSup.MasseUnitaire
        EquipementSupInTreeView.Nombre = EquipementSup.Nombre

    End Function

    Private Function EquipementSupTreeViewInTreeView(ByVal EquipementSup As CEquipementSupplementaireTreeView) As BuoyLib.Buoy.CEquipementSupplementaire

        EquipementSupTreeViewInTreeView = New BuoyLib.Buoy.CEquipementSupplementaire

        EquipementSupTreeViewInTreeView.Categorie = EquipementSup.Categorie
        EquipementSupTreeViewInTreeView.Name = EquipementSup.Name
        EquipementSupTreeViewInTreeView.MasseUnitaire = EquipementSup.MasseUnitaire
        EquipementSupTreeViewInTreeView.Nombre = EquipementSup.Nombre

    End Function

    Private Sub AjoutEquipementInTreeView(ByRef TreeV As TreeView, ByRef ListEquip As Dictionary(Of String, List(Of CEquipementSupplementaireTreeView)), ByVal Equipement As CEquipementSupplementaireTreeView)

        Dim p0 As Integer

        If Not ListEquip.ContainsKey(Equipement.Categorie) Then
            ListEquip.Add(Equipement.Categorie, New List(Of CEquipementSupplementaireTreeView))
        End If

        For p0 = 0 To ListEquip(Equipement.Categorie).Count - 1
            If ListEquip(Equipement.Categorie)(p0).Name = Equipement.Name Then
                Exit For
            End If
        Next

        If p0 < ListEquip(Equipement.Categorie).Count Then
            ListEquip(Equipement.Categorie)(p0).Nombre += 1
        Else
            Equipement.Nombre = 1
            ListEquip(Equipement.Categorie).Add(Equipement)
        End If

        TreeV.ItemsSource = Nothing
        TreeV.ItemsSource = ListEquip

    End Sub

    Private Sub SupprimeEquipementInTreeView(ByRef TreeV As TreeView, ByRef ListEquip As Dictionary(Of String, List(Of CEquipementSupplementaireTreeView)), ByVal Equipement As CEquipementSupplementaireTreeView)

        Dim p0 As Integer

        For p0 = 0 To ListEquip(Equipement.Categorie).Count - 1
            If ListEquip(Equipement.Categorie)(p0).Name = Equipement.Name Then
                Exit For
            End If
        Next

        If p0 >= ListEquip(Equipement.Categorie).Count Then Return

        If ListEquip(Equipement.Categorie)(p0).Nombre = 1 Then
            ListEquip(Equipement.Categorie).RemoveAt(p0)
            If ListEquip(Equipement.Categorie).Count = 0 Then
                ListEquip.Remove(Equipement.Categorie)
            End If
        Else
            ListEquip(Equipement.Categorie)(p0).Nombre -= 1
        End If

        TreeV.ItemsSource = Nothing
        TreeV.ItemsSource = ListEquip

    End Sub

    Private Sub TreeViewEquipement_SelectedItemChanged(sender As Object, e As System.Windows.RoutedPropertyChangedEventArgs(Of Object)) Handles TreeViewEquipement.SelectedItemChanged

        ButtonAjoutEquipement.IsEnabled = False
        If TreeViewEquipement.SelectedItem Is Nothing Then Return

        ButtonAjoutEquipement.IsEnabled = True

    End Sub

    Private Sub TreeViewEquipementSelect_SelectedItemChanged(sender As Object, e As System.Windows.RoutedPropertyChangedEventArgs(Of Object)) Handles TreeViewEquipementSelect.SelectedItemChanged

        ButtonSupprimeEquipement.IsEnabled = False
        If TreeViewEquipementSelect.SelectedItem Is Nothing Then Return

        ButtonSupprimeEquipement.IsEnabled = True

    End Sub

    Private Sub ButtonAjoutEquipement_Click(sender As System.Object, e As System.Windows.RoutedEventArgs)

        If TreeViewEquipement.SelectedItem.GetType Is GetType(CEquipementSupplementaireTreeView) Then
            AjoutEquipementInTreeView(TreeViewEquipementSelect, ListEquipementSelectionner, TreeViewEquipement.SelectedItem)
        ElseIf TreeViewEquipement.SelectedItem.GetType Is GetType(KeyValuePair(Of String, List(Of CEquipementSupplementaireTreeView))) Then
            ListEquipementSelectionner.Add(TreeViewEquipement.SelectedItem.Key, TreeViewEquipement.SelectedItem.Value)
            TreeViewEquipementSelect.ItemsSource = Nothing
            TreeViewEquipementSelect.ItemsSource = ListEquipementSelectionner
        End If

        CalculSommeMasse()

    End Sub

    Private Sub ButtonSupprimeEquipement_Click(sender As System.Object, e As System.Windows.RoutedEventArgs)

        If TreeViewEquipementSelect.SelectedItem.GetType Is GetType(CEquipementSupplementaireTreeView) Then
            SupprimeEquipementInTreeView(TreeViewEquipementSelect, ListEquipementSelectionner, TreeViewEquipementSelect.SelectedItem)
        ElseIf TreeViewEquipementSelect.SelectedItem.GetType Is GetType(KeyValuePair(Of String, List(Of CEquipementSupplementaireTreeView))) Then
            ListEquipementSelectionner.Remove(TreeViewEquipementSelect.SelectedItem.Key)
            TreeViewEquipementSelect.ItemsSource = Nothing
            TreeViewEquipementSelect.ItemsSource = ListEquipementSelectionner
        End If

        CalculSommeMasse()

    End Sub

    Private Sub CalculSommeMasse()

        Dim MasseTotale As Double = 0

        For Each it In ListEquipementSelectionner
            For Each itEquipement In it.Value
                MasseTotale += itEquipement.Masse
            Next
        Next

        TextBoxTotalMasseListeEquipement.Text = "Total : " + MasseTotale.ToString(EnglishFormat) + " kg"

    End Sub

    Private Sub ButtonValidationEquipement_Click(sender As System.Object, e As System.Windows.RoutedEventArgs)

        Me.DialogResult = True

    End Sub

    Private Sub ButtonAjouterEquipement_Click(sender As System.Object, e As System.Windows.RoutedEventArgs)

        Dim Equip As CEquipementSupplementaireTreeView

        If TreeViewEquipement.SelectedItem IsNot Nothing AndAlso TreeViewEquipement.SelectedItem.GetType Is GetType(KeyValuePair(Of String, List(Of CEquipementSupplementaireTreeView))) Then
            Equip = BoiteDialogueEquipement(New CEquipementSupplementaireTreeView() With {.Categorie = TreeViewEquipement.SelectedItem.Key}, False)
        Else
            Equip = BoiteDialogueEquipement(Nothing)
        End If

        If Equip.Categorie <> "" And Equip.Name <> "" And Equip.MasseUnitaire > 0 Then
            BaseGeneral.AjoutEquipement(EquipementSupTreeViewInTreeView(Equip))
            ChargementEquipementDepuisBase()
        End If

    End Sub

    Private Sub ButtonSupprimerEquipement_Click(sender As System.Object, e As System.Windows.RoutedEventArgs)

        If TreeViewEquipement.SelectedItem.GetType IsNot GetType(CEquipementSupplementaireTreeView) Then Return

        BaseGeneral.SupprimeEquipement(EquipementSupTreeViewInTreeView(TreeViewEquipement.SelectedItem))
        ChargementEquipementDepuisBase()

    End Sub

    Private Sub ButtonModifierEquipement_Click(sender As System.Object, e As System.Windows.RoutedEventArgs)

        Dim Equip As CEquipementSupplementaireTreeView

        If TreeViewEquipement.SelectedItem.GetType IsNot GetType(CEquipementSupplementaireTreeView) Then Return

        Equip = BoiteDialogueEquipement(TreeViewEquipement.SelectedItem)

        If Equip.Categorie <> TreeViewEquipement.SelectedItem.Categorie Or Equip.Name <> TreeViewEquipement.SelectedItem.Name Or Equip.MasseUnitaire <> TreeViewEquipement.SelectedItem.MasseUnitaire Then

            BaseGeneral.SupprimeEquipement(EquipementSupTreeViewInTreeView(TreeViewEquipement.SelectedItem))
            BaseGeneral.AjoutEquipement(Equip)

            ChargementEquipementDepuisBase()
        End If

    End Sub

    Private Function BoiteDialogueEquipement(ByVal Equip As CEquipementSupplementaireTreeView, Optional CategorieRequest As Boolean = True) As CEquipementSupplementaireTreeView

        BoiteDialogueEquipement = New CEquipementSupplementaireTreeView

        If Equip IsNot Nothing Then

            If CategorieRequest Then
                BoiteDialogueEquipement.Categorie = DisplayInput(Me, GetTextLangue("GeneralAjoutEquipementCategorie", "Categorie de l'equipement"), Equip.Categorie)
            Else
                BoiteDialogueEquipement.Categorie = Equip.Categorie
            End If
            If BoiteDialogueEquipement.Categorie <> "" Then BoiteDialogueEquipement.Name = DisplayInput(Me, GetTextLangue("GeneralAjoutEquipementNom", "Nom de l'equipement"), Equip.Name)
            If BoiteDialogueEquipement.Categorie <> "" And BoiteDialogueEquipement.Name <> "" Then Double.TryParse(DisplayInput(Me, GetTextLangue("GeneralAjoutEquipementMasse", "Masse unitaire de l'equipement (kg)"), Equip.MasseUnitaire.ToString(EnglishFormat)), Globalization.NumberStyles.Any, EnglishFormat, BoiteDialogueEquipement.MasseUnitaire)

        Else

            BoiteDialogueEquipement.Categorie = DisplayInput(Me, GetTextLangue("GeneralAjoutEquipementCategorie", "Categorie de l'equipement"), "")
            If BoiteDialogueEquipement.Categorie <> "" Then BoiteDialogueEquipement.Name = DisplayInput(Me, GetTextLangue("GeneralAjoutEquipementNom", "Nom de l'equipement"), "")
            If BoiteDialogueEquipement.Categorie <> "" And BoiteDialogueEquipement.Name <> "" Then Double.TryParse(DisplayInput(Me, GetTextLangue("GeneralAjoutEquipementMasse", "Masse unitaire de l'equipement (kg)"), "0"), Globalization.NumberStyles.Any, EnglishFormat, BoiteDialogueEquipement.MasseUnitaire)

        End If

    End Function

    Private Sub Window_KeyUp(sender As System.Object, e As System.Windows.Input.KeyEventArgs) Handles MyBase.KeyUp

        If e.Key = Key.Escape Then
            Me.DialogResult = False
        End If

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
