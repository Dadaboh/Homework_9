using Microsoft.Data.Sqlite;
using System.Text;

namespace Homework_9
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            var connection = new SqliteConnection($"DataSource={Directory.GetCurrentDirectory()};Filename=SqliteHomework.db;");

            connection.Open();

            //var com = new SqliteCommand("DROP TABLE COSTS", connection).ExecuteNonQuery();

            var createTableCom = new SqliteCommand(@"CREATE TABLE IF NOT EXISTS COSTS (id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, 
            Description TEXT, Sum TEXT NOT NULL, Type TEXT, Date TEXT NOT NULL)", connection);
            createTableCom.ExecuteNonQuery();

            UserChoise(connection);
        }


        private static void UserChoise(SqliteConnection connection)
        {
            var checkUserChoise = true;

            do
            {
                Console.WriteLine($"Оберіть тип дії: \n\t 1 - Переглянути історію витрат \n\t 2 - Додати нову витрату \n\t 3 - Вивести статистику");

                string userChoise = Console.ReadLine();

                if (userChoise == "1" || userChoise == "2" || userChoise == "3")
                {
                    checkUserChoise = false;

                    switch (Convert.ToInt32(userChoise))
                    {
                        case 1:
                            ShowCostsHistory(connection);
                            break;

                        case 2:
                            AddCost(connection);
                            break;

                        case 3:
                            ShowStatistics(connection);
                            break;
                    }

                }
                else
                {
                    Console.WriteLine("Недопустиме значення.");
                }
            }
            while (checkUserChoise);
        }

        private static void ShowCostsHistory(SqliteConnection connection)
        {
            var command = new SqliteCommand("SELECT * FROM COSTS", connection);
            SqliteDataReader reader = command.ExecuteReader();
            Console.WriteLine(reader);
            Console.ReadLine();

            //var result = command.ExecuteNonQuery();


            //Console.WriteLine(result);
            //Console.ReadLine();

            UserChoise(connection);
        }

        private static void AddCost(SqliteConnection connection)
        {
            Cost usersCost = new Cost();

            Console.WriteLine("Введіть опис витрати:");
            usersCost.description = Console.ReadLine();

            string tmpString;

            {
                double tmpDouble;

                do
                {
                    Console.WriteLine("Введіть суму витрати:");
                    tmpString = Console.ReadLine();

                }
                while (!double.TryParse(tmpString, out tmpDouble));

                usersCost.sum = tmpDouble;
            }

            Console.WriteLine("Введіть тип витрати:");
            usersCost.type = Console.ReadLine();


            {
                DateTime tmpDateTime;

                do
                {
                    Console.WriteLine("Введіть дату витрати:");
                    tmpString = Console.ReadLine();

                }
                while (!DateTime.TryParse(tmpString, out tmpDateTime));

                usersCost.date = tmpDateTime;
            }

            var dateStr = usersCost.date.ToString();

            Console.WriteLine(usersCost.description);
            Console.WriteLine(usersCost.sum);
            Console.WriteLine(usersCost.type);
            Console.WriteLine(dateStr);

            Console.ReadLine();

            var command = new SqliteCommand($"INSERT INTO COSTS (Description, Sum, Type, Date) VALUES ('{usersCost.description}', '{usersCost.sum}', '{usersCost.type}', '{dateStr}')", connection).ExecuteNonQuery();

            UserChoise(connection);
        }
        private static void ShowStatistics(SqliteConnection connection)
        {

        }
    }


    public class Cost
    {
        public string description { get; set; }
        public double sum { get; set; }
        public string type { get; set; }
        public DateTime date { get; set; }
    }
}