Imports System.ComponentModel
Imports System.Windows.Threading

Public Class ListBuoy

    Enum EnumTri As Integer

        Nom = 0
        Volume = 1

    End Enum

    Private TriCroissant As Boolean = True
    Private TriActuelle As EnumTri = -1
    Private _DictonnaireElements As New Dictionary(Of String, BuoyLib.Buoy.CBouee)
    Private _Elements As New List(Of BuoyLib.Buoy.CBouee)

    ReadOnly Property GetElements As BuoyLib.Buoy.CBouee()
        Get
            Return _Elements.ToArray
        End Get
    End Property

    Public Sub SetElements(ByVal DictonnaireElements As Dictionary(Of String, BuoyLib.Buoy.CBouee))

        _DictonnaireElements = DictonnaireElements
        If TriActuelle <> -1 Then
            Select Case TriActuelle

                Case EnumTri.Nom
                    If TriCroissant Then
                        _DictonnaireElements = SortDictionaryNameAsc(_DictonnaireElements)
                    Else
                        _DictonnaireElements = SortDictionaryNameDesc(_DictonnaireElements)
                    End If

                Case EnumTri.Volume
                    If TriCroissant Then
                        _DictonnaireElements = SortDictionaryVolumeAsc(_DictonnaireElements)
                    Else
                        _DictonnaireElements = SortDictionaryVolumeDesc(_DictonnaireElements)
                    End If


            End Select
        End If

        Me.Dispatcher.Invoke(Sub()
                                 BuoyElements.Visibility = Visibility.Collapsed
                                 BorderProgress.Visibility = Visibility.Visible
                             End Sub, DispatcherPriority.Background)

        PopulateListBox()

        LastIndex = -1

    End Sub

    Private Sub PopulateListBox()

        Dim dic As New Dictionary(Of String, Canvas)

        BuoyElements.SelectedIndex = -1
        BuoyElements.Items.Clear()

        _Elements.Clear()
        BuoyElements.Items.Clear()

        For Each el In _DictonnaireElements
            _Elements.Add(el.Value)
            BuoyElements.Items.Add(CreateGrid(el.Value))
        Next

        Me.Dispatcher.Invoke(Sub()
                                 BorderProgress.Visibility = Visibility.Collapsed
                                 BuoyElements.Visibility = Visibility.Visible
                             End Sub, DispatcherPriority.Background)

    End Sub

    Public Sub Unselect()

        If BuoyElements.SelectedIndex < 0 Then Return
        BuoyElements.SelectedIndex = -1

    End Sub

    Public Function IsMouseOnSelectItem(ByVal e As MouseButtonEventArgs) As Boolean

        Dim i As Point = e.GetPosition(BuoyElements.SelectedItem)
        If i.X < -2 Or i.X > 122 Then Return False
        If i.Y < -2 Or i.Y > 122 Then Return False

        Return True

    End Function

    Private Function CreateGrid(ByVal Buoy As BuoyLib.Buoy.CBouee) As Grid

        Dim Grd As New Grid
        Dim TxB As New TextBlock
        Dim Img As Image
        Dim Cns As Canvas = New CBuoyDrawing(Buoy)

        Grd.RowDefinitions.Add(New RowDefinition())
        Grd.RowDefinitions.Add(New RowDefinition() With {.Height = New GridLength(50)})

        Cns.Height = 70
        Cns.SetValue(Grid.RowProperty, 0)
        Grd.Children.Add(Cns)

        TxB.Text = Buoy.Nom
        TxB.TextAlignment = TextAlignment.Center
        TxB.VerticalAlignment = Windows.VerticalAlignment.Center
        TxB.TextWrapping = TextWrapping.Wrap
        TxB.SetValue(Grid.RowProperty, 1)
        Grd.Children.Add(TxB)
        If Buoy.Protege Then
            Img = New Image()
            Img.Width = 16
            Img.Height = 16
            Img.Source = New BitmapImage(New Uri("/Calmar;component/Images/lock.png", UriKind.Relative))

            Img.Margin = New Thickness(5, 5, 0, 0)
            Img.HorizontalAlignment = Windows.HorizontalAlignment.Left
            Img.VerticalAlignment = Windows.VerticalAlignment.Top

            Grd.Children.Add(Img)
        End If

        Grd.Margin = New Thickness(2)

        Grd.Width = 120
        Grd.Height = 120

        Return Grd

    End Function

    ReadOnly Property SelectElement As BuoyLib.Buoy.CBouee
        Get
            If BuoyElements.SelectedIndex < 0 Then Return Nothing
            Return _Elements(BuoyElements.SelectedIndex)
        End Get
    End Property

    Public Event SelectedChange(sender As Object, e As BuoyLib.Buoy.CBouee)

    Private LastIndex As Integer = -1

    Private Sub BuoyElements_SelectionChanged(sender As Object, e As System.Windows.Controls.SelectionChangedEventArgs) Handles BuoyElements.SelectionChanged

        RaiseEvent SelectedChange(Me, SelectElement)

    End Sub

    Public Sub AucunTri()

        TriActuelle = -1

    End Sub

    Public Sub TriCroissantDe(ByVal Tri As EnumTri)

        TriActuelle = Tri
        TriCroissant = True

    End Sub

    Public Sub TriDecroissantDe(ByVal Tri As EnumTri)

        TriActuelle = Tri
        TriCroissant = False

    End Sub

    Private Function SortDictionaryNameAsc(ByVal dict As Dictionary(Of String, BuoyLib.Buoy.CBouee)) As Dictionary(Of String, BuoyLib.Buoy.CBouee)

        Dim final = From key In dict.Keys Order By key Ascending Select key

        SortDictionaryNameAsc = New Dictionary(Of String, BuoyLib.Buoy.CBouee)
        For Each item In final
            SortDictionaryNameAsc.Add(item, dict(item))
        Next

    End Function

    Private Function SortDictionaryVolumeAsc(ByVal dict As Dictionary(Of String, BuoyLib.Buoy.CBouee)) As Dictionary(Of String, BuoyLib.Buoy.CBouee)

        Dim final As IEnumerable = From Value In dict.Values Order By Value.VOLUME Ascending Select Value.Nom

        SortDictionaryVolumeAsc = New Dictionary(Of String, BuoyLib.Buoy.CBouee)
        For Each item In final
            SortDictionaryVolumeAsc.Add(item, dict(item))
        Next

    End Function

    Private Function SortDictionaryNameDesc(ByVal dict As Dictionary(Of String, BuoyLib.Buoy.CBouee)) As Dictionary(Of String, BuoyLib.Buoy.CBouee)

        Dim final As IEnumerable = From key In dict.Keys Order By key Descending Select key

        SortDictionaryNameDesc = New Dictionary(Of String, BuoyLib.Buoy.CBouee)
        For Each item In final
            SortDictionaryNameDesc.Add(item, dict(item))
        Next

    End Function

    Private Function SortDictionaryVolumeDesc(ByVal dict As Dictionary(Of String, BuoyLib.Buoy.CBouee)) As Dictionary(Of String, BuoyLib.Buoy.CBouee)

        Dim final As IEnumerable = From Value In dict.Values Order By Value.VOLUME Descending Select Value.Nom

        SortDictionaryVolumeDesc = New Dictionary(Of String, BuoyLib.Buoy.CBouee)
        For Each item In final
            SortDictionaryVolumeDesc.Add(item, dict(item))
        Next

    End Function

End Class
