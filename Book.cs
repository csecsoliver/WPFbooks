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

    private int _genreId;
    public int GenreId
    {
        get => _genreId;
        set
        {
            Genre = Data.GetGenreById(_genreId);
            _genreId = value;
        }
    }



    public int Year { get; set; }
    public BookStatus Status { get; set; }
}