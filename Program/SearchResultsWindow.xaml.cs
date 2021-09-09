using System.Collections.Generic;
using System.Windows;

namespace Program
{
    /// <summary>
    /// Логика взаимодействия для SearchResultsWindow.xaml
    /// </summary>
    public partial class SearchResultsWindow : Window
    {
        private List<DataUnit> DataUnits { get; }
        public SearchResultsWindow(List<DataUnit> dataUnits, string collectionName = "")
        {
            DataUnits = dataUnits;

            InitializeComponent();
            Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;

            if(!string.IsNullOrEmpty(collectionName))
            {
                SearchResultsTexBox.Text += $" (Collection: {collectionName})";
            }

            DataUnitsList.ItemsSource = DataUnits;
        }
    }
}
