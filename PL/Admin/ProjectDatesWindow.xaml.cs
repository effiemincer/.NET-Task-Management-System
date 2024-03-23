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

namespace PL.Admin;

/// <summary>
/// Interaction logic for ProjectDatesWindow.xaml
/// </summary>
public partial class ProjectDatesWindow : Window
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get(); // Reference to business logic instance

    /// <summary>
    /// ProjectDatesWindow constructor
    /// </summary>
    public ProjectDatesWindow()
    {
        InitializeComponent();

        // Retrieves project start and end dates from the configuration
        DateTime? projectStart = s_bl.Config.GetProjectStartDate();
        DateTime? projectEnd = s_bl.Config.GetProjectEndDate();

        // Populates the text boxes with the retrieved dates if they exist
        if (projectStart is not null)
        {
            _projectStartDate.Text = projectStart.Value.ToString("dd/MM/yyyy");
        }
        if (projectEnd is not null)
        {
            _projectEndDate.Text = projectEnd.Value.ToString("dd/MM/yyyy");
        }
    }

    /// <summary>
    /// Event handler for the save button click event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void _saveButton_Click(object sender, RoutedEventArgs e)
    {
        // Attempts to parse the input text boxes into DateTime objects
        if (DateTime.TryParse(_projectStartDate.Text, out DateTime projectStart) && DateTime.TryParse(_projectEndDate.Text, out DateTime projectEnd))
        {
            // Checks if the project start date is before the project end date
            if (projectStart > projectEnd)
            {
                MessageBox.Show("Project start date must be before project end date", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            // Checks if the project start date is after the current clock date
            if (projectStart.Day < s_bl.Clock.Day + 1 && projectStart.Month < s_bl.Clock.Month && projectStart.Year < s_bl.Clock.Year)
            {
                MessageBox.Show("Project start date must be after the current clock date", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            // Sets the project start and end dates in the configuration
            s_bl.Config.SetProjectStartDate(projectStart);
            s_bl.Config.SetProjectEndDate(projectEnd);

            // Displays a success message and closes the window
            MessageBox.Show("Project dates saved successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            Close();
        }
        else
        {
            // Displays an error message if the input date format is invalid
            MessageBox.Show("Invalid date format", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
