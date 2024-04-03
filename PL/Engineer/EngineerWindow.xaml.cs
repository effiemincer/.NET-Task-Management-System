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

   /// <summary>
   /// EngineerWindow constructor
   /// </summary>
   /// <param name="CurrentEngineer_"></param>
   /// <param name="isAdd_"></param>
    public EngineerWindow(BO.Engineer CurrentEngineer_, bool isAdd_)
    {
        InitializeComponent();
        CurrentEngineer = CurrentEngineer_; // Sets the current engineer.
        isAdd = isAdd_; // Sets the flag to indicate if it's adding or updating.
        if (isAdd)
        {
            deleteEngBtn.Visibility = Visibility.Collapsed;
            _Id.IsEnabled = true;
        }
        else
        {
            _Id.IsEnabled = false;
        }
    }

    /// <summary>
    /// Event handler for the window loaded event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        return; // Placeholder, not implemented yet.
    }

    /// <summary>
    /// Event handler for the EngineerExperience selection changed event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void cbEngineerStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        return; // Placeholder, not implemented yet.
    }

    /// <summary>
    /// Event handler for the Add/Update Engineer button click event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnAddUpdate_EngineerClick(object sender, RoutedEventArgs e)
    {
        // Checks if adding or updating an engineer.
        if (isAdd)
        {
            try
            {
                BO.Engineer newEng = new BO.Engineer { Id = int.Parse(_Id.Text), Name = _name.Text, EmailAddress = _email.Text, ExperienceLevel = (BO.Enums.EngineerExperience?)Status_ComboBox.SelectedValue, CostPerHour = int.Parse(_cost.Text), Task = null };

                // Calls the business logic layer to create the current engineer.
                s_bl?.Engineer.Create(newEng);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

        }

        else if (s_bl.Milestone.isMilestone(int.Parse(_task.Text)))
        {
            MessageBox.Show("Can't assign an Engineer to a milestone", "TaskNotFound", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        //update engineer
        else
        {

            try
            {
                if (_task.Text == "")
                {
                    BO.Engineer engineer = s_bl?.Engineer.Read(CurrentEngineer.Id);
                    BO.Task task1 = s_bl?.Task.Read((int)engineer!.Task!.Id);

                    BO.Engineer updatedEng1 = new BO.Engineer { Id = CurrentEngineer.Id, Name = _name.Text, EmailAddress = _email.Text, ExperienceLevel = (BO.Enums.EngineerExperience?)Status_ComboBox.SelectedValue, CostPerHour = int.Parse(_cost.Text), Task = null };
                    BO.Task updatedTask1 = new BO.Task
                    {
                        Id = task1!.Id,
                        Alias = task1!.Alias,
                        Description = task1!.Description,
                        Deadline = task1!.Deadline,
                        Status = task1!.Status,
                        Engineer = null,
                        DateCreated = task1!.DateCreated,
                        ActualEndDate = task1!.ActualEndDate,
                        ActualStartDate = task1!.ActualStartDate,
                        Complexity = task1!.Complexity,
                        Deliverable = task1!.Deliverable,
                        Dependencies = task1!.Dependencies,
                        Milestone = task1!.Milestone,
                        ProjectedStartDate = task1!.ProjectedStartDate,
                        Remarks = task1!.Remarks,
                        RequiredEffortTime = task1!.RequiredEffortTime
                    };
                    // Calls the business logic layer to update the current engineer.
                    s_bl?.Engineer.Update(updatedEng1);
                    s_bl?.Task.Update(updatedTask1);
                    Close();
                    return;
                }

                BO.Task? task = s_bl?.Task.Read(int.Parse(_task.Text));
                if (task == null)
                {
                    MessageBox.Show("Task not found", "TaskNotFound", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                BO.Engineer updatedEng = new BO.Engineer { Id = CurrentEngineer.Id, Name = _name.Text, EmailAddress = _email.Text, ExperienceLevel = (BO.Enums.EngineerExperience?)Status_ComboBox.SelectedValue, CostPerHour = int.Parse(_cost.Text), Task = new BO.TaskInEngineer(task!.Id, task!.Alias) };
                BO.Task updatedTask = new BO.Task
                {
                    Id = task!.Id,
                    Alias = task!.Alias,
                    Description = task!.Description,
                    Deadline = task!.Deadline,
                    Status = task!.Status,
                    Engineer = new BO.EngineerInTask(updatedEng.Id, updatedEng.Name),
                    DateCreated = task!.DateCreated,
                    ActualEndDate = task!.ActualEndDate,
                    ActualStartDate = task!.ActualStartDate,
                    Complexity = task!.Complexity,
                    Deliverable = task!.Deliverable,
                    Dependencies = task!.Dependencies,
                    Milestone = task!.Milestone,
                    ProjectedStartDate = task!.ProjectedStartDate,
                    Remarks = task!.Remarks,
                    RequiredEffortTime = task!.RequiredEffortTime
                };
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

    private void Delete_Engineer_Click(object sender, RoutedEventArgs e)
    {
        MessageBoxResult res = MessageBox.Show("Are you sure you want to delete this engineer?", "Delete Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);

        // Resets all data if confirmed by the user.
        if (res == MessageBoxResult.Yes)
        {
            s_bl!.Engineer.Delete(CurrentEngineer.Id); MessageBox.Show("Delete complete!", "DeleteSuccessful", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        Close();
    }

}
