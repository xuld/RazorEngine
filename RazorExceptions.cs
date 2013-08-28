using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Web.Razor.Parser.SyntaxTree;

namespace Xuld.RazorEngine {

    [Serializable]
    public class RazorException : Exception {
        public RazorException() {

        }
        public RazorException(string message) : base(message) {

        }
        public RazorException(string message, Exception inner) : base(message, inner) {

        }

        protected RazorException(SerializationInfo info, StreamingContext context)
            : base(info, context) {

        }
    }

    [Serializable]
    public class RazorParseException : RazorException {

        public IList<RazorError> Errors {
            get;
            private set;
        }

        public RazorParseException(IList<RazorError> errors)
            : base(errors[0].Message) {
                Errors = errors;
        }
    }

    [Serializable]
    public class RazorComplieException : RazorException {

        public CompilerErrorCollection Errors {
            get;
            private set;
        }

        public RazorComplieException(CompilerErrorCollection errors)
            : base(errors[0].ErrorText) {
            Errors = errors;
        }
    }

    [Serializable]
    public class RazorRuntimeException : RazorException {

        public RazorRuntimeException(Exception innerExecption)
            : base(innerExecption.Message, innerExecption) {
            
        }

        public RazorRuntimeException(string message)
            : base(message) {

        }
    }

}
