using System.Diagnostics;

class Program
{
    static void Main(string[] args)
    {
        RootPage();
    }

    static void RootPage()
    {
        DriveInfo[] allDrives = ShowDrivers();
        int pos = Menu.Manager(count: allDrives.Length);

        ShowContents(allDrives[pos - 1].ToString());

    }

    private static DriveInfo[] ShowDrivers()
    {
        DriveInfo[] allDrives = DriveInfo.GetDrives();
        foreach (DriveInfo d in allDrives)
        {
            Console.Write("  " + d.Name);
            if (d.IsReady == true)
            {
                Console.WriteLine( "           свободно места:" + d.TotalFreeSpace / 1073741824 + "гб из " + d.TotalSize / 1073741824 + "гб");
            }
        }
        return allDrives;
    }

    private static void ShowContents(string path)
    {
        string[] contents = Directory.GetDirectories(path).Concat(Directory.GetFiles(path)).ToArray();
        foreach (string c in contents)
        {
            ContentData(c);
        }

        int pos = Menu.Manager(count: contents.Length);
        if (pos == -1)
            OpenPreviousePath(path);
        else
            OpenNextPath(contents[pos - 1]);
    }

    private static void ContentData(string content)
    {
        if (File.Exists(content))
        {
            FileInfo fileInfo = new FileInfo(content);
            Console.WriteLine("  " + fileInfo.FullName + "   дата создания: " + fileInfo.CreationTime + " размер файла: " + fileInfo.Length);
        }
        else
        {
            DirectoryInfo driveInfo = new DirectoryInfo(content);
            Console.WriteLine("  " + driveInfo.FullName + "   дата создания: " + driveInfo.CreationTime);
        }
    }

    private static void OpenPreviousePath(string path)
    {
        Console.Clear();
        string[] previouse = path.Split(@"\");
        if (previouse[previouse.Length - 1] == "")
        {
            previouse = previouse.SkipLast(1).ToArray();
        }
        string newPath = "";
        for (int i = 0; i < previouse.Length - 1; i++)
        {
            newPath += previouse[i] + @"\";
        }
        if (newPath != "")
            ShowContents(newPath);
        else
            RootPage();
    }

    private static void OpenNextPath(string path)
    {
        if (File.Exists(path))
            OpenFile(path);
        else
            ShowContents(path);
    }

    private static void OpenFile(string path)
    {
        Process proc = new();
        proc.StartInfo.FileName = path;
        proc.StartInfo.UseShellExecute = true;
        proc.Start();
        OpenPreviousePath(path);
    }


}

public class Menu
{
    public static int Manager(int startPos = 0, int count = 3, string arrow = "->") 
    { 
        string empty = new string(' ', arrow.Length);
        int i = startPos;
        Console.SetCursorPosition(0, startPos);
        Console.Write(arrow);
        ConsoleKeyInfo key;
        for (; ; )
        {
            key = Console.ReadKey(true);
            switch (key.Key)
            {
                case ConsoleKey.DownArrow:
                    if (i == count + startPos - 1)
                        continue;
                    Console.SetCursorPosition(0, i);
                    Console.Write(empty);
                    Console.SetCursorPosition(0, ++i);
                    Console.Write(arrow);
                    break;
                case ConsoleKey.UpArrow:
                    if (i == startPos)
                        continue;
                    Console.SetCursorPosition(0, i);
                    Console.Write(empty);
                    Console.SetCursorPosition(0, --i);
                    Console.Write(arrow);
                    break;
                case ConsoleKey.Backspace:
                    return -1;
                case ConsoleKey.Escape:
                    Environment.Exit(0);
                    break;
                case ConsoleKey.Enter:
                    Console.Clear();
                    return i + 1 - startPos;
            }
        }
    }
}