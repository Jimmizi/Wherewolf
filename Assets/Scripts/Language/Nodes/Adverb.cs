using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Language {
    public enum AdverbType {
        Slowly,
        Quickly,
        Silently,
        Loudly
    }
    
    public class Adverb : Node {
        public AdverbType Type {
            get;
            private set;
        }
        
        public Adverb(AdverbType type) {
            Type = type;
        }
        
        public override void Visit(Visitor visitor) {
            visitor.Visit(this);
        }
    }
}
