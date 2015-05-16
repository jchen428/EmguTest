using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.Util;

//using DirectShowLib;

namespace CameraCapture
{
    public partial class CameraCapture : Form
    {
        //Declare global variables
        private Capture capture;        //Takes images from camera as frames
        private bool captureInProgress;
        private Image<Bgr, Byte> frame;
        
        public CameraCapture()
        {
            InitializeComponent();
        }

        /**
         * Creates an EmguCV Image, captures a frame from camera and 
         * allocates it to the imageFrame, and displays it in ImageBox
         */
        private void processFrame(object sender, EventArgs arg)
        {
            frame = capture.QueryFrame();
            rawImageBox.Image = frame;

            Image<Gray, Byte> gray = frame.Convert<Gray, Byte>().PyrDown().PyrUp();

            gray = filterColor(frame, new Bgr(0, 116, 167), new Bgr(91, 222, 251));
            frame = getCircles(gray);
            //frame = getCorners(gray).Convert<Bgr, Byte>().PyrDown().PyrUp();

            processedImageBox.Image = frame;
        }

        /**
         * Find regions with color values between lower and upper Bgr bounds
         */
        private Image<Gray, Byte> filterColor(Image<Bgr, Byte> img, Bgr lower, Bgr upper)
        {
            Image<Gray, Byte> regions = img.InRange(lower, upper);

            return regions;
        }
        
        /**
         * Find circles from a Bgr Image
         */
        /*private Image<Bgr, Byte> getCircles(Image<Bgr, Byte> img)
        {
            Image<Gray, Byte> gray = img.Convert<Gray, Byte>().PyrDown().PyrUp();

            double cannyThreshold = 180.0;
            double circleAccumulatorThreshold = 50;

            CircleF[] circles = gray.HoughCircles(new Gray(cannyThreshold), new Gray(circleAccumulatorThreshold), 2.5, 50, 25, 150)[0];

            foreach (CircleF circle in circles)
            {
                frame.Draw(circle, new Bgr(Color.Red), 2);
            }

            return frame;
        }*/

        /**
         * Find circles from a Gray image
         */
        private Image<Bgr, Byte> getCircles(Image<Gray, Byte> gray)
        {
            Image<Bgr, Byte> bgrCopy = gray.Convert<Bgr, Byte>().PyrDown().PyrUp();
            double cannyThreshold = 180.0;
            double circleAccumulatorThreshold = 50;

            CircleF[] circles = gray.HoughCircles(new Gray(cannyThreshold), new Gray(circleAccumulatorThreshold), 2.5, 50, 25, 150)[0];

            foreach (CircleF circle in circles)
            {
                bgrCopy.Draw(circle, new Bgr(Color.Red), 2);
            }

            return bgrCopy;
        }

        /**
         * May be deprecated
         */
        private Image<Gray, Byte> getCorners(Image<Gray, Byte> gray)
        {
            Image<Gray, float> rawCorners = null;   //Raw corner strength image (must be 32-bit float)
            Image<Gray, Byte> dispCorners = null;   //Inverted threshold corner strengths (for display)

            //Create raw corner strength image and do Harris algorithm
            rawCorners = new Image<Gray, float>(gray.Size);
            CvInvoke.cvCornerHarris(gray, rawCorners, 3, 3, 0.01);

            //Create and return inverted threshold image
            dispCorners = new Image<Gray, Byte>(gray.Size);
            CvInvoke.cvThreshold(rawCorners, dispCorners, 0.0001, 255, Emgu.CV.CvEnum.THRESH.CV_THRESH_BINARY);

            return dispCorners;
        }

        /**
         * Toggles captureInProgress and starts/stops capture
         */
        private void btnStart_Click(object sender, EventArgs e)
        {
            #region Instantiate new Capture if one doesn't already exist
            if (capture == null)
            {
                try
                {
                    capture = new Capture();
                }
                catch (NullReferenceException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            #endregion

            if (capture != null)
            {
                if (captureInProgress)      //Stop capture and reset btnStart text to "Start"
                {
                    btnStart.Text = "Start";
                    Application.Idle -= processFrame;
                }
                else                        //Start capture and set btnStart text to "Stop"
                {
                    btnStart.Text = "Stop";
                    Application.Idle += processFrame;
                }

                captureInProgress = !captureInProgress;     //Invert captureInProgress
            }
        }

        private void releaseData()
        {
            if (capture != null)
            {
                capture.Dispose();
            }
        }
    }
}
