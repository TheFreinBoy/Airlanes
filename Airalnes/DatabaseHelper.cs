using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;
using Airalnes;

namespace Airalnes
{
    public class DatabaseHelper
    {
        private string _databaseFile = "mydatabase2.db";
        private string _connectionString;

        public DatabaseHelper()
        {
            _connectionString = $"Data Source={_databaseFile};Version=3;";
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            if (!File.Exists(_databaseFile))
            {
                SQLiteConnection.CreateFile(_databaseFile);
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    CreateTables(connection);
                }
                Console.WriteLine("Створюємо БД");
            }
            Console.WriteLine("База даних вже є");
        }

        private void CreateTables(SQLiteConnection connection)
        {
            try
            {
                string createUsersTable = @"
                CREATE TABLE IF NOT EXISTS Users (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    name TEXT NOT NULL,
                    email TEXT,
                    pass TEXT NOT NULL,
                    rights TEXT
                );";
                ExecuteQuery(createUsersTable, connection);

                string createAirplanesTable = @"
                CREATE TABLE IF NOT EXISTS Airplanes (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    name TEXT NOT NULL,
                    capacity INTEGER NOT NULL
                );";
                ExecuteQuery(createAirplanesTable, connection);

                string createFlightsTable = @"
                CREATE TABLE IF NOT EXISTS Flights (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    from_location TEXT NOT NULL,
                    to_location TEXT NOT NULL,
                    departure TEXT NOT NULL,
                    return_date TEXT,
                    class TEXT NOT NULL,
                    airplane_id INTEGER NOT NULL,
                    flight_number TEXT NOT NULL,
                    capacity INTEGER NOT NULL,
                    time_DP TEXT NOT NULL,
                    time_AR TEXT NOT NULL,
                    FOREIGN KEY(airplane_id) REFERENCES Airplanes(id)
                );";
                ExecuteQuery(createFlightsTable, connection);

                string checkIfAirplanesExist = "SELECT COUNT(*) FROM Airplanes;";
                using (var command = new SQLiteCommand(checkIfAirplanesExist, connection))
                {
                    long count = (long)command.ExecuteScalar();
                    if (count == 0)
                    {
                        string insertAirplanes = @"
                        INSERT INTO Airplanes (name, capacity) VALUES
                            ('Boeing 737', 160),
                            ('Airbus A320', 150),
                            ('Boeing 777', 280),
                            ('Airbus A350', 314);";
                        ExecuteQuery(insertAirplanes, connection);

                        Console.WriteLine("✅ Додано 4 літаки до таблиці Airplanes.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка при створенні таблиць: {ex.Message}");
            }
        }

        private void ExecuteQuery(string query, SQLiteConnection connection)
        {
            using (var command = new SQLiteCommand(query, connection))
            {
                command.ExecuteNonQuery();
            }
        }

        public List<Airplane> GetAirplanes()
        {
            var airplanes = new List<Airplane>();

            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT id, name, capacity FROM Airplanes;";
                using (var command = new SQLiteCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        airplanes.Add(new Airplane
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Capacity = reader.GetInt32(2)
                        });
                    }
                    Console.WriteLine("Da");
                }
            }

            return airplanes;
        }

        public SQLiteConnection GetConnection()
        {
            return new SQLiteConnection(_connectionString);
        }
    }
}
