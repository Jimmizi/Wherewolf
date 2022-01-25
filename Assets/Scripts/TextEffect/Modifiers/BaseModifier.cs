using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseModifier : IModifier {
    protected float _startingTime;
    protected float _totalAnimationTime;
    protected bool _loop;
    
    protected float _progress;

    public float StartingTime {
        get => _startingTime;
        set => _startingTime = value;
    }

    public float TotalAnimationTime {
        get => _totalAnimationTime;
        set => _totalAnimationTime = value;
    }

    public bool Loop {
        get => _loop;
        set => _loop = value;
    }

    public float Progress {
        get { return _progress; }
    }
    
    public virtual void UpdateTime(float time) {
        if (time < _startingTime) {
            _progress = 0;
            return;
        }

        float currentProgress = (time - _startingTime) / _totalAnimationTime;

        if (!_loop) {
            currentProgress = Mathf.Clamp01(currentProgress);
        }

        _progress = currentProgress;
    }

    public virtual void Modify(CharacterData characterData) {
        
    }

    public bool IsActive {
        get {
            return (_loop || _progress > 1f);
        }
    }
}
