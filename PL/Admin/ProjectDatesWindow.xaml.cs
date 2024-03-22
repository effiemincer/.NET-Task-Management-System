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
    public ProjectDatesWindow()
    {
        InitializeComponent();
        DateTime? projectStart = s_bl.Config.GetProjectStartDate();
        DateTime? projectEnd = s_bl.Config.GetProjectEndDate();
        if (projectStart is not null)
        {
            _projectStartDate.Text = projectStart.Value.ToString("dd/MM/yyyy");

        }
        if (projectEnd is not null)
        {
            _projectEndDate.Text = projectEnd.Value.ToString("dd/MM/yyyy");
        }
       
    }

    private void _saveButton_Click(object sender, RoutedEventArgs e)
    {
        if (DateTime.TryParse(_projectStartDate.Text, out DateTime projectStart) && DateTime.TryParse(_projectEndDate.Text, out DateTime projectEnd))
        {
            if (projectStart > projectEnd)
            {
                MessageBox.Show("Project start date must be before project end date", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (projectStart.Day < s_bl.Clock.Day+1 && projectStart.Month < s_bl.Clock.Month && projectStart.Year <  s_bl.Clock.Year)
            {
                MessageBox.Show("Project start date must be after the current clock date", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            s_bl.Config.SetProjectStartDate(projectStart);
            s_bl.Config.SetProjectEndDate(projectEnd);
            MessageBox.Show("Project dates saved successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            Close();
        }
        else
        {
            MessageBox.Show("Invalid date format", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
