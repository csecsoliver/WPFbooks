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
        var booklist = (ListBox) FindName("BooksListBox")!;
        foreach (var book in Data.books)
        {
            var listitem = new ListBoxItem();
            listitem.Content = book.Title;
            listitem.Tag = book;
            listitem.Selected += Item_Click;
            booklist.Items.Add(listitem);
        }
        var genrecombo = (ComboBox) Findname("GenreComboBox")!;

    }

    private void Item_Click(object sender, RoutedEventArgs e)
    {
        var title = FindName("TitleLabel") as Label;
        var author = FindName("AuthorLabel") as Label;
        var genre = FindName("GenreLabel") as Label;
        var status =  FindName("StatusLabel") as Label;
        var totalpages = FindName("TotalPagesLabel") as Label;
        var currentpage = FindName("CurrentPageLabel") as Label;
        var item = sender as ListBoxItem;
        var book = (item!.Tag as Book)!;
        title!.Content = book.Title;
        author!.Content = book.Author;
        genre!.Content = book.Genre.Name;
        status!.Content = book.Status;
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
        var results = (ListBox) FindName("BooksListBox")!;
        results.Items.Clear();
        foreach (var book in Data.books)
        {
            if ((string.IsNullOrEmpty(title!.Text) || book.Title.Contains(title.Text)) &&
                (string.IsNullOrEmpty(author!.Text) || book.Author.Contains(author.Text)) &&
                (status == "Any" || book.Status.ToString() == status)) &&
                ()
            {
                var listitem = new ListBoxItem();
                listitem.Content = book.Title;
                listitem.Tag = book;
                listitem.Selected += Item_Click;
                results.Items.Add(listitem);
            }
        }
    }
}
