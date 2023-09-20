namespace MortagePlan.Pages.MortageCalculator
{
    // Global static helper class to contain variables and functions
    public static class Helper
    {
        // All files use this connectionString
        public static readonly String connectionString = "Data Source=.\\sqlexpress;Initial Catalog=mortage;Integrated Security=True";

        public static double CalculateMonthlyPayment(string totalLoan, string interest, string years, string name)
        {
            double L = double.Parse(totalLoan);
            double c = double.Parse(interest) / 100; // interest is in %, so c = interest / 100
            double n = double.Parse(years) * 12; // number of payments = years * 12

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
