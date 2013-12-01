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
//using System.Windows.Threading.DispatcherTimer;
using Microsoft.Kinect;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for Exercise2Tracker.xaml
    /// </summary>
    public partial class Exercise2Tracker : Window
    {
        public int sets;

        //Wrist Position
        float startLeftWristYPosition = 0;
        float startRightWristYPosition = 0;

        //Shoulder Position

        float startLeftShoulderYPosition = 0;
        float startRightShoulderYPosition = 0;

        //Hip Position
        float startLeftHipZPosition = 0;
        float startRightHipZPosition = 0;

        //Knee Position
        float startLeftKneeZPosition = 0;
        float startRightKneeZPosition = 0;

        int step = 0;
        int startTime = 0;
        int totalTimeElapsed = 0;
        int count = 0;

       // Boolean isExerciseCompleted = false;

        public Exercise2Tracker()
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


                        startLeftWristYPosition = leftwrist.Position.Y;
                        startRightWristYPosition = rightwrist.Position.Y;

                        startLeftShoulderYPosition = leftshoulder.Position.Y;
                        startRightShoulderYPosition = rightshoulder.Position.Y;

                        if ((leftshoulder.Position.Y + 0.08 >= startLeftWristYPosition && leftshoulder.Position.Y - 0.08 <= startLeftWristYPosition) &&
                            (rightshoulder.Position.Y + 0.08 >= startRightWristYPosition && rightshoulder.Position.Y - 0.08 <= startRightWristYPosition))
                        {

                            if (startTime == 0)
                            {
                                startTime = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
                                Console.WriteLine("Condition1 meet at:" + startTime);
                            }
                            else
                            {
                                int currentTime = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
                                totalTimeElapsed = (currentTime - startTime);


                                if (totalTimeElapsed >= 2.0)
                                {
                                    totalTimeElapsed = 0;
                                    startTime = 0;

                                    Console.WriteLine("-------Step1 Done------\n\n");

                                    this.Exercise2_Step1_corr.Visibility = Visibility.Collapsed;
                                    this.Exercise2_Step2_corr.Visibility = Visibility.Visible;
                                    step = 1;
                                    count++;

                                }


                            }
                        }
                        else
                        {
                            startTime = 0;
                            totalTimeElapsed = 0;
                            this.Exercise2_Step1_corr.Visibility = Visibility.Visible;
                        }
                    }

                }
        }
        void Exercise2(object sender, SkeletonFrameReadyEventArgs e)
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
                        Joint leftelbow = playerSkeleton.Joints[JointType.ElbowLeft];
                        Joint rightelbow = playerSkeleton.Joints[JointType.ElbowRight];
                        Joint leftwrist = playerSkeleton.Joints[JointType.WristLeft];
                        Joint rightwrist = playerSkeleton.Joints[JointType.WristRight];
                        Joint lefthip = playerSkeleton.Joints[JointType.HipLeft];
                        Joint righthip = playerSkeleton.Joints[JointType.HipRight];
                        Joint leftknee = playerSkeleton.Joints[JointType.KneeLeft];
                        Joint rightknee = playerSkeleton.Joints[JointType.KneeRight];

                        startLeftWristYPosition = leftwrist.Position.Y;
                        startRightWristYPosition = rightwrist.Position.Y;

                        startLeftHipZPosition = lefthip.Position.Z;
                        startRightHipZPosition = righthip.Position.Z;

                        startLeftKneeZPosition = leftknee.Position.Z;
                        startRightKneeZPosition = rightknee.Position.Z;

                        Console.WriteLine("LeftHipZPosition:" + startLeftHipZPosition + " LeftKneeZPosition:" + startLeftKneeZPosition + " RightHipZPosition:" + startRightHipZPosition + "RightKneeZPosition" + startRightKneeZPosition);


                        //  if (((lefthip.Position.Y + 0.15 >= startLeftKneeYPosition && lefthip.Position.Y - 0.15 <= startLeftKneeYPosition) &&
                        //    (righthip.Position.Y + 0.15 >= startRightKneeYPosition && righthip.Position.Y - 0.15 <= startRightKneeYPosition)))
                        //&&
                        //((leftshoulder.Position.Y + 0.08 >= startLeftWristYPosition && leftshoulder.Position.Y - 0.08 <= startLeftWristYPosition) && 
                        //(rightshoulder.Position.Y + 0.08 >= startRightWristYPosition && rightshoulder.Position.Y - 0.08 <= startRightWristYPosition)))
                        if (((lefthip.Position.Z >= startLeftKneeZPosition + 0.2 && righthip.Position.Z >= startRightKneeZPosition + 0.2)) &&
                            ((leftshoulder.Position.Y + 0.08 >= startLeftWristYPosition && leftshoulder.Position.Y - 0.08 <= startLeftWristYPosition) &&
                        (rightshoulder.Position.Y + 0.08 >= startRightWristYPosition && rightshoulder.Position.Y - 0.08 <= startRightWristYPosition)))
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
                                    Console.WriteLine("-------Step2 Done------\n\n");
                                    //this.countText.AppendText(""+count/2);
                                    //this.countText.Text = "" + count / 2;
                                    this.countText.Content = "" + count / 2;

                                    this.exProgress.Value = count / 2;

                                    if (count / 2 == sets)
                                    {
                                     //   this.exProgress.Value = count / 2;
                                    //    MessageBox.Show("Exercise1 count 2 reached ");
                                     //   Close();


                                        //isExerciseCompleted = true;

                                        this.exProgress.Value = count / 2;
                                        almostDoneLabel.Visibility = Visibility.Hidden;
                                        kinectSkeletonViewer1.Visibility = Visibility.Hidden;
                                        // StopKinect(kinectSensorChooser1.Kinect);
                                        Exercise2_Step1_corr.Visibility = Visibility.Hidden;
                                        Exercise2_Step2_corr.Visibility = Visibility.Hidden;
                                        //errorLabel.Visibility = Visibility.Hidden;
                                        doneLabel1.Visibility = Visibility.Visible;
                                        doneLabel2.Visibility = Visibility.Visible;

                                        //new startwindow(isExerciseCompleted);
                                        new startwindow();
                                    }
                                    else
                                    {
                                        Console.WriteLine(count);
                                        this.Exercise2_Step2_corr.Visibility = Visibility.Collapsed;

                                        this.Exercise2_Step1_corr.Visibility = Visibility.Visible;
                                        step = 0;
                                    }
                                }

                            }
                        }
                        else
                        {
                            startTime = 0;
                            totalTimeElapsed = 0;

                            this.Exercise2_Step2_corr.Visibility = Visibility.Visible;
                        }
                    }

                }

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

