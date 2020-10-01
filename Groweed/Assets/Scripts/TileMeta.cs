using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMeta
{
    object metaData { get; set; }

    public TileMeta( object metaData )
    {
        this.metaData = metaData;
    }
}
