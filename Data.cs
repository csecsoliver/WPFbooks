using System.IO;
using System.Text.Json;

namespace WPFbooks;

public static class Data
{
    static string dataPath = "books.json";
    static string genrePath = "genres.json";
    public static List<Book> books = [];
    public static List<Genre> genres = [];
    static Data()
    {
        LoadData();
    }

    public static void LoadData()
    {
        using var file = File.OpenRead(genrePath);
        genres = JsonSerializer.Deserialize<List<Genre>>(file) ?? [];
        using var sr = new StreamReader(dataPath);
        var data = sr.ReadToEnd();
        books = JsonSerializer.Deserialize<List<Book>>(data) ?? [];
        var local = books;
        return;
    }

    public static void SaveData()
    {
        using var sw = new StreamWriter(dataPath);
        var data = JsonSerializer.Serialize(books);
        sw.Write(data);
        using var sw2 = new StreamWriter(genrePath);
        var genreData = JsonSerializer.Serialize(genres);
        sw2.Write(genreData);
        var local = books;
        
    }

    public static Genre GetGenreByName(string genreName)
    {
        return genres.FirstOrDefault(genre => genre.Name == genreName)??
        throw new KeyNotFoundException();
    }
}