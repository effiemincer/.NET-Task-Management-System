using BO; // Importing the BO namespace for access to its functionalities.
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

namespace Milestone; // Declaring the namespace for the Milestone class.

/// <summary>
/// Interaction logic for MilestoneSingleWindow.xaml
/// </summary>
public partial class MilestoneSingleWindow : Window
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get(); // Static reference to the business logic layer.

    // Property to get or set the milestone's status.
    public BO.Enums.Status MilestoneStatus { get; set; } = BO.Enums.Status.None;

    // Dependency property for binding the current milestone.
    public static readonly DependencyProperty MilestoneProperty =
            DependencyProperty.Register("CurrentMilestone", typeof(BO.Milestone), typeof(MilestoneSingleWindow), new PropertyMetadata(null));

    // Property to get or set the current milestone.
    public BO.Milestone CurrentMilestone
    {
        get { return (BO.Milestone)GetValue(MilestoneProperty); }
        set { SetValue(MilestoneProperty, value); }
    }

    private bool isAdd; // Flag to indicate whether it's adding a new milestone or updating an existing one.

    // Constructor for MilestoneSingleWindow class.
    public MilestoneSingleWindow(BO.Milestone CurrentMilestone_, bool isAdd_)
    {
        InitializeComponent();
        CurrentMilestone = CurrentMilestone_; // Sets the current milestone.
        isAdd = isAdd_; // Sets the flag to indicate if it's adding or updating.
       
        _dependenciesListDisplayTextBlock.Text = DependentsListDisplay;
        _requisitesListDisplayTextBlock.Text = RequisitesListDisplay;
    }


    // Event handler for selection change in milestone status combo box.
    private void cbMilestoneStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        return; // Placeholder, not implemented yet.
    }

    // Event handler for text changed in text boxes.
    private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        return; // Placeholder, not implemented yet.
    }

    private void btnUpdate_Milestone(object sender, RoutedEventArgs e)
    {
        s_bl?.Milestone.Update(CurrentMilestone.Id, _alias.Text, _description.Text, _remarks.Text); // Adds the milestone to the database.
        Close(); // Closes the window.
    }

    public string RequisitesListDisplay
    {
        get
        {
            StringBuilder sb = new StringBuilder();
            List<int> requisiteIds = s_bl.Milestone.getMilestoneIdList(CurrentMilestone.Id);
            int count = requisiteIds.Count;
            foreach (int ids in requisiteIds)
            {
                sb.Append(ids);
                if (--count > 0)
                    sb.Append(", ");
            }
            return sb.ToString();

        }
    }
    public string DependentsListDisplay
    {
        get
        {
            StringBuilder sb = new StringBuilder();
            List<int> dependentIds = s_bl.Milestone.getMilestoneDef(CurrentMilestone.Id);
            int count = dependentIds.Count;
            foreach (int ids in dependentIds)
            {
                sb.Append(ids);
                if (--count > 0)
                    sb.Append(", ");
                if (count%6 == 0)
                {
                    sb.Append("\n");
                }
            }
            return sb.ToString();

        }
    }
}
