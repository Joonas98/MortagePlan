using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Globalization;

namespace MortagePlan.Pages.MortageCalculator
{
    public class EditModel : PageModel
    {
        public CustomerInfo customerInfo = new();
        public string errorMessage = ""; // Used for errors, such as all forms are not filled
        public string successMessage = ""; // Used to inform a successful operation 

		// Executed when the GET request is made to the edit page
		public void OnGet()
        {
            String id = Request.Query["id"];

            try
            {
				using SqlConnection connection = new (Helper.connectionString);
                
                connection.Open();
                String sql = "SELECT * FROM Prospects WHERE id=@id"; // Todo: avoid using SELECT *

				using SqlCommand command = new(sql, connection);
				command.Parameters.AddWithValue("id", id);
				using SqlDataReader reader = command.ExecuteReader();

				if (reader.Read())
				{
					customerInfo.id = reader.GetInt32(0).ToString();
					customerInfo.name = reader.GetString(1);
					customerInfo.total_loan = reader.GetDouble(2).ToString();
					customerInfo.interest = reader.GetDouble(3).ToString();
					customerInfo.years = reader.GetInt32(4).ToString();
				}
			}
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
        }


        public void OnPost()
        {
			customerInfo.id = Request.Form["id"];
			customerInfo.name = Request.Form["name"];
            customerInfo.total_loan = Request.Form["total_loan"];
            customerInfo.interest = Request.Form["interest"];
            customerInfo.years = Request.Form["years"];

            if (string.IsNullOrEmpty(customerInfo.id) || string.IsNullOrEmpty(customerInfo.name) ||
                string.IsNullOrEmpty(customerInfo.total_loan) || string.IsNullOrEmpty(customerInfo.interest) ||
                string.IsNullOrEmpty(customerInfo.years))
            {
                errorMessage = "All fields are required!";
                return;
            }

            try
            {
				using SqlConnection connection = new SqlConnection(Helper.connectionString);
				connection.Open();
				String sql = "UPDATE Prospects " +
					"SET name=@name, total_loan=@total_loan, interest=@interest, years=@years, monthly_payment=@monthly_payment " +
					"WHERE id=@id";

                    using SqlCommand command = new (sql, connection);
				    command.Parameters.AddWithValue("@id", customerInfo.id);
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

            // After successful edit, return to the main page
            Response.Redirect("/MortageCalculator/Index");
		}
	}
}
