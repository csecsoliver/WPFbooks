using System.IO;
using System.Text.Json;

namespace WPFbooks;

public static class Data
{
    static string dataPath = "books.json";
    static List<Book>? books;
    static Data()
    {
        LoadData();
    }

    public static void LoadData()
    {
        using var sr = new StreamReader(dataPath);
        var data = sr.ReadToEnd();
        books = JsonSerializer.Deserialize<List<Book>>(data);
    }

    public static void SaveData()
    {
        using var sw = new StreamWriter(dataPath);
        var data = JsonSerializer.Serialize(books);
        sw.Write(data);
    }
}