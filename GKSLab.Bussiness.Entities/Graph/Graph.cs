﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace GKSLab.Bussiness.Entities.Graph
{
    public class Graph
    {
        public Graph()
        {
            Roots = new List<Node<string>>();
        }

        public override string ToString()
        {
            var simplifiedGraphModel = new HashSet<string>();
            foreach (var item in this.Roots)
            {
                foreach (var child in item.Children)
                {
                    simplifiedGraphModel.Add(item.Value + " "+ "->" + " " + child.Value);
                }
            }
            return string.Join(";", simplifiedGraphModel);
        }

        /// <summary>
        /// List of graph nodes 
        /// </summary>
        public List<Node<string>> Roots { get; private set; }
        /// <summary>
        /// Find node by value
        /// </summary>
        /// <param name="value">
        /// Node value
        /// </param>
        /// <returns>
        /// Return Node of Graph
        /// </returns>
        public Node<string> Find(string value)
        {
            return Roots.FirstOrDefault(x => x.Value == value);
        }
        /// <summary>
        /// Constructor of class Graph
        /// </summary>
        /// <param name="roots">
        /// Array of Nodes
        /// </param>
        public Graph(params Node<string>[] roots)
        {
            Roots.AddRange(roots);
        }

        /// <summary>
        /// Add new node to graph. 
        /// </summary>
        /// <param name="value">Value of the node.</param>
        /// <param name="children">Children of a tree</param>
        public void AddNode(string value)
        {
            var node = Find(value);
            //if node dont exist -> create it
            if (node == null)
            {
                var newNode = new Node<string>(value: value, children: new List<Node<string>>(), parents: new List<Node<string>>());
                Roots.Add(newNode);
            }
        }

        /// <summary>
        /// Add childrens to node
        /// </summary>
        /// <param name="value">Value of the node</param>
        /// <param name="children">Children of a tree</param>
        public void AddChildrens(string value, params Node<string>[] children)
        {
            var node = Find(value);

            if(node != null)
            {
             foreach (var child in children)
                {
                    //if node contain this Child we will not add child to current node
                    if (node.Children.FirstOrDefault(x => x.Value == child.Value) == null)
                    {
                        node.Children.Add(child);
                        node.Children.Find(x => x.Value == child.Value).Parents.Add(node);
                    }
                }
            }  
        }

        /// <summary>
        /// Add parents to node
        /// </summary>
        /// <param name="value">Value of the node</param>
        /// <param name="parents">Parent of a tree</param>
        public void AddParents(string value, params Node<string>[] parents)
        {
            var node = Find(value);
            if(node != null)
            { 
             foreach(var parent in parents)
                {
                    //if node contain this Parent we will not add Parent to current node
                    if (node.Parents.FirstOrDefault(x => x.Value == parent.Value) == null)
                    {
                        node.Parents.Add(parent);
                        node.Parents.Find(x => x.Value == parent.Value).Children.Add(node);
                    }
                }
            }
        }


        /// <summary>
        /// Update graph
        /// </summary>
        /// <param name="value"></param>
        /// <param name="children"></param>
        public void UpdateGraph(Graph graph, Node<string> firstNode, Node<string> secondNode)
        {
            // check amount of relations for nodes and choose node with less amount relations
            if (firstNode.Parents.Count + firstNode.Children.Count < secondNode.Parents.Count + secondNode.Children.Count)
                UnionNodes(graph, secondNode, firstNode);
            else
                UnionNodes(graph, firstNode, secondNode);
        }
        /// <summary>
        /// Deleting node
        /// </summary>
        /// <param name="graph">Grap with nodex</param>
        /// <param name="unionNode">Node that union</param>
        /// <param name="deletingNode">Node that deleting</param>
        public void UnionNodes(Graph graph, Node<string> unionNode, Node<string> deletingNode )
        {
            List<Node<string>> parents = new List<Node<string>>();
            List<Node<string>> childrens = new List<Node<string>>();
            
            //check amount elements in modules that can been less than five
            if (unionNode.CountUnion >= 6 || (unionNode.CountUnion += deletingNode.CountUnion) >= 6) return;

            // save relations(parent, children) for node, that deleting 
            // not including relations that follow to union node
            parents = deletingNode.Parents.FindAll(per => per.Value != unionNode.Value);
            childrens = deletingNode.Children.FindAll(per => per.Value != unionNode.Value);

            //delete all childrens and parents with nodes that inculude deleting node
            graph.Roots.ForEach(x => x.Parents.Remove(deletingNode));
            graph.Roots.ForEach(x => x.Children.Remove(deletingNode));

            //union deleting node and union node
            unionNode.Value += deletingNode.Value;

            // deleting node
            graph.Roots.Remove(deletingNode);

            // add new relations to union node
            childrens.ForEach(x => AddChildrens(unionNode.Value, x));
            parents.ForEach(x => AddParents(unionNode.Value, x));
        }
    }
}
