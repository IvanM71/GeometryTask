using UnityEngine;


public abstract class ZoneBehaviour : MonoBehaviour
{
    private bool _active = false;
    public bool Active {
        set {
            if (_active && value == false)
            {
                _active = false;
                OnDeactivation();
            }else if (!_active && value == true)
            {
                _active = true;
                OnActivation();
            }
        }
        get => _active;
    }
    

    protected virtual void OnActivation()
    {
        print("Behaviour activated");
    }
    
    protected virtual void OnDeactivation()
    {
        print("Behaviour deactivated");
    }
}
