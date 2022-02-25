using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading;

namespace PruebaSQL
{
	class Program
	{
		static void Main(string[] args)
		{

			//Server = tcp:localhost,Port; Initial Catalog = databaseName; "
			//	  + "User ID =UserID;Password =Password";

			//Referencia a las connectionStrings
			//https://docs.microsoft.com/en-us/dotnet/api/system.data.sqlclient.sqlconnection.connectionstring?view=dotnet-plat-ext-6.0

			string connectionString =
			"Server=tcp:sqldatabase,1433;Initial Catalog=model;"
			+ "User ID =sa;Password = Esekuele6595;"
			+ "Timeout = 10";

			// Provide the query string with a parameter placeholder.
			//string queryString =
			//	"SELECT ProductID, UnitPrice, ProductName from dbo.products "
			//		+ "WHERE UnitPrice > @pricePoint "
			//		+ "ORDER BY UnitPrice DESC;";
			string queryString =
				"CREATE TABLE ola (@pricepoint int);";

			// Specify the parameter value.
			int paramValue = 5;

			Console.WriteLine("Creando conexion");

			// Create and open the connection in a using block. This
			// ensures that all resources will be closed and disposed
			// when the code exits.
			using (SqlConnection connection =
				new SqlConnection(connectionString))
			{
				// Create the Command and Parameter objects.
				SqlCommand command = new SqlCommand(queryString, connection);
				command.Parameters.AddWithValue("@pricePoint", paramValue);

				// Open the connection in a try/catch block.
				// Create and execute the DataReader, writing the result
				// set to the console window.
				try
				{
					connection.Open();
					Console.WriteLine("Conectado");
					SqlDataReader reader = command.ExecuteReader();
					while (reader.Read())
					{
						Console.WriteLine("Leyendo");
						Console.WriteLine("\t{0}\t{1}\t{2}",
							reader[0], reader[1], reader[2]);
					}
					reader.Close();
				}
				catch (Exception ex)
				{
					Console.WriteLine("Error:");
					Console.WriteLine(ex.Message);
				}
				Console.ReadLine();
			}
		}
	}
}
