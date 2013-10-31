using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using Microsoft.Kinect;
using Microsoft.Speech.AudioFormat;
using Microsoft.Speech.Recognition;

namespace WpfApplication1
{


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable",
        Justification = "In a full-fledged application, the SpeechRecognitionEngine object should be properly disposed. For the sake of simplicity, we're omitting that code in this sample.")]
    public partial class startwindow : Window
    {

        public String exerciseName = null;
        public int set_val;

        /// <summary>
        /// Active Kinect sensor.
        /// </summary>
        private KinectSensor sensor;
        ExitGesture exitGesture = new ExitGesture();
        /// <summary>
        /// Speech recognition engine using audio data from Kinect.
        /// </summary>
        private SpeechRecognitionEngine speechEngine;

        /// <summary>
        /// List of all UI span elements used to select recognized text.
        /// </summary>
        //private List<Span> recognitionSpans;

        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        /// 
    //    Boolean isExerciseCompleted = false;

        public startwindow()
        {
            InitializeComponent();
        }

        public startwindow(String exerciseName)
        {
            this.exerciseName = exerciseName;
            InitializeComponent();
        }

        /*
        public startwindow(Boolean isExerciseCompleted)
        {
            this.isExerciseCompleted = isExerciseCompleted;
           // InitializeComponent();
        }
         * */

        /// <summary>
        /// Gets the metadata for the speech recognizer (acoustic model) most suitable to
        /// process audio from Kinect device.
        /// </summary>
        /// <returns>
        /// RecognizerInfo if found, <code>null</code> otherwise.
        /// </returns>
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

        /// <summary>
        /// Execute initialization tasks.
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            // Look through all sensors and start the first connected one.
            // This requires that a Kinect is connected at the time of app startup.
            // To make your app robust against plug/unplug, 
            // it is recommended to use KinectSensorChooser provided in Microsoft.Kinect.Toolkit (See components in Toolkit Browser).
            foreach (var potentialSensor in KinectSensor.KinectSensors)
            {
                if (potentialSensor.Status == KinectStatus.Connected)
                {
                    this.sensor = potentialSensor;
                    break;
                }
            }

            if (null != this.sensor)
            {
                try
                {
                    // Start the sensor!
                    this.sensor.Start();
                }
                catch (IOException)
                {
                    // Some other application is streaming from the same Kinect sensor
                    this.sensor = null;
                }
            }

            if (null == this.sensor)
            {
                // this.statusBarText.Text = Properties.Resources.NoKinectReady;
                return;
            }

            RecognizerInfo ri = GetKinectRecognizer();

            if (null != ri)
            {

                this.speechEngine = new SpeechRecognitionEngine(ri.Id);


                var directions = new Choices();
                directions.Add(new SemanticResultValue("begin", "Begin"));
                directions.Add(new SemanticResultValue("stop", "Stop"));
                directions.Add(new SemanticResultValue("home", "Home"));
                directions.Add(new SemanticResultValue("exit", "Exit"));
                directions.Add(new SemanticResultValue("no", "No"));
                var gb = new GrammarBuilder { Culture = ri.Culture };
                gb.Append(directions);

                var g = new Grammar(gb);



                speechEngine.LoadGrammar(g);
                speechEngine.SpeechRecognized += SpeechRecognized;
                speechEngine.SpeechRecognitionRejected += SpeechRejected;

                // For long recognition sessions (a few hours or more), it may be beneficial to turn off adaptation of the acoustic model. 
                // This will prevent recognition accuracy from degrading over time.
                speechEngine.UpdateRecognizerSetting("AdaptationOn", 0);

                speechEngine.SetInputToAudioStream(
                    sensor.AudioSource.Start(), new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null));
                speechEngine.RecognizeAsync(RecognizeMode.Multiple);
               
               
            }
            else
            {
                //this.statusBarText.Text = Properties.Resources.NoSpeechRecognizer;
            }
        }

        /// <summary>
        /// Execute uninitialization tasks.
        /// </summary>
        /// <param name="sender">object sending the event.</param>
        /// <param name="e">event arguments.</param>
        private void WindowClosing(object sender, CancelEventArgs e)
        {
            if (null != this.sensor)
            {
                this.sensor.AudioSource.Stop();

                this.sensor.Stop();
                this.sensor = null;
            }

            if (null != this.speechEngine)
            {
                this.speechEngine.SpeechRecognized -= SpeechRecognized;
                this.speechEngine.SpeechRecognitionRejected -= SpeechRejected;
                this.speechEngine.RecognizeAsyncStop();
            }
        }

        /// <summary>
        /// Remove any highlighting from recognition instructions.
        /// </summary>


        /// <summary>
        /// Handler for recognized speech events.
        /// </summary>
        /// <param name="sender">object sending the event.</param>
        /// <param name="e">event arguments.</param>
        private void SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            // Speech utterance confidence below which we treat speech as if it hadn't been heard
            const double ConfidenceThreshold = 0.3;



            if (e.Result.Confidence >= ConfidenceThreshold)
            {
                switch (e.Result.Semantics.Value.ToString())
                {
                    case "Begin":
                        Console.WriteLine("Start requested for " + exerciseName);

                        switch (exerciseName)
                        {
                            case "Stretching":
                                Exercise1Tracker exercise1Tracker = new Exercise1Tracker();
                                exercise1Tracker.sets = set_val;
                                exercise1Tracker.Show();                               
                                break;

                            case "Squats":
                                Exercise2Tracker exercise2Tracker = new Exercise2Tracker();
                                exercise2Tracker.sets = set_val;
                                exercise2Tracker.Show();                                
                                break;
                        }

                       Application.Current.Windows[0].Close();
                       break;

                    case "Stop":
                        Console.WriteLine("Stop requested");
                       // exerciseexitwindow.Close();
                        exitGesture.Show();
                        break;

                    case "No":

                     /*   if (isExerciseCompleted)
                        {
                            isExerciseCompleted = false;
                            new MainWindow().Show();
                        }
                        else
                            exitGesture.Close();
                    */
                        //exitGesture.Close();                        
                        exitGesture.Hide(); 
                        break;

                    case "Home":
                        Console.WriteLine("Home requested");

                        new MainWindow().Show();

                        break;

                    case "Exit":
                        Console.WriteLine("Exit requested");
                        exitGesture.Show();
                        break;
                   

                }
            }
        }

        /// <summary>
        /// Handler for rejected speech events.
        /// </summary>
        /// <param name="sender">object sending the event.</param>
        /// <param name="e">event arguments.</param>
        private void SpeechRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            Console.WriteLine("Not recognized");
        }
        /*
        /// <summary>
        /// Enumeration of directions in which turtle may be facing.
        /// </summary>
        private enum Direction
        {


        }*/
    }
}