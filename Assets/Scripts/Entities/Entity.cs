using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Entity : MonoBehaviour
{
    private Inventory inventory;

    private float maxHp;
    private float hp;

    private int  level;
    private int  exp;
    
    public Entity(float maxHp, float hp, int level, int exp)
    {
        inventory = new Inventory();
        
        this.maxHp  = maxHp;
        this.hp     = hp;
        
        this.level  = level;
        this.exp    = exp;
            
    }

    public Inventory Inventory
    {
        get { return inventory; }
        private set { }
    }

    public float Hp
    {
        get { return hp; }
        
        set 
        {
            hp = Mathf.Clamp(value, 0, float.MaxValue);
        }

    }

    public float MaxHp
    {
        get { return maxHp; }

        set
        {
            maxHp = Mathf.Clamp(value, 1, float.MaxValue); 
        }
    }

    public int Level
    {
        get { return level ; }
        set { level = Mathf.Clamp(value, 0, int.MaxValue); }
    }

    public int Exp
    {
        get { return exp; }
        set
        {
            exp = value;

            while(exp >= NextLevelExp())
            {
                exp = NextLevelExp() - exp;
                level++;
            }

        }

    }
    
    public abstract float OnEntityFall(float distance);
    public abstract int   NextLevelExp();
    

}
