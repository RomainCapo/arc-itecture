﻿using System;
using System.Windows;

namespace ARC_Itecture.DrawCommand.Commands
{
    class AreaCommand : IDrawCommand
    {

        private Receiver _receiver;
        private const ComponentType _componentType = ComponentType.Area;

        public AreaCommand(Receiver receiver)
        {
            this._receiver = receiver;
        }

        public void Execute(Point point)
        {
            this._receiver.DrawArea(point, _componentType);
        }
    }
}