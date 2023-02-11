using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Priority_Queue;


namespace Utilities.Collections
{
    public class PriorityExecutionQueueNode : StablePriorityQueueNode<int>
    {
        public Task task;

    }

    /// <summary>
    /// LIFO for high priority, FIFO for low prio tasks.
    /// 
    /// Use an array for storing the queue/stack.
    /// </summary>
    public class PriorityExecutionQueue : StablePriorityQueue<PriorityExecutionQueueNode, int>
    {
        public PriorityExecutionQueue(int maxNodes)
            : base(maxNodes)
        {
        }

        /// <summary>
        /// Returns true if 'higher' has higher priority than 'lower', false otherwise.
        /// Note that calling HasHigherPriority(node, node) (ie. both arguments the same node) will return false.
        /// </summary>
        public override bool HasHigherPriority(PriorityExecutionQueueNode higher, PriorityExecutionQueueNode lower)
        {
            return (higher.Priority < lower.Priority ||
                (higher.Priority == lower.Priority && higher.InsertionIndex < lower.InsertionIndex));
        }


        /// <summary>
        /// Enqueue a node to the priority queue.  Lower values are placed in front. Ties are broken by first-in-first-out.
        /// If the queue is full, the result is undefined.
        /// If the node is already enqueued, the result is undefined.
        /// If node is or has been previously added to another queue, the result is undefined unless oldQueue.ResetNode(node) has been called
        /// O(log n)
        /// </summary>
        public new void Enqueue(PriorityExecutionQueueNode node, int priority)
        {
            int size = MaxSize;
            if (Count >= size - 1)
            {
                Resize(size << 1);
            }

            base.Enqueue(node, priority);
        }
    }
}
