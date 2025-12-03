using System.Windows;
using System.Windows.Controls;

namespace WPFbooks;

public partial class GenreManagerWindow : Window
{
    private class GenreViewModel
    {
        public string Name { get; set; }
        public string BookCount { get; set; }

        public GenreViewModel(Genre genre, int bookCount)
        {
            Name = genre.Name;
            BookCount = $"Könyvek: {bookCount}";
        }
    }

    private Genre? selectedGenre = null;
    private bool isNewGenre = false;

    public GenreManagerWindow()
    {
        InitializeComponent();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        LoadGenres();
    }

    private void LoadGenres()
    {
        GenreListBox.Items.Clear();
        foreach (var genre in Data.genres)
        {
            int bookCount = Data.GetBookCountByGenre(genre.Name);
            var viewModel = new GenreViewModel(genre, bookCount);
            GenreListBox.Items.Add(viewModel);
        }
        
        ClearSelection();
    }

    private void GenreListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (GenreListBox.SelectedItem == null)
        {
            ClearSelection();
            return;
        }

        var viewModel = (GenreViewModel)GenreListBox.SelectedItem;
        selectedGenre = Data.genres.FirstOrDefault(g => g.Name == viewModel.Name);
        
        if (selectedGenre != null)
        {
            isNewGenre = false;
            txtGenreName.Text = selectedGenre.Name;
            int bookCount = Data.GetBookCountByGenre(selectedGenre.Name);
            lblBookCount.Text = $"Könyvek száma: {bookCount}";
            
            txtGenreName.IsEnabled = true;
            btnSave.IsEnabled = true;
            btnDelete.IsEnabled = bookCount == 0;
            
            lblErrorMessage.Visibility = Visibility.Collapsed;
        }
    }

    private void btnNewGenre_Click(object sender, RoutedEventArgs e)
    {
        isNewGenre = true;
        selectedGenre = null;
        GenreListBox.SelectedItem = null;
        
        txtGenreName.Text = "";
        lblBookCount.Text = "Könyvek száma: 0";
        
        txtGenreName.IsEnabled = true;
        btnSave.IsEnabled = true;
        btnDelete.IsEnabled = false;
        
        lblErrorMessage.Visibility = Visibility.Collapsed;
        txtGenreName.Focus();
    }

    private void btnSave_Click(object sender, RoutedEventArgs e)
    {
        lblErrorMessage.Visibility = Visibility.Collapsed;
        
        if (string.IsNullOrWhiteSpace(txtGenreName.Text))
        {
            lblErrorMessage.Text = "A műfaj neve nem lehet üres!";
            lblErrorMessage.Visibility = Visibility.Visible;
            return;
        }

        string newName = txtGenreName.Text.Trim();

        if (isNewGenre)
        {
            // Check if genre already exists
            if (Data.genres.Any(g => g.Name.Equals(newName, StringComparison.OrdinalIgnoreCase)))
            {
                lblErrorMessage.Text = "Ez a műfaj már létezik!";
                lblErrorMessage.Visibility = Visibility.Visible;
                return;
            }

            // Add new genre
            var newGenre = new Genre(newName);
            Data.genres.Add(newGenre);
            Data.SaveData();
            
            MessageBox.Show("Új műfaj sikeresen létrehozva!", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);
            LoadGenres();
        }
        else if (selectedGenre != null)
        {
            // Check if renaming to an existing genre name
            if (!selectedGenre.Name.Equals(newName, StringComparison.OrdinalIgnoreCase) &&
                Data.genres.Any(g => g.Name.Equals(newName, StringComparison.OrdinalIgnoreCase)))
            {
                lblErrorMessage.Text = "Ez a műfaj név már használatban van!";
                lblErrorMessage.Visibility = Visibility.Visible;
                return;
            }

            // Update existing genre
            string oldName = selectedGenre.Name;
            selectedGenre.Name = newName;
            
            // Update all books that reference this genre
            if (oldName != newName)
            {
                foreach (var book in Data.books.Where(b => b.GenreName == oldName))
                {
                    book.GenreName = newName;
                }
            }
            
            Data.SaveData();
            
            MessageBox.Show("Műfaj sikeresen frissítve!", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);
            LoadGenres();
        }
    }

    private void btnDelete_Click(object sender, RoutedEventArgs e)
    {
        if (selectedGenre == null)
            return;

        if (!Data.CanDeleteGenre(selectedGenre.Name))
        {
            lblErrorMessage.Text = "Nem törölhető olyan műfaj, amelyhez könyvek tartoznak!";
            lblErrorMessage.Visibility = Visibility.Visible;
            return;
        }

        var result = MessageBox.Show(
            $"Biztosan törölni szeretné a '{selectedGenre.Name}' műfajt?",
            "Megerősítés",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes)
        {
            if (Data.DeleteGenre(selectedGenre.Name))
            {
                MessageBox.Show("Műfaj sikeresen törölve!", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadGenres();
            }
            else
            {
                MessageBox.Show("Hiba történt a műfaj törlésekor.", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private void btnClose_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void ClearSelection()
    {
        selectedGenre = null;
        isNewGenre = false;
        txtGenreName.Text = "";
        lblBookCount.Text = "";
        
        txtGenreName.IsEnabled = false;
        btnSave.IsEnabled = false;
        btnDelete.IsEnabled = false;
        
        lblErrorMessage.Visibility = Visibility.Collapsed;
    }
}

