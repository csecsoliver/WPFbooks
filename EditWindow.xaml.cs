using System.Windows;

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
        var books = Data.books; /*
        if (bookId == null)
            return; //Don't have to load in book datas
        //var book = books[Convert.ToInt32(this.bookId)];
        */
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

        foreach(var g in genres)
        {
            cbGenre.Items.Add(g.Name);
        }
    }
}