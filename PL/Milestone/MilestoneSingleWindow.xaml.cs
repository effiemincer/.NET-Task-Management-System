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

namespace Milestone;

/// <summary>
/// Interaction logic for MilestoneSingleWindow.xaml
/// </summary>
public partial class MilestoneSingleWindow : Window
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
    public BO.Enums.Status MilestoneStatus { get; set; } = BO.Enums.Status.None;
    BO.Milestone CurrentMilestone = new BO.Milestone();
    private int _milestoneId;
    public MilestoneSingleWindow(int milestoneId)
    {
        InitializeComponent();
        _milestoneId = milestoneId;
        if (milestoneId != 0)
        {
            CurrentMilestone = s_bl?.Milestone.Read(milestoneId);
            //DataContext = CurrentMilestone;
        }


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

