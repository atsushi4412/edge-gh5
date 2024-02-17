Imports System
Imports System.Math
Imports System.Runtime.CompilerServices
Imports System.Transactions
Imports System.Xml

Module Program
    Public Enum VECTOR
        X
        Y
        Z
    End Enum
    Dim diameter As Double = 10
    Dim pointAngle As Double = 120 '先端角
    Dim relAngle As Double = 17.6 '逃げ角
    Dim scpAngle As Double = 30 'ねじれ角
    Dim phaseAngle As Double = 15.6 '位相角
    Dim webDiameter As Double = 3
    Dim honingAngle As Double = 25
    Dim honingWidth As Double = 0.1

    Dim oldHoningWidth As Double = 0.5

    Dim Z_Unit As Double





    Dim edgeCoord As New List(Of List(Of Double))
    Dim relCoord As New List(Of List(Of Double))
    Dim scpCoord As New List(Of List(Of Double))

    Sub Main(args As String())


        Dim path As String = System.Environment.CurrentDirectory & "\edge.csv"

        fRead_CSV(path, edgeCoord)
        fWrite_CSV(System.Environment.CurrentDirectory & "\eg.csv", edgeCoord)

        Dim i As Integer
        For i = 0 To edgeCoord.Count - 1
            Get_HoningEdgePoint(i)

        Next

        'fWrite_CSV(System.Environment.CurrentDirectory & "\rel.csv", edgeCoord)
        fWrite_CSV(System.Environment.CurrentDirectory & "\rel.csv", relCoord)
        fWrite_CSV(System.Environment.CurrentDirectory & "\scp.csv", scpCoord)


        'relAngle = 11
        'scpAngle = 45
        'honingAngle = 18
        'honingWidth = 3

        'Dim a, b, c, d, e, f, x, y As Double


        'a = D_Tan(relAngle) + D_Tan(90 - honingAngle)
        'b = -1
        'c = 0
        'd = D_Tan(scpAngle) + D_Tan(90 - honingAngle)
        'e = -1
        'f = (D_Tan(scpAngle) + D_Tan(90 - honingAngle)) * (D_Tan(honingAngle) * honingWidth) * -1

        'Console.WriteLine("a=" & a & "c=" & d & "f=" & f)


        'If Not SimultaneousEquations(a, b, c, d, e, f, x, y) Then
        '    Console.WriteLine("ZERO")
        'Else

        '    Console.WriteLine("x = " & x & "  y = " & y)
        'End If

        ''y = (-tan(90- honingAngle) )x + y
        'Dim x1, y1, x2, y2 As Double
        'x1 = x
        'y1 = (-D_Tan(90 - honingAngle)) * x + y
        'x2 = x1 + honingWidth * D_Tan(honingAngle)
        'y2 = y1 - honingWidth

        'Console.WriteLine("X1 = " & x1)
        'Console.WriteLine("Y1 = " & y1)
        'Console.WriteLine("X2 = " & x2)
        'Console.WriteLine("Y2 = " & y2)








    End Sub



    Public Function ToRadian(ByVal degree As Double) As Double
        Return (PI / 180) * degree
    End Function

    Public Function ToDegree(ByVal radian As Double) As Double
        Return (180 / PI) * radian
    End Function

    Public Function D_Sin(ByVal deg As Double) As Double
        Return Sin(ToRadian(deg))
    End Function
    Public Function D_Cos(ByVal deg As Double) As Double
        Return Cos(ToRadian(deg))
    End Function
    Public Function D_Tan(ByVal deg As Double) As Double
        Return Tan(ToRadian(deg))
    End Function
    Public Function D_ASin(ByVal c As Double) As Double
        Return ToDegree(Asin(c))
    End Function
    Public Function D_ACos(ByVal c As Double) As Double
        Return ToDegree(Acos(c))
    End Function
    Public Function D_ATan(ByVal c As Double) As Double
        Return ToDegree(Atan(c))
    End Function


    Public Sub Generate_B()

        '逃げ側稜線基準
        For i As Integer = 0 To relCoord.Count - 1

        Next

    End Sub

    Public Sub Get_scpIntersectPoint(ByVal relpoint As List(Of Double), ByRef scpPoint As List(Of Double))

        Const RANGE As Integer = 5






    End Sub

    Public Function Get_GrindSpeed_coefficient(ByVal index As Integer) As Double
        Dim oldHoningWidth As Double = 0.5
        Dim area_1, area_2 As Double

        Get_HoningEdgePoint(index, oldHoningWidth, area_1)
        Get_HoningEdgePoint(index, area:=area_2)

        Return area_2 / (area_2 - area_1)

    End Function


    Private Function Get_HoningEdgePoint(ByVal index As Integer, Optional ByVal hw As Double = 0, Optional ByRef area As Double = 0) As Integer

        Dim edgePoint As New List(Of Double)(New Double() {edgeCoord(index)(VECTOR.X), edgeCoord(index)(VECTOR.Y), edgeCoord(index)(VECTOR.Z)})

        Dim edgeVector As New List(Of Double)

        Dim el As Integer = 10

        If hw = 0 Then hw = honingWidth

        Dim i As Integer
        Dim ii As Integer
        If index = 0 Then
            i = 0
            ii = 1
        ElseIf index = edgeCoord.Count - 1 Then
            ii = index
            i = index - 1
        Else
            i = index - 1
            ii = index + 1
        End If


        'ポイントの切れ刃ベクトル
        Base_EdgeLine(edgeCoord(i), edgeCoord(ii), edgeVector)

        Dim deg, deg2 As Double
        'ポイントを切れ刃ベクトルとZ軸が平行になるようX軸回転
        deg = IIf(edgeVector(VECTOR.Z) = 0, 0, D_ATan(edgeVector(VECTOR.Y) / edgeVector(VECTOR.Z))) * -1
        Rotation_X(edgePoint, deg)

        'ポイントを　切れ刃とZ軸が平行になるようにY軸回転
        deg2 = IIf(edgeVector(VECTOR.Z) = 0, 0, D_ATan(edgeVector(VECTOR.X) / edgeVector(VECTOR.Z)))
        Rotation_Y(edgePoint, deg2)

        '一旦端数を切る



        'ポイントでの逃げ角（YZ回転考慮）
        Dim pRel As Double = D_ATan(D_Tan(relAngle) / (1 / D_Cos(phaseAngle - deg)))

        'ポイントでのすくい角（YZ回転考慮）
        Dim L As Double = Sqrt(edgeCoord(index)(VECTOR.Y) ^ 2 + edgeCoord(index)(VECTOR.Z) ^ 2)
        Dim cf As Double
        If L <= (webDiameter / 2) Then
            cf = 0
        Else
            cf = (L - webDiameter / 2) / (diameter - webDiameter) / 2
        End If
        Dim pScp As Double = D_ATan(1 / D_Cos(phaseAngle + deg) / (D_Tan(scpAngle * cf) * (1 / D_Cos(deg2))))

        'ポイントでのホーニング稜線ポイント計算
        Dim a, b, c, d, e, f, x, y As Double
        a = D_Tan(pRel) + D_Tan(90 - honingAngle)
        b = -1
        c = 0
        d = D_Tan(pScp) + D_Tan(90 - honingAngle)
        e = -1
        f = (D_Tan(pScp) + D_Tan(90 - honingAngle)) * D_Tan(honingAngle) * hw * -1
        SimultaneousEquations(a, b, c, d, e, f, x, y)

        Dim x1, y1, x2, y2 As Double
        x1 = x
        y1 = (-D_Tan(90 - honingAngle)) * x + y
        x2 = x1 + hw * D_Tan(honingAngle)
        y2 = (-D_Tan(90 - honingAngle)) * x2 + y

        Dim relEdgePoint As New List(Of Double)(New Double() {edgePoint(VECTOR.X) + y1, edgePoint(VECTOR.Y) + x1, edgePoint(VECTOR.Z)})
        Dim scpEdgePoint As New List(Of Double)(New Double() {edgePoint(VECTOR.X) + y2, edgePoint(VECTOR.Y) + x2, edgePoint(VECTOR.Z)})

        '/// 面積計算 ///
        area = ((edgePoint(VECTOR.Y) - scpEdgePoint(VECTOR.Y)) * (relEdgePoint(VECTOR.X) - scpEdgePoint(VECTOR.X)) _
              - (relEdgePoint(VECTOR.Y) - scpEdgePoint(VECTOR.Y)) * (edgePoint(VECTOR.X) - scpEdgePoint(VECTOR.X))) _
             / 2

        'ポイント座標をYZ回転して元の位置に戻す
        Rotation_Y(relEdgePoint, -deg2)
        Rotation_Y(scpEdgePoint, -deg2)
        Rotation_X(relEdgePoint, -deg)
        Rotation_X(scpEdgePoint, -deg)

        '完成
        relCoord.Add(relEdgePoint)
        scpCoord.Add(scpEdgePoint)



        Return 0
    End Function

    Public Function SimultaneousEquations(ByVal a As Double, ByVal b As Double,
                                           ByVal c As Double, ByVal d As Double,
                                           ByVal e As Double, ByVal f As Double,
                                           ByRef x As Double, ByRef y As Double) As Boolean

        If (a * e - b * d) = 0 Then
            Return False
        Else
            x = (c * e - b * f) / (a * e - b * d)
            y = (a * f - c * d) / (a * e - b * d)
        End If

        Return True
    End Function


    Private Sub Base_EdgeLine(ByVal index As Integer,
                              ByRef deg1 As Double, ByRef deg2 As Double)

        Dim el As Integer = 20
        Dim el_h As Integer = el / 2

        Dim i As Integer
        If index < (el_h - 1) Then
            i = 0
        Else
            i = index - (el_h - 1)
        End If

        Dim ii As Integer
        If index > edgeCoord.Count - 1 - el_h Then
            ii = edgeCoord.Count - 1
        Else
            ii = index + el_h
        End If


        Dim sumX As Double
        Dim sumY As Double
        Dim sumZ As Double
        Dim sumXY As Double
        Dim sumXZ As Double
        Dim squareX As Double

        For j As Integer = i To ii


            sumX += edgeCoord(j)(VECTOR.Z)
            sumY += edgeCoord(j)(VECTOR.X)
            sumZ += edgeCoord(j)(VECTOR.Y)
            sumXY += edgeCoord(j)(VECTOR.Z) * edgeCoord(j)(VECTOR.X)
            sumXZ += edgeCoord(j)(VECTOR.Z) * edgeCoord(j)(VECTOR.Y)
            squareX += edgeCoord(j)(VECTOR.Z)
        Next

        deg1 = D_ATan((el * sumXY - sumX * sumY) / (el * squareX - sumX * sumX) / 1)
        deg2 = D_ATan((el * sumXZ - sumX * sumZ) / (el * squareX - sumX * sumX) / 1) * -1


    End Sub
    Private Function Base_EdgeLine(ByVal p1 As List(Of Double), ByVal p2 As List(Of Double),
                              ByVal line As List(Of Double)) As Integer

        Dim xx As Double = p2(VECTOR.X) - p1(VECTOR.X)
        Dim yy As Double = p2(VECTOR.Y) - p1(VECTOR.Y)
        Dim zz As Double = p2(VECTOR.Z) - p1(VECTOR.Z)
        Dim sqr As Double = Sqrt(xx ^ 2 + yy ^ 2 + zz ^ 2)

        xx /= sqr
        yy /= sqr
        zz /= sqr

        line.Add(xx)
        line.Add(yy)
        line.Add(zz)

        Return 0
    End Function

    Public Sub Rotation_X(ByVal v As List(Of Double), ByVal deg As Double)
        Dim xx, yy, zz As Double

        xx = v(VECTOR.X)
        yy = v(VECTOR.Y) * D_Cos(deg) + v(VECTOR.Z) * D_Sin(deg)
        zz = v(VECTOR.Y) * D_Sin(deg) * -1 + v(VECTOR.Z) * D_Cos(deg)

        v(VECTOR.X) = xx
        v(VECTOR.Y) = yy
        v(VECTOR.Z) = zz

    End Sub

    Public Sub Rotation_Y(ByVal v As List(Of Double), ByVal deg As Double)

        Dim xx, yy, zz As Double

        xx = v(VECTOR.X) * D_Cos(deg) - v(VECTOR.Z) * D_Sin(deg)
        yy = v(VECTOR.Y)
        zz = v(VECTOR.X) * D_Sin(deg) + v(VECTOR.Z) * D_Cos(deg)

        v(VECTOR.X) = xx
        v(VECTOR.Y) = yy
        v(VECTOR.Z) = zz

    End Sub

    Public Sub Rotation_Z(ByVal v As List(Of Double), ByVal deg As Double)

        Dim xx, yy, zz As Double

        xx = v(VECTOR.X) * D_Cos(deg) + v(VECTOR.Y) * D_Sin(deg)
        yy = v(VECTOR.X) * D_Sin(deg) + v(VECTOR.Y) * D_Cos(deg)
        zz = v(VECTOR.Z)

        v(VECTOR.X) = xx
        v(VECTOR.Y) = yy
        v(VECTOR.Z) = zz

    End Sub



    Public Function Now_RelAngle_rad(ByVal rad As Double) As Double

        Return 0
    End Function

    Public Function Now_ScpAngle_rad(ByVal rad As Double) As Double

        Return 0
    End Function

End Module
