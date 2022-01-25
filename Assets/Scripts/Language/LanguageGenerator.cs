using UnityEngine;

namespace Language {
    public class LanguageGenerator : MonoBehaviour {
        public static LanguageGenerator Instance { get; private set; }

        protected void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
            } else {
                Instance = this;
            }
        }
        
        void Start() {
            Node root = new SentenceSVO(
                new Person(), 
                new Verb(VerbType.Go),
                new Place()
            );
            
            TMP_Emitter emitter = new TMP_Emitter();
            root.Visit(emitter);
        }

        void Update() {
        }
    }
}