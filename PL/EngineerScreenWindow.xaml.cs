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
using Task; // Importing the Task namespace for access to its functionalities.

namespace PL;

/// <summary>
/// Interaction logic for EngineerScreenWindow.xaml
/// </summary>
public partial class EngineerScreenWindow : Window
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get(); // Static reference to the business logic layer.

    private int engID; // Field to store the ID of the user.

    // Dependency property for binding the current engineer.
    public static readonly DependencyProperty EngineerProperty =
        DependencyProperty.Register("CurrentEng", typeof(BO.Engineer), typeof(EngineerScreenWindow), new PropertyMetadata(null));

    /// <summary>
    /// Engineer property to get or set the current engineer.
    /// </summary>
    public BO.Engineer CurrentEng
    {
        get { return (BO.Engineer)GetValue(EngineerProperty); }
        set { SetValue(EngineerProperty, value); }
    }

    /// <summary>
    /// Constructor for the EngineerScreenWindow class.
    /// </summary>
    /// <param name="_id"></param>
    public EngineerScreenWindow(int _id)
    {
        InitializeComponent(); // Initializes the window components.
        engID = _id; // Sets the ID field to the given ID.
        CurrentEng = s_bl?.Engineer.Read(_id)!;
    }

    /// <summary>
    /// Event handler for the "Tasks" button click event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Tasks_Button_Click(object sender, RoutedEventArgs e)
    {
        // Opens the TaskListWindow with isAdmin set to false, indicating non-admin access.
        new TaskListWindow(false).Show();
    }

   /// <summary>
   /// Event handler for the "Current Task" button click event.
   /// </summary>
   /// <param name="sender"></param>
   /// <param name="e"></param>
    private void Current_Task_Button_Click(object sender, RoutedEventArgs e)
    {
        // Opens the TaskWindow for a specific task 
        if (CurrentEng.Task is null)
        {
            MessageBox.Show(CurrentEng.Name + " does not have any currently assigned Tasks.", "EngNoTask", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        else
        {
            new TaskWindow(s_bl!.Task.Read((int)CurrentEng.Task!.Id!)!, engineerPage: true).ShowDialog();
        }
       
    }

    /// <summary>
    /// The event handler for the "Back" button click event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Back_Button_Click(object sender, RoutedEventArgs e)
    {
        new MainWindow().Show();
        Close();
    }

    private void Finish_Task_Click(object sender, RoutedEventArgs e)
    {
        // Opens the TaskWindow for a specific task 
        if (CurrentEng.Task is null)
        {
            MessageBox.Show(CurrentEng.Name + " does not have any currently assigned Tasks.", "EngNoTask", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        else
        {
            s_bl.Task.finishTask(CurrentEng.Id, (int)CurrentEng.Task.Id);
            CurrentEng.Task = null;
            MessageBox.Show("Task completed!! :D", "TaskDone", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
