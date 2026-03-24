Public Class FolderExpand

    Private dummyNode As Object = Nothing

    Private Sub FolderExpand_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded

        For Each s In IO.Directory.GetLogicalDrives
            Dim item = New TreeViewItem()

            item.Header = s
            item.Tag = s
            item.FontWeight = FontWeights.Normal
            item.Items.Add(dummyNode)
            AddHandler item.Expanded, New RoutedEventHandler(AddressOf folder_Expanded)
            TreeViewExplorer.Items.Add(item)

        Next

    End Sub

    Private Sub folder_Expanded(ByVal sender As Object, ByVal e As RoutedEventArgs)

        Dim item As TreeViewItem = sender

        If item.Items.Count = 1 AndAlso item.Items(0) = dummyNode Then
            item.Items.Clear()
            Try
                For Each s In IO.Directory.GetDirectories(item.Tag.ToString())
                    Dim subitem = New TreeViewItem()

                    subitem.Header = s
                    subitem.Tag = s
                    subitem.FontWeight = FontWeights.Normal
                    subitem.Items.Add(dummyNode)
                    AddHandler subitem.Expanded, New RoutedEventHandler(AddressOf folder_Expanded)
                    item.Items.Add(subitem)

                Next
            Catch ex As Exception
            End Try
        End If

    End Sub

    Private Sub TreeViewExplorer_SelectedItemChanged(ByVal sender As System.Object, ByVal e As System.Windows.RoutedPropertyChangedEventArgs(Of System.Object))

    End Sub
End Class
