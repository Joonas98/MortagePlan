using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace MortagePlan.Pages.MortageCalculator
{
    public class CreateModel : PageModel
    {
        // This file is used for creating a new customer
        public CustomerInfo customerInfo = new();
        public string errorMessage = "";
        public string successMessage = "";

        public void OnGet()
        {

        }

        public void OnPost() 
        {
        customerInfo.name = Request.Form["name"];
        customerInfo.total_loan = Request.Form["total_loan"];
        customerInfo.interest = Request.Form["interest"];
        customerInfo.years = Request.Form["years"];
        customerInfo.monthly_payment = "0";

			// Make sure that all the fields are filled
			if (string.IsNullOrEmpty(customerInfo.name) || string.IsNullOrEmpty(customerInfo.total_loan) || 
                string.IsNullOrEmpty(customerInfo.interest) || string.IsNullOrEmpty(customerInfo.years))
            {
                errorMessage = "All the fields are required";
                return;
            }

            // Save the new client into the database
            try
            {
				using SqlConnection connection = new (Helper.connectionString);
				connection.Open();
				String sql = "INSERT INTO Prospects " + "(name, total_loan, interest, years, monthly_payment) VALUES " + "(@name, @total_loan, @interest, @years, @monthly_payment);";

                // Create the new customer and calculate the payment
				using SqlCommand command = new (sql, connection);
				command.Parameters.AddWithValue("@name", customerInfo.name);
				command.Parameters.AddWithValue("@total_loan", Convert.ToDouble(customerInfo.total_loan));
				command.Parameters.AddWithValue("@interest", Convert.ToDouble(customerInfo.interest));
				command.Parameters.AddWithValue("@years", customerInfo.years);
				command.Parameters.AddWithValue("@monthly_payment", Helper.CalculateMonthlyPayment(
						customerInfo.total_loan,
						customerInfo.interest,
						customerInfo.years,
						customerInfo.name
						));

				command.ExecuteNonQuery();
			}
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }

            customerInfo.name = ""; customerInfo.total_loan = ""; customerInfo.interest = ""; customerInfo.years = ""; customerInfo.monthly_payment = "";
			successMessage = "The new customer was added successfully into the database";

            // Return to the main page
            Response.Redirect("/MortageCalculator/Index");
		}
    }
}
