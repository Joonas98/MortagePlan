using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace MortagePlan.Pages.MortageCalculator
{
    public class CreateModel : PageModel
    {
        // This file is used for creating a new customer

        public CustomerInfo customerInfo = new(); // Initialize a new customer info object
        public string errorMessage = ""; // Used for displaying error messages
        public string successMessage = ""; // Used to inform about a successful operation

        public void OnGet()
        {
            // This method handles the GET request for the create page
            // Here, you can add any necessary initialization code
        }

        public void OnPost()
        {
            // This method handles the POST request when submitting the create form

            // Retrieve form data from the request
            customerInfo.name = Request.Form["name"];
            customerInfo.total_loan = Request.Form["total_loan"];
            customerInfo.interest = Request.Form["interest"];
            customerInfo.years = Request.Form["years"];
            customerInfo.monthly_payment = "0"; // Initialize monthly_payment field

            // Make sure that all the fields are filled
            if (string.IsNullOrEmpty(customerInfo.name) || string.IsNullOrEmpty(customerInfo.total_loan) ||
                string.IsNullOrEmpty(customerInfo.interest) || string.IsNullOrEmpty(customerInfo.years))
            {
                errorMessage = "All the fields are required"; // Set an error message if any field is missing
                return;
            }

            // Save the new client into the database
            try
            {
                using SqlConnection connection = new(Helper.connectionString);
                connection.Open();
                String sql = "INSERT INTO Prospects " + "(name, total_loan, interest, years, monthly_payment) VALUES " + "(@name, @total_loan, @interest, @years, @monthly_payment);";

                // Create the new customer and calculate the monthly payment
                using SqlCommand command = new(sql, connection);
                command.Parameters.AddWithValue("@name", customerInfo.name);
                command.Parameters.AddWithValue("@total_loan", customerInfo.total_loan);
                command.Parameters.AddWithValue("@interest", customerInfo.interest);
                command.Parameters.AddWithValue("@years", customerInfo.years);
                command.Parameters.AddWithValue("@monthly_payment", Helper.CalculateMonthlyPayment(
                    customerInfo.total_loan,
                    customerInfo.interest,
                    customerInfo.years,
                    customerInfo.name
                ));

                command.ExecuteNonQuery(); // Execute the SQL insert command to add the new customer
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message; // Handle any exceptions and set the error message
                return;
            }

            // Clear form fields and set a success message
            customerInfo.name = "";
            customerInfo.total_loan = "";
            customerInfo.interest = "";
            customerInfo.years = "";
            customerInfo.monthly_payment = "";
            successMessage = "The new customer was added successfully into the database";

            // Redirect back to the main page after successful creation
            Response.Redirect("/MortageCalculator/Index");
        }
    }
}
