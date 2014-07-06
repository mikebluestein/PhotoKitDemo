using System;
using MonoTouch.UIKit;
using MonoTouch.Photos;
using MonoTouch.CoreImage;
using MonoTouch.Foundation;

namespace PhotoKitDemo
{
    public class PhotoViewController : UIViewController
    {
        UIImageView imageView;
        UIBarButtonItem filterButton;

        public PHAsset Asset { get; set; }

        public PhotoViewController ()
        {
            imageView = new UIImageView ();

            filterButton = new UIBarButtonItem ("Noir", UIBarButtonItemStyle.Plain, ApplyNoirFilter);

            NavigationItem.RightBarButtonItem = filterButton;
        }

        void ApplyNoirFilter (object sender, EventArgs e)
        {
            Asset.RequestContentEditingInput (new PHContentEditingInputRequestOptions (), (input, options) => {

                // perform the editing operation, which applies a noir filter in this case
                var image = CIImage.FromUrl (input.FullSizeImageUrl);
                image = image.CreateWithOrientation ((CIImageOrientation)input.FullSizeImageOrientation);
                var noir = new CIPhotoEffectNoir {
                    Image = image
                };
                var ciContext = CIContext.FromOptions (null);
                var output = noir.OutputImage;

                var uiImage = UIImage.FromImage (ciContext.CreateCGImage (output, output.Extent));
                imageView.Image = uiImage;

                // save the filtered image data to a PHContentEditingOutput instance
                var editingOutput = new PHContentEditingOutput (input);
                var adjustmentData = new PHAdjustmentData ();
                var data = uiImage.AsJPEG ();
                NSError error;
                data.Save (editingOutput.RenderedContentUrl, false, out error);
                editingOutput.AdjustmentData = adjustmentData;

                // make a change request to publish the changes form the editing output
                PHPhotoLibrary.GetSharedPhotoLibrary.PerformChanges (
                    () => {
                        PHAssetChangeRequest request = PHAssetChangeRequest.ChangeRequest (Asset);
                        request.ContentEditingOutput = editingOutput;
                    },
                    (ok, err) => Console.WriteLine ("photo updated successfully: {0}", ok));
            });
        }

        public override void ViewDidLoad ()
        {
            View.BackgroundColor = UIColor.White;

            imageView = new UIImageView (View.Frame);

            PHImageManager.GetDefaultManager.RequestImageForAsset (Asset, View.Frame.Size, 
                PHImageContentMode.AspectFit, new PHImageRequestOptions (), (img, info) => {
                imageView.ContentMode = UIViewContentMode.ScaleAspectFit;
                imageView.Image = img;
            });

            View.AddSubview (imageView);
        }
    }
}

