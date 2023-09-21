using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Globalization;

namespace MortagePlan.Pages.MortageCalculator
{
	public class EditModel : PageModel
	{
		public CustomerInfo customerInfo = new(); // Object to store customer information
		public string errorMessage = ""; // Used for error messages, such as missing form fields
		public string successMessage = ""; // Used to inform about a successful operation 

		// Executed when a GET request is made to the edit page
		public void OnGet()
		{
			string id = Request.Query["id"]; // Get the customer ID from the query string

			try
			{
				using SqlConnection connection = new(Helper.connectionString); // Create a database connection
				connection.Open();

				// SQL query to retrieve customer information for the given ID
				string sql = "SELECT id, name, total_loan, interest, years FROM Prospects WHERE id=@id";

				// Create a SQL command and set the parameter for the customer ID
				using SqlCommand command = new(sql, connection);
				command.Parameters.AddWithValue("id", id);

				// Execute the query and read the customer data
				using SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    // Populate the customerInfo object with the retrieved data
                    customerInfo.id = reader.GetInt32(0).ToString();
                    customerInfo.name = reader.GetString(1);
                    customerInfo.total_loan = reader.GetDouble(2).ToString(CultureInfo.InvariantCulture); // Use CultureInfo.InvariantCulture
                    customerInfo.interest = reader.GetDouble(3).ToString(CultureInfo.InvariantCulture); // Use CultureInfo.InvariantCulture
                    customerInfo.years = reader.GetInt32(4).ToString();
                }
            }
			catch (Exception ex)
			{
				// Handle any exceptions that may occur during database access
				errorMessage = ex.Message;
			}
		}

		// Executed when a POST request is made (e.g., form submission)
		public void OnPost()
		{
			// Retrieve customer data from the form fields
			customerInfo.id = Request.Form["id"];
			customerInfo.name = Request.Form["name"];
			customerInfo.total_loan = Request.Form["total_loan"];
			customerInfo.interest = Request.Form["interest"];
			customerInfo.years = Request.Form["years"];

			// Check if any form fields are missing
			if (string.IsNullOrEmpty(customerInfo.id) || string.IsNullOrEmpty(customerInfo.name) ||
				string.IsNullOrEmpty(customerInfo.total_loan) || string.IsNullOrEmpty(customerInfo.interest) ||
				string.IsNullOrEmpty(customerInfo.years))
			{
				errorMessage = "All fields are required!";
				return;
			}

			try
			{
				using SqlConnection connection = new SqlConnection(Helper.connectionString); // Create a database connection
				connection.Open();

				// SQL query to update customer information in the database
				string sql = "UPDATE Prospects " +
					"SET name=@name, total_loan=@total_loan, interest=@interest, years=@years, monthly_payment=@monthly_payment " +
					"WHERE id=@id";

				// Create a SQL command and set parameters for the update
				using SqlCommand command = new(sql, connection);
				command.Parameters.AddWithValue("@id", customerInfo.id);
				command.Parameters.AddWithValue("@name", customerInfo.name);
				command.Parameters.AddWithValue("@total_loan", customerInfo.total_loan);
				command.Parameters.AddWithValue("@interest", customerInfo.interest);
				command.Parameters.AddWithValue("@years", customerInfo.years);

				// Calculate and set the @monthly_payment parameter directly as a double
				double monthlyPaymentValue = Helper.CalculateMonthlyPayment(
					customerInfo.total_loan,
					customerInfo.interest,
					customerInfo.years,
					customerInfo.name
				);
				command.Parameters.AddWithValue("@monthly_payment", monthlyPaymentValue);

				// Execute the SQL update command
				command.ExecuteNonQuery();
			}
			catch (Exception ex)
			{
				// Handle any exceptions that may occur during the update
				errorMessage = ex.Message;
				return;
			}

			// After a successful edit, redirect to the main page
			Response.Redirect("/MortageCalculator/Index");
		}
	}
}
