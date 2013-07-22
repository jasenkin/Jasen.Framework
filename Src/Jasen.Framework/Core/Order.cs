using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jasen.Framework.Resources;

namespace Jasen.Framework
{
    [Serializable]
    public class Order 
    { 
        public static readonly Order Empty = new Order();
        public string OrderString
        {
            get;
            internal set;
        }
         
        public string OrderColumn
        {
            get;
            private set;
        }
         
        public OrderDirection OrderDirection
        {
            get;
            private set;
        }

        public bool IsValid
        {
            get
            {
                return !string.IsNullOrEmpty(this.OrderString);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Order()
        {
        }

        public Order(string orderColumn, OrderDirection direction = OrderDirection.Ascend)
        {
            if (string.IsNullOrEmpty(orderColumn))
            {
                throw new ArgumentNullException(MsgResource.ColumnNameRequired);
            }

            this.OrderColumn = orderColumn;
            this.OrderDirection = direction;
            this.OrderString = orderColumn + " " + GetDirectionString(direction);
        }

        private static string GetDirectionString(OrderDirection direction)
        {
            switch (direction)
            {
                case OrderDirection.Ascend: 
                    return "ASC";
                case OrderDirection.Descend: 
                    return "DESC";
                default: 
                    return "ASC";
            }
        } 

        public static Order Descend(string orderColumn)
        {
            return CreateOrder(orderColumn, OrderDirection.Descend);
        }

        public static Order Ascend(string orderColumn)
        {
            return CreateOrder(orderColumn, OrderDirection.Ascend);
        }

        public static Order CreateOrder(string orderColumn, OrderDirection direction)
        {
            return new Order(orderColumn, direction); 
        }

        public static Order operator &(Order left, Order right)
        {
            if (left == null || right == null)
            {
                throw new ArgumentNullException(MsgResource.InvalidArguments);
            }

            return BitwiseAnd(left, right);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Order BitwiseAnd(Order left, Order right)
        {
            if (left == null || right==null)
            {
                throw new ArgumentNullException(MsgResource.InvalidArguments);
            }
            
            if (!HasOrder(right))
            {
                return left;
            }
            else if (!HasOrder(left))
            {
                return right;
            }
            else
            {
                left.OrderString = left.OrderString + "," + right.OrderString;
                return left;
            }
        }

        private static bool HasOrder(Order order)
        {
            return order != null && !string.IsNullOrEmpty(order.OrderString); 
        }

        public void Clear()
        {
            this.OrderString = null;
            this.OrderColumn = null;
        }

        public override string ToString()
        {
            return this.OrderString;
        }
    }
}
