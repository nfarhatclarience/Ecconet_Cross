using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Reflection;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace ESG.ExpressionLib.DataModels
{
    /// <summary>
    /// The ComponentTreeNodeCollection class for a node's child nodes.
    /// </summary>
    [JsonArray("Components")]
    public class ComponentTreeNodeCollection : ICollection<ComponentTreeNode>
    {
        #region Enumerator
        // Defines the enumerator for the Boxes collection.
        // (Some prefer this class nested in the collection class.)
        public class ComponentTreeNodeCollectionEnumerator : IEnumerator<ComponentTreeNode>
        {
            private ComponentTreeNodeCollection _collection;
            private int curIndex;
            private ComponentTreeNode currentNode;


            public ComponentTreeNodeCollectionEnumerator(ComponentTreeNodeCollection collection)
            {
                _collection = collection;
                curIndex = -1;
                currentNode = default(ComponentTreeNode);

            }

            public bool MoveNext()
            {
                //Avoids going beyond the end of the collection.
                if (++curIndex >= _collection.Count)
                {
                    return false;
                }
                else
                {
                    // Set current box to next item in collection.
                    currentNode = _collection[curIndex];
                }
                return true;
            }

            public void Reset() { curIndex = -1; }

            void IDisposable.Dispose() { }

            public ComponentTreeNode Current
            {
                get { return currentNode; }
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }
        }
        #endregion

        /// <summary>
        /// The parent node of this collection.
        /// </summary>
        public ComponentTreeNode ParentNode { get; set; }

        /// <summary>
        /// The inner collection to store child ComponentTreeNodes
        /// </summary>
        private List<ComponentTreeNode> innerCol;


        /// <summary>
        /// Constructor.
        /// </summary>
        public ComponentTreeNodeCollection()
        {
            innerCol = new List<ComponentTreeNode>();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public ComponentTreeNodeCollection(ComponentTreeNode parentNode)
        {
            ParentNode = parentNode;
            innerCol = new List<ComponentTreeNode>();
        }

        // enumeration
        public IEnumerator<ComponentTreeNode> GetEnumerator()
        {
            return new ComponentTreeNodeCollectionEnumerator(this);
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new ComponentTreeNodeCollectionEnumerator(this);
        }

        //  adds an index to the collection.
        public ComponentTreeNode this[int index]
        {
            get { return innerCol[index]; }
            set { innerCol[index] = value; }
        }

        // Determines if an item is in the collection
        // by using the BoxSameDimensions equality comparer.
        public bool Contains(ComponentTreeNode item)
        {
            bool found = false;

            foreach (var componentTreeNode in innerCol)
            {
                // Equality defined by the ComponentTreeNode
                // class's implmentation of IEquitable<T>.
                if (componentTreeNode.Equals(item))
                {
                    found = true;
                }
            }

            return found;
        }

        // Determines if an item is in the 
        // collection by using a specified equality comparer.
        public bool Contains(ComponentTreeNode item, EqualityComparer<ComponentTreeNode> comp)
        {
            bool found = false;

            foreach (var componentTreeNode in innerCol)
            {
                if (comp.Equals(componentTreeNode, item))
                {
                    found = true;
                }
            }

            return found;
        }

        /// <summary>
        /// Adds a node to the collection, setting its parent.
        /// </summary>
        /// <param name="item">The component tree node item to add.</param>
        public void Add(ComponentTreeNode item)
        {
            if (ParentNode == null)
                throw new Exception("Attempting to add node, but ComponentTreeNodeCollection parent node is null.");

            //  add the item to the collection
            item.ParentNode = ParentNode;
            if (!Contains(item))
                innerCol.Add(item);
        }

        /// <summary>
        /// Adds a node to the collection, setting its parent.
        /// </summary>
        /// <param name="item">The component tree node item to add.</param>
        public void AddRange(ComponentTreeNode[] items)
        {
            if (ParentNode == null)
                throw new Exception("Attempting to add nodes, but ComponentTreeNodeCollection parent node is null.");

            foreach (var item in items)
            {
                //  add the item to the collection
                item.ParentNode = ParentNode;
                if (!Contains(item))
                    innerCol.Add(item);
            }
        }

        /// <summary>
        /// Clears the collection.
        /// </summary>
        public void Clear()
        {
            innerCol.Clear();
        }

        /// <summary>
        /// Copies the collection to the given array.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(ComponentTreeNode[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException("The array cannot be null.");
            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException("The starting array index cannot be negative.");
            if (Count > array.Length - arrayIndex + 1)
                throw new ArgumentException("The destination array has fewer elements than the collection.");

            for (int i = 0; i < innerCol.Count; i++)
            {
                array[i + arrayIndex] = innerCol[i];
            }
        }

        public int Count
        {
            get
            {
                return innerCol.Count;
            }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(ComponentTreeNode item)
        {
            bool removed = false;
            for (int i = 0; i < innerCol.Count; i++)
            {
                if (item.Equals(innerCol[i]))
                {
                    innerCol.RemoveAt(i);
                    removed = true;
                    break;
                }
            }
            return removed;
        }

    }
}




#if GENERIC_COLLECTION_FOR_COMPONENT_TREE_NODE_TYPES

    /// <summary>
    /// The ComponentTreeNodeCollection class for a node's child nodes.
    /// </summary>
    public class ComponentTreeNodeCollection<T> : ICollection<T>
    {
        #region Enumerator
        // Defines the enumerator for the Boxes collection.
        // (Some prefer this class nested in the collection class.)
        public class ComponentTreeNodeCollectionEnumerator : IEnumerator<T>
        {
            private ComponentTreeNodeCollection<T> _collection;
            private int curIndex;
            private T curBox;


            public ComponentTreeNodeCollectionEnumerator(ComponentTreeNodeCollection<T> collection)
            {
                _collection = collection;
                curIndex = -1;
                curBox = default(T);

            }

            public bool MoveNext()
            {
                //Avoids going beyond the end of the collection.
                if (++curIndex >= _collection.Count)
                {
                    return false;
                }
                else
                {
                    // Set current box to next item in collection.
                    curBox = _collection[curIndex];
                }
                return true;
            }

            public void Reset() { curIndex = -1; }

            void IDisposable.Dispose() { }

            public T Current
            {
                get { return curBox; }
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }
        }
        #endregion

        /// <summary>
        /// The parent node of this collection.
        /// </summary>
        public ComponentTreeNode ParentNode { get; set; }

        /// <summary>
        /// The inner collection to store child ComponentTreeNodes
        /// </summary>
        private List<T> innerCol;

        /// <summary>
        /// Constructor.
        /// </summary>
        public ComponentTreeNodeCollection()
        {
            innerCol = new List<T>();
        }


        /// <summary>
        /// Constructor.
        /// </summary>
        public ComponentTreeNodeCollection(ComponentTreeNode parentNode)
        {
            ParentNode = parentNode;
            innerCol = new List<T>();
        }

        // enumeration
        public IEnumerator<T> GetEnumerator()
        {
            return new ComponentTreeNodeCollectionEnumerator(this);
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new ComponentTreeNodeCollectionEnumerator(this);
        }

        //  adds an index to the collection.
        public T this[int index]
        {
            get { return innerCol[index]; }
            set { innerCol[index] = value; }
        }

        // Determines if an item is in the collection
        // by using the BoxSameDimensions equality comparer.
        public bool Contains(T item)
        {
            bool found = false;

            foreach (T ioTreeNode in innerCol)
            {
                // Equality defined by the ComponentTreeNode
                // class's implmentation of IEquitable<T>.
                if (ioTreeNode.Equals(item))
                {
                    found = true;
                }
            }

            return found;
        }

        // Determines if an item is in the 
        // collection by using a specified equality comparer.
        public bool Contains(T item, EqualityComparer<T> comp)
        {
            bool found = false;

            foreach (T bx in innerCol)
            {
                if (comp.Equals(bx, item))
                {
                    found = true;
                }
            }

            return found;
        }

        /// <summary>
        /// Adds a node to the collection, setting its parent.
        /// </summary>
        /// <param name="item">The component tree node item to add.</param>
        public void Add(T item)
        {
            //  try to add parent node
            ComponentTreeNode ctn = item as ComponentTreeNode;
            if (ctn != null)
            {
                if (ParentNode == null)
                    throw new Exception("Attempting to add node, but ComponentTreeNodeCollection parent node is null.");
                ctn.ParentNode = ParentNode;
            }

            //  add the item to the collection
            if (!Contains(item))
                innerCol.Add(item);
        }

        /// <summary>
        /// Adds a node to the collection, setting its parent.
        /// </summary>
        /// <param name="item">The component tree node item to add.</param>
        public void AddRange(T[] items)
        {
            foreach (var item in items)
            {
                //  try to add parent node
                ComponentTreeNode ctn = item as ComponentTreeNode;
                if (ctn != null)
                {
                    if (ParentNode == null)
                        throw new Exception("Attempting to add node, but ComponentTreeNodeCollection parent node is null.");
                    ctn.ParentNode = ParentNode;
                }

                //  add the item to the collection
                if (!Contains(item))
                    innerCol.Add(item);
            }
        }

        /// <summary>
        /// Clears the collection.
        /// </summary>
        public void Clear()
        {
            innerCol.Clear();
        }

        /// <summary>
        /// Copies the collection to the given array.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException("The array cannot be null.");
            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException("The starting array index cannot be negative.");
            if (Count > array.Length - arrayIndex + 1)
                throw new ArgumentException("The destination array has fewer elements than the collection.");

            for (int i = 0; i < innerCol.Count; i++)
            {
                array[i + arrayIndex] = innerCol[i];
            }
        }

        public int Count
        {
            get
            {
                return innerCol.Count;
            }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(T item)
        {
            bool removed = false;
            for (int i = 0; i < innerCol.Count; i++)
            {
                if (item.Equals(innerCol[i]))
                {
                    innerCol.RemoveAt(i);
                    removed = true;
                    break;
                }
            }
            return removed;
        }

#endif

