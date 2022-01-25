using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Language {
    public enum AdjectiveType {
        Small,
        Big,
        Slow,
        Fast,
        Dark,
        Bright,
        
        /* Colors */
        Red,
        Blue,
        Green
    }
    
    public class Adjective : Node {
        public AdjectiveType Type { get; private set; }
        
        public Adjective(AdjectiveType type) {
            Type = type;
        }
        
        public override void Visit(Visitor visitor) {
            visitor.Visit(this);
        }
    }
}
