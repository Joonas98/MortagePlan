using NUnit.Framework;
using MortagePlan.Pages.MortageCalculator; // Make sure to import your Helper class namespace
using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using MortagePlan.Pages;

namespace MortagePlan.Tests
{
	[TestFixture]
	public class HelperTests
	{
		[Test]
		public void CalculateMonthlyPayment_ValidInput_ReturnsCorrectResult()
		{
			// Arrange
			// Remember to use decimal format with ',' instead of '.'
			string totalLoan = "1035";
			string interest = "5,5";
			string years = "5";
			string name = "Matti";

			// Act
			double result = Helper.CalculateMonthlyPayment(totalLoan, interest, years, name);

			// Assert
			// You should calculate the expected result manually based on the provided inputs
			// In this case, you can calculate the expected result using the same formula
			// and compare it with the actual result with a tolerance for floating-point precision
			double expected = CalculateExpectedMonthlyPayment(1035, 0.055, 5 * 12);
			double tolerance = 0.01; // Adjust as needed
			Assert.AreEqual(expected, result, tolerance);
		}

		// Helper method to calculate the expected result
		private double CalculateExpectedMonthlyPayment(double L, double c, double n)
		{
			double factor = 1 + c;
			double power = 1;
			for (int i = 0; i < n; i++)
			{
				power *= factor;
			}
			return (L * c * power) / (power - 1);
		}
	}

	[TestFixture]
	public class DatabaseTests
	{
		[Test]
		public void TestDatabaseConnectionAndDataRetrieval()
		{
			// Arrange
			var connectionString = Helper.connectionString;
			var expectedRowCount = 7; // The amount of rows in the used database

			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open();

				// Act
				string sql = "SELECT COUNT(*) FROM Prospects";

				using (SqlCommand command = new(sql, connection))
				{
					var rowCount = (int)command.ExecuteScalar();

					// Assert
					Assert.AreEqual(expectedRowCount, rowCount);
				}
			}
		}
	}

	[TestFixture]
	public class IndexModelTests
	{
		[Test]
		public void OnGet_PopulatesCustomersList()
		{
			// Arrange
			string connectionString = Helper.connectionString;

			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open();

				// Create and populate test customers in the database for the test
				// Important to use separate database for testing, rather than the real database
				string insertSql = "INSERT INTO ProspectsTesting (name, total_loan, interest, years, monthly_payment) VALUES (@name, @total_loan, @interest, @years, @monthly_payment)";
				using (SqlCommand insertCommand = new SqlCommand(insertSql, connection))
				{
					insertCommand.Parameters.AddWithValue("@name", "Test Customer 1");
					insertCommand.Parameters.AddWithValue("@total_loan", 1000.00);
					insertCommand.Parameters.AddWithValue("@interest", 5.0);
					insertCommand.Parameters.AddWithValue("@years", 2);
					insertCommand.Parameters.AddWithValue("@monthly_payment", 44.0);
					insertCommand.ExecuteNonQuery();
				}

				using (SqlCommand insertCommand = new SqlCommand(insertSql, connection))
				{
					insertCommand.Parameters.AddWithValue("@name", "Test Customer 2");
					insertCommand.Parameters.AddWithValue("@total_loan", 2000.00);
					insertCommand.Parameters.AddWithValue("@interest", 3.5);
					insertCommand.Parameters.AddWithValue("@years", 3);
					insertCommand.Parameters.AddWithValue("@monthly_payment", 60.0);
					insertCommand.ExecuteNonQuery();
				}
			}

			var indexModel = new Pages.MortageCalculator.IndexModel();

			// Act
			indexModel.OnGet();

			// Assert
			List<CustomerInfo> customersList = indexModel.customersList;
			Assert.NotNull(customersList); // Ensure customersList is not null
			Assert.AreEqual(2, customersList.Count); // Check the number of customers retrieved

			// Check the properties of the first customer
			var firstCustomer = customersList[0];
			Assert.AreEqual("Test Customer 1", firstCustomer.name);
			Assert.AreEqual("1000.00", firstCustomer.total_loan);
			Assert.AreEqual("5.0", firstCustomer.interest);
			Assert.AreEqual("2", firstCustomer.years);
			Assert.AreEqual("44.00", firstCustomer.monthly_payment);

			// Check the properties of the second customer
			var secondCustomer = customersList[1];
			Assert.AreEqual("Test Customer 2", secondCustomer.name);
			Assert.AreEqual("2000.00", secondCustomer.total_loan);
			Assert.AreEqual("3.5", secondCustomer.interest);
			Assert.AreEqual("3", secondCustomer.years);
			Assert.AreEqual("60.00", secondCustomer.monthly_payment);
		}

		[TearDown]
		public void TearDown()
		{
			// Clean up test data from the test database after the test
			string connectionString = Helper.connectionString;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open();
				string deleteSql = "DELETE FROM ProspectsTesting";
				using (SqlCommand deleteCommand = new SqlCommand(deleteSql, connection))
				{
					deleteCommand.ExecuteNonQuery();
				}
			}
		}
	}

}
