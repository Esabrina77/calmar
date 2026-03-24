Imports System.IO
Imports System.Xml

Namespace Buoy
    Public Module MLectureBouee

        Private EnglishFormat As New System.Globalization.CultureInfo("en")

#Region "Lecture du fichier Base de données"

        Public Function LectureFichierBouee(ByVal BaseXml As XmlReader) As CBouee

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
                            If BaseXml.Name = "Buoy" Then
                                LectureFichierBouee = LectureBouee(BaseXml)
                            End If
                        End If
                End Select
            End While

        End Function

        Public Function LectureBouee(ByVal BaseXml As XmlReader) As CBouee

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

        Public Sub LectureStructureElement(ByRef NouveauElement As CStructure, ByVal BaseXml As XmlReader)

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

        Public Sub LectureStructureDimElementsVersionActuelle(ByRef NouveauElement As CStructure, ByVal BaseXml As XmlReader)

            While BaseXml.Read()
                If BaseXml.NodeType = XmlNodeType.Element AndAlso BaseXml.Name = "ElementDimItem" Then
                    NouveauElement.AddElement(NouvelleDimElement(BaseXml, NouveauElement))
                ElseIf BaseXml.NodeType = XmlNodeType.EndElement Then
                    Exit While
                End If
            End While

        End Sub

        Public Sub LectureFloatElement(ByRef NouveauElement As CFlotteur, ByVal BaseXml As XmlReader)

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

        Public Sub LectureFloatDimElementsVersionActuelle(ByRef NouveauElement As CFlotteur, ByVal BaseXml As XmlReader)

            While BaseXml.Read()
                If BaseXml.NodeType = XmlNodeType.Element AndAlso BaseXml.Name = "ElementDimItem" Then
                    NouveauElement.AddElement(NouvelleDimElement(BaseXml, NouveauElement))
                ElseIf BaseXml.NodeType = XmlNodeType.EndElement Then
                    Exit While
                End If
            End While

        End Sub

        Public Function NouvelleDimElement(ByVal BaseXml As XmlReader, ByRef parent As Object) As CDimensionElementTroncCone

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

        Public Sub LecturePyloneElement(ByRef NouveauElement As CPylone, ByVal BaseXml As XmlReader)

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

        Public Sub LectureEquipementElement(ByRef NouveauElement As CEquipement, ByVal BaseXml As XmlReader)

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

        Public Sub LectureChaineElements(ByRef dico As List(Of CChaine), ByVal BaseXml As XmlReader)

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

        Public Sub LectureChaineElement(ByRef NouveauElement As CChaine, ByVal BaseXml As XmlReader)

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

        Public Sub LectureEquipementSupplementaireElements(ByRef dico As List(Of CEquipementSupplementaire), ByVal BaseXml As XmlReader)

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

        Public Sub LectureEquipementSupplementaireElement(ByRef NouveauElement As CEquipementSupplementaire, ByVal BaseXml As XmlReader)

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

    End Module

End Namespace