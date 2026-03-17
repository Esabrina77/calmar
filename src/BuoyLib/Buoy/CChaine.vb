Namespace Buoy

    Public Class CChaine
        Implements IComparable

        '' Determiné comme moyenne d'après Rr Carlier sur Studless
        'Const Coef_Section As Double = 1.46

        '' Charge d'epreuve sur Studless
        'Private ConstChargeEpreuve As Dictionary(Of String, Double) = New Dictionary(Of String, Double) _
        '                                                              From {{"Stud", 1.414}, _
        '                                                                    {"3D", 2}, _
        '                                                                    {"3.5D", 2}, _
        '                                                                    {"4D", 2}, _
        '                                                                    {"5D", 2}, _
        '                                                                    {"6D", 2}, _
        '                                                                    {"9D", 2}}

        Private _DN As Double
        Private _TYPE As String
        Private _MASSE_LINEIQUE As Double
        Private _CHARGE_EPREUVE_QL1 As Double
        Private _CHARGE_EPREUVE_QL2 As Double
        Private _CHARGE_EPREUVE_QL3 As Double

        'Private _Rr_QL1 As Double = 430 ' kN
        'Private _Rr_QL2 As Double = 550 ' kN
        'Private _Rr_QL3 As Double = 690 ' kN

        Friend Chaine_NAME As String

        Public Function Clone() As CChaine

            Clone = New CChaine() With {.DN = DN, .TYPE = TYPE, .MASSE_LINEIQUE = MASSE_LINEIQUE}

        End Function

        ReadOnly Property Nom As String
            Get
                Return TYPE + vbTab + "DN:" + DN.ToString
            End Get
        End Property

        Property DN() As Double
            Get
                Return _DN
            End Get
            Set(ByVal value As Double)
                _DN = value
            End Set
        End Property

        Property TYPE() As String
            Get
                Return _TYPE
            End Get
            Set(ByVal value As String)
                _TYPE = value
            End Set
        End Property

        ReadOnly Property TYPE_NUM() As Double
            Get
                If _TYPE.ToLower = "stud" Then Return 4
                Return Double.Parse(_TYPE.Replace("D", ""))
            End Get
        End Property

        Property MASSE_LINEIQUE() As Double
            Get
                Return _MASSE_LINEIQUE
            End Get
            Set(ByVal value As Double)
                _MASSE_LINEIQUE = value
            End Set
        End Property

        Property CHARGE_EPREUVE_QL1() As Double
            Get
                Return _CHARGE_EPREUVE_QL1
            End Get
            Set(ByVal value As Double)
                _CHARGE_EPREUVE_QL1 = value
            End Set
        End Property
        Property CHARGE_EPREUVE_QL2() As Double
            Get
                Return _CHARGE_EPREUVE_QL2
            End Get
            Set(ByVal value As Double)
                _CHARGE_EPREUVE_QL2 = value
            End Set
        End Property
        Property CHARGE_EPREUVE_QL3() As Double
            Get
                Return _CHARGE_EPREUVE_QL3
            End Get
            Set(ByVal value As Double)
                _CHARGE_EPREUVE_QL3 = value
            End Set
        End Property

        ''' <summary>
        ''' Charge d'epreuve (en t)
        ''' </summary>
        ''' <param name="QL"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function CHARGE_EPREUVE(ByVal QL As Integer) As Double

            Select Case QL

                Case 1
                    Return CHARGE_EPREUVE_QL1

                Case 2
                    Return CHARGE_EPREUVE_QL2

                Case 3
                    Return CHARGE_EPREUVE_QL3

            End Select

            Return 0

        End Function

        ' ''' <summary>
        ' ''' Charge d'epreuve (en t)
        ' ''' </summary>
        ' ''' <param name="QL"></param>
        ' ''' <returns></returns>
        ' ''' <remarks></remarks>
        'Public Function CHARGE_EPREUVE(ByVal QL As Integer) As Double

        '    Return RESISTANCE_RUPTURE(QL) / ConstChargeEpreuve(TYPE)

        'End Function

        'Public Function RESISTANCE_RUPTURE(ByVal QL As Integer) As Double

        '    Dim Rr As Double

        '    Select Case QL

        '        Case 1
        '            Rr = _Rr_QL1

        '        Case 2
        '            Rr = _Rr_QL2

        '        Case 3
        '            Rr = _Rr_QL3

        '    End Select

        '    ' Récupération de la resistance de rupture de la chaine en t
        '    Return Math.Round(((Math.PI * (_DN / 2) ^ 2) * Coef_Section * Rr) / 9810, 1)

        'End Function

        Public Overrides Function ToString() As String
            Return Nom
        End Function

        Public Function CompareTo(obj As Object) As Integer Implements System.IComparable.CompareTo
            Return Nothing
        End Function

    End Class

End Namespace