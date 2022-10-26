using Microsoft.Data.SqlClient;
using System.Data;

string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Database=adonetdb;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

string sql = "select * from Users";
using(SqlConnection conn = new SqlConnection(connectionString))
{
    SqlDataAdapter adapter = new SqlDataAdapter(sql, conn);
    DataSet ds = new DataSet();
    adapter.Fill(ds);

    DataTable tb = ds.Tables[0];
    DataRow new_ = tb.NewRow();
    new_["Name"] = "Rick";
    new_["Age"] = 24;
    tb.Rows.Add(new_);

    tb.Rows[0]["Age"] = 5;

    SqlCommandBuilder com = new SqlCommandBuilder(adapter);
    adapter.Update(ds);
    ds.Clear();

    adapter.Fill(ds);

    foreach  (DataTable dt in ds.Tables)
    {
        foreach (DataColumn column in dt.Columns)
        {
            Console.Write($"{column.ColumnName}\t");
        }
        Console.WriteLine();
        foreach (DataRow row1 in dt.Rows)
        {
            var cells = row1.ItemArray;
            foreach (var item in cells)
            {
                Console.Write($"{item}\t");
            }
            Console.WriteLine();
        }
    }
}
Console.WriteLine();
Console.WriteLine();
DataSet userSet = new DataSet("UsersSet");
DataTable users = new DataTable("Users");

userSet.Tables.Add(users);

DataColumn idColumn = new DataColumn("Id", Type.GetType("System.Int32"));
idColumn.Unique = true;
idColumn.AllowDBNull = false;
idColumn.AutoIncrement = true;
idColumn.AutoIncrementSeed = 1;
idColumn.AutoIncrementStep = 1;

DataColumn nameColumn = new DataColumn("Name", typeof(string));
DataColumn ageColumn = new DataColumn("Age", typeof(int));
ageColumn.DefaultValue = 1;

users.Columns.Add(idColumn);
users.Columns.Add(nameColumn);
users.Columns.Add(ageColumn);

users.PrimaryKey = new DataColumn[] { users.Columns["Id"] };

DataRow row = users.NewRow();
row.ItemArray = new object[] { null, "Tom", 36 };
users.Rows.Add(row);
users.Rows.Add(new object[] { null, "Bob", 29 });

Console.Write("Id\tName\tAge");
Console.WriteLine();
foreach (DataRow r in users.Rows)
{
    foreach (var c in r.ItemArray)
    {
        Console.Write($"{c}\t");
    }
    Console.WriteLine();
}

/*
async void Transaction()
{
    using (SqlConnection conn = new SqlConnection(connectionString))
    {
        await conn.OpenAsync();

        SqlTransaction transaction = conn.BeginTransaction();

        SqlCommand sqlCommand = conn.CreateCommand();

        sqlCommand.Transaction = transaction;
        try
        {
            sqlCommand.CommandText = "insert into Users(Age,Name) values(19,'Vladick')";
            await sqlCommand.ExecuteNonQueryAsync();

            sqlCommand.CommandText = "insert into Users(Age,Name) values(22,'Egor')";
            await sqlCommand.ExecuteNonQueryAsync();

            await transaction.CommitAsync();
            Console.WriteLine("Данные добавлены в таблицу");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            await transaction.RollbackAsync();
        }
    }
}
*/
/*
using (SqlConnection conn = new SqlConnection(connectionString))
{
    await conn.OpenAsync();

    SqlCommand cmd = conn.CreateCommand();
    cmd.CommandType = CommandType.StoredProcedure;

    SqlParameter param = cmd.CreateParameter();
    param.ParameterName = "@name";
    param.Value = "Serega";

    SqlParameter param1 = cmd.CreateParameter();
    param1.ParameterName = "@age";
    param1.Value = "19";

    cmd.CommandText = "sp_InsertUser";

    cmd.Parameters.Add(param);
    cmd.Parameters.Add(param1);

    var id = await cmd.ExecuteScalarAsync();

    Console.WriteLine($"Объект добавлен. Id: {id}");

    cmd.CommandText = "sp_GetUser";
    cmd.Parameters.Clear();
    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
    {
        if (reader.HasRows)
        {
            Console.WriteLine();
            string cl0 = reader.GetName(0);
            string cl1 = reader.GetName(1);
            string cl2 = reader.GetName(2);
            Console.WriteLine($"{cl0}\t{cl1}\t{cl2}");
            while (await reader.ReadAsync())
            {
                byte id_ = reader.GetByte(cl0);
                byte age = reader.GetByte(cl1);
                string name = reader.GetString(cl2);
                Console.WriteLine($"{id_}\t{age}\t{name}");
            }
        }
    }

    cmd.CommandText = "sp_GetAgeRange";
    SqlParameter nameParametr = new SqlParameter("@name", "Vlad");

    SqlParameter minAge = new SqlParameter("@min", DbType.Byte);
    minAge.Direction = ParameterDirection.Output;

    SqlParameter maxAge = new SqlParameter("@max", DbType.Byte);
    maxAge.Direction = ParameterDirection.Output;

    cmd.Parameters.Add(nameParametr);
    cmd.Parameters.Add(minAge);
    cmd.Parameters.Add(maxAge);

    await cmd.ExecuteNonQueryAsync();

    Console.WriteLine($"Имя: {nameParametr.Value}\nМин возраст: {minAge.Value}\nМакс возраст: {maxAge.Value}");
}
*/
/*
async void CreateProc()
{

string proc1 = @"create proc [dbo].[sp_InsertUser]
                    @name nvarchar(20),
                    @age tinyint
                as
                    insert into Users(Name,Age) values(@name,@age)
                    select SCOPE_IDENTITY()
                GO;";

string proc2 = @"create proc [dbo].[sp_GetUser]
                as
                    select * from Users
                GO;";

string proc3 = @"create proc [dbo].[sp_GetAgeRange]
                    @name nvarchar(20),
                    @min tinyint out,
                    @max tinyint out
                as
                    select @min=MIN(Age),@max=MAX(Age) from Users where Name like '%'+@name+'%';";
    using (SqlConnection conn = new SqlConnection(connectionString))
    {
        await conn.OpenAsync();

        SqlCommand cmd = conn.CreateCommand();

        cmd.CommandText = proc1;
        await cmd.ExecuteNonQueryAsync();

        cmd.CommandText = proc2;
        await cmd.ExecuteNonQueryAsync();

        cmd.CommandText = proc3;
        await cmd.ExecuteNonQueryAsync();

        Console.WriteLine("Хранимые процедуры созданы");
    }
}
*/
/*
async void Param()
{
    using (SqlConnection conn = new SqlConnection(connectionString))
    {
        await conn.OpenAsync();
        SqlCommand command = conn.CreateCommand();
        command.CommandText = "insert into Users(Name,Age) values(@name,@age);Set @id=SCOPE_IDENTITY()";

        SqlParameter parameter = new SqlParameter("@name", SqlDbType.NVarChar, 20);
        parameter.Value = "Kostya";

        SqlParameter parameter1 = new SqlParameter("@age", 18);

        SqlParameter parameter2 = new SqlParameter("@id", DbType.Byte);
        parameter2.Direction = ParameterDirection.Output;

        command.Parameters.Add(parameter);
        command.Parameters.Add(parameter1);
        command.Parameters.Add(parameter2);

        int count = await command.ExecuteNonQueryAsync();
        Console.WriteLine($"Добавлено объектов: {count}");
        Console.WriteLine($"Id добавленного объекта: {parameter2.Value}");

        command.CommandText = "select * from Users";

        using (SqlDataReader reader = await command.ExecuteReaderAsync())
        {
            if (reader.HasRows)
            {
                string cl0 = reader.GetName(0);
                string cl1 = reader.GetName(1);
                string cl2 = reader.GetName(2);
                Console.WriteLine($"{cl0}\t{cl1}\t{cl2}");
                while (await reader.ReadAsync())
                {
                    byte id = reader.GetByte(cl0);
                    byte age = reader.GetByte(cl1);
                    string name = reader.GetString(cl2);
                    Console.WriteLine($"{id}\t{age}\t{name}");
                }
            }
        }


    }
}
*/
/*
async void Scalar()
{
    using (SqlConnection conn = new SqlConnection(connectionString))
    {
        await conn.OpenAsync();
        SqlCommand command = conn.CreateCommand();

        command.CommandText = "select Count(*) from Users";
        object count = await command.ExecuteScalarAsync();

        command.CommandText = "select Min(Age) from Users";
        object min = await command.ExecuteScalarAsync();

        command.CommandText = "select Max(Age) from Users";
        object max = await command.ExecuteScalarAsync();

        command.CommandText = "select Avg(Age) from Users";
        object avg = await command.ExecuteScalarAsync();

        Console.WriteLine($"Count: {count}\nMin: {min}\nMax: {max}\nAvg: {avg}");
    }
}
*/
/*
async void Reader()
{
    using (SqlConnection conn = new SqlConnection(connectionString))
    {
        await conn.OpenAsync();
        SqlCommand command = conn.CreateCommand();

        command.CommandText = "select * from Users";
        using (SqlDataReader reader = command.ExecuteReader())
        {
            if (reader.HasRows)
            {
                string columnName1 = reader.GetName(0);
                string columnName2 = reader.GetName(1);
                string columnName3 = reader.GetName(2);

                Console.WriteLine($"{columnName1}\t{columnName2}\t{columnName3}");

                while (await reader.ReadAsync())
                {
                    byte id = reader.GetByte(columnName1);
                    byte age = reader.GetByte(columnName2);
                    string name = reader.GetString(columnName3);
                    Console.WriteLine($"{id}\t{age}\t{name}");
                }
            }
        }
    }
}
*/
/*
async void Querys()
{
    using (SqlConnection connection = new SqlConnection(connectionString))
    {
        await connection.OpenAsync();
        SqlCommand command = connection.CreateCommand();

        command.CommandText = "insert into Users(Name,Age) values ('Vlad',17),('Serega',19)";
        int count = await command.ExecuteNonQueryAsync();
        Console.WriteLine($"Добавлено записей {count}");

        command.CommandText = "update Users set Age=18 where Name='Vlad' and Age=17";
        count = await command.ExecuteNonQueryAsync();
        Console.WriteLine($"Изменено записей {count}");

        command.CommandText = "delete from Users where Name='Serega'";
        count = await command.ExecuteNonQueryAsync();
        Console.WriteLine($"Удалено записей {count}");
    }
}
*/
/*
async void CreateTable()
{
    using (SqlConnection connection = new SqlConnection(connectionString))
    {
        await connection.OpenAsync();
        SqlCommand command = connection.CreateCommand();
        command.CommandText = "create table Users(Id tinyint primary key identity,Age tinyint not null,Name nvarchar(20) not null)";
        await command.ExecuteNonQueryAsync();
        Console.WriteLine("Таблица Users создана");
    }
}
*/
/*
async void CreateDb()
{
    using (SqlConnection sqlConnection = new SqlConnection(connectionString))
    {
        await sqlConnection.OpenAsync();

        SqlCommand command = new SqlCommand();
        command.CommandText = "create database adonetdb";
        command.Connection = sqlConnection;

        await command.ExecuteNonQueryAsync();

        Console.WriteLine("База данных создана");
    }
}
*/
/*
async void Propertys()
{
   
    SqlConnection connection = new SqlConnection(connectionString);
    try
    {
        await connection.OpenAsync();
        Console.WriteLine("Подключение открыто");
        Console.WriteLine($"Access token: {connection.AccessToken}");
        Console.WriteLine($"Can create batch: {connection.CanCreateBatch}");
        Console.WriteLine($"Client connection id: {connection.ClientConnectionId}");
        Console.WriteLine($"Command timeout: {connection.CommandTimeout}");
        Console.WriteLine($"Site: {connection.Site}");
        Console.WriteLine($"Container: {connection.Container}");
        Console.WriteLine($"Connection string: {connection.ConnectionString}");
        Console.WriteLine($"Credentiasl: {connection.Credential}");
        Console.WriteLine($"Database: {connection.Database}");
        Console.WriteLine($"Data source: {connection.DataSource}");
        Console.WriteLine($"{connection.FireInfoMessageEventOnUserErrors}");
        Console.WriteLine($"Packet size: {connection.PacketSize}");
        Console.WriteLine($"Retry logic provider: {connection.RetryLogicProvider}");
        Console.WriteLine($"Server process id: {connection.ServerProcessId}");
        Console.WriteLine($"Server version: {connection.ServerVersion}");
        Console.WriteLine($"State: {connection.State}");
        Console.WriteLine($"Statistics enabled: {connection.StatisticsEnabled}");
        Console.WriteLine($"Workstation id: {connection.WorkstationId}");
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }
    finally
    {
        if (connection.State == ConnectionState.Open)
        {
            await connection.CloseAsync();
            Console.WriteLine("Подключение закрыто");
        }
    }
    Console.WriteLine("Программа завершила свою работу");
}
*/