using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Foundation;
using UIKit;
using Xamarin.EZiOS.Interfaces;

namespace Xamarin.EZiOS
{
    /// <summary>
    ///     Simplifies the job of a UITableViewSource.
    ///     If a race condition is encountered with the animation thread, the indexPath returned by Cocoa will be negative, out of range of the array,
    ///     crashing and causing a bad user experience.
    ///     This rarely occurs, but can happen if you are doing too much work or calling TableView.ReloadData in a fast loop.
    ///     If you encounter this, investigate what could be slowing down your animation thread.
    ///     In debug, a race condition is asserted, so you will be notified.  In release it will be handled gracefully and logged.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="UIKit.UITableViewSource" />
    public abstract class EZTableViewSource<T> : UITableViewSource where T : class, IEZRow<T>
    {
        protected readonly UITableViewController _parentViewController;

        protected EZTableViewSource(UITableViewController parentViewController)
        {
            _parentViewController = parentViewController;
        }

        /// <summary>
        /// The list of EZSections, which each contain Rows.
        /// </summary>
        public List<EZSection<T>> EZSections { get; protected set; }

        /// <summary>
        ///     Gets or sets the can edit row function for the entire TableView
        /// </summary>
        public Func<UITableView, NSIndexPath, bool> CanEditRowFunc { get; set; }

        /// <summary>
        /// Refreshes the sections.
        /// Will Clear the sections first, cleanup any resources from the old sections, and reset them.
        /// </summary>
        protected internal virtual void RefreshSections()
        {
            ClearSectionsAndUnhookSubscriptions();
            EZSections.AddRange(ConstructSections());
            _parentViewController.TableView.ReloadData();
        }

        /// <summary>
        /// Clears the sections and unhook subscriptions.
        /// </summary>
        protected internal void ClearSectionsAndUnhookSubscriptions()
        {
            EZSections.Clear();
            //todo: unhook 
        }

        /// <summary>
        /// Constructs the sections.
        /// </summary>
        protected abstract IEnumerable<EZSection<T>> ConstructSections();

        /// <summary>
        /// Gets the row or default (null) if the IndexPath is out of Range.
        ///
        /// </summary>
        /// <param name="indexPath">The index path.</param>
        public IEZRow<T> GetRowOrDefault(NSIndexPath indexPath)
            => indexPath.IsOutOfRange(EZSections) ? null : EZSections[indexPath.Section][indexPath.Row];

        /// <summary>
        /// Deques the reusable cell.
        /// </summary>
        /// <param name="tableView">The table view.</param>
        /// <param name="row">The row.</param>
        /// <returns></returns>
        protected virtual UITableViewCell DequeReusableCell(UITableView tableView, IEZRow<T> row)
            => tableView.DequeueReusableCell(string.IsNullOrWhiteSpace(row.ReuseIdentifier)
                ? "cell"
                : row.ReuseIdentifier);

        /// <summary>
        /// Applies the default styles to cell.
        /// </summary>
        /// <param name="cell">The cell.</param>
        /// <param name="row">The row.</param>
        /// <returns></returns>
        protected virtual UITableViewCell ApplyDefaultStyleToCell(UITableViewCell cell, IEZRow<T> row)
        {
            cell.TextLabel.Text = row.Title;
            if (row.CellStyle == UITableViewCellStyle.Subtitle)
                cell.DetailTextLabel.Text = row.SubTitle;
            cell.Accessory = row.CellAccessory;

            var ezRow = row as EZRow<T>;
            return ezRow == null ? cell : ApplyDefaultStyleToEZRowConcrete(ezRow, cell);
        }

        /// <summary>
        /// Applies the default styles to EZRow concrete.
        /// </summary>
        /// <param name="ezRow">The EZRow.</param>
        /// <param name="cell">The cell.</param>
        /// <returns></returns>
        protected virtual UITableViewCell ApplyDefaultStyleToEZRowConcrete(EZRow<T> ezRow, UITableViewCell cell)
        {
            if (ezRow.Image != null)
                cell.ImageView.Image = ezRow.Image;

            return cell;
        }

        #region Overrides of UITableViewSource

        /// <summary>
        ///     Returns the number of sections that are required to display the data.
        /// </summary>
        /// <param name="tableView">Table view displaying the sections.</param>
        /// <returns>
        ///     Number of sections required to display the data. The default is 1 (a table must have at least one section).
        /// </returns>
        /// <remarks>
        ///     Declared in [UITableViewDataSource]
        /// </remarks>
        public override nint NumberOfSections(UITableView tableView) => EZSections.Count;

        /// <summary>
        ///     Called by the table view to find out how many rows are to be rendered in the section specified by
        ///     <paramref name="section" />.
        /// </summary>
        /// <param name="tableview">Table view displaying the rows.</param>
        /// <param name="section">Index of the section containing the rows.</param>
        /// <returns>
        ///     Number of rows in the section at index <paramref name="section" />.
        /// </returns>
        /// <remarks>
        ///     Declared in [UITableViewDataSource]
        /// </remarks>
        public override nint RowsInSection(UITableView tableview, nint section) => EZSections[(int)section].Count;

        #region Overrides of UITableViewSource

        /// <summary>
        /// Called to populate the header for the specified section.
        /// </summary>
        /// <returns>
        /// Text to display in the section header, or <see langword="null"/> if no title is required.
        /// </returns>
        public override string TitleForHeader(UITableView tableView, nint section) => EZSections[(int) section].HeaderTitle;

        #region Overrides of UITableViewSource

        /// <summary>
        /// Called to populate the footer for the specified section.
        /// </summary>
        /// <returns>
        /// Text to display in the section footer, or <see langword="null"/> if no title is required.
        /// </returns>
        public override string TitleForFooter(UITableView tableView, nint section) => EZSections[(int) section].FooterTitle;

        #endregion

        #endregion

        /// <summary>
        ///     Called by the table view to populate the row at <paramref name="indexPath" /> with a cell view.
        ///     Will attempt to dequeue a reusable cell from tableView. If a reuseIdentifier is found, it will use it, else it will
        ///     use the default string "cell".
        ///     The Reuse Identifier is set in the storyboard, or manually in code-behind, and is a unique identifier for the table
        ///     view to cache reusable cells.
        ///     Will also attempt to apply EZ default styles to the cell, or any cell styles you configure.
        /// </summary>
        /// <param name="tableView">Table view requesting the cell.</param>
        /// <param name="indexPath">Location of the row where the cell will be displayed.</param>
        /// <returns>
        ///     An object that inherits from <see cref="T:UIKit.UITableViewCell" /> that the table can use for the specified row.
        ///     Do not return <see langword="null" /> or an assertion will be raised.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         This method is called once for each row that is visible on screen. During scrolling, it is called additional
        ///         times as new rows come into view. Cells that disappear from view are cached by the table view. The
        ///         implementation of this method should call the table view's
        ///         <see cref="M:UIKit.UITableView.DequeueReusableCell(Foundation.NSString)" /> method to obtain a cached cell
        ///         object for reuse (if <see langword="null" /> is returned, create a new cell instance). Be sure to reset all
        ///         properties of a reused cell.
        ///     </para>
        ///     <para>Declared in [UITableViewDataSource]</para>
        /// </remarks>
        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var row = GetRowOrDefault(indexPath);
            Debug.Assert(row != null, IndexPathOutOfRangeAssertionMessage(indexPath));
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (row == null)
            {
                WriteIndexPathOutOfRangeMessageToConsole(indexPath);
                return new UITableViewCell(); //bail in case of race condition
            }

            var cell = DequeReusableCell(tableView, row);

            return ApplyDefaultStyleToCell(cell, row);
        }

        /// <summary>
        ///     Whether the row located at <paramref name="indexPath" /> should be editable.
        ///     Row behavior overrides Table-Wide behavior.
        ///     If this method is not implemented, all rows are assumed to be non-editable. (This differs from the default iOS
        ///     behavior, where the default if not implemented is editable)
        /// </summary>
        /// <param name="tableView">Table view containing the row.</param>
        /// <param name="indexPath">Location of the row.</param>
        /// <returns>
        ///     <see langword="true" /> if the row is editable, otherwise <see langword="false" />.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         This method allows specific rows to be editable or not. Editable rows display the insertion or deletion
        ///         control in their cell when the table view is in editing mode, or allow for swipte actions.
        ///     </para>
        ///     <para>
        ///         Rows that are not editable will ignore the <see cref="P:UIKit.UITableViewCell.EditingStyle" /> property and
        ///         will not be indented.
        ///     </para>
        ///     <para>
        ///         Rows that are editable, but should not display the insertion or deletion control, can return
        ///         <see cref="F:UIKit.UITableViewCellEditingStyle.None" /> from the
        ///         <see cref="M:UIKit.UITableViewSource.EditingStyleForRow(UIKit.UITableView,Foundation.NSIndexPath)" /> method on
        ///         the table view's <see cref="T:UIKit.UITableViewSource" />.
        ///     </para>
        ///     <para>Declared in [UITableViewDataSource]</para>
        /// </remarks>
        public override bool CanEditRow(UITableView tableView, NSIndexPath indexPath)
        {
            var row = GetRowOrDefault(indexPath);
            Debug.Assert(row != null, IndexPathOutOfRangeAssertionMessage(indexPath));
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (row == null)
            {
                WriteIndexPathOutOfRangeMessageToConsole(indexPath);
                return false; //bail in case of race condition
            }

            var canEditRowOverride = row.EditRowActions?.Any() ?? false;
            if (canEditRowOverride)
                return true; //row behavior overrides tableview behavior

            return CanEditRowFunc?.Invoke(tableView, indexPath) ?? false; //check tableview behavior last
        }

        #endregion

        void WriteIndexPathOutOfRangeMessageToConsole(NSIndexPath indexPath) => Console.WriteLine(IndexPathOutOfRangeAssertionMessage(indexPath));
        string IndexPathOutOfRangeAssertionMessage(NSIndexPath indexPath)
            =>
                "Race condition with Cocoa animation thread likely: indexPath was out of range. " +
                $"Current length of EZSections: {EZSections.Count} - " +
                $"IndexPath values: {indexPath.Section}(section), {indexPath.Row}(row)";
    }
}
