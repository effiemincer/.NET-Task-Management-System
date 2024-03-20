using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PL.Admin;

/// <summary>
/// Interaction logic for GanttChartWindow.xaml
/// </summary>
public partial class GanttChartWindow : Window
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
    private DateTime startDate;
    public GanttChartWindow()
    {
        InitializeComponent();
        try
        {
            TimeSpan t = s_bl.Milestone.getProjectDuration();
            int numOfDays = t.Days;
            startDate = new DateTime(2024, 3, 20); // hardcoded for now we need to change this so it usese the actual values
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    
    }

    private void AddDynamicRowsAndColumns(int rowCount, int columnCount)
    {

        // Clear existing rows and columns
        dynamicGrid.RowDefinitions.Clear();
        dynamicGrid.ColumnDefinitions.Clear();

        // Add rows
        for (int i = 0; i < rowCount; i++)
        {
            dynamicGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        }

        // Add columns
        for (int i = 0; i < columnCount; i++)
        {
            dynamicGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        }

        // Populate cells with content (optional)
        for (int i = 0; i < rowCount; i++)
        {
            for (int j = 0; j < columnCount; j++)
            {
                // Example: Add text blocks to each cell
                TextBlock textBlock = new TextBlock();
                textBlock.Text = $"Row {i}, Column {j}";
                Grid.SetRow(textBlock, i);
                Grid.SetColumn(textBlock, j);
                dynamicGrid.Children.Add(textBlock);
            }
        }
    }

}
