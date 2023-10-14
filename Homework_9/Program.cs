using Microsoft.Data.Sqlite;
using System.Text;

namespace Homework_9
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            using (var connection = new SqliteConnection($"DataSource={Directory.GetCurrentDirectory()};Filename=SqliteHomework.db;"))
            {
                connection.Open();

                //var com = new SqliteCommand("DROP TABLE COSTS", connection).ExecuteNonQuery();

                var com = new SqliteCommand("DELETE FROM COSTS", connection).ExecuteNonQuery();

                var createTableCom = new SqliteCommand(@"CREATE TABLE IF NOT EXISTS COSTS (id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, 
                Description TEXT, Suma TEXT NOT NULL, Type TEXT, Date TEXT NOT NULL)", connection);
                createTableCom.ExecuteNonQuery();

                connection.Close();

                UserChoise(connection);
            }
        }


        private static void UserChoise(SqliteConnection connection)
        {
            using(connection)
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
           
        }

        private static void ShowCostsHistory(SqliteConnection connection)
        {
            using(connection)
            {
                Console.Clear();
                connection.Open();

                var reader = new SqliteCommand("SELECT * FROM COSTS", connection).ExecuteReader();
                
                if(!reader.HasRows)
                {
                    Console.WriteLine("В таблиці немає жодного запису.\n");

                    UserChoise(connection);
                }
                else
                {
                    while(reader.Read())
                    {
                        Console.WriteLine($"Опис витрати: {reader.GetValue(1)}");
                        Console.WriteLine($"Сума витрати: {reader.GetValue(2)}");
                        Console.WriteLine($"Тип витрати: {reader.GetValue(3)}");
                        Console.WriteLine($"Дата витрати: {reader.GetValue(4)}");
                        Console.WriteLine();
                    }
                }
                connection.Close();
            }



            UserChoise(connection);
        }

        private static void AddCost(SqliteConnection connection)
        {
            Console.Clear();

            using (connection)
            {
                connection.Open();

                Cost usersCost = new Cost();

                Console.WriteLine("Введіть опис витрати:");
                usersCost.description = Console.ReadLine();

                string tmpString;

                {
                    decimal tmpDecimal;

                    do
                    {
                        Console.WriteLine("Введіть суму витрати:");
                        tmpString = Console.ReadLine();

                    }
                    while (!decimal.TryParse(tmpString, out tmpDecimal));

                    usersCost.sum = tmpDecimal;
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

                var command = new SqliteCommand($"INSERT INTO COSTS (Description, Suma, Type, Date) VALUES ('{usersCost.description}', '{usersCost.sum}', '{usersCost.type}', '{dateStr}')", connection).ExecuteNonQuery();

                connection.Close();

                UserChoise(connection);
            }
            
        }
        private static void ShowStatistics(SqliteConnection connection)
        {
            Console.Clear();

            using (connection)
            {
                connection.Open();

                var reader = new SqliteCommand("SELECT * FROM COSTS", connection).ExecuteReader();

                if (!reader.HasRows)
                {
                    Console.WriteLine("В таблиці немає жодного запису.\n");
                }
                else
                {
                    var maxSum = new SqliteCommand("SELECT max(SUMA) from COSTS", connection).ExecuteScalar();
                    var minSum = new SqliteCommand("SELECT min(SUMA) from COSTS", connection).ExecuteScalar();
                    var avgSum = new SqliteCommand("SELECT avg(SUMA) from COSTS", connection).ExecuteScalar();

                    Console.WriteLine(avgSum.GetType());
                    Console.ReadLine();

                    Console.WriteLine($"Максимальна сума: {maxSum} | Мінімальна сума: {minSum} | Середня сума: {avgSum}");
                }

                connection.Close();

                UserChoise(connection);
            }
        }
    }


    public class Cost
    {
        public string description { get; set; }
        public decimal sum { get; set; }
        public string type { get; set; }
        public DateTime date { get; set; }
    }
}