using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Program.FileSystem.Exceptions;
using Program.userInterface;

namespace Program
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly UserInterface UserInterface;

        private List<CollectionDefinition> CollectionDefinitions { get; set; }

        public MainWindow()
        {
            UserInterface = new UserInterface(new MainRepo());
            RefreshCollectionList();

            InitializeComponent();

            CollectionsList.ItemsSource = CollectionDefinitions;
            CollectionsList.SelectionChanged += new SelectionChangedEventHandler(UpdateBthStates);
            UpdateBtnStates();
        }

        private void RefreshCollectionList()
        {
            try
            {
                CollectionDefinitions = UserInterface.GetCollectionDefinitions();
            }
            catch (Exception exception)
            {
                string logFilepath = ExceptionLogger.LogException(exception);
                ShowErrorMessage("Error!", "Error while loading collections info!", logFilepath);
            }
        }


        private void CreateCollectionButton_Click(object sender, RoutedEventArgs e)
        {
            var renameCollectionDialog = new CollectionRenameWindow("");
            if (renameCollectionDialog.ShowDialog().Value)
            {
                var newCollectionName = renameCollectionDialog.CollectionNewName;
                if (newCollectionName.Trim().Length != 0)
                {
                    try
                    {
                        var newCollection = UserInterface.CreateCollection(newCollectionName);
                        CollectionDefinitions.Add(newCollection);
                        RefreshListBoxData();
                    }
                    catch(Exception exception)
                    {
                        string logFilepath = ExceptionLogger.LogException(exception);
                        ShowErrorMessage("Error", "Error while creating new collection!", logFilepath);
                    }
                }
            }

        }

        private void OpenCollection(object sender, MouseButtonEventArgs e)
        {
            if (CollectionsList.SelectedItem != null)
            {
                var collectionToOpen = (CollectionDefinition)CollectionsList.SelectedItem;
                try
                {
                    var collectionData = UserInterface.GetCollectionData(collectionToOpen.Id);
                    CollectionWindow collectionWindow = new CollectionWindow(collectionToOpen, collectionData, UserInterface);
                    collectionWindow.Show();
                }
                catch(Exception exception)
                {
                    string logFilepath = ExceptionLogger.LogException(exception);
                    ShowErrorMessage("Error", "Error while loading collection data!", logFilepath);
                }
            }
        }

        private void DeleteCollection(object sender, RoutedEventArgs e)
        {
            if (CollectionsList.SelectedItem != null)
            {
                var deletingCollection = (CollectionDefinition)CollectionsList.SelectedItem;
                try
                {
                    var res = System.Windows.Forms.MessageBox.Show("Are you sure you want to delete the collection?", "Warning!", System.Windows.Forms.MessageBoxButtons.YesNo);
                    if (res == System.Windows.Forms.DialogResult.Yes)
                    {
                        UserInterface.DeleteCollection(CollectionDefinitions[CollectionsList.Items.IndexOf(CollectionsList.SelectedItem)].Id);
                        CollectionDefinitions.Remove(deletingCollection);
                        RefreshListBoxData();
                    }
                } 
                catch(Exception exception)
                {
                    string logFilepath = ExceptionLogger.LogException(exception);
                    ShowErrorMessage("Error", "Error while deleting collection!", logFilepath);
                    RefreshCollectionList();
                    RefreshListBoxData();
                }
            }
        }

        private void RenameCollection(object sender, RoutedEventArgs e)
        {
            if (CollectionsList.SelectedItem != null)
            {
                var renamingCollection = (CollectionDefinition)CollectionsList.SelectedItem;
                var renameCollectionDialog = new CollectionRenameWindow(renamingCollection.Name);
                if (renameCollectionDialog.ShowDialog().Value)
                {
                    var newCollectionName = renameCollectionDialog.CollectionNewName;
                    if (newCollectionName.Trim().Length != 0 && newCollectionName != renamingCollection.Name)
                    {
                        try
                        {
                            UserInterface.RenameCollection(renamingCollection.Id, newCollectionName);
                            var index = CollectionDefinitions.FindIndex(col => col.Id == renamingCollection.Id);
                            CollectionDefinitions.Remove(renamingCollection);
                            renamingCollection.Name = newCollectionName;
                            CollectionDefinitions.Insert(index, renamingCollection);
                            RefreshListBoxData();
                        } 
                        catch(Exception exception)
                        {
                            string logFilepath = ExceptionLogger.LogException(exception);
                            ShowErrorMessage("Error!", "Error while saving new collection data!", logFilepath);
                            RefreshCollectionList();
                            RefreshListBoxData();
                        }
                    }
                }
            }
        }

        private void SearchButtonClick(object sender, RoutedEventArgs e)
        {
            SearchRequestWindow searchWindow = new SearchRequestWindow();
            var dialogResult = searchWindow.ShowDialog().Value;
            var dataUnits = new List<DataUnit>();
            var searchProps = searchWindow.ReductedDataUnitProps;
            if (dialogResult)
            {
                try
                {
                    dataUnits = UserInterface.SearchDataUnitsAllCollections(searchProps);
                }
                catch(Exception exception)
                {
                    string logFilepath = ExceptionLogger.LogException(exception);
                    ShowErrorMessage("Error!", "Error while search data!", logFilepath);
                }
                var searchResultsWindow = new SearchResultsWindow(dataUnits);
                searchResultsWindow.Show();
            }
        }

        private void UpdateBthStates(object sender, RoutedEventArgs e)
        {
            UpdateBtnStates();
        }

        public static void ShowInfoMessage(string title, string body)
        {
            MessageBox.Show(body, title, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static void ShowErrorMessage(string title, string body, string logFilepath = "")
        {
            if (logFilepath != "")
            {
                body += $" Файл с логами ошибки - {logFilepath}";
            }
            MessageBox.Show(body, title, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static void ShowWarningMessage(string title, string body, string logFilepath = "")
        {
            if (logFilepath != "")
            {
                body += $" Файл с логами ошибки - {logFilepath}";
            }
            MessageBox.Show(body, title, MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        private void UpdateBtnStates()
        {
            if (CollectionsList.SelectedItem == null)
            {
                deleteCollectionBtn.IsEnabled = false;
                renameCollectionBtn.IsEnabled = false;
            }
            else
            {
                deleteCollectionBtn.IsEnabled = true;
                renameCollectionBtn.IsEnabled = true;
            }
        }
        private void RefreshListBoxData()
        {
            CollectionsList.ItemsSource = null;
            CollectionsList.ItemsSource = CollectionDefinitions;
        }
    }
}