// Skeleton implementation written by Joe Zachary for CS 3500, September 2013.
// Version 1.1 (Fixed error in comment for RemoveDependency.)
// Made into functioning project by Jack Stafford for CS 3500, September 2014

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpreadsheetUtilities
{

    /// <summary>
    /// A DependencyGraph can be modeled as a set of ordered pairs of strings.  Two ordered pairs
    /// (s1,t1) and (s2,t2) are considered equal if and only if s1 equals s2 and t1 equals t2.
    /// (Recall that sets never contain duplicates.  If an attempt is made to add an element to a 
    /// set, and the element is already in the set, the set remains unchanged.)
    /// 
    /// Given a DependencyGraph DG:
    /// 
    ///    (1) If s is a string, the set of all strings t such that (s,t) is in DG is called dependents(s).
    ///        
    ///    (2) If s is a string, the set of all strings t such that (t,s) is in DG is called dependees(s).
    ///
    /// For example, suppose DG = {("a", "b"), ("a", "c"), ("b", "d"), ("d", "d")}
    ///     dependents("a") = {"b", "c"}
    ///     dependents("b") = {"d"}
    ///     dependents("c") = {}
    ///     dependents("d") = {"d"}
    ///     dependees("a") = {}
    ///     dependees("b") = {"a"}
    ///
    ///     dependees("d") = {"b", "d"}
    /// </summary>
    public class DependencyGraph
    {
        /// <summary>
        /// Maps a string name to the corresponding Node
        /// </summary>
        private Dictionary<string, Node> graph;

        /// <summary>
        /// Creates an empty DependencyGraph.
        /// </summary>
        public DependencyGraph()
        {
            graph = new Dictionary<string, Node>();
        }


        /// <summary>
        /// The number of ordered pairs in the DependencyGraph.
        /// </summary>
        public int Size
        {
            get
            {
                int i = 0;
                foreach (Node n in graph.Values)
                    i += n.dependents.Count;
                return i;
                //return graph.Count;
            }
        }


        /// <summary>
        /// The size of dependees(s).
        /// This property is an example of an indexer.  If dg is a DependencyGraph, you would
        /// invoke it like this:
        /// dg["a"]
        /// It should return the size of dependees("a")
        /// </summary>        
        public int this[string s]
        {
            get
            {
                if (graph.ContainsKey(s))
                    return graph[s].dependees.Count;
                return 0;
            }
        }


        /// <summary>
        /// Reports whether dependents(s) is non-empty.
        /// </summary>
        public bool HasDependents(string s)
        {
            if (graph.ContainsKey(s))
                return graph[s].dependents.Count > 0;
            return false;
        }


        /// <summary>
        /// Reports whether dependees(s) is non-empty.
        /// </summary>
        public bool HasDependees(string s)
        {
            if (graph.ContainsKey(s))
                return graph[s].dependees.Count > 0;
            return false;
        }


        /// <summary>
        /// Enumerates dependents(s).
        /// </summary>
        public IEnumerable<string> GetDependents(string s)
        {
            if (graph.ContainsKey(s))
            {
                Node nodeS = graph[s];
                List<String> dependents = new List<String>();
                foreach (Node n in nodeS.dependents)
                    dependents.Add(n.name);
                return dependents;
            }
            return new List<string>();

        }

        /// <summary>
        /// Enumerates dependees(s).
        /// </summary>
        public IEnumerable<string> GetDependees(string s)
        {
            if (graph.ContainsKey(s))
            {
                Node nodeS = graph[s];
                List<String> dependees = new List<String>();
                foreach (Node n in nodeS.dependees)
                    dependees.Add(n.name);
                return dependees as IEnumerable<String>;
            }
            return new List<string>();
        }


        /// <summary>
        /// Adds the ordered pair (s,t), if it doesn't exist
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        public void AddDependency(string s, string t)
        {
            bool graphContainsS = graph.ContainsKey(s);
            bool graphContainsT = graph.ContainsKey(t);
            if (!graphContainsS)
                graph.Add(s, new Node(s));            
            if (!graphContainsT)
                graph.Add(t, new Node(t));

            Node nodeS = graph[s];
            Node nodeT = graph[t];
            nodeS.addDependent(nodeT);
            nodeT.addDependee(nodeS);
        }


        /// <summary>
        /// Removes the ordered pair (s,t), if it exists
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        public void RemoveDependency(string s, string t)
        {
            bool graphContainsS = graph.ContainsKey(s);
            bool graphContainsT = graph.ContainsKey(t);
            //if (!graphContainsS || !graphContainsT)
            //    return;

            if (!graphContainsS)
                return;

            Node nodeS = graph[s];
            Node nodeT = graph[t];
            nodeS.removeDependent(nodeT);
            nodeT.removeDependee(nodeS);
        }


        /// <summary>
        /// Removes all existing ordered pairs of the form (s,r).  Then, for each
        /// t in newDependents, adds the ordered pair (s,t).
        /// </summary>
        public void ReplaceDependents(string s, IEnumerable<string> newDependents)
        {
            if (newDependents.Count() < 1)
                return;

            if (!graph.ContainsKey(s))
                graph.Add(s, new Node(s));

            Node nodeS = graph[s];

            foreach (Node n in nodeS.dependents)
                n.removeDependee(nodeS);

            ArrayList newNodes = new ArrayList();
            foreach (string t in newDependents)
            {
                if (!graph.ContainsKey(t))
                    graph.Add(t, new Node(t));
                Node nodeT = graph[t];
                newNodes.Add(nodeT);
                nodeT.addDependee(nodeS);
            }

            nodeS.replaceDependents(newNodes);


        }


        /// <summary>
        /// Removes all existing ordered pairs of the form (r,s).  Then, for each 
        /// t in newDependees, adds the ordered pair (t,s).
        /// </summary>
        public void ReplaceDependees(string s, IEnumerable<string> newDependees)
        {
            if (newDependees.Count() < 1)
                return;

            if (!graph.ContainsKey(s))
                graph.Add(s, new Node(s));

            Node nodeS = graph[s];

            foreach (Node n in nodeS.dependees)
                n.removeDependent(nodeS);

            ArrayList newNodes = new ArrayList();
            foreach (string t in newDependees)
            {
                if (!graph.ContainsKey(t))
                    graph.Add(t, new Node(t));
                Node nodeT = graph[t];
                newNodes.Add(nodeT);
                nodeT.addDependent(nodeS);
            }

            nodeS.replaceDependees(newNodes);
        }

        /// <summary>
        /// A Node has a string name, and lists for it's dependents and dependees
        /// </summary>
        internal class Node
        {
            internal ArrayList dependees, dependents;
            internal 
            internal string name;

            /// <summary>
            /// Constructs a Node with the given name, and empty dependents and dependees.
            /// </summary>
            /// <param name="name">a string for use as the name</param>
            internal Node(string name)
            {
                this.name = name;
                dependees = new ArrayList();
                dependents = new ArrayList();
            }         

            /// <summary>
            /// If this node has a given dependent, it will be removed from the dependents
            /// </summary>
            /// <param name="dependent">Node to be added to dependents</param>
            /// <returns>bool true if dependent exists and is removed, false if else</returns>
            internal bool removeDependent(Node dependent)
            {
                if (dependents.Contains(dependent))
                {
                    dependents.Remove(dependent);
                    return true;
                }
                return false;
            }

            /// <summary>
            /// Removes all dependents
            /// </summary>
            internal void removeAllDependents()
            {
                dependents.Clear();
            }

            /// <summary>
            /// Removes given dependee if it exists, returns true if dependee removed
            /// </summary>
            /// <param name="dependee">Node for removal from dependees</param>
            /// <returns>bool true if Node removed from dependees, false if otherwise</returns>
            internal bool removeDependee(Node dependee)
            {
                if (dependees.Contains(dependee))
                {
                    dependees.Remove(dependee);
                    return true;
                }
                return false;
            }

            /// <summary>
            /// Removes all dependees
            /// </summary>
            internal void removeAllDependees()
            {
                dependees.Clear();
            }

            /// <summary>
            /// If the dependent is not already a dependent, it is added as a dependent
            /// </summary>
            /// <param name="dependent">Node to be added as dependent</param>
            internal void addDependent(Node dependent)
            {
                if (dependents.Contains(dependent))
                    return;
                dependents.Add(dependent);
            }

            /// <summary>
            /// Adds all Nodes in given list as dependents, if they are not already dependents
            /// </summary>
            /// <param name="newDependents">ArrayList of dependent Nodes to add</param>
            internal void addDependents(ArrayList newDependents)
            {
                foreach (Node n in newDependents)
                    addDependent(n);
            }

            /// <summary>
            /// Adds given dependee, if it is not already a dependee
            /// </summary>
            /// <param name="dependee">Node to be added as a dependee</param>
            internal void addDependee(Node dependee)
            {
                if (dependees.Contains(dependee))
                    return;
                dependees.Add(dependee);
            }

            /// <summary>
            /// Adds all Nodes given as dependees, if they are not already dependees
            /// </summary>
            /// <param name="newDependees">ArrayList of Nodes to add as dependees</param>
            internal void addDependees(ArrayList newDependees)
            {
                foreach (Node n in newDependees)
                    addDependee(n);
            }

            /// <summary>
            /// Removes all current dependents, and adds all given dependents.
            /// If there are duplicates in the new dependents, only one will be added.
            /// </summary>
            /// <param name="newDependents">ArrayList of Nodes to be added as dependents</param>
            internal void replaceDependents(ArrayList newDependents)
            {
                //dependents = newDependents;
                removeAllDependents();
                addDependents(newDependents);
            }

            /// <summary>
            /// Removes all current dependees, and adds all given dependees.
            /// If there are duplicates in the new dependees, only one will be added.
            /// </summary>
            /// <param name="newDependees">ArrayList of Nodes to be added as dependees</param>
            internal void replaceDependees(ArrayList newDependees)
            {
                //dependees = newDependees;
                removeAllDependees();
                addDependees(newDependees);
            }
        }
    }




}


