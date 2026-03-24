Imports BuoyLib.Buoy
Imports Microsoft.Win32
Imports System.IO
Imports System.Security.Principal

Module MCommun

    Public Enum ENUM_ALERT

        Information = 0
        Attention = 1
        Erreur = 2

    End Enum

    Public RepertoireTravail As String = My.Application.Info.DirectoryPath + "\WorkingDirectory"
    Public RepertoireTravailRoot As String = My.Application.Info.DirectoryPath

    Public ApplicationArgs() As String
    Public BaseBouees As New CBaseTravail
    Public BaseGeneral As CBaseGeneral

    Public EnglishFormat As New System.Globalization.CultureInfo("en")

    Public ProjectName As String = "Nom du projet"
    Public LangueAppli As String = ""

    Public Version As String = ""

    Public Sub FirstCheckDirectory()

        ' Verification que le dossier existe ou pas
        If Not IO.Directory.Exists(RepertoireTravail) Then

            Try
                RepertoireTravailRoot = System.Deployment.Application.ApplicationDeployment.CurrentDeployment.DataDirectory
                RepertoireTravail = RepertoireTravailRoot + "\WorkingDirectory"
            Catch
            End Try

        End If

        BaseGeneral = New CBaseGeneral(RepertoireTravail)
        CheckDirectory()

    End Sub

    Public Sub CheckDirectory()

        If IO.Directory.Exists(RepertoireTravail) Then
            BaseBouees.SetRepertoireTravail(RepertoireTravail)
        Else
            MessageBox.Show("Error data folder corrupted")
        End If

    End Sub

    Public Function GetDirectoryRoot() As String

        Return RepertoireTravailRoot

    End Function

    Public Function DisplayDemande(ByVal WindowParent As Window, ByVal Text As String) As Boolean

        Dim FDemande As New FenetreDemande
        FDemande.Message = Text
        If WindowParent IsNot Nothing Then FDemande.Owner = WindowParent

        Return FDemande.ShowDialog

    End Function

    Public Function DisplayInput(ByVal WindowParent As Window, ByVal Prompt As String, ByVal DefaultReponse As String) As String

        Dim FInput As New FenetreInput
        FInput.Message = Prompt
        FInput.TextBoxMessage = DefaultReponse
        If WindowParent IsNot Nothing Then FInput.Owner = WindowParent

        If FInput.ShowDialog Then
            Return FInput.TextBoxMessage
        End If

        Return ""

    End Function

    Public Function DisplayInformation(ByVal WindowParent As Window, ByVal Text As String) As Boolean

        Dim FInformation As New FenetreInformation
        FInformation.Message = Text
        If WindowParent IsNot Nothing Then FInformation.Owner = WindowParent

        Return FInformation.ShowDialog

    End Function

    Public Sub DisplayAlert(ByVal WindowParent As Window, ByVal Text As String)

        Dim FAlert As New FenetreAlert
        FAlert.Message = Text
        If WindowParent IsNot Nothing Then FAlert.Owner = WindowParent

        FAlert.ShowDialog()

    End Sub

    Public Function GetIconCalmar() As ImageSource

        Dim WpfBmp As ImageSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(My.Resources.IconCalmar.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions())
        Return WpfBmp

    End Function

    Public Structure STR_LANG

        Public Name As String
        Public Text As String

    End Structure

    Public LangDictionnary As New Dictionary(Of String, List(Of STR_LANG))

    Public Sub ReadAllLangFile()

        Dim SplitFs() As String
        Dim LangFichier As String
        Dim TextRd As String
        Dim RdXML As System.Xml.XmlReader

        LangueAppli = System.Globalization.CultureInfo.CurrentCulture.Name.Substring(0, 2)

        For Each fichier In IO.Directory.GetFiles(RepertoireTravailRoot + "\Langues", "lang.*.xml")

            SplitFs = fichier.Split(".")
            If SplitFs.Length < 3 Then Continue For

            LangFichier = SplitFs(SplitFs.Length - 2)
            Try
                TextRd = IO.File.ReadAllText(fichier)
                RdXML = System.Xml.XmlReader.Create(New IO.StringReader(TextRd))

                While RdXML.Read()

                    ' Verification que l'on est bien sur un element
                    If RdXML.NodeType = System.Xml.XmlNodeType.Element Then

                        ' Verification que l'on est bien sur une ligne Langue
                        If RdXML.Name = "Lang" Then

                            ' Verification du dictionnaire
                            If Not LangDictionnary.ContainsKey(LangFichier) Then LangDictionnary.Add(LangFichier, New List(Of STR_LANG))

                            ' Enregistrement de la nouvelle chaine
                            LangDictionnary(LangFichier).Add(New STR_LANG() With {.Name = RdXML.GetAttribute("Name"), .Text = RdXML.GetAttribute("Text")})
                        End If
                    End If
                End While
            Catch
            End Try

        Next

#If DEBUG Then

        Dim InfoRetString As String = ""
        Dim ListElementDouble As New List(Of String)

        For Each dt In LangDictionnary
            For p0 = 0 To dt.Value.Count - 1
                For p1 = p0 + 1 To dt.Value.Count - 1
                    If dt.Value(p0).Name = dt.Value(p1).Name Then
                        ListElementDouble.Add(dt.Key + " - " + dt.Value(p0).Name)
                    End If
                Next
            Next
        Next

        If ListElementDouble.Count > 0 AndAlso DisplayDemande(Nothing, "Erreur " + ListElementDouble.Count.ToString + " champs sont en double" + vbCrLf + "Voulez-vous les afficher?") Then
            For Each Ele In ListElementDouble
                InfoRetString += Ele + vbCrLf
            Next
            DisplayInformation(Nothing, InfoRetString)
        End If

#End If

        ProjectName = GetTextLangue("TitreProjet", "Nom du projet")

    End Sub

    Public Sub GetAllNameAndText()

        Dim TextRd As String
        Dim RdXML As System.Xml.XmlReader

        Dim Fw As IO.StreamWriter
        Dim DialFile As New System.Windows.Forms.FolderBrowserDialog
        Dim DirInfo As IO.DirectoryInfo
        Dim DictionnaryLangue As KeyValuePair(Of String, String)() = {New KeyValuePair(Of String, String)("\MainWindow.xaml", "Main"), _
                                                                      New KeyValuePair(Of String, String)("\Fenetre Complementaire\FenetreBouee.xaml", "ManagerBuoy"), _
                                                                      New KeyValuePair(Of String, String)("\Fenetre Complementaire\FenetreEquipement.xaml", "Equip"), _
                                                                      New KeyValuePair(Of String, String)("\Fenetre Complementaire\FenetreResultatRibbon.xaml", "Result"), _
                                                                      New KeyValuePair(Of String, String)("\Fenetre Complementaire\FenetreVisualisation.xaml", "View"), _
                                                                      New KeyValuePair(Of String, String)("\Langues\TextLangue.xaml", "General")}

        If DialFile.ShowDialog Then

            Try
                DirInfo = New IO.DirectoryInfo(DialFile.SelectedPath)
            Catch ex As Exception
                MessageBox.Show(ex.ToString)
                Return
            End Try

            Fw = New IO.StreamWriter(DirInfo.FullName + "\Langues\lang.fr.xml", False, System.Text.Encoding.UTF8)
            Fw.WriteLine("<?xml version=""1.0"" encoding=""utf-8"" ?>")
            Fw.WriteLine("<Calmar>")

            For Each fs In DictionnaryLangue

                If Not IO.File.Exists(DirInfo.FullName + "\" + fs.Key) Then Continue For

                Fw.WriteLine(" ")
                Fw.WriteLine(" ")
                Fw.WriteLine(vbTab + "<!-- " + fs.Value + " -->")
                Fw.WriteLine(" ")

                Try

                    TextRd = IO.File.ReadAllText(DirInfo.FullName + "\" + fs.Key)
                    RdXML = System.Xml.XmlReader.Create(New IO.StringReader(TextRd))

                    While RdXML.Read()

                        ' Verification que l'on est bien sur un element
                        If RdXML.NodeType = System.Xml.XmlNodeType.Element AndAlso RdXML.GetAttribute("Name") <> "" Then
                            If Not RdXML.GetAttribute("Name").StartsWith(fs.Value) Then Continue While

                            ' Verification que l'on est bien sur une ligne Langue
                            Select Case RdXML.Name

                                Case "TextBlock"
                                    If RdXML.GetAttribute("Text") <> "" Then _
                                        Fw.WriteLine(vbTab + "<Lang Name=""" + RdXML.GetAttribute("Name") + """ Text=""" + RdXML.GetAttribute("Text") + """ />")

                                Case "ribbon:RibbonButton"
                                    If RdXML.GetAttribute("Label") <> "" Then _
                                        Fw.WriteLine(vbTab + "<Lang Name=""" + RdXML.GetAttribute("Name") + """ Text=""" + RdXML.GetAttribute("Label") + """ />")

                                Case "ribbon:RibbonToggleButton"
                                    If RdXML.GetAttribute("Label") <> "" Then _
                                        Fw.WriteLine(vbTab + "<Lang Name=""" + RdXML.GetAttribute("Name") + """ Text=""" + RdXML.GetAttribute("Label") + """ />")

                                Case "Button"
                                    If RdXML.GetAttribute("Content") <> "" Then _
                                        Fw.WriteLine(vbTab + "<Lang Name=""" + RdXML.GetAttribute("Name") + """ Text=""" + RdXML.GetAttribute("Content") + """ />")

                                Case "ribbon:RibbonTab"
                                    If RdXML.GetAttribute("Header") <> "" Then _
                                        Fw.WriteLine(vbTab + "<Lang Name=""" + RdXML.GetAttribute("Name") + """ Text=""" + RdXML.GetAttribute("Header") + """ />")

                                Case "ribbon:RibbonGroup"
                                    If RdXML.GetAttribute("Header") <> "" Then _
                                        Fw.WriteLine(vbTab + "<Lang Name=""" + RdXML.GetAttribute("Name") + """ Text=""" + RdXML.GetAttribute("Header") + """ />")

                                Case "ComboBox"
                                    Fw.WriteLine(vbTab + "<Lang Name=""" + RdXML.GetAttribute("Name") + """ Text="""" />")

                                Case "RadioButton"
                                    If RdXML.GetAttribute("Content") <> "" Then _
                                        Fw.WriteLine(vbTab + "<Lang Name=""" + RdXML.GetAttribute("Name") + """ Text=""" + RdXML.GetAttribute("Content") + """ />")

                                Case "chartingToolkit:LinearAxis"
                                    If RdXML.GetAttribute("Title") <> "" Then _
                                        Fw.WriteLine(vbTab + "<Lang Name=""" + RdXML.GetAttribute("Name") + """ Text=""" + RdXML.GetAttribute("Title") + """ />")

                            End Select
                        End If

                    End While

                Catch
                End Try

            Next

            Fw.WriteLine(" ")
            Fw.WriteLine("</Calmar>")
            Fw.Close()

        End If

    End Sub

    Public Sub ChangeCSVToXml()

        Dim TextRd() As String

        Dim Rw As IO.StreamReader
        Dim Fw As IO.StreamWriter
        Dim DialFile As New OpenFileDialog

        DialFile.Multiselect = True
        DialFile.Filter = "Fichier CSV|*.csv"

        If DialFile.ShowDialog Then

            For Each fs In DialFile.FileNames

                Try
                    Rw = New IO.StreamReader(fs, System.Text.Encoding.UTF8)
                    Fw = New IO.StreamWriter(fs + ".xml", False, System.Text.Encoding.UTF8)
                Catch ex As Exception
                    Continue For
                End Try

                Fw.WriteLine("<?xml version=""1.0"" encoding=""utf-8"" ?>")
                Fw.WriteLine("<Calmar>")

                While Not Rw.EndOfStream

                    TextRd = Rw.ReadLine().Split(";")
                    If TextRd.Length >= 2 Then Fw.WriteLine(vbTab + "<Lang Name=""" + TextRd(0) + """ Text=""" + TextRd(1) + """ />")

                End While

                Fw.WriteLine(" ")
                Fw.WriteLine("</Calmar>")
                Fw.Close()

            Next

        End If

    End Sub

    Public Sub GetAllNameAndTextCSV()

        Dim SplitFs() As String
        Dim LangFichier As String
        Dim TextRd As String
        Dim RdXML As System.Xml.XmlReader
        Dim fw As IO.StreamWriter
        Dim Sep As Char = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator

        LangueAppli = System.Globalization.CultureInfo.CurrentCulture.Name.Substring(0, 2)

        For Each fichier In IO.Directory.GetFiles(RepertoireTravailRoot + "\Langues", "lang.*.xml")

            SplitFs = fichier.Split(".")
            If SplitFs.Length < 3 Then Continue For

            LangFichier = SplitFs(SplitFs.Length - 2)
            Try
                fw = New IO.StreamWriter(fichier + ".csv", False, System.Text.Encoding.UTF8)
                TextRd = IO.File.ReadAllText(fichier, System.Text.Encoding.UTF8)
                RdXML = System.Xml.XmlReader.Create(New IO.StringReader(TextRd))

                While RdXML.Read()

                    ' Verification que l'on est bien sur un element
                    If RdXML.NodeType = System.Xml.XmlNodeType.Element Then

                        ' Verification que l'on est bien sur une ligne Langue
                        If RdXML.Name = "Lang" Then

                            fw.WriteLine(RdXML.GetAttribute("Name") + Sep + RdXML.GetAttribute("Text"))

                        End If
                    End If
                End While
                fw.Close()
            Catch
            End Try

        Next

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

    ''' <summary>
    ''' Check if fileName is OK
    ''' </summary>
    ''' <param name="fileName">FileName</param>
    ''' <param name="allowPathDefinition">(optional) set true to allow path definitions. If set to false only filenames are allowed</param>
    ''' <param name="firstCharIndex">(optional) return the index of first invalid character</param>
    ''' <returns>true if filename is valid</returns>
    ''' <remarks>
    ''' based on Joel Coehoorn answer in 
    ''' http://stackoverflow.com/questions/1014242/valid-filename-check-what-is-the-best-way
    ''' </remarks>
    Public Function FilenameIsOK(ByVal fileName As String,
                                        Optional ByVal allowPathDefinition As Boolean = False,
                                        Optional ByRef firstCharIndex As Integer = Nothing) As Boolean

        Dim file As String = String.Empty
        Dim directory As String = String.Empty

        If allowPathDefinition Then
            file = Path.GetFileName(fileName)
            directory = Path.GetDirectoryName(fileName)
        Else
            file = fileName
        End If

        If Not IsNothing(firstCharIndex) Then
            Dim f As IEnumerable(Of Char)
            f = file.Intersect(Path.GetInvalidFileNameChars())
            If f.Any Then
                firstCharIndex = Len(directory) + file.IndexOf(f.First)
                Return False
            End If

            f = directory.Intersect(Path.GetInvalidPathChars())
            If f.Any Then
                firstCharIndex = directory.IndexOf(f.First)
                Return False
            Else
                Return True
            End If
        Else
            Return Not (file.Intersect(Path.GetInvalidFileNameChars()).Any() _
                        OrElse
                        directory.Intersect(Path.GetInvalidPathChars()).Any())
        End If

    End Function

End Module
