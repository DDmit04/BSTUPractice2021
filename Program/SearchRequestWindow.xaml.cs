using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using Program.userInterface;

namespace Program
{
    /// <summary>
    /// Логика взаимодействия для FieldUpdateWindow.xaml
    /// </summary>
    public partial class SearchRequestWindow : Window
    {
        private UserInterface UserInterface { get; }
        public List<DataUnitProp> ReductedDataUnitProps { get; protected set; }

        public SearchRequestWindow(UserInterface userInterface)
        {
            UserInterface = userInterface;

            InitializeComponent();

            ReductedDataUnitProps = new List<DataUnitProp>();
            ReductedDataUnitProps.Add(new StringDataUnitProp("Search name", "Search value"));
            ReductedDataUnitProps.Add(new StringDataUnitProp("Search name", "Search value"));
            ReductingDataUnitGrid.ItemsSource = ReductedDataUnitProps;
        }
        private void DeleteField(object sender, RoutedEventArgs e)
        {
            if (ReductedDataUnitProps.Count > 1)
            {
                var dataPropToDelete = (DataUnitProp)(sender as DataGridCell).DataContext;
                ReductedDataUnitProps.Remove(dataPropToDelete);
                RefreshPropsGrid();
            }
        }
        private void DataGridCellLostFocus(object sender, RoutedEventArgs e)
        {
            var dataGridCell = sender as DataGridCell;
            var editedProp = dataGridCell.DataContext as DataUnitProp;
            if (editedProp != null)
            {
                var editedPropIdValid = PropIsValid(editedProp);
                if (!editedPropIdValid)
                {
                    dataGridCell.BorderThickness = new Thickness(2, 2, 2, 2);
                    dataGridCell.BorderBrush = Brushes.Red;
                }
            }
        }
        private void DataCellGotFocus(object sender, RoutedEventArgs e)
        {
            var dataGridCell = sender as DataGridCell;
            var parent = VisualTreeHelper.GetParent(dataGridCell);
            while (parent != null && parent.GetType() != typeof(DataGrid))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }
            var dataGrid = (DataGrid)parent;
            var selectedRowIndex = dataGrid.SelectedIndex;
            if (selectedRowIndex == ReductedDataUnitProps.Count - 1)
            {
                ReductedDataUnitProps.Add(new StringDataUnitProp("Search name", "Search value"));
                RefreshPropsGrid();
            }
        }
        private void SearchBtn_Click(object sender, RoutedEventArgs e)
        {
            var resProps = ReductedDataUnitProps.Where(prop => PropIsValid(prop));
            if(resProps.Count() < ReductedDataUnitProps.Count())
            {
                var res = System.Windows.Forms.MessageBox.Show("Some of the props are invalid. Start search witout them?", "Warning!" ,MessageBoxButtons.YesNo);
                if(res == System.Windows.Forms.DialogResult.Yes)
                {
                    ReductedDataUnitProps = new List<DataUnitProp>(resProps);
                    DialogResult = true;
                    Close();
                }
            } 
            else
            {
                DialogResult = true;
                Close();
            }
        }
        private bool PropIsValid(DataUnitProp prop)
        {
            return !string.IsNullOrEmpty(prop.Name.Trim()) && prop.Value != null;
        }
        private void RefreshPropsGrid()
        {
            ReductingDataUnitGrid.ItemsSource = null;
            ReductingDataUnitGrid.ItemsSource = ReductedDataUnitProps;
        }

    }
}