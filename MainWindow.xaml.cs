using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPFbooks;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        var load = Data.books;
    }

    private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        var booklist = (ListBox)FindName("BooksListBox")!;
        booklist.Items.Clear();
        foreach (var book in Data.books)
        {
            var listitem = new ListBoxItem();
            listitem.Content = book.Title;
            listitem.Tag = book;
            listitem.Selected += Item_Click;
            booklist.Items.Add(listitem);
        }
        var genrecombo = (ComboBox)FindName("GenreComboBox")!;
        genrecombo.Items.Clear();
        var anyitem = new ComboBoxItem();
        anyitem.Content = "Mindegy";
        anyitem.IsSelected = true;
        genrecombo.Items.Add(anyitem);
        foreach (var genre in Data.genres)
        {
            var comboitem = new ComboBoxItem();
            comboitem.Content = genre.Name;
            genrecombo.Items.Add(comboitem);
        }
        ResetButton_OnClick(sender, e);
        

    }

    private void Item_Click(object sender, RoutedEventArgs e)
    {
        var title = FindName("TitleLabel") as Label;
        var author = FindName("AuthorLabel") as Label;
        var genre = FindName("GenreLabel") as Label;
        var status = FindName("StatusLabel") as Label;
        var totalpages = FindName("TotalPagesLabel") as Label;
        var currentpage = FindName("CurrentPageLabel") as Label;
        var item = sender as ListBoxItem;
        var book = (item!.Tag as Book)!;
        item.IsSelected = true;
        title!.Content = book.Title;
        author!.Content = book.Author;
        genre!.Content = book.Genre?.Name ?? "Ismeretlen";
        switch (book.Status)
        {
            case BookStatus.Completed:
                status!.Content = "Befejezett";
                break;
            case BookStatus.Reading:
                status!.Content = "Elkezdett";
                break;
            case BookStatus.ToRead:
                status!.Content = "Nem elkezett";
                break;
            default:
                status!.Content = "Ismeretlen";
                break;
        }
        
        totalpages!.Content = book.TotalPages.ToString();
        currentpage!.Content = book.CurrentPage.ToString();

    }

    private void SearchButton_Click(object sender, RoutedEventArgs e)
    {
        var title = FindName("TitleInput") as TextBox;
        var author = FindName("AuthorInput") as TextBox;
        var genre = FindName("GenreComboBox") as ComboBox;
        var status = "Any";
        if (FindName("ToReadRadioButton") is RadioButton toread && toread.IsChecked == true)
        {
            status = "ToRead";
        }
        else if (FindName("ReadingRadioButton") is RadioButton reading && reading.IsChecked == true)
        {
            status = "Reading";
        }
        else if (FindName("CompletedRadioButton") is RadioButton completed && completed.IsChecked == true)
        {
            status = "Completed";
        }

        var genreName = "Any";
        foreach (var option in genre.Items)   
        {
            if (option is ComboBoxItem comboBoxItem && comboBoxItem.IsSelected)
            {
                genreName = comboBoxItem.Content.ToString();
            }
        }
        var results = (ListBox)FindName("BooksListBox")!;
        results.Items.Clear();
        foreach (var book in Data.books)
        {
            if ((string.IsNullOrEmpty(title!.Text) || book.Title.ToLower().Contains(title.Text.ToLower())) &&
                (string.IsNullOrEmpty(author!.Text) || book.Author.ToLower().Contains(author.Text.ToLower())) &&
                (status == "Any" || book.Status.ToString() == status) &&
                (genreName == "Mindegy" || book.Genre?.Name == genreName)
                )
            {
                var listitem = new ListBoxItem();
                listitem.Content = book.Title;
                listitem.Tag = book;
                listitem.Selected += Item_Click;
                results.Items.Add(listitem);

            }
        }
        ClearBook();
    }

    private void ResetButton_OnClick(object sender, RoutedEventArgs e)
    {
        var title = FindName("TitleInput") as TextBox;
        var author = FindName("AuthorInput") as TextBox;
        var genre = FindName("GenreComboBox") as ComboBox;
        var any = FindName("AnyRadioButton") as RadioButton;

        title!.Text = "";
        author!.Text = "";
        genre!.SelectedIndex = 0;
        any!.IsChecked = true;

        SearchButton_Click(sender, e);
    }

    private void EditBook_OnClick(object sender, RoutedEventArgs e)
    {
        var ListBox = (ListBox)FindName("BooksListBox")!;
        var selectedBookId = Guid.Empty;
        foreach (ListBoxItem item in ListBox.Items)
        {
            if (item.IsSelected)
            {
                selectedBookId = (item.Tag as Book)!.Id;
            }
        }
        if (selectedBookId == Guid.Empty)
        {
            MessageBox.Show("Kérlek válassz ki egy könyvet a szerkesztéshez.", "Hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        var editWindow = new EditWindow(selectedBookId);
        editWindow.ShowDialog();
        MainWindow_OnLoaded(sender, e);
    }
    private void NewBook_OnClick(object sender, RoutedEventArgs e)
    {
        var editWindow = new EditWindow(Guid.Empty);
        editWindow.ShowDialog();
        MainWindow_OnLoaded(sender, e);
    }

    private void ClearBook()
    {
        var title = FindName("TitleLabel") as Label;
        var author = FindName("AuthorLabel") as Label;
        var genre = FindName("GenreLabel") as Label;
        var status = FindName("StatusLabel") as Label;
        var totalpages = FindName("TotalPagesLabel") as Label;
        var currentpage = FindName("CurrentPageLabel") as Label;

        title!.Content = "";
        author!.Content = "";
        genre!.Content = "";
        status!.Content = "";
        totalpages!.Content = "";
        currentpage!.Content = "";
    }

    private void DeleteBook_OnClick(object sender, RoutedEventArgs e)
    {
        var listBox = (ListBox)FindName("BooksListBox")!;
        Book? selectedBook = null;
        
        foreach (ListBoxItem item in listBox.Items)
        {
            if (item.IsSelected)
            {
                selectedBook = item.Tag as Book;
                break;
            }
        }

        if (selectedBook == null)
        {
            MessageBox.Show("Kérlek válassz ki egy könyvet a törléshez.", "Hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var confirmResult = MessageBox.Show(
            $"Biztosan törölni szeretnéd a(z) '{selectedBook.Title}' című könyvet?",
            "Törlés megerősítése",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (confirmResult == MessageBoxResult.Yes)
        {
            try
            {
                Data.books.Remove(selectedBook);
                Data.SaveData();
                MessageBox.Show("A könyv sikeresen törölve lett.", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);
                ClearBook();
                MainWindow_OnLoaded(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt a könyv törlése során: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
