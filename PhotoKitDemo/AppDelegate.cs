using System;
using System.Collections.Generic;
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Drawing;

namespace PhotoKitDemo
{
    [Register ("AppDelegate")]
    public partial class AppDelegate : UIApplicationDelegate
    {
        UIWindow window;
        PhotosViewController photosController;
        UICollectionViewFlowLayout layout;
        UINavigationController navController;

        public override bool FinishedLaunching (UIApplication app, NSDictionary options)
        {
            window = new UIWindow (UIScreen.MainScreen.Bounds);
  
            layout = new UICollectionViewFlowLayout {
                ItemSize = new SizeF(100,100)
            };

            photosController = new PhotosViewController (layout);

            navController = new UINavigationController (photosController);

            window.RootViewController = navController;

            window.MakeKeyAndVisible ();
			
            return true;
        }
    }
}

