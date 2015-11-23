﻿using System.Collections.Generic;
using System.Linq;
using GKSLab.Bussiness.Entities.Graph;

namespace GKSLab.Bussiness.Logic.Graph_Manager
{
    /// <summary>
    /// Graph manager. Represents a class for working with Graph
    /// </summary>
    public static class GraphManager
    {
        /// <summary>
        /// Find children for element in row.
        /// </summary>
        /// <param name="graph">Operating graph. Implement Graph</param>
        /// <param name="row">Row of group</param>
        /// <param name="element">Element of row</param>
        /// <returns>Return children of element</returns>
        private static Node<string> FindChildNodeInRow(Graph graph, IList<string> row, string element)
        {
            var elementPosition = row.IndexOf(element);
            string childValue;
            if (elementPosition >= row.Count - 1)
                childValue = string.Empty;
            else// It's obviously that last element in row doesn't have child
                childValue = row.ElementAt(elementPosition + 1);
            return graph.Find(childValue);
        }

        /// <summary>
        /// Create graph. Return implementation of Graph
        /// </summary>
        /// <param name="groups">Array of operation numbers.For example [1,2,5,4]</param>
        /// <param name="operations">Arrays of operations</param>
        /// <returns>Return implementation of class Graph</returns>
        public static Graph Create(List<int> groups, List<List<string>> operations)
        {
            var graph = new Graph();
            //Add new elements to graph
            foreach (var groupN in groups)
            {
                var itemList = operations.ElementAt(groupN);
                //just adding all nodes 
                foreach (var item in itemList)
                {
                    graph.AddNode(item);
                }
                //Updating all nodes. Adding children to existing nodes
                foreach (var item in itemList)
                {
                    //item is parent node, childNode - it's child node
                    var childNode = FindChildNodeInRow(graph, itemList, item);
                    if (childNode != null)
                        graph.AddChildrens(item, childNode);
                }
            }
            return graph;
        }
        
        /// <summary>
        /// Create modules in graph
        /// </summary>
        /// <param name="graph"></param>
        public static HashSet<string> CreateModules(Graph graph, HashSet<string> model)
        {
            int amountNodesGraph;
            model.Add(graph.ToString());
            //wi'll take only not the same elements

            do
            {
                amountNodesGraph = graph.Roots.Count;
                
                FirstCasePack(graph);
                model.Add(graph.ToString());

                SecondPack(graph);
                model.Add(graph.ToString());

                StrongConnection(graph);
                model.Add(graph.ToString());

                FindCycleInGraph(graph);
                model.Add(graph.ToString());

                FindFifthCaseInGraph(graph);
                model.Add(graph.ToString());

            } while (amountNodesGraph != graph.Roots.Count);

            

            return model;
        }

        /// <summary>
        /// Find node that don`t have any Parents
        /// </summary>
        /// <param name="graph">Implementation of class graph</param>
        public static void FirstCasePack(Graph graph)
        {
            foreach (var item in graph.Roots.Where(item => item.HasChildren && !item.HasParrents))
            {
                item.Type = NodeType.Module;
            }
        }

        /// <summary>
        /// Find node that don`t have any Children
        /// </summary>
        /// <param name="graph">Implementation of class graph<</param>
        public static void SecondPack(Graph graph)
        {
            foreach (var item in graph.Roots.Where(item => item.HasParrents && !item.HasChildren))
            {
                item.Type = NodeType.Module;
            }
        }

        /// <summary>
        /// Find strong connection  between nodes in graph
        /// </summary>
        /// <param name="graph">Implementation of class graph</param>
        public static void StrongConnection(Graph graph)
        {
            //find all nodes that have equal element in childrens and parents
            foreach (var item in graph.Roots.Where(item => item.HasChildren && item.HasParrents))
            {
                var findNode = item.Parents.Find(x => item.Children.Contains(x));
                if (findNode != null)
                { 
                    graph.UpdateGraph(graph, findNode, item);
                    break;
                }
            }
        }

        /// <summary>
        /// Find cycle in graph
        /// </summary>
        /// <param name="graph">Implementation of class graph</param>
        public static void FindCycleInGraph(Graph graph)
        {
            List<Node<string>> catalogeCycle = new List<Node<string>>();
            SearchInDepthCycle searchCycle = new SearchInDepthCycle();
            catalogeCycle = searchCycle.FindCycle(graph);

            // Count-2 because final element in cycle is a first element
            for(int i = 0; i < catalogeCycle.Count-2; i++)
            {
                graph.UnionNodes(graph, catalogeCycle[0], catalogeCycle[i + 1]);
            }
        }

        /// <summary>
        /// Find fifth case in graph
        /// </summary>
        /// <param name="graph">Implementation of class graph</param>
        public static void FindFifthCaseInGraph(Graph graph)
        {
            List<Node<string>> catalogeFifthCase = new List<Node<string>>();
            SearchInDepthFifthCase fifthCase = new SearchInDepthFifthCase();
            catalogeFifthCase = fifthCase.FindFifthCase(graph);

            for (int i = 0; i < catalogeFifthCase.Count - 1; i++)
            {
                graph.UnionNodes(graph, catalogeFifthCase[0], catalogeFifthCase[i + 1]);
            }
        }
    }
}
