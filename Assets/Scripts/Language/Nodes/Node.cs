using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Language {
    public abstract class Node {

        public abstract void Visit(Visitor visit);
        
    }
}
