Imports System.IO
Imports System.Xml
Imports System.Xml.Serialization

Namespace Buoy

    Public Class CBaseGeneral

        Const Extension As String = ".xmlMB"

        Private RepertoireBase As String = ""
        Private EnglishFormat As New System.Globalization.CultureInfo("en")

        Private Chaines As New List(Of CChaine)
        Private Equipement As New List(Of CEquipementSupplementaire)

        Public Sub New(ByVal _RepertoireBase As String)

            RepertoireBase = _RepertoireBase

        End Sub

        ReadOnly Property GetChaines() As List(Of CChaine)
            Get
                Return Chaines
            End Get
        End Property

        ReadOnly Property GetEquipement() As List(Of CEquipementSupplementaire)
            Get
                Return Equipement
            End Get
        End Property

        Private Function getVersion() As Version

            Dim version As String = System.Reflection.Assembly.GetExecutingAssembly().FullName
            Dim start As Integer = version.IndexOf("Version=") + 8
            Dim length As Integer = version.IndexOf(",", start) - start

            Return New Version(version.Substring(start, length))

        End Function

        Public Sub LectureFichierGeneraux()

            Chaines.Clear()
            Equipement.Clear()

            If IO.File.Exists(RepertoireBase + "\Chains.xmlMB") Then _
                LectureFichierXMLBouee(RepertoireBase + "\Chains.xmlMB")

            If IO.File.Exists(RepertoireBase + "\Equipements.xmlMB") Then _
                LectureFichierXMLBouee(RepertoireBase + "\Equipements.xmlMB")

        End Sub

        Private Sub LectureFichierXMLBouee(ByVal Fichier As String)

            Dim TextRd As String
            Dim RdXML As XmlReader

            Try
                TextRd = IO.File.ReadAllText(Fichier)
                RdXML = XmlReader.Create(New StringReader(TextRd))
                LectureFichierBouee(RdXML)
            Catch
            End Try

        End Sub

        Public Function AjoutEquipement(ByVal NouvelleEquipement As CEquipementSupplementaire) As Boolean

            Dim p0 As Integer
            Dim fs As String = RepertoireBase + "\Equipements.xmlMB"

            AjoutEquipement = False

            ' Ajout du nouvelle equipement
            For p0 = 0 To Equipement.Count - 1
                If Equipement(p0).Categorie = NouvelleEquipement.Categorie And
                   Equipement(p0).Name = NouvelleEquipement.Name Then
                    Exit For
                End If
            Next

            If p0 < Equipement.Count Then Return False

            ' Ajout du nouvelle equipement
            Equipement.Add(NouvelleEquipement)

            If File.Exists(fs) Then
                Try
                    IO.File.Delete(fs)
                Catch ex As Exception
                    Return False
                End Try
            End If

            Try
                IO.File.WriteAllText(fs, WriteXmlEquipementSupplementaire)
                LectureFichierXMLBouee(fs)
            Catch
                Return False
            End Try

            AjoutEquipement = True

        End Function

        Public Function SupprimeEquipement(ByVal NouvelleEquipement As CEquipementSupplementaire) As Boolean

            Dim p0 As Integer
            Dim fs As String = RepertoireBase + "\Equipements.xmlMB"

            SupprimeEquipement = False

            ' Ajout du nouvelle equipement
            For p0 = 0 To Equipement.Count - 1
                If Equipement(p0).Categorie = NouvelleEquipement.Categorie And
                   Equipement(p0).Name = NouvelleEquipement.Name And
                   Equipement(p0).MasseUnitaire = NouvelleEquipement.MasseUnitaire Then
                    Exit For
                End If
            Next

            If p0 >= Equipement.Count Then Return False
            Equipement.RemoveAt(p0)

            If File.Exists(fs) Then
                Try
                    IO.File.Delete(fs)
                Catch ex As Exception
                    Return False
                End Try
            End If

            Try
                IO.File.WriteAllText(fs, WriteXmlEquipementSupplementaire)
                LectureFichierXMLBouee(fs)
            Catch
                Return False
            End Try

            SupprimeEquipement = True

        End Function


#Region "Lecture du fichier Base de données"

        Private Sub LectureFichierBouee(ByVal BaseXml As XmlReader)

            Dim VersionSW As Version = getVersion()
            Dim Version As Version = Nothing

            ' Lecture du fichier avec les bases
            While BaseXml.Read()
                Select Case BaseXml.NodeType
                    Case XmlNodeType.Element, XmlNodeType.EndElement
                        ' Lecture de la version de la base
                        If BaseXml.Name = "Calmar" Then
                            BaseXml.MoveToFirstAttribute()
                            If BaseXml.Name = "version" Then
                                Version = New Version(BaseXml.Value)
                            End If
                        Else
                            If Version = VersionSW Then
                                If BaseXml.Name = "Chain" Then
                                    LectureChaineElements(Chaines, BaseXml)
                                ElseIf BaseXml.Name = "EquipementSupplementaire" Then
                                    LectureEquipementSupplementaireElements(Equipement, BaseXml)
                                End If
                            Else
                                Throw New Exception("Version differentes")
                            End If
                        End If

                End Select
            End While

        End Sub

        Private Sub LectureChaineElements(ByRef dico As List(Of CChaine), ByVal BaseXml As XmlReader)

            Dim updateChaine As Boolean = False
            Dim NouveauElement As CChaine = Nothing

            While BaseXml.Read()

                If BaseXml.NodeType = XmlNodeType.Element Then
                    If BaseXml.Name = "ChainElementItem" Then
                        LectureChaineElement(NouveauElement, BaseXml)
                        updateChaine = True
                        For Each chn In dico
                            If chn.DN = NouveauElement.DN And chn.TYPE = NouveauElement.TYPE And chn.MASSE_LINEIQUE = NouveauElement.MASSE_LINEIQUE Then
                                updateChaine = False
                                Exit For
                            End If
                        Next
                        If updateChaine Then dico.Add(NouveauElement)
                    End If
                End If

                If BaseXml.NodeType = XmlNodeType.EndElement Then
                    If BaseXml.Name = "Chain" Then Exit While
                End If

            End While

        End Sub

        Private Sub LectureChaineElement(ByRef NouveauElement As CChaine, ByVal BaseXml As XmlReader)

            ' Deplacement sur le premier attribut
            BaseXml.MoveToFirstAttribute()

            ' Declaration d'une structure
            NouveauElement = New CChaine

            ' Recuperation du nom de la flotteur
            NouveauElement.TYPE = BaseXml.Value.Replace(",", ".")
            ' Recuperation du volume
            BaseXml.MoveToNextAttribute()
            NouveauElement.DN = Double.Parse(BaseXml.Value, EnglishFormat)
            ' Recuperation de la masse
            BaseXml.MoveToNextAttribute()
            NouveauElement.MASSE_LINEIQUE = Double.Parse(BaseXml.Value, EnglishFormat)
            ' Recuperation de la charge d'epreuve pour la qualité 1
            BaseXml.MoveToNextAttribute()
            NouveauElement.CHARGE_EPREUVE_QL1 = Double.Parse(BaseXml.Value, EnglishFormat)
            ' Recuperation de la charge d'epreuve pour la qualité 2
            BaseXml.MoveToNextAttribute()
            NouveauElement.CHARGE_EPREUVE_QL2 = Double.Parse(BaseXml.Value, EnglishFormat)
            ' Recuperation de la charge d'epreuve pour la qualité 3
            BaseXml.MoveToNextAttribute()
            NouveauElement.CHARGE_EPREUVE_QL3 = Double.Parse(BaseXml.Value, EnglishFormat)

        End Sub

        Private Sub LectureEquipementSupplementaireElements(ByRef dico As List(Of CEquipementSupplementaire), ByVal BaseXml As XmlReader)

            Dim updateChaine As Boolean = False
            Dim NouveauElement As CEquipementSupplementaire = Nothing

            While BaseXml.Read()

                If BaseXml.NodeType = XmlNodeType.Element Then
                    If BaseXml.Name = "EquipementSupplementaireElementItem" Then
                        LectureEquipementSupplementaireElement(NouveauElement, BaseXml)
                        updateChaine = True
                        For Each chn In dico
                            If chn.Name = NouveauElement.Name And chn.Categorie = NouveauElement.Categorie And chn.MasseUnitaire = NouveauElement.MasseUnitaire Then
                                updateChaine = False
                                Exit For
                            End If
                        Next
                        If updateChaine Then dico.Add(NouveauElement)
                    End If
                End If

                If BaseXml.NodeType = XmlNodeType.EndElement Then
                    If BaseXml.Name = "EquipementSupplementaire" Then Exit While
                End If

            End While

        End Sub

        Private Sub LectureEquipementSupplementaireElement(ByRef NouveauElement As CEquipementSupplementaire, ByVal BaseXml As XmlReader)

            ' Deplacement sur le premier attribut
            BaseXml.MoveToFirstAttribute()

            ' Declaration d'une structure
            NouveauElement = New CEquipementSupplementaire
            NouveauElement.Nombre = 1

            ' Recuperation du nom de la flotteur
            NouveauElement.Categorie = BaseXml.Value
            ' Recuperation du volume
            BaseXml.MoveToNextAttribute()
            NouveauElement.Name = BaseXml.Value
            ' Recuperation de la masse
            BaseXml.MoveToNextAttribute()
            NouveauElement.MasseUnitaire = Double.Parse(BaseXml.Value, EnglishFormat)

        End Sub

#End Region


#Region "Ecriture du fichier Base de données"

        Private Function WriteXmlEquipementSupplementaire() As String

            WriteXmlEquipementSupplementaire = "<?xml version=""1.0"" encoding=""UTF-8"" ?>" + vbCrLf
            WriteXmlEquipementSupplementaire += "<Calmar version=""" + getVersion().ToString + """>" + vbCrLf

            '
            ' ENREGISTREMENT DES EQUIPEMENTS DE LA BOUEE
            '
            WriteXmlEquipementSupplementaire += "<EquipementSupplementaire>" + vbCrLf
            For Each it In Equipement
                WriteXmlEquipementSupplementaire += vbTab + "<EquipementSupplementaireElementItem Categorie=""" + it.Categorie + """ Name=""" + it.Name + """ MasseUnitaire=""" + it.MasseUnitaire.ToString(EnglishFormat) + """ />" + vbCrLf
            Next
            WriteXmlEquipementSupplementaire += "</EquipementSupplementaire>" + vbCrLf

            WriteXmlEquipementSupplementaire += "</Calmar>"

        End Function

        Private Function WriteXmlChaines() As String

            WriteXmlChaines = "<?xml version=""1.0"" encoding=""UTF-8"" ?>" + vbCrLf
            WriteXmlChaines += "<Calmar version=""" + getVersion().ToString + """>" + vbCrLf

            '
            ' ENREGISTREMENT DES EQUIPEMENTS DE LA BOUEE
            '
            WriteXmlChaines += "<Chain>" + vbCrLf
            For Each it In Chaines
                WriteXmlChaines += vbTab + "<ChainElementItem Type=""" + it.TYPE + """ DN=""" + it.DN.ToString + """ MasseLin=""" + it.MASSE_LINEIQUE.ToString(EnglishFormat) + """ />" + vbCrLf
            Next
            WriteXmlChaines += "</Chain>" + vbCrLf

            WriteXmlChaines += "</Calmar>"

        End Function

#End Region

    End Class

    Public Class CBaseTravail

        Const Extension As String = ".xmlMB"
        Const CryptExtension As String = ".prtMB"
        Private RepertoireBase As String = ""

        Private Bouees As New Dictionary(Of String, CBouee)

        Private EnglishFormat As New System.Globalization.CultureInfo("en")

        Public Sub New()

            RepertoireTravail = ""
            RepertoireBase = ""

        End Sub

        Public Sub SetRepertoireTravail(ByVal _RepertoireBase As String)

            RepertoireTravail = _RepertoireBase

            Try
                If Not IO.Directory.Exists(RepertoireTravail) Then IO.Directory.CreateDirectory(RepertoireTravail)
            Catch
            End Try

        End Sub

        Property RepertoireTravail() As String
            Get
                Return RepertoireBase
            End Get
            Set(value As String)
                If Not value.EndsWith("\") Then value += "\"
                RepertoireBase = value
            End Set
        End Property

        ReadOnly Property IsRepertoireTravail() As Boolean
            Get
                If RepertoireTravail = "" Then Return False
                Return IO.Directory.Exists(RepertoireBase)
            End Get
        End Property

        ReadOnly Property GetBouees() As Dictionary(Of String, CBouee)
            Get
                Return Bouees
            End Get
        End Property

        Public Sub LectureRepertoireTravail()

            If RepertoireBase = "" Then Return

            ' MODIFICATION POUR INTEGRATION BASE DES CHAINES
            '
            'If File.Exists(RepertoireTravail + "\Chains.csv") Then
            '    Migration_Chaine_CSV_XML()
            'End If

            Bouees.Clear()

            For Each fs In Directory.GetFiles(RepertoireTravail)
                AjoutFichier(fs)
            Next

        End Sub

        Private Sub Migration_Chaine_CSV_XML()

            Dim line_read As String
            Dim line_read_split() As String
            Dim Sread As StreamReader
            Dim Swrite As StreamWriter

            Try
                ' Ouverture du fichier de lecture
                Sread = New StreamReader(RepertoireTravail + "\Chains.csv")
                Swrite = New StreamWriter(RepertoireTravail + "\Chains.xml")

                ' Lecture de la premiere ligne
                If Not Sread.EndOfStream Then
                    Sread.ReadLine()
                    Swrite.Write("<?xml version=""1.0"" encoding=""UTF-8"" ?>" + vbCrLf + "<Calmar version=""1.0.0.0"">" + vbCrLf + vbTab + "<Chain>" + vbCrLf)
                End If

                While Not Sread.EndOfStream
                    line_read = Sread.ReadLine
                    If line_read <> "" Then
                        line_read_split = line_read.Split(";")
                        If line_read_split.Length = 6 Then
                            Swrite.Write(vbTab + vbTab +
                                         "<ChainElementItem Type=""" + line_read_split(0) +
                                         """ DN=""" + line_read_split(1) +
                                         """ MasseLin=""" + line_read_split(2).Replace(",", ".") +
                                         """ Q1_CE=""" + line_read_split(3).Replace(",", ".") +
                                         """ Q2_CE=""" + line_read_split(4).Replace(",", ".") +
                                         """ Q3_CE=""" + line_read_split(5).Replace(",", ".") +
                                         """ />" + vbCrLf)
                        End If
                    End If
                End While
                Swrite.Write(vbTab + "<Chain>" + vbCrLf + "</Calmar>")

                Swrite.Close()
                Sread.Close()
            Catch
            End Try

        End Sub

        Private Function AjoutFichier(ByVal Fichier As String) As String

            Dim Buoy As CBouee

            If Not Fichier.ToLower.EndsWith(Extension.ToLower) And Not Fichier.ToLower.EndsWith(CryptExtension.ToLower) Then Return ""

            Buoy = LectureFichierXMLBouee(Fichier)

            If Buoy.Nom <> "" Then
                ' Ajout de la bouee si non existant
                If Not Bouees.ContainsKey(Buoy.Nom) Then
                    Bouees.Add(Buoy.Nom, Buoy)
                    Return Buoy.Nom
                End If
            End If

            Return ""

        End Function

        Public Sub AjoutFichierBouee(ByVal Fichier As String)

            Dim FileAjout As String = ""

            If IsRepertoireTravail Then
                FileAjout = AjoutFichier(Fichier)
                If FileAjout <> "" Then
                    Try
                        If Fichier.ToLower.EndsWith(Extension.ToLower) Then
                            If Not File.Exists(RepertoireTravail + FileAjout + Extension) Then _
                                File.Copy(Fichier, RepertoireTravail + FileAjout + Extension)
                        ElseIf Fichier.ToLower.EndsWith(CryptExtension.ToLower) Then
                            If Not File.Exists(RepertoireTravail + FileAjout + CryptExtension) Then _
                                File.Copy(Fichier, RepertoireTravail + FileAjout + CryptExtension)
                        End If
                    Catch
                    End Try
                End If
            End If

        End Sub

        Public Function AjouterBouee(ByVal Buoy As CBouee) As Boolean

            AjouterBouee = False
            If Not IsRepertoireTravail Then Return AjouterBouee

            Try
                Bouees.Add(Buoy.Nom, Buoy)
                AjouterBouee = EnregistreFichierBouee(RepertoireTravail + "\" + Buoy.Nom + Extension, Buoy.Nom)
            Catch
                AjouterBouee = False
                Exit Function
            End Try

        End Function

        Public Function ModificationBouee(ByVal Nom As String, ByVal Buoy As CBouee) As Boolean

            ModificationBouee = False
            If Not IsRepertoireTravail Then Return ModificationBouee

            For Each fs In Directory.GetFiles(RepertoireTravail)
                If LectureFichierXMLBouee(fs).Nom = Nom Then
                    Try
                        IO.File.Delete(fs)
                        Bouees.Remove(Nom)
                        Bouees.Add(Buoy.Nom, Buoy)
                        EnregistreFichierBouee(RepertoireTravail + "\" + Buoy.Nom + Extension, Buoy.Nom)
                        ModificationBouee = True
                    Catch
                        ModificationBouee = False
                        Exit Function
                    End Try
                End If
            Next

        End Function

        Public Function SuppressionBouee(ByVal Nom As String) As Boolean

            Dim Buoy As CBouee

            SuppressionBouee = False
            If Not IsRepertoireTravail Then Return SuppressionBouee

            For Each fs In Directory.GetFiles(RepertoireTravail)
                Buoy = LectureFichierXMLBouee(fs)
                If Buoy.Nom = Nom Then
                    Try
                        IO.File.Delete(fs)
                        Bouees.Remove(Nom)
                        SuppressionBouee = True
                    Catch
                        SuppressionBouee = False
                        Exit Function
                    End Try
                End If
            Next

        End Function

        Private Function LectureFichierXMLBouee(ByVal Fichier As String) As CBouee

            Dim TextRd As String
            Dim RdXML As XmlReader

            LectureFichierXMLBouee = New CBouee
            Try
                TextRd = IO.File.ReadAllText(Fichier)
                If Fichier.EndsWith(CryptExtension) Then TextRd = MCrypt.AES_Decrypt(TextRd)
                RdXML = XmlReader.Create(New StringReader(TextRd))
                LectureFichierXMLBouee = LectureFichierBouee(RdXML)
                LectureFichierXMLBouee.File = TextRd
                If Fichier.EndsWith(CryptExtension) Then LectureFichierXMLBouee.Protege = True
            Catch
            End Try

        End Function

        Public Function EnregistreFichierBouee(ByVal Fichier As String, ByVal Nom As String) As Boolean

            Dim buoy As CBouee

            ' Mise du flag par default
            EnregistreFichierBouee = False

            ' Verification que la bouee existes
            If Bouees.ContainsKey(Nom) Then
                buoy = Bouees(Nom)
                Return WriteFichierBouee(Fichier, buoy)
            End If

        End Function

        Public Function EnregistreFichierBouee(ByVal Fichier As String, ByVal buoy As CBouee) As Boolean

            ' Mise du flag par default
            EnregistreFichierBouee = False

            ' Verification que la bouee existes
            If buoy IsNot Nothing Then
                Return WriteFichierBouee(Fichier, buoy)
            End If

        End Function

        Public Function CryptEnregistreBouee(ByVal Fichier As String, ByVal Nom As String) As Boolean

            Dim buoy As CBouee

            ' Mise du flag par default
            CryptEnregistreBouee = False

            ' Verification que la bouee existes
            If Bouees.ContainsKey(Nom) Then
                buoy = Bouees(Nom)
                Return CryptWriteFichierBouee(Fichier, buoy)
            End If

        End Function

        Public Function CryptEnregistreBouee(ByVal Fichier As String, ByVal buoy As CBouee) As Boolean

            ' Mise du flag par default
            CryptEnregistreBouee = False

            ' Verification que la bouee existes
            If buoy IsNot Nothing Then
                Return CryptWriteFichierBouee(Fichier, buoy)
            End If

        End Function

        Private Function getVersion() As Version

            Dim version As String = System.Reflection.Assembly.GetExecutingAssembly().FullName
            Dim start As Integer = version.IndexOf("Version=") + 8
            Dim length As Integer = version.IndexOf(",", start) - start

            Return New Version(version.Substring(start, length))

        End Function

#Region "Lecture du fichier Base de données"

        Private Function LectureFichierBouee(ByVal BaseXml As XmlReader) As CBouee

            Dim VersionSW As Version = getVersion()
            Dim Version As Version = Nothing

            LectureFichierBouee = New CBouee

            ' Lecture du fichier avec les bases
            While BaseXml.Read()
                Select Case BaseXml.NodeType
                    Case XmlNodeType.Element, XmlNodeType.EndElement
                        ' Lecture de la version de la base
                        If BaseXml.Name = "Calmar" Then
                            BaseXml.MoveToFirstAttribute()
                            If BaseXml.Name = "version" Then
                                Version = New Version(BaseXml.Value)
                            End If
                        Else
                            If Version = VersionSW Then
                                If BaseXml.Name = "Buoy" Then
                                    LectureFichierBouee = LectureBouee(BaseXml)
                                End If
                            Else
                                Throw New Exception("Version differentes")
                            End If
                        End If

                End Select
            End While

        End Function

        Private Function LectureBouee(ByVal BaseXml As XmlReader) As CBouee

            Dim lst As CBouee.CLest = Nothing
            Dim pyl As CPylone
            Dim equ As CEquipement

            ' Deplacement sur le premier attribut
            BaseXml.MoveToFirstAttribute()

            ' Declaration d'une structure
            LectureBouee = New CBouee()

            ' Recuperation du nom de la flotteur
            LectureBouee.Nom = BaseXml.Value

            ' Recuperation de la ChaineMin
            BaseXml.MoveToNextAttribute()
            If BaseXml.Name = "ChaineMin" Then LectureBouee.ChaineMin = Double.Parse(BaseXml.Value, EnglishFormat)

            ' Recuperation de la ChaineMax
            BaseXml.MoveToNextAttribute()
            If BaseXml.Name = "ChaineMax" Then LectureBouee.ChaineMax = Double.Parse(BaseXml.Value, EnglishFormat)

            ' Recuperation de la Masse lest unitaire
            BaseXml.MoveToNextAttribute()
            If BaseXml.Name = "MasseLestUnitaire" Then LectureBouee.MasseLestUnitaire = Double.Parse(BaseXml.Value, EnglishFormat)

            ' Recuperation de la NombreLestMin
            BaseXml.MoveToNextAttribute()
            If BaseXml.Name = "NombreLestMin" Then LectureBouee.NombreLestMin = Double.Parse(BaseXml.Value, EnglishFormat)

            ' Recuperation de la NombreLestMax
            BaseXml.MoveToNextAttribute()
            If BaseXml.Name = "NombreLestMax" Then LectureBouee.NombreLestMax = Double.Parse(BaseXml.Value, EnglishFormat)

            ' Recuperation de la NombreLestMax
            BaseXml.MoveToNextAttribute()
            If BaseXml.Name = "Commentaire" Then LectureBouee.Commentaire = BaseXml.Value

            ' Lecture des Elements de la bouee
            While BaseXml.Read()

                If BaseXml.NodeType = XmlNodeType.Element Then
                    If BaseXml.Name = "Structure" Then LectureStructureElement(LectureBouee.StructureBouee, BaseXml)
                    If BaseXml.Name = "Flotteur" Then LectureFloatElement(LectureBouee.FlotteurBouee, BaseXml)
                    If BaseXml.Name = "Pylone" Then
                        pyl = New CPylone
                        LecturePyloneElement(pyl, BaseXml)
                        LectureBouee.NouveauPyloneBouee = pyl
                    End If
                    If BaseXml.Name = "Equipement" Then
                        equ = New CEquipement
                        LectureEquipementElement(equ, BaseXml)
                        LectureBouee.NouvelleEquipementBouee = equ
                    End If
                End If

                If BaseXml.NodeType = XmlNodeType.EndElement Then
                    ' Fin de la recuperation de la bouée
                    If BaseXml.Name = "Buoy" Then
                        Exit While
                    End If
                End If

            End While

        End Function

        Private Sub LectureStructureElement(ByRef NouveauElement As CStructure, ByVal BaseXml As XmlReader)

            ' Deplacement sur le premier attribut
            BaseXml.MoveToFirstAttribute()

            ' Declaration d'une structure
            NouveauElement = New CStructure

            ' Recuperation du nom de la structure
            NouveauElement.Nom = BaseXml.Value
            ' Recuperation de la masse
            BaseXml.MoveToNextAttribute()
            NouveauElement.Masse = Double.Parse(BaseXml.Value, EnglishFormat)
            ' Recuperation de la offset
            BaseXml.MoveToNextAttribute()
            NouveauElement.OffsetFlotteur = Double.Parse(BaseXml.Value, EnglishFormat)
            ' Recuperation de la offset
            BaseXml.MoveToNextAttribute()
            NouveauElement.OffsetOrganeau = Double.Parse(BaseXml.Value, EnglishFormat)

            ' Recuperation des dimensions de la structure
            LectureStructureDimElementsVersionActuelle(NouveauElement, BaseXml)

        End Sub

        Private Sub LectureStructureDimElementsVersionActuelle(ByRef NouveauElement As CStructure, ByVal BaseXml As XmlReader)

            While BaseXml.Read()
                If BaseXml.NodeType = XmlNodeType.Element AndAlso BaseXml.Name = "ElementDimItem" Then
                    NouveauElement.AddElement(NouvelleDimElement(BaseXml, NouveauElement))
                ElseIf BaseXml.NodeType = XmlNodeType.EndElement Then
                    Exit While
                End If
            End While

        End Sub

        Private Sub LectureFloatElement(ByRef NouveauElement As CFlotteur, ByVal BaseXml As XmlReader)

            ' Deplacement sur le premier attribut
            BaseXml.MoveToFirstAttribute()

            ' Declaration d'une structure
            NouveauElement = New CFlotteur

            ' Recuperation du nom de la flotteur
            NouveauElement.Nom = BaseXml.Value
            ' Recuperation de la masse
            BaseXml.MoveToNextAttribute()
            NouveauElement.Masse = Double.Parse(BaseXml.Value, EnglishFormat)

            ' Recuperation des dimensions de la structure
            LectureFloatDimElementsVersionActuelle(NouveauElement, BaseXml)

        End Sub

        Private Sub LectureFloatDimElementsVersionActuelle(ByRef NouveauElement As CFlotteur, ByVal BaseXml As XmlReader)

            While BaseXml.Read()
                If BaseXml.NodeType = XmlNodeType.Element AndAlso BaseXml.Name = "ElementDimItem" Then
                    NouveauElement.AddElement(NouvelleDimElement(BaseXml, NouveauElement))
                ElseIf BaseXml.NodeType = XmlNodeType.EndElement Then
                    Exit While
                End If
            End While

        End Sub

        Private Function NouvelleDimElement(ByVal BaseXml As XmlReader, ByRef parent As Object) As CDimensionElementTroncCone

            BaseXml.MoveToFirstAttribute()

            NouvelleDimElement = New CDimensionElementTroncCone()
            ' Recuperation de la hauteur
            NouvelleDimElement.HauteurElement = Double.Parse(BaseXml.Value, EnglishFormat)
            ' Recuperation de la longueur 0
            BaseXml.MoveToNextAttribute()
            NouvelleDimElement.DiameterLow = Double.Parse(BaseXml.Value, EnglishFormat)
            ' Recuperation de la longueur 1
            BaseXml.MoveToNextAttribute()
            NouvelleDimElement.DiameterHigh = Double.Parse(BaseXml.Value, EnglishFormat)
            ' Recuperation de la longueur interieur
            BaseXml.MoveToNextAttribute()
            NouvelleDimElement.DiameterInter = Double.Parse(BaseXml.Value, EnglishFormat)
            ' Recuperation du volume
            BaseXml.MoveToNextAttribute()
            NouvelleDimElement.VolumeReel = Double.Parse(BaseXml.Value, EnglishFormat)

        End Function

        Private Sub LecturePyloneElement(ByRef NouveauElement As CPylone, ByVal BaseXml As XmlReader)

            Dim NouvelleDimElement As CDimensionElement

            NouveauElement = New CPylone
            NouvelleDimElement = New CDimensionElement()

            ' Deplacement sur le premier attribut
            BaseXml.MoveToFirstAttribute()
            ' Recuperation du nom de la pylone
            NouveauElement.Nom = BaseXml.Value

            ' Recuperation du Hauteur
            BaseXml.MoveToNextAttribute()
            NouvelleDimElement.HauteurElement = Double.Parse(BaseXml.Value, EnglishFormat)
            ' Recuperation de la longueur haute
            BaseXml.MoveToNextAttribute()
            NouvelleDimElement.LongueurHautElement = Double.Parse(BaseXml.Value, EnglishFormat)
            ' Recuperation de la longueur basse
            BaseXml.MoveToNextAttribute()
            NouvelleDimElement.LongueurBasElement = Double.Parse(BaseXml.Value, EnglishFormat)

            ' Recuperation de la masse
            BaseXml.MoveToNextAttribute()
            NouveauElement.Masse = Double.Parse(BaseXml.Value, EnglishFormat)

            ' Mise en place des valeurs dimensionnelles
            NouveauElement.Element = NouvelleDimElement

        End Sub

        Private Sub LectureEquipementElement(ByRef NouveauElement As CEquipement, ByVal BaseXml As XmlReader)

            Dim NouvelleDimElement As CDimensionElement

            NouveauElement = New CEquipement
            NouvelleDimElement = New CDimensionElement()

            ' Deplacement sur le premier attribut
            BaseXml.MoveToFirstAttribute()
            ' Recuperation du nom de la pylone
            NouveauElement.Nom = BaseXml.Value

            ' Recuperation du Hauteur
            BaseXml.MoveToNextAttribute()
            NouvelleDimElement.HauteurElement = Double.Parse(BaseXml.Value, EnglishFormat)
            ' Recuperation de la longueur haute
            BaseXml.MoveToNextAttribute()
            NouvelleDimElement.LongueurHautElement = Double.Parse(BaseXml.Value, EnglishFormat)
            ' Recuperation de la longueur basse
            BaseXml.MoveToNextAttribute()
            NouvelleDimElement.LongueurBasElement = Double.Parse(BaseXml.Value, EnglishFormat)

            ' Recuperation de la masse
            BaseXml.MoveToNextAttribute()
            NouveauElement.Masse = Double.Parse(BaseXml.Value, EnglishFormat)

            ' Mise en place des valeurs dimensionnelles
            NouveauElement.Element = NouvelleDimElement

        End Sub

        Private Sub LectureChaineElements(ByRef dico As List(Of CChaine), ByVal BaseXml As XmlReader)

            Dim updateChaine As Boolean = False
            Dim NouveauElement As CChaine = Nothing

            While BaseXml.Read()

                If BaseXml.NodeType = XmlNodeType.Element Then
                    If BaseXml.Name = "ChainElementItem" Then
                        LectureChaineElement(NouveauElement, BaseXml)
                        updateChaine = True
                        For Each chn In dico
                            If chn.DN = NouveauElement.DN And chn.TYPE = NouveauElement.TYPE And chn.MASSE_LINEIQUE = NouveauElement.MASSE_LINEIQUE Then
                                updateChaine = False
                                Exit For
                            End If
                        Next
                        If updateChaine Then dico.Add(NouveauElement)
                    End If
                End If

                If BaseXml.NodeType = XmlNodeType.EndElement Then
                    If BaseXml.Name = "Chain" Then Exit While
                End If

            End While

        End Sub

        Private Sub LectureChaineElement(ByRef NouveauElement As CChaine, ByVal BaseXml As XmlReader)

            ' Deplacement sur le premier attribut
            BaseXml.MoveToFirstAttribute()

            ' Declaration d'une structure
            NouveauElement = New CChaine

            ' Recuperation du nom de la flotteur
            NouveauElement.TYPE = BaseXml.Value.Replace(",", ".")
            ' Recuperation du volume
            BaseXml.MoveToNextAttribute()
            NouveauElement.DN = Double.Parse(BaseXml.Value, EnglishFormat)
            ' Recuperation de la masse
            BaseXml.MoveToNextAttribute()
            NouveauElement.MASSE_LINEIQUE = Double.Parse(BaseXml.Value, EnglishFormat)
            ' Recuperation de la charge d'epreuve pour la qualité 1
            BaseXml.MoveToNextAttribute()
            NouveauElement.CHARGE_EPREUVE_QL1 = Double.Parse(BaseXml.Value, EnglishFormat)
            ' Recuperation de la charge d'epreuve pour la qualité 2
            BaseXml.MoveToNextAttribute()
            NouveauElement.CHARGE_EPREUVE_QL2 = Double.Parse(BaseXml.Value, EnglishFormat)
            ' Recuperation de la charge d'epreuve pour la qualité 3
            BaseXml.MoveToNextAttribute()
            NouveauElement.CHARGE_EPREUVE_QL3 = Double.Parse(BaseXml.Value, EnglishFormat)

        End Sub

#End Region

#Region "Ecriture du fichier Base de données"

        Private Function WriteFichierBouee(ByVal fichier As String, ByVal buoy As CBouee) As Boolean

            Try
                IO.File.WriteAllText(fichier, WriteXmlBouee(buoy))
            Catch ex As Exception
                Return False
            End Try

            Return True

        End Function

        Private Function CryptWriteFichierBouee(ByVal fichier As String, ByVal buoy As CBouee) As Boolean

            Try
                Dim ret As String = AES_Encrypt(WriteXmlBouee(buoy))
                If ret = "" Then Return False

                IO.File.WriteAllText(fichier, ret)
            Catch ex As Exception
                Return False
            End Try

            Return True

        End Function

        Private Function WriteXmlBouee(ByVal Buoy As CBouee) As String

            ' Lecture des informations de structure
            WriteXmlBouee = "Calmar"

            WriteXmlBouee = "<?xml version=""1.0"" encoding=""UTF-8"" ?>" + vbCrLf
            WriteXmlBouee += "<Calmar version=""" + getVersion().ToString + """>" + vbCrLf

            '
            ' ENREGISTREMENT DE LA BOUEE
            '
            WriteXmlBouee += vbTab + "<Buoy Name=""" + Buoy.Nom + """ ChaineMin=""" + Buoy.ChaineMin.ToString.Replace(",", ".") + """ ChaineMax=""" + Buoy.ChaineMax.ToString.Replace(",", ".") + """ MasseLestUnitaire=""" + Buoy.MasseLestUnitaire.ToString.Replace(",", ".") + """ NombreLestMin=""" + Buoy.NombreLestMin.ToString.Replace(",", ".") + """ NombreLestMax=""" + Buoy.NombreLestMax.ToString.Replace(",", ".") + """ Commentaire=""" + Buoy.Commentaire + """>" + vbCrLf

            If Buoy.StructureBouee IsNot Nothing Then
                With Buoy.StructureBouee
                    WriteXmlBouee += vbTab + vbTab + "<Structure Name=""" + .Nom + """ Masse=""" + .Masse.ToString.Replace(",", ".") + """ OffsetFlotteur=""" + .OffsetFlotteur.ToString.Replace(",", ".") + """ OffsetOrganeau=""" + .OffsetOrganeau.ToString.Replace(",", ".") + """>" + vbCrLf
                    For Each fldim In .Elements
                        WriteXmlBouee += vbTab + vbTab + vbTab + "<ElementDimItem H=""" + fldim.HauteurElement.ToString.Replace(",", ".") + """ D0=""" + fldim.DiameterLow.ToString.Replace(",", ".") + """ D1=""" + fldim.DiameterHigh.ToString.Replace(",", ".") + """ DI=""" + fldim.DiameterInter.ToString.Replace(",", ".") + """ Volume=""" + fldim.VolumeReel.ToString.Replace(",", ".") + """ />" + vbCrLf
                    Next
                    WriteXmlBouee += vbTab + vbTab + "</Structure>" + vbCrLf
                End With
            End If

            If Buoy.FlotteurBouee IsNot Nothing Then
                With Buoy.FlotteurBouee
                    WriteXmlBouee += vbTab + vbTab + "<Flotteur Name=""" + .Nom + """ Masse=""" + .Masse.ToString.Replace(",", ".") + """>" + vbCrLf
                    For Each fldim In .Elements
                        WriteXmlBouee += vbTab + vbTab + vbTab + "<ElementDimItem H=""" + fldim.HauteurElement.ToString.Replace(",", ".") + """ D0=""" + fldim.DiameterLow.ToString.Replace(",", ".") + """ D1=""" + fldim.DiameterHigh.ToString.Replace(",", ".") + """ DI=""" + fldim.DiameterInter.ToString.Replace(",", ".") + """ Volume=""" + fldim.VolumeReel.ToString.Replace(",", ".") + """ />" + vbCrLf
                    Next
                    WriteXmlBouee += vbTab + vbTab + "</Flotteur>" + vbCrLf
                End With
            End If

            If Buoy.PyloneBouee IsNot Nothing Then
                For Each st In Buoy.PyloneBouee
                    WriteXmlBouee += vbTab + vbTab + "<Pylone Name=""" + st.Nom + """ Height=""" + st.Element.HauteurElement.ToString.Replace(",", ".") + """ WidthHigh=""" + st.Element.LongueurHautElement.ToString.Replace(",", ".") + """ WidthLow=""" + st.Element.LongueurBasElement.ToString.Replace(",", ".") + """ Masse=""" + st.Masse.ToString.Replace(",", ".") + """ />" + vbCrLf
                Next
            End If
            If Buoy.EquipementBouee IsNot Nothing Then
                For Each st In Buoy.EquipementBouee
                    WriteXmlBouee += vbTab + vbTab + "<Equipement Name=""" + st.Nom + """ Height=""" + st.Element.HauteurElement.ToString.Replace(",", ".") + """ WidthHigh=""" + st.Element.LongueurHautElement.ToString.Replace(",", ".") + """ WidthLow=""" + st.Element.LongueurBasElement.ToString.Replace(",", ".") + """ Masse=""" + st.Masse.ToString.Replace(",", ".") + """ />" + vbCrLf
                Next
            End If

            WriteXmlBouee += vbTab + "</Buoy>" + vbCrLf
            WriteXmlBouee += "</Calmar>"

        End Function

#End Region

    End Class

    <XmlRoot("Calmar")>
    Public Class CFileCalmar

        Private _buoy As CBuoyItem
        Private _version As String

        <XmlAttribute("version")>
        Property version As String
            Get
                Return _version
            End Get
            Set(value As String)
                _version = value
            End Set
        End Property

        <XmlElement("Buoy")>
        Property Buoy As CBuoyItem
            Get
                Return _buoy
            End Get
            Set(value As CBuoyItem)
                _buoy = value
            End Set
        End Property

        Public Class CBuoyItem

            Private _name As String
            Private _chaine_min As Integer
            Private _chaine_max As Integer
            Private _masse_lest_unitaire As Integer
            Private _nombre_lest_min As Integer
            Private _nombre_lest_max As Integer

            <XmlAttribute("Name")>
            Property name As String
                Get
                    Return _name
                End Get
                Set(value As String)
                    _name = value
                End Set
            End Property

            <XmlAttribute("ChaineMin")>
            Property Chaine_min As Integer
                Get
                    Return _chaine_min
                End Get
                Set(value As Integer)
                    _chaine_min = value
                End Set
            End Property

            <XmlAttribute("ChaineMax")>
            Property Chaine_max As Integer
                Get
                    Return _chaine_max
                End Get
                Set(value As Integer)
                    _chaine_max = value
                End Set
            End Property

            <XmlAttribute("MasseLestUnitaire")>
            Property MasseLestUnitaire As Integer
                Get
                    Return _masse_lest_unitaire
                End Get
                Set(value As Integer)
                    _masse_lest_unitaire = value
                End Set
            End Property

            <XmlAttribute("NombreLestMin")>
            Property NombreLestMin As Integer
                Get
                    Return _nombre_lest_min
                End Get
                Set(value As Integer)
                    _nombre_lest_min = value
                End Set
            End Property

            <XmlAttribute("NombreLestMax")>
            Property NombreLestMax As Integer
                Get
                    Return _nombre_lest_max
                End Get
                Set(value As Integer)
                    _nombre_lest_max = value
                End Set
            End Property

        End Class

    End Class

End Namespace