﻿using UIKit;

namespace Xamarin.EZiOS.Interfaces
{
    public interface IEZRow<out T>
    {
        string Title { get; }
        string SubTitle { get; }
        UITableViewCellStyle CellStyle { get; }
        T Item { get; }
        UITableViewCellAccessory CellAccessory { get; set; }
        UITableViewRowAction[] EditRowActions { get; set; }
        string ReuseIdentifier { get; set; }
    }
}