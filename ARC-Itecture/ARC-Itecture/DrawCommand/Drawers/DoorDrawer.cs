﻿/*
 * ARC-Itecture
 * Romain Capocasale, Vincent Moulin and Jonas Freiburghaus
 * He-Arc, INF3dlm-a
 * 2019-2020
 * .NET Course
 */


using ARC_Itecture.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ARC_Itecture.DrawCommand.Drawers
{

    /// <summary>
    /// Peforms all the doors drawing
    /// </summary>
    class DoorDrawer : Drawer
    {
        private List<Point> _doorAvailablePoints;
        private Stack<Point> _doorPoints;

        private const int DOOR_MAXIMUM_DISTANCE = 80;
        private const int DOOR_MINIMUM_DISTANCE = 10;
        
        public DoorDrawer(Receiver receiver, ref List<Point> doorAvailablePoints) 
            : base(receiver)
        {
            this._doorAvailablePoints = doorAvailablePoints;
            this._doorPoints = new Stack<Point>();
        }

        public override void Draw(Point p)
        {
            _doorPoints.Push(p);

            //Count two point to create the rectangle area
            if (_doorPoints.Count == 2)
            {
                Point p2 = _doorPoints.Pop();
                Point p1 = _doorPoints.Pop();

                this._fillBrush = new SolidColorBrush(ImageUtil.RandomColor());

                // Get the last shape on the canvas, this represents the preview rectangle
                Rect rect = new Rect(InkCanvas.GetLeft(_receiver.LastShape), InkCanvas.GetTop(_receiver.LastShape), _receiver.LastShape.Width, _receiver.LastShape.Height);

                List<Point> doorAnchorPoints = new List<Point>(); // Contains the points inside the selection rectangle preview

                // Loop through the points available for the drawing of a door, If these points are in the selection rectangle they are added to the list of selected points
                // Check also if the point isn't already in the list
                foreach (Point doorPoint in _doorAvailablePoints)
                {
                    if (rect.Contains(doorPoint) && !doorAnchorPoints.Contains(doorPoint))
                    {
                        doorAnchorPoints.Add(doorPoint);
                    }
                }

                // There must be at least 2 dots to draw a door.
                if (doorAnchorPoints.Count >= 2)
                {
                    double pointsDoorDistance = MathUtil.DistanceBetweenTwoPoints(doorAnchorPoints[0], doorAnchorPoints[1]);

                    //The door must respect a maximum and minimum distance
                    if (pointsDoorDistance > DOOR_MINIMUM_DISTANCE && pointsDoorDistance < DOOR_MAXIMUM_DISTANCE && !IsDoorOnWall(doorAnchorPoints[0], doorAnchorPoints[1]))
                    {

                        Rectangle rectangle = new Rectangle
                        {
                            Fill = Application.Current.TryFindResource("PrimaryHueLightBrush") as SolidColorBrush
                        };

                        // Add the door to the canvas
                        doorAnchorPoints = doorAnchorPoints.OrderBy(point => point.X).ToList();
                        InkCanvas.SetLeft(rectangle, doorAnchorPoints[0].X);

                        doorAnchorPoints = doorAnchorPoints.OrderBy(point => point.Y).ToList();
                        InkCanvas.SetTop(rectangle, doorAnchorPoints[0].Y);

                        double width = Math.Abs(doorAnchorPoints[1].X - doorAnchorPoints[0].X);
                        double height = Math.Abs(doorAnchorPoints[1].Y - doorAnchorPoints[0].Y);

                        // If the height or width of the door is below the size threshold it is set to a certain value, otherwise it is not shown in the drawing
                        if (width <= COMPONENT_OFFSET)
                        {
                            width = COMPONENT_OFFSET;
                        }
                        if (height <= COMPONENT_OFFSET)
                        {
                            height = COMPONENT_OFFSET;
                        }

                        rectangle.Width = width;
                        rectangle.Height = height;

                        _receiver.ViewModel._mainWindow.canvas.Children.Add(rectangle);

                        Door door = _receiver.ViewModel._plan.AddDoor(doorAnchorPoints[0], doorAnchorPoints[1]); // add the door to the plan

                        //Add the door to the history
                        MainWindow.main.History = "Door";
                        _receiver.ViewModel._stackHistory.Push(new Tuple<object, object, string>(rectangle, door, "Door"));
                    }
                }

                _receiver.ViewModel._mainWindow.canvas.Children.Remove(_receiver.LastShape);
                _receiver.LastShape = null;
            }
        }

        /// <summary>
        /// Draw the door rectangle preview
        /// </summary>
        /// <param name="p">Door preview point</param>
        public override void DrawPreview(Point p)
        {
            _fillBrush = new SolidColorBrush(Color.FromArgb(50, 255, 255, 255));

            if (_receiver.LastShape is Rectangle lastRectangle)
                _receiver.ViewModel._mainWindow.canvas.Children.Remove(lastRectangle);

            if (_doorPoints.Count > 0)
                _receiver.LastShape = DrawRectangle(_doorPoints.Peek(), p);
        }



        /// <summary>
        /// Check if a door is on a wall
        /// </summary>
        /// <param name="p1">Door first point</param>
        /// <param name="p2">Door second point</param>
        /// <returns>True if the door is on a wall, otherwise false</returns>
        private bool IsDoorOnWall(Point p1, Point p2)
        {
            bool isDoorOnWall = false;

            // Sort the X and Y coordinates points
            List<double> doorPointsX = new List<double>() { Math.Floor(p1.X), Math.Floor(p2.X) };
            List<double> doorPointsY = new List<double>() { Math.Floor(p1.Y), Math.Floor(p2.Y) };
            doorPointsX.Sort();
            doorPointsY.Sort();

            foreach (UIElement element in _receiver.ViewModel._mainWindow.canvas.Children)
            {
                // Check if a wall contain the given door coordinates
                if (element is Line line)
                {
                    List<double> segmentPointsX = new List<double>() { Math.Floor(line.X1), Math.Floor(line.X2) };
                    List<double> segmentPointsY = new List<double>() { Math.Floor(line.Y1), Math.Floor(line.Y2) };
                    segmentPointsX.Sort();
                    segmentPointsY.Sort();

                    if ((doorPointsX[0] == segmentPointsX[0] && doorPointsX[1] == segmentPointsX[1]) && (doorPointsY[0] == segmentPointsY[0] && doorPointsY[1] == segmentPointsY[1]))
                    {
                        isDoorOnWall = true;
                    }
                }
            }
            return isDoorOnWall;
        }
    }
}
