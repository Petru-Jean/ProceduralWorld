using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityPlayer : Entity
{

    public EntityPlayer() : base(100, 100, 1, 0)
    {
            
    }

    public override float OnEntityFall(float distance)
    {
        float dmg = 0;

        if(distance >= 5.0f)
        {
            dmg = distance * 1.66f;
            Hp -= dmg;
        }

        return dmg;
    }

    public override int NextLevelExp()
    {
        if(Level <= 10) return Level * 3;
        if(Level <= 20) return Level * 5;
        if(Level <= 30) return Level * 8;

        return Level * 10;
    }

}
