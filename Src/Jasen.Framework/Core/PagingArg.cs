using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jasen.Framework
{
    [Serializable]
    public class PagingArg
    {
        private int _pageIndex;
        private int _pageSize;
        private int _pageCount;
        private int _rowCount;

        /// <summary>
        /// 
        /// </summary>
        public int PageIndex
        {
            get
            {
                return _pageIndex;
            }
            set
            {
                _pageIndex = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int PageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                _pageSize = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int PageCount
        {
            get
            {
                return _pageCount;
            }
            set
            {
                _pageCount = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int RowCount
        {
            get
            {
                return _rowCount;
            }
            set
            {
                _rowCount = value;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public PagingArg()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        public PagingArg(int pageIndex, int pageSize)
        {
            _pageIndex = pageIndex;
            _pageSize = pageSize;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="totalRowCount"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static int CalculatePageCount(int totalRowCount, int pageSize)
        {
            if (totalRowCount <= 0 || pageSize <= 0)
            {
                return 0;
            }

            decimal pages = decimal.Divide(totalRowCount, pageSize);

            return (int)Math.Ceiling(pages);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static int MaxRowNumber(int pageIndex, int pageSize)
        {
            if (pageSize <= 0)
            {
                return 0;
            }

            if (pageIndex < 0)
            {
                pageIndex = 0;
            }

            decimal rowNumber = (pageIndex + 1) * pageSize;

            return (int)rowNumber;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static int MinRowNumber(int pageIndex, int pageSize)
        {
            if (pageSize <= 0)
            {
                return 0;
            }

            if (pageIndex < 0)
            {
                pageIndex = 0;
            }

            decimal rowNumber = pageIndex * pageSize + 1;

            return (int)rowNumber;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "{PageIndex:" + this.PageIndex +
                ",PageSize:" + this.PageSize +
                ",PageCount:" + this.PageCount +
                ",RowCount:" + this.RowCount + "}";
        }
    }
}
