﻿//
//  MyFirstFreezingArcherCam.cs
//
//  Author:
//       dboeg <${AuthorEmail}>
//
//  Copyright (c) 2015 dboeg
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
using System;

using FreezingArcher.Math;
using FreezingArcher.Messaging;
using FreezingArcher.Input;
using FreezingArcher.Core;
using FreezingArcher.Output;
using FreezingArcher.Messaging.Interfaces;

namespace FreezingArcher.Game
{

    /*
	 * Dies wird deine allererste Kamera... 
	 * Hier sollst du dich mit dem aktuellen FreezingArcher auseinandersetzen,
	 * das heißt, du wirst UNSERE API benutzen und kennenlernen.
	 * 
	 * Die erste Aufgabe ist eigentlich ganz einfach... 
	 * Hier sollst du eine simple Kamera schreiben, die du nach links, rechts, oben, unten,
	 * hinten und vorne verschieben kannst. Dabei sollst du auf Tastendrücke reagieren, und diese
	 * richtig verarbeiten. Wie du das genau tust, dass erfährst du in den Docs.
	 * Fin hat da gute Arbeit geleistet
	 * 
	 * Hinweis: Größe des Fensters erhälst du auch über Fins MessagingAPI
	 * Achtung: Da gibt es einen fiesen gemeinen BUG, aber das muss sich fin angucken
	 */

    /// <summary>
    /// My first freezing archer cam.
    /// </summary>
    public class MyFirstFreezingArcherCam : FreezingArcher.Renderer.Scene.ICamera, IMessageConsumer
    {
        /// <summary>
        /// Gets the valid messages which can be used in the ConsumeMessage method
        /// </summary>
        /// <value>The valid messages</value>
        public int[] ValidMessages { get; protected set; }

        Vector3 cameraPosition { get; set; }

        Vector3 currentRotation { get; set; }

        float zNear { get; set;}
        float zFar { get; set;}
        float fovY { get; set;}

        const float fak = 0.1f;

        Vector3 cameraReference;
        Vector3 transformedReference;
        Vector3 cameraLookat;

        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Game.MyFirstFreezingArcherCam"/> class.
        /// </summary>
        /// <param name="mssgmngr">Mssgmngr.</param>
        /// <param name="_cameraPosition">Camera position.</param>
        /// <param name="_currentRotation">Current rotation.</param>
        /// <param name="near">Near.</param>
        /// <param name="far">Far.</param>
        /// <param name="fov">Fov.</param>
        public MyFirstFreezingArcherCam (MessageManager mssgmngr, Vector3 _cameraPosition = default(Vector3),
            Vector3 _currentRotation = default(Vector3), float near = 0.1f, float far = 100.0f,
            float fov = (float)System.Math.PI / 4.0f)
        {
            cameraPosition = _cameraPosition;
            currentRotation = _currentRotation;
            cameraReference = new Vector3 (0, 0, -1);
            zNear = near;
            zFar = far;
            fovY = fov;

            ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView (fovY,
                (float)Application.Instance.RendererContext.ViewportSize.X / (float)Application.Instance.RendererContext.ViewportSize.Y,
                zNear, zFar); 
            
            UpdateCamera ();
            Logger.Log.AddLogEntry (LogLevel.Debug, "CAM", Status.Computing);
            ValidMessages = new int[] { (int)MessageId.Input, (int) MessageId.WindowResizeMessage };
            mssgmngr += this;
        }

        /// <summary>
        /// Gets or sets the projection matrix.
        /// </summary>
        /// <value>The projection matrix.</value>
        public Matrix ProjectionMatrix { get; protected set; }

        /// <summary>
        /// Gets or sets the view matrix.
        /// </summary>
        /// <value>The view matrix.</value>
        public Matrix ViewMatrix { get; protected set; }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
        public int Width { get; set; }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>The height.</value>
        public int Height { get; set; }

        private void UpdateCamera ()
        {
            float sinx = (float) System.Math.Sin (currentRotation.X);
            float cosx = (float) System.Math.Cos (currentRotation.X);
            float siny = (float) System.Math.Sin (currentRotation.Y);
            float cosy = (float) System.Math.Cos (currentRotation.Y);
            float sinz = (float) System.Math.Sin (currentRotation.Z);
            float cosz = (float) System.Math.Cos (currentRotation.Z);

            transformedReference = Vector3.UnitX;

            cameraLookat = cameraPosition + transformedReference;

            ViewMatrix = Matrix.LookAt (cameraPosition, cameraLookat, Vector3.UnitY);

            ViewMatrix *= Matrix.CreateFromQuaternion (new Quaternion (sinx, 0, 0, cosx) *
                new Quaternion (0, siny, 0, cosy) *
                new Quaternion (0, 0, sinz, cosz));
        }

        /// <summary>
        /// Rotates the x.
        /// </summary>
        /// <param name="_rotation">Rotation.</param>
        public void rotateX (float _rotation)
        {
            var tmp = currentRotation;
            tmp.X += _rotation;
            currentRotation = tmp;
            UpdateCamera ();
//            Logger.Log.AddLogEntry (LogLevel.Debug, "Rotate X", Status.Computing);
        }

        /// <summary>
        /// Rotates the y.
        /// </summary>
        /// <param name="_rotation">Rotation.</param>
        public void rotateY (float _rotation)
        {
            var tmp = currentRotation;
            tmp.Y += _rotation;
            currentRotation = tmp;
            UpdateCamera ();
//            Logger.Log.AddLogEntry (LogLevel.Debug, "Rotate Y", Status.Computing);
        }

        /// <summary>
        /// Rotates the z.
        /// </summary>
        /// <param name="_rotation">Rotation.</param>
        public void rotateZ (float _rotation)
        {
            var tmp = currentRotation;
            tmp.Z += _rotation;
            currentRotation = tmp;
            UpdateCamera ();
//            Logger.Log.AddLogEntry (LogLevel.Debug, "Rotate Z", Status.Computing);
        }

        /// <summary>
        /// Moves the x.
        /// </summary>
        /// <param name="_position">Posotion.</param>
        public void moveX (float _position)
        {
            var tmp = cameraPosition;
            tmp.X += _position;
            cameraPosition = tmp;
            UpdateCamera ();
//            Logger.Log.AddLogEntry (LogLevel.Debug, "MoveX", Status.Computing);
        }

        /// <summary>
        /// Moves the y.
        /// </summary>
        /// <param name="_position">Posotion.</param>
        public void moveY (float _position)
        {
            var tmp = cameraPosition;
            tmp.Y += _position;
            cameraPosition = tmp;
            UpdateCamera ();
//            Logger.Log.AddLogEntry (LogLevel.Debug, "MoveY", Status.Computing);
        }

        /// <summary>
        /// Moves the z.
        /// </summary>
        /// <param name="_position">Posotion.</param>
        public void moveZ (float _position)
        {
            var tmp = cameraPosition;
            tmp.Z += _position;
            cameraPosition = tmp;
            UpdateCamera ();
//            Logger.Log.AddLogEntry (LogLevel.Debug, "MoveZ", Status.Computing);
        }

        /// <summary>
        /// Updates the projection matrix.
        /// </summary>
        void UpdateProjectionMatrix (WindowResizeMessage msg)
        {
            ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView (fovY, 
                (float)msg.Width / (float)msg.Height, zNear, zFar); 
        }

        /// <summary>
        /// Consumes the message.
        /// </summary>
        /// <param name="msg">Message.</param>
        public virtual void ConsumeMessage (Messaging.Interfaces.IMessage msg)
        {
            InputMessage im = msg as InputMessage;
            if (im != null) {
                if (im.IsActionDown ("left")) {
                    moveX (-1 * fak);
                }

                if (im.IsActionDown ("right")) {
                    moveX (1 * fak);
                }

                if (im.IsActionDown ("forward")) {
                    moveZ (-1 * fak);
                }

                if (im.IsActionDown ("backward")) {
                    moveZ (1 * fak);
                }

                if (im.IsActionDown ("up")) {
                    moveY (1 * fak);
                }

                if (im.IsActionDown ("down")) {
                    moveY (-1 * fak);
                }
                if (im.IsActionDown ("sneek") && im.IsActionDown ("left")) {
                    rotateX (-1 * fak);
                }

                if (im.IsActionDown ("sneek") && im.IsActionDown ("right")) {
                    rotateX (1 * fak);
                }

                if (im.IsActionDown ("sneek") && im.IsActionDown ("forward")) {
                    rotateZ (-1 * fak);
                }

                if (im.IsActionDown ("sneek") && im.IsActionDown ("backward")) {
                    rotateZ (1 * fak);
                }

                if (im.IsActionDown ("sneek") && im.IsActionDown ("up")) {
                    rotateY (1 * fak);
                }

                if (im.IsActionDown ("sneek") && im.IsActionDown ("down")) {
                    rotateY (-1 * fak);
                }
            }

            WindowResizeMessage wrm = msg as WindowResizeMessage;
            if (wrm != null)
            {
                UpdateProjectionMatrix (wrm);
            }
        }
    }
}

