using NUnit.Framework;
using MortagePlan.Pages.MortageCalculator; // Import the Helper class namespace
using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using MortagePlan.Pages;
using System.Transactions;

namespace MortagePlan.Tests
{
    [TestFixture]
    public class HelperTests
    {
        [Test]
        public void CalculateMonthlyPayment_ValidInput_ReturnsCorrectResult()
        {
            // Arrange
            // Provide valid input values for testing
            string totalLoan = "1035";
            string interest = "5,5"; // Use ',' for decimal separator
            string years = "5";
            string name = "Matti";

            // Act
            double result = Helper.CalculateMonthlyPayment(totalLoan, interest, years, name);

            // Assert
            // Calculate the expected result using the same formula with a tolerance for precision
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
            var expectedRowCount = 8; // Has to be adjusted as adding or removing customers

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Act
                string sql = "SELECT COUNT(*) FROM Prospects";

                using (SqlCommand command = new(sql, connection))
                {
                    var rowCount = (int)command.ExecuteScalar();

                    // Assert
                    Assert.AreEqual(expectedRowCount, rowCount); // Ensure the actual row count matches the expected count
                }
            }
        }
    }

    /* !!! This one ended up being a bit too much for this small amount of time
    The purpose was to fill a separated testing database with various values
    and making sure everything works nicely.
    I wasted some time here (as you can see) and ended up thinking this
     would be probably pretty useless anyways.*/
    [TestFixture]
    public class IndexModelTests
    {
        [Test]
        public void OnGet_PopulatesCustomersList()
        {
            // Arrange
            // Use a separate database for testing purposes
            string testConnectionString = "Data Source=.\\sqlexpress;Initial Catalog=mortageTesting;Integrated Security=True";
            var indexModel = new Pages.MortageCalculator.IndexModel(testConnectionString);

            using (SqlConnection connection = new SqlConnection(testConnectionString))
            {
                connection.Open();

                // Create and populate test customers in the ProspectsTesting table
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

            // Act
            indexModel.OnGet();

            List<CustomerInfo> customersList = indexModel.customersList; // Retrieve the populated list

            // Assert
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
            string testConnectionString = "Data Source=.\\sqlexpress;Initial Catalog=mortageTesting;Integrated Security=True";
            using (SqlConnection connection = new SqlConnection(testConnectionString))
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
