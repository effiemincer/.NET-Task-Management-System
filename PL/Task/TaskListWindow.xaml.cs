using Engineer; // Importing the Engineer namespace for access to its functionalities.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
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
using static BO.Enums; // Importing the BO.Enums namespace for access to its enumerated types.

namespace Task; // Declaring the namespace for the Task class.

/// <summary>
/// Interaction logic for TaskListWindow.xaml
/// </summary>
public partial class TaskListWindow : Window
{
    private bool isAdmin; // Flag to indicate whether the user is an admin or not.
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get(); // Static reference to the business logic layer.

    public IEnumerable<BO.TaskInList> TaskList
    {
        get { return (IEnumerable<BO.TaskInList>)GetValue(TaskListProperty); }
        set { SetValue(TaskListProperty, value); }
    }

    // Dependency property for TaskList.
    public static readonly DependencyProperty TaskListProperty =
        DependencyProperty.Register("TaskList",
            typeof(IEnumerable<BO.TaskInList>),
            typeof(TaskListWindow),
            new PropertyMetadata(null)
        );

    // Property to get or set the task's status.
    public BO.Enums.Status TaskStatus { get; set; } = BO.Enums.Status.None;
    // Property to get or set the task's status.
    public BO.Enums.EngineerExperience TaskDifficulty { get; set; } = BO.Enums.EngineerExperience.None;

    /// <summary>
    /// Constructor for the TaskListWindow class.
    /// </summary>
    /// <param name="isAdmin"></param>
    public TaskListWindow(bool isAdmin)
    {
        TaskList = s_bl?.Task.ReadAll(); // Reads all tasks from the data source.
        InitializeComponent();
        this.isAdmin = isAdmin; // Sets the isAdmin flag.
        bool? isScheduleGenerated = s_bl.Config.GetIsScheduleGenerated();

        if (isScheduleGenerated is null || !(bool)isScheduleGenerated!)
        {
            addButton.Visibility = Visibility.Visible;
        }
        else
        {
            addButton.Visibility = Visibility.Collapsed;
        }
    }

    
    /// <summary>
    /// Event handler for the AddTask button click event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void FilterSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var selection = cmbFilterCategory1.SelectedValue;
        if (selection is not null)
        {
            string test = selection.ToString()!;
            if (test == "Status")
            {
                TaskList = (TaskStatus == BO.Enums.Status.None) ? s_bl?.Task.ReadAll()! : s_bl?.Task.ReadAll(task => task.Status == TaskStatus)!;
                cmbFilterCategory2.Visibility = Visibility.Visible;
                cmbFilterCategory3.Visibility = Visibility.Collapsed;
            }
            else
            {
                TaskList = (TaskDifficulty == BO.Enums.EngineerExperience.None) ? s_bl?.Task.ReadAll()! : s_bl?.Task.ReadAll(task => task.Complexity == TaskDifficulty)!;
                cmbFilterCategory3.Visibility = Visibility.Visible;
                cmbFilterCategory2.Visibility = Visibility.Collapsed;
            }
        }
    }


   /// <summary>
   /// Event handler for the AddTask button click event.
   /// </summary>
   /// <param name="sender"></param>
   /// <param name="e"></param>
    private void cbTaskStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // Updates the TaskList based on selected status.
        var selection = cmbFilterCategory1.SelectedValue;
        if (selection is not null)
        {
            string test = selection.ToString()!;
            if (test == "Status")
            {
                TaskList = (TaskStatus == BO.Enums.Status.None) ? s_bl?.Task.ReadAll()! : s_bl?.Task.ReadAll(task => task.Status == TaskStatus)!;
            }
            else
            {
                TaskList = (TaskDifficulty == BO.Enums.EngineerExperience.None) ? s_bl?.Task.ReadAll()! : s_bl?.Task.ReadAll(task => task.Complexity == TaskDifficulty)!;
            }
        }

        
    }

    /// <summary>
    /// Event handler for the AddTask button click event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void doubleClickEvent_UpdateTask(object sender, MouseButtonEventArgs e)
    {
        // Gets the selected task and opens a new TaskWindow to update its details.
        BO.TaskInList? taskInList = (sender as ListView)?.SelectedItem as BO.TaskInList;
        if (taskInList is null)
            return;
        new TaskWindow(s_bl.Task.Read(taskInList.Id)!, false).ShowDialog();
        TaskList = null;
        TaskList = s_bl?.Task.ReadAll()!;
    }

    /// <summary>
    /// Event handler for the AddTask button click event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnAddTask_Click(object sender, RoutedEventArgs e)
    {
        // Closes the current window and opens a new TaskWindow to add a new task.
        //Close();
        new TaskWindow(new BO.Task(), true).ShowDialog();
        TaskList = null;
        TaskList = s_bl?.Task.ReadAll();
    }
}
