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
        var book = item!.Tag as Book;
        title.Content = book.Title;
        author.Content = book.Author;
        genre.Content = book.Genre;
        status.Content = book.Status;
        totalpages.Content = book.TotalPages.ToString();
        currentpage.Content = book.CurrentPage.ToString();

    }
}