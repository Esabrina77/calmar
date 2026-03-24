Imports BuoyLib
Imports BuoyLib.Buoy
Imports BuoyLib.Buoy.CBouee

Public Class CCalculMouillage

#Region "Parametre par défault"

    Public Shared DensiteEau As Double = 1.025
    Public Shared DensiteAir As Double = 0.00129
    Public Shared DensiteMetal As Double = 7.85
    Public Shared DensiteLest As Double = 7.32

    Public Shared COEF_TRAINEE_VENT As Double = 1.2
    Public Shared COEF_TRAINEE_COURANT As Double = 1.2
    Public Shared COEF_TRAINEE_CHAINE As Double = 1.2

    Private Shared TRAINEE_COEF As Double = 1.2

    Public Shared SeaBedInternalFrictionAngle As Double = 45
    Public Shared CoefficientSecuriteMasseCorpsMort As Double = 1.5

#End Region

    Public Shared Property COEF_TRAINEE As Double
        Get
            Return TRAINEE_COEF
        End Get
        Set(value As Double)
            TRAINEE_COEF = value
            'COEF_TRAINEE_VENT = TAB_TRAINEE(Trainee_sel)
            COEF_TRAINEE_COURANT = TRAINEE_COEF
            COEF_TRAINEE_CHAINE = TRAINEE_COEF
        End Set
    End Property

    Public Shared Property DENSITE_EAU As Double
        Get
            Return DensiteEau
        End Get
        Set(value As Double)
            DensiteEau = value
        End Set
    End Property

    Public Structure STR_RET_CALCUL

        Public HAUTEUR_IMMERGE As Double
        Public PROFONDEUR_ORGANEAU As Double
        Public DEPLACEMENT_TOTAL As Double
        Public VOLUME_IMMERGE As Double
        Public SURFACE_LATTERAL_IMMERGE As Double
        Public SURFACE_LATTERAL_EMERGE As Double
        Public HAUTEUR_CATENAIRE As Double
        Public TRAINE_COURANT As Double
        Public TRAINE_VAGUE As Double
        Public TRAINE_VENT As Double
        Public TRAINE_HORIZONTALE As Double
        Public LONGUEUR_CATENAIRE As Double
        Public TENSION_CHAINE As Double
        Public COEFFICIENT_SECURITE_CHAINE As Double
        Public ANGLE_TANGENCE As Double
        Public RESERVE_FLOTABILITE As Double
        Public RAYON_D_EVITAGE As Double
        Public MASSE_MIN_CORPS_MORT As Double

    End Structure

    Public Structure STR_RET_CALCUL_RESULTAT

        Public ProfondeurMax As Double
        Public LongueurCat As Double
        Public TensionMaxMouillage As Double
        Public MasseMinimaleCM As Double
        Public ReserveFlotabilite As Double
        Public FrancBordBouee As Double
        Public SurfaceLateraleEmergee As Double
        Public ProfondeurMin As Double
        Public RayonEvitage As Double
        Public CoefSecChaine As Double
        Public CoefSecCM As Double
        Public DeplacementTotal As Double
        Public TirantEau As Double
        Public SurfaceLateraleImmergee As Double

        Public DiametreChaine As Double
        Public Type As String
        Public Profondeur As Double
        Public MasseEquipement As Double
        Public Marnage As Double
        Public DensiteCM As Double
        Public PeriodeVague As Double
        Public VitMaxCourantMS As Double
        Public VitMaxCourantNDS As Double
        Public HauteurMaxVague As Double
        Public HauteurSignifVague As Double
        Public VitMaxVentMS As Double
        Public VitMaxVentKMH As Double

        Public MasseBouee As Double ' kg
        Public HauteurCatenaire As Double

        Public Enfoncement As Double

        Public VolumeDeplacer As Double ' m3
        Public EffortHorizontalKg As Double ' kg
        Public PoidsLineiqueImmergeChaine As Double ' kg

        Public PoidsLestImmerge As Double ' kg
        Public AngleTangence As Double ' °

        Public TraineeVent As Double ' t
        Public TraineeVague As Double ' t
        Public TraineeCourant As Double ' t

        Public VitesseCourantSurface As Double ' m/s

        Public DensiteEau As Double
        Public CoefTraine As Double

        Public TimeCalcul As TimeSpan

    End Structure

    Event RetourCalcul(ByVal RetourCalcul As STR_RET_CALCUL_RESULTAT)

    Private _BUOY As CBouee
    Private _CHAIN As CChaine
    Private _QualiteChaine As Integer

    Private _Lest As CLest
    Private _MasseLest As Double
    Private _MasseEquipement As Double

    Private _Profondeur As Double
    Private _Marnage As Double
    Private _HouleMax As Double
    Private _HouleSignif As Double
    Private _PeriodeHoule As Double
    Private _VitesseVent As Double
    Private _VitesseCourant As Double

    Private _DensiteCM As Double

    Private _LONGUEUR_CATENAIRE As Double

    Private _LastDateCalcul As Date

    ' VARIABLE TEST
    Private _IALA1066 As Boolean

    Property IALA1066() As Boolean
        Get
            Return _IALA1066
        End Get
        Set(value As Boolean)
            _IALA1066 = value
        End Set
    End Property

    ' FIN VARIABLE TEST

    ReadOnly Property LastStartCalculTime As Date
        Get
            Return _LastDateCalcul
        End Get
    End Property

    ''' <summary>
    ''' Propriete du modele de bouée selectionner - Donnée d'entrée pour le calcul
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Property BUOY() As CBouee
        Get
            Return _BUOY
        End Get
        Set(ByVal value As CBouee)
            _BUOY = value
        End Set
    End Property

    ''' <summary>
    ''' Propriete correspondant a la chaine selectionner - Donnée d'entrée pour le calcul
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Property CHAIN() As CChaine
        Get
            Return _CHAIN
        End Get
        Set(ByVal value As CChaine)
            _CHAIN = value
        End Set
    End Property

    ''' <summary>
    ''' Propriete correspondant a la qualite de la chaine selectionner - Donnée d'entrée pour le calcul
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Property QualiteChaine() As Integer
        Get
            Return _QualiteChaine
        End Get
        Set(ByVal value As Integer)
            _QualiteChaine = value
        End Set
    End Property

    ''' <summary>
    ''' Propriete du lest pour le calcul - Donnée d'entrée pour le calcul
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Property Lest() As CLest
        Get
            Return _Lest
        End Get
        Set(ByVal value As CLest)
            _Lest = value
            If _Lest IsNot Nothing Then MasseLest = _Lest._NombreLest * _Lest._PoidsLest
        End Set
    End Property

    ''' <summary>
    ''' Propiete de la masse du lest (en kg) - Donnée d'entrée pour le calcul
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Property MasseLest() As Double
        Get
            Return _MasseLest
        End Get
        Set(ByVal value As Double)
            _MasseLest = value
        End Set
    End Property

    ''' <summary>
    ''' Propiete de la masse de tous les equipements (en kg) - Donnée d'entrée pour le calcul
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Property MasseEquipement() As Double
        Get
            Return _MasseEquipement
        End Get
        Set(ByVal value As Double)
            _MasseEquipement = value
        End Set
    End Property

    ''' <summary>
    ''' Propiete de la profondeur (en m) - Donnée d'entrée pour le calcul
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Property Profondeur() As Double
        Get
            Return _Profondeur
        End Get
        Set(ByVal value As Double)
            _Profondeur = value
        End Set
    End Property

    ''' <summary>
    ''' Propiete du marnage (en m) - Donnée d'entrée pour le calcul
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Property Marnage() As Double
        Get
            Return _Marnage
        End Get
        Set(ByVal value As Double)
            _Marnage = value
        End Set
    End Property

    ''' <summary>
    ''' Propiete de la houle max (en m) - Donnée d'entrée pour le calcul
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Property HouleMax() As Double
        Get
            Return _HouleMax
        End Get
        Set(ByVal value As Double)
            _HouleMax = value
            _HouleSignif = _HouleMax / 1.85
        End Set
    End Property

    ''' <summary>
    ''' Propiete de la houle significant (en m) - Donnée d'entrée pour le calcul
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Property HouleSignif() As Double
        Get
            Return _HouleSignif
        End Get
        Set(ByVal value As Double)
            _HouleSignif = value
            _HouleMax = _HouleSignif * 1.85
        End Set
    End Property

    ''' <summary>
    ''' Propiete de la periode de la houle (en sec) - Donnée d'entrée pour le calcul
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Property PeriodeHoule() As Double
        Get
            Return _PeriodeHoule
        End Get
        Set(ByVal value As Double)
            If value = 0 Then _PeriodeHoule = 4 Else _PeriodeHoule = value
        End Set
    End Property

    ''' <summary>
    ''' Propiete de la vitesse du vent (en m/s) - Donnée d'entrée pour le calcul
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Property VitesseVent() As Double
        Get
            Return _VitesseVent
        End Get
        Set(ByVal value As Double)
            _VitesseVent = value
        End Set
    End Property

    ''' <summary>
    ''' Propiete de la vitesse en vent (en km/h) - Donnée d'entrée pour le calcul
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Property VitesseVentKMH() As Double
        Get
            Return _VitesseVent * 3.6
        End Get
        Set(ByVal value As Double)
            _VitesseVent = value / 3.6
        End Set
    End Property

    ''' <summary>
    ''' Propiete de la vitesse du courant (en m/s) - Donnée d'entrée pour le calcul
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Property VitesseCourant() As Double
        Get
            Return _VitesseCourant
        End Get
        Set(ByVal value As Double)
            _VitesseCourant = value
        End Set
    End Property

    ''' <summary>
    ''' Propiete de la vitesse du courant (en noeuds) - Donnée d'entrée pour le calcul
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Property VitesseCourantNDS() As Double
        Get
            Return _VitesseCourant * 3.6 / 1.852
        End Get
        Set(ByVal value As Double)
            _VitesseCourant = value * 1.852 / 3.6
        End Set
    End Property

    ''' <summary>
    ''' Propiete de la densité du corps mort (en kg/m³) - Donnée d'entrée pour le calcul
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Property DensiteCM() As Double
        Get
            Return _DensiteCM
        End Get
        Set(ByVal value As Double)
            _DensiteCM = value
        End Set
    End Property

    ''' <summary>
    ''' Ratio d'absurdité pour une comparaison avec le calcul
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ReadOnly Property RATIO_PROFONDEUR_PAR_TROIS() As Double
        Get
            Return _LONGUEUR_CATENAIRE / (3 * Profondeur)
        End Get
    End Property

    ''' <summary>
    ''' Propiete calculer par l'adition de la :
    ''' Profondeur + marnage + houle maximun / 2
    '''  (en m) - Donnée d'entrée pour le calcul
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ReadOnly Property PROFONDEUR_MAX() As Double
        Get
            Return (_Profondeur + _Marnage + _HouleMax / 2)
        End Get
    End Property

    ''' <summary>
    ''' Propiete calculer par la soustraction de la profondeur et la houle maximun sur 2  (en m) - Donnée d'entrée pour le calcul
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ReadOnly Property PROFONDEUR_MIN() As Double
        Get
            Return (_Profondeur - (_HouleMax / 2))
        End Get
    End Property

    ''' <summary>
    ''' Propiete calculer par l'adition de la :
    ''' Profondeur maximun + Profondeur de l'organeau de la bouée
    ''' (en m) - Donnée d'entrée pour le calcul
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ReadOnly Property HAUTEUR_CATENAIRE() As Double
        Get
            Return PROFONDEUR_MAX + _BUOY.PROFONDEUR_SOUS_ORGANEAU
        End Get
    End Property

    ''' <summary>
    ''' Propiete calculer par l'adition de la 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ReadOnly Property VITESSE_COURANT_SURFACE() As Double
        Get
            If IALA1066 Then Return _VitesseCourant
            Return (_VitesseCourant + VITESSE_HOULE + (_VitesseVent * 0.015))
        End Get
    End Property

    ReadOnly Property DIAMETRE_CHAINE() As Double
        Get
            Return CHAIN.DN / 1000
        End Get
    End Property

    ReadOnly Property SURFACE_CHAINE() As Double
        Get
            Return (HAUTEUR_CATENAIRE * (DIAMETRE_CHAINE * 2.65))
        End Get
    End Property

    ReadOnly Property VITESSE_HOULE() As Double
        Get
            Return Math.PI * (HouleMax / 1.85) / PeriodeHoule
        End Get
    End Property

    ReadOnly Property POIDS_LINEIQUE() As Double
        Get
            Return CHAIN.MASSE_LINEIQUE / DensiteMetal * (DensiteMetal - DensiteEau)
        End Get
    End Property

    ReadOnly Property POIDS_CATENAIRE_IMMERGE() As Double
        Get
            Return _LONGUEUR_CATENAIRE * POIDS_LINEIQUE
        End Get
    End Property

    ReadOnly Property POIDS_LEST_IMMERGE() As Double
        Get
            Return _BUOY.MASSE_LEST / DensiteLest * (DensiteLest - DensiteEau)
        End Get
    End Property

    ReadOnly Property DEPLACEMENT_TOTAL() As Double
        Get
            Return _BUOY.MASSE_BOUEE + POIDS_LEST_IMMERGE + POIDS_CATENAIRE_IMMERGE
        End Get
    End Property

    ReadOnly Property VOLUME_DEPLACE() As Double
        Get
            Return DEPLACEMENT_TOTAL / DensiteEau
        End Get
    End Property

    ReadOnly Property EFFORT_HORIZONTALE() As Double
        Get
            If IALA1066 Then Return CalculEffortVent() + CalculEffortCourantSurfaceSurBouee()
            Return CalculEffortVent() + CalculEffortCourantSurChaine() + CalculEffortCourantSurfaceSurBouee()
        End Get
    End Property

    ReadOnly Property LONGUEUR_CATENAIRE() As Double
        Get
            Return CalculLongueurCatenaire(EFFORT_HORIZONTALE)
        End Get
    End Property

    ReadOnly Property TENSION_CHAINE() As Double
        Get
            Return Math.Sqrt((POIDS_CATENAIRE_IMMERGE / 1000) ^ 2 + EFFORT_HORIZONTALE ^ 2)
        End Get
    End Property

    ReadOnly Property ANGLE_TANGENCE() As Double
        Get
            Return DEGREE(Math.Acos(POIDS_CATENAIRE_IMMERGE / (TENSION_CHAINE * 1000)))
        End Get
    End Property

    ReadOnly Property COEFFICIENT_SECURITE_CHAINE() As Double
        Get
            Return CHAIN.CHARGE_EPREUVE(QualiteChaine) / (((HAUTEUR_CATENAIRE * POIDS_LINEIQUE) / 1000) + EFFORT_HORIZONTALE)
        End Get
    End Property

    ReadOnly Property RESERVE_FLOATABILITE() As Double
        Get
            Return _BUOY.RESERVE_FLOTABILITE
        End Get
    End Property

    ReadOnly Property RAYON_D_EVITAGE() As Double
        Get
            Return ((EFFORT_HORIZONTALE * 1000) / POIDS_LINEIQUE) * ACosH((HAUTEUR_CATENAIRE / ((EFFORT_HORIZONTALE * 1000) / POIDS_LINEIQUE)) + 1)
        End Get
    End Property

    ReadOnly Property MASSE_MIN_CORPS_MORT() As Double
        Get
            'Return (CoefficientSecuriteMasseCorpsMort * EFFORT_HORIZONTALE * (_DensiteCM / (_DensiteCM - DensiteEau)) / (Math.Tan(RADIAN(SeaBedInternalFrictionAngle)))) / 9.81
            Return CoefficientSecuriteMasseCorpsMort * EFFORT_HORIZONTALE * (_DensiteCM / (_DensiteCM - DensiteEau)) / (Math.Tan(RADIAN(SeaBedInternalFrictionAngle)))
        End Get
    End Property

    ReadOnly Property POIDS_CORPS_MORT_IMMERGE() As Double
        Get
            Return MASSE_MIN_CORPS_MORT / _DensiteCM * (_DensiteCM - DensiteEau)
        End Get
    End Property

    Public Function MakeCalcul(ByVal PasIteration As Double) As STR_RET_CALCUL()

        'Dim initValue As Boolean = True
        Dim ListRet As New List(Of STR_RET_CALCUL)

        ' Initialisation de la valeur
        _BUOY.SET_HAUTEUR_IMMERGE_MIN()
        _BUOY.MASSE_EQUIPEMENT = _MasseEquipement
        _BUOY.MASSE_LEST = _MasseLest
        _LONGUEUR_CATENAIRE = HAUTEUR_CATENAIRE
        'ListRet.Add(New STR_RET_CALCUL() With {.HAUTEUR_IMMERGE = _BUOY.HAUTEUR_IMMERGE, .PROFONDEUR_ORGANEAU = _BUOY.PROFONDEUR_SOUS_ORGANEAU, .DEPLACEMENT_TOTAL = VOLUME_DEPLACE / 1000, .VOLUME_IMMERGE = _BUOY.VOLUME_IMMERGE / 1000, .HAUTEUR_CATENAIRE = HAUTEUR_CATENAIRE, .TRAINE_COURANT = CalculEffortCourantSurChaine(), .TRAINE_VAGUE = CalculEffortCourantSurfaceSurBouee(), .TRAINE_VENT = CalculEffortVent(), .TRAINE_HORIZONTALE = EFFORT_HORIZONTALE, .LONGUEUR_CATENAIRE = _LONGUEUR_CATENAIRE, .TENSION_CHAINE = TENSION_CHAINE, .COEFFICIENT_SECURITE_CHAINE = COEFFICIENT_SECURITE_CHAINE, .ANGLE_TANGENCE = ANGLE_TANGENCE, .RESERVE_FLOTABILITE = RESERVE_FLOATABILITE, .RAYON_D_EVITAGE = RAYON_D_EVITAGE, .MASSE_MIN_CORPS_MORT = MASSE_MIN_CORPS_MORT})

        Do
            _BUOY.ITERATION_ENFONCEMENT(PasIteration)
            _LONGUEUR_CATENAIRE = LONGUEUR_CATENAIRE

            ListRet.Add(New STR_RET_CALCUL() With {.HAUTEUR_IMMERGE = _BUOY.HAUTEUR_IMMERGE, .PROFONDEUR_ORGANEAU = _BUOY.PROFONDEUR_SOUS_ORGANEAU, .DEPLACEMENT_TOTAL = VOLUME_DEPLACE / 1000, .VOLUME_IMMERGE = _BUOY.VOLUME_IMMERGE / 1000, .SURFACE_LATTERAL_IMMERGE = _BUOY.SURFACE_IMMERGEE, .SURFACE_LATTERAL_EMERGE = _BUOY.SURFACE_EMERGEE, .HAUTEUR_CATENAIRE = HAUTEUR_CATENAIRE, .TRAINE_COURANT = CalculEffortCourantSurChaine(), .TRAINE_VAGUE = CalculEffortCourantSurfaceSurBouee(), .TRAINE_VENT = CalculEffortVent(), .TRAINE_HORIZONTALE = EFFORT_HORIZONTALE, .LONGUEUR_CATENAIRE = _LONGUEUR_CATENAIRE, .TENSION_CHAINE = TENSION_CHAINE, .COEFFICIENT_SECURITE_CHAINE = COEFFICIENT_SECURITE_CHAINE, .ANGLE_TANGENCE = ANGLE_TANGENCE, .RESERVE_FLOTABILITE = RESERVE_FLOATABILITE, .RAYON_D_EVITAGE = RAYON_D_EVITAGE, .MASSE_MIN_CORPS_MORT = MASSE_MIN_CORPS_MORT})
        Loop Until _BUOY.VOLUME_IMMERGE >= VOLUME_DEPLACE

        Return ListRet.ToArray

    End Function

    Private Sub DoCalculThread()

        Const PasIteration As Double = 0.005
        Dim RetCalcul As STR_RET_CALCUL_RESULTAT

        ' Initialisation de la valeur
        _BUOY.SET_HAUTEUR_IMMERGE_MIN()
        _BUOY.MASSE_EQUIPEMENT = _MasseEquipement
        _BUOY.MASSE_LEST = _MasseLest
        _LONGUEUR_CATENAIRE = HAUTEUR_CATENAIRE

        Do
            _BUOY.ITERATION_ENFONCEMENT(PasIteration)
            _LONGUEUR_CATENAIRE = LONGUEUR_CATENAIRE
        Loop Until _BUOY.VOLUME_IMMERGE >= VOLUME_DEPLACE

        With RetCalcul

            .ProfondeurMax = PROFONDEUR_MAX
            .LongueurCat = LONGUEUR_CATENAIRE
            .TensionMaxMouillage = TENSION_CHAINE
            .MasseMinimaleCM = MASSE_MIN_CORPS_MORT
            .ReserveFlotabilite = RESERVE_FLOATABILITE
            .FrancBordBouee = _BUOY.FRANC_BORD
            .SurfaceLateraleEmergee = _BUOY.SURFACE_EMERGEE
            .ProfondeurMin = PROFONDEUR_MIN
            .RayonEvitage = RAYON_D_EVITAGE
            .CoefSecChaine = COEFFICIENT_SECURITE_CHAINE
            .CoefSecCM = POIDS_CORPS_MORT_IMMERGE / EFFORT_HORIZONTALE
            .DeplacementTotal = DEPLACEMENT_TOTAL / 1000
            .TirantEau = _BUOY.TIRANT_D_EAU
            .SurfaceLateraleImmergee = _BUOY.SURFACE_IMMERGEE

            .DiametreChaine = CHAIN.DN
            .Type = CHAIN.TYPE + " " + "Q" + QualiteChaine.ToString
            .Profondeur = _Profondeur
            .Marnage = _Marnage
            .DensiteCM = _DensiteCM
            .PeriodeVague = PeriodeHoule
            .VitMaxCourantMS = VitesseCourant
            .VitMaxCourantNDS = VitesseCourantNDS
            .HauteurMaxVague = HouleMax
            .HauteurSignifVague = HouleSignif
            .VitMaxVentMS = VitesseVent
            .VitMaxVentKMH = VitesseVentKMH

            .DensiteEau = DENSITE_EAU
            .CoefTraine = COEF_TRAINEE

            .EffortHorizontalKg = EFFORT_HORIZONTALE * 1000
            .PoidsLineiqueImmergeChaine = POIDS_LINEIQUE
            .PoidsLestImmerge = POIDS_LEST_IMMERGE
            .AngleTangence = ANGLE_TANGENCE

            .TraineeVent = CalculEffortVent()
            .TraineeCourant = CalculEffortCourantSurChaine()
            .TraineeVague = CalculEffortCourantSurfaceSurBouee()
            .VitesseCourantSurface = VITESSE_COURANT_SURFACE

        End With

        RaiseEvent RetourCalcul(RetCalcul)

    End Sub

    Public Function DoCalcul() As STR_RET_CALCUL_RESULTAT

        Return DoCalcul(0.005)

    End Function

    Public Function DoCalcul(ByVal PasIteration As Double) As STR_RET_CALCUL_RESULTAT

        Dim NewTimeSpan = New TimeSpan(Now.Ticks)
        _LastDateCalcul = New Date(Now.Ticks)

        DoCalcul = InitVarCalcul()
        If Not CheckCalcul() Then Return DoCalcul

        ' Initialisation de la valeur
        _BUOY.SET_HAUTEUR_IMMERGE_MIN()
        _BUOY.MASSE_EQUIPEMENT = _MasseEquipement
        _BUOY.MASSE_LEST = _MasseLest
        _LONGUEUR_CATENAIRE = HAUTEUR_CATENAIRE

        Do
            _BUOY.ITERATION_ENFONCEMENT(PasIteration)
            _LONGUEUR_CATENAIRE = LONGUEUR_CATENAIRE
            If Now.Subtract(_LastDateCalcul).TotalSeconds > 10 Then
                If MessageBox.Show("The mooring calculation is too long (your modele or your input parameter are not correct)" + vbCrLf + "Do you want to stop?", "Mooring process", MessageBoxButton.YesNo) = MessageBoxResult.Yes Then
                    Return Nothing
                Else
                    _LastDateCalcul = New Date(Now.Ticks)
                End If
            End If
        Loop Until _BUOY.VOLUME_IMMERGE >= VOLUME_DEPLACE

        With DoCalcul

            .ProfondeurMax = PROFONDEUR_MAX
            .LongueurCat = LONGUEUR_CATENAIRE
            .TensionMaxMouillage = TENSION_CHAINE
            .MasseMinimaleCM = MASSE_MIN_CORPS_MORT
            .ReserveFlotabilite = RESERVE_FLOATABILITE
            .FrancBordBouee = _BUOY.FRANC_BORD
            .SurfaceLateraleEmergee = _BUOY.SURFACE_EMERGEE
            .ProfondeurMin = PROFONDEUR_MIN
            .RayonEvitage = RAYON_D_EVITAGE
            .CoefSecChaine = COEFFICIENT_SECURITE_CHAINE
            .CoefSecCM = POIDS_CORPS_MORT_IMMERGE / EFFORT_HORIZONTALE
            .DeplacementTotal = DEPLACEMENT_TOTAL / 1000
            .TirantEau = _BUOY.TIRANT_D_EAU
            .SurfaceLateraleImmergee = _BUOY.SURFACE_IMMERGEE

            .DiametreChaine = CHAIN.DN
            .Type = CHAIN.TYPE + " " + "Q" + QualiteChaine.ToString
            .Profondeur = _Profondeur
            .MasseEquipement = _MasseEquipement
            .Marnage = _Marnage
            .DensiteCM = _DensiteCM
            .PeriodeVague = PeriodeHoule
            .VitMaxCourantMS = VitesseCourant
            .VitMaxCourantNDS = VitesseCourantNDS
            .HauteurMaxVague = HouleMax
            .HauteurSignifVague = HouleSignif
            .VitMaxVentMS = VitesseVent
            .VitMaxVentKMH = VitesseVentKMH

            .DensiteEau = DENSITE_EAU
            .CoefTraine = COEF_TRAINEE

            .Enfoncement = _BUOY.FlotteurBouee.HAUTEUR_IMMERGEE
            .MasseBouee = _BUOY.MASSE_BOUEE
            .HauteurCatenaire = HAUTEUR_CATENAIRE

            .VolumeDeplacer = VOLUME_DEPLACE
            .EffortHorizontalKg = EFFORT_HORIZONTALE * 1000
            .PoidsLineiqueImmergeChaine = POIDS_LINEIQUE
            .PoidsLestImmerge = POIDS_LEST_IMMERGE
            .AngleTangence = ANGLE_TANGENCE

            .TraineeVent = CalculEffortVent()
            .TraineeCourant = CalculEffortCourantSurChaine()
            .TraineeVague = CalculEffortCourantSurfaceSurBouee()
            .VitesseCourantSurface = VITESSE_COURANT_SURFACE

            .TimeCalcul = New TimeSpan(Now.Ticks).Subtract(NewTimeSpan)
        End With

        RaiseEvent RetourCalcul(DoCalcul)

    End Function

    Public Function DoCalculPIC(ByVal SetLongueurCatenaire As Double, Optional ByVal PasIteration As Double = 0.001) As STR_RET_CALCUL_RESULTAT

        Dim NewTimeSpan = New TimeSpan(Now.Ticks)
        _LastDateCalcul = New Date(Now.Ticks)

        DoCalculPIC = InitVarCalcul()
        If Not CheckCalcul() Then Return DoCalcul()

        ' Initialisation de la valeur
        _BUOY.SET_HAUTEUR_IMMERGE_MIN()
        _BUOY.MASSE_EQUIPEMENT = _MasseEquipement
        _BUOY.MASSE_LEST = _MasseLest
        _LONGUEUR_CATENAIRE = SetLongueurCatenaire

        ' Calcul normal
        Do
            _BUOY.ITERATION_ENFONCEMENT(PasIteration)
        Loop Until _BUOY.VOLUME_IMMERGE >= VOLUME_DEPLACE

        With DoCalculPIC

            .ProfondeurMax = PROFONDEUR_MAX
            .LongueurCat = LONGUEUR_CATENAIRE
            .TensionMaxMouillage = TENSION_CHAINE
            .MasseMinimaleCM = MASSE_MIN_CORPS_MORT
            .ReserveFlotabilite = RESERVE_FLOATABILITE
            .FrancBordBouee = _BUOY.FRANC_BORD
            .SurfaceLateraleEmergee = _BUOY.SURFACE_EMERGEE
            .ProfondeurMin = PROFONDEUR_MIN
            .RayonEvitage = RAYON_D_EVITAGE
            .CoefSecChaine = COEFFICIENT_SECURITE_CHAINE
            .CoefSecCM = POIDS_CORPS_MORT_IMMERGE / EFFORT_HORIZONTALE
            .DeplacementTotal = DEPLACEMENT_TOTAL / 1000
            .TirantEau = _BUOY.TIRANT_D_EAU
            .SurfaceLateraleImmergee = _BUOY.SURFACE_IMMERGEE

            .DiametreChaine = CHAIN.DN
            .Type = CHAIN.TYPE + " " + "Q" + QualiteChaine.ToString
            .Profondeur = _Profondeur
            .MasseEquipement = _MasseEquipement
            .Marnage = _Marnage
            .DensiteCM = _DensiteCM
            .PeriodeVague = PeriodeHoule
            .VitMaxCourantMS = VitesseCourant
            .VitMaxCourantNDS = VitesseCourantNDS
            .HauteurMaxVague = HouleMax
            .HauteurSignifVague = HouleSignif
            .VitMaxVentMS = VitesseVent
            .VitMaxVentKMH = VitesseVentKMH

            .DensiteEau = DENSITE_EAU
            .CoefTraine = COEF_TRAINEE

            .Enfoncement = _BUOY.FlotteurBouee.HAUTEUR_IMMERGEE
            .MasseBouee = _BUOY.MASSE_BOUEE
            .HauteurCatenaire = HAUTEUR_CATENAIRE

            .VolumeDeplacer = VOLUME_DEPLACE
            .EffortHorizontalKg = EFFORT_HORIZONTALE * 1000
            .PoidsLineiqueImmergeChaine = POIDS_LINEIQUE
            .PoidsLestImmerge = POIDS_LEST_IMMERGE
            .AngleTangence = ANGLE_TANGENCE

            .TraineeVent = CalculEffortVent()
            .TraineeCourant = CalculEffortCourantSurChaine()
            .TraineeVague = CalculEffortCourantSurfaceSurBouee()
            .VitesseCourantSurface = VITESSE_COURANT_SURFACE

            .TimeCalcul = New TimeSpan(Now.Ticks).Subtract(NewTimeSpan)
        End With

        RaiseEvent RetourCalcul(DoCalcul)

    End Function

    Private Function InitVarCalcul() As STR_RET_CALCUL_RESULTAT

        With InitVarCalcul

            .ProfondeurMax = 0.0
            .LongueurCat = 0.0
            .TensionMaxMouillage = 0.0
            .MasseMinimaleCM = 0.0
            .ReserveFlotabilite = 0.0
            .FrancBordBouee = 0.0
            .SurfaceLateraleEmergee = 0.0
            .ProfondeurMin = 0.0
            .RayonEvitage = 0.0
            .CoefSecChaine = 0.0
            .CoefSecCM = 0.0
            .DeplacementTotal = 0.0
            .TirantEau = 0.0
            .SurfaceLateraleImmergee = 0.0

            .DiametreChaine = 0.0
            .Type = 0.0
            .Profondeur = 0.0
            .MasseEquipement = 0.0
            .Marnage = 0.0
            .DensiteCM = 0.0
            .PeriodeVague = 0.0
            .VitMaxCourantMS = 0.0
            .VitMaxCourantNDS = 0.0
            .HauteurMaxVague = 0.0
            .HauteurSignifVague = 0.0
            .VitMaxVentMS = 0.0
            .VitMaxVentKMH = 0.0

            .Enfoncement = 0.0
            .MasseBouee = 0.0
            .HauteurCatenaire = 0.0

            .VolumeDeplacer = 0.0
            .EffortHorizontalKg = 0.0
            .PoidsLineiqueImmergeChaine = 0.0
            .PoidsLestImmerge = 0.0
            .AngleTangence = 0.0

            .TraineeVent = 0.0
            .TraineeCourant = 0.0
            .TraineeVague = 0.0
            .VitesseCourantSurface = 0.0

            .TimeCalcul = New TimeSpan(0)
        End With

    End Function

    Public Function MakeCalculCatenaire(ByVal PasIteration As Double) As STR_RET_CALCUL()

        Dim ListRet As New List(Of STR_RET_CALCUL)

        ' Initialisation de la valeur
        _BUOY.SET_HAUTEUR_IMMERGE_MIN()
        _BUOY.MASSE_EQUIPEMENT = _MasseEquipement
        _BUOY.MASSE_LEST = _MasseLest
        _LONGUEUR_CATENAIRE = 0
        ListRet.Add(New STR_RET_CALCUL() With {.HAUTEUR_IMMERGE = _BUOY.HAUTEUR_IMMERGE, .PROFONDEUR_ORGANEAU = _BUOY.PROFONDEUR_SOUS_ORGANEAU, .DEPLACEMENT_TOTAL = VOLUME_DEPLACE / 1000, .VOLUME_IMMERGE = _BUOY.VOLUME_IMMERGE / 1000, .HAUTEUR_CATENAIRE = HAUTEUR_CATENAIRE, .TRAINE_COURANT = CalculEffortCourantSurChaine(), .TRAINE_VAGUE = CalculEffortCourantSurfaceSurBouee(), .TRAINE_VENT = CalculEffortVent(), .TRAINE_HORIZONTALE = EFFORT_HORIZONTALE, .LONGUEUR_CATENAIRE = _LONGUEUR_CATENAIRE, .TENSION_CHAINE = TENSION_CHAINE, .COEFFICIENT_SECURITE_CHAINE = COEFFICIENT_SECURITE_CHAINE, .ANGLE_TANGENCE = ANGLE_TANGENCE, .RESERVE_FLOTABILITE = RESERVE_FLOATABILITE, .RAYON_D_EVITAGE = RAYON_D_EVITAGE, .MASSE_MIN_CORPS_MORT = MASSE_MIN_CORPS_MORT})

        Do
            _LONGUEUR_CATENAIRE += PasIteration

            ListRet.Add(New STR_RET_CALCUL() With {.HAUTEUR_IMMERGE = _BUOY.HAUTEUR_IMMERGE, .PROFONDEUR_ORGANEAU = _BUOY.PROFONDEUR_SOUS_ORGANEAU, .DEPLACEMENT_TOTAL = VOLUME_DEPLACE / 1000, .VOLUME_IMMERGE = _BUOY.VOLUME_IMMERGE / 1000, .HAUTEUR_CATENAIRE = HAUTEUR_CATENAIRE, .TRAINE_COURANT = CalculEffortCourantSurChaine(), .TRAINE_VAGUE = CalculEffortCourantSurfaceSurBouee(), .TRAINE_VENT = CalculEffortVent(), .TRAINE_HORIZONTALE = EFFORT_HORIZONTALE, .LONGUEUR_CATENAIRE = _LONGUEUR_CATENAIRE, .TENSION_CHAINE = TENSION_CHAINE, .COEFFICIENT_SECURITE_CHAINE = COEFFICIENT_SECURITE_CHAINE, .ANGLE_TANGENCE = ANGLE_TANGENCE, .RESERVE_FLOTABILITE = RESERVE_FLOATABILITE, .RAYON_D_EVITAGE = RAYON_D_EVITAGE, .MASSE_MIN_CORPS_MORT = MASSE_MIN_CORPS_MORT})
        Loop Until _BUOY.VOLUME_IMMERGE >= VOLUME_DEPLACE

        Return ListRet.ToArray

    End Function

    Private Function CalculEffortVent()

        Return (0.5 * (_VitesseVent ^ 2) * DensiteAir * (COEF_TRAINEE_VENT * _BUOY.SURFACE_EMERGEE)) / 9.81

    End Function

    Private Function CalculEffortCourantSurChaine()

        'Return (0.5 * (VITESSE_COURANT_SURFACE ^ 2) * DensiteEau * (COEF_TRAINEE_CHAINE * SURFACE_CHAINE)) / 9.81

        ' Mise à jour DH 05/10/2012
        Return (0.5 * (_VitesseCourant ^ 2) * DensiteEau * (COEF_TRAINEE_CHAINE * SURFACE_CHAINE)) / 9.81

    End Function

    Private Function CalculEffortCourantSurfaceSurBouee() As Double

        Return (0.5 * (VITESSE_COURANT_SURFACE ^ 2) * DensiteEau * (COEF_TRAINEE_COURANT * _BUOY.SURFACE_IMMERGEE)) / 9.81

    End Function

    Private Function CalculLongueurCatenaire(ByVal EffortHorizontale As Double) As Double

        Return Math.Sqrt(HAUTEUR_CATENAIRE ^ 2 + (2 * ((EffortHorizontale * 1000) / POIDS_LINEIQUE) * HAUTEUR_CATENAIRE))

    End Function

    Private Function DEGREE(ByVal rad As Double) As Double

        Return ((rad * 180) / Math.PI)

    End Function

    Private Function RADIAN(ByVal deg As Double) As Double

        Return (Math.PI / 180) * deg

    End Function

    Private Function ACosH(ByVal value As Double) As Double
        Return Math.Log(value + Math.Sqrt(value * value - 1))
    End Function

    Public Function CheckCalcul() As Boolean

        If _BUOY Is Nothing Then Return False
        If _CHAIN Is Nothing Then Return False

        If _Profondeur <= 0 Then Return False
        If _DensiteCM <= 0 Then Return False

        If _HouleMax < 0 Then Return False
        If _Marnage < 0 Then Return False
        If _VitesseVent < 0 Then Return False
        If _VitesseCourant < 0 Then Return False

        Return True

    End Function

    Public Function Clone() As CCalculMouillage

        Clone = New CCalculMouillage

        ' Calcul sur la bouée numero 1
        If BUOY IsNot Nothing Then Clone.BUOY = BUOY.Clone
        If CHAIN IsNot Nothing Then Clone.CHAIN = CHAIN.Clone

        ' Selection de la qualité
        Clone.QualiteChaine = QualiteChaine

        If Lest IsNot Nothing Then Clone.Lest = Lest.Clone
        Clone.MasseEquipement = MasseEquipement
        Clone.Profondeur = Profondeur
        Clone.Marnage = Marnage
        Clone.HouleMax = HouleMax
        Clone.PeriodeHoule = PeriodeHoule
        Clone.VitesseVent = VitesseVent
        Clone.VitesseCourant = VitesseCourant
        Clone.DensiteCM = DensiteCM

    End Function

End Class
