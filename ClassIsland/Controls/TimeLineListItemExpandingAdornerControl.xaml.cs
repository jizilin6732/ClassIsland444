﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ClassIsland.Models;
using static System.Net.Mime.MediaTypeNames;

namespace ClassIsland.Controls;

/// <summary>
/// TimeLineListItemExpandingAdornerControl.xaml 的交互逻辑
/// </summary>
public partial class TimeLineListItemExpandingAdornerControl : UserControl
{
    private static double BaseTicks { get; } = 1000000000.0;

    public static readonly DependencyProperty TimePointProperty = DependencyProperty.Register(
        nameof(TimePoint), typeof(TimeLayoutItem), typeof(TimeLineListItemExpandingAdornerControl), new PropertyMetadata(default(TimeLayoutItem)));

    public TimeLayoutItem TimePoint
    {
        get { return (TimeLayoutItem)GetValue(TimePointProperty); }
        set { SetValue(TimePointProperty, value); }
    }

    public static readonly DependencyProperty TimeLayoutProperty = DependencyProperty.Register(
        nameof(TimeLayout), typeof(ObservableCollection<TimeLayoutItem>), typeof(TimeLineListItemExpandingAdornerControl), new PropertyMetadata(default(ObservableCollection<TimeLayoutItem>)));

    public ObservableCollection<TimeLayoutItem> TimeLayout
    {
        get { return (ObservableCollection<TimeLayoutItem>)GetValue(TimeLayoutProperty); }
        set { SetValue(TimeLayoutProperty, value); }
    }

    public static readonly DependencyProperty ScaleProperty = DependencyProperty.Register(
        nameof(Scale), typeof(double), typeof(TimeLineListItemExpandingAdornerControl), new PropertyMetadata(default(double)));

    public double Scale
    {
        get { return (double)GetValue(ScaleProperty); }
        set { SetValue(ScaleProperty, value); }
    }

    public TimeLineListItemExpandingAdornerControl()
    {
        InitializeComponent();
    }

    private TimeSpan RoundTime(TimeSpan time)
    {
        var b = TimeSpan.FromMinutes(5).Ticks;
        return TimeSpan.FromTicks((long)Math.Round((double)time.Ticks / b) * b);
    }

    private TimeSpan GetDelta(TimeSpan raw, double v)
    {
        var tsDelta = TimeSpan.FromTicks((long)(v / Scale * BaseTicks));
        var t = RoundTime(raw + tsDelta);
        return t - raw;
    }

    private TimeLayoutItem? GetPrevTimeLayoutItem()
    {
        var tp = TimePoint;
        var index = TimeLayout.IndexOf(tp);
        while (index > 0)
        {
            index--;
            if (TimeLayout[index].TimeType != 2)
            {
                return TimeLayout[index];
            }
        }
        return null;
    }

    private TimeLayoutItem? GetNextTimeLayoutItem()
    {
        var tp = TimePoint;
        var index = TimeLayout.IndexOf(tp);
        while (index < TimeLayout.Count - 1)
        {
            index++;
            if (TimeLayout[index].TimeType != 2)
            {
                return TimeLayout[index];
            }
        }
        return null;
    }

    private void ThumbTop_OnDragDelta(object sender, DragDeltaEventArgs e)
    {
        var v = e.VerticalChange;
        var d = GetDelta(TimePoint.StartSecond.TimeOfDay, v);
        if (TimePoint.StartSecond.TimeOfDay + d >= TimePoint.EndSecond.TimeOfDay)
        {
            return;
        }

        var prev = GetPrevTimeLayoutItem();
        if (prev != null)
        {
            if (TimePoint.StartSecond.TimeOfDay +d < prev.EndSecond.TimeOfDay)
                return;
        }
        TimePoint.StartSecond += d;
    }

    private void ThumbBottom_OnDragDelta(object sender, DragDeltaEventArgs e)
    {
        var v = e.VerticalChange;
        var d = GetDelta(TimePoint.EndSecond.TimeOfDay, v);
        if (TimePoint.EndSecond.TimeOfDay + d <= TimePoint.StartSecond.TimeOfDay)
        {
            return;
        }
        var next = GetNextTimeLayoutItem();
        if (next != null)
        {
            if (TimePoint.EndSecond.TimeOfDay + d > next.StartSecond.TimeOfDay)
                return;
        }
        TimePoint.EndSecond += d;
    }
}