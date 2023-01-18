using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperGem : Gem
{
    public enum BoostType { vertical, horizontal }
    public SuperGem(Gem gem, BoostType boost)
    {
        this.type = gem.type;
        this.posIndex = gem.posIndex;
        this.board = gem.board;
        
        Boost = boost;
    }

    public BoostType Boost;
}
