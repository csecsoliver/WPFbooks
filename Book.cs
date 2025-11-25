namespace WPFbooks;

public class Book
{
    public string Title { get; set; }
    public string Author { get; set; }
    public BookGenre Genre { get; set; }
    public int Year { get; set; }
    public BookStatus Status { get; set; }
    public Book(string title, string author, BookStatus status)
    {
        
    }
}