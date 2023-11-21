Imports System
Imports System.Runtime.CompilerServices

Module fileIO

    Public Const SUCCESS As Integer = 0
    Public Const FILE_DOES_NOT_EXIST As Integer = 1



    Public Function fRead_CSV(ByVal path As String, ByVal cd As List(Of List(Of Double))) As Integer

        If Not (IO.File.Exists(path)) Then Return FILE_DOES_NOT_EXIST

        Dim sr As IO.StreamReader = Nothing
        sr = New System.IO.StreamReader(path)

        Dim cnt As Integer = 0
        While sr.Peek >= 0
            Dim line As String() = sr.ReadLine.Split(","c)
            Dim lt As New List(Of Double)
            If cnt = 0 Then
                For i = 0 To 2
                    lt.Add(line(i))
                Next
                cd.Add(lt)
                cnt += 1
            ElseIf cnt < 10 Then

                cnt += 1
            Else
                cnt = 0
            End If
        End While

        sr.Close()

        Return SUCCESS
    End Function

    Public Function fWrite_CSV(ByVal path As String, ByVal cd As List(Of List(Of Double))) As Integer

        'If Not (IO.File.Exists(path)) Then Return FILE_DOES_NOT_EXIST

        Dim sw As System.IO.StreamWriter = Nothing
        sw = New System.IO.StreamWriter(path, False, System.Text.Encoding.UTF8)
        Try


            Dim cnt As Integer = cd.Count - 1
            For i = 0 To cnt
                Dim ct As Integer = 2
                For ii = 0 To ct
                    sw.Write(cd(i)(ii))
                    If ii <> ct Then
                        sw.Write(",")
                    End If
                Next
                sw.Write(vbCrLf)


            Next

            Return SUCCESS

        Catch e As Exception
            Console.WriteLine("ERROR!")
            Return -1

        Finally
            sw.Close()
        End Try


    End Function


End Module
