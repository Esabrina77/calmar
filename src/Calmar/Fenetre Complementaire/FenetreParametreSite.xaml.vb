Imports System.Windows.Controls.Ribbon

Public Class FenetreParametreSite

    Private _Trainee As Double
    Public Property Trainee As Double
        Get
            Return _Trainee
        End Get
        Set(value As Double)
            _Trainee = value
        End Set
    End Property

    Private _DensiteEau As Double
    Public Property DensiteEau As Double
        Get
            Return _DensiteEau
        End Get
        Set(value As Double)
            _DensiteEau = value
        End Set
    End Property

    Private _Longitude As Double
    Public Property Longitude As Double
        Get
            Return _Longitude
        End Get
        Set(value As Double)
            _Longitude = value
        End Set
    End Property

    Private _Latitude As Double
    Public Property Latitude As Double
        Get
            Return _Latitude
        End Get
        Set(value As Double)
            _Latitude = value
        End Set
    End Property

    Private _Loaded As Boolean = False

    Private Sub ButtonValidationSite_Click(sender As System.Object, e As System.Windows.RoutedEventArgs)

        Me.DialogResult = True

    End Sub

    Private Sub FenetreParametreSite_Loaded(sender As Object, e As System.Windows.RoutedEventArgs) Handles Me.Loaded

        Title = "Calmar " + Version
        Icon = GetIconCalmar()

        ChangeLangueInLayout()

        FenetreParametreSiteTextBoxLatitude.Text = Latitude.ToString("0.0000", EnglishFormat)
        FenetreParametreSiteTextBoxLongitude.Text = Longitude.ToString("0.0000", EnglishFormat)

        If DensiteEau = 1.025 Then
            FenetreParametreSiteComboBoxDensiteEau.SelectedIndex = 0
            FenetreParametreSiteTextBoxDensiteEau.IsReadOnly = True
        ElseIf DensiteEau = 1 Then
            FenetreParametreSiteComboBoxDensiteEau.SelectedIndex = 1
            FenetreParametreSiteTextBoxDensiteEau.IsReadOnly = True
        Else
            FenetreParametreSiteComboBoxDensiteEau.SelectedIndex = 2
            FenetreParametreSiteTextBoxDensiteEau.IsReadOnly = False
        End If
        FenetreParametreSiteTextBoxDensiteEau.Text = DensiteEau.ToString("0.000", EnglishFormat)

        Select Case Trainee

            Case 1.4
                FenetreParametreSiteComboBoxTrainee.SelectedIndex = 0

            Case 1.2
                FenetreParametreSiteComboBoxTrainee.SelectedIndex = 1

            Case 0.9
                FenetreParametreSiteComboBoxTrainee.SelectedIndex = 2

            Case 0.6
                FenetreParametreSiteComboBoxTrainee.SelectedIndex = 3

            Case Else
                FenetreParametreSiteComboBoxTrainee.SelectedIndex = 1
                Trainee = 1.2

        End Select
        FenetreParametreSiteTextBoxTrainee.Text = Trainee.ToString("0.0", EnglishFormat)

        _Loaded = True

    End Sub

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

    Private Sub FenetreParametreSiteComboBoxTrainee_SelectionChanged(sender As System.Object, e As System.Windows.Controls.SelectionChangedEventArgs)

        If Not _Loaded Then Return

        Select Case FenetreParametreSiteComboBoxTrainee.SelectedIndex

            Case 0
                Trainee = 1.4

            Case 1
                Trainee = 1.2

            Case 2
                Trainee = 0.9

            Case 3
                Trainee = 0.6

            Case Else
                Return

        End Select
        FenetreParametreSiteTextBoxTrainee.Text = Trainee.ToString("0.0000", EnglishFormat)

    End Sub

    Private Sub FenetreParametreSiteTextBoxLongitude_TextChanged(sender As System.Object, e As System.Windows.Controls.TextChangedEventArgs)

        If Not _Loaded Then Return

        Double.TryParse(sender.Text, Globalization.NumberStyles.Any, EnglishFormat, Longitude)

    End Sub

    Private Sub FenetreParametreSiteTextBoxLatitude_TextChanged(sender As System.Object, e As System.Windows.Controls.TextChangedEventArgs)

        If Not _Loaded Then Return

        Double.TryParse(sender.Text, Globalization.NumberStyles.Any, EnglishFormat, Latitude)

    End Sub

    Private Sub FenetreParametreSiteTextBoxDensiteEau_TextChanged(sender As System.Object, e As System.Windows.Controls.TextChangedEventArgs)

        Dim DensiteEauTmp As Double
        If Not _Loaded Then Return

        Double.TryParse(sender.Text, Globalization.NumberStyles.Any, EnglishFormat, DensiteEauTmp)

        If DensiteEauTmp >= 1 And DensiteEauTmp <= 1.275 Then
            DensiteEau = DensiteEauTmp
        Else
            sender.Text = DensiteEau.ToString("0.000", EnglishFormat)
        End If

    End Sub

    Private Sub FenetreParametreSiteComboBoxDensiteEau_SelectionChanged(sender As System.Object, e As System.Windows.Controls.SelectionChangedEventArgs)

        If Not _Loaded Then Return

        If FenetreParametreSiteComboBoxDensiteEau.SelectedIndex = 0 Then
            DensiteEau = 1.025
            FenetreParametreSiteTextBoxDensiteEau.Text = DensiteEau.ToString("0.000", EnglishFormat)
            FenetreParametreSiteTextBoxDensiteEau.IsReadOnly = True
        ElseIf FenetreParametreSiteComboBoxDensiteEau.SelectedIndex = 1 Then
            DensiteEau = 1.0
            FenetreParametreSiteTextBoxDensiteEau.Text = DensiteEau.ToString("0.000", EnglishFormat)
            FenetreParametreSiteTextBoxDensiteEau.IsReadOnly = True
        Else
            FenetreParametreSiteTextBoxDensiteEau.IsReadOnly = False
        End If

    End Sub

End Class
