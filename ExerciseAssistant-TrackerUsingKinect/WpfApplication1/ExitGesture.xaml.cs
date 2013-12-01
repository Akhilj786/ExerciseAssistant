
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
using System.Windows.Shapes;
using System.Media;
using System.Windows.Navigation;

//using System.Windows.Threading.DispatcherTimer;
using Microsoft.Kinect;
using Microsoft.Speech.AudioFormat;
using System.Speech.Recognition;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for ExitGesture.xaml
    /// </summary>
    public partial class ExitGesture : Window
    {

            //Wrist Position
        float startLeftWristYPosition = 0;
        float startLeftWristXPosition = 0;
        float startRightWristYPosition = 0;
        float startRightWristXPosition = 0;

        //Shoulder Position


        int step = 0;
        int startTime = 0;
        int totalTimeElapsed = 0;
        int count = 0;
        //WpfApplication1.globalAssign globalvar = new globalAssign();

        SoundPlayer exitGesture = new SoundPlayer(@"C:\Users\Akku\Documents\AllIntegrated_V2\AllIntegrated\WpfApplication1\exitGesture.wav");
        
        public ExitGesture()
        {
            InitializeComponent();
            this.WindowState = System.Windows.WindowState.Maximized;
            this.WindowStyle = System.Windows.WindowStyle.None;
            
            /*globalvar.alignHand.Stop();
            globalvar.handHead.Stop();
            globalvar.handUp.Stop();
            globalvar.lowerHand.Stop();
            globalvar.timer.Stop();*/
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            kinectSensorChooser1.KinectSensorChanged += new DependencyPropertyChangedEventHandler(kinectSensorChooser1_KinectSensorChanged);
            try
            {
                exitGesture.Play();
            }
            catch (Exception e1) {
                MessageBox.Show("Hello Inside Window");
            }

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


        void kinectSensorChooser1_KinectSensorChanged(object sender, DependencyPropertyChangedEventArgs e)
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


                newSensor.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(Headposition);

            }

            catch (System.IO.IOException)
            { kinectSensorChooser1.AppConflictOccurred(); }

        }



        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            StopKinect(kinectSensorChooser1.Kinect);
            //MessageBox.Show("Closing");
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

        void Headposition(object sender, SkeletonFrameReadyEventArgs e)
        {
            switch (step)
            {
                case 0:
                    exitSensor(sender, e);
                    break;

            }
        }

        void exitSensor(object sender, SkeletonFrameReadyEventArgs e)
        {
            NavigationService navService = NavigationService.GetNavigationService(this);
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
                if (skeletonFrame != null)
                {

                    Skeleton[] skeletonData = new Skeleton[skeletonFrame.SkeletonArrayLength];

                    skeletonFrame.CopySkeletonDataTo(skeletonData);
                    Skeleton playerSkeleton = (from s in skeletonData where s.TrackingState == SkeletonTrackingState.Tracked select s).FirstOrDefault();
                    if (playerSkeleton != null)
                    {

                        // Joint leftshoulder = playerSkeleton.Joints[JointType.ShoulderLeft];
                        // Joint rightshoulder = playerSkeleton.Joints[JointType.ShoulderRight];
                        // Joint leftelbow = playerSkeleton.Joints[JointType.ElbowLeft];
                        // Joint rightelbow = playerSkeleton.Joints[JointType.ElbowRight];
                        Joint leftwrist = playerSkeleton.Joints[JointType.WristLeft];
                        Joint rightwrist = playerSkeleton.Joints[JointType.WristRight];


                        startLeftWristXPosition = leftwrist.Position.X;
                        startLeftWristYPosition = leftwrist.Position.Y;
                        startRightWristXPosition = rightwrist.Position.X;
                        startRightWristYPosition = rightwrist.Position.Y;




                        // if (((lefthip.Position.Y + 0.15 >= startLeftKneeYPosition && lefthip.Position.Y - 0.15 <= startLeftKneeYPosition) &&
                        //   (righthip.Position.Y + 0.15 >= startRightKneeYPosition && righthip.Position.Y - 0.15 <= startRightKneeYPosition)))

                        if (leftwrist.Position.X > rightwrist.Position.X)
                        {
                            if (startTime == 0)
                            {
                                startTime = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
                            }
                            else
                            {
                                int currentTime = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
                                totalTimeElapsed = (currentTime - startTime);
                                if (totalTimeElapsed >= 1.0)
                                {
                                    totalTimeElapsed = 0;
                                    startTime = 0;
                                    count++;
                                    Console.WriteLine("-------Step3 Done------\n\n");
                                    //MessageBox.Show("Exit");
                                    //StopKinect(kinectSensorChooser1.Kinect);
                                    //step = 0;
                                    //Close();
                                    //MainFrame.Navigate(new Uri("MainMenu.xaml", UriKind.Relative));
                                    
                                   // MainMenu mainmenupage = new MainMenu();
                                   // navService.Navigate=(new System.Uri("MainMenu.xaml",UriKind.Relative);
                                   // this.Close();
                                    App.Current.Shutdown();
                                }

                            }
                        }
                        else
                        {
                            startTime = 0;
                            totalTimeElapsed = 0;

                            //this.Step2_corr.Visibility = Visibility.Visible;
                        }
                    }

                }
        }


    }
}

