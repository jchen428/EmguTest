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
            Image<Gray, Byte> gray = frame.Convert<Gray, Byte>().PyrDown().PyrUp();

            gray = getColorRegions(frame, new Bgr(0, 116, 167), new Bgr(91, 222, 251));
            frame = getCircles(gray);
            camImageBox.Image = frame;
        }

        /**
         * Find regions with color values between lower and upper Bgr bounds
         */
        private Image<Gray, Byte> getColorRegions(Image<Bgr, Byte> img, Bgr lower, Bgr upper)
        {
            Image<Gray, Byte> blobs = img.InRange(lower, upper);

            return blobs;
        }

        /**
         * Find circles from a Bgr Image
         */
        private Image<Bgr, Byte> getCircles(Image<Bgr, Byte> img)
        {
            Image<Gray, Byte> gray = img.Convert<Gray, Byte>().PyrDown().PyrUp();

            double cannyThreshold = 180.0;
            double circleAccumulatorThreshold = 50;

            CircleF[] circles = gray.HoughCircles(new Gray(cannyThreshold), new Gray(circleAccumulatorThreshold), 2.5, 50, 5, 0)[0];

            foreach (CircleF circle in circles)
            {
                frame.Draw(circle, new Bgr(Color.Red), 2);
            }

            return frame;
        }

        /**
         * Find circles from a Gray image
         */
        private Image<Bgr, Byte> getCircles(Image<Gray, Byte> gray)
        {
            Image<Bgr, Byte> bgrCopy = gray.Convert<Bgr, Byte>().PyrDown().PyrUp();
            double cannyThreshold = 180.0;
            double circleAccumulatorThreshold = 50;

            CircleF[] circles = gray.HoughCircles(new Gray(cannyThreshold), new Gray(circleAccumulatorThreshold), 2.5, 50, 5, 0)[0];

            foreach (CircleF circle in circles)
            {
                bgrCopy.Draw(circle, new Bgr(Color.Red), 2);
            }

            return bgrCopy;
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
