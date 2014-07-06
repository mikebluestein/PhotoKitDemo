using MonoTouch.UIKit;
using MonoTouch.Photos;
using MonoTouch.Foundation;
using System.Drawing;
using MonoTouch.CoreFoundation;

namespace PhotoKitDemo
{
    public class PhotosViewController : UICollectionViewController
    {
        static readonly NSString cellId = new NSString ("ImageCell");

        PHFetchResult fetchResults;
        PHImageManager imageMgr;
        PhotoLibraryObserver observer;

        public PhotosViewController (UICollectionViewLayout layout) : base (layout)
        {
            Title = "All Photos";

            imageMgr = new PHImageManager ();
            fetchResults = PHAsset.FetchAssets (PHAssetMediaType.Image, null);
           
            observer = new PhotoLibraryObserver (this);

            PHPhotoLibrary.GetSharedPhotoLibrary.RegisterChangeObserver (observer);
        }

        class PhotoLibraryObserver : PHPhotoLibraryChangeObserver
        {
            readonly PhotosViewController controller;

            public PhotoLibraryObserver (PhotosViewController controller)
            {
                this.controller = controller;
            }

            public override void PhotoLibraryDidChange (PHChange changeInstance)
            {
                DispatchQueue.MainQueue.DispatchAsync (() => {

                    var changes = changeInstance.GetFetchResultChangeDetails (controller.fetchResults);
                    controller.fetchResults = changes.FetchResultAfterChanges;
                    controller.CollectionView.ReloadData ();
                });
            }
        }

        public override void ViewDidLoad ()
        {
            base.ViewDidLoad ();

            CollectionView.RegisterClassForCell (typeof(ImageCell), cellId);
        }

        public override int GetItemsCount (UICollectionView collectionView, int section)
        {
            return (int)fetchResults.Count;
        }

        public override UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
        {
            var imageCell = (ImageCell)collectionView.DequeueReusableCell (cellId, indexPath);

            imageMgr.RequestImageForAsset ((PHAsset)fetchResults [(uint)indexPath.Item], new SizeF (160, 160), 
                PHImageContentMode.AspectFill, new PHImageRequestOptions (), (img, info) => {
                imageCell.ImageView.Image = img;
            });
                    
            return imageCell;
        }

        public override void ItemSelected (UICollectionView collectionView, NSIndexPath indexPath)
        {
            var photoController = new PhotoViewController {
                Asset = (PHAsset)fetchResults [(uint)indexPath.Item]
            };
            NavigationController.PushViewController (photoController, true);
        }
    }

    public class ImageCell : UICollectionViewCell
    {
        public UIImageView ImageView { get; set; }

        [Export ("initWithFrame:")]
        public ImageCell (RectangleF frame) : base (frame)
        {
          
            ImageView = new UIImageView (new RectangleF (0, 0, 100, 100)); 
            ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
            ContentView.AddSubview (ImageView);
        }
    }
}