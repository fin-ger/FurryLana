//
//  Game.cs
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
using System;
using System.Linq;
using System.Collections.Generic;
using FreezingArcher.Core.Interfaces;
using FreezingArcher.Input;
using FreezingArcher.Output;
using FreezingArcher.DataStructures.Graphs;
using FreezingArcher.Core;
using FreezingArcher.Renderer.Scene;
using System.Drawing;

namespace FreezingArcher.Content
{
    /// <summary>
    /// Game.
    /// </summary>
    public sealed class Game : IManageable
    {
        /// <summary>
        /// The name of the class.
        /// </summary>
        public static readonly string ClassName = "Game";

        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Content.Game"/> class.
        /// </summary>
        /// <param name="name">Name.</param>
        public Game (string name, ObjectManager objmnr)
        {
            Logger.Log.AddLogEntry (LogLevel.Info, ClassName, "Creating new game '{0}'", name);
            Name = name;
            GameStateGraph = objmnr.CreateOrRecycle<DirectedWeightedGraph<GameState, GameStateTransition>>();
            GameStateGraph.Init();
        }

        /// <summary>
        /// Gets the state of the current game.
        /// </summary>
        /// <value>The state of the current game.</value>
        public GameState CurrentGameState
        {
            get
            {
                return currentNode.Data;
            }
        }

        DirectedWeightedNode<GameState, GameStateTransition> currentNode;

        /// <summary>
        /// Gets the game state graph.
        /// </summary>
        /// <value>The game state graph.</value>
        public DirectedWeightedGraph<GameState, GameStateTransition> GameStateGraph { get; private set; }

        /// <summary>
        /// Switch to the given game state.
        /// </summary>
        /// <returns><c>true</c>, if successfully switched game state, <c>false</c> otherwise.</returns>
        /// <param name="name">Game state name.</param>
        public bool SwitchToGameState(string name)
        {
            var newstate = currentNode.OutgoingEdges.FirstOrDefault(e =>
                e.DestinationNode.Data.Name == name).DestinationNode;

            if (newstate == null)
            {
                if (!GameStateGraph.Nodes.Any(n => n.Data.Name == name))
                {
                    Logger.Log.AddLogEntry(LogLevel.Error, ClassName,
                        "There is no game state '{0}' registered in this game!", name);
                    return false;
                }

                Logger.Log.AddLogEntry(LogLevel.Warning, ClassName,
                    "The game state '{0}' is not reachable from the current game state!", name);
                return false;
            }
            currentNode = newstate;
            return true;
        }

        /// <summary>
        /// Removes the state of the game.
        /// </summary>
        /// <returns><c>true</c>, if game state was removed, <c>false</c> otherwise.</returns>
        /// <param name="name">Name.</param>
        public bool RemoveGameState(string name)
        {
            var node = GameStateGraph.Nodes.FirstOrDefault(n => n.Data.Name == name);

            if (node != null)
                return GameStateGraph.RemoveNode(node);
            
            return false;
        }

        /// <summary>
        /// Adds the state of the game.
        /// </summary>
        /// <returns><c>true</c>, if game state was added, <c>false</c> otherwise.</returns>
        /// <param name="name">Name.</param>
        /// <param name="env">Env.</param>
        /// <param name="scene">Scene.</param>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        public bool AddGameState(string name, Environment env, CoreScene scene,
            IEnumerable<Tuple<string, GameStateTransition>> from = null,
            IEnumerable<Tuple<string, GameStateTransition>> to = null)
        {
            return AddGameState(new GameState(name, env, scene), from, to);
        }

        /// <summary>
        /// Adds the state of the game.
        /// </summary>
        /// <returns><c>true</c>, if game state was added, <c>false</c> otherwise.</returns>
        /// <param name="gameState">Game state.</param>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        public bool AddGameState(GameState gameState, IEnumerable<Tuple<string, GameStateTransition>> from = null,
            IEnumerable<Tuple<string, GameStateTransition>> to = null)
        {
            if (from == null && to == null)
            {
                GameStateGraph.AddNode(gameState);
            }
            else if (from == null && to != null)
            {
                var outgoing = new List<Pair<DirectedWeightedNode<GameState, GameStateTransition>, GameStateTransition>>();
                foreach (var t in to)
                {
                    var node = GameStateGraph.Nodes.FirstOrDefault(n => n.Data.Name == t.Item1);
                    if (node != null)
                    {
                        var trans = t.Item2 ?? GameStateTransition.DefaultTransition;
                        outgoing.Add(
                            new Pair<DirectedWeightedNode<GameState, GameStateTransition>, GameStateTransition>(
                                node, trans));
                    }
                }

                GameStateGraph.AddNode(gameState, outgoing);
            }
            else if (from != null && to == null)
            {
                var incoming = new List<Pair<DirectedWeightedNode<GameState, GameStateTransition>, GameStateTransition>>();
                foreach (var t in from)
                {
                    var node = GameStateGraph.Nodes.FirstOrDefault(n => n.Data.Name == t.Item1);

                    if (node != null)
                    {
                        var trans = t.Item2 ?? GameStateTransition.DefaultTransition;
                        incoming.Add(
                            new Pair<DirectedWeightedNode<GameState, GameStateTransition>, GameStateTransition>(
                                node, trans));
                    }
                }

                GameStateGraph.AddNode(gameState, null, incoming);
            }
            else if (from != null && to != null)
            {
                var outgoing = new List<Pair<DirectedWeightedNode<GameState, GameStateTransition>, GameStateTransition>>();
                foreach (var t in to)
                {
                    var node = GameStateGraph.Nodes.FirstOrDefault(n => n.Data.Name == t.Item1);
                    if (node != null)
                    {
                        var trans = t.Item2 ?? GameStateTransition.DefaultTransition;
                        outgoing.Add(
                            new Pair<DirectedWeightedNode<GameState, GameStateTransition>, GameStateTransition>(
                                node, trans));
                    }
                }

                var incoming = new List<Pair<DirectedWeightedNode<GameState, GameStateTransition>, GameStateTransition>>();
                foreach (var t in from)
                {
                    var node = GameStateGraph.Nodes.FirstOrDefault(n => n.Data.Name == t.Item1);

                    if (node != null)
                    {
                        var trans = t.Item2 ?? GameStateTransition.DefaultTransition;
                        incoming.Add(
                            new Pair<DirectedWeightedNode<GameState, GameStateTransition>, GameStateTransition>(
                                node, trans));
                    }
                }

                GameStateGraph.AddNode(gameState, outgoing, incoming);
            }
            else
            {
                Logger.Log.AddLogEntry(LogLevel.Severe, ClassName, Status.UnreachableLineReached);
                return false;
            }
            return true;
        }

        #region IManageable implementation

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        #endregion
    }
}
