using BO; // Importing the BO namespace for access to its functionalities.
using Engineer; // Importing the Engineer namespace for access to its functionalities.
using Milestone; // Importing the Milestone namespace for access to its functionalities.
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
        // Opens the MilestoneListWindow.
        new MilestoneListWindow().Show();
    }
}
