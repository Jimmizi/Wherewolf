using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Language {
    public class ModifiedNoun : Noun {
        public Adjective Adjective;
        public Noun Noun;

        public override void Visit(Visitor visitor) {
            visitor.Visit(Adjective);
            visitor.Visit(Noun);
        }
    }
}
