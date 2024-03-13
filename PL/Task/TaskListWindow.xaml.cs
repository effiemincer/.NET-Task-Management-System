using Engineer; // Importing the Engineer namespace for access to its functionalities.
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
using static BO.Enums; // Importing the BO.Enums namespace for access to its enumerated types.

namespace Task; // Declaring the namespace for the Task class.

/// <summary>
/// Interaction logic for TaskListWindow.xaml
/// </summary>
public partial class TaskListWindow : Window
{
    private bool isAdmin; // Flag to indicate whether the user is an admin or not.
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get(); // Static reference to the business logic layer.

    // Property to get or set the task's status.
    public BO.Enums.Status TaskStatus { get; set; } = BO.Enums.Status.None;

    // Constructor for TaskListWindow class.
    public TaskListWindow(bool isAdmin)
    {
        TaskList = s_bl?.Task.ReadAll(); // Reads all tasks from the data source.
        InitializeComponent();
        this.isAdmin = isAdmin; // Sets the isAdmin flag.
    }

    // Property to bind a list of tasks to a UI element.
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

    // Event handler for selection change in task status combo box.
    private void cbTaskStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // Updates the TaskList based on selected status.
        TaskList = (TaskStatus == BO.Enums.Status.None) ? s_bl?.Task.ReadAll()! : s_bl?.Task.ReadAll(task => task.Status == TaskStatus)!;
    }

    // Event handler for double-click event on the task list to update a task's details.
    private void doubleClickEvent_UpdateTask(object sender, MouseButtonEventArgs e)
    {
        // Gets the selected task and opens a new TaskWindow to update its details.
        BO.TaskInList? taskInList = (sender as ListView)?.SelectedItem as BO.TaskInList;
        Close();
        new TaskWindow(s_bl.Task.Read(taskInList.Id), false).ShowDialog();
    }

    // Event handler for add task button click.
    private void btnAddTask_Click(object sender, RoutedEventArgs e)
    {
        // Closes the current window and opens a new TaskWindow to add a new task.
        Close();
        new TaskWindow(new BO.Task(), true).ShowDialog();
    }
}
