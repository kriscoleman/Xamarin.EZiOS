# Xamarin.EZiOS
A Simplified UI-API for Xamarin.iOS - Designed to speed up development and maintanence

# Easy to use, makes your code base easier to maintain and quicker to develop. 
Designed to reduce boilerplate, and provide a degree of default implementation. Architecture is simple, yet robust enough to not limit your options in the Xamarin.iOS framework. Allows you to create smaller ViewControllers, so that your code has a smaller foot-print and you can focus only on the highest-level view logic. 

# OOP and Declarative Natures
The core of EZiOS is designed around an OOP infrastructure, but through a functional API helper, you can use the API in a declartive nature. You can even mix the two!

Consider the following examples, which are functionally the same: 
- Pure OOP 
```javascript
  class MyShoppingCartRow : EZRow<Item>
  {
  
    public MyShoppingCartRow(Item item) : base (item, _ => item.ItemName, _ => $"Price: {item.SalePrice}")
    {
      this.Image = item.Icon;
    }
  
  }
  
  # later in your TableViewSource: 
  
  protected override IEnumerable<EZSection<T>> ConstructSections() =>
    yield return _shoppingCart.ItemLines.Select(line => new MyShoppingCartRow(line.Item)); 
  // all that is needed to present your EZRows!
```
- Declarative (Functional)
```javascript
  # in your TableViewSource: 
  
  protected override IEnumerable<EZSection<T>> ConstructSections() =>
    yield return _shoppingCart.ItemLines.Select(
      line => new EZRow(line.Item, _ => line.Item.ItemName, _ => $"Price: {line.Item.SalePrice}).WithImage(line.Item.Icon)); 
  // all that is needed to present your EZRows!
```

- Hybrids are supported!
```javascript
  class MyShoppingCartRow : EZRow<Item>
  {
  
    public MyShoppingCartRow(Item item) : base (item, _ => item.ItemName, _ => $"Price: {item.SalePrice}")
    {  }
  
  }
  
  # later in your TableViewSource: 
  
  protected override IEnumerable<EZSection<T>> ConstructSections() 
  {
       var itemRows = _shoppingCart.ItemLines.Select(line => new MyShoppingCartRow(line.Item));
       foreach (var row in itemRows)
       {
         // suppose we have a complex repo of images we need to maintain
          yield return row.WithImage(row.Item.Icon == null ? ItemImageCache.GetImageForItem(row.Item.UPC) : row.Item.Icon));
       }
  }
  // all that is needed to present your EZRows!
```

Use it however it fits your style or needs!


# Gettin' Funky - Volatile Data Binding
Because the iOS guidelines suggest an importance on keeping data observable and in real-time, EZiOS provides a lot of funcs for UI properties, so that you can set things like cell titles and subtitles, images, accessories and edit actions through funcs, and update the view in real time as your data changes. A cell can update this way by simply calling cocoa's framework TableView.ReloadData or reloading individual TableView.Rows or TableView.Sections, without the need to interface again with the EZ api and reconstructing all of your underlying sections. If you don't need to update the properties (for static views, like context menus), don't worry, the regular properties are still there, too.

# Easy Table View Controllers
An easy way to abstract out the behavior/structure of a UITableView is to imagine it as a two-dimensional array. EXiOS does this literally, implementing core TableView behavior using a simple 2D array model: List<EZSection<IEZRow<T>>. It's a list of sections, which contain rows. These provide sections and cells in your TableView. 

Tired of the complex TableViewControllers and their cumbersome TableViewSources? Eliminate the boiler-plate and simply define the sections and rows of your table view. That's all you need to do to get a table view working out of the box, all core overrides and behaviors have been implemented for you so you can sit back and fire it up! But customization is key, so you can override any behavior at any time. If your views still grow large, look for common patterns and abstract out further derivatives to default more behaviors. EZiOS helps you design so that only the important logic sits on top, and all of the common logic can hide below.

# Heavily customizable - Spin up your own! 
Designed so you can eliminate code but still have full control! Use the already implemented EZ classes or implement your own derivatives from base implementations all the way down to interfaces. 

#Coming Soon 
- Examples
- Example Project
- Navigation, Modal, Popover, and Alert helpers
- Automatic Compile-Time binding of code-behind ViewControllers to accompyaning Storyboard files. (Storyboard parsing)
- Opt-in Universal iPhone-iPad layout
- Library of General-purpose User Controls and Views 
- App-wide Friendly-Crash UX Logic
