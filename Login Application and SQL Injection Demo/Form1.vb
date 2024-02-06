Imports System.Data.SqlClient

Public Class Form1

    'Adding this comment to trigger a pull request and scan the code

    Private Sub btnLogin_Click(sender As Object, e As EventArgs) Handles btnLogin.Click

        'Connect to the database
        Dim objConnection As New SqlConnection("server=localhost\sqlexpress;database=users;user id=sa;password=password") 'Change to your actual password!
        Dim objPasswordsDataAdapter As SqlDataAdapter
        Dim objPasswordDataTable As New DataTable

        Dim userid As String = txtUsername.Text
        Dim password As String = txtPassword.Text

        Dim getPasswordSQL As String = "SELECT userid, username FROM userdata WHERE userid = '" + userid + "' AND password = '" + password + "'"

        'This line is to show the SQL string sent to the database, to help you build malicious SQL.
        'While it's useful to understand this application's behavior, ** don't ** do this in a real application!
        Console.WriteLine("SQL string sent to DB:")
        Console.WriteLine(getPasswordSQL)

        Try

            objPasswordsDataAdapter = New SqlDataAdapter(getPasswordSQL, objConnection)
            objPasswordsDataAdapter.Fill(objPasswordDataTable)

            If objPasswordDataTable.Rows.Count = 0 Then    'No rows? Either username or password or both are incorrect.

                lblResult.Text = "Access Denied "
                Console.WriteLine("No rows returned = access denied")

            Else

                'Otherwise, access granted! Welcome user by name.
                Dim username As String = CStr(objPasswordDataTable.Rows(0).Item(1))
                lblResult.Text = "Access Granted." + vbNewLine + username + ", welcome to the password protected area!"

                'Again, check the Console. This is just to help understand the application's behavior.
                Console.WriteLine("Rows returned from the database:")
                For Each row As DataRow In objPasswordDataTable.Rows
                    For Each item As Object In row.ItemArray
                        Console.Write(item)
                    Next
                    Console.WriteLine()
                Next
                Console.WriteLine()

            End If


        Catch sq As SqlException
            MessageBox.Show("Error connecting to database " + sq.Message)

        Catch ex As Exception
            MessageBox.Show("Error occurred : " + ex.Message)

        End Try

    End Sub

    Public Sub DoSomething()

        Dim Hres1 As Integer = CoSetProxyBlanket(Nothing, 0, 0, Nothing, 0, 0, IntPtr.Zero, 0) ' Noncompliant
        Dim Hres2 As Integer = CoInitializeSecurity(IntPtr.Zero, -1, IntPtr.Zero, IntPtr.Zero, RpcAuthnLevel.None, RpcImpLevel.Impersonate, IntPtr.Zero, EoAuthnCap.None, IntPtr.Zero) ' Noncompliant

    End Sub

'yes

End Class

