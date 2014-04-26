using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Drawing;

namespace KeyboardObservers
{
    class ViewController : UIViewController
	{
        // Text view to display.
        UITextView _textView = null;

        // Some objects to hold our observers.
        private NSObject _keyboardWillShow = null;
        private NSObject _keyboardWillHide = null;

        public ViewController()
        {
            // Done button in the top right of the nav bar to dismiss the keyboard.
            NavigationItem.RightBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Done, delegate
            {
                _textView.ResignFirstResponder();
            });
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Create the text 
            _textView = new UITextView(new RectangleF(0, 0, View.Frame.Width, View.Frame.Height));

            // Prefill it with data.
            for (int i = 1; i <= 100; ++i)
            {
                _textView.Text += "Hello World #" + i + ((i == 100) ? "" : "\n");
            }
            // Set the auto resizing mask to be flexible width and height as the View.Frame size will change 
            // as it is loaded into the UINavigationController
            _textView.AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth;
            View.AddSubview(_textView);

            // Just some things to tidy up the nav bar on iOS7.
            if (UIDevice.CurrentDevice.CheckSystemVersion(7, 0))
            {
                EdgesForExtendedLayout = UIRectEdge.None;
            }
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            // Create the two observers registered to the two events that fire two methods.
            // UIKeyboard.WillShowNotification -> KeyboardWillShow(NSNotification notification)
            // UIKeyboard.WillHideNotification -> KeyboardWillHide(NSNotification notification)
            _keyboardWillShow = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillShowNotification, KeyboardWillShow);
            _keyboardWillHide = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification, KeyboardWillHide);
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            // Important to stop the observers when we leave this page.
            // Not sure if you should RemoveObserver or Dispose.
            // I have read that disposing an observer also removes it from the default center.
            if (_keyboardWillShow != null)
            {
                NSNotificationCenter.DefaultCenter.RemoveObserver(_keyboardWillShow);
                //_keyboardWillShow.Dispose();
            }

            if (_keyboardWillHide != null)
            {
                NSNotificationCenter.DefaultCenter.RemoveObserver(_keyboardWillHide);
                //_keyboardWillHide.Dispose();
            }
        }

        private void KeyboardWillShow(NSNotification notification)
        {
            // Get the frame of the keyboard at the end of the animation.
            RectangleF frameEnd = UIKeyboard.FrameEndFromNotification(notification);

            // Get the duration of the keyboard animation.
            double duration = UIKeyboard.AnimationDurationFromNotification(notification);

            // Get the animation curve (bounce etc);
            uint curve = UIKeyboard.AnimationCurveFromNotification(notification);
           
            // Because we are using a UITextView (UIScrollView) we are just going
            // to alter the insets. As seen below you can also alter the frame or anything else if you wish.
            UIEdgeInsets insets = _textView.ContentInset;
            insets.Bottom += frameEnd.Height;
           
            insets = _textView.ScrollIndicatorInsets;
            insets.Bottom += frameEnd.Height;

            // Old method, using a frame.
            // RectangleF newFrame = new RectangleF(_textView.Frame.Left, _textView.Frame.Top, _textView.Frame.Width, _textView.Frame.Height - frameEnd.Height);
            // Create an animatin that goes for `duration` time.
            UIView.Animate(duration, delegate
            { 
                // Set the animation curve.
                UIView.SetAnimationCurve((UIViewAnimationCurve)curve);

                // Set the new attributes to take place in the animation.
                _textView.ContentInset = insets;
                _textView.ScrollIndicatorInsets = insets;

                // Old method, using a frame.
                //_textView.Frame = newFrame;

                // Animation just goes.
            });
        }

        private void KeyboardWillHide(NSNotification notification)
        {
            // See comments in KeyboardWillShow.
            RectangleF frameEnd = UIKeyboard.FrameEndFromNotification(notification);
            double duration = UIKeyboard.AnimationDurationFromNotification(notification);
            uint curve = UIKeyboard.AnimationCurveFromNotification(notification);

        
            UIEdgeInsets insets = _textView.ContentInset;
            insets.Bottom -= frameEnd.Height;

            insets = _textView.ScrollIndicatorInsets;
            insets.Bottom -= frameEnd.Height;


            //Old method, using a frame.
            //RectangleF newFrame = new RectangleF(_textView.Frame.Left, _textView.Frame.Top, _textView.Frame.Width, View.Frame.Height);
            UIView.Animate(duration, delegate
            {
                UIView.SetAnimationCurve((UIViewAnimationCurve)curve);

                _textView.ContentInset = insets;
                _textView.ScrollIndicatorInsets = insets;

                //Old method, using a frame.
                //_textView.Frame = newFrame;
            });
        }
	}
}