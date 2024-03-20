using Engineer; // Importing the Engineer namespace for access to its functionalities.
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
using static BO.Enums; // Importing the BO.Enums namespace to use enums directly.

namespace Milestone; // Declaring the namespace for the MilestoneListWindow class.

/// <summary>
/// Interaction logic for MilestoneListWindow.xaml
/// </summary>
public partial class MilestoneListWindow : Window
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get(); // Static reference to the business logic layer.

    // Property to get or set the milestone status.
    public BO.Enums.Status MilestoneStatus { get; set; } = BO.Enums.Status.None;

    // Constructor for MilestoneListWindow class.
    public MilestoneListWindow()
    {
        MilestoneList = s_bl?.Milestone.ReadAll(); // Reads all milestones from the data source.
        InitializeComponent();
    }

    // Property to bind a list of milestones to a UI element.
    public IEnumerable<BO.MilestoneInList> MilestoneList
    {
        get { return (IEnumerable<BO.MilestoneInList>)GetValue(MilestoneListProperty); }
        set { SetValue(MilestoneListProperty, value); }
    }

    // Dependency property for MilestoneList.
    public static readonly DependencyProperty MilestoneListProperty =
        DependencyProperty.Register("MilestoneList",
            typeof(IEnumerable<BO.MilestoneInList>),
            typeof(MilestoneListWindow),
            new PropertyMetadata(null)
        );

    // Event handler for selection change in milestone status combo box.
    private void cbMilestoneStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // Updates the MilestoneList based on selected milestone status.
        MilestoneList = (MilestoneStatus == BO.Enums.Status.None) ? s_bl?.Milestone.ReadAll()! : s_bl?.Milestone.ReadAll(item => item.Status == MilestoneStatus)!;
    }

    // Event handler for double-click event on the milestone list to view details of a milestone.
    private void doubleClickEvent_UpdateMilestone(object sender, MouseButtonEventArgs e)
    {
        // Gets the selected milestone and opens a new MilestoneSingleWindow to view its details.
        BO.MilestoneInList milestoneInList = (sender as ListView)?.SelectedItem as BO.MilestoneInList;
        if (milestoneInList is null)
            return;
        new MilestoneSingleWindow(s_bl.Milestone.Read(milestoneInList!.Id), false).ShowDialog();
        MilestoneList = null; // Resets the MilestoneList to refresh the UI.
        MilestoneList = s_bl?.Milestone.ReadAll(); // Reads all milestones from the data source.
    }
}
