Imports System.Xml.Serialization

Public Class CInputParameter

    Private _ChaineDN As Double
    Private _ChaineType As String
    Private _ChaineQualite As Integer
    Public _NombreLest As Integer
    Public _PoidsLest As Double
    Private ListEquipement As Dictionary(Of String, List(Of BuoyLib.Buoy.CEquipementSupplementaire))

    Private _Profondeur As Double
    Private _Marnage As Double
    Private _HouleMax As Double
    Private _PeriodeHoule As Double
    Private _VitesseVent As Double
    Private _VitesseVentMS As Boolean
    Private _VitesseCourant As Double
    Private _VitesseCourantMS As Boolean
    Private _StringDensiteCM As String

    Private _Equipements As CLSEquipements()

    <XmlArray("Equipements")> _
    Property Equipements As CLSEquipements()
        Get
            Return _Equipements
        End Get
        Set(ByVal value As CLSEquipements())
            _Equipements = value
        End Set
    End Property

    Public Class CLSEquipements

        Private _Categorie As String
        Private _Equipement() As CEquipement

        <XmlAttribute("Categorie")> _
        Property Categorie As String
            Get
                Return _Categorie
            End Get
            Set(ByVal value As String)
                _Categorie = value
            End Set
        End Property

        <XmlArray("Equipement")> _
        Property Equipement As CEquipement()
            Get
                Return _Equipement
            End Get
            Set(ByVal value As CEquipement())
                _Equipement = value
            End Set
        End Property

        Public Class CEquipement

            Public _Name As String
            Public _Categorie As String
            Public _Nombre As Integer
            Public _MasseUnitaire As Double

            <XmlAttribute("Name")> _
            Property Name As String
                Get
                    Return _Name
                End Get
                Set(ByVal value As String)
                    _Name = value
                End Set
            End Property

            <XmlAttribute("Categorie")> _
            Property Categorie As String
                Get
                    Return _Categorie
                End Get
                Set(ByVal value As String)
                    _Categorie = value
                End Set
            End Property

            <XmlAttribute("Nombre")> _
            Property Nombre As Integer
                Get
                    Return _Nombre
                End Get
                Set(ByVal value As Integer)
                    _Nombre = value
                End Set
            End Property

            <XmlAttribute("MasseUnitaire")> _
            Property MasseUnitaire As Double
                Get
                    Return _MasseUnitaire
                End Get
                Set(ByVal value As Double)
                    _MasseUnitaire = value
                End Set
            End Property


        End Class

    End Class

    <XmlAttribute("ChaineDN")> _
    Property ChaineDN As Double
        Get
            Return _ChaineDN
        End Get
        Set(ByVal value As Double)
            _ChaineDN = value
        End Set
    End Property

    <XmlAttribute("ChaineType")> _
    Property ChaineType As String
        Get
            Return _ChaineType
        End Get
        Set(ByVal value As String)
            _ChaineType = value
        End Set
    End Property

    <XmlAttribute("ChaineQualite")> _
    Property ChaineQualite As Integer
        Get
            Return _ChaineQualite
        End Get
        Set(ByVal value As Integer)
            _ChaineQualite = value
        End Set
    End Property

    <XmlAttribute("NombreLest")> _
    Property NombreLest As Integer
        Get
            Return _NombreLest
        End Get
        Set(ByVal value As Integer)
            _NombreLest = value
        End Set
    End Property

    <XmlAttribute("PoidsLest")> _
    Property PoidsLest As Double
        Get
            Return _PoidsLest
        End Get
        Set(ByVal value As Double)
            _PoidsLest = value
        End Set
    End Property

    <XmlAttribute("Profondeur")> _
    Property Profondeur As Double
        Get
            Return _Profondeur
        End Get
        Set(ByVal value As Double)
            _Profondeur = value
        End Set
    End Property

    <XmlAttribute("Marnage")> _
    Property Marnage As Double
        Get
            Return _Marnage
        End Get
        Set(ByVal value As Double)
            _Marnage = value
        End Set
    End Property

    <XmlAttribute("HouleMax")> _
    Property HouleMax As Double
        Get
            Return _HouleMax
        End Get
        Set(ByVal value As Double)
            _HouleMax = value
        End Set
    End Property

    <XmlAttribute("PeriodeHoule")> _
    Property PeriodeHoule As Double
        Get
            Return _PeriodeHoule
        End Get
        Set(ByVal value As Double)
            _PeriodeHoule = value
        End Set
    End Property

    <XmlAttribute("VitesseVent")> _
    Property VitesseVent As Double
        Get
            Return _VitesseVent
        End Get
        Set(ByVal value As Double)
            _VitesseVent = value
        End Set
    End Property

    <XmlAttribute("VitesseVentMS")> _
    Property VitesseVentMS As Boolean
        Get
            Return _VitesseVentMS
        End Get
        Set(ByVal value As Boolean)
            _VitesseVentMS = value
        End Set
    End Property

    <XmlAttribute("VitesseCourant")> _
    Property VitesseCourant As Double
        Get
            Return _VitesseCourant
        End Get
        Set(ByVal value As Double)
            _VitesseCourant = value
        End Set
    End Property

    <XmlAttribute("VitesseCourantMS")> _
    Property VitesseCourantMS As Boolean
        Get
            Return _VitesseCourantMS
        End Get
        Set(ByVal value As Boolean)
            _VitesseCourantMS = value
        End Set
    End Property

    <XmlAttribute("StringDensiteCM")> _
    Property StringDensiteCM As String
        Get
            Return _StringDensiteCM
        End Get
        Set(ByVal value As String)
            _StringDensiteCM = value
        End Set
    End Property

End Class
