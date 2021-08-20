using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Program.FileSystem.Exceptions;
using Program.userInterface;

namespace Program
{
    /// <summary>
    /// Логика взаимодействия для CollectionWindow.xaml
    /// </summary>
    /// 
    public partial class CollectionWindow : Window
    {
        private UserInterface UserInterface { get; }
        private CollectionDefinition CollectionDefinition { get; }
        private List<DataUnit> DataUnits { get; }

        private DataUnit DataUnitBuffer { get; set; }
        public CollectionWindow(CollectionDefinition collectionDefinition, List<DataUnit> dataUnits, UserInterface userInterface)
        {
            CollectionDefinition = collectionDefinition;
            UserInterface = userInterface;
            DataUnits = dataUnits;

            InitializeComponent();

            CollectionNameTexBox.Text = CollectionDefinition.Name;
            DataUnitsList.ItemsSource = DataUnits;

            DeleteRecordBtn.IsEnabled = false;
        }


        private void CreateRecord(object sender, RoutedEventArgs e)
        {
            try
            {
                var addedDataUnit = UserInterface.AddDataUnit(CollectionDefinition.Id);
                DataUnits.Add(addedDataUnit);
                RefreshDataUnitsList();
            }
            catch (Exception exception)
            {
                string logFilepath = ExceptionLogger.LogException(exception);
                MainWindow.ShowErrorMessage("Error", "Error while adding new record!", logFilepath);
            }
            
        }

        private void DeleteRecord(object sender, RoutedEventArgs e)
        {
            if (DataUnitsList.SelectedItem != null)
            {
                var dataUnitToDelete = (DataUnit)DataUnitsList.SelectedItem;
                DataUnits.Remove(dataUnitToDelete);
                try
                {
                    UserInterface.DeleteDataUnit(CollectionDefinition.Id, dataUnitToDelete.Id);
                } 
                catch(Exception exception)
                {
                    string logFilepath = ExceptionLogger.LogException(exception);
                    MainWindow.ShowErrorMessage("Error", "Error while deleting record!", logFilepath);
                    DataUnits.Add(dataUnitToDelete);
                }
                finally
                {
                    RefreshDataUnitsList();
                }
            }

        }

        private void AddField(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;
            var dataGrid = (Grid)btn.Parent;
            DataUnitsList.SelectedItem = (DataUnit)dataGrid.DataContext;
            var dataUnitToUpdate = (DataUnit)DataUnitsList.SelectedItem;
            var newProp = new StringDataUnitProp("NewField", "NewValue");
            dataUnitToUpdate.AddProperty(newProp);
            try
            {
                UserInterface.UpdateDataUnit(CollectionDefinition.Id, dataUnitToUpdate);
            }
            catch(Exception exception)
            {
                string logFilepath = ExceptionLogger.LogException(exception);
                MainWindow.ShowErrorMessage("Error", "Error while adding new field!", logFilepath);
                dataUnitToUpdate.RemoveProperty(newProp.Name);
            }
            finally
            {
                RefreshDataUnitsList();
            }
        }

        private void DeleteField(object sender, RoutedEventArgs e)
        {
            if (DataUnitsList.SelectedItem != null)
            {
                var dataUnitToUpdate = (DataUnit)DataUnitsList.SelectedItem;
                var dataPropToDelete = (DataUnitProp)(sender as Button).DataContext;
                dataUnitToUpdate.RemoveProperty(dataPropToDelete.Name);
                try
                {
                    UserInterface.UpdateDataUnit(CollectionDefinition.Id, dataUnitToUpdate);
                }
                catch(Exception exception)
                {
                    string logFilepath = ExceptionLogger.LogException(exception);
                    MainWindow.ShowErrorMessage("Error", "Error while deleting field!", logFilepath);
                    dataUnitToUpdate.AddProperty(dataPropToDelete);
                } 
                finally
                {
                    RefreshDataUnitsList();
                }
            }
        }
        private void DataGridCellLostFocus(object sender, RoutedEventArgs e)
        {
            var dataGridCell = sender as DataGridCell;
            var editedProp = (DataUnitProp)dataGridCell.DataContext;
            var editedPropIdValid = !string.IsNullOrEmpty(editedProp.Name.Trim()) && editedProp.Value != null;
            if (editedPropIdValid)
            {
                var parent = VisualTreeHelper.GetParent(dataGridCell);
                while (parent != null && parent.GetType() != typeof(DataGrid))
                {
                    parent = VisualTreeHelper.GetParent(parent);
                }

                var dataGrid = (DataGrid) parent;
                var dataUnit = (DataUnit) dataGrid.DataContext;
                if (DataUnitBuffer != null && !dataUnit.DeepEquals(DataUnitBuffer))
                {
                    try
                    {
                        UserInterface.UpdateDataUnit(CollectionDefinition.Id, dataUnit);
                        DataUnitBuffer = (DataUnit)SerializeUtils.DeepClone(dataUnit);
                    }
                    catch(Exception exception)
                    {
                        string logFilepath = ExceptionLogger.LogException(exception);
                        MainWindow.ShowErrorMessage("Error", "Error while saving record!", logFilepath);
                        DataUnits.Remove(dataUnit);
                        DataUnits.Add(DataUnitBuffer);
                    }
                    finally
                    {
                        RefreshDataUnitsList();
                    }
                }
            }
        }
        private void UpdateBthStates(object sender, RoutedEventArgs e)
        {
            UpdataBtnStates();
        }
        private void UpdataBtnStates()
        {
            if (DataUnitsList.SelectedItem == null)
            {
                DeleteRecordBtn.IsEnabled = false;
            }
            else
            {
                DeleteRecordBtn.IsEnabled = true;
            }
        }
        private void RefreshDataUnitsList()
        {
            DataUnitsList.ItemsSource = null;
            DataUnitsList.ItemsSource = DataUnits;
        }
        private void DataGridGotFocus(object sender, RoutedEventArgs e)
        {
            var dataGrid = sender as DataGrid;
            if (dataGrid.DataContext.GetType() == typeof(DataUnit))
            {
                var dataUnit = (DataUnit)dataGrid.DataContext;
                DataUnitsList.SelectedItem = dataUnit;
                DataUnitBuffer = (DataUnit)SerializeUtils.DeepClone(dataUnit);
                UpdataBtnStates();
            }
        }

        private void SearchByCollectionBtn_Click(object sender, RoutedEventArgs e)
        {
            SearchRequestWindow searchWindow = new SearchRequestWindow(UserInterface);
            var dialogResult = searchWindow.ShowDialog().Value;
            var dataUnits = new List<DataUnit>();
            var searchProps = searchWindow.ReductedDataUnitProps;
            if (dialogResult)
            {
                try
                {
                    dataUnits = UserInterface.SearchDataUnits(CollectionDefinition.Id, searchProps);
                }
                catch(Exception exception)
                {
                    string logFilepath = ExceptionLogger.LogException(exception);
                    MainWindow.ShowErrorMessage("Error!", "Error while search data!", logFilepath);
                }
                var searchResultsWindow = new SearchResultsWindow(dataUnits);
                searchResultsWindow.Show();
            }
        }
    }
}
