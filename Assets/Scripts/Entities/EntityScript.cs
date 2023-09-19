using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityScript : MonoBehaviour
{
    [SerializeField]
    private Entity entity;

    public Entity Entity
    {
        get { return entity;        }
        set { this.entity = value;  }
    }

}
