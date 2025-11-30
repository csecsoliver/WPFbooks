using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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
        txtPublishYear.Text = Convert.ToString(book.PublishedYear);

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
        //Return to MainWindow
    }

    private void btnSave_Click(object sender, RoutedEventArgs e)
    {
        if (Data.books.Count(book => book.Id == bookId) != 0)
        {
            ModifyBookData(Data.books.First(book => book.Id == bookId));

        }
        else
        {
            Book newBook = new Book();
            Data.books.Add(newBook);
            ModifyBookData(newBook);
        }

    }

    private void ModifyBookData(Book book)
    {
        List<Control> errors = new List<Control>();
        int publishYear = -1;
        int currentPage = -1;
        int totalPage = -1;
        if (txtAuthor.Text == "")
            errors.Add(txtAuthor);
        if (txtTitle.Text == "")
            errors.Add(txtTitle);

        if (!int.TryParse(txtPublishYear.Text, out publishYear))
            errors.Add(txtPublishYear);

        if (!Data.genres.Any(genre => genre.Name == cbGenre.Text))
            errors.Add(cbGenre);

        if (!int.TryParse(txtCurrentPage.Text, out currentPage) || currentPage < 0)
            errors.Add(txtCurrentPage);
        if (!int.TryParse(txtTotalPages.Text, out totalPage) || totalPage < 0)
            errors.Add(txtTotalPages);

        if(currentPage > totalPage)
        {
            errors.Add(txtCurrentPage);
        }
        
        if(errors.Count > 0)
        {
            ModifyBookError(errors);
        }
        else
        {
            book.Author = txtAuthor.Text;
            book.Title = txtTitle.Text;
            book.PublishedYear = publishYear;
            book.Genre = Data.genres.First(genre => genre.Name == cbGenre.Text);

            if (rbNotStarted.IsChecked == true)
                book.Status = (BookStatus)0;
            else if (rbStarted.IsChecked == true)
                book.Status = (BookStatus)1;
            else
                book.Status = (BookStatus)2;

            book.CurrentPage = currentPage;
            book.TotalPages = totalPage;

            //Data.SaveData();      ---------> AFTER TESTING UNCOMMENT
        }
    }

    private void ModifyBookError(List<Control> errors)
    {
        foreach(Control e in errors)
        {
            getErrorLabel(e).Visibility = Visibility.Visible;
            e.BorderBrush = Brushes.Red;
        }
    }

    private void txt_TextChanged(object sender, TextChangedEventArgs e)
    {
        var errorLbl = getErrorLabel((sender as Control)!);

        if (errorLbl.Visibility == Visibility.Visible)
        {
            errorLbl.Visibility = Visibility.Hidden;
            (sender as Control)!.ClearValue(BorderBrushProperty);
        }
    }

    private Label getErrorLabel(Control control)
    {
        return (FindName(control.Tag as String) as Label)!;
    }
}