using System;
using System.Data.SqlClient;
using FirebirdSql.Data.FirebirdClient;
using LinqToDB;
using LinqToDB.AspNet;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using StockAccounting.Core.Data.DbAccess;
using StockAccounting.Core.Data.Utils.ServiceRegistration;

IConfiguration _configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .Build();

var logFilePath = $"Logs/log-.txt";

#if (RELEASE)
    logFilePath = $"C:\\www\\StockAccounting\\Synchronization\\Logs\\log-.txt";
#endif

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File(logFilePath, rollingInterval: RollingInterval.Day, retainedFileCountLimit: 999)
    .CreateLogger();

var firebirdConnString = _configuration["ConnectionStrings:Firebird"];
var sqlConnString = _configuration["ConnectionStrings:Default"];

using var fbConn = new FbConnection(firebirdConnString);
using var sqlConn = new SqlConnection(sqlConnString);
fbConn.Open();
sqlConn.Open();

Log.Information("------------------------- Synchronization start: {date}", DateTime.Now);
Log.Debug("Firebird string is: {ConnectionString}", firebirdConnString);
Log.Debug("Sql string is: {ConnectionString}", sqlConnString);

const string query = """
                     SELECT Code1, Barcode, Code10, L1, Measure_L1, Entered, Updated FROM GOODS
                     WHERE BARCODE IS NOT NULL AND TRIM(BARCODE) <> '' AND CHAR_LENGTH(Barcode) = 13
                     AND GOOD_TYPE_ID = 3
                     """;
const string checkQuery = """
                          SELECT COUNT(*) FROM TBL_ExternalData WHERE ItemNumber = @ItemNumber OR PluCode = @PluCode
                                                    OR Barcode = @Barcode OR Name = @Name
                          """;

using var fbCmd = new FbCommand(query, fbConn);
using var reader = fbCmd.ExecuteReader();
var addedCount = 0;
while (reader.Read())
{
    using var checkCmd = new SqlCommand(checkQuery, sqlConn);
    checkCmd.Parameters.AddWithValue("@ItemNumber", reader["Code1"]);
    checkCmd.Parameters.AddWithValue("@PluCode", reader["Code10"]);
    checkCmd.Parameters.AddWithValue("@Barcode", reader["Barcode"]);
    checkCmd.Parameters.AddWithValue("@Name", reader["L1"]);
    var count = (int)checkCmd.ExecuteScalar();
    if (count == 0) // Insert only if it doesn't exist
    {
        const string insertQuery = """
                                   INSERT INTO TBL_ExternalData (ItemNumber, PluCode, Barcode, Name, Unit, Created, Updated) 
                                   VALUES (@ItemNumber, @PluCode, @Barcode, @Name, @Unit, @Created, @Updated)
                                   """;
        using SqlCommand insertCmd = new SqlCommand(insertQuery, sqlConn);
        var pluCode = reader["Code10"] == DBNull.Value ? "-" : reader["Code10"];
        insertCmd.Parameters.AddWithValue("@ItemNumber", reader["Code1"]);
        insertCmd.Parameters.AddWithValue("@PluCode", pluCode);
        insertCmd.Parameters.AddWithValue("@Barcode", reader["Barcode"]);
        insertCmd.Parameters.AddWithValue("@Name", reader["L1"]);
        insertCmd.Parameters.AddWithValue("@Unit", reader["Measure_L1"]);
        insertCmd.Parameters.AddWithValue("@Created", reader["Entered"]);
        insertCmd.Parameters.AddWithValue("@Updated", reader["Updated"]);
        insertCmd.ExecuteNonQuery();

        addedCount++;
        Log.Debug($"#{addedCount} {reader["Barcode"]} Item inserted successfully!");
    }
}
Log.Information("Were added {count} new external data", addedCount);