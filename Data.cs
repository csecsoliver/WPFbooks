﻿using System.IO;
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
        using (var sw = new StreamWriter(dataPath))
        {
            var data = JsonSerializer.Serialize(books);
            sw.Write(data);
            sw.Flush();
        }
        
        using (var sw2 = new StreamWriter(genrePath))
        {
            var genreData = JsonSerializer.Serialize(genres);
            sw2.Write(genreData);
            sw2.Flush();
        }
    }

    public static Genre GetGenreByName(string genreName)
    {
        return genres.FirstOrDefault(genre => genre.Name == genreName)??
        throw new KeyNotFoundException();
    }

    public static int GetBookCountByGenre(string genreName)
    {
        return books.Count(book => book.GenreName == genreName);
    }

    public static bool CanDeleteGenre(string genreName)
    {
        return GetBookCountByGenre(genreName) == 0;
    }

    public static bool DeleteGenre(string genreName)
    {
        if (!CanDeleteGenre(genreName))
            return false;
        
        var genre = genres.FirstOrDefault(g => g.Name == genreName);
        if (genre != null)
        {
            genres.Remove(genre);
            SaveData();
            return true;
        }
        return false;
    }
}