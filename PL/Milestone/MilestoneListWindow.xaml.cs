using Engineer;
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
using static BO.Enums;

namespace Milestone;

/// <summary>
/// Interaction logic for MilestoneListWindow.xaml
/// </summary>
public partial class MilestoneListWindow : Window
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
    public BO.Enums.Status MilestoneStatus { get; set; } = BO.Enums.Status.None;
    public MilestoneListWindow()
    {
       MilestoneList = s_bl?.Milestone.ReadAll();
        InitializeComponent();
    }

    public IEnumerable<BO.MilestoneInList> MilestoneList
    {
        get { return (IEnumerable<BO.MilestoneInList>)GetValue(MilestoneListProperty); }
        set { SetValue(MilestoneListProperty, value); }
    }

    public static readonly DependencyProperty MilestoneListProperty =
        DependencyProperty.Register("MilestoneList",
            typeof(IEnumerable<BO.MilestoneInList>),
            typeof(MilestoneListWindow),
            new PropertyMetadata(null)
        );

    private void cbMilestoneStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        MilestoneList = (MilestoneStatus == BO.Enums.Status.None) ? s_bl?.Milestone.ReadAll()! : s_bl?.Milestone.ReadAll(item => item.Status == MilestoneStatus)!;
    }

    private void bcAdd_Milestone(object sender, RoutedEventArgs e)
    {
        new MilestoneSingleWindow(0).Show();
    }
}
