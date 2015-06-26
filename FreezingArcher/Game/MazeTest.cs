﻿//
//  MazeTest.cs
//
//  Author:
//       Fin Christensen <christensen.fin@gmail.com>
//
//  Copyright (c) 2015 Fin Christensen
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
using FreezingArcher.Messaging.Interfaces;
using FreezingArcher.Messaging;
using FreezingArcher.Game.Maze;
using FreezingArcher.Core;
using FreezingArcher.Output;
using FreezingArcher.Math;
using FreezingArcher.Renderer.Scene;
using FreezingArcher.Renderer.Scene.SceneObjects;
using FreezingArcher.Content;

namespace FreezingArcher.Game
{
    /// <summary>
    /// Maze test.
    /// </summary>
    public class MazeTest : IMessageConsumer
    {
        double f = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Game.MazeTest"/> class.
        /// </summary>
        /// <param name="msgmnr">Msgmnr.</param>
        /// <param name="objmnr">Objmnr.</param>
        /// <param name="scene">Scene.</param>
        /// <param name="game">The game the maze should be generated in.</param>
        public MazeTest (MessageManager msgmnr, ObjectManager objmnr, CoreScene scene, Content.Game game)
        {
            ValidMessages = new int[] { (int) MessageId.Input, (int) MessageId.PositionChangedMessage, (int) MessageId.Update };
            msgmnr += this;
            mazeGenerator = new MazeGenerator (objmnr);
            this.scene = scene;

            scene.CameraManager.AddCam (new FirstPersonCamera ("Player", msgmnr, default(Vector3), default(Vector3), 0.1f, 200));
            scene.CameraManager.AddCam (new FreeCamera ("Overview", msgmnr, default(Vector3), default(Vector3), 0.1f, 1000.0f));
//            scene.CameraManager.AddCam (new ThirdPersonCamera("third",msgmnr, new Vector3 (1,1,1),default(Vector3), default(Vector3)));
            BaseCamera test = scene.CameraManager.GetCam ("Player");
            scene.CameraManager.ActiveCamera = test;

            //Skybox
            skybox = EntityFactory.Instance.CreateWith ("skybox", systems: new[] { typeof(ModelSystem) });
            ModelSceneObject skyboxModel = new ModelSceneObject ("lib/Renderer/TestGraphics/Skybox/skybox.xml");
            skybox.GetComponent<TransformComponent>().Scale = 100.0f * Vector3.One;
            scene.AddObject (skyboxModel);
            skyboxModel.WaitTillInitialized ();
            skyboxModel.Model.EnableDepthTest = false;
            skyboxModel.Model.EnableLighting = false;
            skybox.GetComponent<ModelComponent> ().Model = skyboxModel;

            player = EntityFactory.Instance.CreateWith ("player", new[] { typeof(TransformComponent) }, new[] { typeof (MovementSystem) });

            int seed = new Random().Next();
            var rand = new Random(seed);
            Logger.Log.AddLogEntry(LogLevel.Debug, "MazeTest", "Seed: {0}", seed);
            maze[0] = mazeGenerator.CreateMaze(rand.Next(), game.CurrentGameState.PhysicsManager);
            maze[1] = mazeGenerator.CreateMaze(rand.Next(), game.CurrentGameState.PhysicsManager);
        }

        readonly MazeGenerator mazeGenerator;

        readonly Maze.Maze[] maze = new Maze.Maze[2];

        Entity player;

        Entity skybox;

        CoreScene scene;

        #region IMessageConsumer implementation

        /// <summary>
        /// Processes the incoming message
        /// </summary>
        /// <param name="msg">Message to process</param>
        public void ConsumeMessage (IMessage msg)
        {
            var im = msg as InputMessage;
            if (im != null)
            {
                if (im.IsActionPressed("jump"))
                {
                    if (!maze[0].IsGenerated)
                        maze[0].Generate(scene);
                    else if (!maze[1].IsGenerated)
                        maze[1].Generate();
                }
                if (im.IsActionPressed("run"))
                {
                    if (maze[0].IsGenerated && !maze[0].IsExitPathCalculated)
                        maze[0].CalculatePathToExit();
                    else if (maze[1].IsGenerated && !maze[1].IsExitPathCalculated)
                        maze[1].CalculatePathToExit();
                }
                if (im.IsActionPressed("sneek"))
                {
                    if (maze[0].IsGenerated && !maze[0].AreFeaturesPlaced)
                        maze[0].SpawnFeatures(null, maze[1].graph);
                    else if (maze[1].IsGenerated && !maze[1].AreFeaturesPlaced)
                        maze[1].SpawnFeatures(maze[0].graph);
                }
            }

            if (msg.MessageId == (int)MessageId.PositionChangedMessage)
            {
                PositionChangedMessage pcm = msg as PositionChangedMessage;
                if (pcm.Entity.Name == player.Name) 
                {
                    skybox.GetComponent<TransformComponent> ().Position = pcm.Entity.GetComponent<TransformComponent> ().Position;
                }
            }

            if (msg.MessageId == (int)MessageId.Update)
            {
                player.GetComponent<TransformComponent> ().Position = scene.CameraManager.ActiveCamera.CameraPosition;
            }

            var um = msg as UpdateMessage;
            if (um != null)
            {
                f += 0.1;
                const int fak = 1;
//                scene.CameraManager.ActiveCamera.Fov = (float)(System.Math.Sin (f) + 1) > fak * MathHelper.Pi ? fak * MathHelper.Pi : (float)(System.Math.Sin (f) + 1);
            }
        }

        /// <summary>
        /// Gets the valid messages which can be used in the ConsumeMessage method
        /// </summary>
        /// <value>The valid messages</value>
        public int[] ValidMessages { get; private set; }

        #endregion
    }
}
