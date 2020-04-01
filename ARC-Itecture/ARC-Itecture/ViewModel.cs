﻿using ARC_Itecture.DrawCommand;
using ARC_Itecture.DrawCommand.Commands;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ARC_Itecture
{
    class ViewModel
    {
        private MainWindow _mainWindow;
        private Invoker _invoker;
        private Receiver _receiver;
        public Plan plan;

        public ViewModel(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            this._invoker = new Invoker();
            plan = new Plan();
            this._receiver = new Receiver(_mainWindow.canvas, plan);
        }

        public void AddArea()
        {
            this._invoker.Command = new AreaCommand(this._receiver);
        }

        public void AddCamera()
        {
            this._invoker.Command = new CameraCommand(this._receiver);
        }

        public void AddDoor()
        {
            this._invoker.Command = new DoorCommand(this._receiver);
        }

        public void AddWall()
        {
            this._invoker.Command = new WallCommand(this._receiver);
        }

        public void AddWindow()
        {
            this._invoker.Command = new WindowCommand(this._receiver);
        }

        public void CanvasClick(Point p)
        {
            this._invoker.Invoke(p);
        }

        public void LoadJson(string filename)
        {
            plan = JsonConvert.DeserializeObject<Plan>(File.ReadAllText(filename));
        }

        public void SaveJson(string filename)
        {
            using (StreamWriter file = File.CreateText(filename))
            {
                (new JsonSerializer()).Serialize(file, plan);
            }
        }

        public bool ValidateConstantePlan()
        {
            bool isConstantOk = true;
            int value;
            if(_mainWindow.textBoxWindowH2.Text == null || int.TryParse(_mainWindow.textBoxWindowH2.Text, out value))
            {
                MessageBox.Show("Window H2 must contain a value or be a number !");
                isConstantOk = false;
            }
            return isConstantOk;
        }
    }
}