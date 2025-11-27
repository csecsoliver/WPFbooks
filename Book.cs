using System.Runtime.InteropServices.JavaScript;
using System.Text.Json.Serialization;

namespace WPFbooks;

public class Book
{
    public string Title { get; set; }
    public string Author { get; set; }

    [JsonIgnore]
    public Guid Id
    {
        get;
    } = System.Guid.NewGuid();
    
    public Genre Genre;

    private string _genreName;
    public string GenreName
    {
        get => _genreName;
        set
        {
            Genre = Data.GetGenreByName(value);
            _genreName = value;
        }
    }



    public int Year { get; set; }
    public BookStatus Status { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
}