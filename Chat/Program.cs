using StackExchange.Redis;
using System;

namespace Chat
{
    class Program
    {
        static void Main(string[] args)
        {
            const string name = "Client1";
            var options = new ConfigurationOptions();
            options.EndPoints.Add("localhost", 6379);
            options.ClientName = name;

            var redis = ConnectionMultiplexer.Connect(options);
            var db = redis.GetDatabase();

            const string key = "chat:list";
            var command = "";
            Console.WriteLine($"Вы подключены к чату как {name}!");
            //db.StringSet(key, "Test");
            //Console.WriteLine(db.StringGet(key));
            db.ListRightPush(key, $"{name} подключился к чату!");
            int chatCount = Convert.ToInt32(db.ListLength(key));
            while (command != "/exit")
            {
                int chatRefresh = Convert.ToInt32(db.ListLength(key));
                if(chatRefresh > chatCount)
                {
                    for(int i = chatCount; i <= chatRefresh; i++)
                    {
                        Console.WriteLine(db.ListGetByIndex(key,i));
                    }
                    chatCount = chatRefresh;
                }
                command = Console.ReadLine();
                if(command != "/exit" && command != "") db.ListRightPush(key, $"{name}: {command}");
                else if (command == "/exit") db.ListRightPush(key, $"{name} покинул чат!");
            }
            Console.WriteLine("Connection closed...");
        }
    }
}
