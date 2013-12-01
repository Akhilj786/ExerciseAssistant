using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
using Microsoft.Kinect.Toolkit.Controls;
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

namespace WpfApplication1
{
    public partial class MainMenu : Page
    {
        #region "Kinect"
          

        private readonly KinectSensorChooser sensorChooser;
 

        #endregion
        public MainMenu()
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
        /// <summary>
       

        #region "New Kinect Gesture"
          
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

        private void KinectTileButton_Click_1(object sender, RoutedEventArgs e)
        {
            this.sensorChooser.KinectChanged -= SensorChooserOnKinectChanged;
            (Application.Current.MainWindow.FindName("_mainFrame") as Frame).Source = new Uri("Game1.xaml", UriKind.Relative);
        }

        private void KinectTileButton_Click_2(object sender, RoutedEventArgs e)
        {
            this.sensorChooser.KinectChanged -= SensorChooserOnKinectChanged;
            (Application.Current.MainWindow.FindName("_mainFrame") as Frame).Source = new Uri("Game2.xaml", UriKind.Relative);
        }

        private void KinectTileButton_Click_3(object sender, RoutedEventArgs e)
        {
           //his.sensorChooser.KinectChanged -= SensorChooserOnKinectChanged;
            //(Application.Current.MainWindow.FindName("_mainFrame") as Frame).Source = new Uri("videopage.xaml", UriKind.Relative);
           // UnregisterEvents();
            App.Current.Shutdown();
        }
        private void KinectTileButton_Click_4(object sender, RoutedEventArgs e)
        {
            this.sensorChooser.KinectChanged -= SensorChooserOnKinectChanged;
            (Application.Current.MainWindow.FindName("_mainFrame") as Frame).Source = new Uri("Position.xaml", UriKind.Relative);
        }

  
    }
}
