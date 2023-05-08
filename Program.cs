using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using DbConnectionExample;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;
using System.IO;
using System.Threading;

namespace DbConnectionExample
{
    public class userData // создали класс для записи данных пользователя
    {
        public userData(string name, string phoneNumber, string mail) { _name = name;  _numberPhone = phoneNumber; _mail = mail; }
        //создали конструктор класса
        public string _name { get; } // свойства геттер
        public string _mail { get; }// свойства геттер
        public string _numberPhone { get; }// свойства геттер
    }
    delegate void myDel(string _text); //delegate 
    public class myConnectToMSSQLDB  //класс для связи с БД
    {
        public SqlConnection connection;
        public myConnectToMSSQLDB()
        {
            string conStr = @"Data Source=(localdb)\MSSQLLocalDB;" + /* Имя сервера */
                        "Initial Catalog=master;" + /* БД подключения*/
                        "Integrated Security=True;" + /* Использование уч.записи Windows */
                        "Connect Timeout=30;" + /* Таймаут в секундах*/
                        "Encrypt=False;" + /* Поддержка шифрования, работает в паре со сл.параметром */
                        "TrustServerCertificate=False;" + /* Только при подключении к экземпляру SQL Server с допустимым сертификатом. Если ключевому слову TrustServerCertificate присвоено значение true, то транспортный уровень будет использовать протокол SSL для шифрования канала и не пойдет по цепочке сертификатов для проверки доверия. */
                        "ApplicationIntent=ReadWrite;" + /* Режим подключения*/
                        "MultiSubnetFailover=False;"; /* true - поддержка уровня доступности: оптимизирует работу для пользователей одной подсети*/
            var myConn = new SqlConnection(conStr);
            try
            {
                myConn.Open(); // открыли соединение с базоый данных
                Console.WriteLine($"Установлено соединение с параметрами {conStr}");
            }
            catch
            {
                Console.WriteLine($"Не удалось установить соединение с параметрами {conStr}");
            }
            finally
            {
                //myConn.Close();
                connection = myConn;
                Console.WriteLine($"Закрыто соединение с параметрами {conStr}");

            }

        }

    }

}
internal class Program
{
    static void Print(string _text)
    {
        Console.WriteLine(_text);
    }
    static void WriteToFile(string _text) // запись в файл
    {
        string _name = "Data.txt";
        var sw = new StreamWriter(_name, true);
        sw.WriteLine(_text);

        sw.Close();
        //using (var sw01 = new StreamWriter(_name))
        //{
        //    sw01.WriteLine(_text, true);
        //}

    }
    static void WriteToDB(string _text)
    {
        var connect = new myConnectToMSSQLDB();
        string _cmd = "USE users;\n";
        _cmd += @"INSERT INTO [dateUs] ([name],[phone],[mail]) ";// 
        _cmd += @"VALUES (N";
        _cmd += _text;
        _cmd += ");";

        /* Конструктор по умолчанию */
        SqlCommand sqlCommand = new SqlCommand();
        sqlCommand.Connection = connect.connection;
        sqlCommand.CommandText = _cmd;

        /* Использование нестатического метода SqlConnection */
        //var sqlCommand = myConn.CreateCommand();
        //sqlCommand.CommandText = _cmd;

        /* Использование перегрузки конструктора с 2-мя параметрами */
        //SqlCommand sqlCommand = new SqlCommand(_cmd, myConn);

        var dataReader = sqlCommand.ExecuteReader();
        Console.WriteLine(_cmd);
        while (dataReader.Read())
        {
            int row = dataReader.FieldCount; // Вспомогательная переменная, количество возвращённых столбцов
            for (int i = 0; i < row; i++)
            {
                Console.Write("  " + dataReader[i].ToString());
            }
            Console.WriteLine();
        }
    }
    static void Main(string[] args)
    {
        myDel putText; //делегат
        putText = WriteToDB;
        putText += Print;
        putText += WriteToFile;


        string data; // объявили 4 переменные, data для считывания введеных данных, осальные - для записи
        string name;
        string phone;
        string mail;
        List<userData> users = new List<userData>(); // создали лист users для записи данных
        Regex regPhone = new Regex(@"^\+7\(\d{3}\)\d{3}-\d{2}-\d{2}$"); // создаем регексы
        Regex regName = new Regex(@"^[a-zA-Z]+$");
        Regex regMail = new Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
        do
        {
            Console.WriteLine($"Введите имя: ");
            data = Console.ReadLine();
            MatchCollection matchFindName = regName.Matches(data);
            name = matchFindName[0].ToString();

            Console.WriteLine($"Введите телефон в формате +7(ХХХ)ХХХ-ХХ-ХХ :");
            data = Console.ReadLine();
            MatchCollection matchFindPhone = regPhone.Matches(data);
            phone = matchFindPhone[0].ToString();
            Console.WriteLine($"Введите электронную почту:");
            data = Console.ReadLine();
            MatchCollection matchFindMail = regMail.Matches(data);
            mail = matchFindPhone[0].ToString();
            userData user = new userData(name, mail, phone); //создаем объект класса
            users.Add(user);
            string user_list = $"'{name}','{phone}','{mail}'"; // добавляем данные в лист
            putText.Invoke(user_list);// метод вызова делегата 
          
        } while (true);

    }
}