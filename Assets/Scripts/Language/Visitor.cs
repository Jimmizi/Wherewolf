using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Language {
    public abstract class Visitor {
        /// <summary>
        /// Generic visit method.
        /// </summary>
        /// <param name="node"></param>
        public abstract void Visit(Node node);

        public virtual void Visit(Noun noun) {
            Visit((Node) noun);
        }
        
        public virtual void Visit(Verb verb) {
            Visit((Node) verb);
        }
        
        public virtual void Visit(Adjective adjective) {
            Visit((Node) adjective);
        }
        
        public virtual void Visit(Adverb adverb) {
            Visit((Node) adverb);
        }
    }
}