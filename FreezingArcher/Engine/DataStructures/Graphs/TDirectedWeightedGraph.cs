﻿//
//  TDirectedWeightedGraph.cs
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
using FreezingArcher.Core;
using System.Collections.Generic;
using FreezingArcher.Output;

namespace FreezingArcher.DataStructures.Graphs
{
    /// <summary>
    /// Directed weighted graph.
    /// </summary>
    [TypeIdentifier (2)]
    public class DirectedWeightedGraph<TData, TWeight> : FAObject where TWeight : IComparable
    {
        /// <summary>
        /// The name of the module.
        /// </summary>
        public const string ModuleName = "DirectedWeightedGraph";

        /// <summary>
        /// Initialize the graph.
        /// </summary>
        public void Init ()
        {
            InternalEdges = new List<DirectedEdge<TData, TWeight>>();
            InternalNodes = new List<DirectedNode<TData, TWeight>>();
        }

        /// <summary>
        /// The real edges are stored here for internal use.
        /// </summary>
        protected List<DirectedEdge<TData, TWeight>> InternalEdges;

        /// <summary>
        /// The real nodes are stored here for internal use.
        /// </summary>
        protected List<DirectedNode<TData, TWeight>> InternalNodes;

        /// <summary>
        /// Get a read only collection of all registered edges.
        /// </summary>
        /// <value>The edges.</value>
        public IReadOnlyCollection<DirectedEdge<TData, TWeight>> Edges
        {
            get
            {
                return InternalEdges;
            }
        }

        /// <summary>
        /// Get a read only collection of all registered nodes.
        /// </summary>
        /// <value>The nodes.</value>
        public IReadOnlyCollection<DirectedNode<TData, TWeight>> Nodes
        {
            get
            {
                return InternalNodes;
            }
        }

        /// <summary>
        /// Add a node to this graph.
        /// </summary>
        /// <returns><c>true</c>, if node was added, <c>false</c> otherwise.</returns>
        /// <param name="data">The data the node should hold.</param>
        /// <param name="outgoingEdgeNodes">Collection of outgoing edges to be created. The pair consists of a
        /// destination node and an edge weight.</param>
        /// <param name="incomingEdgeNodes">Collection of incoming edges to be created. The pair consists of a
        /// source node and an edge weight.</param>
        public virtual bool AddNode (TData data, ICollection<Pair<DirectedNode<TData, TWeight>, TWeight>> outgoingEdgeNodes,
            ICollection<Pair<DirectedNode<TData, TWeight>, TWeight>> incomingEdgeNodes)
        {
            // create new node with object recycler
            DirectedNode<TData, TWeight> node = ObjectManager.CreateOrRecycle<DirectedNode<TData, TWeight>> (3);

            // initialize new node with data
            node.Init (data);

            // do we have outgoing edges?
            if (outgoingEdgeNodes != null && outgoingEdgeNodes.Count > 0)
            {
                foreach (var edgeNode in outgoingEdgeNodes)
                {
                    // does destination node exist? If not adding this node to the graph will fail
                    if (edgeNode.A != null)
                    {
                        Logger.Log.AddLogEntry (LogLevel.Severe, ModuleName,
                            "Failed to create edge to nonexistent node {0}, skipping...", edgeNode.A);

                        // failure
                        return false;
                    }

                    // add new edge
                    AddEdge(node, edgeNode.A, edgeNode.B);
                }
            }

            // do we have incoming edges?
            if (incomingEdgeNodes != null && incomingEdgeNodes.Count > 0)
            {
                foreach (var edgeNode in incomingEdgeNodes)
                {
                    // does source node exist? If not adding this node to the graph will fail
                    if (edgeNode.A != null)
                    {
                        Logger.Log.AddLogEntry (LogLevel.Severe, ModuleName,
                            "Failed to create edge from nonexistent node {0}", edgeNode.A);

                        // failure
                        return false;
                    }


                    // add new edge
                    AddEdge(edgeNode.A, node, edgeNode.B);
                }
            }

            // add new node to internal node list
            InternalNodes.Add (node);

            return true;
        }

        /// <summary>
        /// Remove the given node by its identifier.
        /// </summary>
        /// <returns><c>true</c>, if node was removed, <c>false</c> otherwise.</returns>
        /// <param name="node">Node identifier.</param>
        public virtual bool RemoveNode (DirectedNode<TData, TWeight> node)
        {
            // print error if remove failed
            if (InternalNodes.Remove(node))
            {
                Logger.Log.AddLogEntry (LogLevel.Warning, ModuleName, "The given Node {0} does not exist!", node);
                return false;
            }

            // remove all incoming edges associated with this node
            foreach (var edge in node.IncomingEdges)
            {
                edge.SourceNode.InternalOutgoingEdges.Remove (edge);
                InternalEdges.Remove (edge);
                edge.Destroy();
            }

            // remove all outgoing edges associated with this node
            foreach (var edge in node.OutgoingEdges)
            {
                edge.DestinationNode.InternalIncomingEdges.Remove(edge);
                InternalEdges.Remove(edge);
                edge.Destroy();
            }

            // destroy the node
            node.Destroy();

            return true;
        }

        /// <summary>
        /// Adds an edge from a given source node to a given destination node with a given edge weight.
        /// </summary>
        /// <returns><c>true</c>, if edge was added, <c>false</c> otherwise.</returns>
        /// <param name="sourceNode">The source node.</param>
        /// <param name="destinationNode">The destination node.</param>
        /// <param name="weight">The edge weight.</param>
        public virtual bool AddEdge (DirectedNode<TData, TWeight> sourceNode, DirectedNode<TData, TWeight> destinationNode,
            TWeight weight)
        {
            // fail if one of the nodes is null
            if (sourceNode == null || destinationNode == null)
            {
                Logger.Log.AddLogEntry(LogLevel.Severe, ModuleName, "Cannot create edge on null node!");
                return false;
            }

            // create new edge with object recycler
            DirectedEdge<TData, TWeight> edge = ObjectManager.CreateOrRecycle<DirectedEdge<TData, TWeight>>(4);

            // initialize edge with data
            edge.Init(weight, sourceNode, destinationNode);

            // add created edge to source and destination nodes
            sourceNode.InternalOutgoingEdges.Add (edge);
            destinationNode.InternalIncomingEdges.Add (edge);

            // add created node to graph
            InternalEdges.Add (edge);

            // everything ok
            return true;
        }

        /// <summary>
        /// Removes an edge from the graph.
        /// </summary>
        /// <returns><c>true</c>, if edge was removed, <c>false</c> otherwise.</returns>
        /// <param name="edge">The edge.</param>
        public virtual bool RemoveEdge (DirectedEdge<TData, TWeight> edge)
        {
            // fail if edge is null
            if (edge == null)
            {
                Logger.Log.AddLogEntry(LogLevel.Severe, ModuleName, "Failed to remove edge as the given edge is null!");
                return false;
            }

            // if source or destination node are null we do really have a problem
            if (edge.SourceNode == null || edge.DestinationNode == null)
            {
                Logger.Log.AddLogEntry (LogLevel.Severe, ModuleName,
                    "Detected an edge with referenced nodes that do not exist!" +
                    "This is a severe bug in the graph implementation.");
                return false;
            }

            // remove edge from source and destination nodes
            edge.SourceNode.InternalOutgoingEdges.Remove(edge);
            edge.DestinationNode.InternalIncomingEdges.Remove(edge);

            // remove edge from graph
            InternalEdges.Remove(edge);

            // destroy the edge
            edge.Destroy();
            return true;
        }
    }
}
