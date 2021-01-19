using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XAutomateMVC
{
    public class SuitePagination
    {
        public List<Suites> Suitelist { get; set; }

        ///<summary>
        /// Gets or sets CurrentPageIndex.
        ///</summary>
        public int CurrentPageIndex { get; set; }

        ///<summary>
        /// Gets or sets PageCount.
        ///</summary>
        public int PageCount { get; set; }
    }
    public class Suites
    {
        public string SuiteName { get; set; }
        public string ContactName { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
    }
}
