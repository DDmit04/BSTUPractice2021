using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Program
{
    /// <summary>
    /// Логика взаимодействия для CollectionRenameWindow.xaml
    /// </summary>
    public partial class CollectionRenameWindow : Window
    {
        public string collectionNewName { get; protected set; }
        public string oldCollectionName { get; }
        public CollectionRenameWindow(string collectionName)
        {
            InitializeComponent();
            oldCollectionName = collectionName;
            NewCollectionNameTextBox.Text = collectionName;
            NewCollectionNameTextBox.Focus();
            NewCollectionNameTextBox.SelectionStart = 0;
            NewCollectionNameTextBox.SelectionLength = NewCollectionNameTextBox.Text.Length;
            OkBtn.IsEnabled = false;
        }

        private void OkBtn_Click(object sender, RoutedEventArgs e)
        {
            SaveNewName();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (OkBtn.IsEnabled && e.Key == Key.Enter)
            {
                SaveNewName();
            }
        }

        private void NewCollectionNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (IsLoaded)
            {
                collectionNewName = NewCollectionNameTextBox.Text;
                if (collectionNewName.Trim().Length == 0)
                {
                    OkBtn.IsEnabled = false;
                }
                else if (collectionNewName == oldCollectionName)
                {
                    OkBtn.IsEnabled = false;
                }
                else
                {
                    OkBtn.IsEnabled = true;
                }
            }
        }
        private void SaveNewName()
        {
            collectionNewName = NewCollectionNameTextBox.Text;
            DialogResult = true;
        }
    }
}
