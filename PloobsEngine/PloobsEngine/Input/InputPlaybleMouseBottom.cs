﻿#region License
/*
    PloobsEngine Game Engine Version 0.3 Beta
    Copyright (C) 2011  Ploobs

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/
#endregion
#if WINDOWS || WINRT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace PloobsEngine.Input
{
    public delegate void MouseStateChangeComplete(MouseState ms);

    public abstract class InputPlaybleMouseBottom : IInput
    {
        public abstract StateKey StateKey
        {
            get;
        }
        public abstract MouseButtons MouseButtons
        {
            get;
        }
        public abstract EntityType EntityType
        {
            get;
        }

        public event MouseStateChangeComplete KeyStateChange = null;

        internal void FireEvent(MouseState ms)
        {
            if (KeyStateChange != null)
                KeyStateChange(ms);

        }
        
        public abstract InputMask InputMask
        {
            get;
        }
        

    }

    
}
#endif