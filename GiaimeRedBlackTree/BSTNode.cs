using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiaimeRedBlackTree
{
    class BSTNode<TData> where TData: IComparable
    {
        private TData data;
        private BSTNode<TData> leftChild;
        private BSTNode<TData> rightChild;

        /// <summary>
        /// Constructs a BSTNode with Data of a given type. Children default to null.
        /// </summary>
        /// <param name="data">Data of type TData held by this node.</param>
        /// <param name="leftChild">Left child of this Node. 
        /// The left child should return 1 when compared to this node.</param>
        /// <param name="rightChild">Right child of this Node. 
        /// The right child should return -1 when compared to this node</param>
        public BSTNode(TData data, BSTNode<TData> leftChild = null, BSTNode<TData> rightChild = null) {
            this.data = data;
            this.leftChild = leftChild;
            this.rightChild = rightChild;
        }


        public BSTNode<TData> LeftChild
        {
            get { return leftChild; }
            set { leftChild = value; }
        }

        public BSTNode<TData> RightChild
        {
            get { return rightChild; }
            set { rightChild = value; }
        }

        public TData Data
        {
            get { return data; }
            set { data = value; }
        }

        override public String ToString() {
            return leftChild +" "+ data.ToString()  +" "+ rightChild;
        }
    }
}
