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

        //1) Используя пример с занятия и прошлое ДЗ реализовать запись в базу данных таких полей,
        //как имя, адрес эл.почты и телефон. 
        string data; // объявили 4 переменные, data для считывания введеных данных, осальные - для записи
        string name;
        string phone;
        string mail;
        Regex regPhone = new Regex(@"((\+7|8)\d{10})"); // создаем регексы
        Regex regName = new Regex(@"((\s|^)[A-Z|a-z]+\D(\s|$))");
        Regex regMail = new Regex(@"([a-zA-Z0-9._-]+@[a-zA-Z0-9._-]+\.[a-zA-Z0-9_-]+)");
        

        Console.WriteLine($"Введите имя, введите телефон в формате +7ХХХХХХХХХХ и введите электронную почту через пробел ");
        data = Console.ReadLine(); // считали данные
        MatchCollection matchFindName = regName.Matches(data);
        name = matchFindName[0].ToString();  //записали name
        MatchCollection matchFindPhone = regPhone.Matches(data);
        phone = matchFindPhone[0].ToString();//записали телефон

        MatchCollection matchFindMail = regMail.Matches(data);
        mail = matchFindPhone[0].ToString();//записали почту
        Console.WriteLine("Выведем записанные данные");
        Console.WriteLine($"Записали данные{name}, {phone}, {mail}");
        string all_data = $"'{name}','{phone}','{mail}'";
        putText.Invoke(all_data); // `Invoke` - это метод, который позволяет вызвать делегат 

    }

}