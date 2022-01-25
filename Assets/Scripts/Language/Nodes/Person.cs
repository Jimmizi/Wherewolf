using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Language {
    public class Person : Noun {
        public override void Visit(Visitor visitor) {
            visitor.Visit(this);
        }
    }
}