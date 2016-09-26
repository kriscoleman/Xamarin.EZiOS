using System;
using UIKit;
using Xamarin.EZiOS.Interfaces;

namespace Xamarin.EZiOS
{
    public class EZRow : IEZRow
    {
        public EZRow() { }

        public EZRow(string title, string subTitle)
        {
            Title = title;
            SubTitle = subTitle;
        }

        #region Implementation of IEZRow

        public virtual string Title { get; }
        public virtual string SubTitle { get; }
        public virtual UIImage Image { get; set; }
        public UITableViewCellStyle CellStyle { get; set; }
        public virtual UITableViewCellAccessory CellAccessory { get; set; }
        public virtual UITableViewRowAction[] EditRowActions { get; set; }
        public virtual string ReuseIdentifier { get; set; }

        #endregion
    }

    public class EZRow<T> : EZRow
    {
        readonly string _titleOnCreated;
        readonly string _subTitleOnCreated;
        readonly string _cellReuseIdentifierOnCreated;
        readonly UITableViewCellAccessory _cellAccessoryOnCreated;

        public EZRow(T item, Func<T, string> getTitleFunc, Func<T, string> getSubtitleFunc = null,
            Func<T, UITableViewCellAccessory> getCellAccessoryFunc = null,
            UITableViewCellStyle? cellStyle = null, Func<T, string> getCellReuseIdentifierFunc = null)
            : this(
                item, getTitleFunc(item), getSubtitleFunc?.Invoke(item), getCellAccessoryFunc?.Invoke(item), cellStyle,
                getCellReuseIdentifierFunc?.Invoke(item))
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

        public T Item { get; }
        public Func<T, string> GetTitleFunc { get; set; }
        public Func<T, string> GetSubtitleFunc { get; set; }
        public Func<T, string> GetCellReuseIdentifierFunc { get; set; }
        public Func<T, UITableViewCellAccessory> GetCellAccessoryFunc { get; set; }
        public Func<T, UITableViewRowAction[]> GetEditRowActionsFunc { get; set; }
        public override string Title => GetTitleFunc?.Invoke(Item) ?? _titleOnCreated;
        public override string SubTitle => GetSubtitleFunc?.Invoke(Item) ?? _subTitleOnCreated;

        public override UITableViewCellAccessory CellAccessory
        {
            get { return GetCellAccessoryFunc?.Invoke(Item) ?? _cellAccessoryOnCreated; }
            set { GetCellAccessoryFunc = _ => value; }
        }

        public override UITableViewRowAction[] EditRowActions
        {
            get { return GetEditRowActionsFunc?.Invoke(Item) ?? Array.Empty<UITableViewRowAction>(); }
            set { GetEditRowActionsFunc = _ => value; }
        }

        public override string ReuseIdentifier
        {
            get { return GetCellReuseIdentifierFunc?.Invoke(Item) ?? _cellReuseIdentifierOnCreated; }
            set { GetCellReuseIdentifierFunc = _ => value; }
        }

    }
}