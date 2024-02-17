Public Class Edge

    Public Enum VECTOR
        X
        Y
        Z
        A
        B
        C
    End Enum

    Dim diameter As Double = 10
    Dim pointAngle As Double = 120 '先端角
    Dim relAngle As Double = 17.6 '逃げ角
    Dim scpAngle As Double = 30 'ねじれ角
    Dim phaseAngle As Double = 15.6 '位相角
    Dim webDiameter As Double = 3
    Dim honingAngle As Double = 25
    Dim _honingWidth As Double = 0.1

    Dim C_Angle As Double = 39 'C軸角度

    Dim z_Unit As Double = 0.01

    Dim WheelDia As Double = 30 '砥石径





    Dim edgeCoord As New List(Of List(Of Double))
    Dim relCoord As New List(Of List(Of Double))
    Dim scpCoord As New List(Of List(Of Double))

    Dim macCoord As New List(Of List(Of Double))

    Dim toolTipPoint As New List(Of Double) '加工時の工具先端位置


    Public Sub Edge_Main()

        'Pythonからエッジデータの取得
        Get_EdgeData()

        'エッジデータを下処理
        Get_SplineEdge(5, z_Unit)


        'エッジデータから、逃げ面稜線とすくい面稜線を生成する
        Get_HoningEdge()

        '加工起動計算
        Machining_Calc()


    End Sub

    ''' <summary>
    ''' Pythonからエッジデータを取得
    ''' </summary>
    Private Sub Get_EdgeData()

    End Sub

    ''' <summary>
    ''' エッジデータをスプライン補完でスムージングしZ軸方向等間隔の座標データに変換する
    ''' </summary>
    ''' <param name="in_cnt">エッジデータの使用する要素数を減らす割合（例：2→1/2, 5→1/5）</param>
    ''' <param name="z_unit">Z方向等間隔のデータを作る際のZ方向間隔</param>
    Private Sub Get_SplineEdge(ByVal in_cnt As Integer, ByVal z_unit As Double)
        '計算結果を安定させるためデータをZ方向に等間隔なデータを作る。

        'XとYそれぞれ行う


    End Sub

    ''' <summary>
    ''' エッジデータから、逃げ面稜線とすくい面稜線を生成する
    ''' </summary>
    Private Sub Get_HoningEdge()
        '前回の稜線生成のソースコードを参照してください。

    End Sub

    ''' <summary>
    ''' 加工起動計算
    ''' </summary>
    Private Sub Machining_Calc()
        '【加工起動の基準は逃げ面側稜線とする】
        For i As Integer = 0 To relCoord.Count - 1
            Dim relPoint As New List(Of Double)(relCoord(i))
            Dim scpPoints As New List(Of List(Of Double))(scpCoord)
            Dim iPoint As New List(Of Double)

            Dim B_Angle As Double

            'エッジがX軸基準で水平になるように回転
            Dim a As Double = Get_A_rotateAngle(i)
            Rotation_X(relPoint, a)
            For ii As Integer = 0 To scpPoints.Count - 1
                Rotation_X(scpPoints(ii), a)
            Next
            Find_scpPoint(i, relPoint, scpPoints, iPoint)
            B_Angle = Get_B_Angle(relPoint, iPoint)

            'まずC角旋回する前の砥石オフセットを計算（工具座標で）
            Dim offsetP As New List(Of Double)

            offsetP.Add(WheelDia * D_Sin(B_Angle)) 'X
            offsetP.Add(WheelDia * D_Cos(B_Angle)) 'Y
            offsetP.Add(0) 'Z

            Rotation_X(offsetP, C_Angle)

            ' ///  +-の逆転、座標軸の方向は違っているかもしれないです！ ///
            Dim mc As New List(Of Double)
            mc.Add(toolTipPoint(VECTOR.X) + relPoint(VECTOR.Z) + offsetP(VECTOR.Z)) 'X-AXIS
            mc.Add(toolTipPoint(VECTOR.Y) + relPoint(VECTOR.X) + offsetP(VECTOR.X)) 'Y-AXIS
            mc.Add(toolTipPoint(VECTOR.Z) + relPoint(VECTOR.Y) + offsetP(VECTOR.Y)) 'Y-AXIS

            macCoord.Add(mc)

        Next



    End Sub

    ''' <summary>
    ''' エッジがX軸基準で水平 - offsetになる角度を計算する
    ''' </summary>
    ''' <param name="index">relCoordの指定の要素番号</param>
    ''' <returns></returns>
    Private Function Get_A_rotateAngle(ByVal index As Integer) As Double

        Const offset As Double = 1.0

        'エッジが平行 - offsetになる角度を計算する
        Return (D_ATan(edgeCoord(index)(VECTOR.Z) / edgeCoord(index)(VECTOR.Y)) - offset) * -1

    End Function

    ''' <summary>
    ''' 指定のrelPointのとき、C角旋回している砥石のエッジがすくい側エッジに当たるポイント
    ''' </summary>
    ''' <param name="relP">指定のrelPoint</param>
    ''' <param name="scp">すくい側エッジデータ</param>
    ''' <param name="scpP">すくい側エッジとC角度旋回した砥石の交点座標[return]</param>
    Private Sub Find_scpPoint(ByVal index As Integer, ByVal relP As List(Of Double), ByVal scp As List(Of List(Of Double)), ByVal scpP As List(Of Double))

        'scpの中でどのあたりに交点に近いデータがあるか大まかな絞り込みを行い、その中で総当たりで一番近い要素２点を見つける。
        'とりあえずrelCoord現在地（index）より手前にあることは無いので、現在のindex以降
        Dim L As Double = 2

        Dim a As New List(Of Double)(New Double() {relP(VECTOR.Z), relP(VECTOR.X)})
        Dim b As New List(Of Double)(New Double() {relP(VECTOR.Z) + L * D_Cos(C_Angle), relP(VECTOR.X) + L * D_Sin(C_Angle)})
        For i As Integer = index To scp.Count - 2
            Dim c As New List(Of Double)(New Double() {scp(index)(VECTOR.Z), scp(index)(VECTOR.X)})
            Dim d As New List(Of Double)(New Double() {scp(index + 1)(VECTOR.Z), scp(index + 1)(VECTOR.X)})
            Dim cP As New List(Of Double)
            If IntersectCheck(a, b, c, d, cP) Then
                scpP.Add(cP(VECTOR.Y)) 'X
                Dim py As Double = (scp(index + 1)(VECTOR.Y) - scp(index)(VECTOR.Y)) *
                                    (cP(VECTOR.X) - c(VECTOR.X)) / (d(VECTOR.X) - c(VECTOR.X)) + scp(index)(VECTOR.Y)
                scpP.Add(py)
                scpP.Add(VECTOR.X)

                Exit Sub
            End If
        Next

    End Sub

    Private Function IntersectCheck(ByVal a As List(Of Double), ByVal b As List(Of Double),
                                    ByVal c As List(Of Double), ByVal d As List(Of Double),
                                    ByVal crossP As List(Of Double)) As Boolean

        Dim ksi, eta, delta As Double
        Dim ramda, mu As Double

        ksi = (d(VECTOR.Y) - c(VECTOR.Y)) * (d(VECTOR.X) - a(VECTOR.X)) - (d(VECTOR.X) - c(VECTOR.X)) * (d(VECTOR.Y) - a(VECTOR.Y))

        eta = (b(VECTOR.X) - a(VECTOR.X)) * (d(VECTOR.Y) - a(VECTOR.Y)) - (b(VECTOR.Y) - a(VECTOR.Y)) * (d(VECTOR.X) - (VECTOR.X))

        delta = (b(VECTOR.X) - a(VECTOR.X)) * (d(VECTOR.Y) - c(VECTOR.Y)) - (b(VECTOR.Y) - a(VECTOR.Y)) * (d(VECTOR.X) - c(VECTOR.X))

        ramda = ksi / delta
        mu = eta / delta

        If ((ramda >= 0 And ramda <= 1) And (mu >= 0 And mu <= 1)) Then

            crossP.Add(a(VECTOR.X) + ramda * (b(VECTOR.X) - a(VECTOR.X)))
            crossP.Add(a(VECTOR.Y) + ramda * (b(VECTOR.Y) - a(VECTOR.Y)))

            Return True
        End If

        Return False
    End Function

    ''' <summary>
    ''' 逃げ側座標点とすくい側座標点からB軸角を計算する。
    ''' </summary>
    ''' <param name="relP"></param>
    ''' <param name="iP"></param>
    ''' <returns></returns>
    Private Function Get_B_Angle(ByVal relP As List(Of Double), ByVal iP As List(Of Double)) As Double

        '座標をC角分逆回転
        '/// 回転方向が逆かも！ ///
        Rotation_Y(relP, C_Angle * -1)
        Rotation_Y(iP, C_Angle * -1)

        'XY平面で角度を算出
        '/// 方向が違っているかもしれないので注意 !!! ///
        Return D_ATan((iP(VECTOR.Y) - relP(VECTOR.Y)) / (iP(VECTOR.X) - relP(VECTOR.X)))
        'この時の角度がC角旋回した砥石がホーニング面に当たる角度となる

    End Function




End Class
