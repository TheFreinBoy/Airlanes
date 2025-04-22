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

                string createAirportsTable = @"
                CREATE TABLE IF NOT EXISTS Airports (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    name TEXT NOT NULL,
                    city TEXT NOT NULL,
                    country TEXT NOT NULL,
                    iata_code TEXT NOT NULL UNIQUE
                );";
                ExecuteQuery(createAirportsTable, connection);

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

                        Console.WriteLine("Додано 4 літаки до таблиці Airplanes.");
                    }
                }
                string checkIfAirportsExist = "SELECT COUNT(*) FROM Airports;";
                using (var command = new SQLiteCommand(checkIfAirportsExist, connection))
                {
                    long count = (long)command.ExecuteScalar();
                    if (count == 0)
                    {
                        string insertAirports = @"
                        INSERT INTO Airports (name, city, country, iata_code) VALUES
                            ('Boryspil International Airport', 'Kyiv', 'Ukraine', 'KBP'),
                            ('Lviv Danylo Halytskyi International Airport', 'Lviv', 'Ukraine', 'LWO'),
                            ('Odesa International Airport', 'Odesa', 'Ukraine', 'ODS'),
                            ('Kharkiv International Airport', 'Kharkiv', 'Ukraine', 'HRK'),
                            ('Heathrow Airport', 'London', 'United Kingdom', 'LHR'),
                            ('Charles de Gaulle Airport', 'Paris', 'France', 'CDG'),
                            ('John F. Kennedy International Airport', 'New York', 'USA', 'JFK'),
                            ('Dubai International Airport', 'Dubai', 'UAE', 'DXB'),
                            ('Frankfurt am Main Airport', 'Frankfurt', 'Germany', 'FRA'),
                            ('Tokyo Haneda Airport', 'Tokyo', 'Japan', 'HND');
                        ";
                        ExecuteQuery(insertAirports, connection);
                        Console.WriteLine("Додано 10 аеропортів до таблиці Airports.");
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
        public List<Airport> GetAirports()
        {
            var airports = new List<Airport>();
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT id, name, city, country, iata_code FROM Airports;";
                using (var command = new SQLiteCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        airports.Add(new Airport
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            City = reader.GetString(2),
                            Country = reader.GetString(3),
                            IATACode = reader.GetString(4)
                        });
                    }
                    Console.WriteLine("Da");
                }
            }
            return airports;
        }
        public void CreateFlight(string from, string to, string departure, string returnDate, string flightClass, int airplaneId, string flightNumber,
                                 int capacity, string timeDP, string timeAR)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                string query = @"
                INSERT INTO Flights (
                    from_location, to_location, departure, return_date, class,
                    airplane_id, flight_number, capacity, time_DP, time_AR
                )
                VALUES (
                    @from, @to, @departure, @return, @class,
                    @airplane_id, @flight_number, @capacity, @timeDP, @timeAR
                );";

                using (var cmd = new SQLiteCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@from", from);
                    cmd.Parameters.AddWithValue("@to", to);
                    cmd.Parameters.AddWithValue("@departure", departure);
                    cmd.Parameters.AddWithValue("@return", returnDate);
                    cmd.Parameters.AddWithValue("@class", flightClass);
                    cmd.Parameters.AddWithValue("@airplane_id", airplaneId);
                    cmd.Parameters.AddWithValue("@flight_number", flightNumber);
                    cmd.Parameters.AddWithValue("@capacity", capacity);
                    cmd.Parameters.AddWithValue("@timeDP", timeDP);
                    cmd.Parameters.AddWithValue("@timeAR", timeAR);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public SQLiteConnection GetConnection()
        {
            return new SQLiteConnection(_connectionString);
        }
    }
}
