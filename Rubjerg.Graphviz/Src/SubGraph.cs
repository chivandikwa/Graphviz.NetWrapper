﻿using System;
using System.Collections.Generic;
using System.Linq;
using static Rubjerg.Graphviz.ForeignFunctionInterface;

namespace Rubjerg.Graphviz
{
    public class SubGraph : Graph
    {
        /// <summary>
        /// rootgraph must not be null
        /// </summary>
        internal SubGraph(IntPtr ptr, RootGraph rootgraph) : base(ptr, rootgraph) { }

        internal static SubGraph Get(Graph parent, string name)
        {
            IntPtr ptr = Agsubg(parent._ptr, name, 0);
            if (ptr == IntPtr.Zero)
                return null;
            return new SubGraph(ptr, parent.MyRootGraph);
        }

        internal static SubGraph GetOrCreate(Graph parent, string name)
        {
            IntPtr ptr = Agsubg(parent._ptr, name, 1);
            return new SubGraph(ptr, parent.MyRootGraph);
        }

        public void AddExisting(Node node)
        {
            Agsubnode(_ptr, node._ptr, 1);
        }

        public void AddExisting(Edge edge)
        {
            Agsubedge(_ptr, edge._ptr, 1);
        }

        /// <summary>
        /// FIXME: use an actual subg equivalent to agsubedge and agsubnode
        /// https://github.com/ellson/graphviz/issues/1206
        /// This might cause a new subgraph creation.
        /// </summary>
        public void AddExisting(SubGraph subgraph)
        {
            Agsubg(_ptr, subgraph.GetName(), 1);
        }

        public void AddExisting(IEnumerable<Node> nodes)
        {
            foreach (var node in nodes)
                AddExisting(node);
        }

        public void AddExisting(IEnumerable<Edge> edges)
        {
            foreach (var edge in edges)
                AddExisting(edge);
        }

        public void Delete()
        {
            Agclose(_ptr);
        }

        /// <summary>
        /// Create and return a subgraph containing the given edges and their endpoints.
        /// </summary>
        public static SubGraph FromEdgeSet(Graph parent, string name, HashSet<Edge> edges)
        {
            var result = GetOrCreate(parent, name);
            result.AddExisting(edges);
            // Since subgraphs can contain edges independently of their endpoints,
            // we need to add the endpoints explicitly.
            result.AddExisting(edges.SelectMany(e => new[] { e.Tail(), e.Head() }));
            return result;
        }
    }
}
