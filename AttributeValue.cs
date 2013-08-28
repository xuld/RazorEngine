using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xuld.RazorEngine {
    public class AttributeValue {
        public AttributeValue(Tuple<string, int> prefix, Tuple<object, int> value, bool literal) {
            this.Prefix = prefix;
            this.Value = value;
            this.Literal = literal;
        }

        public static AttributeValue FromTuple(Tuple<Tuple<string, int>, Tuple<object, int>, bool> value) {
            return new AttributeValue(value.Item1, value.Item2, value.Item3);
        }

        public static AttributeValue FromTuple(Tuple<Tuple<string, int>, Tuple<string, int>, bool> value) {
            return new AttributeValue(value.Item1, new Tuple<object, int>(value.Item2.Item1, value.Item2.Item2), value.Item3);
        }

        public static implicit operator AttributeValue(Tuple<Tuple<string, int>, Tuple<object, int>, bool> value) {
            return FromTuple(value);
        }

        public static implicit operator AttributeValue(Tuple<Tuple<string, int>, Tuple<string, int>, bool> value) {
            return FromTuple(value);
        }

        public bool Literal {
            get;
            private set;
        }

        public Tuple<string, int> Prefix {
            get;
            private set;
        }

        public Tuple<object, int> Value {
            get;
            private set;
        }
    }

}
