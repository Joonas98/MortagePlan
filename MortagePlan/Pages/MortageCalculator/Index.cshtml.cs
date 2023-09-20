using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace MortagePlan.Pages.MortageCalculator
{
    public class IndexModel : PageModel
    {
        public List<CustomerInfo> customersList = new();

		public void OnGet() 
		{
			try { 
				using SqlConnection connection = new(Helper.connectionString);
				connection.Open();
				String sql = "SELECT * FROM Prospects"; // Todo: avoid using SELECT *

				using SqlCommand command = new(sql, connection);
				using SqlDataReader reader = command.ExecuteReader();

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
                Console.WriteLine("Exception: " +  ex.Message);
            }

		}
	}
	
	// The class containing the customer data
	public class CustomerInfo
    {
        public string id, name, total_loan, interest, years, monthly_payment;
    }
}
