namespace Language {
    public enum VerbType {
        Be,
        Go,
        See,
        Talk,
        Kill
    }
    
    public class Verb : Node {

        public VerbType Type { get; private set; }
        
        public Verb(VerbType type) {
            Type = type;
        }
        
        public override void Visit(Visitor visitor) {
            visitor.Visit(this);
        }
    }
}
