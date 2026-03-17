Imports BuoyLib.Buoy

Public Class FenetreCopie

    Private _SourceBuoy As CBouee
    Private _DestinationBuoy As CBouee

    Public Property SourceBuoy As CBouee
        Get
            Return _SourceBuoy
        End Get
        Set(value As CBouee)
            _SourceBuoy = value.Clone
        End Set
    End Property

    Public Property DestinationBuoy As CBouee
        Get
            Return _DestinationBuoy
        End Get
        Set(value As CBouee)
            _DestinationBuoy = value.Clone
        End Set
    End Property

    Private Sub FenetreCopie_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded

        Title = "Calmar " + Version
        Icon = GetIconCalmar()

        ChangeLangueInLayout()

        TextBlockNameBuoyCopy.Text = SourceBuoy.Nom
        BuoyCopy.Child = New CBuoyDrawing(SourceBuoy)

        TextBlockNameBuoyPaste.Text = DestinationBuoy.Nom
        BuoyPaste.Child = New CBuoyDrawing(DestinationBuoy)

    End Sub

    Private Sub CopyStructure_Click(sender As Object, e As RoutedEventArgs)

        DestinationBuoy.StructureBouee = SourceBuoy.StructureBouee.Clone

        DestinationBuoy.ChaineMax = SourceBuoy.ChaineMax
        DestinationBuoy.ChaineMin = SourceBuoy.ChaineMin

        DestinationBuoy.MasseLestUnitaire = SourceBuoy.MasseLestUnitaire
        DestinationBuoy.NombreLestMax = SourceBuoy.NombreLestMax
        DestinationBuoy.NombreLestMin = SourceBuoy.NombreLestMin

        BuoyPaste.Child = New CBuoyDrawing(DestinationBuoy)

    End Sub

    Private Sub CopyFlotteur_Click(sender As Object, e As RoutedEventArgs)

        DestinationBuoy.FlotteurBouee = SourceBuoy.FlotteurBouee.Clone
        BuoyPaste.Child = New CBuoyDrawing(DestinationBuoy)

    End Sub

    Private Sub CopyPylone_Click(sender As Object, e As RoutedEventArgs)

        DestinationBuoy.ClearPylone()
        For Each pyl As CPylone In SourceBuoy.PyloneBouee
            DestinationBuoy.NouveauPyloneBouee = pyl.Clone
        Next
        BuoyPaste.Child = New CBuoyDrawing(DestinationBuoy)

    End Sub

    Private Sub CopyVoyant_Click(sender As Object, e As RoutedEventArgs)

        DestinationBuoy.ClearEquipement()
        For Each equ As CEquipement In SourceBuoy.EquipementBouee
            DestinationBuoy.NouvelleEquipementBouee = equ.Clone
        Next
        BuoyPaste.Child = New CBuoyDrawing(DestinationBuoy)

    End Sub

    Private Sub EquipButtonValidationEquipement_Click(sender As Object, e As RoutedEventArgs)

        Me.DialogResult = True

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
