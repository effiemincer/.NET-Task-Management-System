using BO;
using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
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

namespace Task;

/// <summary>
/// Interaction logic for TaskWindow.xaml
/// </summary>
public partial class TaskWindow : Window
{

    // Dependency property to bind a Task object
    public static readonly DependencyProperty TaskProperty =
        DependencyProperty.Register("CurrentTask", typeof(BO.Task), typeof(TaskWindow), new PropertyMetadata(null));

    // Property to get or set the current task
    public BO.Task CurrentTask
    {
        get { return (BO.Task)GetValue(TaskProperty); }
        set { SetValue(TaskProperty, value); }
    }

    private bool isAdd; // Flag indicating if it's adding a new task

    private bool isDeliverableFirstRun; // Flag indicating if the deliverable property has been set for the first time

    static readonly BlApi.IBl s_bl = BlApi.Factory.Get(); // Reference to business logic instance

    private bool? scheduleCreated = s_bl.Config.GetIsScheduleGenerated();

    /// <summary>
    /// Constructor for the TaskWindow class.
    /// </summary>
    /// <param name="task_"></param>
    /// <param name="isAdd_"></param>
    public TaskWindow(BO.Task task_, bool isAdd_ = false, bool engineerPage = false)
    {
        InitializeComponent();
        CurrentTask = task_; // Initialize current task
        isAdd = isAdd_; // Initialize flag for adding a task
        isDeliverableFirstRun = task_.Deliverable; // Initialize flag for deliverable property
        _dependenciesListDisplayTextBlock.Text = DependenciesListDisplay;
        if (engineerPage)
        {
            assignEngID.Visibility = Visibility.Collapsed;
            assignBtn.Visibility = Visibility.Collapsed;
        }
        //allowing adding depenedencies
        if (isAdd)
        {
            _dateCreated.Text = DateTime.Now.ToString("dd/MM/yyyy");
            if (scheduleCreated is null || !(bool)scheduleCreated)
            {
                _dependenciesListDisplayTextBlock.IsEnabled = true;
                _deadline.IsEnabled = true;
                _projectedStart.IsEnabled = true;
                _requiredEffort.IsEnabled = true;
            }
            else
            {
                _dependenciesListDisplayTextBlock.IsEnabled = false;
                _deadline.IsEnabled = false;
                _projectedStart.IsEnabled = false;
                _requiredEffort.IsEnabled = false;
            }
        }
        //changing the screen layout depending on if the scheudle has been created or not
        else
        {
            if (scheduleCreated is null || !(bool)scheduleCreated)
            {
                _dependenciesListDisplayTextBlock.IsEnabled = false;
                _deadline.IsEnabled = true;
                _projectedStart.IsEnabled = true;
                _requiredEffort.IsEnabled = true;
            }
            else
            {
                _dependenciesListDisplayTextBlock.IsEnabled = false;
                _deadline.IsEnabled = false;
                _projectedStart.IsEnabled = false;
                _requiredEffort.IsEnabled = false;
            }
        }

    }

    /// <summary>
    /// Event handler for the AddTask button click event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        return;
    }

   /// <summary>
   /// Event handler for the AddTask button click event.
   /// </summary>
   /// <param name="sender"></param>
   /// <param name="e"></param>
    private void btnAddUpdate_Click(object sender, RoutedEventArgs e)
    {
        // Add or update task based on isAdd flag

        //add
        if (isAdd)
        {
            try {
                //allow for adding dependencies
                //put a constructor for a task here
                TimeSpan? timeSpan = TimeSpan.Parse(_requiredEffort.Text);

                //public List<BO.TaskInList>? Dependencies { get; set; }
                string dependencies = _dependenciesListDisplayTextBlock.Text;

                List<BO.TaskInList> dependenciesList = new List<BO.TaskInList>();

                if (dependencies != "")
                {
                    string[] dependenciesArray = dependencies.Split(", ");
                    foreach (string dependency in dependenciesArray)
                    {
                        if (int.TryParse(dependency, out int id))
                        {
                            BO.TaskInList taskInList = new BO.TaskInList
                            {
                                Id = id
                            };
                            dependenciesList.Add(taskInList);
                        }
                    }
                }

                BO.Task newTask = new BO.Task
                {
                    Id = 0,
                    Alias = _alias.Text,
                    Description = _description.Text,
                    Deadline = _deadline.SelectedDate,
                    Status = CurrentTask!.Status,
                    Engineer = CurrentTask!.Engineer,
                    DateCreated = DateTime.Now,
                    ActualEndDate = _actualEndDate.SelectedDate,
                    ActualStartDate = _actualStartDate.SelectedDate,
                    Complexity = (BO.Enums.EngineerExperience?)Complexity_ComboBox.SelectedValue,
                    Deliverable = (bool)_deliverable.IsChecked!,
                    Dependencies = dependenciesList,   //change this to the dependencies list
                    Milestone = CurrentTask!.Milestone,
                    ProjectedStartDate = _projectedStart.SelectedDate,
                    Remarks = _remarks.Text,
                    RequiredEffortTime = timeSpan
                };

                s_bl?.Task.Create(newTask); 
            
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "BadDataInput", MessageBoxButton.OK, MessageBoxImage.Error); 
                return;
            }
            
        }

        //update
        else
        {

            //update with the schedule created
            bool? isScheduleGenerated = s_bl.Config.GetIsScheduleGenerated();
            if (isScheduleGenerated is not null && (bool)isScheduleGenerated)
            {
                try
                {
                    BO.Task updatedTask = new BO.Task
                    {
                        Id = CurrentTask!.Id,
                        Alias = _alias.Text,
                        Description = _description.Text,
                        Deadline = _deadline.SelectedDate,
                        Status = CurrentTask!.Status,
                        Engineer = CurrentTask!.Engineer,
                        DateCreated = CurrentTask!.DateCreated,
                        ActualEndDate = _actualEndDate.SelectedDate,
                        ActualStartDate = _actualStartDate.SelectedDate,
                        Complexity = (BO.Enums.EngineerExperience?)Complexity_ComboBox.SelectedValue,
                        Deliverable = (bool)_deliverable.IsChecked!,
                        Dependencies = CurrentTask!.Dependencies,
                        Milestone = CurrentTask!.Milestone,
                        ProjectedStartDate = CurrentTask!.ProjectedStartDate,
                        Remarks = _remarks.Text,
                        RequiredEffortTime = CurrentTask!.RequiredEffortTime
                    };
                    s_bl?.Task.Update(updatedTask);
                }
                catch (Exception)
                {
                    MessageBox.Show("Invalid time format. Please enter time in the format HH:MM:SS", "Invalid Time Format", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            //update if the schedule was not yet created
            else
            {
                try
                {
                    TimeSpan? timeSpan = TimeSpan.Parse(_requiredEffort.Text);
                    BO.Task updatedTask = new BO.Task
                    {
                        Id = CurrentTask!.Id,
                        Alias = _alias.Text,
                        Description = _description.Text,
                        Deadline = CurrentTask!.Deadline,
                        Status = CurrentTask!.Status,
                        Engineer = CurrentTask!.Engineer,
                        DateCreated = CurrentTask!.DateCreated,
                        ActualEndDate = _actualEndDate.SelectedDate,
                        ActualStartDate = _actualStartDate.SelectedDate,
                        Complexity = (BO.Enums.EngineerExperience?)Complexity_ComboBox.SelectedValue,
                        Deliverable = (bool)_deliverable.IsChecked!,
                        Dependencies = CurrentTask!.Dependencies,
                        Milestone = CurrentTask!.Milestone,
                        ProjectedStartDate = _projectedStart.SelectedDate,
                        Remarks = _remarks.Text,
                        RequiredEffortTime = timeSpan
                    };
                    s_bl?.Task.Update(updatedTask);
                }
                catch (Exception)
                {
                    MessageBox.Show("Invalid time format. Please enter time in the format HH:MM:SS", "Invalid Time Format", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            
        }
        Close();
    }

    /// <summary>
    /// Event handler for the AddTask button click event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void cbTaskStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // Update TaskList based on selected task status
        //TaskList = (TaskStatus == BO.Enums.Status.None) ? s_bl?.Task.ReadAll()! : s_bl?.Task.ReadAll(task => task.Status == TaskStatus)!;
        return;
    }

    // Flag to indicate if RadioButton was just checked
    private bool JustChecked;

   /// <summary>
   /// Event handler for RadioButton Checked event.
   /// </summary>
   /// <param name="sender"></param>
   /// <param name="e"></param>
    private void RB_Checked(object sender, RoutedEventArgs e)
    {
        RadioButton s = sender as RadioButton;
        // Action on Check...

        JustChecked = true;
        if (isDeliverableFirstRun)
        {
            JustChecked = false;
            isDeliverableFirstRun = false;
        }
    }

    /// <summary>
    /// Event handler for RadioButton Clicked event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void RB_Clicked(object sender, RoutedEventArgs e)
    {
        if (JustChecked)
        {
            JustChecked = false;
            e.Handled = true;
            return;
        }
        RadioButton s = sender as RadioButton;
        if ((bool)s.IsChecked)
            s.IsChecked = false;

    }

    /// <summary>
    /// Property to display the list of dependencies for the current task.
    /// </summary>
    private string DependenciesListDisplay
    {
        get
        {
            StringBuilder sb = new StringBuilder(); // Initializes a StringBuilder to build the display string.
            if (CurrentTask.Dependencies is null) // Checks if the list of dependencies is null.
                return ""; // Returns an empty string if there are no dependencies.

            int count = CurrentTask.Dependencies.Count; // Gets the count of dependencies.

            // Iterates through each dependent task.
            foreach (BO.TaskInList task in CurrentTask.Dependencies)
            {
                sb.Append(task.Id.ToString()); // Appends the task ID to the StringBuilder.
                if (--count > 0) // Checks if there are more dependencies remaining.
                    sb.Append(", "); // Appends a comma if there are more dependencies.
            }
            return sb.ToString(); // Returns the string representation of the dependencies list.
        }
    }

    private void Assign_Engineer_Click(object sender, RoutedEventArgs e)
    {
        try
        {

            BO.Engineer engineer = s_bl.Engineer.Read(int.Parse(assignEngID.Text));
            // Validates engineer ID input.
            if (string.IsNullOrEmpty(assignEngID.Text))
            {
                MessageBox.Show("Please enter an ID", "EngNotFound", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (!int.TryParse(assignEngID.Text, out _))
            {
                MessageBox.Show("ID must be a number", "EngNotFound", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (!s_bl.Engineer.isEngineer(int.Parse(assignEngID.Text)))
            {
                MessageBox.Show("Engineer not found", "EngNotFound", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Check if Engineer is assigned to any task
            if (engineer.Task is not null)
            {
                throw new Exception("Can't assign multiple tasks to an engineer");
            }

            s_bl.Task.assignEng(int.Parse(assignEngID.Text), CurrentTask.Id);
            MessageBox.Show("Engineer assigned successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            assignEngID.Text = "";
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Doesn't work", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        //add success message
    }

    private void AssignEng_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            Assign_Engineer_Click(sender, e);
        }
    }
}
