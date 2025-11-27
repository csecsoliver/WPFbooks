using System.Windows;
using static System.Reflection.Metadata.BlobBuilder;

namespace WPFbooks;

public partial class EditWindow : Window
{
    public required Guid? bookId;
    public EditWindow()
    {
        InitializeComponent();
        LoadBookData();
    }

    private void LoadBookData()
    {
        var books = Data.books;
        //if(books.Count(book => book.Id == bookId) == 0)
        //    rbNotStarted.IsChecked = true;
        //    return;

        //var book = books.First(book => book.Id == bookId);
        var book = books[2]; //For testing

        txtTitle.Text = book.Title;
        txtAuthor.Text = book.Author;

        LoadGenres();
        cbGenre.SelectedItem = book.Genre.Name;

        switch (book.Status)
        {
            case BookStatus.ToRead:
                rbNotStarted.IsChecked = true;
                break;
            case BookStatus.Reading:
                rbStarted.IsChecked = true;
                break;
            case BookStatus.Completed:
                rbFinished.IsChecked = true;
                break;
            default:
                break;
        }

        txtCurrentPage.Text = Convert.ToString(book.CurrentPage);
        txtTotalPages.Text = Convert.ToString(book.TotalPages);
    }

    private void LoadGenres()
    {
        var genres = Data.genres;
        if(genres == null) return;

        cbGenre.Items.Clear();
        foreach(var g in genres)
        {
            cbGenre.Items.Add(g.Name);
        }
    }

    private void btnNewGenre_Click(object sender, RoutedEventArgs e)
    {
        var newGenre = cbGenre.Text;
        if (Data.genres.Count(genre => genre.Name == newGenre) == 0)
        {
            Data.genres.Add(new Genre(newGenre));
            LoadGenres();
        }
    }

    private void btnCancel_Click(object sender, RoutedEventArgs e)
    {

    }

    private void btnSave_Click(object sender, RoutedEventArgs e)
    {
        if (Data.books.Count(book => book.Id == bookId) == 0)
            NewBook();

        else
            ModifyBook(Data.books.First(book => book.Id == bookId));

    }

    private void ModifyBook(Book book)
    {
        if(txtAuthor.Text != "" && txtTitle.Text != "" && Data.genres.Any(genre => genre.Name == cbGenre.Text) )    //Number TryParse check
        {

        }
    }

    private void NewBook()
    {
        throw new NotImplementedException();
    }
}