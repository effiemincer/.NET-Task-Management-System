using Engineer;
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
using static BO.Enums;

namespace Task;

/// <summary>
/// Interaction logic for TaskListWindow.xaml
/// </summary>
public partial class TaskListWindow : Window
{
    private bool isAdmin;
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
    public BO.Enums.Status TaskStatus { get; set; } = BO.Enums.Status.None;

    public TaskListWindow(bool isAdmin)
    {

        TaskList = s_bl?.Task.ReadAll();
        InitializeComponent();
        this.isAdmin = isAdmin;

    }

    public IEnumerable<BO.TaskInList> TaskList
    {
        get { return (IEnumerable<BO.TaskInList>)GetValue(TaskListProperty); }
        set { SetValue(TaskListProperty, value); }
    }

    public static readonly DependencyProperty TaskListProperty =
        DependencyProperty.Register("TaskList",
            typeof(IEnumerable<BO.TaskInList>),
            typeof(TaskListWindow),
            new PropertyMetadata(null)
        );

    private void cbTaskStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        TaskList = (TaskStatus == BO.Enums.Status.None) ? s_bl?.Task.ReadAll()! : s_bl?.Task.ReadAll(task => task.Status == TaskStatus)!;
    }

    private void doubleClickEvent_UpdateTask(object sender, MouseButtonEventArgs e)
    {
        BO.TaskInList? taskInList = (sender as ListView)?.SelectedItem as BO.TaskInList;
        new TaskWindow(s_bl.Task.Read(taskInList.Id), false).ShowDialog();
    }
}
