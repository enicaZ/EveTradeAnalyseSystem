Imports System.Data.SQLite
Imports System.IO
Imports System.Net
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports System.Net.Http
Imports System.Text

Public Class claEmployee
    Public Property average() As Double
    Public Property [date]() As String
    Public Property highest() As Double
    Public Property lowest() As Double
    Public Property order_count() As Int64
    Public Property volume() As Int64
End Class

Public Class Form1
    Public Structure Employee
        'average DOUBLE,date DateTime,highest DOUBLE,lowest DOUBLE,order_count INTEGER,volume INTEGER

        Dim average As Double
        Dim [date] As Date
        Dim highest As Double
        Dim lowest As Double
        Dim order_count As Int64
        Dim volume As Int64

    End Structure

    Dim conn As SQLiteConnection

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click

        If System.IO.File.Exists("db\test.db3") = True Then

            System.IO.File.Delete("db\test.db3")

        End If

        SQLiteConnection.CreateFile("db\test.db3")

        conn = New SQLiteConnection("Data Source=db\test.db3;Pooling=true;FailIfMissing=false")

        If conn.State <> ConnectionState.Open Then

            conn.Open()

            'MsgBox("打开成功！")

        End If

        Dim stopwatch As New Stopwatch

        stopwatch.Start()

        Dim cmd As New SQLiteCommand

        cmd.Connection = conn

        'cmd.CommandText = "CREATE TABLE Test (ID INTEGER PRIMARY KEY,TestName VARCHAR(500),TestTime DateTime,Operator VARCHAR(100))"



        cmd.CommandText = "CREATE TABLE  IF NOT EXISTS history (typeIDs INTEGER,average DOUBLE,date DateTime,highest DOUBLE,lowest DOUBLE,order_count INTEGER,volume INTEGER)"

        Dim result As Integer = cmd.ExecuteNonQuery()


        If result = 0 Then

            'MsgBox("成功")   d
            'MsgBox(stopwatch.ElapsedMilliseconds & "毫秒")

        Else


            MsgBox("失败")

        End If

        stopwatch.Start()
        Dim urlStr As String
        Dim dataStr As String
        urlStr = "https://esi.evetech.net/latest/markets/10000002/history/"
        dataStr = "datasource=tranquility&type_id="
        Dim idstr As String = "44992"
        Dim tempstr As String = urlStr + "?" + dataStr + idstr


        Dim jsonStr2 = GetDataAsync(urlStr, dataStr + idstr)
        Dim testst = jsonStr2.Value
        'Dim jsonStr2 As String = webCaptureContent(tempstr, True)
        'Dim jsontest = Task(Of String).Factory.StartNew(GetDataAsync(urlStr, dataStr + idstr))
        'Dim jsonStr As Task(Of String) = GetDataAsync(urlStr, dataStr)
        Dim jsonRe As JArray = JArray.Parse(jsonStr2.ToString())
        'Dim jsonData As JObject = CType(JsonConvert.DeserializeObject(jsonRe(0)), JObject)
        'Dim jsonResult As JObject = JObject.Parse(jsonStr)
        stopwatch.Stop()

        MsgBox(stopwatch.ElapsedMilliseconds & "毫秒")

        Dim emp As List(Of Employee)
        emp = JsonConvert.DeserializeObject(Of List(Of Employee))(jsonRe.ToString)

        Dim Items = JsonConvert.DeserializeObject(Of claEmployee())(jsonRe.ToString)
        cmd = conn.CreateCommand()
        cmd.Parameters.Add(cmd.CreateParameter())
        'DbTransaction trans = dbConn.BeginTransaction();



        stopwatch.Restart()
        Dim trans As SQLiteTransaction = conn.BeginTransaction()

        For i As Integer = 0 To emp.Count - 1
            'CREATE TABLE history (ID Integer PRIMARY KEY, typeIDs Integer, average Double, date DateTime, highest Double, lowest Double, order_count Integer, volume INTEGER)
            cmd.CommandText = "insert into history(typeIDs,average,date,highest,lowest,order_count,volume)values(@typeIDs,@average,@date,@highest,@lowest,@order_count,@volume)"
            cmd.Parameters.Add("@typeIDs", Data.DbType.Int64).Value = idstr
            cmd.Parameters.Add("@average", Data.DbType.Double).Value = emp(i).average
            cmd.Parameters.Add("@date", Data.DbType.DateTime).Value = emp(i).[date]
            cmd.Parameters.Add("@highest", Data.DbType.Double).Value = emp(i).highest
            cmd.Parameters.Add("@lowest", Data.DbType.Double).Value = emp(i).lowest
            cmd.Parameters.Add("@order_count", Data.DbType.Int64).Value = emp(i).order_count
            cmd.Parameters.Add("@volume", Data.DbType.Int64).Value = emp(i).volume

            result = cmd.ExecuteNonQuery()
        Next

        trans.Commit()

        stopwatch.Stop()

        If result <> 0 Then

            MsgBox("插入成功,用时：" & stopwatch.ElapsedMilliseconds & "毫秒")

        End If

        SelectShowInfo()


        'MsgBox("插入成功,用时：" & stopwatch.ElapsedMilliseconds & "毫秒")
        ''

        'cmd = conn.CreateCommand()

        'cmd.CommandText = "update Test set TestName='动静1'"

        'result = cmd.ExecuteNonQuery()

        'If result <> 0 Then

        '    MsgBox("更新成功")

        'End If

        'SelectShowInfo()



        ''



        'cmd = conn.CreateCommand()

        'cmd.CommandText = "delete from Test"

        'result = cmd.ExecuteNonQuery()

        'If result <> 0 Then

        '    MsgBox("删除成功")

        'End If

        'SelectShowInfo()



        cmd.Dispose()



        If conn.State = ConnectionState.Open Then

            conn.Close()

        End If

    End Sub
    Public Function GetDataAsync(ByVal url As String, ByVal data As String) As JsonTextReader
        Dim request As HttpWebRequest = WebRequest.Create(url + "?" + data)
        request.ServicePoint.ConnectionLimit = 1024
        Dim result1
        request.Method = "GET"
        ' Dim jsonReader = New JsonTextReader
        'Using sr As StreamReader = New StreamReader(request.GetResponse().GetResponseStream)
        '    result = sr.ReadToEndAsync().Result
        '    sr.Close()
        'End Using

        Dim sr As StreamReader = New StreamReader(request.GetResponse().GetResponseStream)

        Dim jsonRe = New JsonTextReader（sr)

        Dim test = New JTokenReader(JToken.ReadFrom(jsonRe))

        Dim jsonSer = New JsonSerializer

        Dim jsonRecordtemp = jsonSer.Deserialize(Of List(Of Employee))(test)





        Dim fsfd = test.Value



        While (jsonRe.Read())

            If jsonRe.Value <> Nothing Then

                Console.WriteLine("Token: {0}, Value: {1}", jsonRe.TokenType, jsonRe.Value)

            Else

                Console.WriteLine("Token: {0}", jsonRe.TokenType)
            End If
        End While
        'JsonReader
        'Dim JsonOje As JObject = New JObject(JArray.ReadFrom(jsonReader))
        'Dim jsontoken As JToken = New JToken(jsonReader.Value)

        'Dim result = Await sr.ReadToEndAsync()
        'result1 = result.ToString()


        Return jsonRe
    End Function
    Public Function webCaptureContent(ByVal mWebsiteUrl As String, ByVal mWebsiteType As Boolean) As String
        '启动一次具体的数据采集工作,返回采集到的HTML内容:要求必须输入带://的全地址数据
        On Error Resume Next
        Dim Str_WebContent As String = "请输入查找网站地址."
        Dim wb As WebClient = New WebClient() '//创建一个WebClient实例
        If mWebsiteUrl.IndexOf("://") > 0 Then
            '//获取或设置用于对向 Internet 资源的请求进行身份验证的网络凭据。（可有可无）
            wb.Credentials = CredentialCache.DefaultCredentials
            '//从资源下载数据并返回字节数组。（加@是因为网址中间有"/"符号）
            Dim pagedata As Object = wb.DownloadData(mWebsiteUrl)
            '//转换字符
            If mWebsiteType Then
                Str_WebContent = Encoding.Default.GetString(pagedata)
            Else
                Str_WebContent = Encoding.UTF8.GetString(pagedata)
            End If
        End If
        Return Str_WebContent '提取出来新闻内容,删除Body前后的多余内容,同时补充上该 Body标记,形成完整的内容 Str_WebContent '
    End Function

    ' HttpClient is intended to be instantiated once per application, rather than per-use. See Remarks.


    Public Async Function GetJsonAsync(ByVal url As String, ByVal data As String) As Task(Of String)

        ' Call asynchronous network methods in a try/catch block to handle exceptions.

        Dim client As HttpClient = New HttpClient()
        Dim response As HttpResponseMessage = Await client.GetAsync("https://esi.evetech.net/latest/markets/10000002/history/?datasource=tranquility&type_id=34")
        response.EnsureSuccessStatusCode()

        'Dim responseBody As String = Await response.Content.ReadAsStringAsync()
        ' Above three lines can be replaced with new helper method below
        Dim responseBody As String = Await client.GetStringAsync(url + "?" + data)

        Console.WriteLine(responseBody)
        MsgBox("132213")

    End Function

    Public Sub SelectShowInfo()

        Dim sa As New SQLiteDataAdapter("select * from history", conn)

        Dim ds As New System.Data.DataSet

        sa.Fill(ds, "history")

        Dim mytable As New System.Data.DataTable

        mytable = ds.Tables("history")

        Me.DataGridView1.DataSource = mytable

        Me.DataGridView1.Refresh()

    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        frmMain.Show()
    End Sub
End Class