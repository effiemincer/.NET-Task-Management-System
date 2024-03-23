using BO;
using DO;
using Milestone;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.Pkcs;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using Task;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PL.Admin;

/// <summary>
/// Defines the interaction logic for the GanttChartWindow.
/// </summary>
public partial class GanttChartWindow : Window
{
    // Nested class representing a Gantt chart task
    private class ganttTask
    {
        public ganttTask(BO.Task _t, bool m)
        {
            t = _t; // The task itself
            isMilestone = m; // Flag indicating if the task is a milestone
        }
        public BO.Task t { get; set; } // Task object
        public bool isMilestone { get; set; } // Milestone flag
    }

    // Static reference to the business layer interface
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

    // Collections for tasks and milestones
    private IEnumerable<BO.TaskInList> taskList = s_bl.Task.ReadAll();
    private IEnumerable<BO.Milestone> milestoneList = s_bl.Milestone.ReadAllMilestone();

    // Data structures for managing tasks and their properties
    private Dictionary<int, ganttTask> taskArr = new Dictionary<int, ganttTask>();
    private int[] rowcheck; // Array for tracking the filling of rows
    private Dictionary<int, bool> isRed = new Dictionary<int, bool>(); // Tracks if a task is overdue (red)

    // Default mode of the Gantt chart (daily)
    private BO.Enums.Mode mode = BO.Enums.Mode.day;

    // Start and end dates of the project
    private DateTime startDate;
    private DateTime endDate;

    // Constructor for GanttChartWindow
    public GanttChartWindow()
    {
        InitializeComponent(); // Initialize component from XAML

        // Fetch project start and end dates from the configuration
        startDate = (DateTime)s_bl.Config.GetProjectStartDate();
        endDate = (DateTime)s_bl.Config.GetProjectEndDate();

        CreateGanttChart(); // Method call to create the Gantt chart
    }

    /// <summary>
    /// Creates the Gantt chart by initializing and populating UI elements.
    /// </summary>
    private void CreateGanttChart()
    {
        // Calculate the number of tasks and days for sizing the chart
        int numOfTasks = taskList.Count() + milestoneList.Count();
        int numOfDays = (endDate - startDate).Days;

        // Calculate the number of columns based on the date range
        int numOfCols = calsNumOfCols(numOfDays);

        rowcheck = new int[numOfTasks + 1]; // Initialize the row check array

        // Dictionary to hold tasks for easier access by ID
        Dictionary<int, BO.Task> taskDict = new Dictionary<int, BO.Task>();

        // Process each task in the task list
        foreach (BO.TaskInList taskIn in taskList)
        {
            BO.Task currTask = s_bl.Task.Read(taskIn.Id);
            taskDict[taskIn.Id] = currTask;
            if (!isRed.ContainsKey(taskIn.Id))
            {
                calcRed(currTask); // Determine if the task should be marked red
            }
        }

        // Sort milestones by their deadlines
        var sortedMilestones = milestoneList.OrderBy(milestone => milestone.Deadline);

        // Populate taskArr with tasks and milestones
        int j = 0;
        foreach (BO.Milestone milestone in sortedMilestones)
        {
            foreach (int id in s_bl.Milestone.getMilestoneDef(milestone.Id))
            {
                if (taskDict.ContainsKey(id))
                {
                    // Add task to the taskArr dictionary and set it as not a milestone
                    taskArr[j] = new ganttTask(taskDict[id], false);
                    rowcheck[j] = 0; // Initialize row check for this row
                    ++j; // Increment the task index
                    taskDict.Remove(id); // Remove the task from taskDict as it's now accounted for
                }
            }

            // Add the milestone to the taskArr dictionary, setting it as a milestone
            taskArr[j] = new ganttTask(s_bl.Task.Read(milestone.Id), true);
            rowcheck[j] = 0; // Initialize row check for this row
            ++j; // Increment the task index
        }

        // Add dynamically generated rows and columns to the grid based on task and date counts
        AddDynamicRowsAndColumns(numOfTasks + 1, numOfCols + 1);
    }

    /// <summary>
    /// Dynamically adds rows and columns to the grid, populating it with UI elements.
    /// </summary>
    /// <param name="rowCount">The number of rows to add.</param>
    /// <param name="columnCount">The number of columns to add.</param>
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

        // Populate cells with content
        for (int i = 0; i < rowCount; i++)
        {
            if (rowcheck[i] == 0) // Check if the row needs to be filled
            {
                for (int j = 0; j < columnCount; j++)
                {
                    if (rowcheck[i] == 0) // Check if the row needs to be filled
                    {
                        if (i != 0 || j != 0)
                        {
                            // Add UIElement to the grid cell
                            dynamicGrid.Children.Add(fillCell(i, j));
                        }

                    }

                }
            }
        }
    }

    /// <summary>
    /// Fills a specific cell with appropriate UI elements based on its position.
    /// </summary>
    /// <param name="row">Row index of the cell.</param>
    /// <param name="col">Column index of the cell.</param>
    /// <returns>A UIElement that is either a header or a Gantt chart bar.</returns>
    private UIElement fillCell(int row, int col)
    {
        // Initialize a new DateTime object to hold the date for the cell
        DateTime d = new DateTime();
        // Determine the date for the cell based on the mode (day, week, month) and column index
        if (mode == BO.Enums.Mode.day)
            d = startDate.AddDays(col - 1); // Add days for day mode
        else if (mode == BO.Enums.Mode.week)
            d = addWeeks(startDate, col - 1); // Add weeks for week mode
        else
            d = startDate.AddMonths(col - 1); // Add months for month mode

        // If the cell is a column header (first row)
        if (row == 0)
        {
            return fillColHead(row, col, d);
        }
        // If the cell is a row header (first column)
        else if (col == 0)
        {
            return fillRowStart(row, col);
        }
        // If the cell is part of the Gantt chart (task bars)
        else
        {
            return fillGanttRect(row, col, d);
        }
    }

    /// <summary>
    /// Creates and returns a UIElement for column headers based on the provided date.
    /// </summary>
    /// <param name="row">The row index (should always be 0 for column headers).</param>
    /// <param name="col">The column index.</param>
    /// <param name="d">The date that the column represents.</param>
    /// <returns>A UIElement configured as a column header.</returns>
    private UIElement fillColHead(int row, int col, DateTime d)
    {
        Border border = new Border
        {
            BorderBrush = Brushes.Black,
            BorderThickness = new Thickness(1),
            Width = 100 // Default width for day mode
        };

        TextBlock colText = new TextBlock
        {
            TextAlignment = TextAlignment.Center
        };

        // Display different headers based on the current mode of the Gantt chart
        switch (mode)
        {
            case BO.Enums.Mode.day:
                colText.Text = dayString(d); // Format date for day mode
                break;
            case BO.Enums.Mode.week:
                colText.Text = weekString(d); // Format date range for week mode
                border.Width = 200; // Adjust width for week mode
                break;
            case BO.Enums.Mode.month:
                colText.Text = d.ToString("MMMM yyyy"); // Format date for month mode
                break;
        }

        border.Child = colText;

        // Position the header in the grid
        Grid.SetRow(border, row);
        Grid.SetColumn(border, col);

        return border;
    }

    /// <summary>
    /// Creates and returns a UIElement for row headers, displaying task details.
    /// </summary>
    /// <param name="row">The row index.</param>
    /// <param name="col">The column index (should always be 0 for row headers).</param>
    /// <returns>A UIElement configured as a row header.</returns>
    private UIElement fillRowStart(int row, int col)
    {
        ganttTask gTask = taskArr[row - 1]; // Retrieve the task associated with this row
        BO.Task task = gTask.t;
        Border border = new Border
        {
            BorderBrush = Brushes.Black,
            BorderThickness = new Thickness(1)
        };

        TextBlock rowText = new TextBlock
        {
            Text = $"TaskID: {task.Id}\nTask Name: {task.Alias}",
            TextAlignment = TextAlignment.Center,
            Padding = new Thickness(20, 5, 20, 5)
        };

        // Register click event handlers differently if the task is a milestone
        if (gTask.isMilestone)
        {
            rowText.MouseDown += (sender, e) => RowText_MouseDown_Milestone(sender, e, task.Id);
        }
        else
        {
            rowText.MouseDown += (sender, e) => RowText_MouseDown_Task(sender, e, task.Id);
        }

        border.Child = rowText;

        // Position the header in the grid
        Grid.SetRow(border, row);
        Grid.SetColumn(border, col);

        return border;
    }

    /// <summary>
    /// Creates and returns a UIElement (Rectangle) representing a task or milestone on the Gantt chart.
    /// </summary>
    /// <param name="row">The row index of the task or milestone.</param>
    /// <param name="col">The column index representing the day, week, or month.</param>
    /// <param name="d">The date represented by the column.</param>
    /// <returns>A UIElement configured as a Gantt chart bar.</returns>
    private UIElement fillGanttRect(int row, int col, DateTime d)
    {
        BO.Task task = taskArr[row - 1].t;

        Rectangle rect = new Rectangle();
        rect.Width = 100;

        if (s_bl.Milestone.isStart(task.Id) || s_bl.Milestone.isEnd(task.Id))
        {
            rect.Fill = Brushes.LightBlue;
            rect.Stroke = Brushes.Black;
            rect.StrokeThickness = 1;

            TextBlock textBlock = new TextBlock();
            textBlock.Text = s_bl.Milestone.isStart(task.Id) ? "START" : "END";
            textBlock.HorizontalAlignment = HorizontalAlignment.Center;
            textBlock.VerticalAlignment = VerticalAlignment.Center;

            double textLeft = (rect.Width - textBlock.ActualWidth) / 2; // Center horizontally
            double textTop = (rect.Height - textBlock.ActualHeight) / 2; // Center vertically

            // Position the TextBlock within the Canvas
            Canvas.SetLeft(textBlock, textLeft);
            Canvas.SetTop(textBlock, textTop);

            // Create a Canvas to contain both the Rectangle and the TextBlock
            Canvas canvas = new Canvas();
            canvas.Children.Add(rect);
            canvas.Children.Add(textBlock);

            BindingOperations.SetBinding(rect, Rectangle.WidthProperty, new Binding("ActualWidth") { Source = canvas });
            BindingOperations.SetBinding(rect, Rectangle.HeightProperty, new Binding("ActualHeight") { Source = canvas });

            Grid.SetRow(canvas, row);
            Grid.SetColumn(canvas, col);

            return canvas;

        }

        if (mode == BO.Enums.Mode.day)
        {
            if (task.ProjectedStartDate <= d)
            {
                // Create a LinearGradientBrush with two gradient stops

                if (!taskArr[row - 1].isMilestone)
                {
                    if (isRed[task.Id])
                        rect.Fill = Brushes.Red;
                    else
                        rect.Fill = Brushes.Black;
                }
                    
                else
                {
                  rect.Fill = Brushes.Blue;  
                }
                    

                if (HaveSameDate((DateTime)task.Deadline, d)) rowcheck[row] = 1;
            }
            else
            {
                rect.Fill = Brushes.White;
            }
        }

        else
        {
            if (mode == BO.Enums.Mode.week) rect.Width = 200;
            rect.Fill = calcColor(task, row, col, d);
        }
        

        Grid.SetRow(rect, row);
        Grid.SetColumn(rect, col);

        return rect;
    }

    /// <summary>
    /// Creates and returns a color for a rectangle in the grid.
    /// </summary>
    /// <param name="task">The task to check its dates for the current row</param>
    /// <param name="row">The row index of the task or milestone.</param>
    /// <param name="col">The column index representing the day, week, or month.</param>
    /// <param name="d">The date represented by the column.</param>
    /// <returns>A UIElement configured as a Gantt chart bar.</returns>
    private LinearGradientBrush calcColor(BO.Task task, int row, int col, DateTime d)
    {
        LinearGradientBrush brush = new LinearGradientBrush();

        brush.StartPoint = new Point(0, 0);
        brush.EndPoint = new Point(1, 0);

        Color color;

        if (taskArr[row - 1].isMilestone)
            color = Colors.Blue;

        else
        {
            if (isRed[task.Id]) color = Colors.Red;
            else color = Colors.Black;
        }

        // 5 cases

        // Case 0: task start date and end date within week/month 
        if (isDateWithin((DateTime)task.ProjectedStartDate, d) && isDateWithin((DateTime)task.Deadline, d))
        {
            double percentStart, percentEnd;
            
            if (mode == BO.Enums.Mode.week)
            {
                percentStart = calcPercentWeek((DateTime)task.ProjectedStartDate);
                percentEnd = calcPercentWeek((DateTime)task.Deadline) + (1.0/ 7.0);

            }
            else
            {
                percentStart = calcPercentMonth((DateTime)task.ProjectedStartDate);
                percentEnd = calcPercentMonth((DateTime)(task.Deadline));
            }

            brush.GradientStops.Add(new GradientStop(Colors.White, 0));
            brush.GradientStops.Add(new GradientStop(Colors.White, percentStart));

            brush.GradientStops.Add(new GradientStop(color, percentStart));
            brush.GradientStops.Add(new GradientStop(color, percentEnd));

            brush.GradientStops.Add(new GradientStop(Colors.White, percentEnd));
            brush.GradientStops.Add(new GradientStop(Colors.White, 1));



        }

        // Case 1: task start date within week/month
        else if (isDateWithin((DateTime)task.ProjectedStartDate, d))
        {
            double percent;
            if (mode == BO.Enums.Mode.week)
            {
                percent = calcPercentWeek((DateTime)task.ProjectedStartDate);
            }
            else
            {
                percent = calcPercentMonth((DateTime)task.ProjectedStartDate);
            }

            brush.GradientStops.Add(new GradientStop(Colors.White, 0));
            brush.GradientStops.Add(new GradientStop(Colors.White, percent));

            brush.GradientStops.Add(new GradientStop(color, percent));
            brush.GradientStops.Add(new GradientStop(color, 1));
        }

        // Case 2: task end date witin week/month
        else if (isDateWithin((DateTime)task.Deadline, d))
        {
            double percent;
            if (mode == BO.Enums.Mode.week)
            {
                percent = calcPercentWeek((DateTime)task.Deadline) + (1.0/7.0);
            }
            else
            {
                percent = calcPercentMonth((DateTime)task.Deadline);
            }

            brush.GradientStops.Add(new GradientStop(color, 0));
            brush.GradientStops.Add(new GradientStop(color, percent));

            brush.GradientStops.Add(new GradientStop(Colors.White, percent));
            brush.GradientStops.Add(new GradientStop(Colors.White, 1));
        }

        // Case 3: task range not within week/month
        else if(!isRangeWithin(task, d))
        {
            brush.GradientStops.Add(new GradientStop(Colors.White, 0));
            brush.GradientStops.Add(new GradientStop(Colors.White, 1));

            if (d >= task.Deadline) rowcheck[row] = 1;
        }

        // Case 4: task start/end not within week/month but range is 
        else
        {
            brush.GradientStops.Add(new GradientStop(color, 0));
            brush.GradientStops.Add(new GradientStop(color, 1));
        }

        return brush;
    }

    private string dayString( DateTime d)
    {
        string formattedDate = d.ToString("ddd M/d/yy");
        return formattedDate;
    }

    private string weekString( DateTime d) // TODO implement
    {
        // Find the date of the Sunday of the week
        DateTime sundayDate = d.AddDays(-(int)d.DayOfWeek);

        // Calculate the date of the Saturday of the week (Sunday + 6 days)
        DateTime saturdayDate = sundayDate.AddDays(6);

        // Format the dates in the required format
        string sundayString = sundayDate.ToString("ddd MM/dd/yy");
        string saturdayString = saturdayDate.ToString("ddd MM/dd/yy");

        // Concatenate the formatted dates with a hyphen
        string weekString = $"{sundayString} - {saturdayString}";

        return weekString;
    }

    private static bool HaveSameDate(DateTime dateTime1, DateTime dateTime2)
    {
        return dateTime1.Year == dateTime2.Year &&
               dateTime1.Month == dateTime2.Month &&
               dateTime1.Day == dateTime2.Day;
    }

    private int calsNumOfCols(int numOfDays)
    {
        if (numOfDays > 60 && numOfDays < 180) // go into week mode
        {
            mode = BO.Enums.Mode.week;
            return getNumberOfWeeks(numOfDays);
        }
        else if (numOfDays > 180) // go into month mode
        {
            mode = BO.Enums.Mode.month;
            return getTotalMonths();
        }
        else 
            return numOfDays;
    }

    private int getTotalMonths()
    {
        int totalMonths = ((endDate.Year - startDate.Year) * 12) + endDate.Month - startDate.Month + 1;

        return totalMonths;
    }

    private int getNumberOfWeeks(int numOfDays)
    {
        int totalDays = 0;
        int numberOfWeeks = 0;

        while (totalDays < numOfDays)
        {
            int daysInCurrentWeek = 7 - (int)startDate.DayOfWeek; // Days remaining in the current week
            int daysToAdd = Math.Min(numOfDays - totalDays, daysInCurrentWeek); // Days to add in the current week
            totalDays += daysToAdd; // Add days to the total
            numberOfWeeks++; // Increment the number of weeks
        }

        return numberOfWeeks;
    }

    private DateTime addWeeks(DateTime d, int numOfWeeks)
    {
        TimeSpan duration = TimeSpan.FromDays(numOfWeeks * 7); // Convert weeks to days
        return  d + duration;
    }

    private bool isDateWithin(DateTime dateToCheck, DateTime rangeHolder)
    {
        if (mode == BO.Enums.Mode.week)
        {
            // Check if the dates fall within the same week
            return dateToCheck.Date >= rangeHolder.Date.AddDays(-(int)rangeHolder.DayOfWeek) &&
                   dateToCheck.Date <= rangeHolder.Date.AddDays(6 - (int)rangeHolder.DayOfWeek);
        }
        else if (mode == BO.Enums.Mode.month)
        {
            // Check if the dates fall within the same month
            return dateToCheck.Year == rangeHolder.Year && dateToCheck.Month == rangeHolder.Month;
        }
        else
        {
            return false;
        }
    }

    private bool isRangeWithin(BO.Task task, DateTime d)
    {
        return d >= task.ProjectedStartDate && d <= task.Deadline;
    }

    private double calcPercentWeek(DateTime dateToCheck)
    {
        return (double)(dateToCheck.DayOfWeek) / 7.0; 
    }
    private double calcPercentMonth(DateTime dateToCheck)
    {
        // Get the total number of days in the month
        int totalDaysInMonth = DateTime.DaysInMonth(dateToCheck.Year, dateToCheck.Month);

        // Calculate the equivalent fraction for the day of the month
        double percentMonth = (double)dateToCheck.Day / (double)totalDaysInMonth;

        return percentMonth;
     }

    private void calcRed(BO.Task task)
    {
        if (task.Status != BO.Enums.Status.Done && task.Deadline < s_bl.Clock)
        {
            setRed(task.Id);
        }
        else
        {
            isRed[task.Id] = false;
        }
    }

    private void setRed(int id)
    {
        isRed[id] = true;
        List<int> deps = s_bl.Task.findDependants(id);
        if (deps != null)
        {
            foreach (int depId in deps)
            {
                setRed(depId);
            }
        }
    }

    /// <summary>
    /// Event handler for clicking on a task row header.
    /// </summary>
    /// <param name="sender">The sender object (not used).</param>
    /// <param name="e">Event arguments (not used).</param>
    /// <param name="id">The ID of the task clicked.</param>
    private void RowText_MouseDown_Task(object sender, MouseButtonEventArgs e, int id)
    {
        // Open a window to display the task details
        new TaskWindow(s_bl.Task.Read(id)!, false).ShowDialog();
    }

    /// <summary>
    /// Event handler for clicking on a milestone row header.
    /// </summary>
    /// <param name="sender">The sender object (not used).</param>
    /// <param name="e">Event arguments (not used).</param>
    /// <param name="id">The ID of the milestone clicked.</param>
    private void RowText_MouseDown_Milestone(object sender, MouseButtonEventArgs e, int id)
    {
        // Open a window to display the milestone details
        new MilestoneSingleWindow(s_bl.Milestone.Read(id), false).ShowDialog();
    }

}
