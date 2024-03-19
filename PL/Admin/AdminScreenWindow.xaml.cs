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
    public AdminScreenWindow()
    {
        InitializeComponent(); // Initializes the window components.
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
        new GanttChartWindow().Show();
    }

    private void Add_Engineer_Click(object sender, RoutedEventArgs e)
    {
        new EngineerWindow(new BO.Engineer(), true).ShowDialog();
    }
    
    private void Generate_Schedule_Click(object sender, RoutedEventArgs e)
    {
        //generates schedule and locks certain modifcations 
        scheduleCreated = true;
    }
}
