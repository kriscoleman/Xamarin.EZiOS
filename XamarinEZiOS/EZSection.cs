using System;
using System.Collections.Generic;
using Xamarin.EZiOS.Interfaces;

namespace Xamarin.EZiOS
{
    /// <summary>
    /// Represents a Section in a UITableView
    /// </summary>
    /// <typeparam name="T">the Type of object being displayed in a list in the TableView</typeparam>
    /// <seealso cref="System.Collections.Generic.List{T}" />
    public class EZSection<T> : List<T> where T : class, IEZRow<T>
    {
        readonly string _headerTitleOnCreated;
        readonly string _footerTitleOnCreated;

        public EZSection(Func<string> getHeaderTitleFunc, Func<string> getFooterTitleFunc = null)
            : this(getHeaderTitleFunc(), getFooterTitleFunc?.Invoke())
        {
            GetHeaderTitleFunc = getHeaderTitleFunc;
            GetFooterTitleFunc = getFooterTitleFunc;
        }

        public EZSection(string headerTitle, string footerTitle = null)
        {
            _headerTitleOnCreated = headerTitle;
            _footerTitleOnCreated = footerTitle;
        }

        public Func<string> GetHeaderTitleFunc { get; set; }
        public Func<string> GetFooterTitleFunc { get; set; }

        public string HeaderTitle
        {
            get { return GetHeaderTitleFunc?.Invoke() ?? _headerTitleOnCreated; }
            set { GetHeaderTitleFunc = () => value; }
        }

        public string FooterTitle
        {
            get { return GetFooterTitleFunc?.Invoke() ?? _footerTitleOnCreated; }
            set { GetFooterTitleFunc = () => value; }
        }
    }
}