# vb-sql-injection

# vb-sql-injection
**User login application which is vulnerable to SQL injection**

SQL injection attacks often happen when applications don't screen user input before they send it to a database. If a user can send commands to the database, and the database executes these commands, then all kinds of bad things can happen.

This application uses typical SQL login code to demonstrate. 

Run these SQL queries to set up a database,

    use users
    go
  
    create table userdata
    (userid varchar(50) primary key,
    username varchar(50) not null, 
    password varchar(50) not null,); 
  
    insert into userdata values ('admin', 'Abby Admin', 'kitten'),
    ('bob', 'Bob Mould', 'qwerty'),
    ('ted', 'Ted "Theodore" Logan', 'topsecret'),
    ('bill', 'Bill S. Preston', 'letmein'),
    ('will', 'Will Smith', '123456')

Then run the application. If you enter a valid username and password, you should see an 'access granted' message. If not, then access is denied. Seems ok?

If you look at the code, you'll see that user input is being used directly in a SQL query, in this line,

          Dim getPasswordSQL As String = "select userid, username from userdata where userid = '" + userid + "' and password = '" + password + "'"

Try entering this data for both the username and password. 

     whatever' or '1'='1
  
Are you logged in? Who as? 

Since you just created the database, you know that there is no user called `whatever' or '1'='1`. So what happened? Let's look at the SQL string that your application built using this data.

    select userid, username from userdata where userid = 'whatever' or '1'='1' and password = '1'='1' 

'1' is always equal to '1' so all of the rows are returned. Your program assumes 1 or 0 rows will be returned, so if the number of rows is not 0, then assumes the username and password were correct, it reads the username from the first row and displays the welcome message. To make matters worse, the administrator is usually the first row in the database. This is an example of **SQL injection**.

**Note** The program prints messages to the Output window so you can see the query being sent to the database and the rows returned. **Don't** do this in a real application!

In addition to logging in as admin, there's more you can do with SQL injection.

Even though we can gain access to the admmin account, we don't actually know the admin's password. With a little trial-and-error, we can discover the password. Try typing `admin` for the username and this for the password

    ' or password like 'a%

Put that into the SQL string, and you are asking to be logged in if our admin's password starts with `a`. 
Now, try this for the password,

      ' or password like 'k%

Sucess! We now know our admin's password starts with `k`. An attacker can use trial and error to figure out all of the characters in the password. 

    ' or password like 'ka%
    ' or password like 'kb%
    ' or password like 'kc%
    ....

It might take a while, but admin access to a server or access to a whole database full of credit card numbers is worth the effort. (And anyone smart enough to use SQL injection is probably smart enough to write a script that will do the hard work for them... or download one of the many SQL injection tools freely available on the internet.)

Maybe we'd like to create a new account for ourselves. Enter any username and this password

    ' ; insert into userdata values ('evil', 'New Evil User', 'password'); select * from userdata where '1'='1

And then you should be able to log in with the username `evil` and password `password`. 

The `;` signifies the end of a SQL command, so you can add your own SQL command afterwards. Remember to add a SQL statement at the end that can use the final `'` that your code is adding to the SQL.

And how about simply deleting the whole database? Enter anything for the username and this for the password,

    ' ; drop table userdata  ; select * from userdata where '1'='1

You'll see the Access Denied message, but if you try and log in with a valid account you'll see an error that the database table doesn't exist any more. 

This is just the tip of the iceberg of SQL injection. There are many more variations which can be used to discover your database table names, columns, and all of the data within. If you don't filter user input, a malicious user can potentially read all of your data and/or destroy your database. Think about databases of usernanmes and passwords, or credit card data, or names and social security numbers; for example LinkedIn (millions of username and passwords stolen) or the Heartland data breach (millions of credit cards stolen), and many more...

SQL injection hall of shame (Code Curmudgeon): http://codecurmudgeon.com/wp/sql-injection-hall-of-shame/

The fix:

An important part of prevention is to use parameterized queries. Replace the btnLogin_Click method code with the following:

    Private Sub btnLogin_Click(sender As Object, e As EventArgs) Handles btnLogin.Click
        Dim objConnection As New SqlConnection("server=localhost\sqlexpress;database=users;user id=sa;password=password") 'Change to your actual password!
        Dim objPasswordsDataAdapter As SqlDataAdapter
        Dim objPasswordDataTable As New DataTable

        Dim userid As String = txtUsername.Text
        Dim password As String = txtPassword.Text

        Dim getPasswordSQL As String = "select userid, username from userdata where userid = @userid and password=@password"
        Dim getPasswordSSQLCommand = New SqlCommand(getPasswordSQL, objConnection)

        getPasswordSSQLCommand.Parameters.AddWithValue("@userid", userid)
        getPasswordSSQLCommand.Parameters.AddWithValue("@password", password)

        Try
            objPasswordsDataAdapter = New SqlDataAdapter(getPasswordSSQLCommand)
            objPasswordsDataAdapter.Fill(objPasswordDataTable)

            If objPasswordDataTable.Rows.Count = 0 Then    'No rows? Either username or password or both are incorrect.
                lblResult.Text = " Access Denied "
            Else
                Dim username As String = CStr(objPasswordDataTable.Rows(0).Item(1))
                lblResult.Text = username + ", welcome to the password protected area!"
            End If

            'Debugging - look in the Output window
            For Each row As DataRow In objPasswordDataTable.Rows
                For Each item As Object In row.ItemArray
                Next
            Next

           
        Catch sq As SqlException
            MessageBox.Show("Error connecting to database " + sq.Message)
        Catch ex As Exception
            MessageBox.Show("Error occurred : " + ex.Message)
        End Try

    End Sub

and try some of the evil SQL again. Does it work now?  

Parameterizing means that anything the user enters is always treated as data, never SQL. So, you should never trust users to enter good input, and screen, filter and parameterize all user input before it is sent to your precious database. 
