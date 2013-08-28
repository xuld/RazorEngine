using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xuld.RazorEngine {

    public class HtmlString {
        private string _htmlString;

        [System.Runtime.TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        public HtmlString(string value) {
            _htmlString = value;
        }

        public override string ToString() {
            return _htmlString;
        }
    }


}
