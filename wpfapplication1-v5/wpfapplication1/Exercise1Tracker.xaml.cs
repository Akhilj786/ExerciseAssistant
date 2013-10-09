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

        private KinectSensor _Kinect;
        private WriteableBitmap _ColorImageBitmap;
        private Int32Rect _ColorImageBitmapRect;
        private int _ColorImageStride;
        private Skeleton[] FrameSkeletons;

        List<Button> buttons;
        static Button selected;

        float handX;
        float handY;

        public Exercise1Tracker()
        {
           // this.Hide();
            
            InitializeComponent();
            InitializeButtons();

            Generics.ResetHandPosition(kinectButton);
            kinectButton.Click += new RoutedEventHandler(kinectButton_Click);
            this.Loaded += (s, e) => { DiscoverKinectSensor(); };

            this.WindowState = System.Windows.WindowState.Maximized;
            this.WindowStyle = System.Windows.WindowStyle.None;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // MessageBox.Show("Window Loading");
            kinectSensorChooser1.KinectSensorChanged += new DependencyPropertyChangedEventHandler(kinectSensorChooser1_KinectSensorChanged);
            //this.WindowState = FormWindowState.Maximized;
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
                                    //Uri step2_img = new Uri("C:/Users/Akku/Dropbox/NUI/Akhil/Intial_Prototype/img/step2.jpg", UriKind.Absolute);
                                    //ImageSource img_src = new BitmapImage(step2_img);
                                    //this.Step2_corr.Source = img_src;
                                    this.Step2_corr.Visibility = Visibility.Visible;
                                    step = 1;
                                    count++;

                                }
                             

                            }
                        }
                        else
                        {
                            startTime = 0;
                            totalTimeElapsed = 0;
                            //Uri step1_corr = new Uri("C:/Users/Akku/Dropbox/NUI/Akhil/Intial_Prototype/img/step1.jpg", UriKind.Absolute);
                            //ImageSource img_src = new BitmapImage(step1_corr);
                            //this.Step1_corr.Source = img_src;
                            this.Step1_corr.Visibility = Visibility.Visible;
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
                      
                        if ((startLeftWristYPosition  >= head.Position.Y && startRightWristYPosition >= head.Position.Y) &&
                          (startLeftElbowYPosition >= leftshoulder.Position.Y && startRightElbowYPosition >= rightshoulder.Position.Y) 
                           &&
                           (startLeftWristXPosition + 0.08 >= startLeftElbowXPosition && startLeftWristXPosition - 0.08 <= startLeftElbowXPosition) &&
                            (startRightWristXPosition + 0.08 >= startRightElbowXPosition && startRightWristXPosition - 0.08 <= startRightElbowXPosition)
                            )
                      
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
                                    //Console.WriteLine("Exercise2 Done startLeftWristXPosition: " + startLeftWristXPosition + " endLeftWristXPosition" + endLeftWristXPosition + " endLeftWristYPosition: " + endLeftWristYPosition + " startLeftWristYPosition:" + startLeftWristYPosition);
                                    Console.WriteLine("-------Step2 Done------\n\n");
                                    if (count == 4)
                                    {

                                       
                                        MessageBox.Show("Exercise1 Counting 2 Done ");
                                        // Environment.Exit(System);
                                        Close();
                                    }
                                    else {
                                        this.Step2_corr.Visibility = Visibility.Collapsed;
                                        //Uri step1_img = new Uri("C:/Users/Akku/Dropbox/NUI/Akhil/Intial_Prototype/img/step1.jpg", UriKind.Absolute);
                                        //ImageSource img_src = new BitmapImage(step1_img);
                                        //this.Step1_corr.Source = img_src;
                                        this.Step1_corr.Visibility = Visibility.Visible;
                                        step = 0;
                                    }
                                }

                            }
                        }
                        else
                        {
                            startTime = 0;
                            totalTimeElapsed = 0;
                            //Uri step2_corr = new Uri("C:/Users/Akku/Dropbox/NUI/Akhil/Intial_Prototype/img/step2.jpg", UriKind.Absolute);
                            //ImageSource img_src = new BitmapImage(step2_corr);
                            //this.Step2_corr.Source = img_src;
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

        private void kinectSkeletonViewer1_Loaded(object sender, RoutedEventArgs e) {

        }


        #region "Hand Gesture"

        //initialize buttons to be checked
        private void InitializeButtons()
        {
            buttons = new List<Button> { Exit, BACKHOME };
        }
        //raise event for Kinect sensor status changed
        private void DiscoverKinectSensor()
        {
            KinectSensor.KinectSensors.StatusChanged += KinectSensors_StatusChanged;
            this.Kinect = KinectSensor.KinectSensors.FirstOrDefault(x => x.Status == KinectStatus.Connected);
        }


        private void KinectSensors_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            switch (e.Status)
            {
                case KinectStatus.Connected:
                    if (this.Kinect == null)
                    {
                        this.Kinect = e.Sensor;
                    }
                    break;
                case KinectStatus.Disconnected:
                    if (this.Kinect == e.Sensor)
                    {
                        this.Kinect = null;
                        this.Kinect = KinectSensor.KinectSensors.FirstOrDefault(x => x.Status == KinectStatus.Connected);
                        if (this.Kinect == null)
                        {
                            MessageBox.Show("Sensor Disconnected. Please reconnect to continue.");
                        }
                    }
                    break;
            }
        }

        public KinectSensor Kinect
        {
            get { return this._Kinect; }
            set
            {
                if (this._Kinect != value)
                {
                    if (this._Kinect != null)
                    {
                        this._Kinect = null;
                    }
                    if (value != null && value.Status == KinectStatus.Connected)
                    {
                        this._Kinect = value;
                        InitializeKinectSensor(this._Kinect);
                    }
                }
            }
        }



        private void InitializeKinectSensor(KinectSensor kinectSensor)
        {
            if (kinectSensor != null)
            {
                ColorImageStream colorStream = kinectSensor.ColorStream;
                colorStream.Enable();
                this._ColorImageBitmap = new WriteableBitmap(colorStream.FrameWidth, colorStream.FrameHeight,
                    96, 96, PixelFormats.Bgr32, null);
                this._ColorImageBitmapRect = new Int32Rect(0, 0, colorStream.FrameWidth, colorStream.FrameHeight);
                this._ColorImageStride = colorStream.FrameWidth * colorStream.FrameBytesPerPixel;
                // videoStream.Source = this._ColorImageBitmap;

                kinectSensor.SkeletonStream.Enable(new TransformSmoothParameters()
                {
                    Correction = 0.5f,
                    JitterRadius = 0.05f,
                    MaxDeviationRadius = 0.04f,
                    Smoothing = 0.5f
                });

                kinectSensor.SkeletonFrameReady += Kinect_SkeletonFrameReady;
                kinectSensor.ColorFrameReady += Kinect_ColorFrameReady;

                if (!kinectSensor.IsRunning)
                {
                    kinectSensor.Start();
                }

                this.FrameSkeletons = new Skeleton[this.Kinect.SkeletonStream.FrameSkeletonArrayLength];

            }
        }

        private void Kinect_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (ColorImageFrame frame = e.OpenColorImageFrame())
            {
                if (frame != null)
                {
                    byte[] pixelData = new byte[frame.PixelDataLength];
                    frame.CopyPixelDataTo(pixelData);
                    this._ColorImageBitmap.WritePixels(this._ColorImageBitmapRect, pixelData,
                        this._ColorImageStride, 0);
                }
            }
        }

        private void Kinect_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            using (SkeletonFrame frame = e.OpenSkeletonFrame())
            {
                if (frame != null)
                {
                    frame.CopySkeletonDataTo(this.FrameSkeletons);
                    Skeleton skeleton = GetPrimarySkeleton(this.FrameSkeletons);

                    if (skeleton == null)
                    {
                        kinectButton.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        Joint primaryHand = GetPrimaryHand(skeleton);
                        TrackHand(primaryHand);

                    }
                }
            }
        }

        //track and display hand
        private void TrackHand(Joint hand)
        {
            if (hand.TrackingState == JointTrackingState.NotTracked)
            {
                kinectButton.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                kinectButton.Visibility = System.Windows.Visibility.Visible;

                DepthImagePoint point = this.Kinect.CoordinateMapper.MapSkeletonPointToDepthPoint(hand.Position, DepthImageFormat.Resolution640x480Fps30);
                handX = (int)((point.X * LayoutRoot.ActualWidth / this.Kinect.DepthStream.FrameWidth) -
                    (kinectButton.ActualWidth / 2.0));
                handY = (int)((point.Y * LayoutRoot.ActualHeight / this.Kinect.DepthStream.FrameHeight) -
                    (kinectButton.ActualHeight / 2.0));
                Canvas.SetLeft(kinectButton, handX);
                Canvas.SetTop(kinectButton, handY);

                if (isHandOver(kinectButton, buttons)) kinectButton.Hovering();
                else kinectButton.Release();
                if (hand.JointType == JointType.HandRight)
                {
                    kinectButton.ImageSource = "/WpfApplication1;component/Images/myhand.png";
                    kinectButton.ActiveImageSource = "/WpfApplication1;component/Images/myhand.png";
                }
                else
                {
                    kinectButton.ImageSource = "/WpfApplication1;component/Images/myhand.png";
                    kinectButton.ActiveImageSource = "/WpfApplication1;component/Images/myhand.png";
                }
            }
        }

        //detect if hand is overlapping over any button
        private bool isHandOver(FrameworkElement hand, List<Button> buttonslist)
        {
            var handTopLeft = new Point(Canvas.GetLeft(hand), Canvas.GetTop(hand));
            var handX = handTopLeft.X + hand.ActualWidth / 2;
            var handY = handTopLeft.Y + hand.ActualHeight / 2;

            foreach (Button target in buttonslist)
            {

                if (target != null)
                {
                    Point targetTopLeft = new Point(Canvas.GetLeft(target), Canvas.GetTop(target));
                    if (handX > targetTopLeft.X &&
                        handX < targetTopLeft.X + target.Width &&
                        handY > targetTopLeft.Y &&
                        handY < targetTopLeft.Y + target.Height)
                    {
                        selected = target;
                        return true;
                    }
                }
            }
            return false;
        }

        //get the hand closest to the Kinect sensor
        private static Joint GetPrimaryHand(Skeleton skeleton)
        {
            Joint primaryHand = new Joint();
            if (skeleton != null)
            {
                primaryHand = skeleton.Joints[JointType.HandLeft];
                Joint rightHand = skeleton.Joints[JointType.HandRight];
                if (rightHand.TrackingState != JointTrackingState.NotTracked)
                {
                    if (primaryHand.TrackingState == JointTrackingState.NotTracked)
                    {
                        primaryHand = rightHand;
                    }
                    else
                    {
                        if (primaryHand.Position.Z > rightHand.Position.Z)
                        {
                            primaryHand = rightHand;
                        }
                    }
                }
            }
            return primaryHand;
        }

        //get the skeleton closest to the Kinect sensor
        private static Skeleton GetPrimarySkeleton(Skeleton[] skeletons)
        {
            Skeleton skeleton = null;
            if (skeletons != null)
            {
                for (int i = 0; i < skeletons.Length; i++)
                {
                    if (skeletons[i].TrackingState == SkeletonTrackingState.Tracked)
                    {
                        if (skeleton == null)
                        {
                            skeleton = skeletons[i];
                        }
                        else
                        {
                            if (skeleton.Position.Z > skeletons[i].Position.Z)
                            {
                                skeleton = skeletons[i];
                            }
                        }
                    }
                }
            }
            return skeleton;
        }

        void kinectButton_Click(object sender, RoutedEventArgs e)
        {
            selected.RaiseEvent(new RoutedEventArgs(Button.ClickEvent, selected));
        }
        private void UnregisterEvents()
        {
            KinectSensor.KinectSensors.StatusChanged -= KinectSensors_StatusChanged;
            this.Kinect.SkeletonFrameReady -= Kinect_SkeletonFrameReady;
            this.Kinect.ColorFrameReady -= Kinect_ColorFrameReady;

        }

        #endregion 


        private void BACKHOME_Click(object sender, RoutedEventArgs e) {
            (Application.Current.MainWindow.FindName("_mainFrame") as Frame).Source = new Uri("MainMenu.xaml", UriKind.Relative);
        }

        private void Exit_Click(object sender, RoutedEventArgs e) {
            Application.Current.Windows[1].Close();
        }       
      }
    }

