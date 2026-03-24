Imports System.IO
Imports System.Xml

Namespace Buoy

    Public Class CBouee

        Private _File As String

        Public _Nom As String
        Public _Commentaire As String

        Private _Protege As Boolean = False

        Private sStructure As New CStructure
        Private sFlotteur As New CFlotteur
        Private sPylone As New List(Of CPylone)
        Private sEquipement As New List(Of CEquipement)

        Private _ChaineMin As Double
        Private _ChaineMax As Double

        Private _NombreLestMin As Double
        Private _NombreLestMax As Double
        Private _MasseLestUnitaire As Double

        Private LargeurMax As Double
        Private HauteurMax As Double
        Private RatioScreen As Double

        Private _Size As Size

        Public Sub New()
            Nom = ""
        End Sub

        Public Function Clone() As CBouee

            Clone = New CBouee

            Clone.Nom = Nom
            Clone.Commentaire = Commentaire
            Clone.Protege = Protege
            Clone.ChaineMin = ChaineMin
            Clone.ChaineMax = ChaineMax

            Clone.NombreLestMin = NombreLestMin
            Clone.NombreLestMax = NombreLestMax
            Clone.MasseLestUnitaire = MasseLestUnitaire

            If StructureBouee IsNot Nothing Then _
                Clone.StructureBouee = StructureBouee.Clone

            If FlotteurBouee IsNot Nothing Then _
                Clone.FlotteurBouee = FlotteurBouee.Clone

            If PyloneBouee IsNot Nothing Then
                For Each pyl In PyloneBouee
                    Clone.NouveauPyloneBouee = pyl.Clone
                Next
            End If

            If EquipementBouee IsNot Nothing Then
                For Each equ In EquipementBouee
                    Clone.NouvelleEquipementBouee = equ.Clone
                Next
            End If

        End Function

        Public Function IsEqual(ByVal CompBouee As CBouee) As Boolean

            Dim p0 As Integer

            If CompBouee.Nom <> Nom Then Return False
            If CompBouee.ChaineMin <> ChaineMin Then Return False
            If CompBouee.ChaineMax <> ChaineMax Then Return False
            If CompBouee.NombreLestMax <> NombreLestMax Then Return False
            If CompBouee.NombreLestMin <> NombreLestMin Then Return False
            If CompBouee.MasseLestUnitaire <> MasseLestUnitaire Then Return False

            If Not CompBouee.StructureBouee.IsEqual(StructureBouee) Then Return False
            If Not CompBouee.FlotteurBouee.IsEqual(FlotteurBouee) Then Return False

            If CompBouee.PyloneBouee.Count <> PyloneBouee.Count Then Return False
            For p0 = 0 To CompBouee.PyloneBouee.Count - 1
                If Not CompBouee.PyloneBouee(p0).IsEqual(PyloneBouee(p0)) Then Return False
            Next

            If CompBouee.EquipementBouee.Count <> EquipementBouee.Count Then Return False
            For p0 = 0 To CompBouee.EquipementBouee.Count - 1
                If Not CompBouee.EquipementBouee(p0).IsEqual(EquipementBouee(p0)) Then Return False
            Next

            Return True

        End Function

#Region "Propriete de la classe"

        Property File As String
            Get
                Return _File
            End Get
            Set(ByVal value As String)
                _File = value
            End Set
        End Property

        ReadOnly Property FileP As String
            Get
                Return MCrypt.AES_Encrypt(_File)
            End Get
        End Property

        Property Commentaire As String
            Get
                Return _Commentaire
            End Get
            Set(ByVal value As String)
                _Commentaire = value
            End Set
        End Property
        Property Nom As String
            Get
                Return _Nom
            End Get
            Set(ByVal value As String)
                _Nom = value
            End Set
        End Property

        Property Protege As Boolean
            Get
                Return _Protege
            End Get
            Set(value As Boolean)
                _Protege = value
            End Set
        End Property

        Property ChaineMin As Double
            Get
                Return _ChaineMin
            End Get
            Set(value As Double)
                _ChaineMin = value
            End Set
        End Property

        Property ChaineMax As Double
            Get
                Return _ChaineMax
            End Get
            Set(value As Double)
                _ChaineMax = value
            End Set
        End Property

        Property NombreLestMin As Double
            Get
                Return _NombreLestMin
            End Get
            Set(value As Double)
                _NombreLestMin = value
            End Set
        End Property

        Property NombreLestMax As Double
            Get
                Return _NombreLestMax
            End Get
            Set(value As Double)
                _NombreLestMax = value
            End Set
        End Property

        Property MasseLestUnitaire As Double
            Get
                Return _MasseLestUnitaire
            End Get
            Set(value As Double)
                _MasseLestUnitaire = value
            End Set
        End Property

#End Region

#Region "Gestion de l'ajout, suppression de la structure"

        Property StructureBouee As CStructure
            Get
                Return sStructure
            End Get
            Set(value As CStructure)
                sStructure = value
                MakeSizingMax()
            End Set
        End Property

#End Region

#Region "Gestion de l'ajout, suppression du flotteur"

        Property FlotteurBouee As CFlotteur
            Get
                Return sFlotteur
            End Get
            Set(value As CFlotteur)
                sFlotteur = value
                MakeSizingMax()
            End Set
        End Property

#End Region

#Region "Gestion de l'ajout, suppression des pylones"

        ReadOnly Property PyloneBouee As CPylone()
            Get
                Return sPylone.ToArray
            End Get
        End Property

        Public Sub ClearPylone()

            sPylone.Clear()
            MakeSizingMax()

        End Sub

        WriteOnly Property NouveauPyloneBouee As CPylone
            Set(value As CPylone)
                sPylone.Add(value)
                MakeSizingMax()
            End Set
        End Property

        Public Sub MoveUpPylone(ByVal iPylone As Integer)

            Dim TmpNom As CPylone = sPylone(iPylone)

            ' Suppression de l'index
            sPylone.RemoveAt(iPylone)
            sPylone.Insert(iPylone + 1, TmpNom)

            MakeSizingMax()

        End Sub

        Public Sub MoveDownPylone(ByVal iPylone As Integer)

            Dim TmpNom As CPylone = sPylone(iPylone)

            ' Suppression de l'index
            sPylone.RemoveAt(iPylone)
            sPylone.Insert(iPylone - 1, TmpNom)

            MakeSizingMax()

        End Sub

        Public Sub ModifyPylone(ByVal iEquipement As Integer, ByVal PyloneMod As CPylone)

            ' Suppression de l'index
            sPylone.RemoveAt(iEquipement)
            sPylone.Insert(iEquipement, PyloneMod)

            MakeSizingMax()

        End Sub

        Public Sub ModifyNomPylone(ByVal OldNom As String, ByVal NewNom As String)

            Dim p0 As Integer

            For p0 = 0 To sPylone.Count - 1
                If sPylone(p0).Nom = OldNom Then
                    If NewNom = "" Then
                        sPylone.RemoveAt(p0)
                    Else
                        sPylone(p0).Nom = NewNom
                    End If
                End If
            Next

            MakeSizingMax()

        End Sub

        Public Sub RemovePylone(ByVal iPylone As Integer)

            If iPylone < 0 Or iPylone > sPylone.Count - 1 Then Return

            sPylone.RemoveAt(iPylone)
            MakeSizingMax()

        End Sub

#End Region

#Region "Gestion de l'ajout, suppression des equipements"

        Public Sub ClearEquipement()

            sEquipement.Clear()
            MakeSizingMax()

        End Sub

        ReadOnly Property EquipementBouee As CEquipement()
            Get
                Return sEquipement.ToArray
            End Get
        End Property

        WriteOnly Property NouvelleEquipementBouee As CEquipement
            Set(value As CEquipement)
                sEquipement.Add(value)
                MakeSizingMax()
            End Set
        End Property

        Public Sub MoveUpEquipement(ByVal iEquipement As Integer)

            Dim TmpNom As CEquipement = sEquipement(iEquipement)

            ' Suppression de l'index
            sEquipement.RemoveAt(iEquipement)
            sEquipement.Insert(iEquipement + 1, TmpNom)

            MakeSizingMax()

        End Sub

        Public Sub MoveDownEquipement(ByVal iEquipement As Integer)

            Dim TmpNom As CEquipement = sEquipement(iEquipement)

            ' Suppression de l'index
            sEquipement.RemoveAt(iEquipement)
            sEquipement.Insert(iEquipement - 1, TmpNom)

            MakeSizingMax()

        End Sub

        Public Sub ModifyEquipement(ByVal iEquipement As Integer, ByVal EquipementMod As CEquipement)

            ' Suppression de l'index
            sEquipement.RemoveAt(iEquipement)
            sEquipement.Insert(iEquipement, EquipementMod)

            MakeSizingMax()

        End Sub

        Public Sub ModifyNomEquipement(ByVal OldNom As String, ByVal NewNom As String)

            Dim p0 As Integer

            For p0 = 0 To sEquipement.Count - 1
                If sEquipement(p0).Nom = OldNom Then
                    If NewNom = "" Then
                        sEquipement.RemoveAt(p0)
                    Else
                        sEquipement(p0).Nom = NewNom
                    End If
                End If
            Next

            MakeSizingMax()

        End Sub

        Public Sub RemoveEquipement(ByVal iEquipement As Integer)

            If iEquipement < 0 Or iEquipement > sEquipement.Count - 1 Then Return

            sEquipement.RemoveAt(iEquipement)
            MakeSizingMax()

        End Sub

#End Region

#Region "Gestion de l'ajout, suppression des lest"

        ReadOnly Property LestBouee As CLest()
            Get
                Dim Cl As New List(Of CLest)

                For p0 = 0 To (NombreLestMax - NombreLestMin)
                    If NombreLestMin = 0 And p0 = 0 Then Continue For
                    Cl.Add(New CLest(NombreLestMin + p0, MasseLestUnitaire))
                Next

                Return Cl.ToArray
            End Get
        End Property

        Public Class CLest

            Private EnglishFormat As New Globalization.CultureInfo("en-US")

            Public _NombreLest As Integer
            Public _PoidsLest As Double

            Public Sub New(ByVal NombreLest As Integer, ByVal PoidsLest As Double)

                _NombreLest = NombreLest
                _PoidsLest = PoidsLest

            End Sub

            Public Sub New(ByVal Parametre As String)

                Dim SplitParam() As String

                _NombreLest = 0
                _PoidsLest = 0

                If Parametre.Trim = "" Then Return
                SplitParam = Parametre.Split(" kg")

                If SplitParam.Length = 2 Then
                    SplitParam = SplitParam(0).Split("x")
                    If SplitParam.Length = 2 Then
                        _NombreLest = Integer.Parse(SplitParam(0))
                        _PoidsLest = Double.Parse(SplitParam(1), Globalization.NumberStyles.Any, EnglishFormat)
                    End If
                End If

            End Sub

            Public Overrides Function ToString() As String

                If _NombreLest = 0 And _PoidsLest = 0 Then Return ""

                Return _NombreLest.ToString() + "x" + _PoidsLest.ToString(EnglishFormat) + " kg"

            End Function

            Public Function Clone() As CLest

                Clone = New CLest(_NombreLest, _PoidsLest)

            End Function

        End Class

#End Region

#Region "Gestion du dessin de la bouee"

        Private Sub MakeSizingMax()

            HauteurMax = 0
            LargeurMax = 0

            ' Hauteur de la structure
            ' Recuperation de toutes les tailles
            If StructureBouee IsNot Nothing Then
                For Each el In StructureBouee.Elements
                    HauteurMax += el.HauteurElement
                    If el.LongueurMaxElement > LargeurMax Then LargeurMax = el.LongueurMaxElement
                Next
                ' Deduction de la structure
                HauteurMax -= StructureBouee.OffsetFlotteur
            End If

            ' Hauteur de la flotteur
            ' Recuperation de toutes les tailles
            If FlotteurBouee IsNot Nothing Then
                For Each el In FlotteurBouee.Elements
                    HauteurMax += el.HauteurElement
                    If el.LongueurMaxElement > LargeurMax Then LargeurMax = el.LongueurMaxElement
                Next
            End If

            '' Hauteur de la pylone
            '' Recuperation de toutes les tailles
            If PyloneBouee IsNot Nothing Then
                For Each el In PyloneBouee
                    HauteurMax += el.Element.HauteurElement
                    If el.Element.LongueurMaxElement > LargeurMax Then LargeurMax = el.Element.LongueurMaxElement
                Next
            End If

            '' Recuperation de toutes les tailles
            If EquipementBouee IsNot Nothing Then
                For Each el In EquipementBouee
                    HauteurMax += el.Element.HauteurElement
                    If el.Element.LongueurMaxElement > LargeurMax Then LargeurMax = el.Element.LongueurMaxElement
                Next
            End If

            RatioScreen = LargeurMax / HauteurMax

        End Sub

        Property Width() As Double
            Get
                Return _Size.Width
            End Get
            Set(value As Double)
                _Size.Width = value
            End Set
        End Property

        Property Height() As Double
            Get
                Return _Size.Height
            End Get
            Set(value As Double)
                _Size.Height = value
            End Set
        End Property

        ReadOnly Property BuoyCanvas() As Canvas
            Get
                Return DrawCanvas(New Canvas())
            End Get
        End Property

        Private Sub MeSizeChanged(sender As Object, e As System.Windows.SizeChangedEventArgs)
            _Size = e.NewSize
            DrawCanvas(sender)
        End Sub

        Private Function DrawCanvas(ByRef sender As Canvas) As Canvas

            Dim Cnvs As Canvas
            Dim MyHauteurMax As Double = 0 '_Size.Height

            sender.Children.Clear()

            If _Size.Width = 0 Or _Size.Height = 0 Then
                _Size.Height = 80
                _Size.Width = _Size.Height * RatioScreen
            End If

            sender.Width = _Size.Width
            sender.Height = _Size.Height

            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            '''' MISE EN PLACE DE LA STRUCTURE
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            If StructureBouee IsNot Nothing Then
                With StructureBouee
                    Cnvs = .DrawCanvas(New Size(_Size.Width * (.LargeurMax / LargeurMax), _Size.Height * (.HauteurMax / HauteurMax)))
                    ' Mise en place de la position de la structure
                    MyHauteurMax += Cnvs.Height
                    Cnvs.SetValue(Canvas.TopProperty, _Size.Height - MyHauteurMax)
                    ' Deduction de l'offset
                    MyHauteurMax -= _Size.Height * (.OffsetFlotteur / HauteurMax)
                    ' Verification quel est la longueur a prendre
                    Cnvs.SetValue(Canvas.LeftProperty, (_Size.Width - Cnvs.Width) / 2)
                    sender.Children.Add(Cnvs)
                End With
            End If
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            '''' FIN DE LA MISE EN PLACE DE LA STRUCTURE
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            '''' MISE EN PLACE DU FLOTTEUR
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            If FlotteurBouee IsNot Nothing Then
                With FlotteurBouee
                    Cnvs = .DrawCanvas(New Size(_Size.Width * (.LargeurMax / LargeurMax), _Size.Height * (.HauteurMax / HauteurMax)))
                    'Cnvs.Width = _Size.Width * (FlotteurBouee.LargeurMax / LargeurMax)
                    'Cnvs.Height = _Size.Height * (FlotteurBouee.HauteurMax / HauteurMax)

                    ' Mise en place de la position de la structure
                    MyHauteurMax += Cnvs.Height
                    Cnvs.SetValue(Canvas.TopProperty, _Size.Height - MyHauteurMax)
                    ' Verification quel est la longueur a prendre
                    Cnvs.SetValue(Canvas.LeftProperty, (_Size.Width - Cnvs.Width) / 2)
                    sender.Children.Add(Cnvs)
                End With
            End If
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            '''' FIN DE LA MISE EN PLACE DU FLOTTEUR
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            '''' MISE EN PLACE DU PYLONE
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            For Each pyl In PyloneBouee
                If pyl Is Nothing Then Continue For
                Cnvs = pyl.DrawCanvas(New Size(_Size.Width * (pyl.LargeurMax / LargeurMax), _Size.Height * (pyl.HauteurMax / HauteurMax)))
                'Cnvs.Width = _Size.Width * (pyl.LargeurMax / LargeurMax)
                'Cnvs.Height = _Size.Height * (pyl.HauteurMax / HauteurMax)

                ' Mise en place de la position de la structure
                MyHauteurMax += Cnvs.Height
                Cnvs.SetValue(Canvas.TopProperty, _Size.Height - MyHauteurMax)
                ' Verification quel est la longueur a prendre
                Cnvs.SetValue(Canvas.LeftProperty, (_Size.Width - Cnvs.Width) / 2)
                sender.Children.Add(Cnvs)
            Next
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            '''' FIN DE LA MISE EN PLACE DU PYLONE
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            '''' MISE EN PLACE DES EQUIPEMENTS
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            For Each equ In EquipementBouee
                If equ Is Nothing Then Continue For
                Cnvs = equ.DrawCanvas(New Size(_Size.Width * (equ.LargeurMax / LargeurMax), _Size.Height * (equ.HauteurMax / HauteurMax)))
                'Cnvs.Width = _Size.Width * (equ.LargeurMax / LargeurMax)
                'Cnvs.Height = _Size.Height * (equ.HauteurMax / HauteurMax)

                ' Mise en place de la position de la structure
                MyHauteurMax += Cnvs.Height
                Cnvs.SetValue(Canvas.TopProperty, _Size.Height - MyHauteurMax)
                ' Verification quel est la longueur a prendre
                Cnvs.SetValue(Canvas.LeftProperty, (_Size.Width - Cnvs.Width) / 2)
                sender.Children.Add(Cnvs)
            Next
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            '''' FIN DE LA MISE EN PLACE DES EQUIPEMENTS
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            ' Ajout d'un retour sur changement de taille
            AddHandler sender.SizeChanged, AddressOf MeSizeChanged

            Return sender

        End Function

#End Region

#Region "Gestion des propriete pour le calcul de la ligne de mouillage"

        Private _MASSE_EQUIPEMENT As Double
        Private _MASSE_LEST As Double

        Property MASSE_EQUIPEMENT() As Double
            Get
                Return _MASSE_EQUIPEMENT
            End Get
            Set(ByVal value As Double)
                ' Recuperation de la variables
                _MASSE_EQUIPEMENT = value
            End Set
        End Property

        Property MASSE_LEST() As Double
            Get
                Return _MASSE_LEST
            End Get
            Set(ByVal value As Double)
                ' Recuperation de la variables
                _MASSE_LEST = value
            End Set
        End Property

        ReadOnly Property SURFACE_PYLONE_EQUIPEMENT() As Double
            Get
                SURFACE_PYLONE_EQUIPEMENT = 0
                For Each pyl In PyloneBouee
                    SURFACE_PYLONE_EQUIPEMENT += pyl.Surface
                Next
                For Each equ In EquipementBouee
                    SURFACE_PYLONE_EQUIPEMENT += equ.Surface
                Next
            End Get
        End Property

        ReadOnly Property MASSE_PYLONE_EQUIPEMENT() As Double
            Get
                MASSE_PYLONE_EQUIPEMENT = 0
                For Each pyl In PyloneBouee
                    MASSE_PYLONE_EQUIPEMENT += pyl.Masse
                Next
                For Each equ In EquipementBouee
                    MASSE_PYLONE_EQUIPEMENT += equ.Masse
                Next
            End Get
        End Property

        ReadOnly Property MASSE_BOUEE() As Double
            Get

                ' Inialisation de la masse des equipements
                MASSE_BOUEE = _MASSE_EQUIPEMENT + MASSE_PYLONE_EQUIPEMENT
                If StructureBouee IsNot Nothing Then MASSE_BOUEE += StructureBouee.Masse

                ' Rajout de 10% (Visserie)
                'MASSE_BOUEE = MASSE_BOUEE + MASSE_BOUEE * 0.1

                ' Rajout de la masse de la bouée
                If FlotteurBouee IsNot Nothing Then MASSE_BOUEE += FlotteurBouee.Masse

            End Get
        End Property

        ReadOnly Property PROFONDEUR_SOUS_ORGANEAU() As Double
            Get
                ' Inialisation des masses
                PROFONDEUR_SOUS_ORGANEAU = FlotteurBouee.HAUTEUR_IMMERGEE + StructureBouee.OffsetOrganeau

                ' Mise en place de la hauteur en negatif
                PROFONDEUR_SOUS_ORGANEAU = -1 * PROFONDEUR_SOUS_ORGANEAU
            End Get
        End Property

        ReadOnly Property SURFACE_IMMERGEE() As Double
            Get
                Return FlotteurBouee.SURFACE_IMMERGEE + StructureBouee.SURFACE_IMMERGEE
            End Get
        End Property

        ReadOnly Property SURFACE_EMERGEE() As Double
            Get
                Return FlotteurBouee.SURFACE_EMERGEE + SURFACE_PYLONE_EQUIPEMENT
            End Get
        End Property

        ReadOnly Property VOLUME() As Double
            Get
                VOLUME = 0
                If sFlotteur IsNot Nothing Then VOLUME += sFlotteur.Volume
                If sStructure IsNot Nothing Then VOLUME += sStructure.Volume

                Return VOLUME
            End Get
        End Property

        ReadOnly Property RESERVE_FLOTABILITE() As Double
            Get
                Return Math.Round(((VOLUME - ((FlotteurBouee.VOLUME_IMMERGE + StructureBouee.VOLUME_IMMERGE) / 1000)) / VOLUME) * 100, 2)
            End Get
        End Property

        ReadOnly Property FRANC_BORD() As Double
            Get
                Return FlotteurBouee.HauteurMax - FlotteurBouee.HAUTEUR_IMMERGEE
            End Get
        End Property

        ReadOnly Property TIRANT_D_EAU() As Double
            Get
                Return FlotteurBouee.HAUTEUR_IMMERGEE + StructureBouee.OffsetFlotteur
            End Get
        End Property

#Region "Calcul"

        Public Sub SET_HAUTEUR_IMMERGE_VOL(ByVal VOLUME_DEPLACE As Double, ByVal PAS_D_ITERATION As Double)

            StructureBouee.HAUTEUR_FLOTTEUR = FlotteurBouee.HauteurMax

            ' Calcul de la hauteur immerge par rapport à un volume
            Do
                FlotteurBouee.HAUTEUR_IMMERGEE += PAS_D_ITERATION
                StructureBouee.HAUTEUR_IMMERGEE = FlotteurBouee.HAUTEUR_IMMERGEE
            Loop While VOLUME_IMMERGE < VOLUME_DEPLACE

        End Sub

        Public Sub SET_HAUTEUR_IMMERGE_MAX()

            StructureBouee.HAUTEUR_FLOTTEUR = FlotteurBouee.HauteurMax

            FlotteurBouee.HAUTEUR_IMMERGEE = FlotteurBouee.HauteurMax
            StructureBouee.HAUTEUR_IMMERGEE = FlotteurBouee.HAUTEUR_IMMERGEE

        End Sub

        Public Sub SET_HAUTEUR_IMMERGE(ByVal Hauteur As Double)

            StructureBouee.HAUTEUR_FLOTTEUR = FlotteurBouee.HauteurMax

            FlotteurBouee.HAUTEUR_IMMERGEE = Hauteur
            StructureBouee.HAUTEUR_IMMERGEE = Hauteur

        End Sub

        Public Sub SET_HAUTEUR_IMMERGE_MIN()

            StructureBouee.HAUTEUR_FLOTTEUR = FlotteurBouee.HauteurMax

            FlotteurBouee.HAUTEUR_IMMERGEE = 0
            StructureBouee.HAUTEUR_IMMERGEE = FlotteurBouee.HAUTEUR_IMMERGEE

        End Sub

        Public Sub ITERATION_ENFONCEMENT(ByVal PAS_D_ITERATION As Double)

            StructureBouee.HAUTEUR_FLOTTEUR = FlotteurBouee.HauteurMax

            FlotteurBouee.HAUTEUR_IMMERGEE += PAS_D_ITERATION
            StructureBouee.HAUTEUR_IMMERGEE = FlotteurBouee.HAUTEUR_IMMERGEE

        End Sub

        ReadOnly Property VOLUME_IMMERGE() As Double
            Get
                Return (FlotteurBouee.VOLUME_IMMERGE + StructureBouee.VOLUME_IMMERGE)
            End Get
        End Property

        ReadOnly Property HAUTEUR_IMMERGE() As Double
            Get
                Return FlotteurBouee.HAUTEUR_IMMERGEE
            End Get
        End Property

#End Region

#End Region

#Region "Gestion des actions sur les fichiers"

        Private Function getVersion() As Version

            Dim version As String = System.Reflection.Assembly.GetExecutingAssembly().FullName
            Dim start As Integer = version.IndexOf("Version=") + 8
            Dim length As Integer = version.IndexOf(",", start) - start

            Return New Version(version.Substring(start, length))

        End Function

        Public Function WriteXmlBouee() As String

            Dim Buoy As CBouee = Me

            ' Lecture des informations de structure
            WriteXmlBouee = "Calmar"

            WriteXmlBouee = "<?xml version=""1.0"" encoding=""UTF-8"" ?>" + vbCrLf
            WriteXmlBouee += "<Calmar version=""" + getVersion().ToString + """>" + vbCrLf

            '
            ' ENREGISTREMENT DE LA BOUEE
            '
            WriteXmlBouee += vbTab + "<Buoy Name=""" + Buoy.Nom + """ ChaineMin=""" + Buoy.ChaineMin.ToString.Replace(",", ".") + """ ChaineMax=""" + Buoy.ChaineMax.ToString.Replace(",", ".") + """ MasseLestUnitaire=""" + Buoy.MasseLestUnitaire.ToString.Replace(",", ".") + """ NombreLestMin=""" + Buoy.NombreLestMin.ToString.Replace(",", ".") + """ NombreLestMax=""" + Buoy.NombreLestMax.ToString.Replace(",", ".") + """>" + vbCrLf

            If Buoy.StructureBouee IsNot Nothing Then
                With Buoy.StructureBouee
                    WriteXmlBouee += vbTab + vbTab + "<Structure Name=""" + .Nom + """ Masse=""" + .Masse.ToString.Replace(",", ".") + """ OffsetFlotteur=""" + .OffsetFlotteur.ToString.Replace(",", ".") + """ OffsetOrganeau=""" + .OffsetOrganeau.ToString.Replace(",", ".") + """>" + vbCrLf
                    For Each fldim In .Elements
                        WriteXmlBouee += vbTab + vbTab + vbTab + "<ElementDimItem H=""" + fldim.HauteurElement.ToString.Replace(",", ".") + """ D0=""" + fldim.DiameterLow.ToString.Replace(",", ".") + """ D1=""" + fldim.DiameterHigh.ToString.Replace(",", ".") + """ DI=""" + fldim.DiameterInter.ToString.Replace(",", ".") + """ Volume=""" + fldim.Volume.ToString.Replace(",", ".") + """ />" + vbCrLf
                    Next
                    WriteXmlBouee += vbTab + vbTab + "</Structure>" + vbCrLf
                End With
            End If

            If Buoy.FlotteurBouee IsNot Nothing Then
                With Buoy.FlotteurBouee
                    WriteXmlBouee += vbTab + vbTab + "<Flotteur Name=""" + .Nom + """ Masse=""" + .Masse.ToString.Replace(",", ".") + """>" + vbCrLf
                    For Each fldim In .Elements
                        WriteXmlBouee += vbTab + vbTab + vbTab + "<ElementDimItem H=""" + fldim.HauteurElement.ToString.Replace(",", ".") + """ D0=""" + fldim.DiameterLow.ToString.Replace(",", ".") + """ D1=""" + fldim.DiameterHigh.ToString.Replace(",", ".") + """ DI=""" + fldim.DiameterInter.ToString.Replace(",", ".") + """ Volume=""" + fldim.Volume.ToString.Replace(",", ".") + """ />" + vbCrLf
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

End Namespace
