using System;
using UIKit;
using Xamarin.EZiOS.Interfaces;

namespace Xamarin.EZiOS
{
    public class EZRow<T> : IEZRow<T>
    {
        readonly string _titleOnCreated;
        readonly string _subTitleOnCreated;
        readonly string _cellReuseIdentifierOnCreated;
        readonly UITableViewCellAccessory _cellAccessoryOnCreated;

        public EZRow(T item, Func<T, string> getTitleFunc, Func<T, string> getSubtitleFunc = null, Func<T, UITableViewCellAccessory> getCellAccessoryFunc = null,
            UITableViewCellStyle? cellStyle = null, Func<T, string> getCellReuseIdentifierFunc = null)
            : this(item, getTitleFunc(item), getSubtitleFunc?.Invoke(item), getCellAccessoryFunc?.Invoke(item), cellStyle, getCellReuseIdentifierFunc?.Invoke(item))
        {
            GetTitleFunc = getTitleFunc;
            GetSubtitleFunc = getSubtitleFunc;
            GetCellAccessoryFunc = getCellAccessoryFunc;
            GetCellReuseIdentifierFunc = getCellReuseIdentifierFunc;
        }

        public EZRow(T item, string title, string subTitleOnCreated, UITableViewCellAccessory? cellAccessory, UITableViewCellStyle? cellStyle = null, string cellReuseIdentifier = null)
        {
            _titleOnCreated = title;
            _subTitleOnCreated = subTitleOnCreated;
            _cellReuseIdentifierOnCreated = cellReuseIdentifier;
            _cellAccessoryOnCreated = cellAccessory ?? UITableViewCellAccessory.None;
            Item = item;
            CellStyle = cellStyle ??
                        (!string.IsNullOrWhiteSpace(title) && !string.IsNullOrWhiteSpace(subTitleOnCreated)
                            ? UITableViewCellStyle.Subtitle
                            : UITableViewCellStyle.Default);
        }

        public Func<T, string> GetTitleFunc { get; set; }
        public Func<T, string> GetSubtitleFunc { get; set; }
        public Func<T, string> GetCellReuseIdentifierFunc { get; set; } 
        public Func<T, UITableViewCellAccessory> GetCellAccessoryFunc { get; set; }
        public Func<T, UITableViewRowAction[]> GetEditRowActionsFunc { get; set; } 
        public UIImage Image { get; set; }

        #region Implementation of IEZRow<T>

        public string Title => GetTitleFunc?.Invoke(Item) ?? _titleOnCreated;
        public string SubTitle => GetSubtitleFunc?.Invoke(Item) ?? _subTitleOnCreated;
        public UITableViewCellStyle CellStyle { get; }
        public T Item { get; }
        public UITableViewCellAccessory CellAccessory
        {
            get { return GetCellAccessoryFunc?.Invoke(Item) ?? _cellAccessoryOnCreated; }
            set { GetCellAccessoryFunc = _ => value; }
        }

        public UITableViewRowAction[] EditRowActions
        {
            get { return GetEditRowActionsFunc?.Invoke(Item) ?? Array.Empty<UITableViewRowAction>(); }
            set { GetEditRowActionsFunc = _ => value; }
        }

        public string ReuseIdentifier
        {
            get { return GetCellReuseIdentifierFunc?.Invoke(Item) ?? _cellReuseIdentifierOnCreated; }
            set { GetCellReuseIdentifierFunc = _ => value; }
        }
        #endregion
    }
}