Imports System.Math
Module edge


    Dim pointAngle As Double '先端角
    Dim releafAngle As Double '逃げ角
    Dim helixAngle As Double 'ねじれ角
    Dim phaseAngle As Double '位相角










End Module

Class Coordinate
    Public X As Double
    Public Y As Double
    Public Z As Double

    Public Sub New()
        X = 0
        Y = 0
        Z = 0
    End Sub

    Public Sub New(ByVal x As Double, ByVal y As Double, ByVal z As Double)
        Me.X = x
        Me.Y = y
        Me.Z = z
    End Sub
End Class

Class Coordinates
    Dim coord() As Coordinate

    Public Sub New()
        ReDim coord(0)
        coord(0) = New Coordinate()
    End Sub

    Public Sub New(ByVal index As Integer)
        ReDim coord(index)
        For i As Integer = 0 To index
            coord(i) = New Coordinate()
        Next
    End Sub
End Class

Module Math
    Public Function ToRadian(ByVal Degrees As Double) As Double
        Return (PI / 180) * Degrees
    End Function
End Module

Module Matrix
    Public Sub Rotation_X(ByVal deg As Double, ByRef cd As Coordinate)



    End Sub
End Module

