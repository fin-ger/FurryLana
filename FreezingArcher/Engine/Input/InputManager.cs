//
//  InputManager.cs
//
//  Author:
//       Fin Christensen <christensen.fin@gmail.com>
//
//  Copyright (c) 2014 Fin Christensen
//
//  This program is free software; you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation; either version 2 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program; if not, write to the Free Software
//  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
//
using System.Collections.Generic;
using Pencil.Gaming;
using Pencil.Gaming.MathUtils;
using FreezingArcher.Output;

namespace FreezingArcher.Input
{
    /// <summary>
    /// Input manager.
    /// </summary>
    public class InputManager
    {
        /// <summary>
        /// The name of the class.
        /// </summary>
        public static readonly string ClassName = "InputManager";

        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Input.InputManager"/> class.
        /// </summary>
        public InputManager ()
        {
            Logger.Log.AddLogEntry (LogLevel.Debug, ClassName, "Creating new input manager");
            Keys = new List<KeyboardInput> ();
            Mouse = new List<MouseInput> ();
            MouseMovement = Vector2.Zero;
            MouseScroll = Vector2.Zero;
            OldMousePosition = Vector2.Zero;
        }

        /// <summary>
        /// The keys.
        /// </summary>
        protected List<KeyboardInput> Keys;

        /// <summary>
        /// The mouse button inputs.
        /// </summary>
        protected List<MouseInput> Mouse;

        /// <summary>
        /// The mouse movement.
        /// </summary>
        protected Vector2 MouseMovement;

        /// <summary>
        /// The mouse scroll.
        /// </summary>
        protected Vector2 MouseScroll;

        /// <summary>
        /// The old mouse position.
        /// </summary>
        protected Vector2 OldMousePosition;

        /// <summary>
        /// Handles the keyboard input.
        /// </summary>
        /// <param name="window">Window.</param>
        /// <param name="key">Key.</param>
        /// <param name="scancode">Scancode.</param>
        /// <param name="action">Action.</param>
        /// <param name="modifier">Modifier.</param>
        public void HandleKeyboardInput (GlfwWindowPtr window, Key key, int scancode,
                                         KeyAction action, KeyModifiers modifier)
        {
            Keys.Add (new KeyboardInput (key, scancode, action, modifier));
        }

        /// <summary>
        /// Handles the mouse button.
        /// </summary>
        /// <param name="window">Window.</param>
        /// <param name="button">Button.</param>
        /// <param name="action">Action.</param>
        public void HandleMouseButton (GlfwWindowPtr window, MouseButton button, KeyAction action)
        {
            Mouse.Add (new MouseInput (button, action));
        }

        /// <summary>
        /// Handles the mouse move.
        /// </summary>
        /// <param name="window">Window.</param>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        public void HandleMouseMove (GlfwWindowPtr window, double x, double y)
        {
            MouseMovement += new Vector2 ((float) x - OldMousePosition.X, (float) y - OldMousePosition.Y);
            OldMousePosition = new Vector2 ((float) x, (float) y);
        }

        /// <summary>
        /// Handles the mouse scroll.
        /// </summary>
        /// <param name="window">Window.</param>
        /// <param name="xoffs">Xoffs.</param>
        /// <param name="yoffs">Yoffs.</param>
        public void HandleMouseScroll (GlfwWindowPtr window, double xoffs, double yoffs)
        {
            MouseScroll += new Vector2 ((float) xoffs, (float) yoffs);
        }

        /// <summary>
        /// Generates the update description.
        /// </summary>
        /// <returns>The update description.</returns>
        /// <param name="deltaTime">Delta time.</param>
        public UpdateDescription GenerateUpdateDescription (float deltaTime)
        {
            UpdateDescription ud = new UpdateDescription (new List<KeyboardInput> (Keys), new List<MouseInput> (Mouse),
                                                          MouseMovement, MouseScroll, deltaTime);
            Keys.Clear ();
            Mouse.Clear ();
            MouseMovement = Vector2.Zero;
            MouseScroll = Vector2.Zero;
            return ud;
        }
    }
}
