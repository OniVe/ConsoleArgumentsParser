# ConsoleArgumentsParser
Class for parsing args in C# application


# Example
##Code
```c#
class Program
    {
        static void Main(string[] args)
        {
            var parser = new ArgumentParser('-');

            parser.AddAction("hello", new Action(delegate() { Console.WriteLine("Hello"); }));

            parser.AddAction("walk",new Action<int,int>(delegate (int x,int y)
            {
             Console.WriteLine($"Moved to x:{x} y:{y}");   
            }));

            parser.AddAction("move", new Action<string, string, string, string, string>(
                delegate(string s, string s1, string s2, string s3, string s4)
                {
                   
                    Console.WriteLine($"My movement {s} {s1} {s2} {s3} {s4}");
                }));

            parser.AddAction("push", new Action<string, int, bool, string, bool, int>(
                delegate(string s, int i, bool arg3, string arg4, bool arg5, int arg6)
                {
                    Console.WriteLine($"Push {s} {i} => {arg3} \n push {arg4} {arg6} => {arg5}" );
                }));
            parser.Parse(args);

            Console.ReadLine();
        }
```
##Output
