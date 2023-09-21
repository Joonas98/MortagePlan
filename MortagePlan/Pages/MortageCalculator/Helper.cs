using System.Globalization;

namespace MortagePlan.Pages.MortageCalculator
{
    // Global static helper class to contain variables and functions
    public static class Helper
    {
        // All files use this connectionString
        // public static readonly String connectionString = "Data Source=.\\sqlexpress;Initial Catalog=mortage;Integrated Security=True";
        public static readonly String connectionString = "Server=tcp:mortageserver.database.windows.net,1433;Initial Catalog=MortageDB;Persist Security Info=False;User ID=MortageCalculatorAdmin;Password=MortagePassword!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        public static double CalculateMonthlyPayment(string totalLoan, string interest, string years, string name)
        {
			// Converting values, the "CultureInfo.InvariantCulture" is very important part because Azure accepts only en-US
			double L = double.Parse(totalLoan, CultureInfo.InvariantCulture);
			double c = double.Parse(interest, CultureInfo.InvariantCulture) / 100; // interest is in %, so c = interest / 100
            double n = double.Parse(years, CultureInfo.InvariantCulture) * 12; // number of payments = years * 12

            double factor = 1 + c;
            double power = 1;
            double ans;

            // Math.Pow() was not allowed
            for (int i = 0; i < n; i++)
            {
                power *= factor;
            }

            // Calculate and store the answer
            ans = (L * c * power) / (power - 1);

            // Print and return the answer
            Console.OutputEncoding = System.Text.Encoding.UTF8; // Fix € for console
            Console.WriteLine(name + " wants to borrow " + L + " € for a period of " +
            ((int)n / 12) + " years and pay " + ans.ToString("F2") + " € each month");
            return ans;
        }
    }
}
