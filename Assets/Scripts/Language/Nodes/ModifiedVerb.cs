using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Language {
    public class ModifiedVerb : Verb {
        public Adjective Adjective { get; private set;  }
        
        public ModifiedVerb(VerbType type) : base(type) {
        }

        public override void Visit(Visitor visitor) {
            visitor.Visit(Adjective);
            visitor.Visit(this);
        }
    }
}
