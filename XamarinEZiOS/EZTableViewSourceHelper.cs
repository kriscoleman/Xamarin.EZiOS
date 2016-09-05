using System;
using System.Collections.Generic;
using Foundation;
using UIKit;
using Xamarin.EZiOS.Interfaces;

namespace Xamarin.EZiOS
{
    public static class EZTableViewSourceHelper
    {
        /// <summary>
        /// Will add an image to the row.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row">The row.</param>
        /// <param name="image">The image.</param>
        /// <returns></returns>
        public static EZRow<T> WithImage<T>(this EZRow<T> row, UIImage image)
        {
            row.Image = image;
            return row;
        }

        /// <summary>
        /// Will apply the cell accessory to the row.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row">The row.</param>
        /// <param name="cellAccessory">The cell accessory.</param>
        /// <returns></returns>
        public static IEZRow<T> WithCellAccessory<T>(this IEZRow<T> row, UITableViewCellAccessory cellAccessory)
        {
            row.CellAccessory = cellAccessory;
            return row;
        }

        /// <summary>
        /// Will apply the cell reuse identifier to the row.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row">The row.</param>
        /// <param name="cellReuseIdentifier">The cell reuse identifier.</param>
        /// <returns></returns>
        public static IEZRow<T> WithCellReuseIdentifier<T>(this IEZRow<T> row, string cellReuseIdentifier)
        {
            row.ReuseIdentifier = cellReuseIdentifier;
            return row;
        }

        /// <summary>
        /// Will apply the cell reuse identifier to the row.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row">The row.</param>
        /// <param name="getCellReuseIdentifierFunc">The get cell reuse identifier function.</param>
        /// <returns></returns>
        public static EZRow<T> WithCellReuseIdentifier<T>(this EZRow<T> row, Func<T, string> getCellReuseIdentifierFunc)
        {
            row.GetCellReuseIdentifierFunc = getCellReuseIdentifierFunc;
            return row;
        } 

        /// <summary>
        /// Will apply the EditActiosn to the row.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row">The row.</param>
        /// <param name="editRowActions">The edit row actions.</param>
        /// <returns></returns>
        public static IEZRow<T> WithEditActions<T>(this IEZRow<T> row, UITableViewRowAction[] editRowActions)
        {
            row.EditRowActions = editRowActions;
            return row;
        }

        /// <summary>
        /// Determines whether the indexPath is out of range of the specified sections.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="indexPath">The index path.</param>
        /// <param name="sections">The sections.</param>
        /// <returns>
        ///   <c>true</c> if [is out of range] [the specified sections]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsOutOfRange<T>(this NSIndexPath indexPath, List<EZSection<T>> sections) where T : class, IEZRow<T>
        {
            if (indexPath.Section < 0 || indexPath.Section + 1 > sections.Count)
                return true;
            return indexPath.Row < 0 || indexPath.Row + 1 > sections[indexPath.Section].Count;
        } 
    }
}