using BlApi;
using BO; // Importing the BO namespace for access to its functionalities.
using Engineer; // Importing the Engineer namespace for access to its functionalities.
using Milestone; // Importing the Milestone namespace for access to its functionalities.
using PL.Admin;
using System.Windows;

using Task; // Importing the Task namespace for access to its functionalities.

namespace PL; // Declaring the namespace for the PL (Presentation Layer) class.

/// <summary>
/// Interaction logic for AdminScreenWindow.xaml
/// </summary>
public partial class AdminScreenWindow : Window
{
    // Constructor for AdminScreenWindow class.
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

    private bool? ScheduleCreated = s_bl.Config.GetIsScheduleGenerated();

    // Dependency property for binding the current engineer.
    public static readonly DependencyProperty TimeProperty =
        DependencyProperty.Register("CurrentTime", typeof(DateTime), typeof(AdminScreenWindow), new PropertyMetadata(null));

    // Property to get or set the current engineer.
    public DateTime CurrentTime
    {
        get { return (DateTime)GetValue(TimeProperty); }
        set { SetValue(TimeProperty, s_bl.Clock); }
    }

    /// <summary>
    /// AdminScreenWindow constructor.
    /// </summary>
    public AdminScreenWindow()
    {
        InitializeComponent(); // Initializes the window components.
        CurrentTime = s_bl.Clock; // Sets the current time.    
        ScheduleCreated = s_bl.Config.GetIsScheduleGenerated();
        if (ScheduleCreated is null)
        {
            _milestones.Visibility = Visibility.Collapsed;
            _gantt.Visibility = Visibility.Collapsed;
            _generate.Visibility = Visibility.Visible;
            _terminate.Visibility = Visibility.Collapsed;
        }
        else if (!(bool)ScheduleCreated)
        {
            _generate.Visibility = Visibility.Visible;
            _terminate.Visibility = Visibility.Collapsed;
        }
        else
        {
            _generate.Visibility = Visibility.Collapsed;
            _terminate.Visibility = Visibility.Visible;
        }
    }

    /// <summary>
    /// Event handler for "Manage Tasks" button click.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Manage_Tasks_Click(object sender, RoutedEventArgs e)
    {
        // Opens the TaskListWindow with isAdmin set to true, indicating admin access.
        new TaskListWindow(true).Show();
    }

    /// <summary>
    /// Event handler for "Manage Engineers" button click.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Manage_Engineers_Click(object sender, RoutedEventArgs e)
    {
        // Opens the EngineerListWindow.
        new EngineerListWindow().Show();
    }

   /// <summary>
   /// Event handler for "Manage Milestones" button click.
   /// </summary>
   /// <param name="sender"></param>
   /// <param name="e"></param>
    private void Manage_Milestones_Click(object sender, RoutedEventArgs e)
    {
        if (ScheduleCreated is null)
        {
            MessageBox.Show("You must initialize data.", "NoIsScheduleGeneratedInXML", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        else if ((bool)ScheduleCreated)
        {
            // Opens the MilestoneListWindow.
            new MilestoneListWindow().Show();
        }
        else
        {
            MessageBox.Show("You must generate a schedule first in order to see milestones.", "NoScheduleGenerated", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    /// <summary>
    /// Event handler for "Gantt Chart" button click.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Gantt_Chart_Click(object sender, RoutedEventArgs e)
    {
        if (ScheduleCreated is null)
        {
            MessageBox.Show("You must initialize data.", "NoIsScheduleGeneratedInXML", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        else if ((bool)ScheduleCreated)
        {
            // Opens the Gantt Chart Window.
            new GanttChartWindow().Show();
        }
        else
        {
            MessageBox.Show("You must generate a schedule first in order to see the gantt chart view.", "NoScheduleGenerated", MessageBoxButton.OK, MessageBoxImage.Error);
        }

    }

    /// <summary>
    /// Event handler for "Add Task" button click.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Add_Engineer_Click(object sender, RoutedEventArgs e)
    {
        new EngineerWindow(new BO.Engineer(), true).ShowDialog();
    }

    /// <summary>
    /// Event handler for "Generate Schedule/Terminate Project" button click.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Generate_Schedule_Terminate_Project_Click(object sender, RoutedEventArgs e)
    {
        IEnumerable<BO.TaskInList?> taskInLists = s_bl.Task.ReadAll();
        if (taskInLists is null || s_bl.Config.GetProjectStartDate() is null || s_bl.Config.GetProjectEndDate() is null)
        {
            MessageBox.Show("You must initialize data.", "NoIsScheduleGeneratedInXML", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        else if (ScheduleCreated is null || !(bool)ScheduleCreated)
        {
            // Generates schedule and locks certain modifications.
            try
            {
                s_bl.Milestone.CreateSchedule((DateTime)s_bl.Config.GetProjectStartDate()!, (DateTime)s_bl.Config.GetProjectEndDate()!);
                ScheduleCreated = true;
                s_bl.Config.SetIsScheduleGenerated(true);
                _generate.Visibility = Visibility.Collapsed;
                _terminate.Visibility = Visibility.Visible;
                _gantt.Visibility = Visibility.Visible;
                _milestones.Visibility = Visibility.Visible;

                MessageBox.Show("Schedule has been generated.", "GenerationSuccess", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        else
        {
            // Terminate program was pressed.
            ScheduleCreated = false;
            s_bl.Config.SetIsScheduleGenerated(false);
            _generate.Visibility = Visibility.Visible;
            _terminate.Visibility = Visibility.Collapsed;
            _gantt.Visibility = Visibility.Collapsed;
            _milestones.Visibility = Visibility.Collapsed;

            s_bl.Milestone.Reset();

            MessageBox.Show("Schedule has been terminated.", "TerminationSuccess", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    /// <summary>
    /// Event handler for "Set Project Dates" button click.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Set_Project_Dates_Click(object sender, RoutedEventArgs e)
    {
        MessageBoxResult res = MessageBox.Show("By changing the Project Start and End Dates you will reset all data. Are you sure you want to reset all the data?", "ResetConfirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);
        if (res == MessageBoxResult.Yes)
        {
            _milestones.Visibility = Visibility.Collapsed;
            _gantt.Visibility = Visibility.Collapsed;
            _generate.Visibility = Visibility.Visible;
            _terminate.Visibility = Visibility.Collapsed;
            s_bl.Config.Reset();
            s_bl.Config.SetIsScheduleGenerated(false);
            s_bl.Milestone.Reset();
            MessageBox.Show("Data was reset. You may now change project start and end dates.", "ResetSuccessful", MessageBoxButton.OK, MessageBoxImage.Information);
            new ProjectDatesWindow().ShowDialog();
        }

    }

    /// <summary>
    /// Travel forwards/backwards in time by a day.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Travel_Forwards_Day_Click(object sender, RoutedEventArgs e)
    {
        s_bl.travelForwardsDay();
        CurrentTime = s_bl.Clock;
    }

    /// <summary>
    /// Travel forwards/backwards in time by an hour.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Travel_Forwards_Hour_Click(object sender, RoutedEventArgs e)
    {
        s_bl.travelForwardsHour();
        CurrentTime = s_bl.Clock;
    }

    /// <summary>
    /// Travel backwards in time by a day.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Travel_Backwards_Day_Click(object sender, RoutedEventArgs e)
    {
        s_bl.travelBackwardDay();
        CurrentTime = s_bl.Clock;
    }

    /// <summary>
    /// Travel backwards in time by an hour.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Travel_Backwards_Hour_Click(object sender, RoutedEventArgs e)
    {
        s_bl.travelBackwardHour();
        CurrentTime = s_bl.Clock;
    }


    /// <summary>
    /// Reset the clock to the current time.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Reset_Clock_Click(object sender, RoutedEventArgs e)
    {
        s_bl.resetClock();
        CurrentTime = s_bl.Clock;
    }

    /// <summary>
    /// Event handler for "Back" button click.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Back_Click(object sender, RoutedEventArgs e)
    {
        new MainWindow().Show();
        Close();
    }
}
