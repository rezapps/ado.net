using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;


class Program
{

    static void Main(string[] args)
    {
        // Configuration for appsettings
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (connectionString == null)
        {
            Console.WriteLine("Connection string is null.");
            return;
        }

        // New Address Properties
        int addressID = 11384;
        string newAddressLine1 = "Main ST, 1111";
        string newCity = "New City";
        string newStateProvince = "New State";
        string newCountryRegion = "New Country";
        string newPostalCode = "12345";

        // Calling Async CRUD Methods
        CreateAddressAsync(connectionString, newAddressLine1, newCity, newStateProvince, newCountryRegion, newPostalCode).Wait();
        ReadAddressesAsync(connectionString).Wait();
        UpdateAddressAsync(connectionString, addressID, newAddressLine1, newCity, newStateProvince, newCountryRegion, newPostalCode).Wait();
        ReadAddressesAsync(connectionString).Wait();
        DeleteAddress(connectionString, addressID);
        ReadAddressesAsync(connectionString).Wait();
    }


    // Create Address
    static async Task CreateAddressAsync(string connectionString, string newAddressLine1, string newCity, string newStateProvince, string newCountryRegion, string newPostalCode)
    {
        Guid rowGuid = Guid.NewGuid();
        DateTime modifiedDate = DateTime.Now;

        string sql = "INSERT INTO SalesLT.Address (AddressLine1, City, StateProvince, CountryRegion, PostalCode, rowguid, ModifiedDate) " +
                 "VALUES (@addressLine1, @city, @stateProvince, @countryRegion, @postalCode, @rowguid, @modifiedDate)";

        try
        {
            using SqlConnection cnn = new(connectionString);
            {
                await cnn.OpenAsync();
                var cmd = new SqlCommand(sql, cnn);
                var command = cmd.Parameters.AddWithValue;

                command("@addressLine1", newAddressLine1);
                command("@city", newCity);
                command("@stateProvince", newStateProvince);
                command("@countryRegion", newCountryRegion);
                command("@postalCode", newPostalCode);
                command("@rowguid", rowGuid);
                command("@modifiedDate", modifiedDate);

                int rowsAffected = await cmd.ExecuteNonQueryAsync();

                if (rowsAffected == 1)
                {
                    Console.WriteLine("Address created successfully.");
                }
                else
                {
                    Console.WriteLine($"No rows affected (expected 1). Update might have failed.");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    // Read Addresses
    static async Task ReadAddressesAsync(string connectionString)
    {
        string sql = "SELECT * FROM SalesLT.Address";

        try
        {
            using SqlConnection cnn = new(connectionString);
            {
                await cnn.OpenAsync();

                using SqlCommand cmd = new(sql, cnn);
                using SqlDataReader reader = await cmd.ExecuteReaderAsync();
                while (reader.Read())
                {
                    Console.WriteLine($"ID: {reader["AddressID"]}, Street: {reader["AddressLine1"]}, City: {reader["City"]}, State: {reader["StateProvince"]}, Country: {reader["CountryRegion"]}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

    }

    // Update Address
    static async Task UpdateAddressAsync(string connectionString, int addressID, string newAddressLine1, string newCity,
                                      string newStateProvince, string newCountryRegion, string newPostalCode)
    {
        string sql = "UPDATE SalesLT.Address " +
                    "SET AddressLine1 = @addressLine1, City = @city, StateProvince = @stateProvince, " +
                    "CountryRegion = @countryRegion, PostalCode = @postalCode, ModifiedDate = GETUTCDATE() " +
                    "WHERE AddressID = @addressID";

        try
        {
            using SqlConnection cnn = new(connectionString);
            {
                await cnn.OpenAsync();

                var cmd = new SqlCommand(sql, cnn);
                var command = cmd.Parameters.AddWithValue;

                command("@addressLine1", newAddressLine1);
                command("@city", newCity);
                command("@stateProvince", newStateProvince);
                command("@countryRegion", newCountryRegion);
                command("@postalCode", newPostalCode);
                command("@addressID", addressID);

                int rowsAffected = await cmd.ExecuteNonQueryAsync();

                if (rowsAffected == 1)
                {
                    Console.WriteLine("Address updated successfully.");
                }
                else
                {
                    Console.WriteLine($"No rows affected (expected 1). Update might have failed.");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating address: {ex.Message}");
        }
    }

    // Delete Address
    static void DeleteAddress(string connectionString, int addressID)
    {
        string sql = "DELETE FROM SalesLT.Address WHERE AddressID = @addressID";

        try
        {
            using SqlConnection cnn = new(connectionString);
            cnn.Open();

            using SqlCommand cmd = new(sql, cnn);
            cmd.Parameters.AddWithValue("@addressID", addressID);

            int rowsAffected = cmd.ExecuteNonQuery();

            if (rowsAffected == 1)
            {
                Console.WriteLine("Address deleted successfully.");
            }
            else
            {
                Console.WriteLine($"No rows affected (expected 1). Deletion might have failed.");
            }
        }
        catch (SqlException ex)
        {
            Console.WriteLine($"Database error: {ex.Message}");
        }
    }

}
