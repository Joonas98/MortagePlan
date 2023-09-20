using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace MortagePlan.Pages.MortageCalculator
{
	public class IndexModel : PageModel
	{
		// List to store customer information
		public List<CustomerInfo> customersList = new();
		private string _connectionString; // Connection string to the database

		// Constructor for the IndexModel
		// Allows specifying a custom connectionString for NUnit testing, defaults to Helper.connectionString
		public IndexModel(string connectionString = null)
		{
			_connectionString = connectionString ?? Helper.connectionString;
		}

		// Executed when a GET request is made to the page
		public void OnGet()
		{
			try
			{
				// Establish a database connection using the provided or default connection string
				using SqlConnection connection = new(_connectionString);
				connection.Open();

				// SQL query to retrieve customer information
				string sql = "SELECT id, name, total_loan, interest, years, monthly_payment FROM Prospects";

				// Create a SQL command and execute the query
				using SqlCommand command = new(sql, connection);
				using SqlDataReader reader = command.ExecuteReader();

				// Loop through the query results and populate the customersList
				while (reader.Read())
				{
					CustomerInfo clientInfo = new()
					{
						id = reader.GetInt32(0).ToString(),
						name = reader.GetString(1),
						total_loan = reader.GetDouble(2).ToString(),
						interest = reader.GetDouble(3).ToString(),
						years = reader.GetInt32(4).ToString(),
						monthly_payment = reader.GetDouble(5).ToString("F2")
					};

					customersList.Add(clientInfo);
				}
			}
			catch (Exception ex)
			{
				// Handle any exceptions that may occur during database access
				Console.WriteLine("Exception: " + ex.Message);
			}
		}
	}

	// The class containing the customer data
	public class CustomerInfo
	{
		public string id, name, total_loan, interest, years, monthly_payment;
	}
}
