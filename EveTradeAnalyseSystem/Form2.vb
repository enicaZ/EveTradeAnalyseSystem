Imports System.Threading

Public Class Form2
    Dim mythread As Thread
    Private Delegate Sub VoidShow(ByRef i As Int32) '定义要委托的类型  

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        mythread = New Thread(AddressOf ShowNumber)
        mythread.Name = "myShowNumber"
        mythread.Start()
    End Sub

    Private Sub ShowNumber()
        Dim i As Int32
        For i = 0 To 123451
            'TextBox1.Text = i  
            Me.Invoke(New VoidShow(AddressOf TureShowNumber), i) '用New构造委托，再用Invoke执行  
        Next

        mythread.Abort()
    End Sub

    '新加入的被委托要做的事  
    Private Sub TureShowNumber(ByRef i As Int32)
        TextBox1.Text = i
    End Sub


    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        TextBox2.Text = "终于出现奇迹"
    End Sub
End Class