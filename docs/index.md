This is my implementation of a Red-Black Tree in C# (created without any pseudo-code!).
It took a while, but I learned a lot, and I hope to teach you as well.
I'm going to cover all of the things you need to know to fully understand a Red-Black Tree,
from the intuition of self-balancing, to the various rotations and recolorings that handle the implementation.

## Self-Balancing Intuition

A Red-Black Tree is what is known as a self-balancing Binary Search Tree. So, what is a self-balancing Binary Search Tree?

Well, we already know what a Binary Search Tree is, right? A Binary Search Tree is a dta structure that holds data in a node-based, tree-like structure, where any given node has up to two children, with the left child being "less than" the parent, and the right child being "grater than" the parent. However, Binary Search Trees have an inherent flaw, known as imbalance.

##### What is Imbalance?
Imbalance is what happens when data is lopsided, or the order in which data is put into a tree leads to a poor structure. For example, imagine you loaded the entire alphabet, A-Z, into a Binary Search Tree in order. You first load in A, which becomes the root. You then load in B, which becomes A's right child. You then load in C, which become's B's right child. You then load in D, which becomes C's right child. You continue this process, and when you've inserted Z, you realize that your tree isn't a tree at all, it's basically just a linked list!

This is the problem of Imbalance. Although Binary Search Tree's boast an O(logn) search time, such a thing is never guaranteed, and is frequently not present. This is a result of skewed data, or just non-optimized inserts. So, optimally, we want our tree to be exactly equal, or at least really close, with any given parent having an equal number of children on either side. In this way, we could more realistically begin to guarantee the O(logn) search time.

So, how do we rectify this imbalance? How can we start to move towards that optimal balanced tree? Well, what if we had first inserted L into our tree? Then, at least the tree would have the same number of nodes on the left as on the right. Perhaps we could even more optimally insert the nodes, to get even closer to the best-case tree. This is just one simple improvement we could give to our tree. However, so much more can and has been done in the field of data structures and algorithms to combat this problem.

Many smart people came together and came up with several strategies for resloving this issue of imbalance. This resulted in many different "Self-Balancing" Binary Search Trees, which have logic built-in to reorder the nodes in the tree to acheive a more optimal arrangement for the sake of search times. To name a few:
* 2-3 tree
* AA tree
* AVL tree
* Red-black tree
* Scapegoat tree
* Splay tree
* Treap

Here I will attempt to describe the Red-Black Tree, and its strategies for handling such problems, via coloring, rotations, and recursion.

## Red-Black Tree (Basic Overview)
A Red-Black Tree aims to combat the problem of imbalance using two main tools: colors and rotations.

The difference between a node in a Binary Search Tree and a node in a Red-Black Tree is that a node in a Red-Black Tree has an associated color. This color is, as you might guess, either red or black.

The rules for colors are as follows:
1. Every node is either red or black
2. Every leaf (NULL) is black
3. If a node is red, then both its children are black
4. Every simple path from a node to a descendent leaf contains the same number of black nodes.

So what do those mean in English? Well, the first rule is pretty straightforward. Each node will have a color value, and that value will only ever be red or black.

The second rule is a bit odd. In a Red-Black Tree, every node is understood to have two children at all times. If a node has two actual children, that's great! If a node has anything less than that, its missing children will be filled in by black "NULL" nodes. So, although these nodes may not actually "exist" in the tree, they are understood to be there. The actual existence/nonexistence is an implementation detail. In my implementation, I chose not to have any literal existence of these nodes; Simply null values for children that were treated as black when balancing. It may seem odd that we have such a rule, but when we view the balancing strategies later on, this will make more sense.

The third rule is probably the most important in terms of insertion. Every red node is guaranteed to have two black children at all times. Since every node enters the tree red, this causes a rebalance every time a node is inserted below a red node. How this rebalance takes place will be covered later.

The fourth rule speaks towards a concept known as "Black Height". "Black Height" is essentially the number of Black Nodes from any given node to the bottom of the tree (any NULL leaf node). In order for the tree to be valid, "Black Height" must be equal for every path from a given node.  Different nodes may have different black heights, but a given node can have exactly one black height shared by every path from that node to any (NULL) leaf node.

All of these rules will seem much more reasonable in the next sections, when we see how they're used to make rebalancing decisions. 

## Insertion
## Deletion
