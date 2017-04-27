using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedBlackTree
{
    //This enumeration will improve the readability of this project, 
    //so instead of just a 0 or 1, we can say RED or BLACK
    enum NodeColor { BLACK = 0, RED = 1, DBLBLACK = 2}; 

    class RBTNode<TData> where TData : IComparable
    {
        
        private NodeColor color;

        private TData data;
        private RBTNode<TData> leftChild;
        private RBTNode<TData> rightChild;
        private RBTNode<TData> parent;

        /// <summary>
        /// Constructs an RBTNode with Data of a given type. Children default to null.
        /// </summary>
        /// <param name="data">Data of type TData held by this node.</param>
        /// <param name="leftChild">Left child of this Node. 
        /// The left child should return 1 when compared to this node.</param>
        /// <param name="rightChild">Right child of this Node. 
        /// The right child should return -1 when compared to this node</param>
        public RBTNode(TData data, RBTNode<TData> leftChild = null, RBTNode<TData> rightChild = null, NodeColor color = NodeColor.RED)
        {
            this.data = data;
            this.leftChild = leftChild;
            this.rightChild = rightChild;
            this.color = color;
        }


        public RBTNode<TData> LeftChild
        {
            get { return leftChild; }
            set { leftChild = value; }
        }

        public RBTNode<TData> RightChild
        {
            get { return rightChild; }
            set { rightChild = value; }
        }

        public TData Data
        {
            get { return data; }
            set { data = value; }
        }

        public RBTNode<TData> Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        override public String ToString()
        {
            return leftChild + " " + data.ToString() + Color + " " + rightChild;
        }

        public NodeColor Color {
            get { return color; }
            set { color = value; }
        }

        public void Free() {
            Console.WriteLine("Freeing " + Data.ToString());
            Data = default(TData);
            leftChild = null;
            rightChild = null;
            parent = null;
        }

    }
}
