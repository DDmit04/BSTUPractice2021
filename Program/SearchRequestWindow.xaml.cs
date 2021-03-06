using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using Application = System.Windows.Application;

namespace Program
{
    /// <summary>
    /// Логика взаимодействия для FieldUpdateWindow.xaml
    /// </summary>
    public partial class SearchRequestWindow : Window
    {
        public List<DataUnitProp> ReductedDataUnitProps { get; protected set; }

        public SearchRequestWindow()
        {

            InitializeComponent();
            Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
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
                var editedPropIdValid = editedProp.IsValid;
                if (!editedPropIdValid)
                {
                    dataGridCell.BorderThickness = new Thickness(2, 2, 2, 2);
                    dataGridCell.BorderBrush = Brushes.Red;
                }
            }
        }
        private void DataCellGotFocus(object sender, RoutedEventArgs e)
        {
            DataGridCellInfo cell = ReductingDataUnitGrid.CurrentCell;
            int rowIndex = ReductingDataUnitGrid.Items.IndexOf(cell.Item);
            if (rowIndex == ReductedDataUnitProps.Count - 1)
            {
                ReductedDataUnitProps.Add(new StringDataUnitProp("Search name", "Search value"));
                RefreshPropsGrid();
            }
        }
        private void SearchBtn_Click(object sender, RoutedEventArgs e)
        {
            var resProps = ReductedDataUnitProps.Where(prop => prop.IsValid);
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
        private void RefreshPropsGrid()
        {
            ReductingDataUnitGrid.ItemsSource = null;
            ReductingDataUnitGrid.ItemsSource = ReductedDataUnitProps;
        }

    }
}