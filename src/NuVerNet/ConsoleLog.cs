namespace NuVerNet;

public class ConsoleLog
{
    public static void WriteLine(string message, ConsoleColor color = ConsoleColor.White)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    public static void Success(string message) => WriteLine(message, ConsoleColor.Green);

    public static void Error(string message) => WriteLine(message, ConsoleColor.Red);

    public static void Info(string message) => WriteLine(message, ConsoleColor.Cyan);
}