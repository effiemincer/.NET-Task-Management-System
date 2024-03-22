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
/// Interaction logic for GanttChartWindow.xaml
/// </summary>
public partial class GanttChartWindow : Window
{
    private class ganttTask
    {
        public ganttTask(BO.Task _t, bool m) 
        { 
            t = _t;
            isMilestone = m;
        }
        public BO.Task t { get; set; }
        public bool isMilestone { get; set; }
    }

    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
    private IEnumerable<BO.TaskInList> taskList = s_bl.Task.ReadAll();
    private IEnumerable<BO.Milestone> milestoneList = s_bl.Milestone.ReadAllMilestone();

    private Dictionary<int, ganttTask> taskArr = new Dictionary<int, ganttTask>();
    private int[] rowcheck;
    private Dictionary<int, bool> isRed = new Dictionary<int, bool>();

    private BO.Enums.Mode mode = BO.Enums.Mode.day;

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
        int numOfTasks = taskList.Count() + milestoneList.Count();
        int numOfDays = (endDate - startDate).Days;

        int numOfCols = calsNumOfCols((endDate - startDate).Days);

        rowcheck = new int[numOfTasks + 1];

        Dictionary<int, BO.Task> taskDict = new Dictionary<int, BO.Task>();

        foreach(BO.TaskInList taskIn in taskList)
        {
            BO.Task currTask = s_bl.Task.Read(taskIn.Id);
            taskDict[taskIn.Id] = currTask;
            if (!isRed.ContainsKey(taskIn.Id)) 
            {
                calcRed(currTask);
            }
        }


        var sortedMilestones = milestoneList.OrderBy(milestone => milestone.Deadline);


        int j = 0;
        foreach (BO.Milestone milestone in sortedMilestones)
        {
            foreach(int id in s_bl.Milestone.getMilestoneDef(milestone.Id))
            {
                if (taskDict.ContainsKey(id))
                {
                    taskArr[j] = new ganttTask(taskDict[id], false);
                    rowcheck[j] = 0;
                    ++j;
                    taskDict.Remove(id);
                }
            }

            taskArr[j] = new ganttTask(s_bl.Task.Read(milestone.Id), true);
            rowcheck[j] = 0;
            ++j;
        }

        AddDynamicRowsAndColumns(numOfTasks + 1, numOfCols + 1);

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
            if (rowcheck[i] == 0)
            {
                for (int j = 0; j < columnCount; j++)
                {
                    if (rowcheck[i] == 0)
                    {
                        if (i != 0 || j != 0)
                        {
                            dynamicGrid.Children.Add(fillCell(i, j));
                        }

                    }

                }
            }
        }
    }

    private UIElement fillCell(int row, int col)
    {
        DateTime d = new DateTime();
        if (mode == BO.Enums.Mode.day)
            d = startDate.AddDays(col - 1);
        else if (mode == BO.Enums.Mode.week)
            d = addWeeks(startDate, col - 1);
        else
            d = startDate.AddMonths(col - 1);

        //column headers
        if (row == 0)
        {
            return fillColHead(row, col, d);
            
        }

        // row headers
        else if (col == 0)
        {
            return fillRowStart(row, col);
           
        }

        // gantt rectangles 
        else
        {
            return fillGanttRect(row, col, d);
        }
        
    }

    private UIElement fillColHead(int row, int col, DateTime d)
    {
        Border border = new Border();
        border.BorderBrush = Brushes.Black;
        border.BorderThickness = new Thickness(1);
        border.Width = 100;

        TextBlock colText = new TextBlock();

        if (mode == BO.Enums.Mode.day)
            colText.Text = $"{dayString(d)}";

        if (mode == BO.Enums.Mode.week)
        {
            colText.Text = $"{weekString(d)}";
            border.Width = 200;
        }   

        if (mode == BO.Enums.Mode.month)
            colText.Text = $"{d.ToString("MMMM yyyy")}";

        colText.TextAlignment = TextAlignment.Center;
        border.Child = colText;

        // Set cell position
        Grid.SetRow(border, row);
        Grid.SetColumn(border, col);

        return border;
    }

    private UIElement fillRowStart(int row, int col)
    {
        BO.Task task = taskArr[row - 1].t;
        Border border = new Border();
        border.BorderBrush = Brushes.Black;
        border.BorderThickness = new Thickness(1);
        
        // Add content to the cell
        TextBlock rowText = new TextBlock();
        rowText.Text = $"TaskID: {task.Id.ToString()}" +
            $"\n" +
            $"Task Name: {task.Alias}";
        rowText.TextAlignment = TextAlignment.Center;
        rowText.Padding = new Thickness(20, 5, 20, 5);
        if (taskArr[row -1].isMilestone)
        {
            rowText.MouseDown += (sender, e) => RowText_MouseDown_Milestone(sender, e, task.Id);
        }
        else
        {
            rowText.MouseDown += (sender, e) => RowText_MouseDown_Task(sender, e, task.Id);
        }
        border.Child = rowText;
         
        // Set cell position
        Grid.SetRow(border, row);
        Grid.SetColumn(border, col);

        return border;
    }

    private void RowText_MouseDown_Milestone(object sender, MouseButtonEventArgs e, int id)
    {
        new MilestoneSingleWindow(s_bl.Milestone.Read(id), false).ShowDialog();
    }

    private void RowText_MouseDown_Task(object sender, MouseButtonEventArgs e, int id)
    {
        new TaskWindow(s_bl.Task.Read(id)!, false).ShowDialog();
    }

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

}
