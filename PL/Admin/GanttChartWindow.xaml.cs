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
    private IEnumerable<BO.TaskInList> taskList = s_bl.Task.ReadAll();

    private DateTime startDate;
    private DateTime endDate;
    public GanttChartWindow()
    {
        InitializeComponent();

        startDate = (DateTime)s_bl.Config.GetProjectStartDate();
        endDate = (DateTime)s_bl.Config.GetProjectEndDate();
        CreateGanttChart();
    }

    private void CreateGanttChart()
    {      
        int numOfTasks = taskList.Count();
        int numOfDays = (endDate - startDate).Days;

        AddDynamicRowsAndColumns(numOfTasks + 1, numOfDays + 1);

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
                // Exclude populating cell (0,0)
                if (i != 0 || j != 0)
                {
                    if (i == 0) // first row
                    {
                        Border border = new Border();
                        border.BorderBrush = Brushes.Black;
                        border.BorderThickness = new Thickness(1);

                        // Add content to the cell

                        DateTime d = startDate.AddDays(j - 1);

                        TextBlock textBlock = new TextBlock();
                        textBlock.Text = $"{dayString(d)}";
                        textBlock.TextAlignment = TextAlignment.Center;
                        border.Child = textBlock;

                        // Set cell position
                        Grid.SetRow(border, i);
                        Grid.SetColumn(border, j);

                        // Add the border to the grid
                        dynamicGrid.Children.Add(border);
                    }
                    else if (j == 0) // first column
                    {
                        Border border = new Border();
                        border.BorderBrush = Brushes.Black;
                        border.BorderThickness = new Thickness(1);

                        BO.TaskInList task = taskList.ElementAtOrDefault(i - 1);
                        // Add content to the cell
                        TextBlock textBlock = new TextBlock();
                        textBlock.Text = $"TaskID: {task.Id.ToString()}" +
                            $"\n" +
                            $"Task Name: {task.Alias}";
                        textBlock.TextAlignment = TextAlignment.Center;
                        textBlock.Padding = new Thickness(20, 5, 20, 5);
                        border.Child = textBlock;

                        // Set cell position
                        Grid.SetRow(border, i);
                        Grid.SetColumn(border, j);

                        // Add the border to the grid
                        dynamicGrid.Children.Add(border);
                    }
                    // Add content directly for other cells
                    else
                    {
                        TextBlock textBlock = new TextBlock();
                        textBlock.Text = $"Row {i}, Column {j}";
                        Grid.SetRow(textBlock, i);
                        Grid.SetColumn(textBlock, j);
                        dynamicGrid.Children.Add(textBlock);
                    }
                }
            }
        }
    }

    private void taskListOrdered()
    {
        //add functionality fr order of milestones and tasks later
        return;
    }

    private string dayString( DateTime d)
    {
        string formattedDate = d.ToString("ddd M/d/yy");
        return formattedDate;
    }

}
