﻿using System;
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

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (fileNameTextBox.Text != String.Empty)
            {
                Image<Bgr, Byte> img = 
                   new Image<Bgr, byte>(fileNameTextBox.Text)
                   .Resize(640, 480, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR, true);

                rawImageBox.Image = img;
                processedImageBox.Image = getContours(img);
            }
        }

        private void loadImageButton_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK || result == DialogResult.Yes)
            {
                fileNameTextBox.Text = openFileDialog1.FileName;
            }
        }

        private void releaseData()
        {
            if (capture != null)
            {
                capture.Dispose();
            }
        }

        /**
         * Creates an Image, captures a frame from camera and displays it 
         * rawImageBox,and processes and displays the Image in processedImageBox
         */
        private void processFrame(object sender, EventArgs arg)
        {
            frame = capture.QueryFrame();
            rawImageBox.Image = frame;

            Image<Gray, Byte> gray = frame.Convert<Gray, Byte>().PyrDown().PyrUp();

            //gray = filterColor(frame, new Bgr(0, 116, 167), new Bgr(91, 222, 251));
            //frame = getCircles(gray);
            ////frame = getCorners(gray).Convert<Bgr, Byte>().PyrDown().PyrUp();

            processedImageBox.Image = getContours(frame);
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
         * ADUST THE THRESHOLD VALUE
         */
        private Image<Bgr, Byte> getContours(Image<Bgr, Byte> color)
        {
            int thresholdValue = 100;
            Image<Gray, Byte> gray = color.Convert<Gray, Byte>().PyrDown().PyrUp();
            gray = gray.ThresholdBinary(new Gray(thresholdValue), new Gray(255));

            using (MemStorage storage = new MemStorage())
            {
                for (Contour<Point> contours = gray.FindContours(Emgu.CV.CvEnum.CHAIN_APPROX_METHOD.CV_CHAIN_APPROX_SIMPLE, Emgu.CV.CvEnum.RETR_TYPE.CV_RETR_TREE, storage);
                    contours != null; contours = contours.HNext)
                {
                    Contour<Point> curr = contours.ApproxPoly(contours.Perimeter * 0.015, storage);

                    if (curr.BoundingRectangle.Width > 20)
                    {
                        CvInvoke.cvDrawContours(color, contours, new MCvScalar(255), new MCvScalar(255), -1, 2, Emgu.CV.CvEnum.LINE_TYPE.EIGHT_CONNECTED, new Point(0, 0));
                        color.Draw(curr.BoundingRectangle, new Bgr(0, 0, 255), 1);
                    }
                }
            }

            return color;
        }
    }
}
