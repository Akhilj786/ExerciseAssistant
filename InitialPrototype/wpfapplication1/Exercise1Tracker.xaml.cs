using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
//using System.Windows.Threading.DispatcherTimer;
using Microsoft.Kinect;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Exercise1Tracker : Window
    {
        public int sets;

        float startLeftWristYPosition = 0;
        float startLeftWristXPosition = 0;
        float startRightWristYPosition = 0;
        float startRightWristXPosition = 0;
        float startLeftElbowYPosition = 0;
        float startLeftElbowXPosition = 0;
        float startRightElbowYPosition = 0;
        float startRightElbowXPosition = 0;
        //float endLeftWristXPosition = 0;
        //float endLeftWristYPosition = 0;

        //float endRightWristXPosition = 0;
        //float endRightWristYPosition = 0;
        int step = 0;
        int startTime = 0;
        int totalTimeElapsed = 0;
        int count = 0;

      //  Boolean isExerciseCompleted = false;

        public Exercise1Tracker()
        {
            this.WindowState = System.Windows.WindowState.Maximized;
            this.WindowStyle = System.Windows.WindowStyle.None;
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.exProgress.Maximum = sets;
            // MessageBox.Show("Window Loading");
            kinectSensorChooser1.KinectSensorChanged += new DependencyPropertyChangedEventHandler(kinectSensorChooser1_KinectSensorChanged);
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
            if (sensor != null)
            {
                sensor.Stop();
                //sensor.AudioSource.Stop();
            }
        }

        void Exercise1(object sender, SkeletonFrameReadyEventArgs e)
        {

            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
                if (skeletonFrame != null)
                {


                    Skeleton[] skeletonData = new Skeleton[skeletonFrame.SkeletonArrayLength];

                    skeletonFrame.CopySkeletonDataTo(skeletonData);
                    Skeleton playerSkeleton = (from s in skeletonData where s.TrackingState == SkeletonTrackingState.Tracked select s).FirstOrDefault();
                    if (playerSkeleton != null)
                    {

                        Joint leftshoulder = playerSkeleton.Joints[JointType.ShoulderLeft];
                        Joint rightshoulder = playerSkeleton.Joints[JointType.ShoulderRight];
                        Joint leftwrist = playerSkeleton.Joints[JointType.WristLeft];
                        Joint rightwrist = playerSkeleton.Joints[JointType.WristRight];
                        Joint head = playerSkeleton.Joints[JointType.Head];

                        startLeftWristYPosition = leftwrist.Position.Y;
                        startRightWristYPosition = rightwrist.Position.Y;


                        if ((leftshoulder.Position.Y + 0.08 >= startLeftWristYPosition && leftshoulder.Position.Y - 0.08 <= startLeftWristYPosition) && (rightshoulder.Position.Y + 0.08 >= startRightWristYPosition && rightshoulder.Position.Y - 0.08 <= startRightWristYPosition))
                        {


                            // startLeftWristXPosition = leftwrist.Position.X;
                            // endLeftWristXPosition = 0;
                            // endLeftWristYPosition = 0;
                            // startRightWristXPosition = rightwrist.Position.X;
                            // endRightWristXPosition = 0;
                            //endRightWristYPosition = 0;

                            if (startTime == 0)
                            {
                                startTime = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
                                Console.WriteLine("Condition meet at:" + startTime);
                            }
                            else
                            {
                                int currentTime = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
                                totalTimeElapsed = (currentTime - startTime);

                                Console.WriteLine("time elapsed: " + totalTimeElapsed);
                                if (totalTimeElapsed >= 2.0)
                                //if (totalTimeElapsed == 300)
                                {
                                    totalTimeElapsed = 0;
                                    startTime = 0;
                                    Console.WriteLine("-------Step1 Done------\n\n");
                                    this.Step1_corr.Visibility = Visibility.Collapsed;
                                    //Uri step2_img = new Uri("C:/Users/Akku/Dropbox/NUI/step2.jpg", UriKind.Absolute);
                                    //ImageSource img_src = new BitmapImage(step2_img);
                                    //this.Step2_corr.Source = img_src;
                                    this.errorLabel.Content = "Correct";
                                    this.Step2_corr.Visibility = Visibility.Visible;
                                    count++;
                                    step = 1;
                                }
                            }
                        }
                        else
                        {
                            startTime = 0;
                            totalTimeElapsed = 0;
                            //Uri step1_corr = new Uri("C:/Users/Akku/Dropbox/NUI/step1.jpg", UriKind.Absolute);
                            //ImageSource img_src = new BitmapImage(step1_corr);
                            //this.Step1_corr.Source = img_src;
                            this.Step1_corr.Visibility = Visibility.Visible;
                            if ((leftshoulder.Position.Y <= startLeftWristYPosition && rightshoulder.Position.Y <= startRightWristYPosition))
                            {
                                this.errorLabel.Content = "Correction: Lower Hand";
                                this.errorLabel.Visibility = Visibility.Visible;
                            }
                            else if ((leftshoulder.Position.Y >= startLeftWristYPosition && rightshoulder.Position.Y >= startRightWristYPosition))
                            {
                                this.errorLabel.Content = "Correction: Move hand UP";
                                this.errorLabel.Visibility = Visibility.Visible;
                            }
                        }
                    }

                }
        }
        void Exercise2(object sender, SkeletonFrameReadyEventArgs e)
        {
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
                if (skeletonFrame != null)
                {
                    // int skeletonSlot = 0;
                    //MessageBox.Show("e");
                    Console.WriteLine("Inside Step2");

                    Skeleton[] skeletonData = new Skeleton[skeletonFrame.SkeletonArrayLength];

                    skeletonFrame.CopySkeletonDataTo(skeletonData);
                    Skeleton playerSkeleton = (from s in skeletonData where s.TrackingState == SkeletonTrackingState.Tracked select s).FirstOrDefault();
                    if (playerSkeleton != null)
                    {

                        Joint leftshoulder = playerSkeleton.Joints[JointType.ShoulderLeft];
                        Joint rightshoulder = playerSkeleton.Joints[JointType.ShoulderRight];
                        Joint leftelbow = playerSkeleton.Joints[JointType.ElbowLeft];
                        Joint rightelbow = playerSkeleton.Joints[JointType.ElbowRight];
                        Joint leftwrist = playerSkeleton.Joints[JointType.WristLeft];
                        Joint rightwrist = playerSkeleton.Joints[JointType.WristRight];
                        Joint head = playerSkeleton.Joints[JointType.Head];

                        startLeftWristYPosition = leftwrist.Position.Y;
                        startRightWristYPosition = rightwrist.Position.Y;
                        startLeftWristXPosition = leftwrist.Position.X;
                        startRightWristXPosition = rightwrist.Position.X;

                        startLeftElbowYPosition = leftelbow.Position.Y;
                        startRightElbowYPosition = rightelbow.Position.Y;
                        startLeftElbowXPosition = leftelbow.Position.X;
                        startRightElbowXPosition = rightelbow.Position.X;

                        // if ((leftshoulder.Position.Y + 0.08 >= startLeftWristYPosition && leftshoulder.Position.Y - 0.08 <= startLeftWristYPosition) && (rightshoulder.Position.Y + 0.08 >= startRightWristYPosition && rightshoulder.Position.Y - 0.08 <= startRightWristYPosition))

                        if ((startLeftWristYPosition >= head.Position.Y && startRightWristYPosition >= head.Position.Y) &&
                          (startLeftElbowYPosition >= leftshoulder.Position.Y && startRightElbowYPosition >= rightshoulder.Position.Y)
                           &&
                           (startLeftElbowXPosition + 0.08 >= leftshoulder.Position.X && startLeftElbowXPosition - 0.08 <= leftshoulder.Position.X) &&
                           (startRightElbowXPosition + 0.08 >= rightshoulder.Position.X && startRightElbowXPosition - 0.08 <= rightshoulder.Position.X) &&
                           (startLeftWristXPosition + 0.08 >= startLeftElbowXPosition && startLeftWristXPosition - 0.08 <= startLeftElbowXPosition) &&
                            (startRightWristXPosition + 0.08 >= startRightElbowXPosition && startRightWristXPosition - 0.08 <= startRightElbowXPosition))
                        {
                            Console.WriteLine("condition meet2");


                            if (startTime == 0)
                            {
                                startTime = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
                                Console.WriteLine("Condition meet2 at:" + startTime);
                            }
                            else
                            {
                                int currentTime = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
                                totalTimeElapsed = (currentTime - startTime);

                                // MessageBox.Show("start time: " + startTime + " current time: " + currentTime + "diff time: " + totalTimeElapsed);

                                //MessageBox.Show("totalTimeElapsed : " + totalTimeElapsed);
                                Console.WriteLine("time elapsed: " + totalTimeElapsed);
                                if (totalTimeElapsed >= 2.0)
                                //if (totalTimeElapsed == 300)
                                {
                                    totalTimeElapsed = 0;
                                    startTime = 0;
                                    count++;
                                    this.countText.Content = "" + count / 2;
                                    this.exProgress.Value = count / 2;
                                    
                                    //Console.WriteLine("Exercise2 Done startLeftWristXPosition: " + startLeftWristXPosition + " endLeftWristXPosition" + endLeftWristXPosition + " endLeftWristYPosition: " + endLeftWristYPosition + " startLeftWristYPosition:" + startLeftWristYPosition);
                                    Console.WriteLine("-------Step2 Done------\n\n");
                                    if (count/2 == sets)
                                    {
                                        //isExerciseCompleted = true;                                  

                                        this.exProgress.Value = count / 2;
                                        almostDoneLabel.Content = "Done";
                                        kinectSkeletonViewer1.Visibility = Visibility.Hidden;
                                       // StopKinect(kinectSensorChooser1.Kinect);
                                        Step1_corr.Visibility = Visibility.Hidden;
                                        Step2_corr.Visibility = Visibility.Hidden;
                                        errorLabel.Visibility = Visibility.Hidden;
                                        doneLabel1.Visibility = Visibility.Visible;
                                        doneLabel2.Visibility = Visibility.Visible;

//                                        new startwindow(isExerciseCompleted);
                                        new startwindow();

                                        //MessageBox.Show("Exercise1 count 4 reached ");
                                        // Environment.Exit(System);
                                      //  Close();
                                    }
                                    else
                                    {
                                        this.Step2_corr.Visibility = Visibility.Collapsed;
                                        this.errorLabel.Content = "Correct Step2";
                                        this.Step1_corr.Visibility = Visibility.Visible;
                                        if (count / 2 == sets - 1)
                                            this.almostDoneLabel.Visibility = Visibility.Visible;
                                            this.almostDoneLabel.Content = "Almost Done";
                                        step = 0;

                                    }
                                }
                            }
                        }
                        else
                        {
                            startTime = 0;
                            totalTimeElapsed = 0;
                            if ((startLeftWristYPosition < head.Position.Y && startRightWristYPosition < head.Position.Y))
                                this.errorLabel.Content = "Correction: Move Hand above head";
                            else if ((startLeftWristXPosition > startLeftElbowXPosition) && (startRightWristXPosition > startRightElbowXPosition))
                                this.errorLabel.Content = "Correction: Align hand properly with Elbow";
                            else
                                this.errorLabel.Content = "";
                            this.Step2_corr.Visibility = Visibility.Visible;
                        }
                    }

                }
            // MessageBox.Show("Exercise1 Step2 done");
        }

        void Headposition(object sender, SkeletonFrameReadyEventArgs e)
        {
            switch (step)
            {
                case 0:
                    Exercise1(sender, e);
                    break;
                case 1:
                    Exercise2(sender, e);
                    break;
            }
        }


    }
    }

