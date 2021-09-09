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
        public string CollectionNewName { get; protected set; }
        public string OldCollectionName { get; }
        public CollectionRenameWindow(string collectionName)
        {
            InitializeComponent();
            Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
            OldCollectionName = collectionName;
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

        private void UserKeyDown(object sender, KeyEventArgs e)
        {
            if (OkBtn.IsEnabled && e.Key == Key.Enter)
            {
                SaveNewName();
            }
            else if(e.Key == Key.Escape)
            {
                DialogResult = false;
                Close();
            }
        }

        private void NewCollectionNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (IsLoaded)
            {
                CollectionNewName = NewCollectionNameTextBox.Text;
                if (CollectionNewName.Trim().Length == 0)
                {
                    OkBtn.IsEnabled = false;
                }
                else if (CollectionNewName == OldCollectionName)
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
            CollectionNewName = NewCollectionNameTextBox.Text;
            DialogResult = true;
        }
    }
}
