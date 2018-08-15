Imports System.Text
Imports System.IO

Public Class FrmInvertirNudos

    Private Structure TipLineas
        Dim ID As String                    'Identificador de la tubería
        Dim IDN1 As String                  'Identificador nudo inicial
        Dim IDN2 As String                  'Identificador nudo final
        Dim Longitud As String              'Longitud de la tubería
        Dim Diametro As String              'Diámetro de la tubería
        Dim Rugosidad As String             'Rugosidad de la tubería
        Dim PerdMen As String               'Pérdidas menores de la tubería
        Dim EstadoIni As String             'Estado inicial de la tubería
        Dim Descripcion As String           'Descripción auxiliar
        Dim VertX As Collection             'Coordenada X del vértice de una tubería
        Dim VertY As Collection             'Coordenada Y del vértice de una tubería
        Dim InvertNudos As Boolean          'Invertir si o no la tubería
        Dim ListaQ As Collection            'Lista de caudales para cada tubería en toda la simulación
    End Structure

    Dim numTubmalTrazadas As Integer        'Número de tuberías mal trazadas
    Dim IDTubmalTrazadas As Collection      'ID de las tuberías mal trazadas en un fichero de texto
    Dim numLineas As Integer                'Número de líneas en el modelo de red
    Dim Linea() As TipLineas                'Vector de líneas definido por el usuario

    Private Sub CmdAbrir_Click(sender As Object, e As EventArgs) Handles CmdAbrir.Click

        'Abrir fichero INP de Epanet
        With OpenFD
            .ShowReadOnly = True
            .InitialDirectory = System.Environment.CurrentDirectory
            .Filter = "Ficheros inp (*.inp)|*.inp"
            .RestoreDirectory = True
            .Title = "Seleccionar fichero Inp"
            If .ShowDialog() = DialogResult.OK Then TxtINP.Text = .FileName
        End With
        TxtINP.SelectionStart = Len(TxtINP.Text)

    End Sub

    Private Sub CmdAceptar_Click(sender As Object, e As EventArgs) Handles CmdAceptar.Click

        Dim DirInp As String
        Dim DirRpt As String
        Dim DirOut As String

        'Almacenar rutas de los ficheros .inp, .rpt, .out
        DirInp = TxtINP.Text
        DirRpt = DirInp.Substring(0, Len(DirInp) - 4) & ".rpt"
        DirOut = DirInp.Substring(0, Len(DirInp) - 4) & ".out"

        'Llamar procedimiento leer fichero Inp
        LeerInp_RecupCaudales(DirInp, DirRpt, DirOut)

        'Llamar procedimiento escribir fichero inp
        If numTubmalTrazadas > 0 Then
            EscribirFicheroInp(DirInp, DirRpt, DirOut)
            EscribirFicheroTexto(DirInp)
            MessageBox.Show("Existe " + CStr(numTubmalTrazadas) + " línea(s) mal trazada(s)", "Informe")
        Else
            MessageBox.Show("No existe líneas mal trazadas", "Informe")
        End If

    End Sub

    Private Sub LeerInp_RecupCaudales(pathInp As String, pathRpt As String, pathOut As String)
        'Almacenar información de la sección [PIPES], [COORDENATES], [VERTICES], [TIMES]
        'Recuperar los caudales de las tuberías en todo el periodo de simulación

        Dim i, Err, tt, ttt, L As Integer
        Dim indN1, indN2 As Integer
        Dim valor As Single
        Dim ID As New StringBuilder(32)
        Dim IDN1, IDN2 As New StringBuilder(32)

        Err = ENopen(pathInp, pathRpt, pathOut)
        Err = ENgetcount(EN_LINKCOUNT, numLineas)
        IDTubmalTrazadas = New Collection

        'Dimensionar el vector Linea
        ReDim Linea(numLineas)

        'Recorrer todas las líneas y obtener información
        For L = 1 To numLineas

            'Instancear objetos
            Linea(L).ListaQ = New Collection
            Linea(L).VertX = New Collection
            Linea(L).VertY = New Collection

            'Recuperar el identificador
            Err = ENgetlinkid(L, ID)
            Linea(L).ID = ID.ToString

            'Recuperar el nudo inicial y final
            Err = ENgetlinknodes(L, indN1, indN2)
            Err = ENgetnodeid(indN1, IDN1)
            Err = ENgetnodeid(indN2, IDN2)
            Linea(L).IDN1 = IDN1.ToString
            Linea(L).IDN2 = IDN2.ToString

            'Recuperar la longitud
            Err = ENgetlinkvalue(L, EN_LENGTH, valor)
            Linea(L).Longitud = SepDec(valor)

            'Recuperar el diámetro
            Err = ENgetlinkvalue(L, EN_DIAMETER, valor)
            Linea(L).Diametro = SepDec(valor)

            'Recuperar la rugosidad
            Err = ENgetlinkvalue(L, EN_ROUGHNESS, valor)
            Linea(L).Rugosidad = SepDec(valor)

            'Recuperar las pérdidas menores
            Err = ENgetlinkvalue(L, EN_MINORLOSS, valor)
            Linea(L).PerdMen = SepDec(valor)

            'Recuperar el estado inicial
            Err = ENgetlinkvalue(L, EN_INITSTATUS, valor)
            If valor = 0 Then Linea(L).EstadoIni = "Closed"
            If valor = 1 Then Linea(L).EstadoIni = "Open"
            Err = ENgetlinktype(L, valor)
            If valor = EN_CVPIPE Then Linea(L).EstadoIni = "CV"

        Next

        'Realizar cálculo hidráulico y obtener el caudal
        Err = ENopenH : If Err <> 0 Then Stop
        Err = ENinitH(1) : If Err <> 0 Then Stop
        Do
            Err = ENrunH(tt) : If Err > 6 Then Stop
            For L = 1 To numLineas
                Err = ENgetlinkvalue(L, EN_FLOW, valor)
                Linea(L).ListaQ.Add(valor)
            Next L
            Err = ENnextH(ttt)
        Loop Until ttt = 0
        Err = ENcloseH

        'Identificar las tuberías si se cumple la condición de: Q < 0 en todo el periodo
        Dim Invertir As Boolean
        Dim Q As Single

        numTubmalTrazadas = 0
        For L = 1 To numLineas
            Invertir = True
            For i = 1 To Linea(L).ListaQ.Count
                Q = Linea(L).ListaQ.Item(i)
                If Q >= 0 Then Invertir = False
            Next i
            If Invertir = True Then
                Linea(L).InvertNudos = True
                numTubmalTrazadas = numTubmalTrazadas + 1
                IDTubmalTrazadas.Add(Linea(L).ID)
            End If
        Next L

        'Recuperar los vértices de las líneas
        Dim SeccActual As String = vbNullString
        Dim IDLinea As String = vbNullString
        Dim VertX As String = vbNullString
        Dim VertY As String = vbNullString
        Dim pos, iToken As Int32
        Dim subStr() As String
        Dim indLinea As Integer
        Dim line As String
        Dim fr As FileStream
        Dim sr As StreamReader

        fr = New FileStream(pathInp, FileMode.Open)
        sr = New StreamReader(fr, Encoding.Default)

        While Not sr.EndOfStream

            line = sr.ReadLine()
            line = Trim(Replace(line, Chr(9), " "))

            If Len(line) > 0 Then
                If line.Substring(0, 1) = "[" Then
                    SeccActual = UCase(line)
                ElseIf line.Substring(0, 1) <> ";" Then
                    Select Case SeccActual
                        Case "[VERTICES]"
                            pos = InStr(line, ";")
                            If pos > 0 Then line = line.Substring(0, pos - 1)
                            line = Trim(Replace(line, Chr(9), " "))
                            subStr = Split(line)
                            iToken = 0
                            For i = 0 To UBound(subStr)
                                If subStr(i) <> "" Then
                                    iToken = iToken + 1
                                    If iToken = 1 Then IDLinea = subStr(i)
                                    If iToken = 2 Then VertX = subStr(i)
                                    If iToken = 3 Then VertY = subStr(i)
                                End If
                            Next i
                            Err = ENgetlinkindex(IDLinea, indLinea)
                            Linea(indLinea).VertX.Add(VertX)
                            Linea(indLinea).VertY.Add(VertY)
                    End Select
                End If
            End If
        End While
        sr.Close()
        fr.Dispose()

        'Cerrar fichero Inp y liberar memoria
        Err = ENclose()

    End Sub

    Private Sub EscribirFicheroInp(pathInp As String, pathRpt As String, pathOut As String)
        'Escribir nuevo fichero Inp con los nuevos cambios

        Dim pathResult As String
        Dim fw As FileStream
        Dim fr As FileStream
        Dim sr As StreamReader
        Dim sw As StreamWriter
        Dim line, seccActual As String
        Dim i, j As Integer
        Dim Err, valor As Integer

        Dim nTuberias As Integer
        nTuberias = 0

        pathResult = pathInp.Substring(0, Len(pathInp) - 4) & "_Result.inp"
        seccActual = vbNullString

        fr = New FileStream(pathInp, FileMode.Open)
        sr = New StreamReader(fr, Encoding.Default)

        If File.Exists(pathResult) Then File.Delete(pathResult)
        fw = New FileStream(pathResult, FileMode.CreateNew)
        sw = New StreamWriter(fw, Encoding.Default)

        Err = ENopen(pathInp, pathRpt, pathOut)

        While Not sr.EndOfStream

            line = sr.ReadLine()
            line = Trim(Replace(line, Chr(9), " "))

            If Len(line) > 0 Then

                If line.Substring(0, 1) = "[" Then seccActual = line

                If seccActual <> "[PIPES]" And seccActual <> "[VERTICES]" Then sw.WriteLine(line)

                If seccActual = "[PIPES]" Then
                    If line.Substring(0, 1) <> "[" And line.Substring(0, 1) <> ";" Then
                        For i = 1 To numLineas
                            Err = ENgetlinktype(i, valor)
                            If valor = EN_PIPE Or valor = EN_CVPIPE Then
                                nTuberias = nTuberias + 1
                                If Linea(i).InvertNudos = False Then
                                    sw.WriteLine(Linea(i).ID & Space(16) & Linea(i).IDN1 & Space(16) & Linea(i).IDN2 & Space(16) &
                                             Linea(i).Longitud & Space(16) & Linea(i).Diametro & Space(16) &
                                             Linea(i).Rugosidad & Space(16) & Linea(i).PerdMen & Space(16) & Linea(i).EstadoIni)
                                Else
                                    sw.WriteLine(Linea(i).ID & Space(16) & Linea(i).IDN2 & Space(16) & Linea(i).IDN1 & Space(16) &
                                             Linea(i).Longitud & Space(16) & Linea(i).Diametro & Space(16) &
                                             Linea(i).Rugosidad & Space(16) & Linea(i).PerdMen & Space(16) & Linea(i).EstadoIni)
                                End If
                                line = sr.ReadLine()
                            End If
                        Next
                        sw.WriteLine("")
                    Else
                        sw.WriteLine(line)
                    End If
                End If

                If seccActual = "[VERTICES]" Then
                    If line.Substring(0, 1) <> "[" And line.Substring(0, 1) <> ";" Then
                        For i = 1 To numLineas
                            Err = ENgetlinktype(i, valor)
                            If valor = EN_PIPE Then
                                If Linea(i).InvertNudos = True Then
                                    For j = Linea(i).VertX.Count To 1 Step -1
                                        sw.WriteLine(Linea(i).ID & Space(16) & Linea(i).VertX(j) & Space(16) & Linea(i).VertY(j))
                                        line = sr.ReadLine()
                                    Next
                                ElseIf Linea(i).InvertNudos = False Then
                                    For j = 1 To Linea(i).VertX.Count
                                        sw.WriteLine(Linea(i).ID & Space(16) & Linea(i).VertX(j) & Space(16) & Linea(i).VertY(j))
                                        line = sr.ReadLine()
                                    Next
                                End If
                            End If
                            If valor <> EN_PIPE Then
                                For j = 1 To Linea(i).VertX.Count
                                    sw.WriteLine(Linea(i).ID & Space(16) & Linea(i).VertX(j) & Space(16) & Linea(i).VertY(j))
                                    line = sr.ReadLine()
                                Next
                            End If
                        Next
                        sw.WriteLine("")
                    Else
                        sw.WriteLine(line)
                    End If
                End If
            Else
                sw.WriteLine(line)
            End If

        End While

        Err = ENclose()

        sw.Close()
        fw.Dispose()
        sr.Close()
        fr.Dispose()

    End Sub

    Private Sub EscribirFicheroTexto(pathInp As String)

        Dim sw As StreamWriter
        Dim i As Integer
        Dim pathTxt As String

        pathTxt = pathInp.Substring(0, Len(pathInp) - 4) & "_IDTuberias.txt"


        sw = New StreamWriter(pathTxt)
        sw.WriteLine("=========================================")
        sw.WriteLine("Identificadores de tuberías mal trazadas")
        sw.WriteLine("=========================================")
        sw.WriteLine(pathInp)
        sw.WriteLine("")
        sw.WriteLine("ID Tubería")
        sw.WriteLine("-----------")
        For i = 1 To IDTubmalTrazadas.Count
            sw.WriteLine(IDTubmalTrazadas.Item(i))
        Next
        sw.Close()

    End Sub

    Private Function SepDec(valor As String) As String

        Dim i As Int32

        i = InStr(valor, ",")

        If i > 0 Then
            SepDec = valor.Replace(",", ".")
        Else
            SepDec = valor
        End If

    End Function

End Class
