using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.IO;
using ADOX;
using System.Windows.Forms;
using System.Data.SqlTypes;
using System.Runtime.InteropServices;
using System.ServiceModel.Security;
using System.Net.NetworkInformation;

namespace prjDatabaseCreation
{
    internal class DatabaseUtils
    {
        private const string EXAMPLEDB = "MPTMSDatabase.mdb";
        public const string CONNECTION_STRING = @"Provider=Microsoft Jet 4.0 OLE DB Provider;Data Source = " + EXAMPLEDB + ";"; 

        public static void CreateDatabase() //Creates database 
        {
            try
            {
                if (!File.Exists(EXAMPLEDB))
                {
                    CatalogClass cat = new CatalogClass();
                    cat.Create(CONNECTION_STRING);
                    MessageBox.Show("Database Created Successfully", "MPTMS System");

                    CreateTables();
                    SQLCommands();

                    cat = null;
                }
                else
                {
                    MessageBox.Show("The database already exists", "MPTMS System");
                }
            }
            catch (Exception)
            {
                Console.WriteLine("MPTMS database could not be created");
            }
        }
        private static void ExecuteSqlNonQuery(String sSqlString)
        {
            try
            {
                OleDbConnection connection = new OleDbConnection(CONNECTION_STRING);
                connection.Open();
                OleDbCommand cmd = new OleDbCommand(sSqlString, connection);
                cmd.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception)
            {
                MessageBox.Show("Query could not be executed");
            }
        }

        public static void CreateTables() //SQL to create database tables 
        {
            string SqlStringCreateAccount = "CREATE TABLE Account ("
                               + "Username VARCHAR(16) NOT NULL,"
                               + "[Password] CHAR(8) NOT NULL,"
                               + "Email VARCHAR(255) NOT NULL,"
                               + "DateCreated DATE NULL,"
                               + "PRIMARY KEY (Username)"
                               + ")";
            ExecuteSqlNonQuery(SqlStringCreateAccount);

            string SqlStringCreateLocations = "CREATE TABLE Locations ("
                               + "AddressID AUTOINCREMENT NOT NULL,"
                               + "Address VARCHAR(255) NOT NULL,"
                               + "Latitude FLOAT NOT NULL,"
                               + "Longitude FLOAT NOT NULL,"
                               + "PRIMARY KEY (AddressID)"
                               + ")";
            ExecuteSqlNonQuery(SqlStringCreateLocations);

            string SqlStringCreateBankDetails = "CREATE TABLE BankDetails ("
                               + "Username VARCHAR(16) NOT NULL,"
                               + "FirstName VARCHAR(255) NOT NULL,"
                               + "LastName VARCHAR(255) NOT NULL,"
                               + "AccountNumber VARCHAR(8) NOT NULL,"
                               + "PRIMARY KEY (Username),"
                               + "FOREIGN KEY(Username) REFERENCES Account(Username)"
                               + ")";
            ExecuteSqlNonQuery(SqlStringCreateBankDetails);

            string SqlStringCreateTransport = "CREATE TABLE Transport ("
                               + "Mode VARCHAR(15) NOT NULL,"
                               + "Specific VARCHAR(20) NOT NULL,"
                               + "TicketPrice INT NOT NULL,"
                               + "PRIMARY KEY (Mode, Specific)"
                               + ")";
            ExecuteSqlNonQuery(SqlStringCreateTransport);

            string SqlStringCreateRoutes = "CREATE TABLE Routes ("
                               + "RouteID CHAR(5) NOT NULL,"
                               + "StartLocation VARCHAR(255) NOT NULL,"
                               + "FinalLocation VARCHAR(255) NOT NULL,"
                               + "Distance FLOAT NULL,"
                               + "TimeTaken INT NOT NULL,"
                               + "Mode VARCHAR(15) NOT NULL,"
                               + "Specific VARCHAR(20) NOT NULL,"
                               + "PRIMARY KEY (RouteID),"
                               + "FOREIGN KEY (Mode, Specific) REFERENCES Transport (Mode, Specific)"
                               + ")";
            ExecuteSqlNonQuery(SqlStringCreateRoutes);

            string SqlStringCreateUserRoutes = "CREATE TABLE UserRoutes ("
                               + "UserRouteID AUTOINCREMENT NOT NULL,"
                               + "RouteID CHAR(5) NOT NULL,"
                               + "Username VARCHAR(16) NOT NULL,"
                               + "PRIMARY KEY (RouteID, Username),"
                               + "FOREIGN KEY (Username) REFERENCES Account (Username),"
                               + "FOREIGN KEY (RouteID) REFERENCES Routes (RouteID)"
                               + ")";
            ExecuteSqlNonQuery(SqlStringCreateUserRoutes);

            string SqlStringCreateTickets = "CREATE TABLE Tickets ("
                               + "TicketID AUTOINCREMENT NOT NULL, "
                               + "RouteID CHAR(5) NOT NULL, "
                               + "Username VARCHAR(16) NOT NULL, "
                               + "TicketPrice INT NOT NULL, "
                               + "PurchaseDate DATE NULL, "
                               + "FilePath VARCHAR(100) NULL, "
                               + "PRIMARY KEY (TicketID), "
                               + "FOREIGN KEY (RouteID) REFERENCES Routes (RouteID), "
                               + "FOREIGN KEY (Username) REFERENCES Account (Username)"
                               + ")";
            ExecuteSqlNonQuery(SqlStringCreateTickets);
        }

        public static void SQLCommands() //Insert SQL commands to populate databse tables
        {
            string SqlStringAddLocation1 = "INSERT INTO Locations (Address, Latitude, Longitude) " 
                                + "VALUES ('Southampton Airport', 50.9500, -1.3570)";
            ExecuteSqlNonQuery(SqlStringAddLocation1);

            string SqlStringAddLocation2 = "INSERT INTO Locations (Address, Latitude, Longitude) "
                                + "VALUES ('Southampton Central Station', 50.9075, -1.4141)";
            ExecuteSqlNonQuery(SqlStringAddLocation2);

            string SqlStringAddLocation3 = "INSERT INTO Locations (Address, Latitude, Longitude) "
                                + "VALUES ('Peter Symonds College', 51.0656, -1.3150)";
            ExecuteSqlNonQuery(SqlStringAddLocation3);

            string SqlStringTransport1 = "INSERT INTO Transport (Mode, Specific, TicketPrice) "
                                + " VALUES ('Bus', 'BlueStar 1', 2)";
            ExecuteSqlNonQuery(SqlStringTransport1);

            string SqlStringTransport2 = "INSERT INTO Transport (Mode, Specific, TicketPrice) "
                                + " VALUES ('Bus', 'UniLink U1A', 2)";
            ExecuteSqlNonQuery(SqlStringTransport2);

            string SqlStringTransport3 = "INSERT INTO Transport (Mode, Specific, TicketPrice) "
                                + " VALUES ('Train', 'London Waterloo', 8)";
            ExecuteSqlNonQuery(SqlStringTransport3);

            string SqlStringTransport4 = "INSERT INTO Transport (Mode, Specific, TicketPrice) "
                                + " VALUES ('Train', 'Winchester', 8)";
            ExecuteSqlNonQuery(SqlStringTransport4);

            string SqlStringRoutes1 = "INSERT INTO Routes (RouteID, StartLocation, FinalLocation, Distance, TimeTaken, Mode, Specific) "
                                + " VALUES (00001, 'Southampton Airport', 'Southampton Central Station', 0, 10, 'Train', 'Winchester')";
            ExecuteSqlNonQuery(SqlStringRoutes1);

            string SqlStringRoutes2 = "INSERT INTO Routes (RouteID, StartLocation, FinalLocation, Distance, TimeTaken, Mode, Specific) "
                                + " VALUES (00002, 'Peter Symonds College', 'Southampton Central Station', 0, 20, 'Train', 'London Waterloo')";
            ExecuteSqlNonQuery(SqlStringRoutes2);

            string SqlStringRoutes3 = "INSERT INTO Routes (RouteID, StartLocation, FinalLocation, Distance, TimeTaken, Mode, Specific) "
                                + " VALUES (00003, 'Peter Symonds College', 'Southampton Airport', 0, 10, 'Train', 'London Waterloo')";
            ExecuteSqlNonQuery(SqlStringRoutes3);

            string SqlStringRoutes4 = "INSERT INTO Routes (RouteID, StartLocation, FinalLocation, Distance, TimeTaken, Mode, Specific) "
                                + " VALUES (00004, 'Southampton Airport', 'Peter Symonds College', 0, 10, 'Train', 'London Waterloo')";
            ExecuteSqlNonQuery(SqlStringRoutes4);

            string SqlStringRoutes5 = "INSERT INTO Routes (RouteID, StartLocation, FinalLocation, Distance, TimeTaken, Mode, Specific) "
                                + " VALUES (00005, 'Southampton Central Station', 'Peter Symonds College', 0, 60, 'Bus', 'BlueStar 1')";
            ExecuteSqlNonQuery(SqlStringRoutes5);

            string SqlStringRoutes6 = "INSERT INTO Routes (RouteID, StartLocation, FinalLocation, Distance, TimeTaken, Mode, Specific) "
                                + " VALUES (00006, 'Southampton Central Station', 'Southampton Airport', 0, 40, 'Bus', 'UniLink U1A')";
            ExecuteSqlNonQuery(SqlStringRoutes6);
        }
    }
}
