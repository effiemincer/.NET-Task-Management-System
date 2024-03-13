using BO;
using Milestone;
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
using Task;

namespace Milestone;

/// <summary>
/// Interaction logic for MilestoneSingleWindow.xaml
/// </summary>
public partial class MilestoneSingleWindow : Window
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
    public BO.Enums.Status MilestoneStatus { get; set; } = BO.Enums.Status.None;

    public static readonly DependencyProperty MilestoneProperty =
            DependencyProperty.Register("CurrentMilestone", typeof(BO.Milestone), typeof(MilestoneSingleWindow), new PropertyMetadata(null));

    public BO.Milestone CurrentMilestone
    {
        get { return (BO.Milestone)GetValue(MilestoneProperty); }
        set { SetValue(MilestoneProperty, value); }
    }

    private bool isAdd;
    public MilestoneSingleWindow(BO.Milestone CurrentMilestone_, bool isAdd_)
    {
        InitializeComponent();
        CurrentMilestone = CurrentMilestone_;
        isAdd = isAdd_;

    }

    private void cbMilestoneStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        return;
    }

    private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        return;
    }
}

