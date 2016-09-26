using System;
using System.Collections.Generic;
using System.Linq;
using UIKit;

namespace Xamarin.EZiOS
{
    /// <summary>
    /// A TableViewController for an EZTableViewSource and UITableView. 
    /// Works with reflection interop using storyboard instantiation. 
    /// </summary>
    /// <seealso cref="UIKit.UITableViewController" />
    public class EZTableViewController : UITableViewController
    {
        public EZTableViewController()
        { }

        public EZTableViewController(IntPtr handle) : base(handle)
        { }

        /// <summary>
        /// Refreshes the Sections of the EZTableViewSource and ReloadsData on the TableView
        /// </summary>
        public Action RefreshSectionsAndReloadData { get; private set; }

        /// <summary>
        /// Clears the Sections of the EZTableViewSource and ReloadsData on the TableView, unhooking any cell event subscriptions
        /// </summary>
        public Action ClearSectionsAndUnhookSubscriptions { get; set; }

        /// <summary>
        /// Prepares the view to load. SHOULD BE EXECUTED IN THE VIEW CONTROLLER's LOADVIEW OVERRIDE!
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableViewSource">The table view source.</param>
        public void PrepareViewToLoad(EZTableViewSource tableViewSource)
        {
            TableView.Source = tableViewSource;
            RefreshSectionsAndReloadData = tableViewSource.RefreshSections;
            ClearSectionsAndUnhookSubscriptions = tableViewSource.ClearSectionsAndUnhookSubscriptions;
        }


        #region Overrides of UIViewController


        /// <summary>
        /// Called prior to the <see cref="P:UIKit.UIViewController.View"/> being added to the view hierarchy.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method is called prior to the <see cref="T:UIKit.UIView"/> that is this <see cref="T:UIKit.UIViewController"/>’s <see cref="P:UIKit.UIViewController.View"/> property being added to the display <see cref="T:UIKit.UIView"/> hierarchy. 
        /// </para>
        /// <para>
        /// Application developers who override this method must call <c>base.ViewWillAppear()</c> in their overridden method.
        /// </para>
        /// </remarks>
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            RefreshSectionsAndReloadData();
        }

        /// <summary>
        /// <para>
        /// This method is called prior to the removal of the <see cref="T:UIKit.UIView"/>that is this <see cref="T:UIKit.UIViewController"/>’s <see cref="P:UIKit.UIViewController.View"/> from the display <see cref="T:UIKit.UIView"/> hierarchy.
        /// </para>
        /// <para>
        /// Application developers may override this method to configure animations, resign first responder status (see <see cref="M:UIKit.UIResponder.ResignFirstResponder"/>), or perform other tasks.
        /// </para>
        /// <para>
        /// Application developers who override this method must call <c>base.ViewWillDisappear()</c> in their overridden method.
        /// </para>
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method is called in response to a view being removed from a view hierarchy. This method is called before the view is actually removed and before any animations are configured.
        /// </para>
        /// <para>
        /// Subclasses can override this method and use it to commit editing changes, resign the first responder status of the view, or perform other relevant tasks. For example, you might use this method to revert changes to the orientation or style of the status bar that were made in the viewDidDisappear: method when the view was first presented. If you override this method, you must call super at some point in your implementation. 
        /// </para>
        /// </remarks>
        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            ClearSectionsAndUnhookSubscriptions();
        }

        #endregion
    }

    /// <summary>
    /// A TableViewController for an EZTableViewSource and UITableView. 
    /// Does not work with reflection interop using storyboard instantiation. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="UIKit.UITableViewController" />
    public class EZTableViewControllerBase : EZTableViewController
    {
        readonly Func<IEnumerable<EZSection>> _constructSectionsFunc;
        readonly UITableViewController _parentViewController;

        public EZTableViewControllerBase(Func<IEnumerable<EZSection>> constructSectionsFunc, UITableViewController parentViewController)
        {
            _constructSectionsFunc = constructSectionsFunc;
            _parentViewController = parentViewController;
        }

        #region Overrides of UIViewController

        /// <summary>
        /// Initializes the <see cref="P:UIKit.UIViewController.View"/> property.
        /// </summary>
        /// <para>
        /// This method should not be called directly. It is called when the <see cref="P:UIKit.UIViewController.View"/> property is accessed and lazily initialized. Generally, the appropriate <see cref="T:UIKit.UIView"/> will be loaded from a nib file, but application developers may override it to create a custom <see cref="T:UIKit.UIView"/>. This method should not be overridden to provide general initialization on loading a view, that belongs in the <see cref="M:UIKit.UIViewController.ViewDidLoad"/> method.         
        /// </para>
        public override void LoadView()
        {
            base.LoadView();
            PrepareViewToLoad(new DefaultTableViewSource(_constructSectionsFunc, _parentViewController));
        }

        #endregion

        /// <summary>
        /// A Default implementation of EZTableViewSource
        /// </summary>
        /// <seealso cref="UIKit.UITableViewController" />
        class DefaultTableViewSource : EZTableViewSource
        {
            readonly Func<IEnumerable<EZSection>> _constructSectionsFunc;

            public DefaultTableViewSource(Func<IEnumerable<EZSection>> constructSectionsFunc, UITableViewController parentViewController) : base (parentViewController)
            {
                _constructSectionsFunc = constructSectionsFunc;
            }

            /// <summary>
            /// Constructs the sections.
            /// </summary>
            protected override IEnumerable<EZSection> ConstructSections() => _constructSectionsFunc?.Invoke() ?? Enumerable.Empty<EZSection>();
        }
    }
}