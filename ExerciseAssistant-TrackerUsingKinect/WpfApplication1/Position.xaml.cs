using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
using Microsoft.Kinect.Toolkit.Controls;
using Microsoft.Speech.AudioFormat;
using System.Speech.Recognition;
using System.Media;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for Position.xaml
    /// </summary>
    public partial class Position : Page
    {
      

        // Audio if needed
        //SoundPlayer positionAdjust = new SoundPlayer(@"C:\Users\Akku\Documents\NUI\11_20_2013_PositionPageLayoutFix\11_20_2013_PositionPageLayoutFix\WpfApplication1\positionAdjust.wav");
		  SoundPlayer positionAdjust = new SoundPlayer(@"C:\Users\Akku\Documents\AllIntegrated_V2\AllIntegrated\WpfApplication1\positionAdjust.wav");
		
        #region "Kinect"         
        private readonly KinectSensorChooser sensorChooser; 
        #endregion

        public Position()
        {
           this.InitializeComponent();
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
            // Bind the sensor chooser's current sensor to the KinectRegion
            var regionSensorBinding = new Binding("Kinect") { Source = this.sensorChooser };
            BindingOperations.SetBinding(this.kinectRegion, KinectRegion.KinectSensorProperty, regionSensorBinding);
        }

        #region "New Kinect Gesture"

        /// <summary>
        /// Execute shutdown tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        /// 

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            kinectSensorChooser1.KinectSensorChanged += new DependencyPropertyChangedEventHandler(kinectChage);
        }

        private static RecognizerInfo GetKinectRecognizer()
        {
            foreach (RecognizerInfo recognizer in SpeechRecognitionEngine.InstalledRecognizers())
            {
                string value;
                recognizer.AdditionalInfo.TryGetValue("Kinect", out value);
                if ("True".Equals(value, StringComparison.OrdinalIgnoreCase) && "en-US".Equals(recognizer.Culture.Name, StringComparison.OrdinalIgnoreCase))
                {
                    return recognizer;
                }
            }

            return null;
        }


        void kinectChage(object sender, DependencyPropertyChangedEventArgs e)
        {
            //MessageBox.Show("Changing Kinect");
            KinectSensor oldSensor = (KinectSensor)e.OldValue;
            StopKinect(oldSensor);

            KinectSensor newSensor = (KinectSensor)e.NewValue;

            newSensor.DepthStream.Enable();
            newSensor.SkeletonStream.Enable();
            //Headposition(newSensor);

            try
            {
                newSensor.Start();
                newSensor.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(distanceAdjust);

            }

            catch (System.IO.IOException)
            { kinectSensorChooser1.AppConflictOccurred(); }

        }

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

        private void KinectTileButton_Click_3(object sender, RoutedEventArgs e)
        {
            this.sensorChooser.KinectChanged -= SensorChooserOnKinectChanged;
            (Application.Current.MainWindow.FindName("_mainFrame") as Frame).Source = new Uri("MainMenu.xaml", UriKind.Relative);

        }

        void StopKinect(KinectSensor sensor)
        {
            if (sensor != null && sensor.IsRunning)
            {
                //sensor.Stop();
                // this.Close();
                //sensor.AudioSource.Stop();
            }
        }
        
        void distanceAdjust(object sender, SkeletonFrameReadyEventArgs e)
        {
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
                if (skeletonFrame != null)
                {

                    Skeleton[] skeletonData = new Skeleton[skeletonFrame.SkeletonArrayLength];

                    skeletonFrame.CopySkeletonDataTo(skeletonData);
                    Skeleton playerSkeleton = (from s in skeletonData where s.TrackingState == SkeletonTrackingState.Tracked select s).FirstOrDefault();
                    if (playerSkeleton != null)
                    {

                        Joint head = playerSkeleton.Joints[JointType.Head];
                        Joint leftFoot = playerSkeleton.Joints[JointType.FootLeft];
                        Joint rightFoot = playerSkeleton.Joints[JointType.FootRight];


                        if ((head.TrackingState == JointTrackingState.Tracked) && (leftFoot.TrackingState == JointTrackingState.Tracked) && (rightFoot.TrackingState == JointTrackingState.Tracked))
                        {

                            if (head.Position.Z >= 2.5 && leftFoot.Position.Z >= 2.5 && rightFoot.Position.Z >= 2.5)
                            {
                               // this.positionSuggestion.Content = "Bingo! Correct position";
                                //this.positionSuggestion.Content = head.Position.Z + " " + leftFoot.Position.Z + " " + rightFoot.Position.Z;
                                this.positionSuggestion.Content = "Bingo! Correct position";

                                myMediaElement.Stop();

                            }
                        }
                        else
                        {
                            this.positionSuggestion.Content = "Please come in range";
                            myMediaElement.Play();

                        }
                    }
                }
        }
    }
}
