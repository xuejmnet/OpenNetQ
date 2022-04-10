

/*
* @Author: xjm
* @Description:
* @Date: DATE
* @Email: 326308290@qq.com
*/
namespace OpenNetQ
{
    public class AppExister
    {
        public static async Task Exist(string exitCommand,Func<Task> stopFunc)
        {
            Console.WriteLine($"input '{exitCommand}' to exist");
            while (true)
            {
                var readLine = Console.ReadLine();
                if (exitCommand == readLine)
                {
                    Console.WriteLine("app will exist input y/n?");
                     readLine = Console.ReadLine();
                     if ("y".Equals(readLine, StringComparison.OrdinalIgnoreCase) || "yes".Equals(readLine, StringComparison.OrdinalIgnoreCase))
                     {
                         Console.WriteLine("app is exist");
                         await stopFunc();
                         break;
                     }
                }
            }
        }
    }
}