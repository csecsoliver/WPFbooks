using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static System.Reflection.Metadata.BlobBuilder;

namespace WPFbooks;

public partial class EditWindow : Window
{
    public Guid? bookId;
    public EditWindow(Guid bookId)
    {
        this.bookId = bookId;
        InitializeComponent();
        LoadBookData();
    }

    private void LoadBookData()
    {
        var book = Data.books.Find(book1 => book1.Id == this.bookId);
        LoadGenres();
        if (book == null)
        {
            rbNotStarted.IsChecked = true;
            return;
        }
        
        txtTitle.Text = book.Title;
        txtAuthor.Text = book.Author;
        txtPublishYear.Text = Convert.ToString(book.PublishedYear);

        cbGenre.SelectedItem = book.Genre?.Name;

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
        if (Data.genres.Count(genre => genre.Name == newGenre) == 0 && newGenre != "")
        {
            Data.genres.Add(new Genre(newGenre));
            LoadGenres();
            MessageBox.Show("Új műfaj hozzáadva!", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        else if (Data.genres.Count(genre => genre.Name == newGenre) == 0 && newGenre == "")
        {
            MessageBox.Show("Adjon meg műfaj nevet!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        else if (Data.genres.Count(genre => genre.Name == newGenre) > 0)
        {
            MessageBox.Show("A műfaj létezik!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void btnCancel_Click(object sender, RoutedEventArgs e)
    {
        // MainWindow mainWindow = new MainWindow();
        // mainWindow.Show();
        Close();
    }

    private void btnSave_Click(object sender, RoutedEventArgs e)
    {
        if (Data.books.Count(book => book.Id == bookId) != 0)
        {
            ModifyBookData(Data.books.First(book => book.Id == bookId));
            Data.SaveData();
        }
        else
        {
            Book newBook = new Book();
            if (ModifyBookData(newBook))
            {
                Data.books.Add(newBook);
                Data.SaveData();
            }
        }
        
    }

    private bool ModifyBookData(Book book)
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

        if (!int.TryParse(txtTotalPages.Text, out totalPage) || totalPage < 0)
            errors.Add(txtTotalPages);
        if (!int.TryParse(txtCurrentPage.Text, out currentPage) || currentPage < 0 || (currentPage>0&&(bool)rbNotStarted.IsChecked!) || (currentPage==0&&(bool)rbStarted.IsChecked!) || (currentPage<totalPage&&(bool)rbFinished.IsChecked!))
            errors.Add(txtCurrentPage);

        if(currentPage > totalPage)
        {
            errors.Add(txtCurrentPage);
        }
        
        if(errors.Count > 0)
        {
            ModifyBookError(errors);
            return false;
        }
        else
        {
            book.Author = txtAuthor.Text;
            book.Title = txtTitle.Text;
            book.PublishedYear = publishYear;
            book.GenreName = cbGenre.Text;

            if (rbNotStarted.IsChecked == true)
                book.Status = (BookStatus)0;
            else if (rbStarted.IsChecked == true)
                book.Status = (BookStatus)1;
            else
                book.Status = (BookStatus)2;

            book.CurrentPage = currentPage;
            book.TotalPages = totalPage;

            Close();
            return true;
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