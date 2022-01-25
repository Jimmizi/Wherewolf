using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Language {
    public class TMP_Emitter : Visitor {
        public override void Visit(Node node) {
            Debug.LogFormat("GENERIC: {0}", node);
        }

        public override void Visit(Verb verb) {
            Debug.LogFormat("VERB: {0}", verb.Type);
        }
    }
}