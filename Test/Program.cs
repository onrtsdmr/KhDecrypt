// See https://aka.ms/new-console-template for more information

using Test;
using System.Collections;
using MySql.Data.MySqlClient;
const string connectionString = "server=localhost;user=root;password=;database=sifiratikdb;";
var tables = new ArrayList()
{
    "sicil_kartlari",
    "usr_users"
};
var crypto = new Crypto();

var tablesColumns = new ArrayList()
{
    new ArrayList()
    {
        7,
        8,
        9,
        10,
        12,
        29
    },
    
    new ArrayList()
    {
        7,
        8,
        9,
        10,
        11,
        15
    }
};

var tablesColumns2 = new ArrayList()
{
    new ArrayList()
    {
        "NAME",
        "SURNAME",
        "CARD_ID",
        "PHONE",
        "FATHER_NAME",
        "TC_NO"
    },
    
    new ArrayList()
    {
        "NAME",
        "SURNAME",
        "TC_IDENTITY_NUMBER",
        "PHONE",
        "EMAIL",
        "PASSWORD"
    }
};

using (var connection = new MySqlConnection(connectionString))
{
    connection.Open();
    var index = 0;
    foreach (string var in tables)
    {
        if (var == null)
            continue;
        ReadTableData(connection, var, tablesColumns[index], (ArrayList?)tablesColumns2[index]);
        index++;
    }
}
void ReadTableData(MySqlConnection connection, string tableName, object? tablesColumn, ArrayList? tablesColumn2)
{
    using var dataCommand = new MySqlCommand($"SELECT * FROM {tableName}", connection);
    using var dataReader = dataCommand.ExecuteReader();
    while (dataReader.Read())
    {
        if (tablesColumn == null) return;
        if (tablesColumn2 == null) return;
        var index = 0;
        var id = dataReader.GetInt32(0); // Burada ID'yi alın
        foreach (int var in (IEnumerable)tablesColumn)
        {
            var variable = dataReader.IsDBNull(var) ? null : dataReader.GetString(var);
            if (variable == null) continue;
            var decryptText = DecryptAndWriteDb(variable);
            using var updateConnection = new MySqlConnection(connectionString);
            updateConnection.Open();
            UpdateColumnData(updateConnection, tableName, (string)tablesColumn2[index], decryptText, id); // ID'yi güncelleme fonksiyonuna gönderin
            updateConnection.Close();
            index++;
        }
    }
    dataReader.Close();
}

string DecryptAndWriteDb(string decryptText)
{
    return crypto.Decrypt(decryptText);
}

void UpdateColumnData(MySqlConnection connection, string tableName, string columnName, string newValue, int id)
{
    using var updateCommand = new MySqlCommand($"UPDATE {tableName} SET {columnName} = @newValue WHERE ID = @id", connection);
    updateCommand.Parameters.AddWithValue("@newValue", newValue);
    updateCommand.Parameters.AddWithValue("@id", id);
    var rowsAffected = updateCommand.ExecuteNonQuery();
    Console.WriteLine($"Updated {rowsAffected} rows.");
}

