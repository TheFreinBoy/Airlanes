using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;
using Airalnes;
using System.Windows.Media.Media3D;
using Airalnes.Models;

namespace Airalnes.Helpers
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
                            ('Heathrow Airport', 'London', 'UK', 'LHR'),
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
        public List<Flight> SearchFlights(string from, string to, string departure, string arrival, string clas, int passengers)
        {
            var flights = new List<Flight>();

            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                string query = @"
                SELECT 
                    f.id, f.from_location, f.to_location, f.departure, f.return_date,
                    f.class, a.name AS airplane_name, f.flight_number,
                    f.capacity, f.time_DP, f.time_AR
                FROM Flights f
                JOIN Airplanes a ON f.airplane_id = a.id
                WHERE (@from = '' OR f.from_location LIKE @from)
                  AND (@to = '' OR f.to_location LIKE @to)
                  AND (@departure = '' OR f.departure = @departure)
                  AND (@return_date = '' OR f.return_date = @return_date)
                  AND (@class = '' OR f.class = @class)
                  AND f.capacity >= @passengers;";

                using (var command = new SQLiteCommand(query, connection))
                {

                    command.Parameters.AddWithValue("@from", string.IsNullOrEmpty(from) ? "" : $"%{ExtractAirportCode(from)}%");
                    command.Parameters.AddWithValue("@to", string.IsNullOrEmpty(to) ? "" : $"%{ExtractAirportCode(to)}%");
                    command.Parameters.AddWithValue("@departure", string.IsNullOrEmpty(departure) ? "" : ConvertFromIsoDate(departure));
                    command.Parameters.AddWithValue("@return_date", string.IsNullOrEmpty(arrival) ? "" : ConvertFromIsoDate(arrival));
                    command.Parameters.AddWithValue("@class", clas);
                    command.Parameters.AddWithValue("@passengers", passengers);

                    Console.WriteLine("SQL query: " + query);
                    foreach (SQLiteParameter param in command.Parameters)
                    {
                        Console.WriteLine($"Parameter: {param.ParameterName}, Value: {param.Value}");
                    }

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            flights.Add(new Flight
                            {
                                Id = reader.GetInt32(0),
                                FromLocation = reader.GetString(1),
                                ToLocation = reader.GetString(2),
                                Departure = reader.GetString(3),
                                ReturnDate = reader.IsDBNull(4) ? null : reader.GetString(4),
                                Class = reader.GetString(5),
                                AirplaneName = reader.GetString(6),
                                FlightNumber = reader.GetString(7),
                                Capacity = reader.GetInt32(8),
                                TimeDP = reader.GetString(9),
                                TimeAR = reader.GetString(10)
                            });
                        }
                    }
                }
            }

            return flights;
        }
        private string ExtractAirportCode(string input)
        {
            if (input.Contains("(") && input.Contains(")"))
            {
                int start = input.IndexOf('(') + 1;
                int end = input.IndexOf(')');
                return input.Substring(start, end - start);
            }
            return input;
        }

        private string ConvertFromIsoDate(string isoDateStr)
        {

            if (DateTime.TryParse(isoDateStr, out DateTime date))
            {
                return date.ToString("dd.MM.yyyy");
            }
            return isoDateStr;
        }




        public SQLiteConnection GetConnection()
        {
            return new SQLiteConnection(_connectionString);
        }
    }
}
