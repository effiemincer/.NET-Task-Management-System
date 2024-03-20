using BO; // Importing the BO namespace for access to its functionalities.
using Engineer; // Importing the Engineer namespace for access to its functionalities.
using Milestone; // Importing the Milestone namespace for access to its functionalities.
using PL.Admin;
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

namespace PL; // Declaring the namespace for the PL (Presentation Layer) class.

/// <summary>
/// Interaction logic for AdminScreenWindow.xaml
/// </summary>
public partial class AdminScreenWindow : Window
{
    // Constructor for AdminScreenWindow class.
    private bool scheduleCreated = false;
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

    // Dependency property for binding the current engineer.
    public static readonly DependencyProperty TimeProperty =
        DependencyProperty.Register("CurrentTime", typeof(DateTime), typeof(AdminScreenWindow), new PropertyMetadata(null));

    // Property to get or set the current engineer.
    public DateTime CurrentTime
    {
        get { return (DateTime)GetValue(TimeProperty); }
        set { SetValue(TimeProperty, s_bl.Clock); }
    }

    public AdminScreenWindow()
    {
        InitializeComponent(); // Initializes the window components.
        CurrentTime = s_bl.Clock; // Sets the current time.    
    }

    // Event handler for "Manage Tasks" button click.
    private void Manage_Tasks_Click(object sender, RoutedEventArgs e)
    {
        // Opens the TaskListWindow with isAdmin set to true, indicating admin access.
        new TaskListWindow(true).Show();
    }

    // Event handler for "Manage Engineers" button click.
    private void Manage_Engineers_Click(object sender, RoutedEventArgs e)
    {
        // Opens the EngineerListWindow.
        new EngineerListWindow().Show();
    }

    // Event handler for "Manage Milestones" button click.
    private void Manage_Milestones_Click(object sender, RoutedEventArgs e)
    {
        if (scheduleCreated)
        {
            // Opens the MilestoneListWindow.
            new MilestoneListWindow().Show();
        }
        else
        {
            MessageBox.Show("You must generate a schedule first in order to see milestones.", "NoScheduleGenerated", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void Gantt_Chart_Click(object sender, RoutedEventArgs e)
    {
        if (scheduleCreated)
        {
            // Opens the Gantt Chart Window.
            new GanttChartWindow().Show();
        }
        else
        {
            MessageBox.Show("You must generate a schedule first in order to see the gantt chart view.", "NoScheduleGenerated", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        
    }

    private void Add_Engineer_Click(object sender, RoutedEventArgs e)
    {
        new EngineerWindow(new BO.Engineer(), true).ShowDialog();
    }
    
    private void Generate_Schedule_Click(object sender, RoutedEventArgs e)
    {
        //generates schedule and locks certain modifcations 
        try
        {
            //need to put in actual dates here probably prompt the user or something 
            s_bl.Milestone.CreateSchedule(DateTime.MinValue, DateTime.MaxValue);
            scheduleCreated = true;
        }
        catch (Exception ex) {
            MessageBox.Show(ex.Message, "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        

    }

    private void Travel_Forwards_Day_Click(object sender, RoutedEventArgs e)
    {
        s_bl.travelForwardsDay();
        CurrentTime = s_bl.Clock;
    }

    private void Travel_Forwards_Hour_Click(object sender, RoutedEventArgs e)
    {
        s_bl.travelForwardsHour();
        CurrentTime = s_bl.Clock;
    }

    private void Travel_Backwards_Day_Click(object sender, RoutedEventArgs e)
    {
        s_bl.travelBackwardDay();
        CurrentTime = s_bl.Clock;
    }

    private void Travel_Backwards_Hour_Click(object sender, RoutedEventArgs e)
    {
        s_bl.travelBackwardHour();
        CurrentTime = s_bl.Clock;
    }

    private void Reset_Clock_Click(object sender, RoutedEventArgs e)
    {
        s_bl.resetClock();
        CurrentTime = s_bl.Clock;
    }

    private void Back_Click(object sender, RoutedEventArgs e)
    {
        new MainWindow().Show();
        Close();
    }
}
