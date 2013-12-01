using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
using Microsoft.Kinect.Toolkit.Controls;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
using Microsoft.Kinect.Toolkit.Controls;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Media;
namespace WpfApplication1
{
    public partial class Game2 : Page
    {
        
        #region "Kinect"  
        private readonly KinectSensorChooser sensorChooser;

        public String exerciseName = "Squats";
        public int setvalue;

        private KinectSensor _Kinect;
        private WriteableBitmap _ColorImageBitmap;
        private Int32Rect _ColorImageBitmapRect;
        private int _ColorImageStride;
        private Skeleton[] FrameSkeletons;

     
        static Button selected;

        float handX;
        float handY;
        
        #endregion
        //SoundPlayer beginAudio = new SoundPlayer(@"C:\Users\Akku\Documents\NUI\11-19-2013-exercise1andpush\AllIntegrated\WpfApplication1\audioBegin.wav");
        SoundPlayer beginAudio = new SoundPlayer(@"C:\Users\Akku\Documents\AllIntegrated_V2\AllIntegrated\WpfApplication1\audioBegin.wav");
		
		public Game2()
        {
            this.InitializeComponent();  
            // initialize the sensor chooser and UI


            
            if (Generics.GlobalKinectSensorChooser == null)
            {
                // initialize the sensor chooser and UI
                this.sensorChooser = new KinectSensorChooser();
                this.sensorChooser.KinectChanged += SensorChooserOnKinectChanged;
                this.sensorChooserUi.KinectSensorChooser = this.sensorChooser;
                this.sensorChooser.Start();
                Generics.GlobalKinectSensorChooser = this.sensorChooser;
            }
            else
            {   // initialize the sensor chooser and UI 
                this.sensorChooser = new KinectSensorChooser();
                this.sensorChooser = Generics.GlobalKinectSensorChooser;
                this.sensorChooser.KinectChanged += SensorChooserOnKinectChanged;
                this.sensorChooserUi.KinectSensorChooser = sensorChooser;
            }
            var regionSensorBinding = new Binding("Kinect") { Source = this.sensorChooser };
             BindingOperations.SetBinding(this.kinectRegion, KinectRegion.KinectSensorProperty, regionSensorBinding);
        }
        private void MediaTimeline_Completed(object sender, EventArgs e)
        {
            this.VID_20131001_193436_mp4.Visibility = Visibility.Collapsed;
            this.VID_20131001_193436_mp4.Visibility = Visibility.Hidden;
            this.setof2.Visibility = Visibility.Visible;
            this.setof10.Visibility = Visibility.Visible;
            this.setof5.Visibility = Visibility.Visible;
            this.home.Visibility = Visibility.Visible;

          
        }
        
        /// <summary>
        
        #region "New Kinect Gesture"
        
        /// <summary>
        /// Execute shutdown tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.sensorChooser.Stop();
        }
        
        /// <summary>
        /// Called when the KinectSensorChooser gets a new sensor
        /// </summary>
        /// <param name="sender">sender of the event</param>
        /// <param name="args">event arguments</param>
        private static void SensorChooserOnKinectChanged(object sender, KinectChangedEventArgs args)
        {
            if (args.OldSensor != null)
            {
                try
                {
                    args.OldSensor.DepthStream.Range = DepthRange.Near;
                    args.OldSensor.SkeletonStream.EnableTrackingInNearRange = false;
                    args.OldSensor.DepthStream.Disable();
                    args.OldSensor.SkeletonStream.Disable();
                }
                catch (InvalidOperationException)
                {
                    // KinectSensor might enter an invalid state while enabling/disabling streams or stream features.
                    // E.g.: sensor might be abruptly unplugged.
                }
            }
                
            if (args.NewSensor != null)
            {
                try
                {
                    args.NewSensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
                    args.NewSensor.SkeletonStream.Enable();
                        
                    try
                    {
                        args.NewSensor.DepthStream.Range = DepthRange.Default;
                        args.NewSensor.SkeletonStream.EnableTrackingInNearRange = true;
                    }
                    catch (InvalidOperationException)
                    {
                        // Non Kinect for Windows devices do not support Near mode, so reset back to default mode.
                        args.NewSensor.DepthStream.Range = DepthRange.Default;
                        args.NewSensor.SkeletonStream.EnableTrackingInNearRange = false;
                    }
                }
                catch (InvalidOperationException)
                {
                    // KinectSensor might enter an invalid state while enabling/disabling streams or stream features.
                    // E.g.: sensor might be abruptly unplugged.
                }
            }
        }
        
        #endregion
            
       /* private void KinectTileButton_Click_3(object sender, RoutedEventArgs e)
        {
            //Unsubscribe to the sensorchooser's  event SensorChooseronkinectChanged
            this.sensorChooser.KinectChanged -= SensorChooserOnKinectChanged;
            (Application.Current.MainWindow.FindName("_mainFrame") as Frame).Source = new Uri("MainMenu.xaml", UriKind.Relative); 
        }*/
        private void BACKHOME_Click(object sender, RoutedEventArgs e)
        {
           // UnregisterEvents();
            this.sensorChooser.KinectChanged -= SensorChooserOnKinectChanged;
            (Application.Current.MainWindow.FindName("_mainFrame") as Frame).Source = new Uri("MainMenu.xaml", UriKind.Relative);
        }
        private void SetOf5_Click(object sender, RoutedEventArgs e)
        {

            beginAudio.Play();
          //  this.StretchingExerciseVideo_mp4.Visibility = Visibility.Collapsed;
           this.home.Visibility = Visibility.Collapsed;
           this.setof2.Visibility = Visibility.Collapsed;
           this.setof10.Visibility = Visibility.Collapsed;
           this.start.Visibility = Visibility.Visible;
           this.setof5.Visibility = Visibility.Collapsed;
           this.startlabel.Visibility = Visibility.Visible;
            //this.countRectangle.Visibility = Visibility.Collapsed;
            //this.countRectangleLabel.Visibility = Visibility.Collapsed;

            this.setvalue = 5;

          var startwindow = new startwindow(exerciseName);
            startwindow.set_val = setvalue;
            startwindow.Show();

        }
        private void SetOf10_Click(object sender, RoutedEventArgs e)
        {
            beginAudio.Play();
            //UnregisterEvents();
            //this.StretchingExerciseVideo_mp4.Visibility = Visibility.Collapsed;
        //    this.SkipDemo.Visibility = Visibility.Collapsed;

            this.home.Visibility = Visibility.Collapsed;
            this.setof2.Visibility = Visibility.Collapsed;
            this.setof10.Visibility = Visibility.Collapsed;
            this.start.Visibility = Visibility.Visible;
            this.setof5.Visibility = Visibility.Collapsed;
            this.startlabel.Visibility = Visibility.Visible;
            //this.startText.Visibility = Visibility.Visible;
            //this.countRectangle.Visibility = Visibility.Collapsed;
            //this.countRectangleLabel.Visibility = Visibility.Collapsed;

            this.setvalue = 10;

            var startwindow = new startwindow(exerciseName);
         startwindow.set_val = setvalue;
           startwindow.Show();

        }
        private void SetOf2_Click(object sender, RoutedEventArgs e)
        {
            beginAudio.Play();
           // UnregisterEvents();
            //this.StretchingExerciseVideo_mp4.Visibility = Visibility.Collapsed;
            this.home.Visibility = Visibility.Collapsed;
            this.setof2.Visibility = Visibility.Collapsed;
            this.setof10.Visibility = Visibility.Collapsed;
            this.start.Visibility = Visibility.Visible;
            this.setof5.Visibility = Visibility.Collapsed;
            this.startlabel.Visibility = Visibility.Visible;
            //this.startText.Visibility = Visibility.Visible;
            //this.SkipDemo.Visibility = Visibility.Collapsed;
            //this.SetOf5.Visibility = Visibility.Collapsed;
            //this.SetOf10.Visibility = Visibility.Collapsed;
            //this.countRectangle.Visibility = Visibility.Collapsed;
            //this.countRectangleLabel.Visibility = Visibility.Collapsed;

            this.setvalue = 2;
            var startwindow = new startwindow(exerciseName);
            startwindow.set_val = setvalue;
            startwindow.Show();
        }
     
    }
}




