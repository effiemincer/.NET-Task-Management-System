using Milestone; // Importing the Milestone namespace for access to its functionalities.
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace Engineer; // Declaring the namespace for the Engineer class.

/// <summary>
/// Interaction logic for EngineerWindow.xaml
/// </summary>
public partial class EngineerWindow : Window
{

    static readonly BlApi.IBl s_bl = BlApi.Factory.Get(); // Static reference to the business logic layer.

    // Dependency property for binding the current engineer.
    public static readonly DependencyProperty EngineerProperty =
        DependencyProperty.Register("CurrentEngineer", typeof(BO.Engineer), typeof(EngineerWindow), new PropertyMetadata(null));

    // Property to get or set the current engineer.
    public BO.Engineer CurrentEngineer
    {
        get { return (BO.Engineer)GetValue(EngineerProperty); }
        set { SetValue(EngineerProperty, value); }
    }
    private bool isAdd; // Flag to indicate whether it's adding a new engineer or updating an existing one.

    // Constructor for EngineerWindow class.
    public EngineerWindow(BO.Engineer CurrentEngineer_, bool isAdd_)
    {
        InitializeComponent();
        CurrentEngineer = CurrentEngineer_; // Sets the current engineer.
        isAdd = isAdd_; // Sets the flag to indicate if it's adding or updating.
    }

    // Event handler for text changed in text boxes.
    private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        return; // Placeholder, not implemented yet.
    }

    // Event handler for selection change in engineer status combo box.
    private void cbEngineerStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        return; // Placeholder, not implemented yet.
    }

    // Event handler for add/update engineer button click.
    private void btnAddUpdate_EngineerClick(object sender, RoutedEventArgs e)
    {
        // Checks if adding or updating an engineer.
        if (isAdd)
        {
            // Calls the business logic layer to create the current engineer.
            s_bl?.Engineer.Create(CurrentEngineer);
        }

        //update engineer
        else
        {
            try
            {
                BO.Task? task = s_bl?.Task.Read(int.Parse(_task.Text));
                if (task == null)
                {
                    MessageBox.Show("Task not found", "TaskNotFound", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                BO.Engineer updatedEng = new BO.Engineer { Id = CurrentEngineer.Id, Name = _name.Text, EmailAddress = _email.Text, ExperienceLevel = (BO.Enums.EngineerExperience?)Status_ComboBox.SelectedValue, CostPerHour = int.Parse(_cost.Text), Task = new BO.TaskInEngineer(task!.Id, task!.Alias) };
                BO.Task updatedTask = new BO.Task { Id = task!.Id, Alias = task!.Alias, Description = task!.Description, Deadline = task!.Deadline, Status = task!.Status, 
                    Engineer = new BO.EngineerInTask(updatedEng.Id, updatedEng.Name), DateCreated = task!.DateCreated, ActualEndDate = task!.ActualEndDate, ActualStartDate = task!.ActualStartDate, 
                    Complexity = task!.Complexity, Deliverable = task!.Deliverable, Dependencies = task!.Dependencies, Milestone = task!.Milestone, ProjectedStartDate = task!.ProjectedStartDate,
                    Remarks = task!.Remarks, RequiredEffortTime = task!.RequiredEffortTime };
                // Calls the business logic layer to update the current engineer.
                s_bl?.Engineer.Update(updatedEng);
                s_bl?.Task.Update(updatedTask);

            }
            catch (BO.BlDoesNotExistException)
            {
                MessageBox.Show("Task not found", "TaskNotFound", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }


        }
        // Shows the EngineerListWindow and closes the current window.
        Close();
    }

}
