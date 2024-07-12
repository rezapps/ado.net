using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;


class Program
{
  private static string? connectionString;

  static void Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        connectionString = configuration.GetConnectionString("DefaultConnection");

        Console.WriteLine($"Default Connection String: {connectionString}");


        
        CreateRecord();
        ReadRecords();
        UpdateRecord();
        DeleteRecord();
        ReadRecords();
    }

    static void CreateRecord()
    {
        using SqlConnection connection = new(connectionString);
        connection.Open();
        string query = "INSERT INTO SalesLT.Customer (FirstName, LastName, EmailAddress) VALUES (@firstName, @lastName, @EmailAddress)";
        using SqlCommand command = new(query, connection);
        command.Parameters.AddWithValue("@firstName", "John");
        command.Parameters.AddWithValue("@lastName", "Doe");
        command.Parameters.AddWithValue("@EmailAddress", "john.doe@example.com");
        int result = command.ExecuteNonQuery();
        Console.WriteLine(result > 0 ? "Record created successfully" : "Error creating record");
    }

    static void ReadRecords()
    {
        using SqlConnection connection = new(connectionString);
        connection.Open();
        string query = "SELECT * FROM SalesLT.Customer";
        using SqlCommand command = new(query, connection);
        using SqlDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            Console.WriteLine($"{reader["CustomerID"]}, {reader["FirstName"]}, {reader["LastName"]}, {reader["EmailAddress"]}");
        }
    }

    static void UpdateRecord()
    {
        using SqlConnection connection = new(connectionString);
        connection.Open();
        string query = "UPDATE SalesLT.Customer SET EmailAddress = @EmailAddress WHERE FirstName = @firstName AND LastName = @lastName";
        using SqlCommand command = new(query, connection);
        command.Parameters.AddWithValue("@firstName", "John");
        command.Parameters.AddWithValue("@lastName", "Doe");
        command.Parameters.AddWithValue("@EmailAddress", "john.updated@example.com");
        int result = command.ExecuteNonQuery();
        Console.WriteLine(result > 0 ? "Record updated successfully" : "Error updating record");
    }

    static void DeleteRecord()
    {
        using SqlConnection connection = new(connectionString);
        connection.Open();
        string query = "DELETE FROM SalesLT.Customer WHERE FirstName = @firstName AND LastName = @lastName";
        using SqlCommand command = new(query, connection);
        command.Parameters.AddWithValue("@firstName", "John");
        command.Parameters.AddWithValue("@lastName", "Doe");
        int result = command.ExecuteNonQuery();
        Console.WriteLine(result > 0 ? "Record deleted successfully" : "Error deleting record");
    }
}
