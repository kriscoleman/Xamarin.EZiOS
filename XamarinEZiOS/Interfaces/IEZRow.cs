using UIKit;

namespace Xamarin.EZiOS.Interfaces
{
    public interface IEZRow
    {
        string Title { get; }
        string SubTitle { get; }
        UITableViewCellStyle CellStyle { get; }
        UITableViewCellAccessory CellAccessory { get; set; }
        UITableViewRowAction[] EditRowActions { get; set; }
        string ReuseIdentifier { get; set; }
    }
}