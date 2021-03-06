﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedBlackTree
{
    class RedBlackTree<TData> where TData : IComparable
    {

        private RBTNode<TData> root;

        public RedBlackTree(RBTNode<TData> root = null)
        {
            this.root = root;
        }

        public RBTNode<TData> Root
        {
            get { return root; }
            set { root = value; }
        }


        /// <summary>
        /// Adds a node to the tree. 
        /// Basic BST insertion, then calls RebalanceInsert.   
        /// </summary>
        /// <param name="nodeToAdd">Node to be added to the tree.</param>
        /// <param name="currNode">Node currently being examined. 
        /// This defaults to the root.</param>
        /// <returns>True if Node was added, false otherwise.</returns>
        public bool Add(RBTNode<TData> nodeToAdd, RBTNode<TData> currNode = null)
        {
            if (root == null)
            {
                nodeToAdd.Color = NodeColor.BLACK;
                root = nodeToAdd;
                return true;
            }
            else
            {
                if (currNode == null)
                {
                    currNode = root;
                }
                if (nodeToAdd.Data.CompareTo(currNode.Data) == -1)
                {
                    if (currNode.LeftChild == null)
                    {
                        currNode.LeftChild = nodeToAdd;
                        nodeToAdd.Parent = currNode;
                        Console.WriteLine(nodeToAdd.Data + 
                            " is now the left child of " + currNode.Data);
                        RebalanceInsert(nodeToAdd.Parent);
                        return true;
                    }
                    else
                    {
                        return Add(nodeToAdd, currNode.LeftChild);
                    }
                }
                else if (nodeToAdd.Data.CompareTo(currNode.Data) == 1)
                {
                    if (currNode.RightChild == null)
                    {
                        currNode.RightChild = nodeToAdd;
                        nodeToAdd.Parent = currNode;
                        Console.WriteLine(nodeToAdd.Data + 
                            " is now the right child of " + currNode.Data);
                        RebalanceInsert(nodeToAdd.Parent);
                        return true;
                    }
                    else
                    {
                        return Add(nodeToAdd, currNode.RightChild);
                    }
                }
                else
                {
                    RebalanceInsert(nodeToAdd.Parent);
                    return false;
                }
            }
        }

        /// <summary>
        /// Should be run post addition.
        /// Rebalances the tree from the parent of the added node.
        /// </summary>
        /// <param name="currNode">The parent of the node that was added.</param>
        private void RebalanceInsert(RBTNode<TData> currNode)
        {
            Console.WriteLine("Rebalancing!");

            /*Keeps the root black
             *This should only happen if we get to the top or in early cases
             *Since the nodes above the root are "null", 
             *we can simply have this null case 
             *to make the root black at the end*/
            if (root.Color == NodeColor.RED && currNode != root)
            {
                root.Color = NodeColor.BLACK;
            }
            else if (currNode == root)
            {
                if (currNode.Color == NodeColor.RED)
                {
                    if (currNode.RightChild.Color == NodeColor.RED)
                    {
                        Right(currNode);
                        /* In this scenario, after this Rebalance, 
                         * there is nothing left to rebalance
                         * So, we call rebalance on null, 
                         * which will handle the root being red
                         * (if it is red), then end the rebalancing cycle.
                         */
                        RebalanceInsert(null);
                    }
                    else if (currNode.LeftChild.Color == NodeColor.RED)
                    {
                        Left(currNode);
                        /* In this scenario, after this Rebalance, 
                         * there is nothing left to rebalance
                         * So, we call rebalance on null, 
                         * which will handle the root being red
                         * (if it is red), then end the rebalancing cycle.
                         */
                        RebalanceInsert(null);
                    }
                }
            }
            else if (currNode == null)
            {
                //This is how we escape! 
                //This means we have rebalanced everything and the root is black.
                return;
            }
            else
            {

                //-1 if right child, 1 if left child
                int comp = currNode.Parent.Data.CompareTo(currNode.Data);

                /*If the uncle of the added node is red, 
                 *a promotion is possible.
                 *This boolean tells us whether or 
                 *not the uncle of the added node is red, 
                 *telling us if a promotion is possible.*/
                bool promotion;
                if (currNode.Parent.RightChild != null 
                    && currNode.Parent.LeftChild != null)
                {
                    promotion = (comp == 1) ? 
                        (currNode.Parent.RightChild.Color == NodeColor.RED) 
                        : (currNode.Parent.LeftChild.Color == NodeColor.RED);
                }
                else
                {
                    promotion = false;
                }

                //If the parent of the added node isn't red, 
                //we don't need to do anything
                if (currNode.Color == NodeColor.RED)
                {
                    Console.WriteLine("I, " + currNode.Data + " am red!");
                    //If my leftChild is red, I need to fix that!
                    if (currNode.LeftChild != null 
                        && currNode.LeftChild.Color == NodeColor.RED)
                    {
                        Console.WriteLine("I, " + currNode.Data 
                            + ", see that my left child is red!");
                        if (promotion)
                        {
                            //My brother and child are red! 
                            //A promotion is necessary!
                            Promotion(currNode);
                            try
                            {
                                /*If there exists a grandpa, 
                                 * we rebalance from there.
                                 *If not, we have reached the top of the tree, 
                                 *so we call RebalanceInsert on null.
                                 */
                                RebalanceInsert(currNode.Parent.Parent);
                            }
                            catch (NullReferenceException e) {
                                Console.WriteLine("No grandpa! null it is!");
                                RebalanceInsert(null);
                            }

                        }
                        else if (comp == -1)
                        {
                            //I'm a right child! My left child is red! 
                            //rightleft() is needed!

                            /*
                             * Ok, so what is rightleft()?
                             * It's not actually a method in our case, 
                             * but is a concept.
                             * In order to resolve the following scenario, 
                             * two consecutive rotations are needed.
                             * 
                             *       a                 a                    b
                             *        \                 \                  / \
                             *         C      -->        B        -->     A   C
                             *        /                   \
                             *       B                     C
                             *         
                             * This is obviously a very basic form, 
                             * as I have not included any
                             * theoretical children, but those are not 
                             * necessarry for understanding this scenario.
                             * In order to handle this situation, 
                             * we must preform a Left() rotation from B,
                             * then a Right() rotation from B.
                             * 
                             * Effectively, the goal of the first rotation is 
                             * to move the nodes so that they 
                             * match the right rotation scenario.
                             * By placing B above C, we create a 
                             * Right() rotation scenario, 
                             * which we know how to handle.
                             * So, by performing a Left() rotation from B,
                             * we will create that scenario.
                             * 
                             * Since the Left() and Right() rotations are 
                             * already set up to handle any children 
                             * not shown in the diagram, 
                             * we do not need to consider them.
                             *
                             * Note that we must save B in 
                             * a temporary variable to ensure we can
                             * perform two rotations on it 
                             * without losing track of it. 
                             * Note that we first call Left(B), then Right(B).        
                             */
                            currNode = currNode.LeftChild;
                            Left(currNode);
                            Print();
                            Right(currNode);
                            Print();
                            try
                            {
                                RebalanceInsert(currNode.Parent.Parent.Parent);
                            }
                            catch (NullReferenceException e) {
                                Console.WriteLine("No grandpa! null it is!");
                                RebalanceInsert(null);
                            }
                        }
                        else
                        {
                            //I'm a left child! My left child is red! 
                            //left() is needed!
                            Left(currNode);
                            Print();
                            try
                            {
                                /* If there exists a great grandpa, 
                                 * we rebalance from there.
                                 * If not, we have reached the top of the tree, 
                                 * so we call RebalanceInsert on null.
                                 * Why a great grandpa and not just a grandpa?
                                 * In this scenario, the parent of currNode 
                                 * will end up black, meaning the only way
                                 * we would need to Rebalance would be if the 
                                 * grandpa and great grandpa were both red.
                                 */
                                RebalanceInsert(currNode.Parent.Parent.Parent);
                            }
                            catch (NullReferenceException e)
                            {
                                Console.WriteLine("No grandpa! null it is!");
                                RebalanceInsert(null);
                            }
                        }
                    }

                    //If my rightChild is red, I need to fix that!
                    else if (currNode.RightChild != null && 
                        currNode.RightChild.Color == NodeColor.RED)
                    {
                        Console.WriteLine("I, " + currNode.Data + 
                            ", see that my right child is red!");
                        if (promotion)
                        {
                            //My brother and child are red! 
                            //A promotion is necessary!
                            Promotion(currNode);
                            try
                            {
                                //If there exists a grandpa, 
                                //we rebalance from there.
                                //If not, we have reached the top of the tree, 
                                //so we call RebalanceInsert on null.
                                RebalanceInsert(currNode.Parent.Parent);
                            }
                            catch (NullReferenceException e) {
                                Console.WriteLine("No grandpa! null it is!");
                                RebalanceInsert(null);
                            }
                        }
                        else if (comp == -1)
                        {
                            //I'm a right child! My right child is red! 
                            //right() is needed!
                            Console.WriteLine("Right rotation!");
                            Right(currNode);
                            Print();
                            try
                            {
                                /* If there exists a great grandpa, 
                                 * we rebalance from there.
                                 * If not, we have reached the top of the tree, 
                                 * so we call RebalanceInsert on null.
                                 * Why a great grandpa and not just a grandpa?
                                 * In this scenario, the parent of currNode 
                                 * will end up black, meaning the only way
                                 * we would need to Rebalance would be if the 
                                 * grandpa and great grandpa were both red.
                                 */
                                RebalanceInsert(currNode.Parent.Parent.Parent);
                            }
                            catch (NullReferenceException e) {
                                Console.WriteLine("No grandpa! null it is!");
                                RebalanceInsert(null);
                            }
                        }
                        else
                        {
                            /* I'm a left child! My right child is red! 
                             * leftright() is needed!
                             * This is simply a mirror of 
                             * the above rightleft() scenario.
                             * Check that to understand 
                             * why we rotate in this way.
                            */
                            currNode = currNode.RightChild;
                            Right(currNode);
                            Print();
                            Left(currNode);
                            Print();
                            try
                            {
                                /* If there exists a great grandpa, 
                                 * we rebalance from there.
                                 * If not, we have reached the top of the tree,
                                 *  so we call RebalanceInsert on null.
                                 * Why a great grandpa and not just a grandpa?
                                 * In this scenario, the parent of currNode 
                                 * will end up black, meaning the only way
                                 * we would need to Rebalance would be if the 
                                 * grandpa and great grandpa were both red.
                                 */
                                RebalanceInsert(currNode.Parent.Parent.Parent);
                            }
                            catch (NullReferenceException e) {
                                Console.WriteLine("No grandpa! null it is!");
                                RebalanceInsert(null);
                            }
                        }
                    }

                    //If I'm not red, nothing needs to change!
                    //We're still balanced!
                }
            }
        }

        /*
         * A right rotation occurs when we have 
         * two red nodes that are right children.
         * 
         * In this example, 
         * capital letters are RED nodes, 
         * lowercase are black nodes
         * 
         *         a                    c
         *          \                  / \
         *           C      -->       A   D
         *          / \                \
         *         b   D                b
         * Where D is the most recently added node
         * 
         * The key to understanding rotations is understanding that 
         * a rotation consists of exactly three things:
         * 1) Two nodes shift position
         * 2) Those same two nodes swap colors
         * 3) The child of one node becomes the child of the other
         * 
         * So, in this scenario, a and C change positions
         * In the right rotation, the grandparent of the added node becomes 
         * the left child of the parent of the added node
         * So, a becomes C's left child.
         * Since there two nodes have shifted positions, 
         * they will swap their colors.
         * Since we previously had a and C, we now have A and c
         * 
         * ***IMPORTANT NOTE ABOUT COLOR SWAP:***
         * When two nodes switch colors, they will NOT ALWAYS BE DIFFERENT
         * We will see this later when we get to the 
         * "rightleft" and "leftright" scenarios
         * As such, if two nodes are red and change positions, 
         * the two nodes will "swap colors" from red to red.
         * 
         * Since c now has A as it's left child, 
         * c's previous left child, b, must be appended to A
         * Since b must be greater than A, we append b as A's right child
         * 
         * ***IMPORTANT NOTE ABOUT CHILD SHIFT:***
         * Only one child of each moved node should change.
         * In this scenario, A would maintain any previous left children, 
         * and c would maintain any previous right children
         * 
         * So, now into the details, 
         * how do we, step by step, preform this operation?
         * 
         * Our method takes in the node "parentofAddition", 
         * which is the parent of the node that has been added
         * In our diagram, this is node C.
         * Both nodes C and a must change positions, 
         * so we must simultaneously have C take a's place, 
         * while also not losing track of C's left child
         * 
         * So, for this movement, we must:
         * Set a's right child to b
         * Set b's parent to a
         * Set C's left child to a
         * Set C's parent to a's parent
         * Set a's parent to C
         * Swap the colors of a and C
         * 
         * This order may seem a bit odd, 
         * but it ensures we don't accidentally lose any references mid swap
         * 
         */
        private void Right(RBTNode<TData> parentOfAddition)
        {

            //Remember, parentOfAddition is node C in this scenario
            if (parentOfAddition.Parent == root)
            {
                Console.WriteLine("Beginning Right shift of " 
                    + parentOfAddition.Data);
                //set a's right child to b
                parentOfAddition.Parent.RightChild = parentOfAddition.LeftChild;        
                if (parentOfAddition.LeftChild != null)
                {
                    //set b's parent to a
                    parentOfAddition.LeftChild.Parent = parentOfAddition.Parent;            
                }
                //set C's left child to a
                parentOfAddition.LeftChild = parentOfAddition.Parent;                   
                parentOfAddition.Parent = null;
                root = parentOfAddition;
                //set a's parent to C 
                //**Note: at this point, a is the leftChild of our node**
                parentOfAddition.LeftChild.Parent = parentOfAddition;                   
            }
            else
            {
                Console.WriteLine("Beginning Right shift of " 
                    + parentOfAddition.Data);
                //set a's right child to b
                parentOfAddition.Parent.RightChild = parentOfAddition.LeftChild;        
                if (parentOfAddition.LeftChild != null)
                {
                    //set b's parent to a
                    parentOfAddition.LeftChild.Parent = parentOfAddition.Parent;            
                }
                //set C's left child to a
                parentOfAddition.LeftChild = parentOfAddition.Parent;
                //set C's parent to a's parent                   
                parentOfAddition.Parent = parentOfAddition.Parent.Parent;
                //set a's parent to C 
                //**Note: at this point, a is the leftChild of our node**               
                parentOfAddition.LeftChild.Parent = parentOfAddition;                   


                //This just checks to see if a was a left or right child,
                //so that a's parent has the appropriate child set to c
                //-1 if right child, 1 if left child

                if (parentOfAddition.Parent.Data
                    .CompareTo(parentOfAddition.Data) == -1)
                {
                    parentOfAddition.Parent.RightChild = parentOfAddition;
                }
                else
                {
                    parentOfAddition.Parent.LeftChild = parentOfAddition;
                }
            }

            //Checks that the nodes are of different colors. 
            //If they are, swap the color of each.
            if (parentOfAddition.Color != parentOfAddition.LeftChild.Color)
            {
                NodeColor temp = parentOfAddition.Color;
                //swaps the color of C
                parentOfAddition.Color = parentOfAddition.LeftChild.Color;
                //swaps the color of a                    
                parentOfAddition.LeftChild.Color = temp;          
            }

        }

        /*
         * The Left rotation scenario is simply the inverse of the Right rotation
         * As such, not much explanation beyond the previous is necessary, 
         * only that a few rights and lefts will swap.
         * However, for the sake of absolute clarity, 
         * let's go through it anyway.
         * 
         * In this example, capital letters are RED nodes, 
         * lowercase are black nodes
         *       d                b
         *      /                / \
         *     B      -->       A   D
         *    / \                  /
         *   A   c                c
         * 
         * Since we've already discussed the importance of color swaps, 
         * and what a rotation consists of,
         * We're just going to cover the exact manner 
         * in which we will perform this shift
         * 
         * Set d's left child to c
         * Set c's parent to d
         * Set B's right child to d
         * Set d's parent to b
         */
        private void Left(RBTNode<TData> parentOfAddition)
        {

            //Remember, parentOfAddition is node C in this scenario
            if (parentOfAddition.Parent == root)
            {
                Console.WriteLine("Beginning Left shift of " 
                    + parentOfAddition.Data);
                //set d's left child to c
                parentOfAddition.Parent.LeftChild 
                    = parentOfAddition.RightChild;        
                if (parentOfAddition.RightChild != null)
                {
                    //set c's parent to d
                    parentOfAddition.RightChild.Parent 
                        = parentOfAddition.Parent;       
                }
                //set B's right child to d
                parentOfAddition.RightChild = parentOfAddition.Parent;
                //set the new root's parent to null
                parentOfAddition.Parent = null;
                //set the actual root of the tree to b
                root = parentOfAddition;
                //set d's parent to b 
                //**Note: at this point, a is the leftChild of our node**                                
                parentOfAddition.RightChild.Parent = parentOfAddition;                  
            }
            else
            {
                Console.WriteLine("Beginning Left shift of " 
                    + parentOfAddition.Data);
                //set d's left child to c
                parentOfAddition.Parent.LeftChild = parentOfAddition.RightChild;        
                if (parentOfAddition.RightChild != null)
                {
                    //set c's parent to d
                    parentOfAddition.RightChild.Parent = parentOfAddition.Parent;       
                }
                //set B's right child to d
                parentOfAddition.RightChild = parentOfAddition.Parent;
                //set B's parent to d's previous parent                   
                parentOfAddition.Parent = parentOfAddition.Parent.Parent;
                //set d's parent to b 
                //**Note: at this point, a is the leftChild of our node**                
                parentOfAddition.RightChild.Parent = parentOfAddition;                   

                //This just checks to see if d was a left or right child,
                //so that d's parent has the appropriate child set to B
                //-1 if right child, 1 if left child
                if (parentOfAddition.Parent.Data
                    .CompareTo(parentOfAddition.Data) == -1)
                {
                    parentOfAddition.Parent.RightChild = parentOfAddition;
                }
                else
                {
                    parentOfAddition.Parent.LeftChild = parentOfAddition;
                }
            }
            //Checks that the nodes are of different colors. 
            //If they are, changes the color of each.
            if (parentOfAddition.Color != parentOfAddition.RightChild.Color)
            {
                NodeColor temp = parentOfAddition.Color;
                //swaps the color of C
                parentOfAddition.Color = parentOfAddition.RightChild.Color;
                //swaps the color of a           
                parentOfAddition.RightChild.Color = temp;          
            }

        }

        public void Promotion(RBTNode<TData> parentOfAddition)
        {
            int comp = 
                parentOfAddition.Parent.Data.CompareTo(parentOfAddition.Data);

            Console.WriteLine("Promotion from " + parentOfAddition.Data);

            parentOfAddition.Parent.Color = NodeColor.RED;
            parentOfAddition.Color = NodeColor.BLACK;
            if (comp == -1)
            {
                parentOfAddition.Parent.LeftChild.Color = NodeColor.BLACK;
            }
            else
            {
                parentOfAddition.Parent.RightChild.Color = NodeColor.BLACK;
            }

        }


        /// <summary>
        /// Removes a node from the tree, then calls appropriate rebalance.
        /// </summary>
        /// <param name="nodeToRemove">Node to be temoved from tree</param>
        /// <param name="currNode">Current node in traversal</param>
        /// <returns></returns>
        public bool Remove(RBTNode<TData> nodeToRemove,
            RBTNode<TData> currNode = null,
            bool lastCall = false)
        {

            if (currNode == null)
            {
                currNode = root;
            }
            if (nodeToRemove.Data.CompareTo(currNode.Data) == -1)
            {
                if (currNode.LeftChild == null)
                {
                    //If this occurs, the node does not exist.
                    return false;
                }
                else
                {
                    //If this occurs, we must continue searching.
                    return Remove(nodeToRemove, currNode.LeftChild);
                }
            }
            else if (nodeToRemove.Data.CompareTo(currNode.Data) == 1)
            {
                if (currNode.RightChild == null)
                {
                    //If this occurs, the node does not exist.
                    return false;
                }
                else
                {
                    //If this occurs, we must continue searching.
                    return Remove(nodeToRemove, currNode.RightChild);
                }
            }
            else
            {
                /*This is where it gets tricky. 
                 * We've found the node, so now we must properly remove it.
                 * There are exactly five cases we must handle: 
                 * root, no children, only a right child, 
                 * only a left child, or two children.
                 * We will cover how to handle each of those as we reach them.
                 */

                //If the root is the node we're removing, 
                //there is no parent, so this case is special.
                if (currNode == root)
                {
                    //If the root has no children, simply delete the root.
                    if (currNode.LeftChild == null 
                        && currNode.RightChild == null)
                    {
                        Console.WriteLine("No children!");
                        root = null;
                        return true;
                    }
                    //If the root has only a left child, 
                    //set that left child to the root.
                    //Then, clean up the previous root 
                    //by removing all of its references
                    else if (currNode.LeftChild != null 
                        && currNode.RightChild == null)
                    {
                        Console.WriteLine("Only a left child!");
                        root = currNode.LeftChild;
                        currNode.LeftChild = null;
                        root.Color = NodeColor.BLACK;
                        return true;
                    }
                    //If the root has only a right child, 
                    //set that right child to the root
                    //then, clean up the previous root by 
                    //removing all of its referenecs
                    else if (currNode.LeftChild == null 
                        && currNode.RightChild != null)
                    {
                        Console.WriteLine("Only a right child!");
                        root = currNode.RightChild;
                        currNode.RightChild = null;
                        root.Color = NodeColor.BLACK;
                        return true;
                    }
                    /*Finally, we hit the case of two children. In this case, there are a few things we must do.
                     *           A
                     *         /   \
                     *        B     C
                     *       / \   / \
                     *      D   E F   G
                     *              
                     * Alright, so  A is our root. If we want to remove A, 
                     * we want to replace A with it's lowest right child, F.
                     * This isn't very hard, as it just means we will need to 
                     * traverse the right tree, and effectively "remove" F.
                     * How do we find F? We simply move right from A, 
                     * then move left as many times as possible.
                     * Well, we know for sure that C and G are greater than F, 
                     * and that B, D, and E are less than C,
                     * so no nodes need to be moved other than F, 
                     * and possibly F's right child (if it has one).
                     * 
                     * So, what is our process?
                     * 
                     * Set our current node to C (the right child of A).
                     * Find the leftmost child of C 
                     * (which could very well be C)(in this example, this is F).
                     * If F has a right child (we'll call it T), 
                     * set that right child to F's old position
                     * This consists of the following steps:
                     *  Set T's parent = F's parent
                     *  Set F's parent's left child = T
                     * Moving back to the main removal, 
                     * Simply copy F's data to A.
                     * This will maintain all pointers and children and such, 
                     * but effectively "copy" F to A's position.
                     * Clean up old F by removing all of its references.
                     * This results in the following tree:
                     * 
                     *           F
                     *         /   \
                     *        B     C
                     *       / \     \
                     *      D   E     G
                     *   
                     *   IMPORTANT NOTE:
                     *   If F has any right children, 
                     *   they would be the left child of C.
                    */
                    else
                    {
                        Console.WriteLine("two children! Getting min");
                        RBTNode<TData> F = MinNode(currNode.RightChild);
                        Console.WriteLine("Node to actually remove: " + F.Data);
                        if (F.RightChild != null)
                        {
                            F.RightChild.Parent = F.Parent;
                        }

                        root.Data = F.Data;

                        if (F.Parent != root)
                        {
                            F.Parent.LeftChild = F.RightChild;
                            if (F.RightChild != null)
                            {
                                F.RightChild.Parent = F.Parent;
                            }
                        }
                        RBTRemove(F);

                        F.Free();
                        return true;
                    }
                }
                else
                {
                    //First, we compare the node to its parent. 
                    //This way, we know if the nodeToRemove 
                    //is a left or right child.
                    // 1 = left child, -1 = right child
                    int comp = currNode.Parent.Data.CompareTo(currNode.Data);

                    /*Case 1: No children
                    * For the sake of code simplicity,
                    * this section is going to be slightly inefficient.
                    * What do we do when we want 
                    * to remove a node with no children?
                    * We simply remove the parent's connection to it.
                    * This will send the node to garbage collection.
                    * From the point-of-view of the tree, however, 
                    * the node will no longer exist,
                    * having been replaced by null.
                   */
                    if (currNode.LeftChild == null && currNode.RightChild == null)
                    {
                        if (!lastCall)
                        {
                            return RBTRemove(currNode);
                        }
                        else
                        {
                            Console.WriteLine("No kids!");
                            if (comp == 1)
                            {
                                currNode.Parent.LeftChild = null;
                                currNode.Parent = null;
                            }
                            else
                            {

                                currNode.Parent.RightChild = null;
                                currNode.Parent = null;
                            }
                            return true;
                        }
                    }

                    /*Case 2: One child
                     * How do we remove a Node with only one child?
                     * 
                     * If that node itself is a leftChild:
                     * Here's a Visualization: 
                     *     A        A       A       A
                     *    /        /       /       /
                     *   B  --->  C   OR  B  ---> C
                     *  /                  \
                     * C                    C
                     * Where B is the NodeToRemove.
                     * 1st, We set A's leftChild to C
                     * 2nd, We set C's Parent to A
                     * Finally, we clean up B by removing all its references.
                     * 
                     * If that node itself is a rightChild:
                     * Here's a Visualization: 
                     * A        A           A       A
                     *  \        \           \       \
                     *   B  --->  C    OR     B --->  C  
                     *  /                      \
                     * C                        C
                     * Where B is the NodeToRemove.
                     * 1st, We set A's rightChild to C
                     * 2nd, We set C's Parent to A
                     * Finally, we clean up B by removing all its references.
                     * 
                     * Notice that all steps other than the 1st are the same. 
                     * As a result our conditional only changes 
                     * which child of the Parent we change.
                     * 
                     * The only difference between having a left or 
                     * right child is which reference you change.
                     * Given a left child, 
                     * we want to set the parent's child to the left child.
                     * Given a right child, 
                     * we want to set the parent's child to the right child.
                     */

                    //Checks that the Node has only a left child
                    else if (currNode.LeftChild != null 
                        && currNode.RightChild == null)
                    {
                        RBTNode<TData> maxNode = MaxNode(currNode.LeftChild);
                        Console.WriteLine("currNode: " + currNode.Data);
                        Console.WriteLine("Node to actually remove: " 
                            + maxNode.Data);
                        if (currNode.Data.CompareTo(maxNode.Data) == 0)
                        {
                            Console.WriteLine("Now deleting " + currNode.Data);
                            Console.WriteLine("Only a left child!");
                            if (comp == 1)
                            {
                                currNode.Parent.LeftChild = currNode.LeftChild;
                            }
                            else
                            {
                                currNode.Parent.RightChild = currNode.LeftChild;
                            }

                            //This happens regardless of what 
                            //type of child our nodeToRemove is.
                            currNode.LeftChild.Parent = currNode.Parent;
                            currNode.Free();

                            //return true, since we successfully added a node.
                            return true;
                        }
                        else
                        {
                            currNode.Data = maxNode.Data;
                            Console.WriteLine("Only a left child! I'm not " 
                                + maxNode.Data);
                            return RBTRemove(maxNode);
                        }
                    }

                    //Checks that the Node has only a right child
                    else if (currNode.LeftChild == null && 
                        currNode.RightChild != null)
                    {
                        Console.WriteLine("Only a right child!");
                        RBTNode<TData> minNode = MinNode(currNode.RightChild);
                        if (currNode.Data.CompareTo(minNode.Data) == 0)
                        {
                            if (comp == 1)
                            {
                                currNode.Parent.LeftChild = currNode.RightChild;
                            }
                            else
                            {
                                currNode.Parent.RightChild = currNode.RightChild;
                            }

                            //This happens regardless of 
                            //what type of child our nodeToRemove is.
                            currNode.RightChild.Parent = currNode.Parent;
                            currNode.Parent = null;
                            currNode.LeftChild = null;

                            //return true, since we successfully added a node.
                            return true;
                        }
                        else
                        {
                            currNode.Data = minNode.Data;
                            Console.WriteLine("Only a right child!");
                            return RBTRemove(minNode);
                        }
                    }

                    //Final case: Two children. 
                    //This scenario has been covered in the root scenario
                    //Since we simply copy the data, 
                    //we do not actually have to change any pointers,
                    //so this becomes very simple.
                    else
                    {
                        Console.WriteLine("Two kids! Getting min");
                        RBTNode<TData> F = MinNode(currNode.RightChild);
                        Console.WriteLine("Node to actually remove: " + F.Data);
                        if (F.RightChild != null)
                        {
                            F.RightChild.Parent = F.Parent;
                        }

                        F.Parent.RightChild = F.RightChild;
                        F.Parent.LeftChild = null;
                        currNode.Data = F.Data;
                        F.Free();
                        return true;
                    }

                }
                //This should never happen
                Console.WriteLine("I'm equal! But I have not been removed.");   
                return false;
            }

        }

        public bool RBTRemove(RBTNode<TData> v)
        {
            /*
             * Removal is a bit confusing. 
             * Unlike insertion, which depends on 
             * the uncle of the inserted node, 
             * removal depends on the sibling of the node to be removed.
             * The following notation will be used the removal process:
             * 
             /*          p
             *         / \
             *        v   s
             *       /     \
             *      u       r
             * 
             * Where v is the node to remove,
             * u is the child of the (left or right) of v,
             * p is the parent of u,
             * s is the child of p this is not v,
             * r is the red child of s 
             * (right or left, or right in the case of both).
             * 
             * Removal has many, many cases based on the color of these nodes.
             * The important thing to keep in mind is 
             * that the node to be removed will always have either 
             * one child or none, based on standard BST removal.
             */

            Tuple<RBTNode<TData>, RBTNode<TData>, RBTNode<TData>, RBTNode<TData>>
                relativeNodes = FitToModel(v);

            RBTNode<TData> u = relativeNodes.Item1;
            RBTNode<TData> p = relativeNodes.Item2;
            RBTNode<TData> s = relativeNodes.Item3;
            RBTNode<TData> r = relativeNodes.Item4;



            /*
             * Alright, now that we've got our variables set, 
             * it's time to go over cases.
             * Since there are so many, 
             * we're going to explain them as we cover them,
             * as opposed to looking through them all now. 
             */

            /*
            * Simple Case: either u or v is red.
            * In this case, we can simply mark the replaced child as black.
            * Since only one of them can have been red, 
            * this will maintain black height.
            * So, we simply remove v as normal, then recolor u to be black.
            */
            if (u != null && u.Color == NodeColor.RED ||
                v != null && v.Color == NodeColor.RED)
            {
                Remove(v, v, true);

                if (u != null)
                {
                    u.Color = NodeColor.RED;
                }
                return true;
            }
            else
            {
                /*
                * Second Case: both u and v are black.
                * Here, we begin our encounter with double black nodes.
                * The double black color is a tool 
                * used to help us balance after removal.
                * 
                * The only real rule to remember with double black nodes, 
                * is that they cannot exist,
                * so we create them simply to remove them.
                * 
                * When u and v are black, removing v will color u double black
                * u will be null, however, 
                * so we can just move to rebalance 
                * without really considering it.
                * Rebalance will call itself with non-null nodes
                * if necessary internally.
                * 
                */
                Remove(v, v, true);
                //Begin rebalancing
                RebalanceDoubleBlack(u, p, s, r);
            }


            return true;
            //if (v == root) { root = null; }
        }

        private bool RebalanceDoubleBlack(RBTNode<TData> u,
            RBTNode<TData> p,
            RBTNode<TData> s,
            RBTNode<TData> r)
        {
            //We have essentially every other scenario inside of this one, 
            //so this will be long.

            /*
             * Case 3: Do the following when 
             * the current node u is double black,
             * or is not root
             */

            int sComp=0;
            int rComp=0;
            if (s != null)
            {
                sComp = s.Data.CompareTo(p.Data);
                Console.WriteLine(
                    sComp == 1 ? "s is right child" : "s is left child");
            }
            if (r != null)
            {
                rComp = r.Data.CompareTo(s.Data);
                Console.WriteLine(
                    rComp == 1 ? "r is right child" : "r is left child");
            }
            /*
             * Case 3a: If sibling s is black
             * and at least one of sibling's children r is red
             * 
             * This case is much easier to deal with than most,
             * as it is very similar to the addition restructuring.
             * We have a Right, left, rightleft, and leftright case in 3a.
             * However, in removal we focus on the orientation of s and r.
             * Left Left = s and r are both left children
             * Right Right = s and r are both right children
             * Left Right = s is a left child, r is a right child
             * Right Left = s is a right child, r is a left child
             */


            if ((s == null || s.Color == NodeColor.BLACK) && r != null)
            {
                if (sComp == 1 && rComp == 1)
                {
                    //Right Right
                    //If s and r are both right children,
                    //we preform a right rotation.
                    //Check the Right() documentation for details
                    Console.WriteLine("3a Right Right");
                    Right(s);
                }
                else if (sComp == -1 && rComp == -1)
                {
                    //Left Left
                    //If s and r are both left children,
                    //we preform a left rotation.
                    //Check the Right() documentation for details
                    Console.WriteLine("3a Left Left");
                    Left(s);
                }
                else if (sComp == 1 && rComp == -1)
                {
                    /*Right Left
                     *If s is a right child
                     *and r is a left child
                     *We preform a "rightleft" rotation
                     *(This actually consists of one call to right, one to left)
                     *We call in the order of Left() then Right()
                     *since we start from the bottom and work our way up.
                     *Check the Right() and Left() documentations for details*/
                    Console.WriteLine("3a Right Left");
                    Left(r);
                    Right(r);
                }
                else if (sComp == -1 && rComp == 1)
                {
                    /*Left Right
                     *If s is a left child
                     *and r is a right child
                     *We preform a "leftright" rotation
                     *(This actually consists of one call to left, one to right)
                     *We call in the order of Right() then Left()
                     *since we start from the bottom and work our way up.
                     *Check the Right() and Left() documentations for details */
                     Console.WriteLine("3a Left Right");
                    Right(r);
                    Left(r);
                }
                else
                {
                    Console.WriteLine("This shouldn't ever happen!");
                }

                //This happens in all cases
                //It's a bit weird, but this ensures that the tree stays valid.
                p.Color = NodeColor.BLACK;
                s.Color = NodeColor.RED;
                r.Color = NodeColor.BLACK;
            }

            /*
             * Case 3b: If sibling s is black
             * and both of sibling's children are black
             * 
             * This case is relatively simple, but does involve somce recursion.
             * In this scenario, u, v, s, and r are all black.
             * p can also be black, but is not necesarily.
             * 
             * We don't have any actual representation of a double black node,
             * but in this case we must act as though we do, and continously
             * recurse up the tree until we can resolve it.
             * 
             * This is effectively what we're dealing with:
             *  
             *     p          p
             *    / \   ->   / \
             *   u   s      N   s
             * 
             * Where N is a double black null node.
             * The way that this is resolved is by pushing the double black
             * up the tree via recoloring until we can properly handle it.
             * 
             * When we cannot properly handle it,
             * the recoloring method consists of the following:
             * Set node N to Black
             * Set node p to Dbl Black
             * Set node s to Red
             * 
             * Every time we hit this scenario, we must perform this recoloring,
             * Then move up the tree to see if a new solution has become viable.
             * A new solution consists of anything that resolves a dbl black.
             * 
             * We can properly handle it when:
             * The parent of the double black node is red, or
             * The double black node is root.
             * 
             * When we can properly handle the dbl black:
             * 
             * If p is red, instead of recoloring do the following:
             * Set node N to black
             * Set node p to Black
             * set node s to Red
             * 
             * Is node N is root, instead of recoloring do the following:
             * Set node N to black
             * 
             */
            else if (s != null && s.Color == NodeColor.BLACK && r == null)
            {
                Console.WriteLine("Case 3b");
                //If it's red, color it black and we're done
                if (p.Color == NodeColor.RED)
                {
                    p.Color = NodeColor.BLACK;
                }
                //If it isn't red, color it double black and try again
                else
                {
                    p.Color = NodeColor.DBLBLACK;
                    //Get model using this node as the double black
                    Tuple<RBTNode<TData>, RBTNode<TData>, 
                        RBTNode<TData>, RBTNode<TData>>
                        relativeNodes = FitToModel(p);
                    RebalanceDoubleBlack(relativeNodes.Item1,
                        relativeNodes.Item2,
                        relativeNodes.Item3,
                        relativeNodes.Item4);

                }
                s.Color = NodeColor.RED;
            }
            /*
             * Case 3c: Sibling is red.
             * In this case, we perform a rotation to move the sibling up.
             * We then recolor the old sibling and parent.
             * 
             * 
             *    P           P             S             S
             *   / \         / \           / \    (3b)   / \
             *  V   s   ->  N   s    ->   p   R    ->   P   R
             *     / \         / \       / \           / \
             *    c   r       c   r     N   c         n   c
             * 
             * Where c is the left red child of s, 
             * N is a double black null leaf,
             * and n is a black null leaf
             *    
             * To achieve this, we will take the following steps:
             * 
             * First, remove node V as always. 
             * (This has already happened by this time)
             * Second, perform a rotation on s.
             *  If s is a left child, perform a left rotation.
             *  If s is a right child, perform a right rotation.
             * Finally, we have reshaped our scenario 
             * into a solvable case 3b,
             * so we simply call RebalanceDoubleBlack
             * again with an adjusted model.
             * 
             * Instead of calling the method with 
             * (u, p, s, r) as we normally would,
             * we will call it relative to N after we reshape to match 3b.
             * 
             * So, in this case we will manually remake our model, 
             * since altering FitToModel and such would require 
             * refactoring the entire architecture of the program.
             * 
             * In our new model, we want to rebalance based on N,
             * so p will be p (as p is the parent of N),
             * s will be c (as c is the sibling of n),
             * and r will be the appropriate child of c
             * 
             * However, since we are guaranteed to have a solvable 3b, 
             * we don't actually need to find r,
             * so we'll just call it null
             * 
             */

            else if (s != null && s.Color == NodeColor.RED)
            {
                Console.WriteLine("Case 3c");
                //V has already been removed, so go to step 2
                //Rotate c appropriately

                //If c is a right child...
                if (p.RightChild != null)
                {
                    Right(p.RightChild);

                    //Finally, recall with adjusted model
                    RebalanceDoubleBlack(u, p, p.RightChild, null);
                }
                //Otherwise, c is a left child...
                else
                {
                    Left(p.LeftChild);

                    //Finally, recall with adjusted model
                    RebalanceDoubleBlack(u, p, p.LeftChild, null);
                }

            }

            /*
             * Case 4: if root is double black, color it black
             * 
             * This works because every single node will have it's black height
             * decreased by 1, so the entire tree will still be valid.
             */
            else if (p == null)
            {
                root.Color = NodeColor.BLACK;
            }
            else
            {
                Console.WriteLine("Let's hope this never happens");
            }
            return true;
        }

        /// <summary>
        /// Gives a tuple in form (u, p, s, r) of nodes relative to given v
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        private Tuple<RBTNode<TData>, RBTNode<TData>, 
            RBTNode<TData>, RBTNode<TData>>
            FitToModel(RBTNode<TData> v)
        {
            RBTNode<TData> u;
            RBTNode<TData> p = v.Parent;
            RBTNode<TData> s;
            RBTNode<TData> r;

            /*
             * First off, we've got to correctly set each of these nodes.
             * Since v can only have 1 child, 
             * u can be either the left child, the right child, or null.
             * The following check will figure out which child u is, 
             * then set it to that.
             */
            if (v.LeftChild != null)
            {
                u = v.LeftChild;
                Console.WriteLine("u = " + u.Data);
            }
            else if (v.RightChild != null)
            {
                u = v.RightChild;
                Console.WriteLine("u = " + u.Data);
            }
            else
            {
                u = null;
                Console.WriteLine("u = null");
            }



            /*
             * Next up is s. In order to determine what child s is, 
             * we need to figure out which child v is.
             * We can do this with a simple data comparison.
             * 
             * **IMPORTANT NOTE**
             * If the parent is null, v is root. 
             * In this case, we cannot make a comparison.
             * Therefore, we need an extra conditional 
             */
            if (p != null)
            {
                //vCompP will be -1 if v is a left child, 
                //or 1 if v is a right child
                int vCompP = v.Data.CompareTo(p.Data);


                if (vCompP == 1)
                {
                    //If v is a right child...
                    //v's sibling must be the left child!
                    s = p.LeftChild;
                }
                else
                {
                    //otherwise, v's sibling is the right child!
                    s = p.RightChild;
                }
                Console.WriteLine("s = " + s.Data);

                /*
                 * Now, when we go to find r, 
                 * we need to keep in mind that r is the RED child of s.
                 * Also, we need to keep in mind that 
                 * in the case of both children being red, right gets preference
                 * As such, we first check if the right child exists and is red,
                 * and otherwise we check if the left child exists and is red.
                 * Or, of course, there can be no red children, 
                 * in which case r is null;
                 */

                if (s.RightChild != null && 
                    s.RightChild.Color == NodeColor.RED)
                {
                    r = s.RightChild;
                    Console.WriteLine("r = " + r.Data);
                }
                else if (s.LeftChild != null && 
                    s.LeftChild.Color == NodeColor.RED)
                {
                    r = s.LeftChild;
                    Console.WriteLine("r = " + r.Data);
                }
                else
                {
                    r = null;
                    Console.WriteLine("r = null");
                }

            }
            else
            {
                s = null;
                r = null;
            }
            return new Tuple<RBTNode<TData>, RBTNode<TData>, 
                RBTNode<TData>, RBTNode<TData>>(
                u,
                p,
                s,
                r);
        }

        /// <summary>
        /// Finds the minimum node from a given node
        /// </summary>
        /// <param name="currNode">Node to find minimum from. 
        /// Should generally be a right child</param>
        /// <returns>RBTNode representing the 
        /// lowest child of given node</returns>
        private RBTNode<TData> MinNode(RBTNode<TData> currNode)
        {
            return (currNode.LeftChild != null) 
                ? (MinNode(currNode.LeftChild)) : (currNode);
        }

        /// <summary>
        /// Finds the maximum node from a given node
        /// </summary>
        /// <param name="currNode">Node to find maximum from. 
        /// Should generally be a left child</param>
        /// <returns>RBTNode representing the 
        /// lowest child of given node</returns>
        private RBTNode<TData> MaxNode(RBTNode<TData> currNode)
        {
            return (currNode.RightChild != null) 
                ? (MaxNode(currNode.RightChild)) : (currNode);
        }

        public void Print()
        {
            Console.WriteLine(root);
        }
    }
}
