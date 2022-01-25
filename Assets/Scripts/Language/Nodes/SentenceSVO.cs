using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Language {
    public class SentenceSVO : Sentence {
        public Noun Subject { get; private set; }
        public Verb Verb { get; private set; }
        public Noun Object { get; private set; }
        
        public SentenceSVO(Noun subject, Verb verb, Noun objekt) {
            Subject = subject;
            Verb = verb;
            Object = objekt;
        }
        
        public override void Visit(Visitor visitor) {
            visitor.Visit(Subject);
            visitor.Visit(Verb);
            visitor.Visit(Object);
        }
    }
}