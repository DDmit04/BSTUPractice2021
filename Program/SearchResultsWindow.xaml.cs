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
        public SearchResultsWindow(List<DataUnit> dataUnits)
        {
            DataUnits = dataUnits;

            InitializeComponent();

            DataUnitsList.ItemsSource = DataUnits;
        }
    }
}
